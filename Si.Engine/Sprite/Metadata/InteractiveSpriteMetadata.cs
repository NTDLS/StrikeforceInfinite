﻿using System.Collections.Generic;

namespace Si.GameEngine.Sprite.Metadata
{
    /// <summary>
    /// Contains sprite metadata.
    /// </summary>
    public class InteractiveSpriteMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Speed { get; set; } = 1f;
        public float Boost { get; set; } = 0f;
        public int HullHealth { get; set; } = 0;
        public int ShieldHealth { get; set; } = 0;
        public int Bounty { get; set; } = 0;
        public bool TakesMunitionDamage { get; set; } = true;
        public bool CollisionDetection { get; set; } = true;

        /// <summary>
        /// Used for the players "primary weapon slot".
        /// </summary>
        public InteractiveSpriteWeapon PrimaryWeapon { get; set; }
        public List<InteractiveSpriteAttachment> Attachments { get; set; } = new();
        public List<InteractiveSpriteWeapon> Weapons { get; set; } = new();
    }
}
