namespace Si.MpCommsMessages.Models
{
    public class Lobby
    {
        public Guid LobbyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MaxPlayers { get; set; }
        public int CurrentPlayers { get; set; }
    }
}
