using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Interactive;

namespace Ae.Engine.Sprite.Munition
{
    /// <summary>
    /// Projectile munitions just go straight - these are physical bullets that have no power of their own once fired.
    /// </summary>
    [AssetClass("Munition - Projectile Type", "", AeBaseAssetType.Image, true)]
    public class AeSpriteProjectileMunition
        : AeSpriteMunition
    {
        /// <summary>
        /// Initializes a new instance of the AeSpriteProjectileMunition class representing a projectile munition in the
        /// engine.
        /// </summary>
        /// <param name="engine">The engine instance that manages the game environment and simulation.</param>
        /// <param name="weapon">The weapon that is responsible for firing the projectile.</param>
        /// <param name="firedFrom">The interactive entity from which the projectile is fired.</param>
        /// <param name="assetKey">The asset key identifying the visual representation of the projectile.</param>
        /// <param name="lockedTarget">The target entity that the projectile is locked onto, or null if no target is specified.</param>
        /// <param name="location">The initial location of the projectile within the game world.</param>
        public AeSpriteProjectileMunition(AeEngine engine, AeSpriteWeapon weapon, AeSpriteInteractive firedFrom, string assetKey,
             AeSpriteInteractive? lockedTarget, AeVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }
    }
}
