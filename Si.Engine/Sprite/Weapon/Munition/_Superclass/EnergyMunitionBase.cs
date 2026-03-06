using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon.Munition._Superclass
{
    /// <summary>
    /// Energy munitions just go straight - for now.... still thinking this one out.
    /// </summary>
    internal class EnergyMunitionBase : SpriteMunition
    {
        public EnergyMunitionBase(EngineCore engine, WeaponBase weapon, SpriteInteractive firedFrom, string assetKey,
             SpriteInteractive? lockedTarget, SiVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }
    }
}
