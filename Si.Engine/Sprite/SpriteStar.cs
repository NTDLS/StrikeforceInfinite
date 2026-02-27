using Si.Engine.Sprite._Superclass._Root;
using Si.Library;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite
{
    public class SpriteStar
        : SpriteBase
    {
        public SpriteStar(EngineCore engine, string spritePath)
            : base(engine, spritePath)
        {
            X = SiRandom.Between(0, engine.Display.TotalCanvasSize.Width);
            Y = SiRandom.Between(0, engine.Display.TotalCanvasSize.Height);

            Z = int.MinValue + 1000;

            //if (selectedImageIndex >= 0 && selectedImageIndex <= 0)
            //{
            //Throttle = SiRandom.Between(8, 10) / 10.0f;
            //}
            //else
            //{
            Throttle = SiRandom.Between(4, 8) / 10.0f;
            //}
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            //We omit orientation for stars since they are point-like.
            Location -= displacementVector * Speed * Throttle * epoch;
        }
    }
}
