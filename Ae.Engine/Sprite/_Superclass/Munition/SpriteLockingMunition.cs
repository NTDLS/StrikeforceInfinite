using Ae.Engine.Sprite._Superclass.Interactive;
using Ae.Library.ExtensionMethods;
using Ae.Library.Mathematics;
using Ae.Library.Metadata;
using System;
using static Ae.Library.AeConstants;

namespace Ae.Engine.Sprite._Superclass.Munition
{
    /// <summary>
    /// Guided munitions need to be locked onto a target before they are fired. They will adjust heading within given parameters to hit the locked target.
    /// </summary>
    [AssetClass("Munition - Locking Type", "", AeBaseAssetType.Image, true)]
    internal class SpriteLockingMunition
        : SpriteMunition
    {
        public SpriteInteractive? LockedTarget { get; private set; }

        public SpriteLockingMunition(AeEngine engine, SpriteWeapon weapon, SpriteInteractive firedFrom, string assetKey,
             SpriteInteractive? lockedTarget, AeVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
            LockedTarget = lockedTarget;
        }

        public override void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
        {
            if (LockedTarget != null)
            {
                if (LockedTarget.IsVisible)
                {
                    var deltaAngle = this.HeadingAngleToInSignedDegrees(LockedTarget);

                    if (Math.Abs((float)deltaAngle) < Metadata.SeekingEscapeAngleDegrees && !deltaAngle.IsNearZero())
                    {
                        RotateMovementVector(Metadata.SeekingRotationRateDegrees ?? 0 * (deltaAngle > 0 ? 1 : -1), epoch);
                    }
                }
            }

            base.ApplyIntelligence(epoch, cameraDisplacement);
        }
    }
}
