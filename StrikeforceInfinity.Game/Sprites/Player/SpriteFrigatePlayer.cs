﻿using StrikeforceInfinity.Game.Engine;
using StrikeforceInfinity.Game.Loudouts;
using StrikeforceInfinity.Game.Sprites.Player.BaseClasses;
using StrikeforceInfinity.Game.Weapons;
using System.Drawing;

namespace StrikeforceInfinity.Game.Sprites.Player
{
    internal class SpriteFrigatePlayer : SpritePlayerBase
    {
        public SpriteFrigatePlayer(EngineCore gameCore)
            : base(gameCore)
        {
            ShipClass = HgPlayerClass.Frigate;

            string imagePath = @$"Graphics\Player\Ships\{ShipClass}.png";
            Initialize(imagePath, new Size(32, 32));

            //Load the loadout from file or create a new one if it does not exist.
            PlayerShipLoadout loadout = LoadLoadoutFromFile(ShipClass);
            if (loadout == null)
            {
                loadout = new PlayerShipLoadout(ShipClass)
                {
                    Description = "→ Nimble Interceptor ←\n"
                        + "A nimble interceptor, designed for hit-and-run tactics\n"
                        + "and lightning-fast strikes against enemy forces.",
                    MaxSpeed = 4.5,
                    MaxBoost = 1.5,
                    HullHealth = 500,
                    ShieldHealth = 100,
                    PrimaryWeapon = new ShipLoadoutWeapon(typeof(WeaponScattershot), 10000)
                };

                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponFragMissile), 42));
                loadout.SecondaryWeapons.Add(new ShipLoadoutWeapon(typeof(WeaponThunderstrikeMissile), 16));

                SaveLoadoutToFile(loadout);
            }

            ResetLoadout(loadout);
        }
    }
}