using Si.Engine.Level._Superclass;
using Si.Engine.Sprite.Enemy._Superclass;
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
                    "The situation is dire. Explosions here can cause\r\n"
                  + " the entire universe to end - as well as the program."
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

                _engine.Audio.RadarBlipsSound?.Play();

                CurrentWave++;
            }
        }

        private void AddEnemies()
        {
            /*
            var asteroid = _engine.Sprites.InteractiveBitmaps.Add($@"Sprites\Asteroid\{SiRandom.Between(0, 0)}.png");

            asteroid.SpriteTag = "DEBUG_ASTEROID";
            asteroid.Location = new SiVector(1, 1);
            asteroid.Speed = 1.0f;
            asteroid.RotationSpeed = 45.ToRadians();
            //asteroid.Orientation = SiVector.FromUnsignedDegrees(-45);
            asteroid.IsHighlighted = true;
            asteroid.SetHullHealth(int.MaxValue);
            */

            //var sprite = _engine.Sprites.Enemies.Add(@"Sprites\Enemy\Peon\Phoenix.png");

            //var sprite = _engine.Sprites.Enemies.Add(@"Sprites\Enemy\Boss\Devastator\Hull.png");


            /*
            for (int i = 0; i < 10; i++)
            {
                var sprite = _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyPhoenix>();
                sprite.Location = _engine.Display.RandomOffScreenLocation();
                //sprite.SetHullHealth(int.MaxValue);
                sprite.Orientation = SiRandom.RandomOrientationVector();

                sprite.ClearAIControllers();
                sprite.AddAIController(new AILogisticsHostileEngagement2(_engine, sprite, _engine.Player.Sprite, new AILogisticsHostileEngagement2.ModelParameters()
                {
                }));
                sprite.SetCurrentAIController<AILogisticsHostileEngagement2>();
            }
            */

            /*
            var garrison = _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyStarbaseGarrison>();
            garrison.Location = new(500, 500);

            //This is cool!
            for (int i = 0; i < 25; i++)
            {
                var sprite = _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyPhoenix>();
                sprite.Location = _engine.Display.RandomOffScreenLocation();
                sprite.SetHullHealth(int.MaxValue);
                sprite.Orientation = SiRandom.RandomOrientationVector();

                sprite.ClearAIControllers();
                sprite.AddAIController(new AILogisticsGuardTarget(_engine, sprite, garrison, new AILogisticsGuardTarget.ModelParameters
                {
                    MaxDistance = 200
                }));
                sprite.SetCurrentAIController<AILogisticsGuardTarget>();
            }
            */

            /* //This is cool!
            for (int i = 0; i < 10; i++)
            {
                var sprite = _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyPhoenix>();
                sprite.Location = _engine.Display.RandomOffScreenLocation();
                sprite.SetHullHealth(int.MaxValue);
                sprite.Orientation = SiRandom.RandomOrientationVector();
            }
            */

            //_engine.Sprites.Debugs.AddAt(new SiVector(1000, 1000));

            //_engine.Sprites.Enemies.AddTypeOf<SpriteEnemyBossDevastator>();
            //_engine.Sprites.Enemies.AddTypeOf<SpriteEnemyDebugPoly>().Location = new(500, 500);


            /*
            _engine.Sprites.TextBlocks.Add(_engine.Rendering.TextFormats.Debug,
                _engine.Rendering.Materials.Brushes.
            
            
            Red,
                new SiVector(100, 100), true, "", "Test");
            */

            /*
            var asteroid1 = _engine.Sprites.InteractiveBitmaps.Add($@"Sprites\Asteroid\{SiRandom.Between(0, 0)}.png");
            asteroid1.SpriteTag = "asteroid1";
            asteroid1.Location = new SiVector(1000, 1000);
            asteroid1.Orientation = SiVector.Zero;
            asteroid1.Speed = 0f;
            asteroid1.RecalculateMovementVectorFromAngle(0.ToRadians());
            asteroid1.VectorType = ParticleVectorType.Default;
            asteroid1.Metadata.Mass = 100f;

            var asteroid2 = _engine.Sprites.InteractiveBitmaps.Add($@"Sprites\Asteroid\{SiRandom.Between(0, 0)}.png");
            asteroid2.SpriteTag = "asteroid2";
            asteroid2.Location = new SiVector(1200, 1000);
            asteroid2.Orientation = SiVector.Zero;
            asteroid2.Speed = 0.80f;
            asteroid2.RecalculateMovementVectorFromAngle(180.ToRadians());
            asteroid2.VectorType = ParticleVectorType.Default;
            asteroid2.Metadata.Mass = 10f;

            var asteroid3 = _engine.Sprites.InteractiveBitmaps.Add($@"Sprites\Asteroid\{SiRandom.Between(0, 0)}.png");
            asteroid3.SpriteTag = "asteroid3";
            asteroid3.Location = new SiVector(1400, 1000);
            asteroid3.Orientation = SiVector.Zero;
            asteroid3.Speed = 0f;
            asteroid3.RecalculateMovementVectorFromAngle(0.ToRadians());
            asteroid3.VectorType = ParticleVectorType.Default;
            asteroid3.Metadata.Mass = 100f;
            */

            //var debugEnemy = _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyDebug>();
            //debugEnemy.Orientation = SiVector.FromDegrees(0);
            //debugEnemy.Location = new SiVector(1000, 1000);

            //var debug = _engine.Sprites.Debugs.Add(1000, 1000);
            //debug.Speed = 0.5f;
            //debug.Location = new SiVector(900, 900);
            //debug.MovementVector = debug.CalculateMovementVector();

            //_engine.Sprites.Enemies.AddTypeOf<SpriteEnemyStarbaseGarrison>().Location = new(500, 500);

            //AddAsteroidField(new SiVector(1000, 1000), 8, 8);

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
            asteroid.Orientation = SiVector.FromUnsignedDegrees(-45);

            asteroid.SetHullHealth(100);
        }

        public void AddAsteroidField(SiVector offset, int rowCount, int colCount)
        {
            for (int row = 0; row < rowCount; row++)
            {
                for (int col = 0; col < colCount; col++)
                {
                    var asteroid = _engine.Sprites.InteractiveBitmaps.Add($"Sprites/Asteroid/{SiRandom.Between(0, 23)}");

                    var asteroidSize = asteroid.Size.Width + asteroid.Size.Height;

                    float totalXOffset = (offset.X + asteroidSize * colCount);
                    float totalYOffset = (offset.Y + (asteroidSize * rowCount));

                    asteroid.Location = new SiVector(totalXOffset - asteroidSize * col, totalYOffset - asteroidSize * row);

                    asteroid.Orientation = SiRandom.RandomOrientationVector();
                    asteroid.Speed = SiRandom.Variance(asteroid.Speed, 0.20f);
                    asteroid.Throttle = 1;
                    asteroid.RotationSpeed = SiRandom.RandomSign(SiRandom.Between(1f, 360f).ToRadians());
                    asteroid.Metadata.Mass = new SiRange<float>(Mass.Large);

                    asteroid.RecalculateMovementVectorFromAngle(SiRandom.Between(1f, 360f).ToRadians());
                    asteroid.VectorType = ParticleVectorType.Default;

                    //asteroid.RotationSpeed = SiRandom.FlipCoin() ? SiRandom.Between(-1.5f, -0.4f) : SiRandom.Between(0.4f, 1.5f);
                    //asteroid.RotationSpeed = 0;

                    asteroid.SetHullHealth(100);
                }
            }
        }
    }
}
