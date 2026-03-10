using Ae.Engine.Level;

namespace Ae.Engine.Situation
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This situation is for debugging only.
    /// </summary>
    internal class AeSituationDebuggingStarbase
        : AeSituation
    {
        public AeSituationDebuggingStarbase(AeEngine engine)
            : base(engine,
                  "Debugging Starbase",
                  "The situation is dire and the explosions here typically\r\n"
                  + "cause the entire universe to end - as well as the program."
                  )
        {
            Levels.Add(new AeLevelDebuggingStarbase(engine));
        }
    }
}
