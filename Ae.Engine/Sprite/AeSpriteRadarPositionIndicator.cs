using Ae.Engine.Sprite.Base;

namespace Ae.Engine.Sprite
{
    /// <summary>
    /// Represents a radar position indicator sprite within the engine, providing visual feedback for object locations
    /// on a radar display.
    /// </summary>
    /// <remarks>This class is intended for use in scenarios where a radar or minimap visualization is
    /// required. It inherits from AeSprite, enabling integration with the engine's sprite management and rendering
    /// system.</remarks>
    public class AeSpriteRadarPositionIndicator
        : AeSprite
    {
        /// <summary>
        /// Initializes a new instance of the AeSpriteRadarPositionIndicator class using the specified engine and asset
        /// key.
        /// </summary>
        /// <param name="engine">The engine instance used to manage rendering and game logic for the radar position indicator.</param>
        /// <param name="assetKey">The key identifying the sprite asset to be used for the radar position indicator.</param>
        public AeSpriteRadarPositionIndicator(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            X = 0;
            Y = 0;
        }
    }
}
