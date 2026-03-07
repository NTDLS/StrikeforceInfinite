using NTDLS.DatagramMessaging;

namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
{
    public class SiSpriteActionCollection
        : IDmDatagram
    {
        public SiSpriteAction[] Collection { get; set; }

        public SiSpriteActionCollection()
        {
            Collection = [];
        }

        public SiSpriteActionCollection(SiSpriteAction[] collection)
        {
            Collection = collection;
        }
    }
}
