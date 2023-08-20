﻿using AI2D.Engine;
using AI2D.Types;
using System;
using System.Drawing;

namespace AI2D.Actors
{
    public class ActorTextBlock : ActorBase
    {
        private Rectangle? _prevRegion;
        private Font _font;
        private Graphics _genericDC; //Not used for drawing, only measuring.
        private Brush _color;

        public bool IsPositionStatic { get; set; }

        #region Properties.

        double _height = 0;
        public double Height
        {
            get
            {
                if (_height == 0)
                {
                    _height = _genericDC.MeasureString("void", _font).Height;
                }
                return _height;
            }
        }

        private string _lastTextSizeCheck;
        private Size _size = Size.Empty;
        public override Size Size
        {
            get
            {
                if (_size.IsEmpty || _text != _lastTextSizeCheck)
                {
                    var fSize = _genericDC.MeasureString(_text, _font);

                    _size = new Size((int)Math.Ceiling(fSize.Width), (int)Math.Ceiling(fSize.Height));
                    _lastTextSizeCheck = _text;
                }
                return _size;
            }
        }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                //If we have previously drawn text, then we need to invalidate the entire region which it occupied.
                if (_prevRegion != null)
                {
                    _core.Display.DrawingSurface.Invalidate((Rectangle)_prevRegion);
                }

                //Now that we have used _prevRegion to invaldate the previous region, set it to the new region coords.
                //And invalidate them for the new text.
                var stringSize = _genericDC.MeasureString(_text, _font);
                _prevRegion = new Rectangle((int)X, (int)Y, (int)stringSize.Width, (int)stringSize.Height);
                _core.Display.DrawingSurface.Invalidate((Rectangle)_prevRegion);
            }
        }

        #endregion

        public ActorTextBlock(Core core, string font, Brush color, double size, Point<double> location, bool isPositionStatic)
            : base(core)
        {
            IsPositionStatic = isPositionStatic;
            Location = new Point<double>(location);
            _color = color;
            _font = new Font(font, (float)size);
            _genericDC = _core.Display.DrawingSurface.CreateGraphics();
        }

        public new void Render(Graphics dc)
        {
            if (Visable)
            {
                dc.DrawString(_text, _font, _color, (float)X, (float)Y);

                //TODO: Rotate text is required.
            }
        }
    }
}