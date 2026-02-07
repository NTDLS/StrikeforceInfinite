using NTDLS.Helpers;
using Si.Engine.Menu._Superclass;
using Si.Engine.Sprite.MenuItem;
using Si.Library.Mathematics;

namespace Si.Engine.Menu
{
    /// <summary>
    //// The menu is used to set the player name and select a lobby to join.
    /// </summary>
    internal class MenuJoinMultiplayer : MenuBase
    {
        public MenuJoinMultiplayer(EngineCore engine)
            : base(engine)
        {
            engine.CommsManager.EnsureNotNull();

            var currentScaledScreenBounds = _engine.Display.GetCurrentScaledScreenBounds();

            float offsetX = _engine.Display.TotalCanvasSize.Width / 2;
            float offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = AddTitleItem(new SiVector(offsetX, offsetY), "Join Multiplayer");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;

            var menuItem = AddSelectableItem(new SiVector(offsetX, offsetY), "GO_BACK", " Go Back ");
            menuItem.Selected = true;
            menuItem.X -= menuItem.Size.Width / 2;
            offsetY += menuItem.Size.Height + 5;


            var lobbies = engine.CommsManager.GetLobbiesPaged(1);

            foreach(var lobby in lobbies.Collection)
            {
                menuItem = AddSelectableItem(new SiVector(offsetX, offsetY), "LOBBY_ITEM", $" {lobby.Name} ({lobby.CurrentPlayers}/{lobby.MaxPlayers}) ");
                menuItem.UserData = lobby;
                menuItem.X -= menuItem.Size.Width / 2;
                offsetY += menuItem.Size.Height + 5;
            }

            menuItem = AddSelectableItem(new SiVector(offsetX, offsetY), "PAGE_NEXT", " Next Page ");
            menuItem.X -= menuItem.Size.Width / 2;
            offsetY += menuItem.Size.Height + 5;


            menuItem = AddSelectableItem(new SiVector(offsetX, offsetY), "PAGE_Prev", " Previous Page ");
            menuItem.X -= menuItem.Size.Width / 2;
            offsetY += menuItem.Size.Height + 5;

            OnExecuteSelection += MenuJoinMultiplayer_OnExecuteSelection;
        }

        private bool MenuJoinMultiplayer_OnExecuteSelection(SpriteMenuItem item)
        {
            switch (item.Key)
            {
                case "????":
                    return false;
                case "GO_BACK":
                    _engine.Menus.Show(new MenuStartNewGame(_engine));
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            return true;
        }
    }
}
