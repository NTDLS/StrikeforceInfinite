using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using NTDLS.Helpers;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;
using System.Drawing;

namespace Ae.Engine.Sprite
{
    /// <summary>
    /// Represents a sprite-based animation asset that supports frame-based playback and rendering within the engine.
    /// </summary>
    /// <remarks>AeSpriteAnimation provides functionality for playing, advancing, and rendering sprite
    /// animations using a bitmap sheet. The animation can be configured with different play modes and frame rates. Use
    /// Play() to start the animation and AdvanceImage() to progress frames based on elapsed time. The class inherits
    /// from AeSpriteMinimalBitmap, enabling integration with engine rendering and asset management. Thread safety is
    /// not guaranteed; access from multiple threads should be synchronized.</remarks>
    [AssetClass("Animation", "", AeBaseAssetType.Image, true)]
    public class AeSpriteAnimation
        : AeSpriteMinimalBitmap
    {
        private bool _isComplete = false;
        private int _frameCount;
        private int _currentFrame = 0;
        private int _currentRow = 0;
        private int _currentColumn = 0;
        private int _rows;
        private int _columns;
        private float _epochsSinceLastAdvancement = int.MaxValue;

        /// <summary>
        /// Gets or sets the playback mode for the animation.
        /// </summary>
        /// <remarks>The playback mode determines how the animation is played, such as looping, reversing,
        /// or playing once. Changing this property affects the behavior of the animation during playback.</remarks>
        public AeAnimationPlayMode PlayMode { get; set; }

        /// <summary>
        /// Gets the number of frames rendered per second.
        /// </summary>
        public float FramesPerSecond { get; private set; } = 1;

        /// <summary>
        /// Initializes a new instance of the AeSpriteAnimation class using the specified engine and asset key.
        /// </summary>
        /// <remarks>The animation is initialized with metadata from the specified asset, including frame
        /// rate, size, and play mode. The initial frame is set upon construction.</remarks>
        /// <param name="engine">The engine instance that manages the animation and provides rendering context.</param>
        /// <param name="assetKey">The key identifying the sprite animation asset to load. Cannot be null or empty.</param>
        public AeSpriteAnimation(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            Location = AeVector.Zero();

            FramesPerSecond = AeRandom.Between(Metadata.FramesPerSecond, 0);
            SetSize(new Size(Metadata.FrameWidth.EnsureNotNull(), Metadata.FrameHeight.EnsureNotNull()));

            PlayMode = Metadata.PlayMode.EnsureNotNull();

            AdvanceImage(0);
        }

        /// <summary>
        /// Sets the size of each frame within the sprite bitmap and updates the internal frame layout accordingly.
        /// </summary>
        /// <remarks>This method recalculates the number of rows and columns based on the provided frame
        /// size. The total frame count is updated to reflect the new layout. Calling this method will affect subsequent
        /// frame indexing and rendering operations.</remarks>
        /// <param name="frameSize">The dimensions of a single frame within the sprite bitmap. The width and height must be positive values.</param>
        public new void SetSize(Size frameSize)
        {
            base.SetSize(frameSize);

            SpriteBitmap.EnsureNotNull();

            _rows = (int)(SpriteBitmap.Size.Height / frameSize.Height);
            _columns = (int)(SpriteBitmap.Size.Width / frameSize.Width);
            _frameCount = _rows * _columns;
        }

        /// <summary>
        /// Starts playback of the animation from the beginning and makes it visible.
        /// </summary>
        /// <remarks>Calling this method resets the animation state and displays it. Use this method to
        /// restart the animation after it has completed or been stopped.</remarks>
        public void Play()
        {
            _isComplete = false;
            _currentFrame = 0;
            _currentRow = 0;
            _currentColumn = 0;
            _epochsSinceLastAdvancement = int.MaxValue;
            IsVisible = true;
        }

        internal override void Render(RenderTarget renderTarget, float epoch)
        {
            var sourceRect = new RawRectangleF(
                _currentColumn * Size.Width,
                _currentRow * Size.Height,
                _currentColumn * Size.Width + Size.Width,
                _currentRow * Size.Height + Size.Height);

            Engine.Rendering.DrawBitmap(
                renderTarget,
                SpriteBitmap ?? throw new NullReferenceException(),
                RenderLocation.X - Size.Width / 2.0f,
                RenderLocation.Y - Size.Height / 2.0f,
                Orientation.RadiansSigned,
                sourceRect,
                new Size2F(Size.Width, Size.Height)
            );
        }

        /// <summary>
        /// Advances the animation by the specified epoch time, updating the current frame and state as appropriate.
        /// </summary>
        /// <remarks>This method updates the animation's frame based on the elapsed time and the
        /// configured play mode. If the animation completes and the play mode is set to delete after play, the
        /// animation is queued for deletion. For infinite play mode, the animation loops back to the start. For single
        /// play mode, the animation stops at the last frame until restarted. Calling this method with invalid values
        /// (zero, negative, NaN, or infinity) has no effect.</remarks>
        /// <param name="epoch">The amount of time, in seconds, to advance the animation. Must be positive and finite; values greater than
        /// 0.25 are clamped to 0.25.</param>
        public void AdvanceImage(float epoch)
        {
            // guard bogus values
            if (epoch <= 0 || float.IsNaN(epoch) || float.IsInfinity(epoch))
                return;

            // Clamp delta to avoid runaway.
            epoch = MathF.Min(epoch, 0.25f);

            _epochsSinceLastAdvancement += epoch;

            float secondsPerFrame = 1.0f / FramesPerSecond;

            if (!_isComplete && _epochsSinceLastAdvancement >= secondsPerFrame)
            {
                _epochsSinceLastAdvancement = 0;

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
                        case AeAnimationPlayMode.DeleteAfterPlay:
                            //Delete the animation sprite.
                            QueueForDelete();
                            break;
                        case AeAnimationPlayMode.Infinite:
                            //Reset the frame, but retain the _lastFrameChange.
                            _currentFrame = 0;
                            _currentColumn = 0;
                            _currentRow = 0;
                            _isComplete = false;
                            break;
                        case AeAnimationPlayMode.Single:
                            //Nothing to do unless the player calls Play() again.
                            break;
                    }

                    return;
                }
            }
        }
    }
}
