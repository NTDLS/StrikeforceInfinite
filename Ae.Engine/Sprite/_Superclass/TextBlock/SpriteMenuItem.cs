using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Ae.Engine.Menu._Superclass;
using Ae.Library.Mathematics;
using static Ae.Library.SiConstants;

namespace Ae.Engine.Sprite._Superclass.TextBlock
{
    /// <summary>
    /// Represents a selectable menu item.
    /// </summary>
    public class SpriteMenuItem
        : SpriteTextBlock
    {
        /// <summary>
        /// User object associated with the menu item.
        /// </summary>
        public object? UserData { get; set; }
        public MenuBase Menu { get; private set; }

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

        public SiMenuItemType ItemType { get; set; }

        public SpriteMenuItem(SiEngine engine, MenuBase menu, TextFormat format, SolidColorBrush color, SiVector location)
            : base(engine, format, color, location, true)
        {
            ItemType = SiMenuItemType.Undefined;
            Menu = menu;
            IsVisible = true;
        }
    }
}
