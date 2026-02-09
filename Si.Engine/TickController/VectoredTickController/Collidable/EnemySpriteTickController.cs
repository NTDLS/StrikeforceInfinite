using NTDLS.Helpers;
using Si.Engine.Manager;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library.Mathematics;
using System;

namespace Si.Engine.TickController.VectoredTickController.Collidable
{
    public class EnemySpriteTickController : VectoredCollidableTickControllerBase<SpriteEnemyBase>
    {
        private readonly EngineCore _engine;

        public EnemySpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
            _engine = engine;
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(epoch, displacementVector);
                sprite.ApplyMotion(epoch, displacementVector);
                sprite.PerformCollisionDetection(epoch);
                sprite.RenewableResources.RenewAllResources(epoch);

                Engine.MultiplayLobby?.ActionBuffer.RecordVector(sprite.GetMultiPlayActionVector());
            }
        }

        public T AddTypeOf<T>() where T : SpriteEnemyBase
        {
            object[] param = { Engine };
            SpriteEnemyBase obj = (SpriteEnemyBase)Activator.CreateInstance(typeof(T), param).EnsureNotNull();

            //If we do this here, then it doesn't allow us to set the location and orientation in the constructor of the SpriteEnemyBase subclass, which is a problem.
            //obj.Location = Engine.Display.RandomOffScreenLocation();
            //obj.Orientation.Degrees = SiRandom.Between(0, 359);
            //obj.RecalculateOrientationMovementVector();

            obj.BeforeCreate();
            SpriteManager.Add(obj);
            obj.AfterCreate();

            return (T)obj;
        }
    }
}
