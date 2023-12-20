﻿using NTDLS.ReliableMessaging;
using NTDLS.StreamFraming.Payloads;
using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Shared.Messages.Notify;
using StrikeforceInfinity.Shared.Messages.Query;
using StrikeforceInfinity.Shared.Payload;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace StrikeforceInfinity.Game.Managers
{
    /// <summary>
    /// Contains everything you need to manage multiplayer functionality.
    /// </summary>
    internal class EngineMultiplayManager
    {
        public HgPlayMode PlayMode { get; private set; }
        public Guid LobbyUID { get; private set; } = Guid.Empty;

        private readonly EngineCore _gameCore;
        private MessageClient _messageClient;
        private MessageClient MessageClient
        {
            get
            {
                #region Message Client Creation and Connectivity.
                if (_messageClient == null)
                {
                    lock (this)
                    {
                        if (_messageClient == null)
                        {
                            var messageClient = new MessageClient();

                            messageClient.OnNotificationReceived += MessageClient_OnNotificationReceived;
                            messageClient.OnQueryReceived += MessageClient_OnQueryReceived;

                            messageClient.Connect(Constants.DataAddress, Constants.DataPort);
                            _messageClient = messageClient;
                        }
                    }
                }
                #endregion
                return _messageClient;
            }
        }

        public EngineMultiplayManager(EngineCore gameCore)
        {
            _gameCore = gameCore;
        }

        public void GetSettingsFromServer()
        {
            //Tell the server hello and request any settings that the server wants to enforce on the client.
            var reply = MessageClient.Query<SiConfigureReply>(new SiConfigure()).Result;
            _gameCore.Settings.Multiplayer.PlayerAbsoluteStateDelayMs = reply.PlayerAbsoluteStateDelayMs;
        }

        public void SetPlayMode(HgPlayMode playMode) => PlayMode = playMode;

        public void RegisterLobbyUID(Guid lobbyUID)
        {
            LobbyUID = lobbyUID;
            if (PlayMode != HgPlayMode.SinglePlayer)
            {
                Notify(new SiRegisterToLobby(LobbyUID));
            }
        }

        public void DeregisterLobbyUID()
        {
            if (PlayMode != HgPlayMode.SinglePlayer)
            {
                Notify(new SiRegisterToLobby(LobbyUID));
            }
            LobbyUID = Guid.Empty;
        }

        public void BroadcastLevelLayout()
        {
            var spriteLayouts = new List<SiSpriteLayout>();

            //--------------------------------------------------------------------------------------
            //-- Send the enemy sprites:
            //--------------------------------------------------------------------------------------
            var enemies = _gameCore.Sprites.Enemies.All();
            foreach (var enemy in enemies)
            {
                //Make sure the MultiplayUID matches the MultiplayUID at all other connections.
                enemy.MultiplayUID = enemy.UID;

                spriteLayouts.Add(new SiSpriteLayout(enemy.GetType().FullName, enemy.UID)
                {
                    Vector = new SiSpriteVector() { X = enemy.X, Y = enemy.Y }
                });
            }

            //--------------------------------------------------------------------------------------
            //-- Send the human player sprite (drone):
            //--------------------------------------------------------------------------------------
            //Make sure the MultiplayUID matches the MultiplayUID at all other connections.
            _gameCore.Player.Sprite.MultiplayUID = _gameCore.Player.Sprite.UID;
            spriteLayouts.Add(new SiSpriteLayout(_gameCore.Player.Sprite.GetType().FullName + "Drone", _gameCore.Player.Sprite.UID)
            {
                Vector = new SiSpriteVector() { X = _gameCore.Display.BackgroundOffset.X, Y = _gameCore.Display.BackgroundOffset.Y }
            });

            //--------------------------------------------------------------------------------------
            MessageClient.Notify(new SiSituationLayout()
            {
                SpriteLayouts = spriteLayouts
            });
        }

        public void SetReadyToPlay()
        {
            if (PlayMode != HgPlayMode.SinglePlayer)
            {
                Notify(new SiReadyToPlay());
            }
        }

        public void SetWaitingInLobby()
        {
            if (PlayMode != HgPlayMode.SinglePlayer)
            {
                Notify(new SiWaitingInLobby());
            }
        }

        public async Task<SiLobbyInfo> GetLobbyInfo(Guid lobbyUID)
        {
            var reply = await MessageClient.Query<SiGetLobbyInfoReply>(new SiGetLobbyInfo() { LobyUID = lobbyUID });
            return reply.Info;
        }

        public Guid CreateHost(SiLobbyConfiguration gameHostConfiguration)
        {
            var query = new SiCreateLobby()
            {
                Configuration = gameHostConfiguration
            };

            var reply = MessageClient.Query<SiCreateLobbyReply>(query).Result;

            return reply.LobbyUID;
        }

        public List<SiLobbyConfiguration> GetHostList()
        {
            var reply = MessageClient.Query<SiListLobbiesReply>(new SiListLobbies()).Result;
            return reply.Collection;
        }

        private void MessageClient_OnNotificationReceived(MessageClient client, Guid connectionId, IFramePayloadNotification payload)
        {
            //------------------------------------------------------------------------------------------------------------------------------
            if (payload is SiSituationLayout layoutDirective)
            {
                //The server is telling us to initialize the layout using the supplied sprites and their states.

                _gameCore.Sprites.Enemies.DeleteAll();

                foreach (var spriteInfo in layoutDirective.SpriteLayouts)
                {
                    Debug.WriteLine($"Adding Sprite: {spriteInfo.MultiplayUID}->'{spriteInfo.FullTypeName}'");

                    var sprite = _gameCore.Sprites.CreateByNameOfType(spriteInfo.FullTypeName);
                    sprite.MultiplayUID = spriteInfo.MultiplayUID;
                    sprite.X = spriteInfo.Vector.X;
                    sprite.Y = spriteInfo.Vector.Y;
                    sprite.Velocity.Angle.Degrees = spriteInfo.Vector.AngleDegrees;
                    sprite.Velocity.ThrottlePercentage = spriteInfo.Vector.ThrottlePercentage;
                    sprite.Velocity.BoostPercentage = spriteInfo.Vector.BoostPercentage;
                    sprite.ControlledBy = _gameCore.Multiplay.PlayMode switch
                    {
                        HgPlayMode.MutiPlayerHost => HgControlledBy.LocalAI,
                        HgPlayMode.MutiPlayerClient => HgControlledBy.Server,
                        _ => throw new InvalidOperationException("Unhandled PlayMode")
                    };
                }
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiRequestSituationLayout)
            {
                //TODO: Send current sprite layout...
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else if (payload is SiSpriteVectors spriteVectors)
            {
                foreach (var spriteVector in spriteVectors.Collection)
                {
                    //TODO: Updates sprites...
                    //Debug.WriteLine(spriteVector.MultiplayUID);

                    var sprite = _gameCore.Sprites.Collection.Where(o => o.MultiplayUID == spriteVector.MultiplayUID).FirstOrDefault();
                    if (sprite != null)
                    {
                        sprite.X = spriteVector.X;
                        sprite.Y = spriteVector.Y;
                        sprite.Velocity.Angle.Degrees = spriteVector.AngleDegrees;
                    }
                }
            }
            //------------------------------------------------------------------------------------------------------------------------------
            else
            {
                throw new NotImplementedException("The client notification is not implemented.");
            }
        }

        private IFramePayloadQueryReply MessageClient_OnQueryReceived(MessageClient client, Guid connectionId, IFramePayloadQuery payload)
        {
            if (false)
            {

            }
            else
            {
                throw new NotImplementedException("The client query is not implemented.");
            }
        }


        public void NotifyddddSpriteVector(IFramePayloadNotification multiplayerEvent)
        {
            //TODO: We should really package these into a collection instead of sending one at a time.

            if (LobbyUID != Guid.Empty)
            {
                if (PlayMode != HgPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
                {
                    MessageClient?.Notify(multiplayerEvent);
                }
            }
        }


        readonly List<SiSpriteVector> _spriteVectors = new();

        public void RecordSpriteVector(SiSpriteVector multiplayerEvent)
        {
            if (LobbyUID != Guid.Empty)
            {
                if (PlayMode != HgPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
                {
                    _spriteVectors.Add(multiplayerEvent);
                }
            }
        }

        public void FlushSpriteVectorsToServer()
        {
            if (LobbyUID != Guid.Empty && _spriteVectors.Any())
            {
                if (PlayMode != HgPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
                {
                    MessageClient?.Notify(new SiSpriteVectors(_spriteVectors));
                    _spriteVectors.Clear();
                }
            }
        }

        public void Notify(IFramePayloadNotification multiplayerEvent)
        {
            if (PlayMode != HgPlayMode.SinglePlayer && MessageClient?.IsConnected == true)
            {
                MessageClient?.Notify(multiplayerEvent);
            }
        }
    }
}
