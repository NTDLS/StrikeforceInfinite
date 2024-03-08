﻿using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library.Mathematics.Geometry;

namespace Si.Engine.Sprite.Weapon.Munition._Superclass
{
    /// <summary>
    /// Projectile munitions just go straight - these are physical bullets that have no power of their own once fired.
    /// </summary>
    internal class ProjectileMunitionBase : MunitionBase
    {
        public ProjectileMunitionBase(EngineCore engine, WeaponBase weapon, SpriteBase firedFrom, string imagePath, SiPoint location = null, float? angle = null)
            : base(engine, weapon, firedFrom, imagePath, location, angle)
        {
        }
    }
}