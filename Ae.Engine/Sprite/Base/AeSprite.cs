using Ae.Engine.Audio;
using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Interactive;
using Ae.Engine.Sprite.Interactive.Ship;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Ae.Engine.Sprite.Base
{
    /// <summary>
    /// Represents a single item that can be rendered to the screen. All on-screen objects are derived from this class.
    /// </summary>
    public partial class AeSprite
        : IAeSprite
    {
        /// <summary>
        /// Gets the instance of the underlying AeEngine used by the class.
        /// </summary>
        public AeEngine Engine { get; private set; }

        /// <summary>
        /// Gets the collection of audio clips associated with this instance.
        /// </summary>
        public List<AeAudioClip>? Sounds { get; private set; }

        /// <summary>
        /// Gets the bitmap used to render the sprite image.
        /// </summary>
        public SharpDX.Direct2D1.Bitmap? SpriteBitmap { get; private set; }
        private bool _readyForDeletion;
        private AeVector _location = new();
        private Size _size;

        private AssetMetadata? _metadata = null;

        /// <summary>
        /// Gets the metadata associated with the asset.
        /// </summary>
        /// <remarks>Throws a NullReferenceException if the metadata has not been initialized. Ensure that
        /// the asset is properly loaded before accessing this property.</remarks>
        public AssetMetadata Metadata => _metadata ?? throw new NullReferenceException();

        /// <summary>
        /// Initializes a new instance of the AeSprite class using the specified engine and asset key.
        /// </summary>
        /// <remarks>The sprite's initial highlight state and orientation are determined by the engine's
        /// settings. Asset metadata is loaded during construction.</remarks>
        /// <param name="engine">The engine instance that manages the sprite's lifecycle and rendering context. Cannot be null.</param>
        /// <param name="assetKey">The key identifying the asset to use for the sprite's image and metadata. If null, a default image may be
        /// used.</param>
        public AeSprite(AeEngine engine, string? assetKey)
        {
            Engine = engine;

            IsHighlighted = Engine.Settings.HighlightAllSprites;
            Orientation = AeVector.One();

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

            var asset = Engine.Assets.GetAsset(assetKey);

            _metadata = asset.Metadata;

            if (AeConstants.ImageTypes.Contains(asset.BaseType, StringComparer.OrdinalIgnoreCase))
            {
                SpriteBitmap = Engine.Assets.GetBitmap(assetKey);
                _size = new Size((int)SpriteBitmap.Size.Width, (int)SpriteBitmap.Size.Height);
            }

            if (Metadata.SoundAssetKeys != null)
            {
                Sounds = new List<AeAudioClip>();
                foreach (var soundAssetKey in Metadata.SoundAssetKeys)
                {
                    Sounds.Add(Engine.Assets.GetAudio(soundAssetKey));
                }
            }

            // Set standard variables here:
            Speed = AeRandom.Between(Metadata.Speed, 0);
            Throttle = Metadata.Throttle ?? 1; //We assume a throttle of 100% becasuse this is a factor of speed - I dont want to require throttle when speed is specified.
            MaxThrottle = Metadata.MaxThrottle ?? 0;

            SetHullHealth(Metadata.Hull ?? 0);
            SetShieldHealth(Metadata.Shields ?? 0);

            if (this is AeSpriteInteractive interactive)
            {
                Metadata.WeaponAssetKeys?.ForEach(weaponAssetKey =>
                {
                    var weaponMetadata = Engine.Assets.GetAsset(weaponAssetKey).Metadata;
                    interactive.AddWeapon(weaponAssetKey, weaponMetadata.MunitionCount ?? 0);
                });

                Metadata.Attachments?.ForEach(attachment =>
                {
                    if (attachment.AssetKey == null) throw new InvalidOperationException("AssetKey cannot be null");
                    var locationRelativeToOwner = new AeVector(attachment.AttachmentPosition?.X ?? 0, attachment.AttachmentPosition?.Y ?? 0);
                    interactive.AttachOfType(attachment.AssetKey, locationRelativeToOwner, (sprite) =>
                    {
                        //We take the orientation and position type of the attachment from the attachment section in the parent metadata if it is specified,
                        //   otherwise we use the default values set in the SpriteAttachment class.
                        sprite.AttachmentOrientationType = attachment.AttachmentOrientationType ?? AeAttachmentOrientationType.Independent;
                        sprite.AttachmentPositionType = attachment.AttachmentPositionType ?? AeAttachmentPositionType.Independent;
                    });
                });
            }

            if (this is AeSpritePlayer player)
            {
                if (!string.IsNullOrEmpty(Metadata?.PrimaryWeaponAssetKey))
                {
                    var weaponMetadata = Engine.Assets.GetAsset(Metadata.PrimaryWeaponAssetKey).Metadata;
                    player.SetPrimaryWeapon(Metadata.PrimaryWeaponAssetKey, weaponMetadata.MunitionCount ?? 0);
                    player.SelectFirstAvailableUsableSecondaryWeapon();
                }
            }
        }

        /// <summary>
        /// Marks the current object and its attachments for deletion and hides them from view.
        /// </summary>
        /// <remarks>Calling this method sets the object as ready for deletion, makes it invisible, and
        /// recursively queues all attachments for deletion. The method also triggers the OnQueuedForDelete event if it
        /// is subscribed. Once queued for deletion, the object should not be used for further operations.</remarks>
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
            X = Engine.Display.TotalCanvasSize.Width / 2 /*- Size.Width / 2*/;
            Y = Engine.Display.TotalCanvasSize.Height / 2 /*- Size.Height / 2*/;
        }

        /// <summary>
        /// Sets the hull health to the specified number of points.
        /// </summary>
        /// <param name="points">The number of health points to assign to the hull. Must be a non-negative value.</param>
        public void SetHullHealth(int points)
        {
            HullHealth = 0;
            AddHullHealth(points);
        }

        /// <summary>
        /// Adds the specified number of points to the hull health, ensuring the value remains within valid bounds.
        /// </summary>
        /// <param name="pointsToAdd">The number of health points to add to the hull. Can be negative to reduce health. The resulting hull health
        /// will be clamped between zero and the maximum allowed value.</param>
        public virtual void AddHullHealth(int pointsToAdd)
            => HullHealth = (HullHealth + pointsToAdd).Clamp(0, Engine.Settings.MaxHullHealth);

        /// <summary>
        /// Sets the shield health to the specified number of points.
        /// </summary>
        /// <param name="points">The number of shield health points to assign. Must be a non-negative value.</param>
        public virtual void SetShieldHealth(int points)
        {
            ShieldHealth = 0;
            AddShieldHealth(points);
        }

        /// <summary>
        /// Adds the specified number of points to the shield health, ensuring the value remains within valid limits.
        /// </summary>
        /// <param name="pointsToAdd">The number of shield health points to add. Can be negative to reduce shield health. The resulting shield
        /// health will be clamped between 0 and the maximum allowed by the engine settings.</param>
        public virtual void AddShieldHealth(int pointsToAdd)
            => ShieldHealth = (ShieldHealth + pointsToAdd).Clamp(0, Engine.Settings.MaxShieldHealth);

        /// <summary>
        /// Sets the bitmap used for rendering the sprite.
        /// </summary>
        /// <remarks>Calling this method updates the sprite's size to match the dimensions of the provided
        /// bitmap.</remarks>
        /// <param name="bitmap">The bitmap to associate with the sprite. Cannot be null.</param>
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
        public virtual void ApplyMotion(float epoch, AeVector cameraDisplacement)
        {
            //Perform any auto-rotation.
            Orientation.Radians += RotationSpeed * epoch;

            //Move the sprite based on its vector.
            Location += MovementVector * epoch;
        }

        /// <summary>
        /// Performs cleanup operations for the current object, including hiding it and scheduling associated resources
        /// for deletion.
        /// </summary>
        /// <remarks>Call this method to release resources and detach any attachments owned by the object.
        /// After cleanup, the object will no longer be visible and its related sprites and attachments will be queued
        /// for deletion. This method is intended to be overridden in derived classes to provide additional cleanup
        /// logic as needed.</remarks>
        public virtual void Cleanup()
        {
            IsVisible = false;

            Engine.Sprites.QueueAllForDeletionByOwner(UID);

            foreach (var attachments in Attachments)
            {
                attachments.QueueForDelete();
            }
        }

        /// <summary>
        /// Generates a formatted, multi-line string containing inspection details for the current sprite instance.
        /// </summary>
        /// <remarks>The returned string is intended for debugging or diagnostic purposes and provides a
        /// snapshot of the sprite's current state. The format is suitable for display in logs or inspection
        /// panels.</remarks>
        /// <returns>A string representing the inspection information for the sprite, including identifiers, state, location, and
        /// relevant properties. If the instance is an enemy sprite, additional AI controller information is included.</returns>
        public string GetInspectionText()
        {
            string extraInfo = string.Empty;

            if (this is AeSpriteEnemy enemy)
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
                + $"\t       Background Offset: {Engine.Display.CameraPosition}\r\n"
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
