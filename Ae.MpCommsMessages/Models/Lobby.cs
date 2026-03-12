using System;

namespace Ae.MpCommsMessages.Models
{
    /// <summary>
    /// Represents a multiplayer game lobby with identifying information and player counts.
    /// </summary>
    /// <remarks>The Lobby class provides properties for managing the lobby's unique identifier, name, maximum
    /// allowed players, and current player count. It can be used to track and display lobby status in multiplayer
    /// scenarios.</remarks>
    public class Lobby
    {
        /// <summary>
        /// Gets or sets the unique identifier for the lobby.
        /// </summary>
        public Guid LobbyId { get; set; }
        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the maximum number of players allowed in the game.
        /// </summary>
        public int MaxPlayers { get; set; }
        /// <summary>
        /// Gets or sets the number of players currently active in the game.
        /// </summary>
        public int CurrentPlayers { get; set; }
    }
}
