using Si.MpComms.DatagramMessages.SpriteActions;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite._Superclass._Root
{
    public partial class SpriteBase
    {
        //public virtual SiSpriteActionVector GetMultiplayVector() { return null; }
        private DateTime _lastMultiplaySpriteVectorUpdate = DateTime.MinValue;

        public virtual SiSpriteActionVector? GetMultiPlayActionVector()
        {
            if (_engine.ExecutionMode == SiEngineExecutionMode.ServerHost)
            {
                if ((DateTime.UtcNow - _lastMultiplaySpriteVectorUpdate).TotalMilliseconds >= 5)
                {
                    _lastMultiplaySpriteVectorUpdate = DateTime.UtcNow;

                    return new SiSpriteActionVector(UID)
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

        public virtual SiSpriteActionSpawn? GetMultiPlayActionSpawn()
        {
            if (_engine.ExecutionMode == SiEngineExecutionMode.ServerHost)
            {
                if ((DateTime.UtcNow - _lastMultiplaySpriteVectorUpdate).TotalMilliseconds >= 5)
                {
                    _lastMultiplaySpriteVectorUpdate = DateTime.UtcNow;

                    return new SiSpriteActionSpawn(UID, GetType().Name)
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
