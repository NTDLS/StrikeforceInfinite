﻿using NebulaSiege.Engine;
using NebulaSiege.Loudouts;
using NebulaSiege.Weapons;
using System.Drawing;

namespace NebulaSiege.Sprites.Player
{
    internal class SpriteStarfighterPlayer : _SpritePlayerBase
    {
        public SpriteStarfighterPlayer(EngineCore core)
            : base(core)
        {
            ShipClass = HgPlayerClass.Starfighter;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath, new Size(32, 32));

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    Description = "→ Celestial Aviator ←\n"
                        + "A sleek and versatile spacecraft, built for supremacy among the stars.\n"
                        + "It's the first choice for finesse, agility, and unmatched combat prowess\n"
                        + "without all the fuss of a powerful loadout.",
                    MaxSpeed = 5.0,
                    MaxBoost = 3.5,
                    HullHealth = 100,
                    ShieldHealth = 15000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponVulcanCannon), 5000)
                };
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 2500));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}