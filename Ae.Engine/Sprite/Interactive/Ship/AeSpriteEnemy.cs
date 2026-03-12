using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;

namespace Ae.Engine.Sprite.Interactive.Ship
{
    /// <summary>
    /// The enemy base is a sub-class of the ship base. It is used by Peon and Boss enemies.
    /// </summary>
    [AssetClass("Enemy", "", AeBaseAssetType.Image, true)]
    public class AeSpriteEnemy
        : AeSpriteShip
    {
        /// <summary>
        /// Initializes a new instance of the AeSpriteEnemy class using the specified engine and asset key.
        /// </summary>
        /// <remarks>The constructor sets up the enemy's movement vector based on its orientation and
        /// initializes radar indicators for position tracking. The radar position indicator is hidden by
        /// default.</remarks>
        /// <param name="engine">The engine instance that manages rendering and sprite operations for the enemy.</param>
        /// <param name="assetKey">The key identifying the asset to use for the enemy's visual representation.</param>
        public AeSpriteEnemy(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            RecalculateMovementVectorFromOrientation();

            RadarPositionIndicator = Engine.Sprites.RadarPositions.Add();
            RadarPositionIndicator.IsVisible = false;

            RadarPositionText = Engine.Sprites.TextBlocks.CreateRadarPosition(
                engine.Rendering.TextFormats.RadarPositionIndicator,
                engine.Rendering.Materials.Brushes.Red, new AeVector());
        }

        /// <summary>
        /// Handles changes in device orientation by updating the location state.
        /// </summary>
        /// <remarks>This method is typically called when the device's orientation changes, ensuring that
        /// any dependent location information is refreshed accordingly. Override this method to customize behavior when
        /// orientation changes occur.</remarks>
        public override void OrientationChanged() => LocationChanged();

        /// <summary>
        /// Triggers the explosion behavior for the current object.
        /// </summary>
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
