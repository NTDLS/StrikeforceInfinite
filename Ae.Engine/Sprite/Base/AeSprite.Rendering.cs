using Ae.Engine.Sprite.Interactive.Ship;
using Ae.Engine.Sprite.Munition;
using SharpDX.Mathematics.Interop;
using System.Drawing;

namespace Ae.Engine.Sprite.Base
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class AeSprite
    {
        /// <summary>
        /// Renders the sprite and its visual highlights onto the specified Direct2D render target for the given
        /// animation epoch.
        /// </summary>
        /// <remarks>Highlights and motion rectangles are rendered if their respective flags are set. The
        /// method does not perform rendering if the sprite is not visible or its bitmap is null.</remarks>
        /// <param name="renderTarget">The Direct2D render target to which the sprite and its highlights are drawn.</param>
        /// <param name="epoch">The animation epoch, in seconds, representing the current time frame for rendering. Negative values may be
        /// used to account for motion that has already occurred.</param>
        internal virtual void Render(SharpDX.Direct2D1.RenderTarget renderTarget, float epoch)
        {
            if (_isVisible && SpriteBitmap != null)
            {
                DrawImage(renderTarget, SpriteBitmap);

                if (IsHighlighted)
                {
                    Engine.Rendering.DrawRectangle(renderTarget, RawRenderBounds,
                        Engine.Rendering.Materials.Colors.Red, 0, 1, Orientation.RadiansSigned);
                }

                if (HighlightSweptMotionRect)
                {
                    //We use negative epoch because when we reach rendering, the sprite has already moved.
                    var swept = SweptAabbForMotion(-epoch);

                    var sweptRect = new RawRectangleF(
                        swept.min.X - Engine.Display.CameraPosition.X,
                        swept.min.Y - Engine.Display.CameraPosition.Y,
                        swept.max.X - Engine.Display.CameraPosition.X,
                        swept.max.Y - Engine.Display.CameraPosition.Y
                    );

                    Engine.Rendering.DrawRectangle(renderTarget, sweptRect,
                            Engine.Rendering.Materials.Colors.Red, 0, 1, 0);
                }
            }
        }

        internal virtual void Render(Graphics dc)
        {
        }

        internal void RenderRadar(SharpDX.Direct2D1.RenderTarget renderTarget, int x, int y)
        {
            if (_isVisible && SpriteBitmap != null)
            {
                if (this is AeSpriteEnemy)
                {
                    Engine.Rendering.DrawTriangle(renderTarget, x, y, 3, 3, Engine.Rendering.Materials.Colors.OrangeRed);
                }
                else if (this is AeSpriteMunition munition)
                {
                    float size;
                    RawColor4 color;

                    if (munition.FiredFromType == AeFiredFromType.Enemy)
                    {
                        color = Engine.Rendering.Materials.Colors.Red;
                    }
                    else
                    {
                        color = Engine.Rendering.Materials.Colors.Green;
                    }

                    if (munition.Weapon.Metadata?.ExplodesOnImpact == true)
                    {
                        size = 2;
                    }
                    else
                    {
                        size = 1;
                    }

                    Engine.Rendering.DrawSolidEllipse(renderTarget, x, y, size, size, color);
                }
            }
        }

        /// <summary>
        /// Draws the specified bitmap onto the given render target at the object's location, optionally rotated by the
        /// specified angle in radians.
        /// </summary>
        /// <remarks>The bitmap is centered at the object's render location. Rotation is applied around
        /// the center of the bitmap.</remarks>
        /// <param name="renderTarget">The render target on which the bitmap will be drawn. Must not be null.</param>
        /// <param name="bitmap">The bitmap image to draw. Must not be null.</param>
        /// <param name="angleRadians">The angle, in radians, to rotate the bitmap when drawing. If null, the object's current orientation is used.</param>
        public void DrawImage(SharpDX.Direct2D1.RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float? angleRadians = null)
        {
            float angle = (float)(angleRadians == null ? Orientation.RadiansSigned : angleRadians);

            Engine.Rendering.DrawBitmap(renderTarget, bitmap,
                RenderLocation.X - bitmap.Size.Width / 2.0f,
                RenderLocation.Y - bitmap.Size.Height / 2.0f, angle);
        }
    }
}
