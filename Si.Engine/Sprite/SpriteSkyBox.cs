using Si.Engine.Sprite._Superclass._Root;
using Si.Library.Mathematics;
using System;

namespace Si.Engine.Sprite
{
    public class SpriteSkyBox
        : SpriteBase
    {
        public SpriteSkyBox(EngineCore engine, string assetKey)
            : base(engine, assetKey)
        {
            //selectedImageIndex = SiRandom.Between(0, _imageCount - 1);

            //X = SiRandom.Between(0, engine.Display.TotalCanvasSize.Width);
            //Y = SiRandom.Between(0, engine.Display.TotalCanvasSize.Height);
            //Z = int.MinValue;

            //Speed = 0.10f;

            //if (selectedImageIndex >= 0 && selectedImageIndex <= 0)
            //{
            //    Throttle = SiRandom.Between(8, 10) / 10.0f;
            //}
            //else
            //{
            //    Throttle = SiRandom.Between(4, 8) / 10.0f;
            //}
        }

        private SiVector _currentOffset = new();
        private readonly float _maxOffset = 200;

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            if (displacementVector.Sum() != 0)
            {
                var offsetIncrement = new SiVector(displacementVector.Normalize());

                offsetIncrement.X *= (1 - (Math.Abs(_currentOffset.X) / _maxOffset));
                offsetIncrement.Y *= (1 - (Math.Abs(_currentOffset.Y) / _maxOffset));

                _currentOffset += offsetIncrement;

                Location = _engine.Display.CenterOfCurrentScreen - _currentOffset;
            }
        }
    }
}
