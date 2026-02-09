using Si.Engine.AI._Superclass;
using Si.Engine.Sprite._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using static Si.Library.SiConstants;

namespace Si.Engine.AI.Logistics
{
    /// <summary>
    /// Keeps an object swooping past an object at an indirect angle.
    /// </summary>
    internal class AILogisticsOffScreenReentry
        : AIStateMachine
    {
        public AILogisticsOffScreenReentry(EngineCore engine, SpriteInteractiveShipBase owner)
            : base(engine, owner, null)
        {
            SetAIState(new ExitScreen(this));
        }

        /// <summary>
        /// Exit the screen at a high speed, then change state to start swooping back in.
        /// </summary>
        private class ExitScreen(AILogisticsOffScreenReentry stateMachine)
            : AIStateHandler
        {
            public void Execute(float epoch)
            {
                stateMachine.Owner.Throttle = SiMath.Damp(stateMachine.Owner.Throttle, 3.0f, 1.0f, epoch);

                if (stateMachine.Owner.IsWithinCurrentScaledScreenBounds == false)
                {
                    stateMachine.SetAIState(new RotateToCenterScene(stateMachine));
                }
            }
        }

        /// <summary>
        /// After exiting the screen, rotate to face the center of the screen.
        /// </summary>
        private class RotateToCenterScene(AILogisticsOffScreenReentry stateMachine)
            : AIStateHandler
        {
            private SimpleDirection _rotateDirection = SiRandom.FlipCoin() ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;

            public void Execute(float epoch)
            {
                if (stateMachine.Owner.IsPointingAt(stateMachine.Engine.Display.CenterOfCurrentScreen, 10.0f))
                {
                    stateMachine.SetAIState(new ApproachTarget(stateMachine));
                }
                else
                {
                    stateMachine.Owner.Throttle = SiMath.Damp(stateMachine.Owner.Throttle, 1.0f, 1.0f, epoch);

                    if (_rotateDirection == SimpleDirection.Clockwise)
                        stateMachine.Owner.Orientation += 45f * epoch;
                    else
                        stateMachine.Owner.Orientation -= 45f * epoch;
                }
            }
        }

        private class ApproachTarget(AILogisticsOffScreenReentry stateMachine)
            : AIStateHandler
        {
            private readonly SimpleDirection _rotateDirection = SiRandom.FlipCoin() ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;
            private float _lastDistance = stateMachine.Owner.DistanceTo(stateMachine.Engine.Display.CenterOfCurrentScreen);

            public void Execute(float epoch)
            {
                var currentDistance = stateMachine.Owner.DistanceTo(stateMachine.Engine.Display.CenterOfCurrentScreen);

                if (currentDistance > _lastDistance)
                {
                    stateMachine.SetAIState(new ExitScreen(stateMachine));
                }
                else
                {
                    _lastDistance = currentDistance;

                    stateMachine.Owner.Throttle = SiMath.Damp(stateMachine.Owner.Throttle, 2.0f, 1.0f, epoch);

                    if (_rotateDirection == SimpleDirection.Clockwise)
                        stateMachine.Owner.Orientation += 1f * epoch;
                    else
                        stateMachine.Owner.Orientation -= 1f * epoch;
                }
            }
        }
    }
}
