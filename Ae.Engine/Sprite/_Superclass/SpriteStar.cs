using Ae.Engine.Sprite._Superclass._Root;
using Ae.Library;
using Ae.Library.Mathematics;
using Ae.Library.Metadata;
using static Ae.Library.AeConstants;

namespace Ae.Engine.Sprite._Superclass
{
    [AssetClass("Star", "", AeBaseAssetType.Image, true)]
    public class SpriteStar
        : SpriteBase
    {
        public SpriteStar(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            X = AeRandom.Between(0, engine.Display.TotalCanvasSize.Width);
            Y = AeRandom.Between(0, engine.Display.TotalCanvasSize.Height);

            Z = int.MinValue + 1000;

            //if (selectedImageIndex >= 0 && selectedImageIndex <= 0)
            //{
            //Throttle = SiRandom.Between(8, 10) / 10.0f;
            //}
            //else
            //{
            Throttle = AeRandom.Between(4, 8) / 10.0f;
            //}
        }

        public override void ApplyMotion(float epoch, AeVector cameraDisplacement)
        {
            //We omit orientation for stars since they are point-like.
            Location -= cameraDisplacement * Speed * Throttle * epoch;
        }
    }
}
