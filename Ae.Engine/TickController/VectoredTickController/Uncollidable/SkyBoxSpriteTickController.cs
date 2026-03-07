using Ae.Engine.Manager;
using Ae.Engine.Sprite._Superclass;
using Ae.Engine.TickController._Superclass;
using Ae.Library.Mathematics;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    public class SkyBoxSpriteTickController
        : VectoredTickControllerBase<SpriteSkyBox>
    {
        public SkyBoxSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            if (cameraDisplacement.Sum() != 0)
            {
                foreach (var skyBox in All())
                {
                    skyBox.ApplyMotion(epoch, cameraDisplacement);
                }
            }
        }
    }
}
