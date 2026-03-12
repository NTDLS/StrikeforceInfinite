using Ae.Engine.Helpers;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ae.Engine.Rendering
{
    /// <summary>
    /// Provides utility methods for rendering operations, including color generation, rectangle calculations, and
    /// graphics adapter information retrieval.
    /// </summary>
    /// <remarks>This class contains static methods to assist with rendering tasks such as generating
    /// fire-associated colors, converting HSL values to RGB, calculating centered rectangles, and querying graphics
    /// adapter details. All methods are thread-safe and do not maintain internal state. Intended for use in graphics
    /// and rendering contexts where such utilities are required.</remarks>
    public static class AeRenderingUtility
    {
        /// <summary>
        /// Gets a random color that would be associated with fire.
        /// </summary>
        /// <returns></returns>
        static public Color4 GetRandomHotColor()
        {
            float hue = AeRandom.Between(0, 60);
            float saturation = (float)AeRandom.Between(0.8f, 1.0f);
            float lightness = (float)AeRandom.Between(0.5f, 1);
            return RGBFromHSL(hue, saturation, lightness);
        }

        /// <summary>
        /// RGB from HSL (hue, saturation, lightness).
        /// </summary>
        /// <param name="hue"></param>
        /// <param name="saturation"></param>
        /// <param name="lightness"></param>
        static public Color4 RGBFromHSL(float hue, float saturation, float lightness)
        {
            float c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
            float x = c * (1 - Math.Abs(hue / 60 % 2 - 1));
            float m = lightness - c / 2;

            float r, g, b;

            if (0 <= hue && hue < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (60 <= hue && hue < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (120 <= hue && hue < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (180 <= hue && hue < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (240 <= hue && hue < 300)
            {
                r = x; g = 0; b = c;
            }
            else
            {
                r = c; g = 0; b = x;
            }

            return new Color4(r + m, g + m, b + m, 1);
        }

        /// <summary>
        /// Calculates a rectangle centered within a larger area, with its size determined by a specified percentage of
        /// the larger area's dimensions.
        /// </summary>
        /// <remarks>If percentage is negative, the rectangle's coordinates are reversed, resulting in an
        /// inverted rectangle. This can be used to indicate a special or invalid region.</remarks>
        /// <param name="largerSize">The size of the larger area within which the rectangle will be centered.</param>
        /// <param name="percentage">The proportion of the larger area's width and height to use for the rectangle's size. Must be in the range
        /// [-1, 1].</param>
        /// <returns>A rectangle centered within the specified area, sized according to the given percentage. The rectangle's
        /// coordinates are returned as a RawRectangleF.</returns>
        /// <exception cref="ArgumentException">Thrown if percentage is less than -1 or greater than 1.</exception>
        public static RawRectangleF CalculateCenterCopyRectangle(Size2F largerSize, float percentage)
        {
            if (percentage < -1 || percentage > 1)
            {
                throw new ArgumentException("Percentage must be in the range [-1, 1].");
            }

            float centerX = largerSize.Width * 0.5f;
            float centerY = largerSize.Height * 0.5f;

            float smallerWidth = largerSize.Width * percentage;
            float smallerHeight = largerSize.Height * percentage;

            float left = centerX - smallerWidth * 0.5f;
            float top = centerY - smallerHeight * 0.5f;
            float right = left + smallerWidth;
            float bottom = top + smallerHeight;

            if (percentage >= 0)
            {
                return new RawRectangleF(left, top, right, bottom);
            }
            else
            {
                return new RawRectangleF(right, bottom, left, top);
            }
        }

        /// <summary>
        /// Retrieves descriptions and dedicated video memory information for all installed graphics adapters, excluding
        /// the Microsoft Basic Render Driver.
        /// </summary>
        /// <remarks>The returned information can be used for diagnostics or display purposes. Only
        /// adapters with dedicated video memory are included; software renderers are excluded.</remarks>
        /// <returns>A string containing the names and dedicated video memory (in megabytes) of each detected graphics adapter.
        /// The string will be empty if no adapters are found.</returns>
        public static string GetGraphicsAdaptersDescriptions()
        {
            var text = new StringBuilder();
            using (var factory = new Factory1())
            {
                foreach (var adapter in factory.Adapters)
                {
                    if (adapter.Description.Description != "Microsoft Basic Render Driver")
                    {
                        string adapterName = adapter.Description.Description;
                        var videoMemory = adapter.Description.DedicatedVideoMemory / 1024.0 / 1024.0;

                        text.AppendLine($"\"{adapterName}\" : Dedicated Video Memory {videoMemory:n2}MB");
                    }
                }
            }

            return text.ToString();
        }

        /// <summary>
        /// Retrieves a list of available graphics adapters on the system, excluding the Microsoft Basic Render Driver.
        /// </summary>
        /// <remarks>Each adapter in the returned list includes its device ID, description, and dedicated
        /// video memory in megabytes. The Microsoft Basic Render Driver is excluded as it does not represent a hardware
        /// graphics adapter.</remarks>
        /// <returns>A list of <see cref="AeGraphicsAdapter"/> instances representing the detected graphics adapters. The list
        /// will be empty if no suitable adapters are found.</returns>
        public static List<AeGraphicsAdapter> GetGraphicsAdapters()
        {
            var result = new List<AeGraphicsAdapter>();
            using (var factory = new Factory1())
            {
                foreach (var adapter in factory.Adapters)
                {
                    if (adapter.Description.Description != "Microsoft Basic Render Driver")
                    {
                        result.Add(new AeGraphicsAdapter(adapter.Description.DeviceId, adapter.Description.Description)
                        {
                            VideoMemoryMb = adapter.Description.DedicatedVideoMemory / 1024.0 / 1024.0
                        });
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves the refresh rate, in hertz, of the specified screen for the given device identifier.
        /// </summary>
        /// <remarks>This method searches for the display mode with the highest resolution for the
        /// specified screen and device, and returns its refresh rate. If no matching display mode is found, a default
        /// value is returned. The method ensures a minimum refresh rate of 30 hertz.</remarks>
        /// <param name="screen">The screen object representing the display whose refresh rate is to be determined.</param>
        /// <param name="deviceId">The unique identifier of the graphics device associated with the screen.</param>
        /// <returns>The refresh rate of the screen, in hertz. Returns 60 if the refresh rate cannot be determined. If the
        /// detected refresh rate is below 30, returns 30.</returns>
        public static float GetScreenRefreshRate(Screen screen, int deviceId)
        {
            using var factory = new Factory1();
            foreach (var adapter in factory.Adapters)
            {
                if (adapter.Description.DeviceId == deviceId)
                {
                    foreach (var output in adapter.Outputs)
                    {
                        if (output.Description.DeviceName.Equals(screen.DeviceName, StringComparison.OrdinalIgnoreCase))
                        {
                            var displayModes = output.GetDisplayModeList(Format.R8G8B8A8_UNorm, DisplayModeEnumerationFlags.Interlaced);

                            var nativeMode = displayModes.OrderByDescending(mode => mode.Width * mode.Height)
                                .ThenByDescending(o => o.RefreshRate.Numerator / o.RefreshRate.Denominator).FirstOrDefault();

                            var refreshRate = nativeMode.RefreshRate.Numerator / (float)nativeMode.RefreshRate.Denominator;

                            return refreshRate < 30f ? 30f : refreshRate;
                        }
                    }
                }
            }

            return 60; //A safe default, I would think.
        }
    }
}
