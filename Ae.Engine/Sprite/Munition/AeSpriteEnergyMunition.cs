using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Interactive;

namespace Ae.Engine.Sprite.Munition
{
    /// <summary>
    /// Energy munitions just go straight - for now.... still thinking this one out.
    /// </summary>
    [AssetClass("Munition - Energy Type", "", AeBaseAssetType.Image, true)]
    public class AeSpriteEnergyMunition
        : AeSpriteMunition
    {
        /// <summary>
        /// Initializes a new instance of the AeSpriteEnergyMunition class representing an energy-based munition sprite
        /// fired from a weapon.
        /// </summary>
        /// <param name="engine">The engine instance that manages the game state and rendering for this munition.</param>
        /// <param name="weapon">The weapon that fired this energy munition.</param>
        /// <param name="firedFrom">The interactive sprite from which the munition was fired.</param>
        /// <param name="assetKey">The asset key identifying the visual representation of the munition.</param>
        /// <param name="lockedTarget">The target sprite that the munition is locked onto, or null if no target is specified.</param>
        /// <param name="location">The initial location of the munition in the game world.</param>
        public AeSpriteEnergyMunition(AeEngine engine, AeSpriteWeapon weapon, AeSpriteInteractive firedFrom, string assetKey,
             AeSpriteInteractive? lockedTarget, AeVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }
    }
}
