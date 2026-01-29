using Si.Engine.AI._Superclass;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library;
using Si.Library.Mathematics;
using System;
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

        private class StateGoToRandomLocation : AIStateHandler
        {
            private readonly AILogisticsHostileEngagement _stateMachine;
            private SimpleDirection RotateDirection;

            //public float VarianceAngle = SiRandom.Variance(45, 0.2f);

            public SiVector TargetLocation { get; set; }

            public StateGoToRandomLocation(AILogisticsHostileEngagement stateMachine)
            {
                _stateMachine = stateMachine;
                TargetLocation = stateMachine.ObservedObject.Location;//.RandomAtDistance(10, 100);

                var deltaAngle = stateMachine.Owner.HeadingAngleToInSignedDegrees(TargetLocation);
                RotateDirection = deltaAngle >= 0 ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;
            }

            public void Execute(float epoch, float deltaSeconds)
            {
                if (_stateMachine.Owner.RotateMovementVectorIfNotPointingAt(_stateMachine.ObservedObject, 1 * epoch, RotateDirection, 10.0f) == false)
                {
                    _stateMachine.ChangeState(new StateGoToRandomLocation(_stateMachine));
                }
            }
        }

        #endregion

        public AILogisticsHostileEngagement(EngineCore engine, SpriteInteractiveShipBase owner, SpriteBase observedObject)
            : base(engine, owner, observedObject)
        {
            owner.OnHit += Owner_OnHit;

            Owner.RenewableResources.Create(_boostResourceName, 800, 0, 10);

            ChangeState(new StateGoToRandomLocation(this));
            Owner.RecalculateMovementVector();

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

        float burndownSeconds = 1;

        private void AILogistics_OnApplyIntelligence(float epoch, float deltaSeconds, SiVector displacementVector, AIStateHandler state)
        {
            //var deltaAngle = Owner.HeadingAngleToInUnsignedDegrees(ObservedObject);
            //Console.WriteLine($"deltaAngle {deltaAngle}");

            //var distanceToObservedObject = Owner.DistanceTo(ObservedObject);

            burndownSeconds -= deltaSeconds;

            if (burndownSeconds <= 0)
            {
                //Console.WriteLine($"Change State {epoch}");

                burndownSeconds = 1;
                //ChangeState(new StateGoToRandomLocation(this));
            }

            state.Execute(epoch, deltaSeconds);

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
