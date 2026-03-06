using SharpDX.Mathematics.Interop;
using Si.Engine.Sprite._Superclass.Interactive.Ship;
using Si.Engine.Sprite._Superclass.Munition;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite._Superclass._Root
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class SpriteBase
    {
        public virtual void Render(SharpDX.Direct2D1.RenderTarget renderTarget, float epoch)
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

        public virtual void Render(Graphics dc)
        {
        }

        public void RenderRadar(SharpDX.Direct2D1.RenderTarget renderTarget, int x, int y)
        {
            if (_isVisible && SpriteBitmap != null)
            {
                if (this is SpriteEnemy)
                {
                    Engine.Rendering.DrawTriangle(renderTarget, x, y, 3, 3, Engine.Rendering.Materials.Colors.OrangeRed);
                }
                else if (this is SpriteMunition munition)
                {
                    float size;
                    RawColor4 color;

                    if (munition.FiredFromType == SiFiredFromType.Enemy)
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

        public void DrawImage(SharpDX.Direct2D1.RenderTarget renderTarget, SharpDX.Direct2D1.Bitmap bitmap, float? angleRadians = null)
        {
            float angle = (float)(angleRadians == null ? Orientation.RadiansSigned : angleRadians);

            Engine.Rendering.DrawBitmap(renderTarget, bitmap,
                RenderLocation.X - bitmap.Size.Width / 2.0f,
                RenderLocation.Y - bitmap.Size.Height / 2.0f, angle);
        }
    }
}
