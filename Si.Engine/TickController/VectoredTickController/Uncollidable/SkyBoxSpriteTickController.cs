using Si.Engine.Manager;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics;

namespace Si.Engine.TickController.VectoredTickController.Uncollidable
{
    public class SkyBoxSpriteTickController : VectoredTickControllerBase<SpriteSkyBox>
    {
        public SkyBoxSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector cameraDisplacement)
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
