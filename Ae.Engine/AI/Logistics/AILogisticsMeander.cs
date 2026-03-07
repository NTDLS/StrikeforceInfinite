using Ae.Engine.AI._Superclass;
using Ae.Engine.Sprite._Superclass._Root;
using Ae.Engine.Sprite._Superclass.Interactive.Ship;
using System.Collections.Generic;

namespace Ae.Engine.AI.Logistics
{
    /// <summary>
    /// Keeps an object at a generally safe distance from another object.
    /// </summary>
    public class AILogisticsMeander : AIStateMachine
    {
        //DO NOT USE WITHOUT REWRITE!!

        public AILogisticsMeander(SiEngine engine, SpriteShip owner, List<SpriteBase>? observedObjects)
            : base(engine, owner, observedObjects)
        {
        }
    }
}
