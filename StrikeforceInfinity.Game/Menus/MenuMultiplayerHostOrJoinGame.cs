﻿using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Engine.Types.Geometry;
using StrikeforceInfinity.Game.Menus.BaseClasses;
using StrikeforceInfinity.Game.Sprites.MenuItems;
using StrikeforceInfinity.Shared.MultiplayerEvents;


namespace StrikeforceInfinity.Game.Menus
{
    /// <summary>
    /// Allows the player to select to host or join a game.
    /// </summary>
    internal class MenuMultiplayerHostOrJoin : MenuBase
    {
        public MenuMultiplayerHostOrJoin(EngineCore core)
            : base(core)
        {
            var currentScaledScreenBounds = _core.Display.GetCurrentScaledScreenBounds();

            double offsetX = _core.Display.TotalCanvasSize.Width / 2;
            double offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = CreateAndAddTitleItem(new SiPoint(offsetX, offsetY), "Multiplayer");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;
            itemTitle.Highlight = true;

            var helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), "JOIN", " Join a Game ");
            helpItem.Selected = true;
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = CreateAndAddSelectableItem(new SiPoint(offsetX, offsetY), "HOST", " Host a Game ");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            _core.MessageClient.Notify(new MultiplayerEventHello());

            OnExecuteSelection += MenuMultiplayerHostOrJoin_OnExecuteSelection;
            OnEscape += MenuMultiplayerHostOrJoin_OnEscape;
        }

        private void MenuMultiplayerHostOrJoin_OnEscape()
        {
            QueueForDelete();
            _core.Menus.Insert(new MenuStartNewGame(_core));
        }

        private void MenuMultiplayerHostOrJoin_OnExecuteSelection(SpriteMenuItem item)
        {
            if (item.Key == "JOIN")
            {
                _core.Menus.Insert(new MenuMultiplayerJoinGame(_core));
            }
            else if (item.Key == "HOST")
            {
                _core.Menus.Insert(new MenuMultiplayerHostGame(_core));
            }
        }
    }
}
