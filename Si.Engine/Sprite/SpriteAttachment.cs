using Si.Engine.Sprite._Superclass;
using Si.Library.Mathematics;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite
{
    public class SpriteAttachment : SpriteInteractiveBase
    {
        private SpriteInteractiveBase? _rootOwner = null;
        private SpriteInteractiveBase? _owner = null;
        public SiVector? LocationRelativeToOwner { get; set; }

        /// <summary>
        /// Determines the behavior of a attachment sprite's orientation. By default, it is fixed to owner.
        /// </summary>
        public AttachmentOrientationType OrientationType { get; set; } = AttachmentOrientationType.FixedToOwner;

        /// <summary>
        /// Determines the behavior of a attachment sprite's position. By default, it is fixed to owner.
        /// </summary>
        public AttachmentPositionType PositionType { get; set; } = AttachmentPositionType.FixedToOwner;

        public SpriteAttachment(EngineCore engine, string? imagePath)
            : base(engine, imagePath)
        {
        }

        /// <summary>
        /// We expose the CalculatedLocation because the actual Location is not updated when the sprite is dead.
        /// This allows us to still get the correct location of the attachment even when dead.
        /// </summary>
        public SiVector CalculatedLocation
        {
            get
            {
                if (PositionType == AttachmentPositionType.FixedToOwner && LocationRelativeToOwner != null)
                {
                    // Since the attachment BaseLocation is relative to the top-left corner of the base sprite, we need
                    // to get the position relative to the center of the base sprite image so that we can rotate around that.
                    var attachmentOffset = LocationRelativeToOwner - (RootOwner.Size / 2.0f);

                    // Apply the rotated offset to get the new attachment location relative to the base sprite center.
                    return RootOwner.Location + attachmentOffset.RotatedBy(RootOwner.Orientation.RadiansSigned);
                }

                return Location;
            }
        }

        /// <summary>
        /// We expose the CalculatedOrientation because the actual Orientation is not updated when the sprite is dead.
        /// This allows us to still get the correct Orientation of the attachment even when dead.
        /// </summary>
        public SiVector CalculatedOrientation
        {
            get
            {
                if (OrientationType == AttachmentOrientationType.FixedToOwner)
                {
                    //Make sure the attachment faces forwards.
                    return RootOwner.Orientation.Clone();
                }
                return Orientation;
            }
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            if (PositionType == AttachmentPositionType.FixedToOwner && LocationRelativeToOwner != null)
            {
                Location = CalculatedLocation;
            }

            if (OrientationType == AttachmentOrientationType.FixedToOwner)
            {
                Orientation = CalculatedOrientation;
            }

            base.ApplyMotion(epoch, displacementVector);
        }

        /// <summary>
        /// Gets and caches the root owner of this attachment.
        /// </summary>
        /// <returns></returns>
        public SpriteInteractiveBase RootOwner
        {
            get
            {
                if (_rootOwner == null)
                {
                    _rootOwner = this;

                    do
                    {
                        _rootOwner = _engine.Sprites.GetSpriteByOwner<SpriteInteractiveBase>(_rootOwner.OwnerUID);
                    } while (_rootOwner != null && _rootOwner.OwnerUID != 0);
                }
                return _rootOwner ?? throw new Exception("Attachment must have a root owner.");
            }
        }

        /// <summary>
        /// Gets and caches the root owner of this attachment.
        /// </summary>
        /// <returns></returns>
        public SpriteInteractiveBase Owner
        {
            get
            {
                _owner ??= _engine.Sprites.GetSpriteByOwner<SpriteInteractiveBase>(OwnerUID);
                return _owner ?? throw new Exception("Attachment must have a root owner.");
            }
        }
    }
}
