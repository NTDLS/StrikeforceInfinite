using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite;
using Ae.Engine.Sprite.Base;
using Ae.Engine.Sprite.Interactive;
using Ae.Engine.Sprite.Munition;
using NTDLS.DelegateThreadPooling;
using System;
using System.Collections.Concurrent;
using System.Linq;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    public class MunitionSpriteTickController
        : VectoredTickControllerBase<AeSpriteMunition>
    {
        #region Private Classes.

        private struct MunitionObjectHit
        {
            public AeSprite Object { get; set; }
            public AeSpriteMunition Munition { get; set; }

            public MunitionObjectHit(AeSpriteMunition munition, AeSprite obj)
            {
                Object = obj;
                Munition = munition;
            }
        }

        #endregion

        private readonly DelegateThreadPool _munitionTraversalThreadPool;

        public MunitionSpriteTickController(AeEngine engine, SpriteManager manager)
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

        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            var munitions = VisibleOfType<AeSpriteMunition>();
            if (munitions.Count() != 0)
            {
                var interactiveSprites = SpriteManager.VisibleDamageable();
                var objectsPlayerCanHit = interactiveSprites.Where(o => o is not AeSpritePlayer).ToArray();
                var objectsEnemyCanHit = interactiveSprites.Where(o => o is AeSpritePlayer).ToArray();

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
                            => AeAxisAlignedBoundingBox.AabbOverlaps(munition.SweptAabbForMotion(epoch), o.GetAabbMinMaxRotated())).ToArray();

                        threadPoolTracker.Enqueue(() => //Enqueue an item into the thread pool.
                        {
                            munition.ApplyMotion(epoch, cameraDisplacement); //Move the munition.
                            munition.ApplyIntelligence(epoch, cameraDisplacement);
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
                try
                {
                    threadPoolTracker.WaitForCompletion();
                }
                catch
                {
                    //This is likely a shutdown of the engine while waiting, so we can just ignore it.
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

        public void Add(AeSpriteWeapon weapon)
        {
            var obj = weapon.CreateMunition();
            SpriteManager.Insert(obj);
        }

        public void Add(AeSpriteWeapon weapon, AeVector? location = null)
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
        public void AddLockedOnTo(AeSpriteWeapon weapon, AeSpriteInteractive lockedTarget, AeVector? location = null)
        {
            var obj = weapon.CreateMunition(location, lockedTarget);
            SpriteManager.Insert(obj);
        }
    }
}
