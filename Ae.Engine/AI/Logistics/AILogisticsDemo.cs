using Ae.Engine.AI._Superclass;
using Ae.Engine.Sprite._Superclass.Interactive.Ship;
using Ae.Library;
using Ae.Library.Mathematics;
using System.Linq;
using static Ae.Library.AeConstants;

namespace Ae.Engine.AI.Logistics
{
    /// <summary>
    /// Keeps an object swooping past an object at an indirect angle.
    /// </summary>
    public class AILogisticsDemo
        : AIStateMachine
    {
        private float _explodeCooldown = AeRandom.Between(5, 10);
        private readonly bool _doExplosions = false;

        public AILogisticsDemo(AeEngine engine, SpriteShip owner)
            : base(engine, owner, observedObjects: null)
        {
            SetAIState(new ExitScreen(this));
            OnApplyIntelligence += AILogisticsDemo_OnApplyIntelligence;
        }

        private void AILogisticsDemo_OnApplyIntelligence(float epoch, AeVector cameraDisplacement, AIStateHandler? state)
        {
            if (_doExplosions)
            {
                _explodeCooldown -= epoch;
                if (_explodeCooldown <= 0f)
                {
                    _explodeCooldown = AeRandom.Between(2.0f, 5.0f);

                    if (Owner.IsWithinCurrentScaledScreenBounds)
                    {
                        var attachments = Owner.Attachments.Where(a => !a.IsDeadOrExploded).ToList();
                        attachments.OneOfNullable()?.Explode();

                        if (!Owner.IsDeadOrExploded && Owner.IsVisible && attachments.Count == 0)
                            Owner.Explode();
                    }
                }
            }
        }

        private class FollowRandomShip
            : AIStateHandler
        {
            private readonly AILogisticsDemo _stateMachine;
            private readonly SpriteEnemy? _followSprite;
            private readonly SimpleDirection _rotateDirection = AeRandom.FlipCoin() ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;

            public FollowRandomShip(AILogisticsDemo stateMachine)
            {
                _stateMachine = stateMachine;
                _followSprite = _stateMachine.Engine.Sprites.Enemies.Visible().OneOfNullable();
            }

            public void Tick(float epoch)
            {
                if (_followSprite == null)
                {
                    _stateMachine.SetAIState(new ExitScreen(_stateMachine));
                    return;
                }
                else if (_stateMachine.Owner.IsWithinCurrentScaledScreenBounds)
                {
                    _stateMachine.Owner.RotateMovementVectorIfNotPointingAt(_followSprite, 10.0f, _rotateDirection, 10, epoch);
                }
                else
                {
                    _stateMachine.SetAIState(new RotateToCenterScene(_stateMachine));
                }
            }
        }


        /// <summary>
        /// Exit the screen at a high speed, then change state to start swooping back in.
        /// </summary>
        private class ExitScreen(AILogisticsDemo stateMachine)
            : AIStateHandler
        {
            public void Tick(float epoch)
            {
                stateMachine.Owner.Throttle = AeMath.Damp(stateMachine.Owner.Throttle, 3.0f, 1.0f, epoch);

                if (stateMachine.Owner.IsWithinCurrentScaledScreenBounds == false)
                {
                    stateMachine.SetAIState(new RotateToCenterScene(stateMachine));
                }
            }
        }

        /// <summary>
        /// After exiting the screen, rotate to face the center of the screen.
        /// </summary>
        private class RotateToCenterScene(AILogisticsDemo stateMachine)
            : AIStateHandler
        {
            private readonly SimpleDirection _rotateDirection = AeRandom.FlipCoin() ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;

            public void Tick(float epoch)
            {
                if (stateMachine.Owner.IsPointingAt(stateMachine.Engine.Display.CenterOfCurrentScreen, 10.0f))
                {
                    stateMachine.SetAIState(new ApproachTarget(stateMachine));
                }
                else
                {
                    stateMachine.Owner.Throttle = AeMath.Damp(stateMachine.Owner.Throttle, 1.0f, 1.0f, epoch);

                    if (_rotateDirection == SimpleDirection.Clockwise)
                        stateMachine.Owner.RotateMovementVector(45, epoch);
                    else
                        stateMachine.Owner.RotateMovementVector(-45, epoch);
                }
            }
        }

        private class ApproachTarget(AILogisticsDemo stateMachine)
            : AIStateHandler
        {
            private readonly SimpleDirection _rotateDirection = AeRandom.FlipCoin() ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;
            private float _lastDistance = stateMachine.Owner.DistanceTo(stateMachine.Engine.Display.CenterOfCurrentScreen);

            public void Tick(float epoch)
            {
                var currentDistance = stateMachine.Owner.DistanceTo(stateMachine.Engine.Display.CenterOfCurrentScreen);

                if (currentDistance > _lastDistance)
                {
                    stateMachine.SetAIState(new ExitScreen(stateMachine));
                }
                else
                {
                    _lastDistance = currentDistance;

                    stateMachine.Owner.Throttle = AeMath.Damp(stateMachine.Owner.Throttle, 2.0f, 1.0f, epoch);

                    if (_rotateDirection == SimpleDirection.Clockwise)
                        stateMachine.Owner.RotateMovementVector(1f, epoch);
                    else
                        stateMachine.Owner.RotateMovementVector(-1f, epoch);
                }
            }
        }
    }
}
