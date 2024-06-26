﻿using System.Drawing;

namespace Si.Library.Mathematics
{
    public class SiQuadrant
    {
        public Point Key { get; private set; }
        public Rectangle Bounds { get; private set; }

        public SiQuadrant(Point key, Rectangle bounds)
        {
            Key = key;
            Bounds = bounds;
        }

        public override string ToString() => Key.ToString();
    }
}