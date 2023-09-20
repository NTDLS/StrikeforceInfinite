﻿using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Utility;
using NebulaSiege.Weapons.Bullets;

namespace NebulaSiege.Weapons
{
    internal class WeaponPulseMeson : _WeaponBase
    {
        static new string Name { get; } = "Pulse Meson";
        private const string soundPath = @"Sounds\Weapons\WeaponPulseMeson.wav";
        private const float soundVolumne = 0.4f;

        private bool _toggle = false;

        public WeaponPulseMeson(EngineCore core, _SpriteShipBase owner)
            : base(core, owner, Name, soundPath, soundVolumne) => InitializeWeapon();

        public WeaponPulseMeson(EngineCore core)
            : base(core, Name, soundPath, soundVolumne) => InitializeWeapon();

        private void InitializeWeapon()
        {
            Damage = 25;
            FireDelayMilliseconds = 1000;

            Damage = 25;
            FireDelayMilliseconds = 1000;
            Speed = 25;
            AngleVarianceDegrees = 0.00;
            SpeedVariancePercent = 0.00;
            RecoilAmount = 0.65;
        }

        public override _BulletBase CreateBullet(_SpriteBase lockedTarget, NsPoint xyOffset = null)
        {
            return new BulletPulseMeson(_core, this, _owner, lockedTarget, xyOffset);
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

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

                ApplyRecoil();

                return true;
            }
            return false;

        }
    }
}