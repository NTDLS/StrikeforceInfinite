namespace Si.MpServer
{
    internal class Session
    {
        public Guid SessionId { get; private set; } = Guid.NewGuid();
    }
}
