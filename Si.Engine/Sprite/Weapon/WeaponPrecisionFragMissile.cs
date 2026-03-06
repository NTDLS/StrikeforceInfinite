using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass.Interactive;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System.Linq;

namespace Si.Engine.Sprite.Weapon
{
    public class WeaponPrecisionFragMissile : SpriteWeapon
    {
        private bool _toggle = false;

        public WeaponPrecisionFragMissile(EngineCore engine, SpriteInteractive owner, string assetKey)
            : base(engine, owner, assetKey)
        {
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound?.Play();
                RoundQuantity--;

                var offset = Owner.Orientation.RotatedBy(90.Invert(_toggle)) * new SiVector(10, 10);

                _toggle = !_toggle;

                if (LockedTargets?.Count > 0)
                {
                    foreach (var weaponLock in LockedTargets.Where(o => o.LockType == Library.SiConstants.SiWeaponsLockType.Hard))
                    {
                        Engine.Sprites.Munitions.AddLockedOnTo(this, weaponLock.Sprite, Owner.Location + offset);
                    }
                }
                else
                {
                    Engine.Sprites.Munitions.Add(this, Owner.Location + offset);
                }

                return true;
            }
            return false;
        }
    }
}
