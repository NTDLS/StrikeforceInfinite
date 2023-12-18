﻿using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Situations.BaseClasses;
using NebulaSiege.Game.TickControllers.BaseClasses;
using NebulaSiege.Game.Utility;
using System.Linq;

namespace NebulaSiege.Game.Controller
{
    internal class SituationTickController : UnvectoredTickControllerBase<SituationBase>
    {
        private readonly EngineCore _core;
        public SituationBase CurrentSituation { get; private set; }

        public SituationTickController(EngineCore core)
            : base(core)
        {
            _core = core;
        }

        public void Select(string name)
        {
            var situationTypes = NsReflection.GetSubClassesOf<SituationBase>();
            var situationType = situationTypes.Where(o => o.Name == name).First();
            CurrentSituation = NsReflection.CreateInstanceFromType<SituationBase>(situationType, new object[] { _core, });
        }

        public bool AdvanceLevel()
        {
            return CurrentSituation?.Advance() ?? false;
        }

        public void End()
        {
            CurrentSituation?.End();
        }
    }
}