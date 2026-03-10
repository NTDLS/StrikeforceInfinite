using Ae.Engine.Mathematics;
using Ae.Engine.Menu._Superclass;
using Ae.Engine.Sprite.TextBlock;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.MenuItem
{
    /// <summary>
    /// Menu item that accepts user text input.
    /// </summary>
    public class SpriteMenuSelectableTextInput
        : SpriteMenuItem
    {
        public int CharacterLimit { get; set; }

        public SpriteMenuSelectableTextInput(AeEngine engine, MenuBase menu, TextFormat format, SolidColorBrush color, AeVector location, int characterLimit = 100)
            : base(engine, menu, format, color, location)
        {
            ItemType = SiMenuItemType.SelectableTextInput;
            IsVisible = true;
            CharacterLimit = characterLimit;
        }

        public void Backspace()
        {
            if (Text.Length > 0)
            {
                Text = Text.Substring(0, Text.Length - 1);
            }
        }

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
