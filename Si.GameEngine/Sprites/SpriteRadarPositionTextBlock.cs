﻿using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Si.GameEngine.Core;
using Si.Library.Mathematics;
using Si.Library.Mathematics.Geometry;
using static Si.Library.SiConstants;

namespace Si.GameEngine.Sprites
{
    public class SpriteRadarPositionTextBlock : SpriteTextBlock
    {
        public SpriteRadarPositionTextBlock(GameEngineCore gameEngine, TextFormat format, SolidColorBrush color, SiVector location)
            : base(gameEngine, format, color, location, false)
        {
            RenderScaleOrder = SiRenderScaleOrder.PreScale;
            Visable = false;
            Velocity = new SiVelocity();
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
