using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System;

namespace Si.Engine.Sprite.Weapon.Munition._Superclass
{
    /// <summary>
    /// Guided munitions need to be locked onto a target before they are fired. They will adjust heading within given parameters to hit the locked target.
    /// </summary>
    internal class LockingMunitionBase
        : MunitionBase
    {
        public SpriteInteractiveBase? LockedTarget { get; private set; }

        public LockingMunitionBase(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, string assetKey,
             SpriteInteractiveBase? lockedTarget, SiVector location)
            : base(engine, weapon, firedFrom, assetKey, location)
        {
            LockedTarget = lockedTarget;
        }

        public override void ApplyIntelligence(float epoch, SiVector displacementVector)
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

            base.ApplyIntelligence(epoch, displacementVector);
        }
    }
}
