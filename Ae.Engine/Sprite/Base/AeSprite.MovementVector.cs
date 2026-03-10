using Ae.Engine.Mathematics;

namespace Ae.Engine.Sprite.Base
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class AeSprite
    {
        /// <summary>
        /// Sets the movement vector in the direction of the sprite taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVectorFromOrientation() => MovementVector = MakeMovementVectorFromOrientation();

        /// <summary>
        /// Sets the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVectorFromAngle(float angleInRadians) => MovementVector = MakeMovementVectorFromAngle(angleInRadians);

        /// <summary>
        /// Sets the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public void RecalculateMovementVectorFromAngle(AeVector angle) => MovementVector = MakeMovementVectorFromAngle(angle);

        /// <summary>
        /// Returns the movement vector in the direction of the sprite taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public AeVector MakeMovementVectorFromOrientation() => Orientation * Speed * Throttle;

        /// <summary>
        /// Returns the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public AeVector MakeMovementVectorFromAngle(float angleInRadians) => new AeVector(angleInRadians) * Speed * Throttle;

        /// <summary>
        /// Returns the movement vector in the given direction taking into account the speed and throttle percentage.
        /// </summary>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public AeVector MakeMovementVectorFromAngle(AeVector angle) => angle.Normalize() * Speed * Throttle;
    }
}
