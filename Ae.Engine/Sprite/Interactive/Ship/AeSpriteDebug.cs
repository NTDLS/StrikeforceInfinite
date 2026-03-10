using Ae.Engine.Mathematics;

namespace Ae.Engine.Sprite.Interactive.Ship
{
    public class AeSpriteDebug
        : AeSpriteShip
    {
        public AeSpriteDebug(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            RecalculateMovementVectorFromOrientation();
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
