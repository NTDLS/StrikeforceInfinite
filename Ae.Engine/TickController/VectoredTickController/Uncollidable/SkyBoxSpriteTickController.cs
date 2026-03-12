using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    /// <summary>
    /// Controls the tick-based motion updates for sky box sprites in response to world clock events and camera
    /// displacement.
    /// </summary>
    /// <remarks>This controller applies motion to all managed sky box sprites when the camera displacement is
    /// non-zero. Use this type to synchronize sky box sprite movement with the world clock and camera position changes.
    /// Thread safety depends on the underlying base class and sprite manager implementation.</remarks>
    public class SkyBoxSpriteTickController
        : VectoredTickControllerBase<AeSpriteSkyBox>
    {
        /// <summary>
        /// Initializes a new instance of the SkyBoxSpriteTickController class using the specified engine and sprite
        /// manager.
        /// </summary>
        /// <param name="engine">The engine instance that provides core functionality and services for the controller.</param>
        /// <param name="manager">The sprite manager responsible for managing sprite objects used by the controller.</param>
        public SkyBoxSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        /// <summary>
        /// Updates all sky boxes based on the specified camera displacement and epoch time.
        /// </summary>
        /// <remarks>No motion is applied if the camera displacement vector is zero. This method is
        /// typically called once per world clock tick to synchronize sky box motion with camera movement.</remarks>
        /// <param name="epoch">The current epoch time, used to determine the motion applied to each sky box.</param>
        /// <param name="cameraDisplacement">The vector representing the camera's displacement. If the sum of its components is nonzero, motion is
        /// applied to all sky boxes.</param>
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
