using Ae.Engine.ExtensionMethods;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Interactive;
using System;

namespace Ae.Engine.Sprite.Munition
{
    /// <summary>
    /// Guided munitions need to be locked onto a target before they are fired. They will adjust heading within given parameters to hit the locked target.
    /// </summary>
    [AssetClass("Munition - Locking Type", "", AeBaseAssetType.Image, true)]
    public class AeSpriteLockingMunition
        : AeSpriteMunition
    {
        /// <summary>
        /// Gets the currently locked interactive sprite target, if any.
        /// </summary>
        public AeSpriteInteractive? LockedTarget { get; private set; }

        /// <summary>
        /// Initializes a new instance of the AeSpriteLockingMunition class with the specified engine, weapon, firing
        /// entity, asset key, locked target, and location.
        /// </summary>
        /// <param name="engine">The engine instance that manages the game state and rendering for this munition.</param>
        /// <param name="weapon">The weapon associated with this munition, which determines its behavior and effects.</param>
        /// <param name="firedFrom">The interactive entity that fired this munition. Used to track ownership and source.</param>
        /// <param name="assetKey">The asset key identifying the visual representation of the munition.</param>
        /// <param name="lockedTarget">The target entity that this munition is locked onto, or null if no target is locked.</param>
        /// <param name="location">The initial location of the munition in the game world.</param>
        public AeSpriteLockingMunition(AeEngine engine, AeSpriteWeapon weapon, AeSpriteInteractive firedFrom, string assetKey,
             AeSpriteInteractive? lockedTarget, AeVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
            LockedTarget = lockedTarget;
        }

        /// <summary>
        /// Applies intelligence logic to update the object's movement and orientation based on the current epoch and
        /// camera displacement.
        /// </summary>
        /// <remarks>Overrides the base implementation to incorporate target tracking and seeking behavior
        /// when a locked target is visible. The method adjusts heading and movement based on target visibility and
        /// relative angle.</remarks>
        /// <param name="epoch">The time interval, in seconds, representing the current simulation step. Used to scale movement and rotation
        /// calculations.</param>
        /// <param name="cameraDisplacement">The vector representing the camera's displacement during the current epoch. Used to inform movement
        /// adjustments relative to camera position.</param>
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
