using NTDLS.Helpers;
using SharpDX.Mathematics.Interop;
using Si.Engine.Menu._Superclass;
using Si.Engine.Sprite;
using Si.Engine.Sprite.MenuItem;
using Si.Library.Mathematics;
using Si.Rendering;
using System.Linq;

namespace Si.Engine.Menu
{
    /// <summary>
    //// The menu is used to set the player name and select a lobby to join.
    /// </summary>
    internal class MenuJoinMultiplayer : MenuBase
    {
        private readonly SpriteTextBlock _boxTitle;

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

            CenterHorizontally([
                    AddSelectableItem(new SiVector(offsetX, offsetY), "PAGE_PREV", " Previous Page "),
                    AddSelectableItem(new SiVector(offsetX, offsetY), "PAGE_NEXT", " Next Page ")
                ], offsetY, 5);
            offsetY += menuItem.Size.Height + 30;

            _boxTitle = AddTextBlock(new SiVector(offsetX, offsetY), "Lobbies");
            _boxTitle.X = (_engine.Display.TotalCanvasSize.Width / 2) - (_boxTitle.Size.Width / 2);
            offsetY += _boxTitle.Size.Height + 20;

            var lobbies = engine.CommsManager.GetLobbiesPaged(1);

            var dbg = lobbies.Collection.ToList();

            dbg.Add(new MpCommsMessages.Models.Lobby() { Name = _engine.Assets.GetRandomLobbyName(), CurrentPlayers = 5, MaxPlayers = 10 });
            dbg.Add(new MpCommsMessages.Models.Lobby() { Name = _engine.Assets.GetRandomLobbyName(), CurrentPlayers = 5, MaxPlayers = 10 });
            dbg.Add(new MpCommsMessages.Models.Lobby() { Name = _engine.Assets.GetRandomLobbyName(), CurrentPlayers = 5, MaxPlayers = 10 });
            dbg.Add(new MpCommsMessages.Models.Lobby() { Name = _engine.Assets.GetRandomLobbyName(), CurrentPlayers = 5, MaxPlayers = 10 });
            dbg.Add(new MpCommsMessages.Models.Lobby() { Name = _engine.Assets.GetRandomLobbyName(), CurrentPlayers = 5, MaxPlayers = 10 });
            dbg.Add(new MpCommsMessages.Models.Lobby() { Name = _engine.Assets.GetRandomLobbyName(), CurrentPlayers = 5, MaxPlayers = 10 });
            dbg.Add(new MpCommsMessages.Models.Lobby() { Name = _engine.Assets.GetRandomLobbyName(), CurrentPlayers = 5, MaxPlayers = 10 });
            dbg.Add(new MpCommsMessages.Models.Lobby() { Name = _engine.Assets.GetRandomLobbyName(), CurrentPlayers = 5, MaxPlayers = 10 });
            dbg.Add(new MpCommsMessages.Models.Lobby() { Name = _engine.Assets.GetRandomLobbyName(), CurrentPlayers = 5, MaxPlayers = 10 });
            dbg.Add(new MpCommsMessages.Models.Lobby() { Name = _engine.Assets.GetRandomLobbyName(), CurrentPlayers = 5, MaxPlayers = 10 });

            foreach (var lobby in dbg /*lobbies.Collection*/)
            {
                menuItem = AddSelectableItem(new SiVector(offsetX, offsetY), "LOBBY_ITEM", $" {lobby.Name} ({lobby.CurrentPlayers}/{lobby.MaxPlayers}) ");
                menuItem.UserData = lobby;
                menuItem.X -= menuItem.Size.Width / 2;
                offsetY += menuItem.Size.Height + 5;
            }

            offsetY += 25;

            CenterHorizontally([
                    AddSelectableItem(new SiVector(offsetX, offsetY), "PAGE_PREV", " Previous Page "),
                    AddSelectableItem(new SiVector(offsetX, offsetY), "PAGE_NEXT", " Next Page ")
                ], offsetY, 5);
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

        public override void Render(SharpDX.Direct2D1.RenderTarget renderTarget, float epoch)
        {
            var lobbyMenuItems = AllMenuItemsByTag("LOBBY_ITEM");
            if (lobbyMenuItems.Any())
            {
                var top = _boxTitle.RawRenderBounds.Top - 10;
                var left = lobbyMenuItems.Min(item => item.RawRenderBounds.Left - 10);
                var right = lobbyMenuItems.Max(item => item.RawRenderBounds.Right + 20);
                var bottom = lobbyMenuItems.Max(item => item.RawRenderBounds.Bottom + 20);
                var fullBox = new RawRectangleF(left, top, right, bottom);

                var titleBox = fullBox.Clone();
                titleBox.Bottom = _boxTitle.Bounds.Bottom + 10;
                _engine.Rendering.DrawRectangle(renderTarget, titleBox, _engine.Rendering.Materials.Colors.Red, 2, 2, 0);

                _engine.Rendering.DrawRectangle(renderTarget, fullBox, _engine.Rendering.Materials.Colors.Red, 2, 2, 0);
            }

            base.Render(renderTarget, epoch);
        }
    }
}
