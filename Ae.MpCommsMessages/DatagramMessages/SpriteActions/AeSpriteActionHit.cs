namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
{
    public class AeSpriteActionHit : AeSpriteAction
    {
        public uint MunitionUID { get; set; }

        public AeSpriteActionHit(uint spriteUID, uint munitionUID)
            : base(spriteUID)
        {
            MunitionUID = munitionUID;
        }
    }
}
