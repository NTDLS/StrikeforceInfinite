using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Interactive;
using System;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.AI
{
    /// <summary>
    /// A sprite that is controlled by an AI state-machine.
    /// </summary>
    [AssetClass("AI State Machine", "", AeBaseAssetType.Code, true)]
    public class AeAIStateMachine
        : AeIAIController
    {
        public AeAIParameterCollection Parameters { get; private set; } = new();

        /// <summary>
        /// Reference to the engine core class.
        /// </summary>
        public AeEngine Engine { get; private set; }

        /// <summary>
        /// Reference to the sprite that is being controlled by this AI model.
        /// </summary>
        public AeSpriteInteractive Owner { get; private set; }

        /// <summary>
        /// The current state that the AI is in.
        /// </summary>
        public AeAIStateHandler? CurrentAIState { get; private set; }

        public DateTime StateChangeDateTime { get; set; }

        public double TimeInStateSeconds => (DateTime.UtcNow - StateChangeDateTime).TotalSeconds;

        #region Events.

        /// <summary>
        /// Fired when the state is changed through a call to ChangeState().
        /// </summary>
        public event AIStateChanged? OnAIStateChanged;
        public delegate void AIStateChanged(AeAIStateMachine sender);

        /// <summary>
        /// Fired when the engine wants the sprite to make a decision based on the current AI state.
        /// </summary>
        public event ApplyIntelligenceProc? OnApplyIntelligence;
        public delegate void ApplyIntelligenceProc(float epoch, AeVector cameraDisplacement, AeAIStateHandler? state);

        #endregion

        public virtual void OnMaterialized()
        {
        }

        /// <summary>
        /// A sprite that is controlled by an AI state-machine.
        /// </summary>
        /// <param name="engine">Reference to the engine core class.</param>
        /// <param name="owner">Reference to the sprite that is being controlled by this AI model.</param>
        /// <param name="observedObject">Reference to the object that the sprite is observing (probably the player, but can be other objects).</param>
        public AeAIStateMachine(AeEngine engine, AeSpriteInteractive owner)
        {
            Engine = engine;
            Owner = owner;
            OnMaterialized();
        }

        public void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
        {
            OnApplyIntelligence?.Invoke(epoch, cameraDisplacement, CurrentAIState);
            CurrentAIState?.Tick(epoch);
        }

        /// <summary>
        /// Sets a new AI state.
        /// </summary>
        public void SetAIState(AeAIStateHandler state)
        {
            StateChangeDateTime = DateTime.UtcNow;
            CurrentAIState = state;
            OnAIStateChanged?.Invoke(this);
        }
    }
}
