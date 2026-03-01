using NTDLS.Helpers;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using Si.Library;
using Si.Library.Mathematics;
using System;
using System.Drawing;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite
{
    public class SpriteAnimation
        : SpriteMinimalBitmap
    {
        private bool _isComplete = false;
        private int _frameCount;
        private int _currentFrame = 0;
        private int _currentRow = 0;
        private int _currentColumn = 0;
        private int _rows;
        private int _columns;
        private float _epochsSinceLastAdvancment = int.MaxValue;

        public SiAnimationPlayMode PlayMode { get; set; }
        public float FramesPerSecond { get; private set; } = 1;

        public SpriteAnimation(EngineCore engine, string assetKey)
            : base(engine, assetKey)
        {
            Location = SiVector.Zero();

            FramesPerSecond = SiRandom.Between(Metadata.FramesPerSecond, 0);
            SetSize(new Size(Metadata.FrameWidth.EnsureNotNull(), Metadata.FrameHeight.EnsureNotNull()));

            PlayMode = Metadata.PlayMode.EnsureNotNull();

            AdvanceImage(0);
        }

        public new void SetSize(Size frameSize)
        {
            base.SetSize(frameSize);

            SpriteBitmap.EnsureNotNull();

            _rows = (int)(SpriteBitmap.Size.Height / frameSize.Height);
            _columns = (int)(SpriteBitmap.Size.Width / frameSize.Width);
            _frameCount = _rows * _columns;
        }

        public void Play()
        {
            _isComplete = false;
            _currentFrame = 0;
            _currentRow = 0;
            _currentColumn = 0;
            _epochsSinceLastAdvancment = int.MaxValue;
            IsVisible = true;
        }

        public override void Render(RenderTarget renderTarget, float epoch)
        {
            var sourceRect = new RawRectangleF(
                _currentColumn * Size.Width,
                _currentRow * Size.Height,
                _currentColumn * Size.Width + Size.Width,
                _currentRow * Size.Height + Size.Height);

            _engine.Rendering.DrawBitmap(
                renderTarget,
                SpriteBitmap ?? throw new NullReferenceException(),
                RenderLocation.X - Size.Width / 2.0f,
                RenderLocation.Y - Size.Height / 2.0f,
                Orientation.RadiansSigned,
                sourceRect,
                new Size2F(Size.Width, Size.Height)
            );
        }

        public void AdvanceImage(float epoch)
        {
            // guard bogus values
            if (epoch <= 0 || float.IsNaN(epoch) || float.IsInfinity(epoch))
                return;

            // Clamp delta to avoid runaway.
            epoch = MathF.Min(epoch, 0.25f);

            _epochsSinceLastAdvancment += epoch;

            float secondsPerFrame = 1.0f / FramesPerSecond;

            if (!_isComplete && _epochsSinceLastAdvancment >= secondsPerFrame)
            {
                _epochsSinceLastAdvancment = 0;

                if (++_currentColumn == _columns)
                {
                    _currentColumn = 0;
                    _currentRow++;
                }

                _currentFrame++;

                if (_currentFrame == _frameCount)
                {
                    _isComplete = true;
                    switch (PlayMode)
                    {
                        case SiAnimationPlayMode.DeleteAfterPlay:
                            //Delete the animation sprite.
                            QueueForDelete();
                            break;
                        case SiAnimationPlayMode.Infinite:
                            //Reset the frame, but retain the _lastFrameChange.
                            _currentFrame = 0;
                            _currentColumn = 0;
                            _currentRow = 0;
                            _isComplete = false;
                            break;
                        case SiAnimationPlayMode.Single:
                            //Nothing to do unless the player calls Play() again.
                            break;
                    }

                    return;
                }
            }
        }
    }
}
