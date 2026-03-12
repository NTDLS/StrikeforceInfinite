using Ae.Engine.Level;
using Ae.Engine.Types;
using System.Collections.Generic;

namespace Ae.Engine.Situation
{
    /// <summary>
    /// Situations are collections of levels. Once each level is completed, the next one is loaded.
    /// </summary>
    public class AeSituation
    {
        /// <summary>
        /// Gets the engine instance used to execute automation tasks.
        /// </summary>
        public AeEngine Engine { get; private set; }

        /// <summary>
        /// Stores the collection of deferment events associated with the current instance.
        /// </summary>
        protected List<AeDefermentEvent> Events = new();

        /// <summary>
        /// Gets or sets the current AE level for the operation.
        /// </summary>
        public AeLevel? CurrentLevel { get; protected set; }
        private int _currentLevelIndex = 0;

        /// <summary>
        /// Gets or sets the name associated with the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the textual description associated with the object.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the current state of the situation.
        /// </summary>
        public AeSituationState State { get; protected set; } = AeSituationState.NotYetStarted;

        /// <summary>
        /// Gets the collection of levels associated with the current instance.
        /// </summary>
        public List<AeLevel> Levels { get; protected set; } = new();

        /// <summary>
        /// Initializes a new instance of the AeSituation class with the specified engine, name, and description.
        /// </summary>
        /// <param name="engine">The engine instance associated with this situation. Cannot be null.</param>
        /// <param name="name">The name that uniquely identifies the situation. Cannot be null or empty.</param>
        /// <param name="description">A description providing additional context or details about the situation. Cannot be null.</param>
        public AeSituation(AeEngine engine, string name, string description)
        {
            Engine = engine;
            Name = name;
            Description = description;
            State = AeSituationState.NotYetStarted;
        }

        /// <summary>
        /// Ends the current level and updates the situation state to indicate completion.
        /// </summary>
        /// <remarks>This method finalizes all levels by invoking their end operations and resets the
        /// current level state. It should be called when the situation is ready to be concluded. Calling this method
        /// when no current level is active has no effect.</remarks>
        public void End()
        {
            if (CurrentLevel != null)
            {
                lock (CurrentLevel)
                {
                    foreach (var obj in Levels)
                    {
                        obj.End();
                    }
                }

                State = AeSituationState.Ended;

                CurrentLevel = null;
                _currentLevelIndex = 0;
            }
        }

        /// <summary>
        /// Returns true of the situation is advanced, returns FALSE if we have have no more situations in the queue.
        /// </summary>
        /// <returns></returns>
        public bool AdvanceLevel()
        {
            lock (Levels)
            {
                if (_currentLevelIndex < Levels.Count)
                {
                    Engine.Player.Hide();
                    CurrentLevel = Levels[_currentLevelIndex];
                    CurrentLevel.Begin();
                    _currentLevelIndex++;

                    State = AeSituationState.Started;

                    return true;
                }
                else
                {
                    State = AeSituationState.Ended;

                    CurrentLevel = null;
                    return false;
                }
            }
        }
    }
}
