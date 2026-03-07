using Ae.Engine.Sprite._Superclass.Interactive;
using Ae.Library.ExtensionMethods;
using Ae.Library.Mathematics;
using Ae.Library.Metadata;
using System;
using static Ae.Library.AeConstants;

namespace Ae.Engine.Sprite._Superclass.Munition
{
    /// <summary>
    /// Seeking munitions do not lock on to targets, but they will follow a target withing some defined parameters.
    /// </summary>
    [AssetCategory("Munition - Seeking Type", "", true)]
    internal class SpriteSeekingMunition
        : SpriteMunition
    {
        public SpriteSeekingMunition(AeEngine engine, SpriteWeapon weapon, SpriteInteractive firedFrom, string assetKey,
             SpriteInteractive? lockedTarget, AeVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
        }

        public override void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
        {
            if (FiredFromType == SiFiredFromType.Enemy)
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
            else if (FiredFromType == SiFiredFromType.Player)
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
