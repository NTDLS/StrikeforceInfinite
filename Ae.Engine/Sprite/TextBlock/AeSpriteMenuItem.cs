using Ae.Engine.Mathematics;
using Ae.Engine.Menu;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

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

        /// <summary>
        /// Gets the menu associated with the current instance.
        /// </summary>
        public AeMenu Menu { get; private set; }

        private bool _selected = false;

        /// <summary>
        /// Gets or sets a value indicating whether this menu item is currently selected.
        /// </summary>
        /// <remarks>Changing the value of this property triggers the menu's selection changed event. This
        /// property can be used to programmatically select or deselect a menu item.</remarks>
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

        /// <summary>
        /// Gets or sets the type of the menu item.
        /// </summary>
        public AeMenuItemType ItemType { get; set; }

        /// <summary>
        /// Initializes a new instance of the AeSpriteMenuItem class and associates it with the specified menu and
        /// rendering parameters.
        /// </summary>
        /// <param name="engine">The engine instance used for rendering and managing the menu item.</param>
        /// <param name="menu">The menu to which this item belongs. Cannot be null.</param>
        /// <param name="format">The text formatting to apply to the menu item's label.</param>
        /// <param name="color">The brush used to render the menu item's text color.</param>
        /// <param name="location">The location of the menu item within the menu, specified as a vector.</param>
        public AeSpriteMenuItem(AeEngine engine, AeMenu menu, TextFormat format, SolidColorBrush color, AeVector location)
            : base(engine, format, color, location, true)
        {
            ItemType = AeMenuItemType.Undefined;
            Menu = menu;
            IsVisible = true;
        }
    }
}
