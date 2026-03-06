using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon.Munition._Superclass
{
    /// <summary>
    /// Projectile munitions just go straight - these are physical bullets that have no power of their own once fired.
    /// </summary>
    internal class ProjectileMunitionBase : SpriteMunition
    {
        public ProjectileMunitionBase(EngineCore engine, WeaponBase weapon, SpriteInteractive firedFrom, string assetKey,
             SpriteInteractive? lockedTarget, SiVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }
    }
}
