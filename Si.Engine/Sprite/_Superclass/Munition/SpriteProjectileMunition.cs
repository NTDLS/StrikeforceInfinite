using Si.Engine.Sprite._Superclass.Interactive;
using Si.Library.Mathematics;
using Si.Library.Metadata;

namespace Si.Engine.Sprite._Superclass.Munition
{
    /// <summary>
    /// Projectile munitions just go straight - these are physical bullets that have no power of their own once fired.
    /// </summary>
    [AssetCategory("Munition - Projectile Type", "", true)]
    internal class SpriteProjectileMunition
        : SpriteMunition
    {
        public SpriteProjectileMunition(SiEngine engine, SpriteWeapon weapon, SpriteInteractive firedFrom, string assetKey,
             SpriteInteractive? lockedTarget, SiVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }
    }
}
