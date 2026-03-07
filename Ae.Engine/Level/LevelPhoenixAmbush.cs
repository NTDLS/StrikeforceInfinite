using Ae.Engine.Level._Superclass;
using Ae.Engine.Sprite._Superclass.Interactive.Ship;
using Ae.Library;
using System.Linq;

namespace Ae.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class LevelPhoenixAmbush
        : LevelBase
    {
        public LevelPhoenixAmbush(AeEngine engine)
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

            AddSingleFireEvent(500, FirstShowPlayerCallback);
            AddRecuringFireEvent(5000, AddFreshEnemiesCallback);

            _engine.Player.Sprite.AddHullHealth(100);
            _engine.Player.Sprite.AddShieldHealth(10);
        }

        private void FirstShowPlayerCallback(AeDefermentEvent sender, object? refObj)
        {
            _engine.Player.ResetAndShow();
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

                int enemyCount = AeRandom.Between(CurrentWave + 1, CurrentWave + 5);

                for (int i = 0; i < enemyCount; i++)
                {
                    AddSingleFireEvent(AeRandom.Between(0, 800), AddEnemyCallback);
                }

                _engine.Audio.RadarBlipsSound?.Play();

                CurrentWave++;
            }
        }

        private void AddEnemyCallback(AeDefermentEvent sender, object? refObj)
        {
            //_engine.Sprites.Enemies.AddTypeOf<SpriteEnemyPhoenix>();
        }
    }
}
