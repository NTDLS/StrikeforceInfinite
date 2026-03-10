using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.Interactive.Ship
{
    /// <summary>
    /// The enemy base is a sub-class of the ship base. It is used by Peon and Boss enemies.
    /// </summary>
    [AssetClass("Enemy", "", AeBaseAssetType.Image, true)]
    public class SpriteEnemy
        : SpriteShip
    {
        public SpriteEnemy(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            RecalculateMovementVectorFromOrientation();

            RadarPositionIndicator = Engine.Sprites.RadarPositions.Add();
            RadarPositionIndicator.IsVisible = false;

            RadarPositionText = Engine.Sprites.TextBlocks.CreateRadarPosition(
                engine.Rendering.TextFormats.RadarPositionIndicator,
                engine.Rendering.Materials.Brushes.Red, new AeVector());
        }

        public override void OrientationChanged() => LocationChanged();

        public override void Explode()
        {
            base.Explode();
        }

        /// <summary>
        /// Moves the sprite based on its velocity/boost (velocity) taking into account the background scroll.
        /// </summary>
        public override void ApplyMotion(float epoch, AeVector cameraDisplacement)
        {
            base.ApplyMotion(epoch, cameraDisplacement);

            AdjustRadarPositionIndicator();
        }
    }
}
