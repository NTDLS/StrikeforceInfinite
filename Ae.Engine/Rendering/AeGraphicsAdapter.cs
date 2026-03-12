namespace Ae.Engine.Rendering
{
    /// <summary>
    /// Represents a graphics adapter available on the system, including its device identifier and descriptive
    /// information.
    /// </summary>
    /// <remarks>Use this class to access information about a specific graphics adapter, such as its device
    /// ID, description, and available video memory. This type is typically used for enumerating and selecting graphics
    /// hardware in applications that require explicit adapter management.</remarks>
    public class AeGraphicsAdapter
    {
        /// <summary>
        /// Gets or sets the unique identifier for the device.
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the textual description associated with the object.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the amount of video memory, in megabytes, available to the system.
        /// </summary>
        public double VideoMemoryMb { get; set; }

        /// <summary>
        /// Initializes a new instance of the AeGraphicsAdapter class with the specified device identifier and
        /// description.
        /// </summary>
        /// <param name="deviceId">The unique identifier for the graphics device. Used to distinguish between different adapters.</param>
        /// <param name="description">A descriptive string for the graphics adapter. Typically includes model or vendor information.</param>
        public AeGraphicsAdapter(int deviceId, string description)
        {
            DeviceId = deviceId;
            Description = description;
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string containing the description of the object.</returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
