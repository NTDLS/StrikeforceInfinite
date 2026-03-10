using Ae.Engine.Helpers;
using Ae.Engine.Situation;
using System.Linq;

namespace Ae.Engine.TickController.UnvectoredTickController
{
    public class SituationTickController
        : UnvectoredTickControllerBase<AeSituation>
    {
        private readonly AeEngine _engine;
        public AeSituation? CurrentSituation { get; private set; }

        public SituationTickController(AeEngine engine)
            : base(engine)
        {
            _engine = engine;
        }

        public void Select(string name)
        {
            var situationTypes = AeReflection.GetSubClassesOf<AeSituation>();
            var situationType = situationTypes.Where(o => o.Name == name).First();
            CurrentSituation = AeReflection.CreateInstanceFromType<AeSituation>(situationType, new object[] { _engine, });
        }

        public override void ExecuteWorldClockTick()
        {
            if (CurrentSituation?.CurrentLevel != null)
            {
                if (CurrentSituation.CurrentLevel.State == AeConstants.AeLevelState.Ended)
                {
                    AdvanceLevel();
                }
            }
        }

        public bool AdvanceLevel()
        {
            return CurrentSituation?.AdvanceLevel() ?? false;
        }

        public void End()
        {
            CurrentSituation?.End();
        }
    }
}
