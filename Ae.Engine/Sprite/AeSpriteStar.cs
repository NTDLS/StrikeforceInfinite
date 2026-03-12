using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;

namespace Ae.Engine.Sprite
{
    /// <summary>
    /// Represents a star sprite asset within the engine, providing specialized behavior for rendering and motion as a
    /// point-like object.
    /// </summary>
    /// <remarks>AeSpriteStar is used to simulate stars in the scene, inheriting from AeSprite but omitting
    /// orientation to reflect their point-like nature. The class initializes position and depth randomly within the
    /// display area, and applies motion relative to camera displacement. This type is typically used for background or
    /// decorative star effects in 2D environments.</remarks>
    [AssetClass("Star", "", AeBaseAssetType.Image, true)]
    public class AeSpriteStar
        : AeSprite
    {
        /// <summary>
        /// Initializes a new instance of the AeSpriteStar class using the specified engine and asset key.
        /// </summary>
        /// <param name="engine">The engine instance used to manage rendering and game logic for the sprite.</param>
        /// <param name="assetKey">The key identifying the asset to be used for the sprite's visual representation.</param>
        public AeSpriteStar(AeEngine engine, string assetKey)
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

        /// <summary>
        /// Updates the star's location based on the specified camera displacement and elapsed time.
        /// </summary>
        /// <remarks>Orientation is not updated for stars, as they are treated as point-like
        /// objects.</remarks>
        /// <param name="epoch">The elapsed time, in seconds, over which the motion is applied.</param>
        /// <param name="cameraDisplacement">The vector representing the camera's displacement during the specified epoch.</param>
        public override void ApplyMotion(float epoch, AeVector cameraDisplacement)
        {
            //We omit orientation for stars since they are point-like.
            Location -= cameraDisplacement * Speed * Throttle * epoch;
        }
    }
}
