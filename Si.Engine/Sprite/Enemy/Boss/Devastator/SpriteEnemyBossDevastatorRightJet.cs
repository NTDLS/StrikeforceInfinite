using Si.Engine;
using Si.Engine.Sprite;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;

namespace Si.GameEngine.Sprite.Enemy.Starbase.Garrison
{
    internal class SpriteEnemyBossDevastatorRightJet : SpriteAttachment
    {
        public SpriteEnemyBossDevastatorRightJet(EngineCore engine, bool useDetachedMetadata = false)
            : base(engine, $@"Sprites\Enemy\Boss\Devastator\Jet.Right.png", useDetachedMetadata)
        {
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            Visible = !RootOwner.MovementVector.Magnitude().IsNearZero();
            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
