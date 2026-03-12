using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.Interactive;
using System.Linq;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    /// <summary>
    /// Controls the ticking and motion logic for sprite attachments within the engine, managing their updates and
    /// interactions during world clock ticks.
    /// </summary>
    /// <remarks>This controller is responsible for updating all visible sprite attachments that are not dead
    /// or exploded, applying their motion and intelligence each tick. It also records multiplayer actions for these
    /// attachments when applicable. Use this class to add and manage sprite attachments that are visually layered above
    /// their owners and require coordinated updates with the engine's world clock.</remarks>
    public class AttachmentSpriteTickController
        : VectoredTickControllerBase<AeSpriteAttachment>
    {
        /// <summary>
        /// Initializes a new instance of the AttachmentSpriteTickController class with the specified engine and sprite
        /// manager.
        /// </summary>
        /// <param name="engine">The engine instance used to coordinate game logic and rendering operations.</param>
        /// <param name="manager">The sprite manager responsible for handling sprite objects and their lifecycle.</param>
        public AttachmentSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        /// <summary>
        /// Advances the simulation state for all visible sprites by applying motion and intelligence updates for the
        /// current world clock tick.
        /// </summary>
        /// <remarks>This method does not perform any updates when the engine is in edit mode. Motion and
        /// intelligence are applied only to sprites that are visible and not dead or exploded. Multiplayer action
        /// vectors are recorded if a lobby is active.</remarks>
        /// <param name="epoch">The current simulation time, in seconds, used to update sprite states.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera, used to adjust sprite motion and intelligence calculations.</param>
        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            if (Engine.ExecutionMode == AeEngineExecutionMode.Edit)
            {
                return;
            }

            foreach (var sprite in Visible().Where(o => o.IsDeadOrExploded == false))
            {
                sprite.ApplyMotion(epoch, cameraDisplacement);
                sprite.ApplyIntelligence(epoch, cameraDisplacement);

                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(sprite.GetMultiPlayActionVector());
            }
        }

        /// <summary>
        /// Creates and attaches a new sprite attachment to the specified owner at a given relative location.
        /// </summary>
        /// <remarks>The attachment is rendered above the owner sprite. The attachment's ownership and
        /// relative location are set upon creation.</remarks>
        /// <param name="assetKey">The key identifying the asset to use for the attachment. Must correspond to a valid sprite asset.</param>
        /// <param name="owner">The interactive sprite that will own the new attachment. Cannot be null.</param>
        /// <param name="locationRelativeToOwner">The location, relative to the owner, where the attachment will be placed. Cannot be null.</param>
        /// <returns>A new instance of AeSpriteAttachment representing the attached sprite.</returns>
        public AeSpriteAttachment AddAttachment(string assetKey, AeSpriteInteractive owner, AeVector locationRelativeToOwner)
        {
            var sprite = Engine.Sprites.Add<AeSpriteAttachment>(assetKey, (o) =>
            {
                o.Z = owner.Z + 1; //We want to make sure these go on top of the parent.
                o.OwnerUID = owner.UID;
                o.LocationRelativeToOwner = locationRelativeToOwner.Clone();
            });
            return sprite;
        }
    }
}
