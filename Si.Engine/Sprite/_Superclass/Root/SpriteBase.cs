using NTDLS.Helpers;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Library.Sprite;
using System;
using System.Drawing;

namespace Si.Engine.Sprite._Superclass._Root
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class SpriteBase
        : ISprite
    {
        protected EngineCore _engine;

        public SharpDX.Direct2D1.Bitmap? SpriteBitmap { get; private set; }
        private bool _readyForDeletion;
        private SiVector _location = new();
        private Size _size;

        private AssetMetadata? _metadata = null;
        public AssetMetadata Metadata => _metadata ?? throw new NullReferenceException();

        public SpriteBase(EngineCore engine, string? assetKey)
        {
            _engine = engine;

            IsHighlighted = _engine.Settings.HighlightAllSprites;
            Orientation = SiVector.One();

            SetImageAndLoadMetadata(assetKey);
        }

        /// <summary>
        /// Sets the sprites image, sets speed, shields, adds attachments and weapons
        /// from a .json file in the same path with the same name as the sprite image.
        /// </summary>
        private void SetImageAndLoadMetadata(string? assetKey)
        {
            if (string.IsNullOrEmpty(assetKey))
            {
                _metadata = new AssetMetadata();
                return;
            }

            var asset = _engine.Assets.GetAsset(assetKey);

            _metadata = asset.Metadata;

            if (SiConstants.ImageTypes.Contains(asset.BaseType, StringComparer.OrdinalIgnoreCase))
            {
                SpriteBitmap = _engine.Assets.GetBitmap(assetKey);
                _size = new Size((int)SpriteBitmap.Size.Width, (int)SpriteBitmap.Size.Height);
            }

            // Set standard variables here:
            Speed = SiRandom.Between(Metadata.Speed, 0);
            Throttle = Metadata.Throttle ?? 1; //We assume a throttle of 100% becasuse this is a factor of speed - I dont want to require throttle when speed is specified.
            MaxThrottle = Metadata.MaxThrottle ?? 0;

            SetHullHealth(Metadata.Hull ?? 0);
            SetShieldHealth(Metadata.Shields ?? 0);

            if (this is SpriteInteractiveBase interactive)
            {
                Metadata.Weapons?.ForEach(weapon =>
                {
                    interactive.AddWeapon(weapon.Type.EnsureNotNull(), weapon.MunitionCount ?? 0);
                });

                Metadata.Attachments?.ForEach(attachment =>
                {
                    if (attachment.Type == null) throw new InvalidOperationException("Attachment type cannot be null");
                    var locationRelativeToOwner = new SiVector(attachment.AttachmentPosition?.X ?? 0, attachment.AttachmentPosition?.Y ?? 0);
                    interactive.AttachOfType(attachment.Type, locationRelativeToOwner, (sprite) =>
                    {
                        //We take the orientation and position type of the attachment from the attachment section in the parent metadata if it is specified,
                        //   otherwise we use the default values set in the SpriteAttachment class.
                        sprite.AttachmentOrientationType = attachment.AttachmentOrientationType ?? SiConstants.AttachmentOrientationType.Independent;
                        sprite.AttachmentPositionType = attachment.AttachmentPositionType ?? SiConstants.AttachmentPositionType.Independent;
                    });
                });
            }

            if (this is SpritePlayer player)
            {
                if (Metadata?.PrimaryWeapon?.Type != null)
                {
                    player.SetPrimaryWeapon(Metadata.PrimaryWeapon.Type, Metadata.PrimaryWeapon.MunitionCount ?? 0);
                    player.SelectFirstAvailableUsableSecondaryWeapon();
                }
            }
        }


        public void QueueForDelete()
        {
            _readyForDeletion = true;
            IsVisible = false;

            foreach (var attachment in Attachments)
            {
                attachment.QueueForDelete();
            }

            OnQueuedForDelete?.Invoke(this);
        }

        /// <summary>
        /// Sets the sprites center to the center of the screen.
        /// </summary>
        public void CenterInUniverse()
        {
            X = _engine.Display.TotalCanvasSize.Width / 2 /*- Size.Width / 2*/;
            Y = _engine.Display.TotalCanvasSize.Height / 2 /*- Size.Height / 2*/;
        }

        public void SetHullHealth(int points)
        {
            HullHealth = 0;
            AddHullHealth(points);
        }

        public virtual void AddHullHealth(int pointsToAdd)
            => HullHealth = (HullHealth + pointsToAdd).Clamp(0, _engine.Settings.MaxHullHealth);

        public virtual void SetShieldHealth(int points)
        {
            ShieldHealth = 0;
            AddShieldHealth(points);
        }

        public virtual void AddShieldHealth(int pointsToAdd)
            => ShieldHealth = (ShieldHealth + pointsToAdd).Clamp(0, _engine.Settings.MaxShieldHealth);

        public void SetBitmap(SharpDX.Direct2D1.Bitmap bitmap)
        {
            SpriteBitmap = bitmap;
            _size = new Size((int)SpriteBitmap.Size.Width, (int)SpriteBitmap.Size.Height);
        }

        /// <summary>
        /// Sets the size of the sprite. This is generally set by a call to SetImage() but some sprites (such as particles) have no images.
        /// </summary>
        /// <param name="size"></param>
        public void SetSize(Size size)
            => _size = size;

        /// <summary>
        /// Moves the sprite based on its movement vector and the epoch.
        /// </summary>
        /// <param name="displacementVector"></param>
        public virtual void ApplyMotion(float epoch, SiVector displacementVector)
        {
            //Perform any auto-rotation.
            Orientation.Radians += RotationSpeed * epoch;

            //Move the sprite based on its vector.
            Location += MovementVector * epoch;
        }

        public virtual void Cleanup()
        {
            IsVisible = false;

            _engine.Sprites.QueueAllForDeletionByOwner(UID);

            foreach (var attachments in Attachments)
            {
                attachments.QueueForDelete();
            }
        }

        public string GetInspectionText()
        {
            string extraInfo = string.Empty;

            if (this is SpriteEnemyBase enemy)
            {
                extraInfo =
                  $"\t           AI Controller: {enemy.CurrentAIController}\r\n";
            }

            return
                  $"\t                     UID: {UID}\r\n"
                + $"\t               Owner UID: {OwnerUID:n0}\r\n"
                + $"\t                    Type: {GetType().Name}\r\n"
                + $"\t                     Tag: {SpriteTag:n0}\r\n"
                + $"\t             Is Visible?: {IsVisible:n0}\r\n"
                + $"\t                    Size: {Size:n0}\r\n"
                + $"\t                  Bounds: {Bounds:n0}\r\n"
                + $"\t       Ready for Delete?: {IsQueuedForDeletion}\r\n"
                + $"\t                Is Dead?: {IsDeadOrExploded}\r\n"
                + $"\t         Render-Location: {RenderLocation}\r\n"
                + $"\t                Location: {Location}\r\n"
                + $"\t                   Angle: {Orientation}\r\n"
                + $"\t                          {Orientation.DegreesSigned:n2}deg\r\n"
                + $"\t                          {Orientation.RadiansSigned:n2}rad\r\n"
                + extraInfo
                + $"\t       Background Offset: {_engine.Display.CameraPosition}\r\n"
                + $"\t                  Thrust: {MovementVector * 100:n2}\r\n"
                + $"\t                   Boost: {Throttle * 100:n2}\r\n"
                + $"\t                    Hull: {HullHealth:n0}\r\n"
                + $"\t                  Shield: {ShieldHealth:n0}\r\n"
                + $"\t             Attachments: {Attachments?.Count ?? 0:n0}\r\n"
                + $"\t               Highlight: {IsHighlighted}\r\n"
                + $"\t       Is Fixed Position: {IsFixedPosition}\r\n"
                //+ $"\t            Is Locked On: {IsLockedOnHard}\r\n"
                //+ $"\t     Is Locked On (Soft): {IsLockedOnSoft:n0}\r\n"
                + $"\tIn Current Scaled Bounds: {IsWithinCurrentScaledScreenBounds}\r\n"
                + $"\t          Visible Bounds: {Bounds}\r\n";
        }

    }
}
