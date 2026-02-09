using Si.Engine.AI._Superclass;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;

namespace Si.Engine.AI.Logistics
{
    /// <summary>
    /// Keeps an object swooping past an object at an indirect angle.
    /// </summary>
    internal class AILogisticsTaunt
        : AIStateMachine
    {
        public AILogisticsTaunt(EngineCore engine, SpriteInteractiveShipBase owner, SpriteBase observedObject)
            : base(engine, owner, observedObject)
        {
        }
    }
}
