namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
{
    /// <summary>
    /// Represents an action that spawns a sprite with specified position, orientation, and movement parameters.
    /// </summary>
    /// <remarks>This class is used to define the initial state and properties for a sprite when it is spawned
    /// in the environment. It allows configuration of position, orientation, speed, and sprite type, enabling flexible
    /// control over sprite creation scenarios.</remarks>
    public class AeSpriteActionSpawn
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
        /// Gets or sets the type of sprite represented by this instance.
        /// </summary>
        public string SpriteType { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AeSpriteActionSpawn(uint spriteUID, string spriteType)
            : base(spriteUID)
        {
            SpriteType = spriteType;
        }
    }
}
