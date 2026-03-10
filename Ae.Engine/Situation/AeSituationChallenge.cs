using Ae.Engine.Level;

namespace Ae.Engine.Situation
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This is the first ever built challenge situation - it needs to be expanded.
    /// </summary>
    internal class AeSituationChallenge
        : AeSituation
    {
        public AeSituationChallenge(AeEngine engine)
            : base(engine,
                  "The First Challenge",
                  "The first challenge level... play at your own risk."
                  )
        {
            Levels.Add(new AeLevelPhoenixAmbush(engine));
            Levels.Add(new AeLevelFreeFlight(engine));
        }
    }
}
