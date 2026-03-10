using Ae.Engine.Types;
using System;
using System.Collections.Generic;
using static Ae.Engine.AeConstants;
using static Ae.Engine.Types.AeDefermentEvent;

namespace Ae.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    public class AeLevel
    {
        protected AeEngine _engine;
        protected List<AeDefermentEvent> Events = new();

        public Guid UID { get; private set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrentWave { get; set; } = 0;
        public int TotalWaves { get; set; } = 1;
        public AeLevelState State { get; protected set; } = AeLevelState.NotYetStarted;

        public AeLevel(AeEngine engine, string name, string description)
        {
            _engine = engine;
            Name = name;
            Description = description;
        }

        public virtual void End()
        {
            Events.ForEach(e => e.QueueForDeletion());
            State = AeLevelState.Ended;
        }

        public virtual void Begin()
        {
            State = AeLevelState.Started;
        }

        protected AeDefermentEvent AddRecuringFireEvent(int milliseconds, SiDefermentExecuteCallback executeCallback)
        {
            //Keep track of recurring events to we can delete them when we are done.
            var obj = _engine.Events.Add(milliseconds, executeCallback, null, SiDefermentEventMode.Recurring);
            Events.Add(obj);
            return obj;
        }

        protected AeDefermentEvent AddSingleFireEvent(int milliseconds, SiDefermentExecuteCallback executeCallback)
        {
            return _engine.Events.Add(milliseconds, executeCallback);
        }
    }
}
