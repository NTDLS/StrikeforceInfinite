using NTDLS.DelegateThreadPooling;
using Si.Engine.Manager;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.Mathematics;
using System;
using System.Collections.Concurrent;
using System.Linq;
using static Si.Library.SiConstants;

namespace Si.Engine.TickController.VectoredTickController.Uncollidable
{
    public class MunitionSpriteTickController : VectoredTickControllerBase<MunitionBase>
    {
        #region Private Classes.

        private struct MunitionObjectHit
        {
            public SpriteBase Object { get; set; }
            public MunitionBase Munition { get; set; }

            public MunitionObjectHit(MunitionBase munition, SpriteBase obj)
            {
                Object = obj;
                Munition = munition;
            }
        }

        #endregion

        private readonly DelegateThreadPool _munitionTraversalThreadPool;

        public MunitionSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
            _munitionTraversalThreadPool = new(new DelegateThreadPoolConfiguration()
            {
                InitialThreadCount = engine.Settings.MunitionTraversalThreads,
                MaximumThreadCount = engine.Settings.MunitionTraversalThreads * 4
            });

            engine.OnShutdown += (engine) =>
            {
                _munitionTraversalThreadPool.Stop();
            };
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            var munitions = VisibleOfType<MunitionBase>();
            if (munitions.Count() != 0)
            {
                var interactiveSprites = SpriteManager.VisibleDamageable();
                var objectsPlayerCanHit = interactiveSprites.Where(o => o is not SpritePlayerBase).ToArray();
                var objectsEnemyCanHit = interactiveSprites.Where(o => o is SpritePlayerBase).ToArray();

                //Create a collection of threads so we can wait on the ones that we start.
                var threadPoolTracker = _munitionTraversalThreadPool.CreateChildPool();

                var hitObjects = new ConcurrentBag<MunitionObjectHit>();

                foreach (var munition in munitions)
                {
                    if (munition.IsDeadOrExploded == false)
                    {
                        var hitCandidates = munition.FiredFromType == SiFiredFromType.Player ? objectsPlayerCanHit : objectsEnemyCanHit;

                        //Filter the hit candidates down to just those that are in the general area of the munition's movement this tick,
                        //  so we don't have to do expensive collision checks against objects that are nowhere near the munition.
                        var filteredCandidates = hitCandidates.Where(o
                            => SiAxisAlignedBoundingBox.AabbOverlaps(munition.SweptAabbForMotion(epoch), o.GetAabbMinMaxRotated())).ToArray();

                        threadPoolTracker.Enqueue(() => //Enqueue an item into the thread pool.
                        {
                            munition.ApplyMotion(epoch, displacementVector); //Move the munition.
                            munition.ApplyIntelligence(epoch, displacementVector);
                            Engine.MultiplayLobby?.ActionBuffer.RecordMotion(munition.GetMultiPlayActionVector());

                            if (filteredCandidates.Length > 0)
                            {
                                var hitObject = munition.FindFirstReverseCollisionAlongMovementVectorAabb(filteredCandidates, epoch);
                                if (hitObject != null)
                                {
                                    hitObjects.Add(new(munition, hitObject));
                                }
                            }
                        });
                    }
                }

                //Wait on all enqueued threads to complete.
                if (SiUtility.TryAndIgnore(() => threadPoolTracker.WaitForCompletion()) == false)
                {
                    return;
                }

                //Take actions with the munitions that hit objects.
                foreach (var hitObject in hitObjects)
                {
                    if (hitObject.Object.IsDeadOrExploded == false)
                    {
                        hitObject.Munition.Explode();
                        Engine.MultiplayLobby?.ActionBuffer.RecordExplode(hitObject.Munition.UID);

                        hitObject.Object.MunitionHit(hitObject.Munition);
                        Engine.MultiplayLobby?.ActionBuffer.RecordHit(hitObject.Object.UID, hitObject.Munition.UID);
                    }
                }
            }
        }

        public void Add(WeaponBase weapon)
        {
            var obj = weapon.CreateMunition();
            SpriteManager.Insert(obj);
        }

        public void Add(WeaponBase weapon, SiVector? location = null)
        {
            var obj = weapon.CreateMunition(location);
            SpriteManager.Insert(obj);
        }

        /// <summary>
        /// Creates a munition that is locked on to another sprite.
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="lockedTarget"></param>
        /// <param name="xyOffset"></param>
        /// <returns></returns>
        public void AddLockedOnTo(WeaponBase weapon, SpriteInteractiveBase lockedTarget, SiVector? location = null)
        {
            var obj = weapon.CreateMunition(location, lockedTarget);
            SpriteManager.Insert(obj);
        }
    }
}
