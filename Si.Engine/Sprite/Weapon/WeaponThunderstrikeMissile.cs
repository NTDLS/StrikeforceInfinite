using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    internal class WeaponThunderstrikeMissile : WeaponBase
    {
        private bool _toggle = false;

        public WeaponThunderstrikeMissile(EngineCore engine, SpriteInteractiveBase owner, string spritePath)
            : base(engine, owner, spritePath)
        {
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound?.Play();
                RoundQuantity--;

                var offset = Owner.Orientation.RotatedBy(90.Invert(_toggle)) * new SiVector(10, 10);
                _engine.Sprites.Munitions.Add(this, Owner.Location + offset);

                _toggle = !_toggle;

                return true;
            }

            return false;
        }
    }
}
