using Ae.Engine.Mathematics;

namespace Ae.Engine.Sprite.Interactive.Ship
{
    /// <summary>
    /// Provides a specialized sprite ship for debugging purposes within the engine.
    /// </summary>
    /// <remarks>AeSpriteDebug extends AeSpriteShip to facilitate debugging scenarios, such as visualizing
    /// movement and radar positioning. This class is intended for use in development and testing environments, and may
    /// expose additional behaviors or indicators to assist with diagnostics.</remarks>
    public class AeSpriteDebug
        : AeSpriteShip
    {
        /// <summary>
        /// Initializes a new instance of the AeSpriteDebug class using the specified engine and asset key.
        /// </summary>
        /// <remarks>The movement vector is recalculated based on the sprite's orientation during
        /// initialization. Ensure that the asset key provided is valid to avoid runtime errors.</remarks>
        /// <param name="engine">The engine instance that manages the sprite's lifecycle and rendering context. Cannot be null.</param>
        /// <param name="assetKey">The key identifying the asset to be used for the sprite. Must correspond to a valid asset in the engine's
        /// asset store.</param>
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
