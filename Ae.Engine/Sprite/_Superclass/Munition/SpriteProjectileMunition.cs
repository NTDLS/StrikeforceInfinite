using Ae.Engine.Sprite._Superclass.Interactive;
using Ae.Library.Mathematics;
using Ae.Library.Metadata;
using static Ae.Library.AeConstants;

namespace Ae.Engine.Sprite._Superclass.Munition
{
    /// <summary>
    /// Projectile munitions just go straight - these are physical bullets that have no power of their own once fired.
    /// </summary>
    [AssetClass("Munition - Projectile Type", "", AeBaseAssetType.Image, true)]
    internal class SpriteProjectileMunition
        : SpriteMunition
    {
        public SpriteProjectileMunition(AeEngine engine, SpriteWeapon weapon, SpriteInteractive firedFrom, string assetKey,
             SpriteInteractive? lockedTarget, AeVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }
    }
}
