﻿using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types.Geometry;
using NebulaSiege.Game.Sprites;
using NebulaSiege.Game.Utility;
using NebulaSiege.Game.Weapons.BaseClasses;
using System.IO;

namespace NebulaSiege.Game.Weapons.Munitions
{
    internal class MunitionScattershot : ProjectileMunitionBase
    {
        private const string _assetPath = @"Graphics\Weapon\Scattershot";
        private readonly int imageCount = 4;
        private readonly int selectedImageIndex = 0;

        public MunitionScattershot(EngineCore core, WeaponBase weapon, SpriteBase firedFrom, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, null, xyOffset)
        {
            selectedImageIndex = HgRandom.Generator.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"));

            Initialize();
        }
    }
}