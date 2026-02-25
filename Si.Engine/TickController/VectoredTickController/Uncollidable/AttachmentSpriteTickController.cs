using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController._Superclass;
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
            foreach (var sprite in Visible().Where(o => o.IsDeadOrExploded == false))
            {
                sprite.ApplyMotion(epoch, displacementVector);
                sprite.ApplyIntelligence(epoch, displacementVector);

                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(sprite.GetMultiPlayActionVector());
            }
        }

        /*
        public SpriteAttachment Add(SpriteBase owner, string? imagePath = null)
        {
            var sprite = new SpriteAttachment(Engine, imagePath)
            {
                Z = owner.Z + 1, //We want to make sure these go on top of the parent.
                OwnerUID = owner.UID
            };

            SpriteManager.Add(sprite);
            return sprite;
        }

        public SpriteAttachment AddTypeOf<T>(SpriteBase owner, string? imagePath = null) where T : SpriteAttachment
        {
            var sprite = SpriteManager.CreateByType<T>();

            if (imagePath != null) sprite.SetImage(imagePath);
            sprite.Z = owner.Z + 1; //We want to make sure these go on top of the parent.
            sprite.OwnerUID = owner.UID;

            SpriteManager.Add(sprite);
            return sprite;
        }
        */

        public SpriteAttachment AddTypeOf(string spritePath, SpriteInteractiveBase owner, SiVector locationRelativeToOwner)
        {
            var sprite = Engine.Sprites.Add<SpriteAttachment>(spritePath);

            sprite.Z = owner.Z + 1; //We want to make sure these go on top of the parent.
            sprite.OwnerUID = owner.UID;
            sprite.LocationRelativeToOwner = locationRelativeToOwner.Clone();

            SpriteManager.Insert(sprite);
            return sprite;
        }
    }
}
