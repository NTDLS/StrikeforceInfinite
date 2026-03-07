using NTDLS.DatagramMessaging;

namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
{
    public class AeSpriteAction
        : IDmDatagram
    {
        public uint SpriteUID { get; set; }

        public AeSpriteAction(uint spriteUID)
        {
            SpriteUID = spriteUID;
        }
    }
}
