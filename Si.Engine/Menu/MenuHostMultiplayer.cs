using Si.Engine.Menu._Superclass;
using Si.Engine.Sprite.MenuItem;
using Si.Library.Mathematics;

namespace Si.Engine.Menu
{
    /// <summary>
    //// The menu is used to set the player name and create a lobby to host.
    /// </summary>
    internal class MenuHostMultiplayer : MenuBase
    {
        public MenuHostMultiplayer(EngineCore engine)
            : base(engine)
        {
            var currentScaledScreenBounds = _engine.Display.GetCurrentScaledScreenBounds();

            float offsetX = _engine.Display.TotalCanvasSize.Width / 2;
            float offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = AddTitleItem(new SiVector(offsetX, offsetY), "Host Multiplayer");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;

            //Left aligned with some padding from the left edge of the screen.
            offsetX = currentScaledScreenBounds.X + 40;

            var textBlock = AddTextBlock(new SiVector(offsetX, offsetY), "Lobby Name: ");
            var input = AddSelectableTextInput(new SiVector(textBlock.X + textBlock.Size.Width + 5, textBlock.Y), "LOBBY_NAME", _engine.Assets.GetRandomLobbyName(), 30);
            input.Selected = true;
            offsetY = input.Y + input.Size.Height + 20;

            textBlock = AddTextBlock(new SiVector(offsetX, offsetY), "Player Name: ");
            input = AddSelectableTextInput(new SiVector(textBlock.X + textBlock.Size.Width + 5, textBlock.Y), "PLAYER_NAME", _engine.Assets.GetRandomGamerTag(), 20);
            offsetY = input.Y + input.Size.Height + 20;

            //Center aligned.
            offsetX = _engine.Display.TotalCanvasSize.Width / 2;

            var menuItem = AddSelectableItem(new SiVector(offsetX, offsetY), "CREATE_LOBBY", " Create Lobby ");
            menuItem.X -= menuItem.Size.Width / 2;
            offsetY += menuItem.Size.Height + 5;

            menuItem = AddSelectableItem(new SiVector(offsetX, offsetY), "GO_BACK", " Go Back ");
            menuItem.X -= menuItem.Size.Width / 2;
            offsetY += menuItem.Size.Height + 5;

            OnExecuteSelection += MenuJoinMultiplayer_OnExecuteSelection;
        }

        private bool MenuJoinMultiplayer_OnExecuteSelection(SpriteMenuItem item)
        {
            switch (item.SpriteTag)
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
