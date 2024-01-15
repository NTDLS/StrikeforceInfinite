﻿using Si.GameEngine.Menus._Superclass;
using Si.GameEngine.Sprites.MenuItems;
using Si.Menus.SinglePlayer;
using Si.Shared.Types.Geometry;
using System.Drawing;
using System.Timers;

namespace Si.Menus.MultiPlayer.Host
{
    /// <summary>
    /// The menu that the host owner waits at for clients to join.
    /// </summary>
    internal class MpMenuHostLobbyWait : MenuBase
    {
        private readonly Timer _timer = new(1000);
        private readonly SpriteMenuItem _countdownToAutoStart;
        private readonly SpriteMenuItem _countOfReadyPlayers;
        private readonly RectangleF _currentScaledScreenBounds;

        public MpMenuHostLobbyWait(GameEngine.Core.Engine gameEngine)
            : base(gameEngine)
        {
            _currentScaledScreenBounds = _gameEngine.Display.GetCurrentScaledScreenBounds();

            double offsetX = _gameEngine.Display.TotalCanvasSize.Width / 2;
            double offsetY = _currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Waiting in your Lobby");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.IsHighlighted = true;

            _countOfReadyPlayers = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "?");
            _countOfReadyPlayers.X -= _countOfReadyPlayers.Size.Width / 2;

            offsetY += _countOfReadyPlayers.Size.Height + 10;

            _countdownToAutoStart = CreateAndAddTextblock(new SiPoint(offsetX, offsetY), "");
            _countdownToAutoStart.X -= _countdownToAutoStart.Size.Width / 2;

            offsetY += _countdownToAutoStart.Size.Height + 10;

            var helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), "START_NOW", " Start Now! ");
            helpItem.Selected = true;
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            OnExecuteSelection += MpMenuHostLobbyWait_OnExecuteSelection;
            OnCleanup += MpMenuHostLobbyWait_OnCleanup;
            OnEscape += MpMenuHostLobbyWait_OnEscape;

            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();

            _gameEngine.Multiplay.SetWaitingInLobby();
        }

        private bool MpMenuHostLobbyWait_OnEscape()
        {
            _gameEngine.Multiplay.SetLeftLobby();
            _gameEngine.Menus.Add(new MpMenuHostSelectLoadout(_gameEngine));
            return true;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _gameEngine.Multiplay.GetLobbyInfo(_gameEngine.Multiplay.State.LobbyUID).ContinueWith(o =>
            {
                _countOfReadyPlayers.Text = $"Players: {o.Result.WaitingCount:n0}";
                _countOfReadyPlayers.X = (_gameEngine.Display.TotalCanvasSize.Width / 2) - (_countOfReadyPlayers.Size.Width / 2);

                if (o.Result.RemainingSecondsUntilAutoStart != null)
                {
                    _countdownToAutoStart.Text = $"Auto-starting in {o.Result.RemainingSecondsUntilAutoStart:n0}s.";
                    _countdownToAutoStart.X = (_gameEngine.Display.TotalCanvasSize.Width / 2) - (_countdownToAutoStart.Size.Width / 2);
                }
                else
                {
                    _countdownToAutoStart.Text = "";
                }
            });
        }

        private void MpMenuHostLobbyWait_OnCleanup()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        private bool MpMenuHostLobbyWait_OnExecuteSelection(SpriteMenuItem item)
        {
            _gameEngine.StartGame();
            return true;
        }
    }
}
