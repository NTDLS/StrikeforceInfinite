namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
{
    /// <summary>
    /// Represents a motion action for a sprite, including position, orientation, and movement parameters.
    /// </summary>
    /// <remarks>This class encapsulates the state and control values used to direct a sprite's movement and
    /// orientation within an action sequence. It extends the base AeSpriteAction to provide additional properties for
    /// motion control, such as position, speed, and rotation. Use this type when specifying or querying the
    /// motion-related aspects of a sprite's action.</remarks>
    public class AeSpriteActionMotion
        : AeSpriteAction
    {
        /// <summary>
        /// Gets or sets the X-coordinate value.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y-coordinate value.
        /// </summary>
        public float Y { get; set; }
        /// <summary>
        /// Gets or sets the orientation in degrees, signed.
        /// </summary>
        public float OrientationDegreesSigned { get; set; }
        /// <summary>
        /// Gets or sets the throttle value used to control the speed or power output.
        /// </summary>
        public float Throttle { get; set; }
        //public float BoostPercentage { get; set; }
        /// <summary>
        /// Gets or sets the speed value for the object.
        /// </summary>
        public float Speed { get; set; }
        /// <summary>
        /// Gets or sets the speed at which the object rotates.
        /// </summary>
        public float RotationSpeed { get; set; }
        //public float Boost { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AeSpriteActionMotion(uint spriteUID)
            : base(spriteUID)
        {
        }
    }
}
