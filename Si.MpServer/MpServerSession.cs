namespace Si.MpServer
{
    internal class MpServerSession
    {
        public Guid SessionId { get; private set; } = Guid.NewGuid();
    }
}
