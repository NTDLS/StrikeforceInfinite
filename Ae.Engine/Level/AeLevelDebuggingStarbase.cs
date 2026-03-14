using Ae.Engine.Helpers;
using Ae.Engine.Sprite.Interactive.Ship;
using Ae.Engine.Types;
using System.Linq;

namespace Ae.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// This level is for debugging only.
    /// </summary>
    internal class AeLevelDebuggingStarbase
        : AeLevel
    {
        public AeLevelDebuggingStarbase(AeEngine engine)
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

            Engine.Events.Once(500, FirstShowPlayerCallback);
            Engine.Events.Add(5000, AddFreshEnemiesCallback, eventMode: SiDefermentEventMode.Recurring);

            Engine.Player.Sprite.AddHullHealth(100);
            Engine.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback()
        {
            Engine.Player.ResetAndShow();

            Engine.Events.Once(AeRandom.Between(0, 800), AddFreshEnemiesCallback);
        }

        private void AddFreshEnemiesCallback(AeDefermentEvent sender, object? parameter)
        {
            if (Engine.Sprites.OfType<AeSpriteEnemy>().Count() == 0)
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

                Engine.Audio.RadarBlipsSound?.Play();

                CurrentWave++;
            }
        }

        private void AddEnemies()
        {
            //_engine.Sprites.Enemies.AddTypeOf<SpriteEnemyStarbaseGarrison>();
        }
    }
}
