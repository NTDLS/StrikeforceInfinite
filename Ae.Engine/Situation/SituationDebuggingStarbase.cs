using Ae.Engine.Level;
using Ae.Engine.Situation._Superclass;

namespace Ae.Engine.Situation
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This situation is for debugging only.
    /// </summary>
    internal class SituationDebuggingStarbase
        : SituationBase
    {
        public SituationDebuggingStarbase(AeEngine engine)
            : base(engine,
                  "Debugging Starbase",
                  "The situation is dire and the explosions here typically\r\n"
                  + "cause the entire universe to end - as well as the program."
                  )
        {
            Levels.Add(new LevelDebuggingStarbase(engine));
        }
    }
}
