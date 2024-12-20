﻿using Si.Engine;
using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics;
using System.Linq;

namespace Si.GameEngine.TickController.VectoredTickController.Uncollidable
{
    public class AttachmentSpriteTickController : VectoredTickControllerBase<SpriteAttachment>
    {
        public AttachmentSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            foreach (var attachment in Visible().Where(o => o.IsDeadOrExploded == false))
            {
                attachment.ApplyMotion(epoch, displacementVector);
                attachment.ApplyIntelligence(epoch, displacementVector);
            }
        }

        public SpriteAttachment Add(SpriteBase owner, string? imagePath = null)
        {
            var obj = new SpriteAttachment(Engine, imagePath)
            {
                Z = owner.Z + 1, //We want to make sure these go on top of the parent.
                OwnerUID = owner.UID
            };
            SpriteManager.Add(obj);
            return obj;
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

        public SpriteAttachment AddTypeOf(string typeName, SpriteInteractiveBase owner, SiVector locationRelativeToOwner)
        {
            var sprite = CreateSubtypeByName(typeName);

            sprite.Z = owner.Z + 1; //We want to make sure these go on top of the parent.
            sprite.OwnerUID = owner.UID;
            sprite.LocationRelativeToOwner = locationRelativeToOwner.Clone();

            SpriteManager.Add(sprite);
            return sprite;
        }
    }
}
