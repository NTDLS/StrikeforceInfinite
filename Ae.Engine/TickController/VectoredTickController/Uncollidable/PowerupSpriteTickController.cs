using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite;
using NTDLS.Helpers;
using System;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    public class PowerupSpriteTickController
        : VectoredTickControllerBase<AeSpritePowerup>
    {
        public PowerupSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(epoch, cameraDisplacement);
                sprite.ApplyMotion(epoch, cameraDisplacement);

                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(sprite.GetMultiPlayActionVector());
            }
        }

        public T AddAt<T>(float x, float y) where T : AeSpritePowerup
        {
            object[] param = { Engine };
            var obj = (AeSpritePowerup)Activator.CreateInstance(typeof(T), param).EnsureNotNull();
            obj.Location = new AeVector(x, y);
            SpriteManager.Insert(obj);
            return (T)obj;
        }
    }
}
