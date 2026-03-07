using Ae.Engine.Level;
using Ae.Engine.Situation._Superclass;

namespace Ae.Engine.Situation
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This is the first ever built challenge situation - it needs to be expanded.
    /// </summary>
    internal class SituationChallenge
        : SituationBase
    {
        public SituationChallenge(AeEngine engine)
            : base(engine,
                  "The First Challenge",
                  "The first challenge level... play at your own risk."
                  )
        {
            Levels.Add(new LevelPhoenixAmbush(engine));
            Levels.Add(new LevelFreeFlight(engine));
        }
    }
}
