using Ae.Engine.AI._Superclass;
using Ae.Engine.Sprite._Superclass._Root;
using Ae.Engine.Sprite._Superclass.Interactive.Ship;
using System.Collections.Generic;

namespace Ae.Engine.AI.Logistics
{
    /// <summary>
    /// Keeps an object swooping past an object at an indirect angle.
    /// </summary>
    public class AILogisticsTaunt
        : AIStateMachine
    {
        //DO NOT USE WITHOUT REWRITE!!

        public AILogisticsTaunt(AeEngine engine, SpriteShip owner, List<SpriteBase> observedObjects)
            : base(engine, owner, observedObjects)
        {
        }
    }
}
