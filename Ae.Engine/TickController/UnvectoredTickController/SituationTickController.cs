using Ae.Engine.Situation._Superclass;
using Ae.Engine.TickController._Superclass;
using Ae.Library;
using System.Linq;

namespace Ae.Engine.TickController.UnvectoredTickController
{
    public class SituationTickController
        : UnvectoredTickControllerBase<SituationBase>
    {
        private readonly SiEngine _engine;
        public SituationBase? CurrentSituation { get; private set; }

        public SituationTickController(SiEngine engine)
            : base(engine)
        {
            _engine = engine;
        }

        public void Select(string name)
        {
            var situationTypes = SiReflection.GetSubClassesOf<SituationBase>();
            var situationType = situationTypes.Where(o => o.Name == name).First();
            CurrentSituation = SiReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { _engine, });
        }

        public override void ExecuteWorldClockTick()
        {
            if (CurrentSituation?.CurrentLevel != null)
            {
                if (CurrentSituation.CurrentLevel.State == SiConstants.SiLevelState.Ended)
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
