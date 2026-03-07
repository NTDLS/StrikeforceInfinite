using NTDLS.DatagramMessaging;

namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
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
