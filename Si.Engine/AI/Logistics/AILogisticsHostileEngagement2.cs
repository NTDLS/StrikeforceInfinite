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
    /// Hostile engagement behavior:
    /// - If too far from the player: turn toward player and close distance.
    /// - If within a preferred band: strafe/swoop around player (keeps motion indirect).
    /// - If too close or taking hits: break away, then re-commit.
    ///
    /// observedObject is expected to be the player sprite.
    /// </summary>
    internal class AILogisticsHostileEngagement2
        : AIStateMachine
    {
        protected ModelParameters Parameters { get; set; }

        public class ModelParameters
        {
            /// <summary>Distance at which we start re-orienting/closing (beyond this, we re-engage).</summary>
            public float EngageDistance { get; set; } = 450;

            /// <summary>Preferred "fight ring" distance we try to maintain.</summary>
            public float PreferredDistance { get; set; } = 250;

            /// <summary>Distance considered "too close" where we should break away.</summary>
            public float BreakAwayDistance { get; set; } = 120;

            /// <summary>Degrees tolerance to consider "pointing at" the target.</summary>
            public float AimToleranceDegrees { get; set; } = 10;

            /// <summary>How aggressively we rotate (deg/sec random range).</summary>
            public float MinRotateDegPerSec { get; set; } = 25;
            public float MaxRotateDegPerSec { get; set; } = 70;

            /// <summary>Strafe/swoop rotation burst duration range (seconds).</summary>
            public float MinStrafeSeconds { get; set; } = 0.5f;
            public float MaxStrafeSeconds { get; set; } = 1.6f;

            /// <summary>How often (and how much) damage triggers evasive behavior.</summary>
            public int DamageEvasiveThreshold { get; set; } = 5;

            /// <summary>Seconds to stay in break-away before trying to re-acquire.</summary>
            public float BreakAwaySeconds { get; set; } = 1.5f;
        }

        public AILogisticsHostileEngagement2(
            EngineCore engine,
            SpriteInteractiveShipBase owner,
            SpriteBase observedObject,
            ModelParameters parameters)
            : base(engine, owner, [observedObject])
        {
            Parameters = parameters ?? new ModelParameters();
            SetAIState(new AcquireTarget(this));
        }

        private SpriteBase Target => ObservedObjects.First();

        /// <summary>
        /// Ensure we're oriented toward the target enough to start closing distance.
        /// </summary>
        private class AcquireTarget(AILogisticsHostileEngagement2 stateMachine)
            : AIStateHandler
        {
            private readonly float _rotationDegreesPerSec =
                SiRandom.Between(stateMachine.Parameters.MinRotateDegPerSec, stateMachine.Parameters.MaxRotateDegPerSec);

            private SimpleDirection _rotateDirection = SiRandom.FlipCoin() ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;

            public void Tick(float epoch)
            {
                // If we have a distance band already, we can decide state immediately.
                var dist = stateMachine.Owner.DistanceTo(stateMachine.Target);

                if (dist <= stateMachine.Parameters.BreakAwayDistance)
                {
                    stateMachine.SetAIState(new BreakAway(stateMachine));
                    return;
                }

                // If we are already roughly pointing at the player, start closing.
                if (stateMachine.Owner.IsPointingAt(stateMachine.Target, stateMachine.Parameters.AimToleranceDegrees))
                {
                    stateMachine.SetAIState(new CloseDistance(stateMachine));
                    return;
                }

                // Rotate (current pattern matches your existing style).
                stateMachine.Owner.Throttle = SiMath.Damp(stateMachine.Owner.Throttle, 0.8f, 1.0f, epoch);

                if (_rotateDirection == SimpleDirection.Clockwise)
                    stateMachine.Owner.RotateMovementVector(_rotationDegreesPerSec, epoch);
                else
                    stateMachine.Owner.RotateMovementVector(-_rotationDegreesPerSec, epoch);

                // Occasionally flip direction so we don't get stuck in a bad orbit if your "pointing" != "movement".
                // (Keeps behavior similar to your other logistics AIs.)
                if (SiRandom.PercentChance(2))
                    _rotateDirection = (_rotateDirection == SimpleDirection.Clockwise) ? SimpleDirection.CounterClockwise : SimpleDirection.Clockwise;
            }
        }

        /// <summary>
        /// Close the gap to the preferred combat distance.
        /// </summary>
        private class CloseDistance(AILogisticsHostileEngagement2 stateMachine)
            : AIStateHandler
        {
            private int _lastHullHealth = stateMachine.Owner.HullHealth;

            public void Tick(float epoch)
            {
                var dist = stateMachine.Owner.DistanceTo(stateMachine.Target);

                // Too close => break away.
                if (dist <= stateMachine.Parameters.BreakAwayDistance)
                {
                    stateMachine.SetAIState(new BreakAway(stateMachine));
                    return;
                }

                // If we took a noticeable hit while charging in, break off.
                if (stateMachine.Owner.HullHealth < _lastHullHealth - stateMachine.Parameters.DamageEvasiveThreshold)
                {
                    stateMachine.SetAIState(new BreakAway(stateMachine));
                    return;
                }

                // If we're far, keep closing; if we're in the ring, switch to strafe/orbit.
                if (dist > stateMachine.Parameters.PreferredDistance)
                {
                    stateMachine.Owner.Throttle = SiMath.Damp(stateMachine.Owner.Throttle, 2.0f, 1.0f, epoch);

                    // If we've drifted off the aim, reacquire rather than blindly thrusting.
                    if (!stateMachine.Owner.IsPointingAt(stateMachine.Target, stateMachine.Parameters.AimToleranceDegrees))
                        stateMachine.SetAIState(new AcquireTarget(stateMachine));
                }
                else
                {
                    stateMachine.SetAIState(new StrafeRing(stateMachine));
                }

                _lastHullHealth = stateMachine.Owner.HullHealth;
            }
        }

        /// <summary>
        /// Swoop/strafe around the target at an indirect angle while trying to stay near PreferredDistance.
        /// This creates the "hostile engagement" feel without simple straight-line ramming.
        /// </summary>
        private class StrafeRing(AILogisticsHostileEngagement2 stateMachine)
            : AIStateHandler
        {
            private int _lastHullHealth = stateMachine.Owner.HullHealth;

            private SimpleDirection _strafeDirection = SiRandom.FlipCoin() ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;
            private float _strafeEpochs = 0;
            private float _rotationDegreesPerSec = 0;

            public void Tick(float epoch)
            {
                var dist = stateMachine.Owner.DistanceTo(stateMachine.Target);

                // If too far, re-engage.
                if (dist >= stateMachine.Parameters.EngageDistance)
                {
                    stateMachine.SetAIState(new AcquireTarget(stateMachine));
                    return;
                }

                // If too close, break away.
                if (dist <= stateMachine.Parameters.BreakAwayDistance)
                {
                    stateMachine.SetAIState(new BreakAway(stateMachine));
                    return;
                }

                // If taking hits, increase evasiveness / break away.
                if (stateMachine.Owner.HullHealth < _lastHullHealth - stateMachine.Parameters.DamageEvasiveThreshold)
                {
                    stateMachine.SetAIState(new BreakAway(stateMachine));
                    return;
                }

                // Maintain speed in the fight ring.
                stateMachine.Owner.Throttle = SiMath.Damp(stateMachine.Owner.Throttle, 1.6f, 1.0f, epoch);

                // Start / refresh a strafe burst occasionally or on small damage.
                if (_strafeEpochs <= 0)
                {
                    _strafeDirection = SiRandom.FlipCoin() ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;
                    _strafeEpochs = SiRandom.Between(stateMachine.Parameters.MinStrafeSeconds, stateMachine.Parameters.MaxStrafeSeconds);
                    _rotationDegreesPerSec = SiRandom.Between(stateMachine.Parameters.MinRotateDegPerSec, stateMachine.Parameters.MaxRotateDegPerSec);
                }

                _strafeEpochs -= epoch;
                if (_strafeDirection == SimpleDirection.Clockwise)
                    stateMachine.Owner.RotateMovementVector(_rotationDegreesPerSec, epoch);
                else
                    stateMachine.Owner.RotateMovementVector(-_rotationDegreesPerSec, epoch);

                // Optional: if we drift outside the preferred ring, bias toward reacquiring/closing.
                // (This avoids long-term spirals outward.)
                if (dist > stateMachine.Parameters.PreferredDistance * 1.25f &&
                    !stateMachine.Owner.IsPointingAt(stateMachine.Target, stateMachine.Parameters.AimToleranceDegrees))
                {
                    stateMachine.SetAIState(new AcquireTarget(stateMachine));
                    return;
                }

                _lastHullHealth = stateMachine.Owner.HullHealth;
            }
        }

        /// <summary>
        /// Break away for a short duration, then re-acquire and re-engage.
        /// This is the "I'm under pressure / too close" escape valve.
        /// </summary>
        private class BreakAway(AILogisticsHostileEngagement2 stateMachine)
            : AIStateHandler
        {
            private float _remaining = stateMachine.Parameters.BreakAwaySeconds;

            private readonly SimpleDirection _rotateDirection =
                SiRandom.FlipCoin() ? SimpleDirection.Clockwise : SimpleDirection.CounterClockwise;

            private readonly float _rotationDegreesPerSec =
                SiRandom.Between(stateMachine.Parameters.MinRotateDegPerSec, stateMachine.Parameters.MaxRotateDegPerSec);

            public void Tick(float epoch)
            {
                // Push hard while breaking away.
                stateMachine.Owner.Throttle = SiMath.Damp(stateMachine.Owner.Throttle, 2.2f, 1.0f, epoch);

                // Spiral out a bit (keeps it from just backing straight out).
                if (_rotateDirection == SimpleDirection.Clockwise)
                    stateMachine.Owner.RotateMovementVector(_rotationDegreesPerSec, epoch);
                else
                    stateMachine.Owner.RotateMovementVector(-_rotationDegreesPerSec, epoch);

                _remaining -= epoch;
                if (_remaining <= 0)
                {
                    stateMachine.SetAIState(new AcquireTarget(stateMachine));
                }
            }
        }
    }
}