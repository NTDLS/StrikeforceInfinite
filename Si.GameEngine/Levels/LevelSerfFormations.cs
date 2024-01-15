﻿using Si.GameEngine.Core;
using Si.GameEngine.Core.Types;
using Si.GameEngine.Levels._Superclass;
using Si.GameEngine.Sprites.Enemies._Superclass;
using Si.GameEngine.Sprites.Enemies.Peons;
using Si.GameEngine.Utility;
using Si.Shared.Types.Geometry;
using System.Linq;

namespace Si.GameEngine.Levels
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class LevelSerfFormations : LevelBase
    {
        private bool _waitingOnPopulation = false;

        public LevelSerfFormations(Engine gameCore)
            : base(gameCore,
                  "Serf Formations",
                  "They fly in formation, which look like easy targets...."
                  )
        {
            TotalWaves = 5;
        }

        public override void Begin()
        {
            base.Begin();

            AddSingleFireEvent(new System.TimeSpan(0, 0, 0, 0, 500), FirstShowPlayerCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 1), AdvanceWaveCallback);
            AddRecuringFireEvent(new System.TimeSpan(0, 0, 0, 5), RedirectFormationCallback);

            _gameCore.Player.Sprite.AddHullHealth(100);
            _gameCore.Player.Sprite.AddShieldHealth(10);
        }

        private void RedirectFormationCallback(Engine gameCore, SiEngineCallbackEvent sender, object refObj)
        {
            var formationSerfs = _gameCore.Sprites.Enemies.VisibleOfType<SpriteEnemySerf>()
                .Where(o => o.Mode == SpriteEnemySerf.AIMode.InFormation).ToList();

            if (formationSerfs.Count > 0)
            {
                if (formationSerfs.Exists(o => o.IsWithinCurrentScaledScreenBounds == true) == false)
                {
                    double angleToPlayer = formationSerfs.First().AngleTo360(_gameCore.Player.Sprite);

                    foreach (SpriteEnemySerf enemy in formationSerfs)
                    {
                        enemy.Velocity.Angle.Degrees = angleToPlayer;
                    }
                }
            }
        }

        private void FirstShowPlayerCallback(Engine gameCore, SiEngineCallbackEvent sender, object refObj)
        {
            _gameCore.Player.ResetAndShow();
        }

        private void AdvanceWaveCallback(Engine gameCore, SiEngineCallbackEvent sender, object refObj)
        {
            if (_gameCore.Sprites.OfType<SpriteEnemyBase>().Count == 0 && !_waitingOnPopulation)
            {
                if (CurrentWave == TotalWaves && _waitingOnPopulation != true)
                {
                    End();
                    return;
                }

                _waitingOnPopulation = true;
                _gameCore.Events.Create(new System.TimeSpan(0, 0, 0, 5), AddFreshEnemiesCallback);
                CurrentWave++;
            }
        }

        private void AddFreshEnemiesCallback(Engine gameCore, SiEngineCallbackEvent sender, object refObj)
        {
            SiPoint baseLocation = _gameCore.Display.RandomOffScreenLocation();
            CreateTriangleFormation(baseLocation, 100 - (CurrentWave + 1) * 10, CurrentWave * 5);
            _gameCore.Audio.RadarBlipsSound.Play();
            _waitingOnPopulation = false;
        }

        private SpriteEnemySerf AddOneEnemyAt(double x, double y, double angle)
        {
            var enemy = _gameCore.Sprites.Enemies.Create<SpriteEnemySerf>();
            enemy.X = x;
            enemy.Y = y;
            enemy.Velocity.ThrottlePercentage = 0.8;
            enemy.Velocity.MaxSpeed = 6;
            enemy.Velocity.Angle.Degrees = angle;
            return enemy;
        }

        private void CreateTriangleFormation(SiPoint baseLocation, double spacing, int depth)
        {
            double angle = SiMath.AngleTo360(baseLocation, _gameCore.Player.Sprite);

            for (int col = 0; col < depth; col++)
            {
                for (int row = 0; row < depth - col; row++)
                {
                    AddOneEnemyAt(baseLocation.X + col * spacing,
                        baseLocation.Y + row * spacing + col * spacing / 2,
                        angle);
                }
            }
        }
    }
}
