using Ae.Engine.ExtensionMethods;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Interactive;
using System;

namespace Ae.Engine.Sprite.Munition
{
    /// <summary>
    /// Seeking munitions do not lock on to targets, but they will follow a target withing some defined parameters.
    /// </summary>
    [AssetClass("Munition - Seeking Type", "", AeBaseAssetType.Image, true)]
    public class AeSpriteSeekingMunition
        : AeSpriteMunition
    {
        /// <summary>
        /// Initializes a new instance of the AeSpriteSeekingMunition class with the specified engine, weapon, firing
        /// entity, asset key, optional locked target, and location.
        /// </summary>
        /// <param name="engine">The engine instance that manages the game state and simulation.</param>
        /// <param name="weapon">The weapon associated with this munition, defining its behavior and properties.</param>
        /// <param name="firedFrom">The interactive entity that fired the munition.</param>
        /// <param name="assetKey">The key identifying the visual asset to use for this munition.</param>
        /// <param name="lockedTarget">The target entity that the munition is locked onto, or null if no target is specified.</param>
        /// <param name="location">The initial location of the munition in the game world.</param>
        public AeSpriteSeekingMunition(AeEngine engine, AeSpriteWeapon weapon, AeSpriteInteractive firedFrom, string assetKey,
             AeSpriteInteractive? lockedTarget, AeVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }

        /// <summary>
        /// Adjusts the movement behavior of the projectile based on its origin and proximity to targets, enabling
        /// seeking or evasive actions during the specified epoch.
        /// </summary>
        /// <remarks>This method modifies the projectile's trajectory to seek or evade targets depending
        /// on whether it was fired by the player or an enemy. It is typically called once per simulation step to update
        /// movement logic.</remarks>
        /// <param name="epoch">The time interval, in seconds, representing the current simulation step for which intelligence adjustments
        /// are applied.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera during the current epoch, used to inform movement calculations.</param>
        public override void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
        {
            if (FiredFromType == AeFiredFromType.Enemy)
            {
                if (DistanceTo(Engine.Player.Sprite) < Metadata.SeekingEscapeDistance)
                {
                    var deltaAngle = this.HeadingAngleToInSignedDegrees(Engine.Player.Sprite);

                    if (Math.Abs((float)deltaAngle) < Metadata.SeekingEscapeAngleDegrees && !deltaAngle.IsNearZero())
                    {
                        RotateMovementVector(Metadata.SeekingRotationRateDegrees ?? 0 * (deltaAngle > 0 ? 1 : -1), epoch);
                    }
                }
            }
            else if (FiredFromType == AeFiredFromType.Player)
            {
                float? smallestAngle = null;

                foreach (var enemy in Engine.Sprites.Enemies.Visible())
                {
                    if (DistanceTo(enemy) < Metadata.SeekingEscapeDistance)
                    {
                        var deltaAngle = this.HeadingAngleToInSignedDegrees(enemy);
                        if (smallestAngle == null || Math.Abs(deltaAngle) < Math.Abs((float)smallestAngle))
                        {
                            smallestAngle = deltaAngle;
                        }
                    }
                }

                if (smallestAngle != null && Math.Abs((float)smallestAngle) < Metadata.SeekingEscapeAngleDegrees && !smallestAngle.IsNearZero())
                {
                    RotateMovementVector(Metadata.SeekingRotationRateDegrees ?? 0 * (smallestAngle > 0 ? 1 : -1), epoch);
                }
            }

            base.ApplyIntelligence(epoch, cameraDisplacement);
        }
    }
}
