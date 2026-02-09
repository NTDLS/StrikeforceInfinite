using Si.Engine.AI._Superclass;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using System.Collections.Generic;

namespace Si.Engine.AI.Logistics
{
    /// <summary>
    /// Keeps an object swooping past an object at an indirect angle.
    /// </summary>
    internal class AILogisticsTaunt
        : AIStateMachine
    {
        public AILogisticsTaunt(EngineCore engine, SpriteInteractiveShipBase owner, List<SpriteBase> observedObjects)
            : base(engine, owner, observedObjects)
        {
        }
    }
}
