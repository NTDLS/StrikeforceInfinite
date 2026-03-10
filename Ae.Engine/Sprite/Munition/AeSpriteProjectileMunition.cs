using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Interactive;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.Munition
{
    /// <summary>
    /// Projectile munitions just go straight - these are physical bullets that have no power of their own once fired.
    /// </summary>
    [AssetClass("Munition - Projectile Type", "", AeBaseAssetType.Image, true)]
    internal class AeSpriteProjectileMunition
        : AeSpriteMunition
    {
        public AeSpriteProjectileMunition(AeEngine engine, AeSpriteWeapon weapon, AeSpriteInteractive firedFrom, string assetKey,
             AeSpriteInteractive? lockedTarget, AeVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }
    }
}
