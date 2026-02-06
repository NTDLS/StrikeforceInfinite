namespace Si.MpLibrary
{
    public class Session
    {
        public Guid SessionId { get; private set; } = Guid.NewGuid();
    }
}
