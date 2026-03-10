using Ae.Engine.Level;

namespace Ae.Engine.Situation
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This situation is for debugging only.
    /// </summary>
    internal class AeSituationDebuggingGalore
        : AeSituation
    {
        public AeSituationDebuggingGalore(AeEngine engine)
            : base(engine,
                  "Debugging Galore",
                  "The situation is dire and the explosions here typically\r\n"
                  + "cause the entire universe to end - as well as the program."
                  )
        {
            Levels.Add(new AeLevelDebuggingGalore(engine));
            Levels.Add(new AeLevelDebuggingGalore(engine));
            Levels.Add(new AeLevelDebuggingGalore(engine));
            Levels.Add(new AeLevelFreeFlight(engine));
        }
    }
}
