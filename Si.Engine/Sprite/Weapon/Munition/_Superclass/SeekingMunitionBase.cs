using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Weapon.Munition._Superclass
{
    /// <summary>
    /// Seeking munitions do not lock on to targets, but they will follow a target withing some defined parameters.
    /// </summary>
    internal class SeekingMunitionBase : MunitionBase
    {
        public SeekingMunitionBase(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, string spritePath,
             SpriteInteractiveBase? lockedTarget, SiVector location)
            : base(engine, weapon, firedFrom, spritePath, location)
        {
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (FiredFromType == SiFiredFromType.Enemy)
            {
                if (DistanceTo(_engine.Player.Sprite) < Metadata.SeekingEscapeDistance)
                {
                    var deltaAngle = this.HeadingAngleToInSignedDegrees(_engine.Player.Sprite);

                    if (Math.Abs((float)deltaAngle) < Metadata.SeekingEscapeAngleDegrees && !deltaAngle.IsNearZero())
                    {
                        RotateMovementVector(Metadata.SeekingRotationRateDegrees ?? 0 * (deltaAngle > 0 ? 1 : -1), epoch);
                    }
                }
            }
            else if (FiredFromType == SiFiredFromType.Player)
            {
                float? smallestAngle = null;

                foreach (var enemy in _engine.Sprites.Enemies.Visible())
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

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
