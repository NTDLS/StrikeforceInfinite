using NTDLS.DatagramMessaging;

namespace Ae.MpCommsMessages.DatagramMessages.SpriteActions
{
    public class AeSpriteActionCollection
        : IDmDatagram
    {
        public AeSpriteAction[] Collection { get; set; }

        public AeSpriteActionCollection()
        {
            Collection = [];
        }

        public AeSpriteActionCollection(AeSpriteAction[] collection)
        {
            Collection = collection;
        }
    }
}
