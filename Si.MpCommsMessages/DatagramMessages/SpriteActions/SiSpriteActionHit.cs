namespace Si.MpCommsMessages.DatagramMessages.SpriteActions
{
    public class SiSpriteActionHit : SiSpriteAction
    {
        public uint MunitionUID { get; set; }

        public SiSpriteActionHit(uint spriteUID, uint munitionUID)
            : base(spriteUID)
        {
            MunitionUID = munitionUID;
        }
    }
}
