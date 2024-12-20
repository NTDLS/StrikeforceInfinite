﻿using Si.Engine.Core.Types;
using Si.Engine.Level._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Enemy.Peon;
using Si.GameEngine.Sprite.Enemy.Starbase.Garrison;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using System;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// This level is for debugging only.
    /// </summary>
    internal class LevelDebuggingGalore : LevelBase
    {
        public LevelDebuggingGalore(EngineCore engine)
            : base(engine,
                  "Debugging Galore",
                  "The level is dire, the explosions here typically\r\n"
                  + "cause the entire universe to end - as well as the program."
                  )
        {
            TotalWaves = 100;
        }

        public override void Begin()
        {
            base.Begin();

            AddSingleFireEvent(500, FirstShowPlayerCallback);
            //AddRecuringFireEvent(5000, AddFreshEnemiesCallback);

            _engine.Player.Sprite.AddHullHealth(100);
            _engine.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(SiDefermentEvent sender, object? refObj)
        {
            _engine.Player.ResetAndShow();
            AddSingleFireEvent(SiRandom.Between(0, 800), AddFreshEnemiesCallback);
        }

        private void AddFreshEnemiesCallback(SiDefermentEvent sender, object? refObj)
        {
            if (_engine.Sprites.OfType<SpriteEnemyBase>().Count() == 0)
            {
                if (CurrentWave == TotalWaves)
                {
                    End();
                    return;
                }

                //int enemyCount = Utility.Random.Next(CurrentWave + 1, CurrentWave + 5);
                int enemyCount = 1;

                for (int i = 0; i < enemyCount; i++)
                {
                    AddEnemies();
                }

                _engine.Audio.RadarBlipsSound.Play();

                CurrentWave++;
            }
        }

        private void AddEnemies()
        {
            for (int i = 0; i < 1; i++)
            {
                _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyPhoenix>();
            }

            var debug = _engine.Sprites.Debugs.Add();
            debug.Orientation = SiVector.FromDegrees(-90);
            debug.Location = new SiVector(1000, 1000);

            //_engine.Sprites.Enemies.AddTypeOf<SpriteEnemyBossDevastator>();

            //var debugEnemy = _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyDebug>();
            //debugEnemy.Orientation = SiVector.FromDegrees(0);
            //debugEnemy.Location = new SiVector(1000, 1000);

            //var debug = _engine.Sprites.Debugs.Add(1000, 1000);
            //debug.Speed = 0.5f;
            //debug.Location = new SiVector(900, 900);
            //debug.MovementVector = debug.CalculateMovementVector();

            _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyStarbaseGarrison>();

            //AddAsteroidField(new SiVector(), 8, 8);

            //AddSingleAsteroid();

            //_engine.Sprites.Enemies.Create<EnemyRepulsor>();
            //_engine.Sprites.Enemies.Create<EnemyRepulsor>();
            //_engine.Sprites.Enemies.Create<EnemyRepulsor>();
            //_engine.Sprites.Enemies.Create<EnemyRepulsor>();

            //_engine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            //_engine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();
            //_engine.Sprites.Enemies.Create<SpriteEnemyPhoenix>();

            //_engine.Sprites.Debugs.CreateAtCenterScreen();
            //_engine.Sprites.Enemies.Create<SpriteEnemyDebug>();
            //_engine.Sprites.Enemies.Create<EnemyDebug>();
            //_engine.Sprites.Enemies.Create<EnemyDebug>();
            //_engine.Sprites.Enemies.Create<EnemyDebug>();
            //_engine.Sprites.Enemies.Create<EnemyDebug>();
            //_engine.Sprites.Enemies.Create<EnemyDebug>();
            //_engine.Sprites.Enemies.Create<EnemyPhoenix>();
            //_engine.Sprites.Enemies.Create<EnemyPhoenix>();
            //_engine.Sprites.Enemies.Create<EnemyPhoenix>();
            //_engine.Sprites.Enemies.Create<EnemyDevastator>();
            //_engine.Sprites.Enemies.Create<EnemyRepulsor>();
            //_engine.Sprites.Enemies.Create<EnemySpectre>();
            //_engine.Sprites.Enemies.Create<EnemyDevastator>();
            //_engine.Sprites.Enemies.Create<EnemyDevastator>();
        }

        public void AddSingleAsteroid()
        {
            var asteroid = _engine.Sprites.InteractiveBitmaps.Add($@"Sprites\Asteroid\{SiRandom.Between(0, 0)}.png");

            asteroid.Location = new SiVector(800, 800);
            asteroid.Speed = 1.0f;
            asteroid.Orientation = SiVector.FromDegrees(-45);
            asteroid.RecalculateMovementVector();

            asteroid.SetHullHealth(100);
        }

        public void AddAsteroidField(SiVector offset, int rowCount, int colCount)
        {
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    var asteroid = _engine.Sprites.InteractiveBitmaps.Add($@"Sprites\Asteroid\{SiRandom.Between(0, 0)}.png");

                    var asteroidSize = asteroid.Size.Width + asteroid.Size.Height;

                    float totalXOffset = (offset.X + asteroidSize * colCount);
                    float totalYOffset = (offset.Y + (asteroidSize * rowCount));

                    asteroid.Location = new SiVector(totalXOffset - asteroidSize * col, totalYOffset - asteroidSize * row);

                    asteroid.Orientation = SiVector.FromDegrees(SiRandom.Between(0, 359));
                    asteroid.Speed = SiRandom.Variance(asteroid.Speed, 0.20f);
                    asteroid.Throttle = 1;

                    asteroid.RecalculateMovementVector(SiRandom.Variance(-45, 0.10f).ToRadians());
                    asteroid.VectorType = ParticleVectorType.Default;

                    //asteroid.RotationSpeed = SiRandom.FlipCoin() ? SiRandom.Between(-1.5f, -0.4f) : SiRandom.Between(0.4f, 1.5f);
                    //asteroid.RotationSpeed = 0;

                    asteroid.SetHullHealth(100);
                }
            }
        }
    }
}
