using Ae.Engine.Helpers;
using Ae.Engine.Situation;
using System.Linq;

namespace Ae.Engine.TickController.UnvectoredTickController
{
    /// <summary>
    /// Controls the ticking and state transitions for the current situation within the engine. Provides methods to
    /// select, advance, and end situations, as well as to process world clock ticks.
    /// </summary>
    /// <remarks>This controller manages the lifecycle of an active situation, including level advancement and
    /// ending. It interacts with the engine to instantiate and update situations based on their type. Use this class to
    /// coordinate situation changes and tick processing in scenarios where multiple situation types may be
    /// present.</remarks>
    public class SituationTickController
        : UnvectoredTickControllerBase<AeSituation>
    {
        /// <summary>
        /// Gets the current situation associated with the instance.
        /// </summary>
        public AeSituation? CurrentSituation { get; private set; }

        /// <summary>
        /// Initializes a new instance of the SituationTickController class using the specified engine.
        /// </summary>
        /// <param name="engine">The engine instance used to drive situation tick processing. Cannot be null.</param>
        public SituationTickController(AeEngine engine)
            : base(engine)
        {
        }

        /// <summary>
        /// Selects and activates a situation by its type name.
        /// </summary>
        /// <remarks>This method replaces the current situation with a new instance of the specified type.
        /// If the provided name does not correspond to a valid situation type, an exception will be thrown.</remarks>
        /// <param name="name">The name of the situation type to activate. Must match the name of a subclass of AeSituation.</param>
        public void Select(string name)
        {
            var situationTypes = AeReflection.GetSubClassesOf<AeSituation>();
            var situationType = situationTypes.Where(o => o.Name == name).First();
            CurrentSituation = AeReflection.CreateInstanceFromType<AeSituation>(situationType, new object[] { Engine, });
        }

        /// <summary>
        /// Advances the current level when the world clock tick occurs and the level has ended.
        /// </summary>
        /// <remarks>This method should be called on each world clock tick to ensure that the game
        /// progresses to the next level when appropriate. If the current situation or level is not set, no action is
        /// taken.</remarks>
        public override void ExecuteWorldClockTick()
        {
            if (CurrentSituation?.CurrentLevel != null)
            {
                if (CurrentSituation.CurrentLevel.State == AeLevelState.Ended)
                {
                    AdvanceLevel();
                }
            }
        }

        /// <summary>
        /// Attempts to advance the current situation to the next level.
        /// </summary>
        /// <remarks>If there is no current situation, the method returns false. Use this method to
        /// progress through levels in the current context.</remarks>
        /// <returns>true if the current situation was successfully advanced; otherwise, false.</returns>
        public bool AdvanceLevel()
        {
            return CurrentSituation?.AdvanceLevel() ?? false;
        }

        /// <summary>
        /// Ends the current situation, if one is active.
        /// </summary>
        /// <remarks>If no current situation exists, this method performs no action. Use this method to
        /// signal the completion or termination of the current situation.</remarks>
        public void End()
        {
            CurrentSituation?.End();
        }
    }
}
