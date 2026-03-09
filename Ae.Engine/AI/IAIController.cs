using Ae.Library.Mathematics;

namespace Ae.Engine.AI
{
    /// <summary>
    /// A sprite that is controlled by an AI (either a state machine (IIAStateMachine) or later a more advanced AI).
    /// </summary>
    public interface IAIController
    {
        void ApplyIntelligence(float epoch, AeVector cameraDisplacement);
        AIStateHandler? CurrentAIState { get; }
    }
}
