﻿using System.Collections.Generic;

namespace AI2D.Engine.Scenarios
{
    public class BaseScenario
    {
        public string Name { get; set; }
        public enum ScenarioState
        {
            NotStarted,
            Running,
            Ended
        }

        protected List<EngineCallbackEvent> Events = new List<EngineCallbackEvent>();


        protected Core _core;
        public int CurrentWave { get; set; } = 0;
        public int TotalWaves { get; set; } = 1;
        public ScenarioState State { get; protected set; } = ScenarioState.NotStarted;

        public BaseScenario(Core core, string name)
        {
            _core = core;
            Name = name;
        }

        public virtual void Cleanup()
        {
            foreach (var obj in Events)
            {
                obj.ReadyForDeletion = true;
            }

            State = ScenarioState.Ended;
        }

        public virtual void Execute()
        {
        }
    }
}
