﻿using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Utility;
using NebulaSiege.Weapons.Bullets;

namespace NebulaSiege.Weapons
{
    internal class WeaponPrecisionGuidedFragMissile : _WeaponBase
    {
        static new string Name { get; } = "Precision Guided Frag Missile";
        private const string soundPath = @"Sounds\Weapons\WeaponGuidedFragMissile.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponPrecisionGuidedFragMissile(EngineCore core, _SpriteShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponPrecisionGuidedFragMissile(EngineCore core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 8;
            FireDelayMilliseconds = 800;
            Speed = 13;
            SpeedVariancePercent = 0.10;

            CanLockOn = true;
            MinLockDistance = 100;
            MaxLockDistance = 1000;
            MaxLocks = 1;
            MaxLockOnAngle = 45;
            ExplodesOnImpact = true;
        }

        public override _BulletBase CreateBullet(_SpriteBase lockedTarget, NsPoint xyOffset = null)
        {
            return new BulletGuidedFragMissile(_core, this, _owner, lockedTarget, xyOffset);
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                if (LockedOnObjects == null || LockedOnObjects.Count == 0)
                {
                    if (_toggle)
                    {
                        var pointRight = HgMath.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new NsPoint(10, 10));
                        _core.Sprites.Bullets.Create(this, pointRight);
                    }
                    else
                    {
                        var pointLeft = HgMath.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new NsPoint(10, 10));
                        _core.Sprites.Bullets.Create(this, pointLeft);
                    }

                    _toggle = !_toggle;
                }
                else
                {
                    foreach (var lockedOn in LockedOnObjects)
                    {
                        if (_toggle)
                        {
                            var pointRight = HgMath.AngleFromPointAtDistance(_owner.Velocity.Angle + 90, new NsPoint(10, 10));
                            _core.Sprites.Bullets.CreateLocked(this, lockedOn, pointRight);
                        }
                        else
                        {
                            var pointLeft = HgMath.AngleFromPointAtDistance(_owner.Velocity.Angle - 90, new NsPoint(10, 10));
                            _core.Sprites.Bullets.CreateLocked(this, lockedOn, pointLeft);
                        }
                        _toggle = !_toggle;
                    }
                }


                return true;
            }
            return false;

        }
    }
}