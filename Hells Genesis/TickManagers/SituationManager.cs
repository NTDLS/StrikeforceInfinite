﻿using HG.Engine;
using HG.Engine.Situations;
using HG.TickManagers.Interfaces;
using System.Collections.Generic;

namespace HG.TickManagers
{
    internal class SituationManager : IUnvectoredTickManager
    {
        public Core _core { get; private set; }
        public BaseSituation CurrentSituation { get; private set; }

        public List<BaseSituation> Situations = new();

        public SituationManager(Core core)
        {
            _core = core;
        }

        public void ExecuteWorldClockTick()
        {
            if (CurrentSituation?.State == BaseSituation.ScenarioState.Ended)
            {
                if (AdvanceSituation() == false)
                {
                    _core.Events.QueueTheDoorIsAjar();
                }
            }
        }

        public void ClearScenarios()
        {
            lock (Situations)
            {
                foreach (var obj in Situations)
                {
                    obj.EndSituation();
                }
            }

            CurrentSituation = null;
            Situations.Clear();
        }

        public void Reset()
        {
            lock (Situations)
            {
                ClearScenarios();

                Situations.Add(new SituationDebuggingGalore(_core));
                Situations.Add(new SituationScinzadSkirmish(_core));
                Situations.Add(new SituationIrlenFormations(_core));
                Situations.Add(new SituationAvvolAmbush(_core));
            }
        }

        /// <summary>
        /// Returns true of the situation is advanced, returns FALSE if we have have no more situations in the queue.
        /// </summary>
        /// <returns></returns>
        public bool AdvanceSituation()
        {
            lock (Situations)
            {
                if (CurrentSituation != null)
                {
                    Situations.Remove(CurrentSituation);
                }

                if (Situations.Count > 0)
                {
                    _core.Player.Hide();
                    CurrentSituation = Situations[0];
                    CurrentSituation.BeginSituation();
                }
                else
                {
                    CurrentSituation = null;
                    return false;
                }
            }
            return true;
        }
    }
}