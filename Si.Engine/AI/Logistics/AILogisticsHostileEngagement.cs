using NTDLS.Helpers;
using Si.Engine.AI._Superclass;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library;
using Si.Library.Mathematics;
using static Si.Library.SiConstants;
namespace Si.Engine.AI.Logistics
{
    /// <summary>
    /// Keeps an object swooping past an object at an indirect angle.
    /// </summary>
    internal class AILogisticsHostileEngagement : AIStateMachine
    {
        #region Instance parameters.

        private readonly string _boostResourceName = "AILogisticsHostileEngagement_Boost";

        //We use this so we can commit to an arch when banking, coming about, etc.

        #endregion

        #region AI States.

        private class GotoRadiusOfObservedObject : AIStateHandler
        {
            private readonly AILogisticsHostileEngagement _stateMachine;
            private SimpleDirection _rotateDirection;
            private float _rotationAngle = SiRandom.Variance(5, 0.10f);
            private readonly SiVector _targetLocation;
            private readonly SpriteBase _observedObject;

            public GotoRadiusOfObservedObject(AILogisticsHostileEngagement stateMachine)
            {
                _stateMachine = stateMachine;
                _observedObject = stateMachine.ObservedObject.EnsureNotNull();
                _targetLocation = _observedObject.Location.RandomAtDistance(10, 50);

                var deltaAngle = stateMachine.Owner.HeadingAngleToInSignedDegrees(_targetLocation);
                _rotateDirection = deltaAngle >= 0 ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;
            }

            public void Execute(float epoch)
            {
                if (_stateMachine.TimeInStateSeconds >= 2.5)
                {
                    _rotationAngle = SiMath.Damp(_rotationAngle, 25, decayRatePerSecond: 4.5f, epoch);
                }

                //Throttle up during the turn.
                _stateMachine.Owner.Throttle = SiMath.Damp(_stateMachine.Owner.Throttle, 1.5f, 0.2f, epoch);

                if (_rotationAngle >= 4.9f)
                {
                    //Lets just change the state....
                }

                if (_stateMachine.Owner.RotateMovementVectorIfNotPointingAt(_observedObject, _rotationAngle * epoch, _rotateDirection, 10.0f) == false)
                {
                    _stateMachine.ChangeState(new SteadyOnCurrentPath(_stateMachine));
                }
            }
        }

        private class SteadyOnCurrentPath : AIStateHandler
        {
            private readonly AILogisticsHostileEngagement _stateMachine;
            private float _burndownEpochs = 3;

            public SteadyOnCurrentPath(AILogisticsHostileEngagement stateMachine)
            {
                _stateMachine = stateMachine;
            }

            public void Execute(float epoch)
            {
                //Throttle down during the steady path.
                _stateMachine.Owner.Throttle = SiMath.Damp(_stateMachine.Owner.Throttle, 0, 0.2f, epoch);

                _burndownEpochs -= epoch;

                if (_burndownEpochs <= 0)
                {
                    _stateMachine.ChangeState(new GotoRadiusOfObservedObject(_stateMachine));
                }
            }
        }

        #endregion

        public AILogisticsHostileEngagement(EngineCore engine, SpriteInteractiveShipBase owner, SpriteBase observedObject)
            : base(engine, owner, observedObject)
        {
            owner.OnHit += Owner_OnHit;

            Owner.RenewableResources.Create(_boostResourceName, 800, 0, 10);

            ChangeState(new GotoRadiusOfObservedObject(this));
            Owner.RecalculateOrientationMovementVector();

            OnApplyIntelligence += AILogistics_OnApplyIntelligence;
        }

        private void Owner_OnHit(SpriteBase sender, SiDamageType damageType, int damageAmount)
        {
            /*
            if (sender.HullHealth <= 10)
            {
                //Do something different when we get low on health?
                ChangeState(new AIStateTransitionToEvasiveEscape(this));
            }
            */
        }

        private void AILogistics_OnApplyIntelligence(float epoch, SiVector displacementVector, AIStateHandler state)
        {
            //var deltaAngle = Owner.HeadingAngleToInUnsignedDegrees(ObservedObject);
            //Console.WriteLine($"deltaAngle {deltaAngle}");

            //var distanceToObservedObject = Owner.DistanceTo(ObservedObject);

            state.Execute(epoch);

            /*
            switch (state)
            {
                //----------------------------------------------------------------------------------------------------------------------------------------------------
                //
                case StateGoToRandomLocation approaching:
                    //Attempt to follow the observed object.

                    Owner.RotateMovementVectorIfNotPointingAt(ObservedObject, 1, ref _lastDirection, approaching.VarianceAngle);

                    if (Owner.DistanceTo(ObservedObject) < _idealMinDistance)
                    {
                        ChangeState(new AIStateTransitionToDepart());
                    }

                    break;
                //----------------------------------------------------------------------------------------------------------------------------------------------------

                //----------------------------------------------------------------------------------------------------------------------------------------------------
                default:
                    throw new Exception($"Unknown AI state: {state.GetType()}");
            }
            */
        }
    }
}
