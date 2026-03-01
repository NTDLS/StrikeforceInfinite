using Si.Engine.Sprite.Weapon;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Enemy.Boss.Devastator
{
    internal class SpriteEnemyBossDevastatorLeftGun : SpriteAttachment
    {
        public SpriteEnemyBossDevastatorLeftGun(EngineCore engine, string assetKey)
            : base(engine, assetKey)
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
