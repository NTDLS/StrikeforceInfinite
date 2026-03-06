using Si.Engine.AI._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.Sprite._Superclass.Interactive.Ship;
using System.Collections.Generic;

namespace Si.Engine.AI.Logistics
{
    /// <summary>
    /// Keeps an object swooping past an object at an indirect angle.
    /// </summary>
    public class AILogisticsTaunt
        : AIStateMachine
    {
        //DO NOT USE WITHOUT REWRITE!!

        public AILogisticsTaunt(EngineCore engine, SpriteShip owner, List<SpriteBase> observedObjects)
            : base(engine, owner, observedObjects)
        {
        }
    }
}
