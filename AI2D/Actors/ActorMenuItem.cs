﻿using AI2D.Engine;
using AI2D.Types;
using System.Drawing;

namespace AI2D.Actors
{
    public class ActorMenuItem : ActorTextBlock
    {
        public bool Selected { get; set; }
        public enum MenuItemType
        {
            Title,
            Text,
            Item
        }

        public string Name { get; set; }

        public MenuItemType ItemType { get; set; }

        public ActorMenuItem(Core core, string font, Brush color, double size, Point<double> location)
            : base(core, font, color, size, location, true)
        {
            Visable = true;
            Velocity = new Velocity<double>();
        }
    }
}
