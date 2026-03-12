using Ae.MpCommsMessages.DatagramMessages.SpriteActions;
using System;

namespace Ae.Engine.Sprite.Base
{
    /// <summary>
    /// Represents a sprite entity used in multiplayer scenarios for synchronizing actions and state between server and
    /// clients.
    /// </summary>
    /// <remarks>Use this class to manage and synchronize sprite actions in multiplay environments,
    /// particularly when operating in server host mode. Methods provided allow retrieval of current action vectors and
    /// spawn states for network synchronization, subject to update interval constraints. Intended for integration with
    /// multiplayer engines that require timely and consistent sprite state updates.</remarks>
    public partial class AeSprite
    {
        //public virtual SiSpriteActionVector GetMultiplayVector() { return null; }
        private DateTime _lastMultiplaySpriteVectorUpdate = DateTime.MinValue;

        /// <summary>
        /// Retrieves the current action vector for multiplay scenarios if the engine is operating in server host mode
        /// and the update interval has elapsed.
        /// </summary>
        /// <remarks>This method only returns a new action vector when called in server host mode and at
        /// least 5 milliseconds have passed since the last update. Otherwise, it returns null. Use this method to
        /// obtain up-to-date motion information for multiplay synchronization.</remarks>
        /// <returns>An instance of AeSpriteActionMotion containing the latest action vector data if available; otherwise, null.</returns>
        public virtual AeSpriteActionMotion? GetMultiPlayActionVector()
        {
            if (Engine.ExecutionMode == AeEngineExecutionMode.ServerHost)
            {
                if ((DateTime.UtcNow - _lastMultiplaySpriteVectorUpdate).TotalMilliseconds >= 5)
                {
                    _lastMultiplaySpriteVectorUpdate = DateTime.UtcNow;

                    return new AeSpriteActionMotion(UID)
                    {
                        X = X,
                        Y = Y,
                        OrientationDegreesSigned = Orientation.DegreesSigned,
                        //BoostPercentage = Velocity.ForwardBoostMomentum,
                        Throttle = Throttle,
                        Speed = Speed,
                        RotationSpeed = RotationSpeed
                        //Boost = ???
                    };
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a new action spawn object representing the current state for multiplayer synchronization, if the
        /// server host execution mode is active and the update interval has elapsed.
        /// </summary>
        /// <remarks>This method is intended for use in multiplayer scenarios to synchronize sprite
        /// actions from the server host. It returns null if called outside the server host execution mode or if the
        /// minimum update interval has not elapsed.</remarks>
        /// <returns>An instance of AeSpriteActionSpawn containing the current state if the method is called on the server host
        /// and the update interval has passed; otherwise, null.</returns>
        public virtual AeSpriteActionSpawn? GetMultiPlayActionSpawn()
        {
            if (Engine.ExecutionMode == AeEngineExecutionMode.ServerHost)
            {
                if ((DateTime.UtcNow - _lastMultiplaySpriteVectorUpdate).TotalMilliseconds >= 5)
                {
                    _lastMultiplaySpriteVectorUpdate = DateTime.UtcNow;

                    return new AeSpriteActionSpawn(UID, GetType().Name)
                    {
                        X = X,
                        Y = Y,
                        OrientationDegreesSigned = Orientation.DegreesSigned,
                        //BoostPercentage = Velocity.ForwardBoostMomentum,
                        Throttle = Throttle,
                        Speed = Speed,
                        RotationSpeed = RotationSpeed
                        //Boost = ???
                    };
                }
            }
            return null;
        }
    }
}
