using NTDLS.Helpers;
using Ae.Engine.Manager;
using Ae.Engine.Sprite._Superclass;
using Ae.Engine.TickController._Superclass;
using Ae.Library.Mathematics;
using System;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    public class PowerupSpriteTickController
        : VectoredTickControllerBase<SpritePowerup>
    {
        public PowerupSpriteTickController(SiEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector cameraDisplacement)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(epoch, cameraDisplacement);
                sprite.ApplyMotion(epoch, cameraDisplacement);

                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(sprite.GetMultiPlayActionVector());
            }
        }

        public T AddAt<T>(float x, float y) where T : SpritePowerup
        {
            object[] param = { Engine };
            var obj = (SpritePowerup)Activator.CreateInstance(typeof(T), param).EnsureNotNull();
            obj.Location = new SiVector(x, y);
            SpriteManager.Insert(obj);
            return (T)obj;
        }
    }
}
