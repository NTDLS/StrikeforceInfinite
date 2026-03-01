using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using System.Linq;

namespace Si.Engine.TickController.VectoredTickController.Uncollidable
{
    public class AttachmentSpriteTickController : VectoredTickControllerBase<SpriteAttachment>
    {
        public AttachmentSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            if (Engine.ExecutionMode == SiConstants.SiEngineExecutionMode.Edit)
            {
                return;
            }

            foreach (var sprite in Visible().Where(o => o.IsDeadOrExploded == false))
            {
                sprite.ApplyMotion(epoch, displacementVector);
                sprite.ApplyIntelligence(epoch, displacementVector);

                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(sprite.GetMultiPlayActionVector());
            }
        }

        public SpriteAttachment AddAttachment(string assetKey, SpriteInteractiveBase owner, SiVector locationRelativeToOwner)
        {
            var sprite = Engine.Sprites.Add<SpriteAttachment>(assetKey, (o) =>
            {
                o.Z = owner.Z + 1; //We want to make sure these go on top of the parent.
                o.OwnerUID = owner.UID;
                o.LocationRelativeToOwner = locationRelativeToOwner.Clone();
            });
            return sprite;
        }
    }
}
