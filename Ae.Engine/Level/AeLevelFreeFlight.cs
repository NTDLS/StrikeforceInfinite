using Ae.Engine.Types;

namespace Ae.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// This level is just a peaceful free flight.
    /// </summary>
    internal class AeLevelFreeFlight
        : AeLevel
    {
        public AeLevelFreeFlight(AeEngine engine)
            : base(engine,
                  "Free Flight",
                  "There's nothing in this quadrant or the next that will threaten us.")
        {
            TotalWaves = 5;
        }

        public override void Begin()
        {
            base.Begin();

            Engine.Events.Add(500, FirstShowPlayerCallback);
        }

        private void FirstShowPlayerCallback(AeDefermentEvent sender, object? refObj)
        {
            Engine.Player.ResetAndShow();
        }
    }
}
