﻿using NTDLS.StreamFraming.Payloads;
using Si.Library.Payload;

namespace Si.Library.Messages.Query
{
    /// <summary>
    /// Requests a list of lobbies from the server.
    /// </summary>
    public class SiListLobbies : IFramePayloadQuery
    {
        //TODO: include some types of filters here
    }

    public class SiListLobbiesReply : IFramePayloadQueryReply
    {
        public List<SiLobbyConfiguration> Collection { get; set; } = new();
    }
}