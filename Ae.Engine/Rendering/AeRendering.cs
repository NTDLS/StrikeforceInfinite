using Ae.Engine.DataModels;
using Ae.Engine.Helpers;
using NTDLS.Semaphore;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Ae.Engine.Rendering
{
    /// <summary>
    /// Provides rendering functionality for drawing bitmaps, shapes, text, and effects onto Direct2D surfaces. Manages
    /// render targets, materials, and text formats for use in graphical operations.
    /// </summary>
    /// <remarks>AeRendering is responsible for coordinating drawing operations, including support for screen
    /// shake effects and fragment generation. It encapsulates resource management for render targets and exposes
    /// precreated materials and text formats for efficient rendering. The class is not thread-safe; all rendering
    /// operations should be performed on the UI thread associated with the drawing surface. Dispose the instance when
    /// finished to release underlying graphics resources.</remarks>
    public class AeRendering
        : IDisposable
    {
        private struct ScreenShake
        {
            public float Intensity = 0;
            public double Duration = 0;
            public Stopwatch Timer = new();
            public ScreenShake() { }
        }

        internal PessimisticCriticalResource<AeCriticalRenderTargets> RenderTargets { get; private set; } = new();

        /// <summary>
        /// Gets the collection of precreated materials available for use in the application.
        /// </summary>
        public AePrecreatedMaterials Materials { get; private set; }

        /// <summary>
        /// Gets the collection of precreated text format settings used for rendering text elements.
        /// </summary>
        public AePrecreatedTextFormats TextFormats { get; private set; }

        private readonly List<ScreenShake> _screenShakes = new();
        private readonly SharpDX.Direct2D1.Factory _direct2dFactory = new(FactoryType.SingleThreaded);
        private readonly SharpDX.DirectWrite.Factory _directWriteFactory = new();
        private readonly ImagingFactory _wicFactory = new();
        private Size _totalCanvasSize;
        private Size _drawingSurfaceSize;

        /// <summary>
        /// Initializes a new instance of the AeRendering class, configuring rendering targets and materials for drawing
        /// operations based on the specified engine settings and canvas size.
        /// </summary>
        /// <remarks>The constructor sets up both screen and intermediate render targets, allowing for
        /// efficient rendering and zooming capabilities. Anti-aliasing and presentation options are configured
        /// according to the provided engine settings. The intermediate render target is sized to support zoom-out
        /// scenarios for broader universe views.</remarks>
        /// <param name="settings">The engine settings that determine rendering options such as vertical synchronization and anti-aliasing.</param>
        /// <param name="drawingSurface">The control that serves as the drawing surface for rendering output. Must be a valid, initialized window
        /// handle.</param>
        /// <param name="totalCanvasSize">The total size of the canvas, in pixels, used to configure the intermediate render target for zooming and
        /// universe visualization.</param>
        public AeRendering(AeEngineSettings settings, Control drawingSurface, Size totalCanvasSize)
        {
            _drawingSurfaceSize = drawingSurface.Size;
            _totalCanvasSize = totalCanvasSize;

            var presentOptions = PresentOptions.Immediately;
            var antiAliasMode = AntialiasMode.Aliased;

            if (settings.VerticalSync == true)
            {
                presentOptions = PresentOptions.None;
            }

            if (settings.AntiAliasing == true)
            {
                antiAliasMode = AntialiasMode.PerPrimitive;
            }

            var windowRenderProperties = new HwndRenderTargetProperties()
            {
                PresentOptions = presentOptions,
                Hwnd = drawingSurface.Handle,
                PixelSize = new Size2(_drawingSurfaceSize.Width, _drawingSurfaceSize.Height)
                //PixelSize = new Size2(engine.Display.NaturalScreenSize.Width, engine.Display.NaturalScreenSize.Height)
            };

            var renderTargetProperties = new RenderTargetProperties
            {
                PixelFormat = new SharpDX.Direct2D1.PixelFormat(Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied),
                //MinLevel = FeatureLevel.Level_10,
                Type = RenderTargetType.Hardware
            };

            //The intermediate render target is much larger than the render target window. We create this
            //  larger render target so that we can zoom-out when we want to see more of the universe.
            var intermediateRenderTargetSize = new Size2F(_totalCanvasSize.Width, _totalCanvasSize.Height);

            var renderTargets = new AeCriticalRenderTargets()
            {
                ScreenRenderTarget = new WindowRenderTarget(_direct2dFactory, renderTargetProperties, windowRenderProperties)
                {
                    AntialiasMode = antiAliasMode
                }
            };

            renderTargets.IntermediateRenderTarget = new BitmapRenderTarget(
                renderTargets.ScreenRenderTarget, CompatibleRenderTargetOptions.None, intermediateRenderTargetSize)
            {
                AntialiasMode = antiAliasMode
            };

            RenderTargets.Use(o =>
            {
                o.ScreenRenderTarget = renderTargets.ScreenRenderTarget;
                o.IntermediateRenderTarget = renderTargets.IntermediateRenderTarget;
            });

            Materials = new AePrecreatedMaterials(renderTargets.ScreenRenderTarget);
            TextFormats = new AePrecreatedTextFormats(_directWriteFactory);

            AeTransforms.RegisterRenderTarget(renderTargets.ScreenRenderTarget);
            AeTransforms.RegisterRenderTarget(renderTargets.IntermediateRenderTarget);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the class.
        /// </summary>
        /// <remarks>Call this method when you are finished using the object to ensure that all associated
        /// resources are properly released. After calling Dispose, the object should not be used further.</remarks>
        public void Dispose()
        {
            RenderTargets.Use(o =>
            {
                o.ScreenRenderTarget?.Dispose();
                o.ScreenRenderTarget?.Dispose();
            });
        }

        /// <summary>
        /// Transfers the image from one render target to the other with a scaling factor.
        /// Also applies screen shake since this is just a hella convenient place to do it.
        /// </summary>
        /// <param name="intermediateRenderTarget"></param>
        /// <param name="screenRenderTarget"></param>
        /// <param name="scale"></param>
        public void TransferWithZoom(BitmapRenderTarget intermediateRenderTarget, RenderTarget screenRenderTarget, float scale)
        {
            var sourceRect = AeRenderingUtility.CalculateCenterCopyRectangle(intermediateRenderTarget.Size, scale);
            var destRect = new RawRectangleF(0, 0, _drawingSurfaceSize.Width, _drawingSurfaceSize.Height);

            var appliedScreenShakes = new List<ScreenShake>();

            foreach (var screenShake in _screenShakes)
            {
                var totalElapsedScreenShakeTime = ((double)screenShake.Timer.ElapsedTicks / (double)Stopwatch.Frequency) * 1000.0;

                var percentComplete = (float)(totalElapsedScreenShakeTime / screenShake.Duration);
                if (percentComplete >= 1)
                {
                    screenShake.Timer.Stop();
                }

                var intensity = screenShake.Intensity * (1 - percentComplete);

                var offsetX = (float)(AeRandom.NextFloat() * intensity * 2 - intensity);
                var offsetY = (float)(AeRandom.NextFloat() * intensity * 2 - intensity);

                AeTransforms.PushTransform(screenRenderTarget, Matrix3x2.Translation(offsetX, offsetY));

                appliedScreenShakes.Add(screenShake);
            }

            screenRenderTarget.DrawBitmap(intermediateRenderTarget.Bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);

            foreach (var screenShake in appliedScreenShakes)
            {
                if (screenShake.Timer.IsRunning == false)
                {
                    _screenShakes.Remove(screenShake);
                }

                AeTransforms.PopTransform(screenRenderTarget);
            }
        }

        #region Rending: Bitmaps.

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public void DrawBitmap(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float x, float y, float angleRadians)
        {
            if (angleRadians > 6.3)
            {
                //throw new Exception($"Radians are out of range: {angleRadians:n4}");
            }

            var destRect = new RawRectangleF(x, y, (x + bitmap.PixelSize.Width), (y + bitmap.PixelSize.Height));
            AeTransforms.PushTransform(renderTarget, AeTransforms.CreateAngleTransform(destRect, angleRadians));
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            AeTransforms.PopTransform(renderTarget);
        }

        /// Draws a bitmap from a specified location of a given size, to the the specified location.
        public void DrawBitmap(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap,
            float x, float y, float angleRadians, RawRectangleF sourceRect, Size2F destSize)
        {
            if (angleRadians > 6.3)
            {
                //throw new Exception($"Radians are out of range: {angleRadians:n4}");
            }

            var destRect = new RawRectangleF(x, y, (x + destSize.Width), (y + destSize.Height));
            AeTransforms.PushTransform(renderTarget, AeTransforms.CreateAngleTransform(destRect, angleRadians));
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear, sourceRect);
            AeTransforms.PopTransform(renderTarget);
        }

        /// <summary>
        /// Draws a bitmap at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the bitmap.</returns>
        public void DrawBitmap(RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float x, float y)
        {
            var destRect = new RawRectangleF(x, y, (x + bitmap.PixelSize.Width), (y + bitmap.PixelSize.Height));
            renderTarget.DrawBitmap(bitmap, destRect, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
        }

        #endregion

        #region Rending: Text.

        /// <summary>
        /// Calculates the bounding rectangle for the specified text at the given position using the provided text
        /// format.
        /// </summary>
        /// <remarks>The returned rectangle includes the full width and height required to render the text
        /// with the specified format. This method does not render the text; it only calculates the layout
        /// bounds.</remarks>
        /// <param name="x">The horizontal coordinate, in pixels, of the upper-left corner where the text is positioned.</param>
        /// <param name="y">The vertical coordinate, in pixels, of the upper-left corner where the text is positioned.</param>
        /// <param name="text">The text string for which the bounding rectangle is calculated. Cannot be null.</param>
        /// <param name="format">The text format to apply when measuring the text. Cannot be null.</param>
        /// <returns>A RawRectangleF structure representing the bounding rectangle of the text at the specified position. The
        /// rectangle's width and height are determined by the measured size of the text.</returns>
        public RawRectangleF GetTextRect(float x, float y, string text, SharpDX.DirectWrite.TextFormat format)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, text, format, float.MaxValue, float.MaxValue);
            return new RawRectangleF(x, y, (x + textLayout.Metrics.Width), (y + textLayout.Metrics.Height));
        }

        /// <summary>
        /// Calculates the size, in device-independent pixels, required to render the specified text using the given
        /// text format.
        /// </summary>
        /// <remarks>The measurement accounts for potential trimming of trailing characters by the
        /// underlying DirectWrite layout engine, ensuring accurate sizing for the provided text. Use this method to
        /// determine layout requirements before rendering text.</remarks>
        /// <param name="text">The text string to measure. May include any characters supported by the specified format.</param>
        /// <param name="format">The text formatting options to apply when measuring the text, such as font family, size, and style.</param>
        /// <returns>A SizeF structure representing the width and height needed to display the text with the specified format.
        /// The width and height are measured in device-independent pixels.</returns>
        public SizeF GetTextSize(string text, SharpDX.DirectWrite.TextFormat format)
        {
            //We have to check the size with some ending characters because TextLayout() seems to want to trim the text before calculating the metrics.
            using var textLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, $"[{text}]", format, float.MaxValue, float.MaxValue);
            using var spacerLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, "[]", format, float.MaxValue, float.MaxValue);
            return new SizeF(textLayout.Metrics.Width - spacerLayout.Metrics.Width, textLayout.Metrics.Height);
        }

        /// <summary>
        /// Draws text at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the text.</returns>
        public void DrawText(RenderTarget renderTarget,
            float x, float y, float angleRadians, string text, SharpDX.DirectWrite.TextFormat format, SolidColorBrush brush)
        {
            using var textLayout = new SharpDX.DirectWrite.TextLayout(_directWriteFactory, text, format, float.MaxValue, float.MaxValue);

            var textWidth = textLayout.Metrics.Width;
            var textHeight = textLayout.Metrics.Height;

            // Create a rectangle that fits the text
            var destRect = new RawRectangleF(x, y, (x + textWidth), (y + textHeight));

            //DrawRectangle(renderTarget, textRectangle, 0, Materials.Raw.Blue, 0, 1);

            AeTransforms.PushTransform(renderTarget, AeTransforms.CreateAngleTransform(destRect, angleRadians));
            renderTarget.DrawText(text, format, destRect, brush);
            AeTransforms.PopTransform(renderTarget);
        }

        #endregion

        #region Rending: Lines.

        /// <summary>
        /// Draws a straight line between two points on the specified render target using the given brush and stroke
        /// width.
        /// </summary>
        /// <param name="renderTarget">The render target on which the line will be drawn. Cannot be null.</param>
        /// <param name="startPointX">The X-coordinate of the starting point of the line, in device-independent pixels.</param>
        /// <param name="startPointY">The Y-coordinate of the starting point of the line, in device-independent pixels.</param>
        /// <param name="endPointX">The X-coordinate of the ending point of the line, in device-independent pixels.</param>
        /// <param name="endPointY">The Y-coordinate of the ending point of the line, in device-independent pixels.</param>
        /// <param name="brush">The brush used to paint the line. Cannot be null.</param>
        /// <param name="strokeWidth">The width of the line stroke, in device-independent pixels. Must be greater than zero. Defaults to 1.</param>
        public void DrawLine(RenderTarget renderTarget,
            float startPointX, float startPointY, float endPointX, float endPointY,
            SolidColorBrush brush, float strokeWidth = 1)
        {
            var startPoint = new RawVector2(startPointX, startPointY);
            var endPoint = new RawVector2(endPointX, endPointY);

            renderTarget.DrawLine(startPoint, endPoint, brush, strokeWidth);
        }

        #endregion

        #region Rending: Ellipse.

        /// <summary>
        /// Draws a color filled ellipse at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public void DrawSolidEllipse(RenderTarget renderTarget, float x, float y,
            float radiusX, float radiusY, Color4 color, float angleRadians = 0)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2(x, y),
                RadiusX = radiusX,
                RadiusY = radiusY,
            };

            var destRect = new RawRectangleF(
                (x - radiusX / 2.0f),
                (y - radiusY / 2.0f),
                ((x - radiusX / 2.0f) + radiusX),
                ((y - radiusY / 2.0f) + radiusY));
            AeTransforms.PushTransform(renderTarget, AeTransforms.CreateAngleTransform(destRect, angleRadians));

            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.FillEllipse(ellipse, brush);

            AeTransforms.PopTransform(renderTarget);
        }

        /// <summary>
        /// Draws a color gradient filled ellipse at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public void DrawGradientEllipse(RenderTarget renderTarget, float x, float y,
            float radiusX, float radiusY, Color4 startColor, Color4 endColor, float angleRadians = 0)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2(x, y),
                RadiusX = radiusX,
                RadiusY = radiusY,
            };

            var destRect = new RawRectangleF(
                (x - radiusX / 2.0f),
                (y - radiusY / 2.0f),
                ((x - radiusX / 2.0f) + radiusX),
                ((y - radiusY / 2.0f) + radiusY));
            AeTransforms.PushTransform(renderTarget, AeTransforms.CreateAngleTransform(destRect, angleRadians));

            // Define gradient stops
            using var gradientStops = new GradientStopCollection(renderTarget, new GradientStop[]
            {
                new GradientStop() { Position = 0.0f, Color = startColor },
                new GradientStop() { Position = 1.0f, Color = endColor }
            });

            // Create linear gradient brush
            using var linearGradientBrush = new LinearGradientBrush(renderTarget,
                new LinearGradientBrushProperties()
                {
                    StartPoint = new RawVector2(x - radiusX, y),
                    EndPoint = new RawVector2(x + radiusX, y)
                }, gradientStops);

            // Fill ellipse with gradient brush
            renderTarget.FillEllipse(ellipse, linearGradientBrush);

            AeTransforms.PopTransform(renderTarget);
        }

        /// <summary>
        /// Draws a hollow ellipse at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public void DrawEllipse(RenderTarget renderTarget, float x, float y,
            float radiusX, float radiusY, Color4 color, float strokeWidth = 1, float angleRadians = 0)
        {
            var ellipse = new Ellipse()
            {
                Point = new RawVector2(x, y),
                RadiusX = radiusX,
                RadiusY = radiusY,
            };

            var destRect = new RawRectangleF(
                (x - radiusX / 2.0f),
                (y - radiusY / 2.0f),
                ((x - radiusX / 2.0f) + radiusX),
                ((y - radiusY / 2.0f) + radiusY));

            AeTransforms.PushTransform(renderTarget, AeTransforms.CreateAngleTransform(destRect, angleRadians));
            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.DrawEllipse(ellipse, brush, strokeWidth);

            AeTransforms.PopTransform(renderTarget);
        }

        #endregion

        #region Rending: Triangle.

        /// <summary>
        /// Draws a filled triangle on the specified render target at the given position, size, color, stroke width, and
        /// rotation angle.
        /// </summary>
        /// <remarks>The triangle is centered at the specified (x, y) coordinates and rotated by the given
        /// angle. The method applies both translation and rotation transforms before drawing. Stroke width affects the
        /// outline thickness; the triangle is always filled.</remarks>
        /// <param name="renderTarget">The render target on which the triangle will be drawn.</param>
        /// <param name="x">The x-coordinate of the center position where the triangle will be rendered.</param>
        /// <param name="y">The y-coordinate of the center position where the triangle will be rendered.</param>
        /// <param name="height">The height of the triangle, in device-independent pixels.</param>
        /// <param name="width">The width of the triangle, in device-independent pixels.</param>
        /// <param name="color">The color used to fill and outline the triangle.</param>
        /// <param name="strokeWidth">The width of the triangle's outline, in device-independent pixels. Defaults to 1.</param>
        /// <param name="angleRadians">The rotation angle of the triangle, in radians. The triangle is rotated around its center. Defaults to 0.</param>
        public void DrawTriangle(RenderTarget renderTarget, float x, float y,
            float height, float width, Color4 color, float strokeWidth = 1, float angleRadians = 0)
        {
            // Define the points for the triangle
            var trianglePoints = new RawVector2[]
            {
                new RawVector2(0, height),           // Vertex 1 (bottom-left)
                new RawVector2(width, height), // Vertex 2 (bottom-right)
                new RawVector2((width / 2.0f), 0)      // Vertex 3 (top-center)
            };

            // Create a PathGeometry and add the triangle to it
            var triangleGeometry = new PathGeometry(_direct2dFactory);
            using (GeometrySink sink = triangleGeometry.Open())
            {
                sink.BeginFigure(trianglePoints[0], FigureBegin.Filled);
                sink.AddLines(trianglePoints);
                sink.EndFigure(FigureEnd.Closed);
                sink.Close();
            }

            // Calculate the center of the triangle
            float centerX = (trianglePoints[0].X + trianglePoints[1].X + trianglePoints[2].X) / 3;
            float centerY = (trianglePoints[0].Y + trianglePoints[1].Y + trianglePoints[2].Y) / 3;

            // Calculate the adjustment needed to center the triangle at the desired position
            x -= centerX;
            y -= centerY;

            // Create a translation transform to move the triangle to the desired position
            var destRect = new RawRectangleF(x, y, (x + width), (y + height));

            AeTransforms.PushTransform(renderTarget,
                Matrix3x2.Multiply(AeTransforms.CreateOffsetTransform(x, y), AeTransforms.CreateAngleTransform(destRect, angleRadians)));

            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.DrawGeometry(triangleGeometry, brush, strokeWidth);

            AeTransforms.PopTransform(renderTarget);
        }

        #endregion

        #region Rending: Polygon.

        /// <summary>
        /// Draws a polygon defined by the specified points onto the given render target using the specified color and
        /// stroke width.
        /// </summary>
        /// <param name="renderTarget">The render target on which the polygon will be drawn.</param>
        /// <param name="points">An array of points that define the vertices of the polygon. The points must be ordered to represent the
        /// desired shape.</param>
        /// <param name="color">The color used to draw the outline of the polygon.</param>
        /// <param name="strokeWidth">The width, in pixels, of the polygon's outline. Defaults to 1.0 if not specified.</param>
        public void DrawPolygon(RenderTarget renderTarget, PointF[] points, RawColor4 color, float strokeWidth = 1.0f)
        {
            DrawPolygon(renderTarget, 0, 0, points, color, strokeWidth);
        }

        /// <summary>
        /// Draws a closed polygon on the specified render target using the given points, color, and stroke width.
        /// </summary>
        /// <remarks>The polygon is drawn as a closed shape, connecting the last point to the first. The
        /// points are offset by the specified x and y values before drawing. If the points array is empty, no polygon
        /// is drawn.</remarks>
        /// <param name="renderTarget">The render target on which the polygon will be drawn.</param>
        /// <param name="x">The horizontal offset applied to each point in the polygon.</param>
        /// <param name="y">The vertical offset applied to each point in the polygon.</param>
        /// <param name="points">An array of points defining the vertices of the polygon. The array must contain at least one point.</param>
        /// <param name="color">The color used to draw the outline of the polygon.</param>
        /// <param name="strokeWidth">The width, in pixels, of the polygon's outline. Defaults to 1.0 if not specified.</param>
        public void DrawPolygon(RenderTarget renderTarget, float x, float y, PointF[] points, RawColor4 color, float strokeWidth = 1.0f)
        {
            if (points.Length == 0)
            {
                return;
            }
            var rawPoints = Array.ConvertAll(points, point => new RawVector2(point.X + x, point.Y + y));

            // Create a PathGeometry to define the shape of the polygon
            using (var pathGeometry = new PathGeometry(_direct2dFactory))
            {
                using (var geometrySink = pathGeometry.Open())
                {
                    geometrySink.BeginFigure(rawPoints[0], FigureBegin.Filled);
                    geometrySink.AddLines(rawPoints);
                    geometrySink.EndFigure(FigureEnd.Closed);
                    geometrySink.Close();
                }

                // Draw the polygon
                using (var brush = new SolidColorBrush(renderTarget, color))
                {
                    renderTarget.DrawGeometry(pathGeometry, brush, strokeWidth);
                }
            }
        }

        #endregion

        #region Rending: Rectangle.

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawRectangle(RenderTarget renderTarget, RawRectangleF destRect,
            RawColor4 color, float expand = 0, float strokeWidth = 1, float angleRadians = 0)
            => DrawRectangle(renderTarget, 0, 0, destRect, color, expand, strokeWidth, angleRadians);

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawRectangle(RenderTarget renderTarget, float x, float y, RawRectangleF destRect,
            Color4 color, float expand = 0, float strokeWidth = 1, float angleRadians = 0)
        {
            if (expand != 0)
            {
                destRect.Left -= expand;
                destRect.Top -= expand;
                destRect.Bottom += expand;
                destRect.Right += expand;
            }

            if (x != 0 && y != 0)
            {
                destRect.Left += x;
                destRect.Top += y;
                destRect.Bottom += y;
                destRect.Right += x;
            }

            AeTransforms.PushTransform(renderTarget, AeTransforms.CreateAngleTransform(destRect, angleRadians));
            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.DrawRectangle(destRect, brush, strokeWidth);
            AeTransforms.PopTransform(renderTarget);

            return destRect;
        }

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawSolidRectangle(RenderTarget renderTarget, RawRectangleF destRect,
            RawColor4 color, float expand = 0, float angleRadians = 0)
            => DrawSolidRectangle(renderTarget, 0, 0, destRect, color, expand, angleRadians);

        /// <summary>
        /// Draws a solid rectangle on the specified render target with the given color, position, size, and optional
        /// expansion and rotation.
        /// </summary>
        /// <remarks>The rectangle is filled with the specified color and transformed according to the
        /// provided parameters. The returned rectangle reflects any modifications made by expansion and offset, but not
        /// rotation. This method does not modify the original destRect parameter.</remarks>
        /// <param name="renderTarget">The render target on which the rectangle will be drawn.</param>
        /// <param name="x">The horizontal offset to apply to the rectangle's position. If nonzero, shifts the rectangle by this amount.</param>
        /// <param name="y">The vertical offset to apply to the rectangle's position. If nonzero, shifts the rectangle by this amount.</param>
        /// <param name="destRect">The destination rectangle specifying the initial position and size. This rectangle may be modified by
        /// expansion and offset parameters.</param>
        /// <param name="color">The color used to fill the rectangle.</param>
        /// <param name="expand">The amount, in pixels, by which to expand the rectangle on all sides. If zero, no expansion is applied.</param>
        /// <param name="angleRadians">The angle, in radians, to rotate the rectangle around its center. If zero, no rotation is applied.</param>
        /// <returns>A RawRectangleF representing the final rectangle after applying expansion, offset, and rotation.</returns>
        public RawRectangleF DrawSolidRectangle(RenderTarget renderTarget, float x, float y, RawRectangleF destRect,
            Color4 color, float expand = 0, float angleRadians = 0)
        {
            if (expand != 0)
            {
                destRect.Left -= expand;
                destRect.Top -= expand;
                destRect.Bottom += expand;
                destRect.Right += expand;
            }

            if (x != 0 && y != 0)
            {
                destRect.Left += x;
                destRect.Top += y;
                destRect.Bottom += y;
                destRect.Right += x;
            }

            AeTransforms.PushTransform(renderTarget, AeTransforms.CreateAngleTransform(destRect, angleRadians));
            using var brush = new SolidColorBrush(renderTarget, color);
            renderTarget.FillRectangle(destRect, brush);
            AeTransforms.PopTransform(renderTarget);

            return destRect;
        }

        /// <summary>
        /// Draws a rectangle at the specified location.
        /// </summary>
        /// <returns>Returns the rectangle that was calculated to hold the Rectangle.</returns>
        public RawRectangleF DrawGradientRectangle(RenderTarget renderTarget, RawRectangleF destRect,
            Color4 startColor, Color4 endColor, float expand = 0, float angleRadians = 0)
            => DrawGradientRectangle(renderTarget, 0, 0, destRect, startColor, endColor, expand, angleRadians);

        /// <summary>
        /// Draws a rectangle filled with a linear gradient between two colors, applying optional expansion and rotation
        /// transformations.
        /// </summary>
        /// <remarks>The gradient is applied vertically from the top to the bottom of the rectangle. The
        /// rectangle is transformed before drawing, based on the specified expansion, offset, and rotation
        /// parameters.</remarks>
        /// <param name="renderTarget">The render target on which the rectangle will be drawn.</param>
        /// <param name="x">The horizontal offset to apply to the rectangle's position. If nonzero, shifts the rectangle by this amount.</param>
        /// <param name="y">The vertical offset to apply to the rectangle's position. If nonzero, shifts the rectangle by this amount.</param>
        /// <param name="destRect">The destination rectangle specifying the area to fill. The rectangle may be modified by expansion and offset
        /// parameters.</param>
        /// <param name="startColor">The color used at the start of the gradient fill.</param>
        /// <param name="endColor">The color used at the end of the gradient fill.</param>
        /// <param name="expand">The amount, in pixels, by which to expand the rectangle on all sides. If zero, no expansion is applied.</param>
        /// <param name="angleRadians">The angle, in radians, to rotate the rectangle. If zero, no rotation is applied.</param>
        /// <returns>A RawRectangleF representing the final rectangle area after applying expansion and offset transformations.</returns>
        public RawRectangleF DrawGradientRectangle(RenderTarget renderTarget, float x, float y, RawRectangleF destRect,
            Color4 startColor, Color4 endColor, float expand = 0, float angleRadians = 0)
        {
            if (expand != 0)
            {
                destRect.Left -= expand;
                destRect.Top -= expand;
                destRect.Bottom += expand;
                destRect.Right += expand;
            }

            if (x != 0 && y != 0)
            {
                destRect.Left += x;
                destRect.Top += y;
                destRect.Bottom += y;
                destRect.Right += x;
            }

            AeTransforms.PushTransform(renderTarget, AeTransforms.CreateAngleTransform(destRect, angleRadians));

            // Define start and end points for the gradient
            var startPoint = new RawVector2(destRect.Left, destRect.Top);
            var endPoint = new RawVector2(destRect.Left, destRect.Bottom);

            // Create gradient stops
            var gradientStops = new GradientStop[]
            {
                new GradientStop { Color = startColor, Position = 0.0f },
                new GradientStop { Color = endColor, Position = 1.0f }
            };

            using (var gradientStopCollection = new GradientStopCollection(renderTarget, gradientStops, Gamma.Linear, ExtendMode.Clamp))

            using (var linearGradientBrush = new LinearGradientBrush(renderTarget,
                new LinearGradientBrushProperties
                {
                    StartPoint = startPoint,
                    EndPoint = endPoint
                }, gradientStopCollection))

                renderTarget.FillRectangle(destRect, linearGradientBrush);
            AeTransforms.PopTransform(renderTarget);

            return destRect;
        }

        #endregion

        /// <summary>
        /// Generates a collection of irregular bitmap fragments from the specified original bitmap.
        /// </summary>
        /// <remarks>Each fragment is generated with a unique shape based on the specified vertex count.
        /// The method does not modify the original bitmap. The returned fragments may overlap or leave gaps depending
        /// on the fragmentation algorithm.</remarks>
        /// <param name="originalBitmap">The source bitmap to be fragmented. Cannot be null.</param>
        /// <param name="countOfFragments">The number of fragments to generate from the original bitmap. Must be greater than zero.</param>
        /// <param name="countOfVertices">The number of vertices used to define the shape of each fragment. Must be greater than two. Defaults to 8.</param>
        /// <returns>A list of bitmap fragments representing irregular portions of the original bitmap. The list contains exactly
        /// the specified number of fragments.</returns>
        public List<SharpDX.Direct2D1.Bitmap> GenerateIrregularFragments(SharpDX.Direct2D1.Bitmap originalBitmap, int countOfFragments, int countOfVertices = 8)
            => AeBitmapFragmenter.GenerateIrregularFragments(this, originalBitmap, countOfFragments, countOfVertices);

        /// <summary>
        /// Generates a collection of irregular bitmap fragments from the specified original bitmap.
        /// </summary>
        /// <param name="originalBitmap">The source bitmap to be fragmented. Cannot be null.</param>
        /// <returns>A list of bitmap fragments representing irregular portions of the original bitmap. The list will be empty if
        /// no fragments are generated.</returns>
        public List<SharpDX.Direct2D1.Bitmap> GenerateIrregularFragments(SharpDX.Direct2D1.Bitmap originalBitmap)
            => AeBitmapFragmenter.GenerateIrregularFragments(this, originalBitmap);

        /// <summary>
        /// Adds a new screen shake effect with the specified intensity and duration.
        /// </summary>
        /// <remarks>Multiple screen shake effects can be active simultaneously. The overall shake may be
        /// influenced by the combination of active effects.</remarks>
        /// <param name="intensity">The strength of the screen shake effect. Must be a positive value to produce a visible shake.</param>
        /// <param name="duration">The length of time, in seconds, that the screen shake effect will last. Must be greater than zero.</param>
        public void AddScreenShake(float intensity, float duration)
        {
            var screenShake = new ScreenShake
            {
                Intensity = intensity,
                Duration = duration,
            };

            screenShake.Timer.Start();
            _screenShakes.Add(screenShake);
        }

        /// <summary>
        /// Creates a Direct2D bitmap from the provided image stream using a 32bpp premultiplied BGRA pixel format.
        /// </summary>
        /// <remarks>The method loads the entire image from the stream and converts it to a format
        /// compatible with Direct2D rendering. The caller is responsible for disposing the returned bitmap when it is
        /// no longer needed.</remarks>
        /// <param name="stream">The image data stream to decode. Must be a valid, readable stream containing image data in a supported
        /// format.</param>
        /// <returns>A Direct2D bitmap representing the decoded image. The bitmap will use a 32bpp premultiplied BGRA pixel
        /// format.</returns>
        public SharpDX.Direct2D1.Bitmap BitmapStreamToD2DBitmap(Stream stream)
        {
            using var decoder = new BitmapDecoder(_wicFactory, stream, DecodeOptions.CacheOnLoad);
            using var frame = decoder.GetFrame(0);
            using var converter = new FormatConverter(_wicFactory);

            converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPBGRA);

            return RenderTargets.Use(o => SharpDX.Direct2D1.Bitmap.FromWicBitmap(o.ScreenRenderTarget, converter));
        }
    }
}
