﻿using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using Si.Library.Mathematics;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite
{
    public class SpriteRadarPositionTextBlock : SpriteTextBlock
    {
        public SpriteRadarPositionTextBlock(EngineCore engine, TextFormat format, SolidColorBrush color, SiVector location)
            : base(engine, format, color, location, false)
        {
            RenderScaleOrder = SiRenderScaleOrder.PreScale;
            Visible = false;
        }

        private float _distanceValue;
        public float DistanceValue
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
