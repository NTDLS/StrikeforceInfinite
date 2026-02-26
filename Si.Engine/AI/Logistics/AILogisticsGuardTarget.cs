using Si.Engine.AI._Superclass;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library;
using Si.Library.Mathematics;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.AI.Logistics
{
    /// <summary>
    /// Keeps an object swooping past an object at an indirect angle.
    /// </summary>
    internal class AILogisticsGuardTarget
        : AIStateMachine
    {
        protected ModelParameters Parameters { get; set; }

        public class ModelParameters
        {
            public float MaxDistance { get; set; } = 200;
        }

        public AILogisticsGuardTarget(EngineCore engine, SpriteInteractiveShipBase owner, SpriteBase observedObject, ModelParameters parameters)
            : base(engine, owner, [observedObject])
        {
            SetAIState(new GoToDistance(this));
            Parameters = parameters;
        }

        /// <summary>
        /// Exit the screen at a high speed, then change state to start swooping back in.
        /// </summary>
        private class GoToDistance(AILogisticsGuardTarget stateMachine)
            : AIStateHandler
        {
            private int _lastHullHealth = stateMachine.Owner.HullHealth;
            private SimpleDirection _rotateDirection = SiRandom.FlipCoin() ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;
            private float _rotationEpochs = 0;
            private float _rotationDegreesPerSec = 0;

            public void Tick(float epoch)
            {
                stateMachine.Owner.Throttle = SiMath.Damp(stateMachine.Owner.Throttle, 2.0f, 1.0f, epoch);

                var currentDistance = stateMachine.Owner.DistanceTo(stateMachine.ObservedObjects.First());

                if (currentDistance > stateMachine.Parameters.MaxDistance)
                {
                    stateMachine.SetAIState(new RotateToObservedObject(stateMachine));
                }
                else
                {
                    if (_rotationEpochs <= 0 && stateMachine.Owner.HullHealth < _lastHullHealth)
                    {
                        _rotateDirection = SiRandom.FlipCoin() ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;
                        _rotationEpochs = SiRandom.Between(1.0f, 2.0f);
                        _rotationDegreesPerSec = SiRandom.Between(20, 50);
                    }
                    _lastHullHealth = stateMachine.Owner.HullHealth;

                    if (_rotationEpochs > 0)
                    {
                        _rotationEpochs -= epoch;
                        if (_rotateDirection == SimpleDirection.Clockwise)
                            stateMachine.Owner.RotateMovementVector(_rotationDegreesPerSec, epoch);
                        else
                            stateMachine.Owner.RotateMovementVector(-_rotationDegreesPerSec, epoch);
                    }
                }
            }
        }

        /// <summary>
        /// After exiting the screen, rotate to face the center of the screen.
        /// </summary>
        private class RotateToObservedObject(AILogisticsGuardTarget stateMachine)
            : AIStateHandler
        {
            private readonly SimpleDirection _rotateDirection = SiRandom.FlipCoin() ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;
            private readonly float _rotationDegreesPerSec = SiRandom.Between(20, 50);

            public void Tick(float epoch)
            {
                if (stateMachine.Owner.IsPointingAt(stateMachine.ObservedObjects.First(), 10.0f))
                {
                    stateMachine.SetAIState(new ApproachTarget(stateMachine));
                }
                else
                {
                    stateMachine.Owner.Throttle = SiMath.Damp(stateMachine.Owner.Throttle, 1.0f, 1.0f, epoch);

                    if (_rotateDirection == SimpleDirection.Clockwise)
                        stateMachine.Owner.RotateMovementVector(_rotationDegreesPerSec, epoch);
                    else
                        stateMachine.Owner.RotateMovementVector(-_rotationDegreesPerSec, epoch);
                }
            }
        }

        private class ApproachTarget(AILogisticsGuardTarget stateMachine)
            : AIStateHandler
        {
            private float _lastDistance = stateMachine.Owner.DistanceTo(stateMachine.ObservedObjects.First());
            private int _lastHullHealth = stateMachine.Owner.HullHealth;

            public void Tick(float epoch)
            {
                var currentDistance = stateMachine.Owner.DistanceTo(stateMachine.ObservedObjects.First());

                if (currentDistance > _lastDistance)
                {
                    stateMachine.SetAIState(new GoToDistance(stateMachine));
                }
                else
                {
                    _lastDistance = currentDistance;

                    if (stateMachine.Owner.HullHealth < _lastHullHealth - 5)
                    {
                        //If we take hits on approach, leave.
                        stateMachine.SetAIState(new GoToDistance(stateMachine));
                        return;
                    }

                    stateMachine.Owner.Throttle = SiMath.Damp(stateMachine.Owner.Throttle, 2.0f, 1.0f, epoch);
                }

                _lastHullHealth = stateMachine.Owner.HullHealth;
            }
        }
    }
}
