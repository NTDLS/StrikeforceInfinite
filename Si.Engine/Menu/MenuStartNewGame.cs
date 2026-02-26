using Si.Engine.Menu._Superclass;
using Si.Engine.Sprite.MenuItem;
using Si.Library.Mathematics;

namespace Si.Engine.Menu
{
    /// <summary>
    /// The menu that is shows when the game is first started.
    /// Allows the player to select single player, join multiplayer or host multiplayer.
    /// </summary>
    internal class MenuStartNewGame : MenuBase
    {
        public MenuStartNewGame(EngineCore engine)
            : base(engine)
        {
            var currentScaledScreenBounds = _engine.Display.GetCurrentScaledScreenBounds();

            float offsetX = _engine.Display.TotalCanvasSize.Width / 2;
            float offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = AddTitleItem(new SiVector(offsetX, offsetY), "Strikeforce Infinite");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;

            var menuItem = AddSelectableItem(new SiVector(offsetX, offsetY), "SINGLE_PLAYER", " Single Player ");
            menuItem.Selected = true;
            menuItem.X -= menuItem.Size.Width / 2;
            offsetY += menuItem.Size.Height + 5;

            menuItem = AddSelectableItem(new SiVector(offsetX, offsetY), "JOIN_MULTIPLAYER", " Join Multiplayer ");
            menuItem.X -= menuItem.Size.Width / 2;
            offsetY += menuItem.Size.Height + 5;

            menuItem = AddSelectableItem(new SiVector(offsetX, offsetY), "HOST_MULTIPLAYER", " Host Multiplayer ");
            menuItem.X -= menuItem.Size.Width / 2;
            offsetY += menuItem.Size.Height + 5;

            offsetY += 50;

            var helpItem = AddTextBlock(new SiVector(offsetX, offsetY), "Forward, Reverse and Rotate with <W>, <A>, <S>, and <D>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = AddTextBlock(new SiVector(offsetX, offsetY), "Strafe with <LEFT> and <RIGHT> arrows.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = AddTextBlock(new SiVector(offsetX, offsetY), "Surge Drive with <SHIFT>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 5;

            helpItem = AddTextBlock(new SiVector(offsetX, offsetY), "Fire primary with <SPACE>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            helpItem = AddTextBlock(new SiVector(offsetX, offsetY), "Fire secondary with <CTRL>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            helpItem = AddTextBlock(new SiVector(offsetX, offsetY), "Change weapons with <Q> and <E>.");
            helpItem.X -= helpItem.Size.Width / 2;
            offsetY += helpItem.Size.Height + 10;

            //helpItem = AddTextBlock(new SiPoint(offsetX, offsetY), "Change speed with <UP> and <DOWN> arrows.");
            //helpItem.X -= helpItem.Size.Width / 2;
            //offsetY += helpItem.Size.Height + 10;

            //itemYes.Selected = true;

            OnExecuteSelection += MenuStartNewGame_OnExecuteSelection;
        }

        private bool MenuStartNewGame_OnExecuteSelection(SpriteMenuItem item)
        {
            switch (item.SpriteTag)
            {
                case "SINGLE_PLAYER":
                    _engine.InitializeForSinglePlayer();
                    _engine.Menus.Show(new MenuSituationSelect(_engine));
                    break;
                case "JOIN_MULTIPLAYER":
                    _engine.InitializeForMultiplayer();
                    _engine.Menus.Show(new MenuJoinMultiplayer(_engine));
                    break;
                case "HOST_MULTIPLAYER":
                    _engine.InitializeForMultiplayer();
                    _engine.Menus.Show(new MenuHostMultiplayer(_engine));
                    break;
                default:
                    throw new System.NotImplementedException();
            }

            return true;
        }
    }
}
