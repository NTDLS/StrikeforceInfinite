using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library.Mathematics;
using System;

namespace Si.Engine.AI._Superclass
{
    /// <summary>
    /// A sprite that is controlled by an AI state-machine.
    /// </summary>
    public class AIStateMachine
        : IAIController
    {
        /// <summary>
        /// Reference to the engine core class.
        /// </summary>
        public EngineCore Engine { get; private set; }

        /// <summary>
        /// Reference to the sprite that is being controlled by this AI model.
        /// </summary>
        public SpriteInteractiveShipBase Owner { get; private set; }

        /// <summary>
        /// Reference to the object that the sprite is observing.
        /// </summary>
        public SpriteBase? ObservedObject { get; private set; }

        /// <summary>
        /// The current state that the AI is in.
        /// </summary>
        public AIStateHandler? CurrentAIState { get; private set; }

        public DateTime StateChangeDateTime { get; set; }

        public double TimeInStateSeconds => (DateTime.UtcNow - StateChangeDateTime).TotalSeconds;

        #region Events.

        /// <summary>
        /// Fired when the state is changed through a call to ChangeState().
        /// </summary>
        public event AIStateChanged? OnAIStateChanged;
        public delegate void AIStateChanged(AIStateMachine sender);

        /// <summary>
        /// Fired when the engine wants the sprite to make a decision based on the current AI state.
        /// </summary>
        public event ApplyIntelligenceProc? OnApplyIntelligence;
        public delegate void ApplyIntelligenceProc(float epoch, SiVector displacementVector, AIStateHandler? state);

        #endregion

        /// <summary>
        /// A sprite that is controlled by an AI state-machine.
        /// </summary>
        /// <param name="engine">Reference to the engine core class.</param>
        /// <param name="owner">Reference to the sprite that is being controlled by this AI model.</param>
        /// <param name="observedObject">Reference to the object that the sprite is observing (probably the player, but can be other objects).</param>
        public AIStateMachine(EngineCore engine, SpriteInteractiveShipBase owner, SpriteBase? observedObject)
        {
            Engine = engine;
            Owner = owner;
            ObservedObject = observedObject;
        }

        public void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            OnApplyIntelligence?.Invoke(epoch, displacementVector, CurrentAIState);
            CurrentAIState?.Execute(epoch);
        }

        /// <summary>
        /// Sets a new AI state.
        /// </summary>
        public void SetAIState(AIStateHandler state)
        {
            StateChangeDateTime = DateTime.UtcNow;
            CurrentAIState = state;
            OnAIStateChanged?.Invoke(this);
        }
    }
}
