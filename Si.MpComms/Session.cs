using System.Net;

namespace Si.MpComms
{
    public class Session
    {
        public Guid SessionId { get; private set; } = Guid.NewGuid();

        public IPEndPoint? DatagramEndPoint { get; private set; }

        public void AttachDatagramEndpoint(IPEndPoint? ipEndPoint)
        {
            DatagramEndPoint = ipEndPoint;
        }
    }
}
