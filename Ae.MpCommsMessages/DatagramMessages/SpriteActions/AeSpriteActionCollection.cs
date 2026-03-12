using NTDLS.DatagramMessaging;

namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
{
    /// <summary>
    /// Represents a collection of sprite actions used for animation or state management.
    /// </summary>
    /// <remarks>Use this class to group multiple sprite actions together for processing or serialization. The
    /// collection can be initialized empty or with a predefined set of actions.</remarks>
    public class AeSpriteActionCollection
        : IDmDatagram
    {
        /// <summary>
        /// Gets or sets the array of actions associated with the sprite.
        /// </summary>
        public AeSpriteAction[] Collection { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AeSpriteActionCollection()
        {
            Collection = [];
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AeSpriteActionCollection(AeSpriteAction[] collection)
        {
            Collection = collection;
        }
    }
}
