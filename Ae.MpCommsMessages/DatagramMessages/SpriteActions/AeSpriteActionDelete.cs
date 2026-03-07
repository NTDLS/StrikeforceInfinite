namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
{
    /// <summary>
    /// Drone needs to be deleted, not exploded.
    /// </summary>
    public class AeSpriteActionDelete : AeSpriteAction
    {
        public AeSpriteActionDelete(uint spriteUID)
            : base(spriteUID)
        {
        }
    }
}
