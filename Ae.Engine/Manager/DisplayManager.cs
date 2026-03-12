using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Ae.Engine.Manager
{
    /// <summary>
    /// Various metrics related to display.
    /// </summary>
    public class DisplayManager
    {
        private readonly AeEngine _engine;

        internal AeFrameCounter FrameCounter { get; private set; } = new();

        /// <summary>
        /// Computed quadrants of the screen based on the natural screen size. The key is the X,Y of the quadrant, and the value is the quadrant object which contains absolute bounds.
        /// </summary>
        public Dictionary<Point, AeQuadrant> Quadrants { get; private set; } = new();

        /// <summary>
        /// The X,Y of the top left of the render window. This is the corner of the total
        /// canvas which includes offscreen locations when not zoomed out. The local player
        /// will be centered in this window and the window will moved with the players movements.
        /// This can be thought of as the camera.
        /// </summary>
        public AeVector CameraPosition { get; set; } = new();

        /// <summary>
        /// Gets the control that serves as the drawing surface for rendering graphics or visual content.
        /// </summary>
        public Control DrawingSurface { get; private set; }

        /// <summary>
        /// Gets the screen associated with the current context.
        /// </summary>
        public Screen Screen { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the drawing surface currently has input focus.
        /// </summary>
        public bool IsDrawingSurfaceFocused { get; set; } = false;

        /// <summary>
        /// Provides a mechanism for the control that owns the drawing surface to notify the DisplayManager of focus changes.
        /// </summary>
        /// <param name="isFocused"></param>
        public void SetIsDrawingSurfaceFocused(bool isFocused) => IsDrawingSurfaceFocused = isFocused;

        //#if DEBUG
        //public float? ZoomOverride = 1.0f; Makes it easier to debug collisions.
        //#else
        /// <summary>
        /// Specifies an optional override for the zoom level used in rendering or calculations.
        /// </summary>
        /// <remarks>If set, this value replaces the default zoom behavior. Typically used for debugging
        /// or testing purposes. When null, the standard zoom logic applies.</remarks>
        public float? ZoomOverride = null;
        //#endif

        /// <summary>
        /// Calculates the scaling factor for frame rendering based on the player's current speed and throttle settings.
        /// </summary>
        /// <remarks>The scaling factor is influenced by both the player's velocity and throttle, allowing
        /// for dynamic adjustment of frame rendering to match gameplay speed. When the engine is initializing, the
        /// method returns a default scaling factor of 1.</remarks>
        /// <returns>A floating-point value representing the scaling factor for frame rendering. The value ranges from the base
        /// draw scale up to 1, depending on the player's movement and throttle.</returns>
        public float SpeedOrientedFrameScalingFactor()
        {
            if (_engine.IsInitializing)
            {
                return 1;
            }

            float weightedThrottlePercent = (
            (_engine.Player.Sprite.MovementVector.Magnitude() / _engine.Player.Sprite.Speed) * 0.8f //80% of zoom is standard velocity
             + (_engine.Player.Sprite.Throttle <= 1 ? 1 : _engine.Player.Sprite.Throttle / _engine.Player.Sprite.MaxThrottle) * 0.2f //20% of the zoom will be the "boost".
            ).Clamp(0, 1);

            return BaseDrawScale + ((1 - BaseDrawScale) * weightedThrottlePercent);
        }

        /// <summary>
        /// Gets the base scale factor used for drawing operations, adjusted according to the engine's overdraw
        /// settings.
        /// </summary>
        /// <remarks>This property reflects the current overdraw scale configuration and is used to
        /// determine the default scaling for rendering. Changing the overdraw settings in the engine will affect the
        /// value returned by this property.</remarks>
        public float BaseDrawScale => 100.0f / _engine.Settings.OverdrawScale / 100.0f;

        /// <summary>
        /// The number of extra pixels to draw beyond the NaturalScreenSize.
        /// </summary>
        public Size OverdrawSize { get; private set; }

        /// <summary>
        /// The total size of the rendering surface (no scaling).
        /// </summary>
        public Size TotalCanvasSize { get; private set; }

        /// <summary>
        /// Gets the length of the diagonal across the entire canvas.
        /// </summary>
        public float TotalCanvasDiagonal { get; private set; }


        /// <summary>
        /// Represents the center point of the canvas as a vector.
        /// </summary>
        public AeVector CenterCanvas;

        /// <summary>
        /// Gets the coordinates representing the center point of the current screen in world space.
        /// </summary>
        public AeVector CenterOfCurrentScreen => CameraPosition + CenterCanvas;

        /// <summary>
        /// The size of the screen with no scaling.
        /// </summary>
        public Size NaturalScreenSize { get; private set; }

        /// <summary>
        /// The bounds of the screen with no scaling.
        /// </summary>
        public RectangleF NaturalScreenBounds =>
            new(OverdrawSize.Width / 2.0f, OverdrawSize.Height / 2.0f, NaturalScreenSize.Width, NaturalScreenSize.Height);

        /// <summary>
        /// The total bounds of the drawing surface (canvas) natural + overdraw (with no scaling).
        /// </summary>
        public RectangleF TotalCanvasBounds => new RectangleF(0, 0, TotalCanvasSize.Width, TotalCanvasSize.Height);

        /// <summary>
        /// Translates the given screen position (pixel coordinates) into TotalCanvas coordinates.
        /// </summary>
        /// <param name="screenPosition"></param>
        /// <returns></returns>
        public AeVector TranslateScreenPosition(Point screenPosition)
        {
            var src = _engine.Display.GetCurrentScaledScreenBounds();

            // Map mouse pixel to TotalCanvas coordinate (inside src rectangle)
            var x = src.Left + (screenPosition.X * (src.Width / _engine.Display.NaturalScreenSize.Width));
            var y = src.Top + (screenPosition.Y * (src.Height / _engine.Display.NaturalScreenSize.Height));

            return new AeVector(x, y);
        }

        /// <summary>
        /// Calculates and returns the bounds of the screen area after applying the current scaling factor.
        /// </summary>
        /// <remarks>The scaling factor determines whether the screen area is zoomed in or out. A positive
        /// value scales the area down, while a negative value zooms in. The method centers the scaled area within the
        /// total canvas size.</remarks>
        /// <returns>A RectangleF representing the scaled screen bounds. The rectangle reflects the area centered within the
        /// total canvas size, adjusted according to the scaling factor.</returns>
        /// <exception cref="ArgumentException">Thrown if the scaling factor is outside the range [-1, 1].</exception>
        public RectangleF GetCurrentScaledScreenBounds()
        {
            var scale = SpeedOrientedFrameScalingFactor();

            if (scale < -1 || scale > 1)
            {
                throw new ArgumentException("Scale must be in the range [-1, 1].");
            }

            float centerX = TotalCanvasSize.Width * 0.5f;
            float centerY = TotalCanvasSize.Height * 0.5f;

            float smallerWidth = (float)(TotalCanvasSize.Width * scale);
            float smallerHeight = (float)(TotalCanvasSize.Height * scale);

            float left = centerX - smallerWidth * 0.5f;
            float top = centerY - smallerHeight * 0.5f;
            float right = smallerWidth;
            float bottom = smallerHeight;

            if (scale >= 0)
            {
                return new RectangleF(left, top, right, bottom);
            }
            else
            {
                //TODO: Zoom-in is untested.
                return new RectangleF(right, bottom, left, top);
            }
        }

        /// <summary>
        /// Generates a random location within the current visible screen bounds.
        /// </summary>
        /// <remarks>The returned location is guaranteed to be within the current screen bounds as
        /// determined by the scaling settings. This method is useful for scenarios where random placement within the
        /// visible area is required, such as spawning objects or simulating random user interactions.</remarks>
        /// <returns>An instance of AeVector representing a randomly selected point within the current scaled screen area.</returns>
        public AeVector RandomOnScreenLocation()
        {
            var currentScaledScreenBounds = GetCurrentScaledScreenBounds();

            return new AeVector(
                    AeRandom.Between((int)currentScaledScreenBounds.Left, (int)(currentScaledScreenBounds.Left + currentScaledScreenBounds.Width)),
                    AeRandom.Between((int)currentScaledScreenBounds.Top, (int)(currentScaledScreenBounds.Top + currentScaledScreenBounds.Height))
                );
        }

        /// <summary>
        /// Generates a random location outside the visible canvas area, at a specified minimum and maximum distance
        /// from the camera position.
        /// </summary>
        /// <remarks>Use this method to spawn objects or effects outside the visible area, such as for
        /// offscreen enemy spawning or visual transitions. The returned location will always be outside the current
        /// canvas bounds, but the exact position is randomized within the specified distance range.</remarks>
        /// <param name="minOffscreenDistance">The minimum distance, in pixels, from the camera position that the generated location will be placed
        /// offscreen. Must be non-negative.</param>
        /// <param name="maxOffscreenDistance">The maximum distance, in pixels, from the camera position that the generated location can be placed
        /// offscreen. Must be greater than or equal to minOffscreenDistance.</param>
        /// <returns>An AeVector representing a random offscreen location relative to the camera position.</returns>
        //TODO: Test and fix this.
        public AeVector RandomOffScreenLocation(int minOffscreenDistance = 100, int maxOffscreenDistance = 500)
        {
            if (AeRandom.FlipCoin())
            {
                if (AeRandom.FlipCoin())
                {
                    return new AeVector(
                        CameraPosition.X + -AeRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        CameraPosition.Y + AeRandom.Between(0, TotalCanvasSize.Height));
                }
                else
                {
                    return new AeVector(
                        CameraPosition.X + AeRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        CameraPosition.Y + AeRandom.Between(0, TotalCanvasSize.Height));
                }
            }
            else
            {
                if (AeRandom.FlipCoin())
                {
                    return new AeVector(
                        CameraPosition.X + TotalCanvasSize.Width + AeRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        CameraPosition.Y + AeRandom.Between(0, TotalCanvasSize.Height));
                }
                else
                {
                    return new AeVector(
                        CameraPosition.X + TotalCanvasSize.Width + AeRandom.Between(minOffscreenDistance, maxOffscreenDistance),
                        CameraPosition.Y + -AeRandom.Between(0, TotalCanvasSize.Height));
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the DisplayManager class, configuring the drawing surface and screen size for
        /// rendering operations.
        /// </summary>
        /// <remarks>If sizeOverride is provided, it will be used as the screen size for rendering, which
        /// may affect layout and scaling. The constructor calculates the total canvas size based on the engine's
        /// overdraw scale, ensuring even dimensions for optimal rendering. The associated screen is determined from the
        /// drawing surface handle.</remarks>
        /// <param name="engine">The engine instance used to provide rendering settings and context for display management.</param>
        /// <param name="drawingSurface">The control that serves as the drawing surface for rendering output. Must be a valid, initialized Control.</param>
        /// <param name="sizeOverride">An optional size to override the natural screen size. If specified, this value determines the rendering
        /// area; otherwise, the size of the drawing surface is used.</param>
        public DisplayManager(AeEngine engine, Control drawingSurface, Size? sizeOverride = null)
        {
            _engine = engine;
            DrawingSurface = drawingSurface;

            if (sizeOverride != null)
            {
                NaturalScreenSize = new Size(sizeOverride.Value.Width, sizeOverride.Value.Height);
            }
            else
            {
                NaturalScreenSize = new Size(drawingSurface.Width, drawingSurface.Height);
            }

            Screen = Screen.FromHandle(drawingSurface.Handle);

            int totalSizeX = (int)(NaturalScreenSize.Width * _engine.Settings.OverdrawScale);
            int totalSizeY = (int)(NaturalScreenSize.Height * _engine.Settings.OverdrawScale);

            if (totalSizeX % 2 != 0) totalSizeX++;
            if (totalSizeY % 2 != 0) totalSizeY++;

            TotalCanvasSize = new Size(totalSizeX, totalSizeY);
            OverdrawSize = new Size(totalSizeX - NaturalScreenSize.Width, totalSizeY - NaturalScreenSize.Height);
            CenterCanvas = new AeVector(TotalCanvasSize.Width / 2.0f, TotalCanvasSize.Height / 2.0f);

            TotalCanvasDiagonal = (float)Math.Sqrt(TotalCanvasSize.Width * TotalCanvasSize.Width + TotalCanvasSize.Height * TotalCanvasSize.Height);
        }

        /// <summary>
        /// Retrieves the quadrant corresponding to the specified screen coordinates, creating it if it does not already
        /// exist.
        /// </summary>
        /// <remarks>Quadrants are determined by dividing the screen into regions based on the natural
        /// screen size. Repeated calls with the same coordinates will return the same quadrant instance.</remarks>
        /// <param name="x">The horizontal position, in pixels, relative to the natural screen size.</param>
        /// <param name="y">The vertical position, in pixels, relative to the natural screen size.</param>
        /// <returns>The quadrant instance associated with the given coordinates. If no quadrant exists for the coordinates, a
        /// new one is created and returned.</returns>
        public AeQuadrant GetQuadrant(float x, float y)
        {
            var coordinates = new Point((int)(x / NaturalScreenSize.Width), (int)(y / NaturalScreenSize.Height));

            if (Quadrants.ContainsKey(coordinates) == false)
            {
                var absoluteBounds = new Rectangle(
                    NaturalScreenSize.Width * coordinates.X,
                    NaturalScreenSize.Height * coordinates.Y,
                    NaturalScreenSize.Width,
                    NaturalScreenSize.Height);

                var quad = new AeQuadrant(coordinates, absoluteBounds);

                Quadrants.Add(coordinates, quad);
            }

            return Quadrants[coordinates];
        }
    }
}
