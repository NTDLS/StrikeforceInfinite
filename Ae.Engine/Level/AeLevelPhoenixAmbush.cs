using Ae.Engine.Helpers;
using Ae.Engine.Sprite.Interactive.Ship;
using Ae.Engine.Types;
using System.Linq;
using static Ae.Engine.Types.AeDefermentEvent;

namespace Ae.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class AeLevelPhoenixAmbush
        : AeLevel
    {
        public AeLevelPhoenixAmbush(AeEngine engine)
            : base(engine,
                  "Phoenix Ambush",
                  "We're safe now - or are we? Its an AMBUSH!"
                  )
        {
            TotalWaves = 5;
        }

        public override void Begin()
        {
            base.Begin();

            Engine.Events.Add(500, FirstShowPlayerCallback);
            Engine.Events.Add(5000, AddFreshEnemiesCallback, eventMode: SiDefermentEventMode.Recurring);

            Engine.Player.Sprite.AddHullHealth(100);
            Engine.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(AeDefermentEvent sender, object? refObj)
        {
            Engine.Player.ResetAndShow();
        }

        private void AddFreshEnemiesCallback(AeDefermentEvent sender, object? refObj)
        {
            if (Engine.Sprites.OfType<AeSpriteEnemy>().Count() == 0)
            {
                if (CurrentWave == TotalWaves)
                {
                    End();
                    return;
                }

                int enemyCount = AeRandom.Between(CurrentWave + 1, CurrentWave + 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    Engine.Events.Add(AeRandom.Between(0, 800), AddEnemyCallback);
                }

                Engine.Audio.RadarBlipsSound?.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(AeDefermentEvent sender, object? refObj)
        {
            //_engine.Sprites.Enemies.AddTypeOf<SpriteEnemyPhoenix>();
        }
    }
}
