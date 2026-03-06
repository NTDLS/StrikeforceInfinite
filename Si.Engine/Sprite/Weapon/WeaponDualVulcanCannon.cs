using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass.Interactive;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    public class WeaponDualVulcanCannon : SpriteWeapon
    {
        public WeaponDualVulcanCannon(EngineCore engine, SpriteInteractive owner, string assetKey)
            : base(engine, owner, assetKey)
        {
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound?.Play();

                if (RoundQuantity > 0)
                {
                    var offsetRight = Owner.Orientation.RotatedBy(90) * new SiVector(5, 5);
                    Engine.Sprites.Munitions.Add(this, Owner.Location + offsetRight);

                    var offsetLeft = Owner.Orientation.RotatedBy(-90) * new SiVector(5, 5);
                    Engine.Sprites.Munitions.Add(this, Owner.Location + offsetLeft);
                }

                RoundQuantity--;

                return true;
            }
            return false;
        }
    }
}
