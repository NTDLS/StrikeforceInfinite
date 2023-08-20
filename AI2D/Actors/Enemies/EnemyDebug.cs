﻿using AI2D.AI;
using AI2D.AI.Logistics;
using AI2D.Engine;
using AI2D.Types;
using AI2D.Weapons;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace AI2D.Actors.Enemies
{
    /// <summary>
    /// Debugging enemy uint.
    /// </summary>
    public class EnemyDebug : EnemyBase
    {
        public const int ScoreMultiplier = 15;
        private const string _assetPath = @"..\..\..\Assets\Graphics\Enemy\Debug\";
        private readonly int imageCount = 6;
        private readonly int selectedImageIndex = 0;

        public EnemyDebug(Core core)
            : base(core, EnemyBase.GetGenericHP(), ScoreMultiplier)
        {
            selectedImageIndex = Utility.Random.Next(0, 1000) % imageCount;
            SetImage(Path.Combine(_assetPath, $"{selectedImageIndex}.png"), new Size(32, 32));

            base.SetHitPoints(Utility.Random.Next(Constants.Limits.MinEnemyHealth, Constants.Limits.MaxEnemyHealth));

            Velocity.MaxSpeed = Utility.Random.Next(Constants.Limits.MaxSpeed - 2, Constants.Limits.MaxSpeed); //Upper end of the speed spectrum

            AddSecondaryWeapon(new WeaponVulcanCannon(_core)
            {
                RoundQuantity = 1000,
                FireDelayMilliseconds = 250
            });

            AddSecondaryWeapon(new WeaponDualVulcanCannon(_core)
            {
                RoundQuantity = 500,
                FireDelayMilliseconds = 500
            });

            SelectSecondaryWeapon(typeof(WeaponVulcanCannon));

            //AddAIController(new HostileEngagement(_core, this, _core.Actors.Player));
            AddAIController(new FlyBy(_core, this, _core.Actors.Player));
            //AddAIController(new Meander(_core, this, _core.Actors.Player));

            SetCurrentAIController(AIControllers[typeof(FlyBy)]);

        }

        #region Artificial Intelligence.



        public override void ApplyIntelligence(Point<double> frameAppliedOffset)
        {
            base.ApplyIntelligence(frameAppliedOffset);

            if (CurrentAIController != null)
            {
                CurrentAIController.ApplyIntelligence(frameAppliedOffset);
            }
        }

        #endregion
    }
}
