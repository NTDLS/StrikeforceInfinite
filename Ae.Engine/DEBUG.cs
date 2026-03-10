using Ae.Engine;
using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite;
using Ae.Engine.Sprite.Base;
using System.Linq;


public class Sprites_Weapon_Precision_Frag_Missile(AeEngine engine, SpriteBase owner, string assetKey)
    : SpriteWeapon(engine, owner, assetKey), Ae.Engine.Compiler.IAeRuntimeCompiledSpriteAsset
{
    public static string AeClassName => "Sprites_Weapon_Precision_Frag_Missile";
    public static string AeFriendlyName => "Precision Frag Missile";

    private bool _toggle = false;

    public override bool Fire()
    {
        if (CanFire)
        {
            Sounds?.OneOf()?.Play();
            MunitionQuantity--;

            var offset = Owner.Orientation.RotatedBy(90.Invert(_toggle)) * new AeVector(10, 10);

            _toggle = !_toggle;

            if (LockedTargets?.Count > 0)
            {
                foreach (var weaponLock in LockedTargets.Where(o => o.LockType == AeConstants.SiWeaponsLockType.Hard))
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