using Si.Engine.Sprite.Weapon;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Enemy.Boss.Devastator
{
    internal class SpriteEnemyBossDevastatorLeftCannon : SpriteAttachment
    {
        public SpriteEnemyBossDevastatorLeftCannon(EngineCore engine)
            : base(engine, $@"Sprites\Enemy\Boss\Devastator\Cannon.Left.png")
        {
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (this.IsPointingAt(_engine.Player.Sprite, 10, 1000))
            {
                FireWeapon<WeaponVulcanCannon>();
            }

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
