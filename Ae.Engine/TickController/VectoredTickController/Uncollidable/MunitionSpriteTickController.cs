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

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    /// <summary>
    /// Controls the ticking and collision handling of munition sprites within the game world. Manages their movement,
    /// collision detection, and interaction with other sprites during each world clock tick.
    /// </summary>
    /// <remarks>This controller uses a thread pool to efficiently process munition movement and collision
    /// checks in parallel, improving performance when handling large numbers of munitions. It distinguishes between
    /// player-fired and enemy-fired munitions to determine valid collision targets. The controller ensures that
    /// munitions are properly exploded and that hit events are recorded for multiplayer synchronization. Thread pool
    /// resources are released automatically when the engine shuts down.</remarks>
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

        /// <summary>
        /// Initializes a new instance of the MunitionSpriteTickController class, managing sprite updates and traversal
        /// for munitions within the engine.
        /// </summary>
        /// <remarks>The controller configures a thread pool for munition traversal based on engine
        /// settings and ensures proper shutdown by stopping the thread pool when the engine shuts down.</remarks>
        /// <param name="engine">The engine instance that provides configuration and lifecycle events for the controller.</param>
        /// <param name="manager">The sprite manager responsible for handling sprite objects associated with munitions.</param>
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

        /// <summary>
        /// Processes all visible munitions for the current world clock tick, applying their motion, intelligence, and
        /// handling collisions with interactive sprites.
        /// </summary>
        /// <remarks>This method updates the state of munitions, detects collisions with interactive
        /// objects, and triggers appropriate actions such as explosions and hit notifications. It uses multithreading
        /// to optimize collision checks and munition updates. Only objects that are not dead or exploded are considered
        /// for collision and hit processing.</remarks>
        /// <param name="epoch">The current time value, in seconds, representing the world clock tick for which munitions are processed.</param>
        /// <param name="cameraDisplacement">The vector representing the camera's displacement during this tick, used to adjust munition movement and
        /// intelligence calculations.</param>
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
                        var hitCandidates = munition.FiredFromType == AeFiredFromType.Player ? objectsPlayerCanHit : objectsEnemyCanHit;

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

        /// <summary>
        /// Adds a munition created from the specified weapon to the sprite manager.
        /// </summary>
        /// <param name="weapon">The weapon used to create the munition to be added. Cannot be null.</param>
        public void Add(AeSpriteWeapon weapon)
        {
            var obj = weapon.CreateMunition();
            SpriteManager.Insert(obj);
        }

        /// <summary>
        /// Adds a munition created from the specified weapon to the sprite manager at the given location.
        /// </summary>
        /// <param name="weapon">The weapon used to create the munition. Cannot be null.</param>
        /// <param name="location">The location where the munition will be placed. If null, the default location is used.</param>
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
        /// <param name="location"></param>
        /// <returns></returns>
        public void AddLockedOnTo(AeSpriteWeapon weapon, AeSpriteInteractive lockedTarget, AeVector? location = null)
        {
            var obj = weapon.CreateMunition(location, lockedTarget);
            SpriteManager.Insert(obj);
        }
    }
}
