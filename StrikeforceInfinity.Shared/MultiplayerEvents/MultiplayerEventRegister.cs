﻿namespace StrikeforceInfinity.Shared.MultiplayerEvents
{
    /// <summary>
    /// Tells the server that a client will be playing on a given game host.
    /// </summary>
    public class MultiplayerEventRegister : MultiplayerEventBase
    {
        public Guid GameHostUID { get; set; }

        public MultiplayerEventRegister(Guid gameHostUID)
        {
            GameHostUID = gameHostUID;            
        }
    }
}
