using Ae.Engine.Types;
using System;
using System.Collections.Generic;

namespace Ae.Engine.Level
{
    /// <summary>
    /// Levels are contained inside Situations. Each level contains a set of waves that are progressed. 
    /// </summary>
    public class AeLevel
    {
        /// <summary>
        /// Represents the engine instance used by the derived class to perform core operations.
        /// </summary>
        public AeEngine Engine { get; private set; }

        /// <summary>
        /// Stores the collection of deferment events associated with the current instance.
        /// </summary>
        /// <remarks>Intended for use by derived classes to manage event data related to
        /// deferments.</remarks>
        public List<AeDefermentEvent> Events = new();

        /// <summary>
        /// Gets the unique identifier for this instance.
        /// </summary>
        public Guid UID { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the name associated with this instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the textual description associated with the object.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the index of the current wave in the sequence.
        /// </summary>
        public int CurrentWave { get; set; } = 0;

        /// <summary>
        /// Gets or sets the total number of waves in the sequence.
        /// </summary>
        public int TotalWaves { get; set; } = 1;

        /// <summary>
        /// Gets the current execution state of the AE level.
        /// </summary>
        public AeLevelState State { get; protected set; } = AeLevelState.NotYetStarted;

        /// <summary>
        /// Initializes a new instance of the AeLevel class with the specified engine, name, and description.
        /// </summary>
        /// <param name="engine">The engine instance associated with this level. Cannot be null.</param>
        /// <param name="name">The name of the level. Cannot be null or empty.</param>
        /// <param name="description">The description of the level. Provides additional context or information about the level.</param>
        public AeLevel(AeEngine engine, string name, string description)
        {
            Engine = engine;
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Transitions the level to the ended state and schedules all associated events for deletion.
        /// </summary>
        /// <remarks>Call this method to finalize the level and clean up its events. Once invoked, the
        /// level state is set to ended and no further event processing should occur.</remarks>
        public virtual void End()
        {
            Events.ForEach(e => e.QueueForDeletion());
            State = AeLevelState.Ended;
        }

        /// <summary>
        /// Transitions the current instance to the started state.
        /// </summary>
        /// <remarks>Call this method to initiate the operation or workflow associated with this instance.
        /// Once invoked, the state will be set to indicate that processing has begun.</remarks>
        public virtual void Begin()
        {
            State = AeLevelState.Started;
        }
    }
}
