using Ae.Engine.ExtensionMethods;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Interactive;
using System;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.Munition
{
    /// <summary>
    /// Seeking munitions do not lock on to targets, but they will follow a target withing some defined parameters.
    /// </summary>
    [AssetClass("Munition - Seeking Type", "", AeBaseAssetType.Image, true)]
    internal class AeSpriteSeekingMunition
        : AeSpriteMunition
    {
        public AeSpriteSeekingMunition(AeEngine engine, AeSpriteWeapon weapon, AeSpriteInteractive firedFrom, string assetKey,
             AeSpriteInteractive? lockedTarget, AeVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }

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
