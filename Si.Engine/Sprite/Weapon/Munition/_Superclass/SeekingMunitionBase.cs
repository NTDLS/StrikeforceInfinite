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
        /// <summary>
        /// The max distance at which the munition will attempt to seek an object.
        /// </summary>
        public int SeekingEscapeDistance { get; set; } = 1000;

        /// <summary>
        /// The angle in degrees at which the munition will attempt to seek an object.
        /// </summary>
        public int SeekingEscapeAngleDegrees { get; set; } = 20;

        /// <summary>
        /// The rate in degrees per second at which the munition will rotate to seek an object.
        /// </summary>
        public float SeekingRotationRateDegrees { get; set; } = 4;

        public SeekingMunitionBase(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, string? imagePath, SiVector? location = null)
            : base(engine, weapon, firedFrom, imagePath, location)
        {
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (FiredFromType == SiFiredFromType.Enemy)
            {
                if (DistanceTo(_engine.Player.Sprite) < SeekingEscapeDistance)
                {
                    var deltaAngle = this.HeadingAngleToInSignedDegrees(_engine.Player.Sprite);

                    if (Math.Abs((float)deltaAngle) < SeekingEscapeAngleDegrees && !deltaAngle.IsNearZero())
                    {
                        RotateMovementVector(SeekingRotationRateDegrees * (deltaAngle > 0 ? 1 : -1), epoch);
                    }
                }
            }
            else if (FiredFromType == SiFiredFromType.Player)
            {
                float? smallestAngle = null;

                foreach (var enemy in _engine.Sprites.Enemies.Visible())
                {
                    if (DistanceTo(enemy) < SeekingEscapeDistance)
                    {
                        var deltaAngle = this.HeadingAngleToInSignedDegrees(enemy);
                        if (smallestAngle == null || Math.Abs(deltaAngle) < Math.Abs((float)smallestAngle))
                        {
                            smallestAngle = deltaAngle;
                        }
                    }
                }

                if (smallestAngle != null && Math.Abs((float)smallestAngle) < SeekingEscapeAngleDegrees && !smallestAngle.IsNearZero())
                {
                    RotateMovementVector(SeekingRotationRateDegrees * (smallestAngle > 0 ? 1 : -1), epoch);
                }
            }

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
