using Ae.Engine.Helpers;
using Ae.Engine.Situation._Superclass;
using System.Linq;

namespace Ae.Engine.TickController.UnvectoredTickController
{
    public class SituationTickController
        : UnvectoredTickControllerBase<SituationBase>
    {
        private readonly AeEngine _engine;
        public SituationBase? CurrentSituation { get; private set; }

        public SituationTickController(AeEngine engine)
            : base(engine)
        {
            _engine = engine;
        }

        public void Select(string name)
        {
            var situationTypes = AeReflection.GetSubClassesOf<SituationBase>();
            var situationType = situationTypes.Where(o => o.Name == name).First();
            CurrentSituation = AeReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { _engine, });
        }

        public override void ExecuteWorldClockTick()
        {
            if (CurrentSituation?.CurrentLevel != null)
            {
                if (CurrentSituation.CurrentLevel.State == AeConstants.SiLevelState.Ended)
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
