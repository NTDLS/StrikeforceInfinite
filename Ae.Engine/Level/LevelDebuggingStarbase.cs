using Ae.Engine.Level._Superclass;
using Ae.Engine.Sprite._Superclass.Interactive.Ship;
using Ae.Library;
using System.Linq;

namespace Ae.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// This level is for debugging only.
    /// </summary>
    internal class LevelDebuggingStarbase
        : LevelBase
    {
        public LevelDebuggingStarbase(AeEngine engine)
            : base(engine,
                  "Debugging Starbase",
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
            AddRecuringFireEvent(5000, AddFreshEnemiesCallback);

            _engine.Player.Sprite.AddHullHealth(100);
            _engine.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(AeDefermentEvent sender, object? refObj)
        {
            _engine.Player.ResetAndShow();
            AddSingleFireEvent(AeRandom.Between(0, 800), AddFreshEnemiesCallback);
        }

        private void AddFreshEnemiesCallback(AeDefermentEvent sender, object? refObj)
        {
            if (_engine.Sprites.OfType<SpriteEnemy>().Count() == 0)
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
            //_engine.Sprites.Enemies.AddTypeOf<SpriteEnemyStarbaseGarrison>();
        }
    }
}
