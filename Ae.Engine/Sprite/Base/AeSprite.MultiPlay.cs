using Ae.MpCommsMessages.DatagramMessages.SpriteActions;
using System;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.Base
{
    public partial class AeSprite
    {
        //public virtual SiSpriteActionVector GetMultiplayVector() { return null; }
        private DateTime _lastMultiplaySpriteVectorUpdate = DateTime.MinValue;

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
