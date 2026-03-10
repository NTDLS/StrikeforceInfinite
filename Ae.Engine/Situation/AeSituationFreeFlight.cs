using Ae.Engine.Level;

namespace Ae.Engine.Situation
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// This is a peaceful situation.
    /// </summary>
    internal class AeSituationFreeFlight
        : AeSituation
    {
        public AeSituationFreeFlight(AeEngine engine)
            : base(engine,
                  "Free Flight",
                  "Theres nothing in this quadrant or the next that will threaten us.")
        {
            Levels.Add(new AeLevelFreeFlight(engine));
        }
    }
}
