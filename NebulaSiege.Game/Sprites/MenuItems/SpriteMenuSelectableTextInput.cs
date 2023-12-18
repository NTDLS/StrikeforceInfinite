﻿using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Menus.BaseClasses;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace NebulaSiege.Game.Sprites.MenuItems
{
    /// <summary>
    /// Menu item that accepts user text input.
    /// </summary>
    internal class SpriteMenuSelectableTextInput : SpriteMenuItem
    {
        public int CharacterLimit { get; set; }

        public SpriteMenuSelectableTextInput(EngineCore core, MenuBase menu, TextFormat format, SolidColorBrush color, NsPoint location, int characterLimit = 100)
            : base(core, menu, format, color, location)
        {
            ItemType = HgMenuItemType.SelectableTextInput;
            Visable = true;
            Velocity = new HgVelocity();
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