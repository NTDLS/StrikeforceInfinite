namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
{
    /// <summary>
    /// Represents an action indicating that a sprite has been hit by a munition.
    /// </summary>
    public class AeSpriteActionHit : AeSpriteAction
    {
        /// <summary>
        /// Gets or sets the unique identifier for the munition.
        /// </summary>
        public uint MunitionUID { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public AeSpriteActionHit(uint spriteUID, uint munitionUID)
            : base(spriteUID)
        {
            MunitionUID = munitionUID;
        }
    }
}
