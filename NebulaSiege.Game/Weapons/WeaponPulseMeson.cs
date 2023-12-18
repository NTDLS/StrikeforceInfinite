﻿using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Sprites;
using NebulaSiege.Game.Utility;
using NebulaSiege.Game.Weapons.BaseClasses;
using NebulaSiege.Game.Weapons.Munitions;

namespace NebulaSiege.Game.Weapons
{
    internal class WeaponPulseMeson : WeaponBase
    {
        static new string Name { get; } = "Pulse Meson";
        private const string soundPath = @"Sounds\Weapons\PulseMeson.wav";
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

        public override MunitionBase CreateMunition(NsPoint xyOffset, SpriteBase targetOfLock = null)
        {
            return new MunitionPulseMeson(_core, this, _owner, xyOffset);
        }

        public override bool Fire()
        {
            if (CanFire)
            {
                _fireSound.Play();
                RoundQuantity--;

                if (_toggle)
                {
                    var pointRight = HgMath.PointFromAngleAtDistance360(_owner.Velocity.Angle + 90, new NsPoint(10, 10));
                    _core.Sprites.Munitions.Create(this, pointRight);
                }
                else
                {
                    var pointLeft = HgMath.PointFromAngleAtDistance360(_owner.Velocity.Angle - 90, new NsPoint(10, 10));
                    _core.Sprites.Munitions.Create(this, pointLeft);
                }

                _toggle = !_toggle;

                ApplyRecoil();

                return true;
            }
            return false;

        }
    }
}