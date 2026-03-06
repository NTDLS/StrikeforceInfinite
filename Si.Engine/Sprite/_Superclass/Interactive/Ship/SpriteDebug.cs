using Si.Library.Mathematics;

namespace Si.Engine.Sprite._Superclass.Interactive.Ship
{
    public class SpriteDebug
        : SpriteShip
    {
        public SpriteDebug(EngineCore engine, string assetKey)
            : base(engine, assetKey)
        {
            RecalculateMovementVectorFromOrientation();
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
