using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Enemy._Superclass
{
    /// <summary>
    /// The enemy base is a sub-class of the ship base. It is used by Peon and Boss enemies.
    /// </summary>
    public class SpriteEnemy
        : SpriteInteractiveShipBase
    {
        public SpriteEnemy(EngineCore engine, string assetKey)
                : base(engine, assetKey)
        {
            RecalculateMovementVectorFromOrientation();

            RadarPositionIndicator = Engine.Sprites.RadarPositions.Add();
            RadarPositionIndicator.IsVisible = false;

            RadarPositionText = Engine.Sprites.TextBlocks.CreateRadarPosition(
                engine.Rendering.TextFormats.RadarPositionIndicator,
                engine.Rendering.Materials.Brushes.Red, new SiVector());
        }

        public override void OrientationChanged() => LocationChanged();

        public override void Explode()
        {
            base.Explode();
        }

        /// <summary>
        /// Moves the sprite based on its velocity/boost (velocity) taking into account the background scroll.
        /// </summary>
        public override void ApplyMotion(float epoch, SiVector cameraDisplacement)
        {
            base.ApplyMotion(epoch, cameraDisplacement);

            AdjustRadarPositionIndicator();
        }
    }
}
