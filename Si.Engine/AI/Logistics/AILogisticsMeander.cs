using Si.Engine.AI._Superclass;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using System.Collections.Generic;

namespace Si.Engine.AI.Logistics
{
    /// <summary>
    /// Keeps an object at a generally safe distance from another object.
    /// </summary>
    public class AILogisticsMeander : AIStateMachine
    {
        //DO NOT USE WITHOUT REWRITE!!

        public AILogisticsMeander(EngineCore engine, SpriteShip owner, List<SpriteBase>? observedObjects)
            : base(engine, owner, observedObjects)
        {
        }
    }
}
