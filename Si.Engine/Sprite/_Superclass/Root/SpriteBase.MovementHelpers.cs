using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite._Superclass._Root
{
    public partial class SpriteBase
    {
        /// <summary>
        /// Instantly rotates this objects orientation by the given degrees.
        /// </summary>
        public void RotateOrientation(float degrees, float epoch)
        {
            Orientation.Rotate(degrees.ToRadians() * epoch);
        }

        /// <summary>
        /// Instantly rotates this objects movement vector by the given degrees and then recalculates the movement vector.
        /// </summary>
        public void RotateMovementVector(float degrees, float epoch)
        {
            Orientation.Rotate(degrees.ToRadians() * epoch);
            RecalculateMovementVectorFromOrientation();
        }

        /// <summary>
        /// Rotates the objects movement vector by the specified amount if it not pointing at the target
        ///     angle (with given tolerance) then recalculates Orientation.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object is already in the specified range.</returns>
        public bool RotateMovementVectorIfNotPointingAt(SpriteBase obj, float rotationDegreesPerSecond, SimpleDirection simpleDirection, float varianceDegrees, float epoch)
        {
            var deltaAngle = this.HeadingAngleToInSignedDegrees(obj);

            if (Math.Abs(deltaAngle) > varianceDegrees)
            {
                if (simpleDirection == SimpleDirection.CounterClockwise)
                {
                    RotateMovementVector(-rotationDegreesPerSecond, epoch);
                }
                else if (simpleDirection == SimpleDirection.Clockwise)
                {
                    RotateMovementVector(rotationDegreesPerSecond, epoch);
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// Rotates the objects movement vector by the specified amount if it not pointing at the target
        /// angle (with given tolerance) then recalculates the Orientation.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object is already in the specified range.</returns>
        public bool RotateMovementVectorIfNotPointingAt(SiVector toLocation, float rotationAmountDegrees, SimpleDirection simpleDirection, float varianceDegrees, float epoch)
        {
            var deltaAngle = this.HeadingAngleToInSignedDegrees(toLocation);

            if (Math.Abs(deltaAngle) > varianceDegrees)
            {
                if (simpleDirection == SimpleDirection.CounterClockwise)
                {
                    RotateMovementVector(-rotationAmountDegrees, epoch);
                }
                else if (simpleDirection == SimpleDirection.Clockwise)
                {
                    RotateMovementVector(+rotationAmountDegrees, epoch);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the objects movement vector by the specified amount if it not pointing at the target angle
        /// (with given tolerance) then recalculates the Orientation.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object is already in the specified range.</returns>
        public bool RotateMovementVectorIfNotPointingAt(float toDegrees, float rotationAmountDegrees, SimpleDirection simpleDirection, float tolerance, float epoch)
        {
            toDegrees = toDegrees.DenormalizeDegrees();

            if (Orientation.Degrees.IsBetween(toDegrees - tolerance, toDegrees + tolerance) == false)
            {
                if (simpleDirection == SimpleDirection.CounterClockwise)
                {
                    RotateMovementVector(-rotationAmountDegrees, epoch);
                }
                else if (simpleDirection == SimpleDirection.Clockwise)
                {
                    RotateMovementVector(+rotationAmountDegrees, epoch);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the objects movement vector by the given amount if it is pointing in the given direction then recalculates the Orientation.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if the object is not pointing in the given direction.
        public bool RotateMovementVectorIfPointingAt(SpriteBase obj, float rotationAmountDegrees, SimpleDirection simpleDirection, float varianceDegrees, float epoch)
        {
            var deltaAngle = this.HeadingAngleToInSignedDegrees(obj);

            if (deltaAngle.IsNotBetween(0, varianceDegrees))
            {
                if (simpleDirection == SimpleDirection.CounterClockwise)
                {
                    RotateMovementVector(-rotationAmountDegrees, epoch);
                }
                else if (simpleDirection == SimpleDirection.Clockwise)
                {
                    RotateMovementVector(+rotationAmountDegrees, epoch);
                }

                return true;
            }

            return false;
        }
    }
}
