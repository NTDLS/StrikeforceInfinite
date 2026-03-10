using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    public class SkyBoxSpriteTickController
        : VectoredTickControllerBase<AeSpriteSkyBox>
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
