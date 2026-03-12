using Ae.Engine.Mathematics;
using Ae.Engine.Menu;
using Ae.Engine.Sprite.TextBlock;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace Ae.Engine.Sprite.MenuItem
{
    /// <summary>
    /// Menu item that accepts user text input.
    /// </summary>
    public class AeSpriteMenuSelectableTextInput
        : AeSpriteMenuItem
    {
        /// <summary>
        /// Gets or sets the maximum number of characters allowed for input.
        /// </summary>
        public int CharacterLimit { get; set; }

        /// <summary>
        /// Initializes a new instance of the AeSpriteMenuSelectableTextInput class, representing a selectable text
        /// input item within a menu.
        /// </summary>
        /// <param name="engine">The engine instance used to render and manage menu items.</param>
        /// <param name="menu">The menu to which this text input item belongs.</param>
        /// <param name="format">The text formatting settings applied to the input text.</param>
        /// <param name="color">The brush used to render the input text color.</param>
        /// <param name="location">The position of the text input item within the menu.</param>
        /// <param name="characterLimit">The maximum number of characters allowed in the input. Defaults to 100.</param>
        public AeSpriteMenuSelectableTextInput(AeEngine engine, AeMenu menu, TextFormat format, SolidColorBrush color, AeVector location, int characterLimit = 100)
            : base(engine, menu, format, color, location)
        {
            ItemType = AeMenuItemType.SelectableTextInput;
            IsVisible = true;
            CharacterLimit = characterLimit;
        }

        /// <summary>
        /// Removes the last character from the current text, if any.
        /// </summary>
        /// <remarks>If the text is already empty, this method has no effect.</remarks>
        public void Backspace()
        {
            if (Text.Length > 0)
            {
                Text = Text.Substring(0, Text.Length - 1);
            }
        }

        /// <summary>
        /// Appends the specified text to the current value, truncating if the combined length exceeds the character
        /// limit.
        /// </summary>
        /// <remarks>If the combined length of the current value and the appended text exceeds the
        /// character limit, only the first characters up to the limit are retained. This method does not throw an
        /// exception if truncation occurs.</remarks>
        /// <param name="text">The text to append to the existing value. Cannot be null.</param>
        public void Append(string text)
        {
            var totalString = Text + text;

            if (totalString.Length > CharacterLimit)
            {
                Text = totalString.Substring(0, CharacterLimit);
            }
            else
            {
                Text = totalString;
            }
        }

    }
}
