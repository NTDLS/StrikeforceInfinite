using Ae.Engine.ExtensionMethods;
using Ae.Engine.Mathematics;
using System;

namespace Ae.Engine.Sprite.Base
{
    /// <summary>
    /// Represents a sprite with orientation and movement vector manipulation capabilities, providing methods to rotate
    /// its orientation and movement vector based on various criteria.
    /// </summary>
    /// <remarks>AeSprite enables precise control over a sprite's direction and movement by offering rotation
    /// methods that can be triggered conditionally or instantly. These methods are useful for implementing behaviors
    /// such as steering, targeting, or aligning movement in 2D simulations or games. Thread safety and performance
    /// considerations depend on the underlying implementation of orientation and movement vector updates.</remarks>
    public partial class AeSprite
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
        public bool RotateMovementVectorIfNotPointingAt(AeSprite obj, float rotationDegreesPerSecond, AeRotationDirection simpleDirection, float varianceDegrees, float epoch)
        {
            var deltaAngle = this.HeadingAngleToInSignedDegrees(obj);

            if (Math.Abs(deltaAngle) > varianceDegrees)
            {
                if (simpleDirection == AeRotationDirection.CounterClockwise)
                {
                    RotateMovementVector(-rotationDegreesPerSecond, epoch);
                }
                else if (simpleDirection == AeRotationDirection.Clockwise)
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
        public bool RotateMovementVectorIfNotPointingAt(AeVector toLocation, float rotationAmountDegrees, AeRotationDirection simpleDirection, float varianceDegrees, float epoch)
        {
            var deltaAngle = this.HeadingAngleToInSignedDegrees(toLocation);

            if (Math.Abs(deltaAngle) > varianceDegrees)
            {
                if (simpleDirection == AeRotationDirection.CounterClockwise)
                {
                    RotateMovementVector(-rotationAmountDegrees, epoch);
                }
                else if (simpleDirection == AeRotationDirection.Clockwise)
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
        public bool RotateMovementVectorIfNotPointingAt(float toDegrees, float rotationAmountDegrees, AeRotationDirection simpleDirection, float tolerance, float epoch)
        {
            toDegrees = toDegrees.DenormalizeDegrees();

            if (Orientation.Degrees.IsBetween(toDegrees - tolerance, toDegrees + tolerance) == false)
            {
                if (simpleDirection == AeRotationDirection.CounterClockwise)
                {
                    RotateMovementVector(-rotationAmountDegrees, epoch);
                }
                else if (simpleDirection == AeRotationDirection.Clockwise)
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
        /// <returns>Returns TRUE if rotation occurs, returns FALSE if the object is not pointing in the given direction.</returns>
        public bool RotateMovementVectorIfPointingAt(AeSprite obj, float rotationAmountDegrees, AeRotationDirection simpleDirection, float varianceDegrees, float epoch)
        {
            var deltaAngle = this.HeadingAngleToInSignedDegrees(obj);

            if (deltaAngle.IsNotBetween(0, varianceDegrees))
            {
                if (simpleDirection == AeRotationDirection.CounterClockwise)
                {
                    RotateMovementVector(-rotationAmountDegrees, epoch);
                }
                else if (simpleDirection == AeRotationDirection.Clockwise)
                {
                    RotateMovementVector(+rotationAmountDegrees, epoch);
                }

                return true;
            }

            return false;
        }
    }
}
