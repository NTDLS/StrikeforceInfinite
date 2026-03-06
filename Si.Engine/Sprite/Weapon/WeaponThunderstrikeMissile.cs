using Si.Engine.Sprite._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    public class WeaponThunderstrikeMissile : WeaponBase
    {
        private bool _toggle = false;

        public WeaponThunderstrikeMissile(EngineCore engine, SpriteInteractive owner, string assetKey)
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
                Engine.Sprites.Munitions.Add(this, Owner.Location + offset);

                _toggle = !_toggle;

                return true;
            }

            return false;
        }
    }
}
