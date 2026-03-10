using Ae.Engine.Mathematics;
using Ae.Engine.Menu;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.TextBlock
{
    /// <summary>
    /// Represents a selectable menu item.
    /// </summary>
    public class AeSpriteMenuItem
        : AeSpriteTextBlock
    {
        /// <summary>
        /// User object associated with the menu item.
        /// </summary>
        public object? UserData { get; set; }
        public AeMenu Menu { get; private set; }

        private bool _selected = false;

        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                if (_selected != value)
                {
                    Menu.InvokeSelectionChanged(this);
                }
                _selected = value;
            }
        }

        public AeMenuItemType ItemType { get; set; }

        public AeSpriteMenuItem(AeEngine engine, AeMenu menu, TextFormat format, SolidColorBrush color, AeVector location)
            : base(engine, format, color, location, true)
        {
            ItemType = AeMenuItemType.Undefined;
            Menu = menu;
            IsVisible = true;
        }
    }
}
