﻿using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types;
using NebulaSiege.Game.Engine.Types.Geometry;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;

namespace NebulaSiege.Game.Sprites
{
    internal class SpriteRadarPositionTextBlock : SpriteTextBlock
    {
        public SpriteRadarPositionTextBlock(EngineCore core, TextFormat format, SolidColorBrush color, NsPoint location)
            : base(core, format, color, location, false)
        {
            Visable = false;
            Velocity = new HgVelocity();
        }

        private double _distanceValue;
        public double DistanceValue
        {
            get
            {
                return _distanceValue;
            }
            set
            {
                _distanceValue = value;
                Text = DistanceValue.ToString("#,#");
            }
        }
    }
}