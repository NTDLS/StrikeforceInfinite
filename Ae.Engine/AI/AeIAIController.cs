using Ae.Engine.Mathematics;

namespace Ae.Engine.AI
{
    /// <summary>
    /// A sprite that is controlled by an AI (either a state machine (IIAStateMachine) or later a more advanced AI).
    /// </summary>
    public interface AeIAIController
    {
        void ApplyIntelligence(float epoch, AeVector cameraDisplacement);
        AeAIStateHandler? CurrentAIState { get; }
    }
}
