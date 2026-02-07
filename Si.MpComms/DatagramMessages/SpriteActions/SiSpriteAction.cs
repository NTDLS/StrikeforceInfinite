using NTDLS.DatagramMessaging;

namespace Si.MpComms.DatagramMessages.SpriteActions
{
    public class SiSpriteAction
        : IDmDatagram
    {
        public uint SpriteUID { get; set; }

        public SiSpriteAction(uint spriteUID)
        {
            SpriteUID = spriteUID;
        }
    }
}
