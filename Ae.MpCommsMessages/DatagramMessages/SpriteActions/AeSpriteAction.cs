using NTDLS.DatagramMessaging;

namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
{
    /// <summary>
    /// Represents an action to be performed on a sprite, such as exploding or deleting it.
    /// This is a base class for specific sprite actions.
    /// </summary>
    public class AeSpriteAction
        : IDmDatagram
    {
        /// <summary>
        /// Gets or sets the unique identifier for the sprite.
        /// </summary>
        public uint SpriteUID { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AeSpriteAction(uint spriteUID)
        {
            SpriteUID = spriteUID;
        }
    }
}
