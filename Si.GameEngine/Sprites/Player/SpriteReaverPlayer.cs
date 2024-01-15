﻿using Si.GameEngine.Core;
using Si.GameEngine.Loudouts;
using Si.GameEngine.Sprites.Player._Superclass;
using Si.GameEngine.Sprites.Weapons;
using System.Drawing;
using static Si.Shared.SiConstants;

namespace Si.GameEngine.Sprites.Player
{
    internal class SpriteReaverPlayer : SpritePlayerBase
    {
        public SpriteReaverPlayer(GameEngineCore gameEngine)
            : base(gameEngine)
        {
            ShipClass = SiPlayerClass.Reaver;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath, new Size(32, 32));

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    Description = "→ Rogue Reaver ←\n"
                        + "A rogue fighter, known for its hit-and-fade tactics,\n"
                        + "striking and disappearing into the cosmos with warp speed.",
                    MaxSpeed = 5.5,
                    MaxBoost = 3.5,
                    HullHealth = 500,
                    ShieldHealth = 1000,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponDualVulcanCannon), 5000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPhotonTorpedo), 100));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponPulseMeson), 8));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}
