using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponDualVulcanCannon : WeaponBase
    {
        public WeaponDualVulcanCannon(EngineCore engine, SpriteInteractiveBase owner, string spritePath)
            : base(engine, owner, spritePath)
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
                    _engine.Sprites.Munitions.Add(this, Owner.Location + offsetRight);

                    var offsetLeft = Owner.Orientation.RotatedBy(-90) * new SiVector(5, 5);
                    _engine.Sprites.Munitions.Add(this, Owner.Location + offsetLeft);
                }

                RoundQuantity--;

                return true;
            }
            return false;
        }
    }
}
