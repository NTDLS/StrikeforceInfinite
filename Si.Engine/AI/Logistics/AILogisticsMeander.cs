using NTDLS.Helpers;
using Si.Engine.AI._Superclass;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library;
using Si.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Si.Engine.AI.Logistics
{
    /// <summary>
    /// Keeps an object at a generally safe distance from another object.
    /// </summary>
    internal class AILogisticsMeander : AIStateMachine
    {
        private DateTime _lastDecisionTime = DateTime.UtcNow;
        private readonly float _millisecondsBetweenDecisions = SiRandom.Between(2000, 10000);
        private float _angleToAdd = SiRandom.Between(0.004f, 0.006f);
        private readonly float _varianceAngleForTravel = SiRandom.Between(5, 15);
        private readonly float _idealMaxDistance = SiRandom.Variance(8000, 0.20f);
        private readonly float _idealMinDistance = SiRandom.Variance(2500, 0.10f);

        public AILogisticsMeander(EngineCore engine, SpriteInteractiveShipBase owner, List<SpriteBase>? observedObjects)
            : base(engine, owner, observedObjects)
        {
            OnApplyIntelligence += AILogistics_OnApplyIntelligence;
        }

        private void AILogistics_OnApplyIntelligence(float epoch, SiVector displacementVector, AIStateHandler? state)
        {
            var observedObject = ObservedObjects.First();

            var distanceToObservedObject = Owner.DistanceTo(observedObject);

            if (distanceToObservedObject > _idealMaxDistance)
            {
                if (Owner.IsPointingAt(observedObject, _varianceAngleForTravel) == false)
                {
                    Owner.RotateMovementVector(_angleToAdd * epoch);
                }
            }
            else if (distanceToObservedObject < _idealMinDistance)
            {
                if (Owner.IsPointingAway(observedObject, _varianceAngleForTravel) == false)
                {
                    Owner.RotateMovementVector(_angleToAdd * epoch);
                }
            }
            else
            {
                Owner.Orientation.RadiansSigned += (_angleToAdd * epoch); //Just do loops.

                if ((DateTime.UtcNow - _lastDecisionTime).TotalMilliseconds > _millisecondsBetweenDecisions) //Change directions from time to time.
                {
                    _angleToAdd = SiRandom.Between(0.004f, 0.006f);
                    if (SiRandom.PercentChance(50))
                    {
                        _angleToAdd *= -1;
                    }
                    _lastDecisionTime = DateTime.UtcNow;
                }
            }
        }
    }
}
