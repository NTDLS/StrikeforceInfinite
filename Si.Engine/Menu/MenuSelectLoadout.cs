using Si.Engine.Menu._Superclass;
using Si.Engine.Sprite;
using Si.Engine.Sprite.MenuItem;
using Si.Library.Mathematics;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Si.Engine.Menu
{
    /// <summary>
    /// The menu that is displayed at game start to allow the player to select a loadout.
    /// </summary>
    internal class MenuSelectLoadout : MenuBase
    {
        class SelectedSprite
        {
            public SpritePlayer Sprite { get; set; }
            public string SpritePath { get; set; }

            public SelectedSprite(string spritePath, SpritePlayer sprite)
            {
                Sprite = sprite;
                SpritePath = spritePath;

            }
        }

        private readonly SpriteMenuItem _shipBlurb;
        private Timer _animationTimer;
        private SelectedSprite? _selectedSprite;

        public MenuSelectLoadout(EngineCore engine)
            : base(engine)
        {
            var currentScaledScreenBounds = _engine.Display.GetCurrentScaledScreenBounds();

            float offsetX = _engine.Display.TotalCanvasSize.Width / 2;
            float offsetY = currentScaledScreenBounds.Y + 100;

            var itemTitle = AddTitleItem(new SiVector(offsetX, offsetY), "Select a Loadout");
            itemTitle.X -= itemTitle.Size.Width / 2;
            offsetY += itemTitle.Size.Height + 60;

            offsetX = currentScaledScreenBounds.X + 40;
            offsetY += itemTitle.Height;

            _shipBlurb = AddTextBlock(new SiVector(offsetX, offsetY), "");
            _shipBlurb.X = offsetX + 250;
            _shipBlurb.Y = offsetY - _shipBlurb.Size.Height;

            var playerShipContainers = engine.Assets.GetAssetsInPath(@"Sprites/Player/Ships");

            List<SpritePlayer> playerSprites = new();

            float previousSpriteSize = 0;

            foreach (var playerShipContainer in playerShipContainers)
            {
                var playerSprite = engine.Sprites.Add<SpritePlayer>(playerShipContainer.Key);

                playerSprite.SpriteTag = "MENU_SHIP_SELECT";
                playerSprite.Orientation.Degrees = 45;

                offsetY += playerSprite.Size.Height / 2.0f + previousSpriteSize / 2.0f + 25;
                previousSpriteSize = playerSprite.Size.Height;

                var menuItem = AddSelectableItem(new SiVector(offsetX + 75, offsetY), playerSprite.Metadata.Name ?? "Unknown Ship", playerSprite.Metadata.Name ?? "Unknown Ship");
                menuItem.Y -= menuItem.Size.Height / 2;

                menuItem.UserData = new SelectedSprite(playerShipContainer.Key, playerSprite);

                playerSprite.X = offsetX;
                playerSprite.Y = offsetY;

                playerSprites.Add(playerSprite);
            }

            playerSprites.ForEach(sprite =>
            {
                sprite.ThrusterAnimation?.IsVisible = true;
            });

            OnSelectionChanged += PlayerLoadoutMenu_OnSelectionChanged;
            OnExecuteSelection += PlayerLoadoutMenu_OnExecuteSelection;
            OnCleanup += PlayerLoadoutMenu_OnCleanup;
            OnEscape += SpMenuSelectLoadout_OnEscape;

            VisibleSelectableItems().First().Selected = true;

            _animationTimer = new Timer(PlayerLoadoutMenu_Tick, null, 10, 10);
        }

        private bool SpMenuSelectLoadout_OnEscape()
        {
            _engine.Menus.Show(new MenuSituationSelect(_engine));
            return true;
        }

        private void PlayerLoadoutMenu_OnCleanup()
        {
            _engine.Sprites.QueueAllForDeletionByTag("MENU_SHIP_SELECT");
        }

        private bool PlayerLoadoutMenu_OnExecuteSelection(SpriteMenuItem item)
        {
            _animationTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _animationTimer.Dispose();

            if (item.UserData is SelectedSprite selectedSprite)
            {
                _engine.Player.InstantiatePlayerClass(selectedSprite.SpritePath);
                _engine.StartGame();
            }

            return true;
        }

        private void PlayerLoadoutMenu_OnSelectionChanged(SpriteMenuItem item)
        {
            if (item.UserData is SelectedSprite selectedSprite)
            {
                _shipBlurb.Text = selectedSprite.Sprite.GetLoadoutHelpText();
                _selectedSprite = selectedSprite;
            }
        }

        private void PlayerLoadoutMenu_Tick(object? sender)
        {
            _selectedSprite?.Sprite.RotateOrientation(1, 1);
        }
    }
}
