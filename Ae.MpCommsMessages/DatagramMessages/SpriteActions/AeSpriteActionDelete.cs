namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
{
    /// <summary>
    /// Sprite needs to be deleted, not exploded.
    /// </summary>
    public class AeSpriteActionDelete
        : AeSpriteAction
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AeSpriteActionDelete(uint spriteUID)
            : base(spriteUID)
        {
        }
    }
}
