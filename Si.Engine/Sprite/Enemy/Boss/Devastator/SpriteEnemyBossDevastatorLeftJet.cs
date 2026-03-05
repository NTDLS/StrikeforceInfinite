using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Enemy.Boss.Devastator
{
    internal class SpriteEnemyBossDevastatorLeftJet : SpriteAttachment
    {
        public SpriteEnemyBossDevastatorLeftJet(EngineCore engine, string assetKey)
            : base(engine, assetKey)
        {
        }

        public override void ApplyIntelligence(float epoch, SiVector cameraDisplacement)
        {
            IsVisible = !RootOwner.MovementVector.Magnitude().IsNearZero();
            base.ApplyIntelligence(epoch, cameraDisplacement);
        }
    }
}
