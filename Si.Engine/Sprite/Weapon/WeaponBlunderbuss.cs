using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Weapon
{
    public class WeaponBlunderbuss : WeaponBase
    {
        public WeaponBlunderbuss(EngineCore engine, SpriteInteractive owner, string assetKey)
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
                    for (int i = -15; i < 15; i++) // Create an initial spread so the bullets don't come from the same point.
                    {
                        var offset = Owner.Orientation.RotatedBy(90) * new SiVector(i, i);
                        Engine.Sprites.Munitions.Add(this, Owner.Location + offset);
                    }
                    RoundQuantity--;
                }

                return true;
            }
            return false;
        }
    }
}
