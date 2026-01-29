using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.Engine.Sprite.Enemy.Boss.Devastator
{
    internal class SpriteEnemyBossDevastatorRightJet : SpriteAttachment
    {
        public SpriteEnemyBossDevastatorRightJet(EngineCore engine)
            : base(engine, $@"Sprites\Enemy\Boss\Devastator\Jet.Right.png")
        {
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            Visible = !RootOwner.OrientationMovementVector.Magnitude().IsNearZero();
            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
