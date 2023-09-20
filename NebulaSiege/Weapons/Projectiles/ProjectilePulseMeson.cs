﻿using NebulaSiege.Engine;
using NebulaSiege.Engine.Types.Geometry;
using NebulaSiege.Sprites;
using NebulaSiege.Weapons;

namespace HellsGenesis.Weapons.Projectiles
{
    internal class ProjectilePulseMeson : _EnergyProjectileBase
    {
        private const string imagePath = @"Graphics\Weapon\PulseMeson.png";

        public ProjectilePulseMeson(EngineCore core, _WeaponBase weapon, _SpriteBase firedFrom, NsPoint xyOffset = null)
            : base(core, weapon, firedFrom, imagePath, xyOffset)
        {
            Initialize(imagePath);
        }
    }
}
