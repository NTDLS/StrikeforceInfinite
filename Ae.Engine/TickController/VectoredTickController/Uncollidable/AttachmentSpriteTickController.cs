using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.Interactive;
using System.Linq;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    public class AttachmentSpriteTickController
        : VectoredTickControllerBase<AeSpriteAttachment>
    {
        public AttachmentSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            if (Engine.ExecutionMode == AeConstants.AeEngineExecutionMode.Edit)
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
