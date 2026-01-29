using Si.Engine.Sprite.Weapon;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Enemy.Boss.Garrison
{
    internal class SpriteEnemyBossDevastatorRightGun : SpriteAttachment
    {
        public SpriteEnemyBossDevastatorRightGun(EngineCore engine)
            : base(engine, $@"Sprites\Enemy\Boss\Devastator\Gun.Right.png")
        {
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (this.IsPointingAt(_engine.Player.Sprite, 10, 1000))
            {
                FireWeapon<WeaponLancer>();
            }

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
