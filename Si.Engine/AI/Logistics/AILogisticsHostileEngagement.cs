using Si.Engine.AI._Superclass;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library;
using Si.Library.Mathematics;
using System.Collections.Generic;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.AI.Logistics
{
    //DO NOT USE WITHOUT REWRITE!!

    /// <summary>
    /// Keeps an object swooping past an object at an indirect angle.
    /// </summary>
    internal class AILogisticsHostileEngagement
        : AIStateMachine
    {
        private readonly string _boostResourceName = "AILogisticsHostileEngagement_Boost";

        public AILogisticsHostileEngagement(EngineCore engine, SpriteInteractiveShipBase owner, List<SpriteBase>? observedObjects)
            : base(engine, owner, observedObjects)
        {
            owner.OnHit += Owner_OnHit;

            Owner.RenewableResources.Create(_boostResourceName, 800, 0, 10);

            SetAIState(new GotoRadiusOfObservedObject(this));
        }

        #region AI States.

        private class GotoRadiusOfObservedObject
            : AIStateHandler
        {
            private readonly AILogisticsHostileEngagement _stateMachine;
            private SimpleDirection _rotateDirection;
            private float _rotationAngle = SiRandom.Variance(5, 0.10f);
            private readonly SiVector _targetLocation;
            private readonly SpriteBase _observedObject;

            public GotoRadiusOfObservedObject(AILogisticsHostileEngagement stateMachine)
            {
                _stateMachine = stateMachine;
                _observedObject = stateMachine.ObservedObjects.First();
                _targetLocation = _observedObject.Location.RandomAtDistance(10, 50);

                var deltaAngle = stateMachine.Owner.HeadingAngleToInSignedDegrees(_targetLocation);
                _rotateDirection = deltaAngle >= 0 ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;
            }

            public void Tick(float epoch)
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

                if (_stateMachine.Owner.RotateMovementVectorIfNotPointingAt(_observedObject, _rotationAngle, _rotateDirection, 10.0f, epoch) == false)
                {
                    _stateMachine.SetAIState(new SteadyOnCurrentPath(_stateMachine));
                }
            }
        }

        private class SteadyOnCurrentPath
            : AIStateHandler
        {
            private readonly AILogisticsHostileEngagement _stateMachine;
            private float _burndownEpochs = 3;

            public SteadyOnCurrentPath(AILogisticsHostileEngagement stateMachine)
            {
                _stateMachine = stateMachine;
            }

            public void Tick(float epoch)
            {
                //Throttle down during the steady path.
                _stateMachine.Owner.Throttle = SiMath.Damp(_stateMachine.Owner.Throttle, 0, 0.2f, epoch);

                _burndownEpochs -= epoch;

                if (_burndownEpochs <= 0)
                {
                    _stateMachine.SetAIState(new GotoRadiusOfObservedObject(_stateMachine));
                }
            }
        }

        #endregion

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
    }
}
