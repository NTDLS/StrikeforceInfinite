using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite._Superclass._Root
{
    public partial class SpriteBase
    {
        /// <summary>
        /// Instantly rotates this objects movement vector by the given radians and then recalculates the Orientation.
        /// </summary>
        public void RotatePointingDirection(float radians)
        {
            Orientation.Rotate(radians);
        }

        /// <summary>
        /// Instantly rotates this objects movement vector by the given radians and then recalculates the Orientation.
        /// </summary>
        public void RotateMovementVector(float radians)
        {
            OrientationMovementVector.Rotate(radians);
            Orientation.Radians = OrientationMovementVector.OrientationInRadians();
        }

        /// <summary>
        /// Instantly points a sprite at another by rotating the movement vector and then recalculates the Orientation.
        /// </summary>
        public void RotateMovementVector(SiVector toLocationOf)
        {
            var radians = Location.AngleToInSignedRadians(toLocationOf);

            OrientationMovementVector.SetDirectionMaintainMagnitude(radians);
            Orientation.Radians = OrientationMovementVector.OrientationInRadians();
        }

        /// <summary>
        /// Rotates the objects movement vector by the specified amount if it not pointing at the target
        ///     angle (with given tolerance) then recalculates Orientation.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if object is already in the specified range.</returns>
        public bool RotateMovementVectorIfNotPointingAt(SpriteBase obj, float rotationAmountDegrees, SimpleDirection simpleDirection, float varianceDegrees = 10)
        {
            var deltaAngle = this.HeadingAngleToInSignedDegrees(obj);

            if (Math.Abs(deltaAngle) > varianceDegrees)
            {
                if (simpleDirection == SimpleDirection.CounterClockwise)
                {
                    RotateMovementVector(-SiMath.DegToRad(rotationAmountDegrees));
                }
                else if (simpleDirection == SimpleDirection.Clockwise)
                {
                    RotateMovementVector(SiMath.DegToRad(rotationAmountDegrees));
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
        public bool RotateMovementVectorIfNotPointingAt(SiVector toLocation, float rotationAmountDegrees, SimpleDirection simpleDirection, float varianceDegrees = 10)
        {
            var deltaAngle = this.HeadingAngleToInSignedDegrees(toLocation);

            if (Math.Abs(deltaAngle) > varianceDegrees)
            {
                if (simpleDirection == SimpleDirection.CounterClockwise)
                {
                    RotateMovementVector(-SiMath.DegToRad(rotationAmountDegrees));
                }
                else if (simpleDirection == SimpleDirection.Clockwise)
                {
                    RotateMovementVector(SiMath.DegToRad(rotationAmountDegrees));
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
        public bool RotateMovementVectorIfNotPointingAt(float toDegrees, float rotationAmountDegrees, float tolerance = 10)
        {
            toDegrees = toDegrees.DenormalizeDegrees();

            if (Orientation.Degrees.IsBetween(toDegrees - tolerance, toDegrees + tolerance) == false)
            {
                RotateMovementVector(-SiMath.DegToRad(rotationAmountDegrees));

                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the objects movement vector by the given amount if it is pointing in the given direction then recalculates the Orientation.
        /// </summary>
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if the object is not pointing in the given direction.
        public bool RotateMovementVectorIfPointingAt(SpriteBase obj, float rotationAmountDegrees, SimpleDirection simpleDirection, float varianceDegrees = 10)
        {
            var deltaAngle = this.HeadingAngleToInSignedDegrees(obj);

            if (deltaAngle.IsNotBetween(0, varianceDegrees))
            {
                if (simpleDirection == SimpleDirection.CounterClockwise)
                {
                    RotateMovementVector(-SiMath.DegToRad(rotationAmountDegrees));
                }
                else if (simpleDirection == SimpleDirection.Clockwise)
                {
                    RotateMovementVector(SiMath.DegToRad(rotationAmountDegrees));
                }

                RecalculateOrientationMovementVector();

                return true;
            }

            return false;
        }
    }
}
