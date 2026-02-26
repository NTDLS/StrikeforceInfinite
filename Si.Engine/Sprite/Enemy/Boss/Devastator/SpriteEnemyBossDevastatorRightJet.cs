using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Enemy.Boss.Devastator
{
    internal class SpriteEnemyBossDevastatorRightJet : SpriteAttachment
    {
        public SpriteEnemyBossDevastatorRightJet(EngineCore engine, string spritePath)
            : base(engine, spritePath)
        {
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            IsVisible = !RootOwner.MovementVector.Magnitude().IsNearZero();
            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
