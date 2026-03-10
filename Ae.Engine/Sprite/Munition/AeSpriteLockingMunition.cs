using Ae.Engine.ExtensionMethods;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Interactive;
using System;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.Munition
{
    /// <summary>
    /// Guided munitions need to be locked onto a target before they are fired. They will adjust heading within given parameters to hit the locked target.
    /// </summary>
    [AssetClass("Munition - Locking Type", "", AeBaseAssetType.Image, true)]
    internal class AeSpriteLockingMunition
        : AeSpriteMunition
    {
        public AeSpriteInteractive? LockedTarget { get; private set; }

        public AeSpriteLockingMunition(AeEngine engine, AeSpriteWeapon weapon, AeSpriteInteractive firedFrom, string assetKey,
             AeSpriteInteractive? lockedTarget, AeVector location)
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
