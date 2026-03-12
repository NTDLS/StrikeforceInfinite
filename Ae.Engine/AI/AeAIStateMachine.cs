using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Interactive;
using System;

namespace Ae.Engine.AI
{
    /// <summary>
    /// Represents an artificial intelligence (AI) state machine for controlling interactive sprites within the engine.
    /// Provides mechanisms for managing AI states, parameters, and decision-making events.
    /// </summary>
    /// <remarks>The AI state machine coordinates state transitions and intelligence application for a sprite,
    /// enabling complex behavior patterns. It exposes events for state changes and intelligence application, allowing
    /// integration with engine logic and custom AI handlers.</remarks>
    [AssetClass("AI State Machine", "", AeBaseAssetType.Code, true)]
    public class AeAIStateMachine
    {
        /// <summary>
        /// Represents a collection of named parameters for storing and retrieving values by key.
        /// </summary>
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
        public IAeAIStateHandler? CurrentAIState { get; private set; }

        /// <summary>
        /// Gets the UTC date and time of the last state change.
        /// </summary>
        public DateTime LastStateChangeUTC { get; internal set; }

        /// <summary>
        /// Get the time in seconds since the last state change.
        /// </summary>
        public double TimeInStateSeconds => (DateTime.UtcNow - LastStateChangeUTC).TotalSeconds;

        #region Events.

        /// <summary>
        /// Fired when the state is changed through a call to ChangeState().
        /// </summary>
        public event AIStateChanged? OnAIStateChanged;
        /// <summary>
        /// Fired when the state is changed through a call to ChangeState().
        /// </summary>
        public delegate void AIStateChanged(AeAIStateMachine stateMachine);

        /// <summary>
        /// Fired when the engine wants the sprite to make a decision based on the current AI state.
        /// </summary>
        public event ApplyIntelligenceProc? OnApplyIntelligence;
        /// <summary>
        /// Fired when the engine wants the sprite to make a decision based on the current AI state.
        /// </summary>
        public delegate void ApplyIntelligenceProc(float epoch, AeVector cameraDisplacement, IAeAIStateHandler? state);

        #endregion

        /// <summary>
        /// Called when the object has been fully materialized from its data source and is ready for user initialization.
        /// </summary>
        /// <remarks>Override this method to perform custom actions after the object is materialized. This
        /// method is intended for scenarios where additional initialization or processing is required once the object's
        /// data has been loaded.</remarks>
        public virtual void OnMaterialized()
        {
        }

        /// <summary>
        /// A sprite that is controlled by an AI state-machine.
        /// </summary>
        public AeAIStateMachine(AeEngine engine, AeSpriteInteractive owner)
        {
            Engine = engine;
            Owner = owner;
            OnMaterialized();
        }

        /// <summary>
        /// Called by the engine when it is time for the sprite to make a decision based on the current AI state.
        /// Note that ApplyIntelligence is called on the controller and not the state handler, so that the intelligence
        /// application can be handled by the controller and not the state handler if desired.
        /// Then the state handler's Tick() method is called to allow the state handler to perform its logic.
        /// This allows for a separation of concerns between the controller and the state handler, where the
        /// controller can handle the application of intelligence and the state handler can handle the logic of the current state.
        /// </summary>
        /// <param name="epoch"></param>
        /// <param name="cameraDisplacement"></param>
        internal void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
        {
            OnApplyIntelligence?.Invoke(epoch, cameraDisplacement, CurrentAIState);
            CurrentAIState?.Tick(epoch);
        }

        /// <summary>
        /// Sets a new AI state.
        /// </summary>
        public void SetAIState(IAeAIStateHandler state)
        {
            LastStateChangeUTC = DateTime.UtcNow;
            CurrentAIState = state;
            OnAIStateChanged?.Invoke(this);
        }
    }
}
