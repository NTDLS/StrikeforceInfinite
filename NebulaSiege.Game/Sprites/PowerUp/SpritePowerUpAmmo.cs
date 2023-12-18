﻿using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Sprites.PowerUp.BaseClasses;
using NebulaSiege.Game.Utility;
using System.Drawing;
using System.IO;

namespace NebulaSiege.Game.Sprites.PowerUp
{
    internal class SpritePowerUpAmmo : SpritePowerUpBase
    {
        private const string _assetPath = @"Graphics\PowerUp\Ammo\";
        private readonly int imageCount = 3;
        private readonly int selectedImageIndex = 0;

        private readonly int _powerUpAmount = 100;

        public SpritePowerUpAmmo(EngineCore core)
            : base(core)
        {
            selectedImageIndex = HgRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));
            _powerUpAmount *= selectedImageIndex + 1;
        }

        public override void ApplyIntelligence(NsPoint displacementVector)
        {
            if (Intersects(_core.Player.Sprite))
            {
                _core.Player.Sprite.PrimaryWeapon.RoundQuantity += _powerUpAmount;
                if (_core.Player.Sprite.SelectedSecondaryWeapon != null)
                {
                    _core.Player.Sprite.SelectedSecondaryWeapon.RoundQuantity += _powerUpAmount;
                }
                Explode();
            }
            else if (AgeInMiliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}