using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using System;

namespace Ae.Engine.Sprite.Interactive
{
    /// <summary>
    /// Represents a sprite attachment that can be positioned and oriented relative to an owner sprite within the
    /// engine. Provides mechanisms for fixed or dynamic attachment behaviors and exposes calculated location and
    /// orientation even when the sprite is inactive.
    /// </summary>
    /// <remarks>AeSpriteAttachment is used to visually or logically attach a sprite to another, enabling
    /// complex composite behaviors such as equipment, effects, or decorations. The attachment's position and
    /// orientation can be configured to follow the owner or remain independent. Calculated properties allow access to
    /// accurate state regardless of the attachment's lifecycle. This class is typically used in scenarios where sprite
    /// relationships and dynamic positioning are required.</remarks>
    [AssetClass("Attachment", "", AeBaseAssetType.Image, true)]
    public class AeSpriteAttachment
        : AeSpriteInteractive
    {
        private AeSpriteInteractive? _rootOwner = null;

        /// <summary>
        /// Gets or sets the location of the object relative to its owner.
        /// </summary>
        public AeVector? LocationRelativeToOwner { get; set; }

        /// <summary>
        /// Gets the unique identifier for the associated asset.
        /// </summary>
        public string? AssetKey { get; private set; }

        /// <summary>
        /// Determines the behavior of a attachment sprite's orientation.
        /// </summary>
        public AeAttachmentOrientationType AttachmentOrientationType { get; set; }

        /// <summary>
        /// Determines the behavior of a attachment sprite's position.
        /// </summary>
        public AeAttachmentPositionType AttachmentPositionType { get; set; }

        /// <summary>
        /// Initializes a new instance of the AeSpriteAttachment class using the specified engine and asset key.
        /// </summary>
        /// <param name="engine">The engine instance used to manage and render the sprite attachment.</param>
        /// <param name="assetKey">The key identifying the asset to attach. Can be null if no asset is specified.</param>
        public AeSpriteAttachment(AeEngine engine, string? assetKey)
            : base(engine, assetKey)
        {
            AssetKey = assetKey;
        }

        /// <summary>
        /// We expose the CalculatedLocation because the actual Location is not updated when the sprite is dead.
        /// This allows us to still get the correct location of the attachment even when dead.
        /// </summary>
        public AeVector CalculatedLocation
        {
            get
            {
                if (AttachmentPositionType == AeAttachmentPositionType.FixedToOwner && LocationRelativeToOwner != null)
                {
                    // Since the attachment BaseLocation is relative to the top-left corner of the base sprite, we need
                    // to get the position relative to the center of the base sprite image so that we can rotate around that.
                    var attachmentOffset = LocationRelativeToOwner - (RootOwner.Size / 2.0f);

                    // Apply the rotated offset to get the new attachment location relative to the base sprite center.
                    return RootOwner.Location + attachmentOffset.RotatedBy(RootOwner.Orientation.DegreesSigned);
                }

                return Location;
            }
        }

        /// <summary>
        /// We expose the CalculatedOrientation because the actual Orientation is not updated when the sprite is dead.
        /// This allows us to still get the correct Orientation of the attachment even when dead.
        /// </summary>
        public AeVector CalculatedOrientation
        {
            get
            {
                if (AttachmentOrientationType == AeAttachmentOrientationType.FixedToOwner)
                {
                    //Make sure the attachment faces forwards.
                    return RootOwner.Orientation.Clone();
                }
                return Orientation;
            }
        }

        /// <summary>
        /// Updates the object's position and orientation based on the specified epoch and camera displacement, applying
        /// attachment constraints as defined.
        /// </summary>
        /// <remarks>If the object is attached to an owner with fixed position or orientation constraints,
        /// its location and orientation are updated accordingly before applying motion. This method overrides the base
        /// implementation to enforce attachment rules.</remarks>
        /// <param name="epoch">The time value, in seconds, representing the current simulation epoch. Used to determine motion updates.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera relative to the object. Influences how the object's motion is applied.</param>
        public override void ApplyMotion(float epoch, AeVector cameraDisplacement)
        {
            if (AttachmentPositionType == AeAttachmentPositionType.FixedToOwner && LocationRelativeToOwner != null)
            {
                Location = CalculatedLocation;
            }

            if (AttachmentOrientationType == AeAttachmentOrientationType.FixedToOwner)
            {
                Orientation = CalculatedOrientation;
            }

            base.ApplyMotion(epoch, cameraDisplacement);
        }

        /// <summary>
        /// Gets and caches the root owner of this attachment.
        /// </summary>
        /// <returns></returns>
        public AeSpriteInteractive RootOwner
        {
            get
            {
                if (_rootOwner == null)
                {
                    _rootOwner = this;

                    do
                    {
                        _rootOwner = Engine.Sprites.GetSpriteByOwner<AeSpriteInteractive>(_rootOwner.OwnerUID);
                    } while (_rootOwner != null && _rootOwner.OwnerUID != 0);
                }
                return _rootOwner ?? throw new Exception("Attachment must have a root owner.");
            }
        }
    }
}
