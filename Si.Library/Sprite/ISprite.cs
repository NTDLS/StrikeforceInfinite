﻿using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;

namespace Si.Library.Sprite
{
    public interface ISprite
    {
        public SiPoint Location { get; set; }
        public SiVelocity Velocity { get; set; }
    }
}
