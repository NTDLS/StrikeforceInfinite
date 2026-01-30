using Si.Engine.AI.Logistics;
using Si.Engine.Sprite.Enemy.Boss._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using Si.Rendering;
using System.Drawing;
using System.Linq;

namespace Si.Engine.Sprite.Enemy.Boss.Devastator
{
    internal class SpriteEnemyBossDevastator : SpriteEnemyBossBase
    {
        private readonly SpriteAttachment _thrusterLeft;
        private readonly SpriteAttachment _thrusterRight;

        public SpriteEnemyBossDevastator(EngineCore engine)
            : base(engine, @"Sprites\Enemy\Boss\Devastator\Hull.png")
        {
            Orientation.Degrees = SiRandom.Between(0, 359);

            AddAIController(new AILogisticsHostileEngagement(_engine, this, _engine.Player.Sprite));

            SetCurrentAIController<AILogisticsHostileEngagement>();

            _thrusterLeft = Attachments.OfType<SpriteEnemyBossDevastatorLeftJet>().First();
            _thrusterRight = Attachments.OfType<SpriteEnemyBossDevastatorRightJet>().First();
        }

        private float TargetThrottle
        {
            get
            {
                if (_thrusterLeft.IsDeadOrExploded && _thrusterRight.IsDeadOrExploded)
                {
                    return 0.05f; // idle drift
                }
                else if (_thrusterLeft.IsDeadOrExploded || _thrusterRight.IsDeadOrExploded)
                {
                    return 0.5f;  // limp mode
                }
                else
                {
                    return 1.0f;  // full thrust
                }
            }
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            Throttle = SiMath.Damp(Throttle, TargetThrottle, 0.01f, epoch);

            var offset = this.Orientation * new SiVector(40f, 40f);

            if (_thrusterLeft.IsDeadOrExploded)
            {
                _engine.Sprites.Particles.EmitConeAt(_thrusterLeft.CalculatedLocation + offset, _thrusterLeft.CalculatedOrientation.Degrees, 15f, 2, 150f, 250f, SiRenderingUtility.GetRandomHotColor(), new Size(1, 1));
            }
            if (_thrusterRight.IsDeadOrExploded)
            {
                _engine.Sprites.Particles.EmitConeAt(_thrusterRight.CalculatedLocation + offset, _thrusterRight.CalculatedOrientation.Degrees, 15f, 2, 150f, 250f, SiRenderingUtility.GetRandomHotColor(), new Size(1, 1));
            }

            if (HullHealth <= Metadata.Hull / 2)
            {
                _engine.Sprites.Particles.ParticleBlastAt(this, SiRandom.Between(0, 1));
            }

            base.ApplyMotion(epoch, displacementVector);
        }
    }
}
