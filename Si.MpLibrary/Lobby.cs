namespace Si.MpLibrary
{
    public class Lobby
    {
        public Guid LobbyId { get; private set; } = Guid.NewGuid();

        public Session OwnerSession { get; private set; }

        public SpriteActionBuffer ActionBuffer { get; private set; }= new();

        public Lobby(Session ownerSession)
        {
            OwnerSession = ownerSession;
            //var engine = new EngineCore();
        }
    }
}
