﻿using NebulaSiege.Game.Engine;
using NebulaSiege.Game.Engine.Types;
using System;
using System.Collections.Generic;
using static NebulaSiege.Game.Engine.Types.NsEngineCallbackEvent;

namespace NebulaSiege.Game.Levels.BaseClasses
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    internal class LevelBase
    {
        protected EngineCore _core;
        protected List<NsEngineCallbackEvent> Events = new();

        public Guid UID { get; private set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Description { get; set; }
        public int CurrentWave { get; set; } = 0;
        public int TotalWaves { get; set; } = 1;
        public HgLevelState State { get; protected set; } = HgLevelState.NotYetStarted;

        public LevelBase(EngineCore core, string name, string description)
        {
            _core = core;
            Name = name;
            Description = description;
        }

        public virtual void End()
        {
            foreach (var obj in Events)
            {
                obj.QueuedForDeletion = true;
            }

            State = HgLevelState.Ended;
        }

        public virtual void Begin()
        {
            State = HgLevelState.Started;
        }

        protected NsEngineCallbackEvent AddRecuringFireEvent(TimeSpan timeout, HgOnExecute executeCallback)
        {
            //Keep track of recurring events to we can delete them when we are done.
            var obj = _core.Events.Create(timeout, executeCallback, null, HgCallbackEventMode.Recurring);
            Events.Add(obj);
            return obj;
        }

        protected NsEngineCallbackEvent AddSingleFireEvent(TimeSpan timeout, HgOnExecute executeCallback)
        {
            return _core.Events.Create(timeout, executeCallback);
        }
    }
}