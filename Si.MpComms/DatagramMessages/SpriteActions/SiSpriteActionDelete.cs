namespace Si.MpComms.DatagramMessages.SpriteActions
{
    /// <summary>
    /// Drone needs to be deleted, not exploded.
    /// </summary>
    public class SiSpriteActionDelete : SiSpriteAction
    {
        public SiSpriteActionDelete(uint spriteUID)
            : base(spriteUID)
        {
        }
    }
}
