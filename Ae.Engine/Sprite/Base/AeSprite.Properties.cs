using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.Interactive;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Ae.Engine.Sprite.Base
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class AeSprite
    {
        #region Travel Vector.

        private float _speed;
        /// <summary>
        /// The speed that this object can generally travel in any direction.
        /// </summary>
        public float Speed
        {
            get => _speed;
            set
            {
                _speed = value;
                RecalculateMovementVectorFromOrientation();
            }
        }

        private AeVector _movementVector = AeVector.One();
        /// <summary>
        /// Vector representing both speed and direction (Orientation * Speed * Throttle).
        /// Typically set by a call to RecalculateOrientationMovementVector()
        /// </summary>
        public AeVector MovementVector
        {
            get
            {
                return _movementVector;
            }
            set
            {
                if (value.IsNan())
                    throw new Exception("MovementVector is invalid");

                _movementVector = value;
            }
        }

        private float _throttle = 1.0f;
        /// <summary>
        /// Percentage of speed expressed as a decimal percentage from 0.0 (stopped) to float.max.
        /// Note that a throttle of 2.0 is twice the normal speed.
        /// </summary>
        public float Throttle
        {
            get => _throttle;
            set
            {
                _throttle = value.Clamp(0, float.MaxValue);
                RecalculateMovementVectorFromOrientation();
            }
        }

        private float _maxThrottle = 1.0f;
        /// <summary>
        /// The general maximum throttle that can be applied. This can be considered the "boost" speed.
        /// </summary>
        public float MaxThrottle
        {
            get => _maxThrottle;
            set => _maxThrottle = value.Clamp(0, float.MaxValue);
        }

        #endregion

        /// <summary>
        /// Number or radians=per-second to rotate the sprite Orientation along its center at each call to ApplyMotion().
        /// Negative for counter-clockwise, positive for clockwise.
        /// </summary>
        public float RotationSpeed { get; set; } = 0;

        private AeVector _orientation = AeVector.One();
        /// <summary>
        /// The angle in which the sprite is pointing, note that this is NOT the travel angle.
        /// The travel angle is baked into the MovementVector. If you need the movement vector
        /// to follow this direction angle then call RecalculateOrientationMovementVector() after modifying
        /// the PointingAngle.
        /// </summary>
        public AeVector Orientation
        {
            get => _orientation;
            set
            {
                if (value.IsNan())
                    throw new Exception("Orientation is invalid");

                _orientation = value.Clone();
                _orientation.OnChangeEvent += (AeVector vector) => OrientationChanged();
                OrientationChanged();
            }
        }

        /// <summary>
        /// Retrieves the bitmap image associated with the sprite, if available.
        /// </summary>
        /// <returns>A <see cref="SharpDX.Direct2D1.Bitmap"/> representing the sprite's image; <see langword="null"/> if no image
        /// is associated.</returns>
        public SharpDX.Direct2D1.Bitmap? GetImage() => SpriteBitmap;

        /// <summary>
        /// Gets or sets the tag associated with the sprite.
        /// </summary>
        public string? SpriteTag { get; set; }

        /// <summary>
        /// Gets the unique identifier assigned to this instance.
        /// </summary>
        public uint UID { get; private set; } = AeSequenceGenerator.Next();

        /// <summary>
        /// Gets or sets the unique identifier of the owner associated with this entity.
        /// </summary>
        public uint OwnerUID { get; set; }

        /// <summary>
        /// Gets the collection of attachments associated with the sprite.
        /// </summary>
        public List<AeSpriteAttachment> Attachments { get; private set; } = new();

        /// <summary>
        /// Gets or sets the size of the radar dot as a vector.
        /// </summary>
        public AeVector RadarDotSize { get; set; } = new AeVector(4, 4);

        /// <summary>
        /// Gets a value indicating whether the object's render bounds intersect with the current scaled screen bounds.
        /// </summary>
        /// <remarks>Use this property to determine if the object is visible within the current display
        /// area, accounting for any scaling applied to the screen. This can be useful for optimizing rendering or
        /// handling visibility-related logic.</remarks>
        public bool IsWithinCurrentScaledScreenBounds => Engine.Display.GetCurrentScaledScreenBounds().IntersectsWith(RenderBounds);

        /// <summary>
        /// Gets or sets a value indicating whether the item is visually highlighted.
        /// </summary>
        public bool IsHighlighted { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the swept motion rectangle should be visually highlighted.
        /// </summary>
        public bool HighlightSweptMotionRect { get; set; } = false;

        /// <summary>
        /// Gets the current hull health of the ship.
        /// </summary>
        public int HullHealth { get; private set; } = 0; //Ship hit-points.

        /// <summary>
        /// Gets the current shield health value, representing the number of hit points remaining for the shield.
        /// </summary>
        /// <remarks>Shield health determines how much damage the shield can absorb before it is depleted.
        /// Damage to the shield is reduced by half compared to regular health.</remarks>
        public int ShieldHealth { get; private set; } = 0; //Shield hit-points, these take 1/2 damage.

        /// <summary>
        /// The sprite still exists, but is not functional (e.g. its been shot and exploded).
        /// </summary>
        public bool IsDeadOrExploded { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the item is scheduled to be deleted.
        /// </summary>
        public bool IsQueuedForDeletion => _readyForDeletion;

        /// <summary>
        /// If true, the sprite does not respond to changes in background offset.
        /// </summary>
        public bool IsFixedPosition { get; set; }

        /// <summary>
        /// Width and height of the sprite.
        /// </summary>
        public virtual Size Size => _size;

        /// <summary>
        /// Whether the sprite is rendered before speed based scaling.
        /// Note that pre-scaled sprite X,Y is the top, left of the natural screen bounds.
        /// </summary>
        public AeRenderScaleOrder RenderScaleOrder { get; set; } = AeRenderScaleOrder.PreScale;

        /// <summary>
        /// The bounds of the sprite in the universe.
        /// </summary>
        public virtual RectangleF Bounds => new(
                Location.X - Size.Width / 2.0f,
                Location.Y - Size.Height / 2.0f,
                Size.Width,
                Size.Height);

        /// <summary>
        /// The raw bounds of the sprite in the universe.
        /// </summary>
        public virtual RawRectangleF RawBounds => new(
                        Location.X - Size.Width / 2.0f,
                        Location.Y - Size.Height / 2.0f,
                        Location.X - Size.Width / 2.0f + Size.Width,
                        Location.Y - Size.Height / 2.0f + Size.Height);

        /// <summary>
        /// The bounds of the sprite on the display.
        /// </summary>
        public virtual RectangleF RenderBounds => new(
                        RenderLocation.X - Size.Width / 2.0f,
                        RenderLocation.Y - Size.Height / 2.0f,
                        Size.Width,
                        Size.Height);

        /// <summary>
        /// The raw bounds of the sprite on the display.
        /// </summary>
        public virtual RawRectangleF RawRenderBounds => new(
                        RenderLocation.X - Size.Width / 2.0f,
                        RenderLocation.Y - Size.Height / 2.0f,
                        RenderLocation.X - Size.Width / 2.0f + Size.Width,
                        RenderLocation.Y - Size.Height / 2.0f + Size.Height);


        /// <summary>
        /// The x,y, location of the center of the sprite in the universe.
        /// Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public AeVector Location
        {
            get => _location; //Changes made to the location object do not affect the sprite.
            set
            {
                if (value.IsNan())
                    throw new Exception("Location is invalid");

                _location = value.Clone();
                LocationChanged();
            }
        }

        /// <summary>
        /// The top left corner of the sprite in the universe.
        /// </summary>
        public AeVector LocationTopLeft
        {
            get => _location - Size / 2.0f; //Changes made to the location object do not affect the sprite.
            set
            {
                _location = value.Clone();
                LocationChanged();
            }
        }

        /// <summary>
        /// The x,y, location of the center of the sprite on the screen.
        /// Do not modify the X,Y of the returned location, it will have no effect.
        /// </summary>
        public AeVector RenderLocation
        {
            get
            {
                if (IsFixedPosition)
                {
                    return _location;
                }
                else
                {
                    return _location - Engine.Display.CameraPosition;
                }
            }
        }

        /// <summary>
        /// The X location of the center of the sprite in the universe.
        /// </summary>
        public float X
        {
            get => _location.X;
            set
            {
                _location.X = value;
                LocationChanged();
            }
        }

        /// <summary>
        /// The Y location of the center of the sprite in the universe.
        /// </summary>
        public float Y
        {
            get => _location.Y;
            set
            {
                _location.Y = value;
                LocationChanged();
            }
        }

        /// <summary>
        /// The Z location. Given that this is a 2d engine, the Z order is just a render order.
        /// </summary>
        public int Z { get; set; } = 0;

        private bool _isVisible = true;

        /// <summary>
        /// Gets or sets a value indicating whether the item is currently visible.
        /// </summary>
        /// <remarks>Changing this property triggers visibility change notifications. The item is
        /// considered visible only if it is not marked for deletion.</remarks>
        public bool IsVisible
        {
            get => _isVisible && !_readyForDeletion;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnVisibilityChanged?.Invoke(this);
                    VisibilityChanged();
                }
            }
        }
    }
}
