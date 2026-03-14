using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Mathematics.KinematicBody;
using Ae.Engine.Menu;
using Ae.Engine.Sprite;
using Ae.Engine.Sprite.Base;
using Ae.Engine.Sprite.Interactive;
using Ae.Engine.Sprite.Interactive.Ship;
using Ae.Engine.Sprite.Munition;
using Ae.Engine.TickController.UnvectoredTickController;
using Ae.Engine.TickController.VectoredTickController.Collidable;
using Ae.Engine.TickController.VectoredTickController.Uncollidable;
using Microsoft.CodeAnalysis;
using NTDLS.Helpers;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ae.Engine.Manager
{
    /// <summary>
    /// Contains the collection of all sprites and their factories. This class stringently controls access to the internal collection
    ///     only allowing insertion and deletions from it to occur within events so that it can be safely assumes that the collection
    ///     can be enumerated in the world clock controllers without fear of collection modification during enumeration.
    /// </summary>
    public class SpriteManager
        : IDisposable
    {
        /// <summary>
        /// Delegate that allows access to the sprite collection for the developer console.
        /// This is to be used ONLY for the developer console to access the collection. Otherwise, this class manages all access to the internal collection,
        /// </summary>
        public delegate void CollectionAccessor(AeSprite[] sprites);

        /// <summary>
        /// Delegate that allows access to the sprite collection for the developer console.
        /// This is to be used ONLY for the developer console to access the collection. Otherwise, this class manages all access to the internal collection,
        /// </summary>
        public delegate T CollectionAccessorT<T>(AeSprite[] sprites);

        private readonly AeEngine _engine;
        private AeVector? _radarScale;
        private AeVector? _radarOffset;

        /// <summary>
        /// Whether the radar should be rendered.
        /// </summary>
        public bool RenderRadar { get; set; } = false;

        private readonly List<AeSprite> _collection = new();

        #region Sprite Tick Controllerss.

        /// <summary>
        /// Tick controller for Animations sprites.
        /// </summary>
        public AnimationSpriteTickController Animations { get; private set; }
        /// <summary>
        /// Tick controller for Attachments sprites.
        /// </summary>
        public AttachmentSpriteTickController Attachments { get; private set; }
        /// <summary>
        /// Tick controller for InteractiveBitmaps sprites.
        /// </summary>
        public InteractiveBitmapSpriteTickController InteractiveBitmaps { get; private set; }
        /// <summary>
        /// Tick controller for GenericBitmaps sprites.
        /// </summary>
        public MinimalBitmapSpriteTickController GenericBitmaps { get; private set; }
        /// <summary>
        /// Tick controller for Munitions sprites.
        /// </summary>
        public MunitionSpriteTickController Munitions { get; private set; }
        /// <summary>
        /// Tick controller for Debugs sprites.
        /// </summary>
        public DebugSpriteTickController Debugs { get; private set; }
        /// <summary>
        /// Tick controller for Enemies sprites.
        /// </summary>
        public EnemySpriteTickController Enemies { get; private set; }
        /// <summary>
        /// Tick controller for Particles sprites.
        /// </summary>
        public ParticleSpriteTickController Particles { get; private set; }
        /// <summary>
        /// Tick controller for Powerups sprites.
        /// </summary>
        public PowerupSpriteTickController Powerups { get; private set; }
        /// <summary>
        /// Tick controller for RadarPositions sprites.
        /// </summary>
        public RadarPositionsSpriteTickController RadarPositions { get; set; }
        /// <summary>
        /// Tick controller for Stars sprites.
        /// </summary>
        public StarSpriteTickController Stars { get; private set; }
        /// <summary>
        /// Tick controller for TextBlocks sprites.
        /// </summary>
        public TextBlocksSpriteTickController TextBlocks { get; private set; }
        /// <summary>
        /// Tick controller for SkyBoxes sprites.
        /// </summary>
        public SkyBoxSpriteTickController SkyBoxes { get; private set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the SpriteManager class using the specified engine. Provides access to
        /// controllers for managing various sprite types within the engine.
        /// </summary>
        /// <remarks>Each controller property manages a specific category of sprites, such as animations,
        /// enemies, or particles. Use these controllers to update, render, or interact with their respective sprite
        /// types. The SpriteManager should be created once per engine instance to ensure consistent sprite
        /// management.</remarks>
        /// <param name="engine">The engine instance used to coordinate sprite management and rendering operations. Cannot be null.</param>
        public SpriteManager(AeEngine engine)
        {
            _engine = engine;

            Animations = new AnimationSpriteTickController(_engine, this);
            Attachments = new AttachmentSpriteTickController(_engine, this);
            Debugs = new DebugSpriteTickController(_engine, this);
            Enemies = new EnemySpriteTickController(_engine, this);
            InteractiveBitmaps = new InteractiveBitmapSpriteTickController(_engine, this);
            GenericBitmaps = new MinimalBitmapSpriteTickController(_engine, this);
            Munitions = new MunitionSpriteTickController(_engine, this);
            Particles = new ParticleSpriteTickController(_engine, this);
            Powerups = new PowerupSpriteTickController(_engine, this);
            RadarPositions = new RadarPositionsSpriteTickController(_engine, this);
            SkyBoxes = new SkyBoxSpriteTickController(_engine, this);
            Stars = new StarSpriteTickController(_engine, this);
            TextBlocks = new TextBlocksSpriteTickController(_engine, this);
        }

        /// <summary>
        /// Returns an array of sprites that are currently visible.
        /// </summary>
        /// <returns>An array of <see cref="AeSprite"/> objects for which <see cref="AeSprite.IsVisible"/> is <see
        /// langword="true"/>. The array will be empty if no sprites are visible.</returns>
        public AeSprite[] Visible() => _collection.Where(o => o.IsVisible == true).ToArray();

        /// <summary>
        /// Returns an array containing all sprites in the collection.
        /// </summary>
        /// <returns>An array of <see cref="AeSprite"/> objects representing the current contents of the collection. The array
        /// will be empty if the collection contains no sprites.</returns>
        public AeSprite[] All() => _collection.ToArray();

        /// <summary>
        /// Gets a list of all player sprites currently visible in the scene, including the main player.
        /// </summary>
        /// <remarks>The returned list includes both dynamically visible player sprites and the main
        /// player sprite, even if the main player is not visible to others. Use this property to access all player
        /// entities relevant for rendering or interaction.</remarks>
        public List<AeSpritePlayer> AllVisiblePlayers
        {
            get
            {
                var players = VisibleOfType<AeSpritePlayer>().ToList();
                players.Add(_engine.Player.Sprite);
                return players;
            }
        }

        /// <summary>
        /// This is to be used ONLY for the debugger to access the collection. Otherwise, this class managed all access to the internal collection,
        /// </summary>
        public void DeveloperOnlyAccess(CollectionAccessor collectionAccessor)
            => collectionAccessor(All());

        /// <summary>
        /// Queues all sprites of a given type for deletion.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void QueueAllForDeletionOfType<T>() where T : AeSprite
        {
            var sprites = OfType<T>();
            foreach (var sprite in sprites)
            {
                sprite.QueueForDelete();
            }
        }

        /// <summary>
        /// Cleanup any resources used by the SpriteManager. This should be called when the engine is shutting down to ensure that all resources are properly released.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Creates a new instance of the AeSprite class using the specified asset key.
        /// </summary>
        /// <param name="assetKey">The key identifying the asset to use for the sprite. Cannot be null or empty.</param>
        /// <param name="initializationProc">An optional delegate to perform additional initialization on the AeSprite instance after creation. If null,
        /// no extra initialization is performed.</param>
        /// <returns>An AeSprite instance initialized with the specified asset key. The instance will be further configured if an
        /// initialization procedure is provided.</returns>
        public AeSprite Create(string assetKey, Action<AeSprite>? initializationProc = null)
            => Create<AeSprite>(assetKey, initializationProc);

        /// <summary>
        /// Creates a new sprite instance of the specified type using the provided asset key and optional initialization
        /// procedure.
        /// </summary>
        /// <remarks>The created sprite instance is initialized with the engine and asset key. Use the
        /// initialization procedure to configure additional properties or state as needed.</remarks>
        /// <typeparam name="T">The type of sprite to create. Must inherit from AeSprite.</typeparam>
        /// <param name="assetKey">The key identifying the sprite asset to instantiate. Cannot be null or empty.</param>
        /// <param name="initializationProc">An optional procedure to initialize the created sprite instance. If provided, it will be invoked after the
        /// sprite is constructed.</param>
        /// <returns>A new instance of type T representing the created sprite.</returns>
        /// <exception cref="Exception">Thrown if the asset key does not correspond to a valid sprite asset or if the asset metadata does not define
        /// a class or controller.</exception>
        public T Create<T>(string assetKey, Action<T>? initializationProc = null) where T : AeSprite
        {
            var asset = _engine.Assets.GetAsset(assetKey)
                ?? throw new Exception($"No metadata found for sprite path: {assetKey}");

            var className = (string.IsNullOrEmpty(asset.ControllerName) ? asset.Metadata.Class : asset.ControllerName)
                ?? throw new Exception($"The sprite {assetKey} does not have a class or controller defined in its metadata.");
            var type = AeReflection.GetTypeByName(className);
            var sprite = (T)Activator.CreateInstance(type, [_engine, assetKey]).EnsureNotNull();
            initializationProc?.Invoke(sprite);
            return sprite;
        }

        /// <summary>
        /// Adds a new sprite to the editor using the specified asset key and optional initialization procedure.
        /// </summary>
        /// <remarks>This method is intended for use only in Editor mode. If the asset key does not
        /// correspond to a valid sprite asset, or if an error occurs during sprite creation, a default sprite is
        /// returned and the error is logged using the provided delegate.</remarks>
        /// <param name="assetKey">The unique key identifying the sprite asset to add. Must correspond to a valid sprite asset in the engine's
        /// asset collection.</param>
        /// <param name="writeLog">A delegate used to log errors or informational messages during sprite creation. Can be null if logging is
        /// not required.</param>
        /// <param name="initializationProc">An optional action to perform additional initialization on the created sprite before it is inserted into the
        /// editor. Can be null if no custom initialization is needed.</param>
        /// <returns>The created sprite instance. If sprite creation fails, returns a default sprite constructed with the
        /// specified asset key.</returns>
        /// <exception cref="Exception">Thrown if the method is called when the engine is not in Editor mode, or if required constructor parameters
        /// are not handled.</exception>
        public AeSprite EditorAdd(string assetKey, WriteLogDelegate writeLog, Action<AeSprite>? initializationProc = null)
        {
            if (_engine.ExecutionMode != AeEngineExecutionMode.Edit)
            {
                throw new Exception("EditorAdd can only be used in Editor mode.");
            }

            try
            {
                var metadata = _engine.Assets.GetMetadata(assetKey)
                     ?? throw new Exception($"No metadata found for sprite path: {assetKey}");

                var className = string.IsNullOrEmpty(metadata.Class) ? "AeSprite" : metadata.Class;
                var classType = AeReflection.GetTypeByName(className);

                if (typeof(AeSprite).IsAssignableFrom(classType))
                {
                    //We only add assets to the sprite collection that are actually sprites.

                    var firstConstructor = classType.GetConstructors().First();
                    var parameters = firstConstructor.GetParameters();

                    List<dynamic?> constructorParams = new();

                    foreach (var parameter in parameters)
                    {
                        switch (parameter.Name)
                        {
                            case "engine":
                                constructorParams.Add(_engine);
                                break;
                            case "assetKey":
                                constructorParams.Add(assetKey);
                                break;
                            case "firedFrom":
                                constructorParams.Add(new AeSpriteEnemy(_engine, "Sprites/#Internal/Ghost"));
                                break;
                            case "owner":
                                constructorParams.Add(new AeSpriteInteractive(_engine, "Sprites/#Internal/Ghost"));
                                break;
                            case "weapon":
                                constructorParams.Add(new AeSpriteWeapon(_engine, new AeSpriteInteractive(_engine, "Sprites/#Internal/Ghost"), "Sprites/#Internal/Ghost"));
                                break;
                            case "lockedTarget":
                                constructorParams.Add(new AeSpriteInteractive(_engine, "Sprites/#Internal/Ghost"));
                                break;
                            case "location":
                                constructorParams.Add(AeVector.Zero());
                                break;
                            default:
                                throw new Exception($"Constructor parameter {parameter.Name} for {classType.Name} is not handled.");
                        }
                    }

                    var sprite = (AeSprite)Activator.CreateInstance(classType, constructorParams.ToArray()).EnsureNotNull();
                    initializationProc?.Invoke(sprite);

                    Insert(sprite);
                    return sprite;
                }
            }
            catch (Exception ex)
            {
                writeLog?.Invoke($"Error creating sprite with asset key {assetKey}: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }

            return new AeSprite(_engine, assetKey);
        }

        /// <summary>
        /// Creates and adds a new sprite using the specified asset key, optionally applying a custom initialization
        /// procedure.
        /// </summary>
        /// <param name="assetKey">The key identifying the asset to use for the sprite. Cannot be null or empty.</param>
        /// <param name="initializationProc">An optional action to perform additional initialization on the created sprite before it is added. If null,
        /// no custom initialization is applied.</param>
        /// <returns>The newly created and added sprite instance.</returns>
        public AeSprite Add(string assetKey, Action<AeSprite>? initializationProc = null)
            => Add<AeSprite>(assetKey, initializationProc);

        /// <summary>
        /// Adds a new sprite of the specified type to the collection using the given asset key.
        /// </summary>
        /// <typeparam name="T">The type of sprite to add. Must inherit from AeSprite.</typeparam>
        /// <param name="assetKey">The key identifying the asset to use when creating the sprite. Cannot be null or empty.</param>
        /// <param name="initializationProc">An optional action to initialize the sprite after creation. If provided, this action will be invoked with
        /// the new sprite instance.</param>
        /// <returns>The newly created and added sprite instance of type T.</returns>
        public T Add<T>(string assetKey, Action<T>? initializationProc = null) where T : AeSprite
        {
            var sprite = Create<T>(assetKey, initializationProc);
            Insert(sprite);
            return sprite;
        }

        /// <summary>
        /// Creates and inserts a new sprite of the specified type using the provided bitmap, optionally initializing it
        /// with a custom procedure.
        /// </summary>
        /// <remarks>The sprite is immediately inserted into the collection after creation. Use the
        /// initialization procedure to set up custom properties or state before insertion if needed.</remarks>
        /// <typeparam name="T">The type of sprite to create. Must inherit from AeSprite.</typeparam>
        /// <param name="bitmap">The bitmap to associate with the new sprite. Used to define the sprite's visual content.</param>
        /// <param name="initializationProc">An optional procedure to initialize the sprite after creation. If provided, it is invoked with the new
        /// sprite instance.</param>
        /// <returns>The newly created and inserted sprite instance of type T.</returns>
        public T Add<T>(SharpDX.Direct2D1.Bitmap bitmap, Action<T>? initializationProc = null) where T : AeSprite
        {
            T sprite = (T)Activator.CreateInstance(typeof(T), [_engine, bitmap]).EnsureNotNull();
            initializationProc?.Invoke(sprite);
            Insert(sprite);
            return sprite;
        }

        /// <summary>
        /// Adds the specified sprite to the manager for later processing and materialization.
        /// </summary>
        /// <remarks>If the engine is currently initializing, the sprite will not be added to the
        /// collection. The sprite is materialized and its spawn action is recorded for multiplayer lobbies after
        /// insertion.</remarks>
        /// <param name="sprite">The sprite to be added to the manager. Cannot be null.</param>
        /// <exception cref="Exception">Thrown if the sprite parameter is null.</exception>
        public void Insert(AeSprite sprite)
        {
            if (_engine.IsInitializing == true)
            {
                //When the engine is initializing, we do all kinds of pre-caching.
                //We want to make sure that none of these new classes make it to the sprite collection.
                return;
            }

            if (sprite == null)
            {
                throw new Exception("NULL sprites cannot be added to the manager.");
            }

            _engine.Invoke(() =>
            {
                _collection.Add(sprite);
                sprite.OnMaterialized();
            });

            _engine.MultiplayLobby?.ActionBuffer.RecordSpawn(sprite.GetMultiPlayActionSpawn());
        }

        /// <summary>
        /// Permanently removes all sprites that are queued for deletion from the collection and performs related
        /// cleanup operations.
        /// </summary>
        /// <remarks>This method records deletion actions, cleans up associated resources, and updates
        /// game state as needed. If the player's sprite is dead or exploded, it becomes invisible, is revived, and the
        /// new game menu is shown. Use this method when you need to ensure that all queued deletions are fully
        /// processed and the game state is reset accordingly.</remarks>
        public void HardDeleteAllQueuedDeletions()
        {
            _collection.Where(o => o.IsQueuedForDeletion).ToList().ForEach(sprite =>
            {
                _engine.MultiplayLobby?.ActionBuffer.RecordDelete(sprite.UID);

                sprite.Cleanup();
            });

            _collection.RemoveAll(o => o.IsQueuedForDeletion);

            _engine.Events.CleanupQueuedForDeletion();

            if (_engine.Player.Sprite.IsDeadOrExploded)
            {
                _engine.Player.Sprite.IsVisible = false;
                _engine.Player.Sprite.ReviveDeadOrExploded();
                _engine.Menus.Show(new AeMenuStartNewGame(_engine));
            }
        }

        /// <summary>
        /// Queues all items in the collection for deletion.
        /// </summary>
        /// <remarks>This method marks every item in the underlying collection for deletion by invoking
        /// their respective deletion queue operation. Use this method when you need to schedule all items for removal
        /// in a single operation.</remarks>
        public void QueueAllForDeletion()
            => _collection.ForEach(o => o.QueueForDelete());

        /// <summary>
        /// Deletes all the non-background sprite types.
        /// </summary>
        public void QueueDeletionOfActionSprites()
        {
            Powerups.QueueAllForDeletion();
            Enemies.QueueAllForDeletion();
            Munitions.QueueAllForDeletion();
            Animations.QueueAllForDeletion();
        }

        /// <summary>
        /// Retrieves an array of sprites of type T that have the specified tag name.
        /// </summary>
        /// <typeparam name="T">The sprite type to filter and return. Must inherit from AeSprite.</typeparam>
        /// <param name="name">The tag name used to filter sprites. Only sprites with a matching tag are included in the result.</param>
        /// <returns>An array of sprites of type T that have the specified tag name. Returns null if no matching sprites are
        /// found or if the cast fails.</returns>
        public T[]? GetSpritesByTag<T>(string name) where T : AeSprite
            => _collection.Where(o => o.SpriteTag == name).ToArray() as T[];

        /// <summary>
        /// Retrieves a single sprite of the specified type that matches the given tag name, or returns null if no match
        /// is found.
        /// </summary>
        /// <remarks>If multiple sprites share the same tag name, an exception is thrown. Use this method
        /// when the tag is expected to be unique within the collection.</remarks>
        /// <typeparam name="T">The type of sprite to retrieve. Must inherit from AeSprite.</typeparam>
        /// <param name="name">The tag name used to identify the sprite to retrieve. Cannot be null.</param>
        /// <returns>The sprite of type T with the specified tag name, or null if no matching sprite exists.</returns>
        public T? GetSingleSpriteByTag<T>(string name) where T : AeSprite
            => _collection.Where(o => o.SpriteTag == name).SingleOrDefault() as T;

        /// <summary>
        /// Retrieves the sprite of the specified type that is owned by the given unique identifier.
        /// </summary>
        /// <remarks>Use this method to obtain a sprite instance associated with a particular owner. If
        /// multiple sprites exist for the owner, only one will be returned. The result is cast to the specified type T;
        /// if the sprite is not of type T, null is returned.</remarks>
        /// <typeparam name="T">The type of sprite to retrieve. Must inherit from AeSprite.</typeparam>
        /// <param name="ownerUID">The unique identifier of the sprite owner. Specifies which owner's sprite to retrieve.</param>
        /// <returns>The sprite of type T owned by the specified unique identifier, or null if no matching sprite is found.</returns>
        public T? GetSpriteByOwner<T>(uint ownerUID) where T : AeSprite
            => _collection.Where(o => o.UID == ownerUID).SingleOrDefault() as T;

        /// <summary>
        /// Returns an array containing all elements of the specified type from the collection.
        /// </summary>
        /// <remarks>Use this method to retrieve only elements of a specific derived type from the
        /// collection. The returned array contains only those elements that are assignable to T.</remarks>
        /// <typeparam name="T">The type of elements to filter. Must inherit from AeSprite.</typeparam>
        /// <returns>An array of elements of type T found in the collection. The array will be empty if no elements of the
        /// specified type are present.</returns>
        public T[] OfType<T>() where T : AeSprite
            => _collection.OfType<T>().ToArray();

        /// <summary>
        /// Returns an array of visible sprites of the specified type from the collection.
        /// </summary>
        /// <typeparam name="T">The type of sprite to filter. Must inherit from AeSprite.</typeparam>
        /// <returns>An array containing all visible sprites of type T. The array will be empty if no visible sprites of the
        /// specified type are found.</returns>
        public T[] VisibleOfType<T>() where T : AeSprite
            => _collection.OfType<T>().Where(o => o.IsVisible).ToArray();

        /// <summary>
        /// Returns an array of visible interactive objects of the specified type that are detected as munitions.
        /// </summary>
        /// <remarks>Objects are included only if they are visible and their munition detection metadata
        /// is set to <see langword="true"/>. The returned array contains only elements of type T or null if the cast
        /// fails.</remarks>
        /// <typeparam name="T">The reference type to filter and return from the collection. Must be a class.</typeparam>
        /// <returns>An array of objects of type T that are visible and detected as munitions. The array may be empty if no
        /// matching objects are found.</returns>
        public T?[] VisibleDamageable<T>() where T : class
            => _collection.OfType<AeSpriteInteractive>().Where(o => o.IsVisible && o.Metadata.MunitionDetection == true).Select(o => o as T).ToArray();

        /// <summary>
        /// Returns an array of visible interactive sprites that are eligible for munition detection.
        /// Probably faster than VisibleDamageableT().
        /// </summary>
        /// <remarks>Use this method to retrieve all interactive sprites that can be targeted or affected
        /// by munitions based on their visibility and detection settings.</remarks>
        /// <returns>An array of <see cref="AeSpriteInteractive"/> objects that are currently visible and have munition detection
        /// enabled. The array will be empty if no such sprites are found.</returns>
        public AeSpriteInteractive[] VisibleDamageable()
            => _collection.OfType<AeSpriteInteractive>().Where(o => o.IsVisible && o.Metadata.MunitionDetection == true).ToArray();

        /// <summary>
        /// Returns an array of objects of type T that are visible and support collision detection.
        /// </summary>
        /// <remarks>Only objects that are both visible and have collision detection enabled are included
        /// in the result. Use this method to retrieve interactive sprites that are currently active for collision
        /// processing.</remarks>
        /// <typeparam name="T">The type of interactive sprite to filter and return. Must be a reference type.</typeparam>
        /// <returns>An array of T containing all visible collidable objects. The array may be empty if no matching objects are
        /// found.</returns>
        public T?[] VisibleCollidable<T>() where T : class
            => _collection.OfType<AeSpriteInteractive>().Where(o => o.IsVisible && o.Metadata.CollisionDetection == true).Select(o => o as T).ToArray();

        /// <summary>
        /// Returns an array of interactive sprites that are both visible and configured for collision detection.
        /// Probably faster than VisibleCollidableT().
        /// </summary>
        /// <remarks>Use this method to retrieve only those interactive sprites that can participate in
        /// collision events and are currently rendered. This is useful for scenarios where collision checks should be
        /// limited to visible objects.</remarks>
        /// <returns>An array of <see cref="AeSpriteInteractive"/> objects that are currently visible and have collision
        /// detection enabled. The array will be empty if no such sprites are present.</returns>
        public AeSpriteInteractive[] VisibleCollidable()
            => _collection.OfType<AeSpriteInteractive>().Where(o => o.IsVisible && o.Metadata.CollisionDetection == true).ToArray();

        /// <summary>
        /// Predicts the movement of all visible and collidable sprites for the specified epoch.
        /// </summary>
        /// <param name="epoch">The simulation time, in seconds, for which to predict the movement of visible collidable sprites.</param>
        /// <returns>An array of predicted kinematic bodies representing the state of each visible and collidable sprite at the
        /// specified epoch. The array will be empty if no such sprites are present.</returns>
        public PredictedKinematicBody[] VisibleCollidablePredictiveMove(float epoch)
            => _engine.Sprites.VisibleCollidable().Select(o => new PredictedKinematicBody(o, _engine.Display.CameraPosition, epoch)).ToArray();

        /// <summary>
        /// Returns an array of visible sprites whose types match any of the specified types.
        /// </summary>
        /// <param name="types">An array of types to filter the visible sprites. Only sprites whose type is assignable from any of these
        /// types are included.</param>
        /// <returns>An array of visible sprites matching the specified types. The array will be empty if no matching sprites are
        /// found.</returns>
        public AeSprite[] VisibleOfTypes(Type[] types)
        {
            var result = new List<AeSprite>();
            foreach (var type in types)
            {
                result.AddRange(_collection.Where(o => o.IsVisible == true && type.IsAssignableFrom(o.GetType())));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Queues all sprites with the specified tag for deletion.
        /// </summary>
        /// <remarks>This method iterates through the collection and queues each sprite with a matching
        /// tag for deletion. No action is taken for sprites without the specified tag.</remarks>
        /// <param name="name">The tag name used to identify sprites to be queued for deletion. Cannot be null.</param>
        public void QueueAllForDeletionByTag(string name)
        {
            foreach (var sprite in _collection)
            {
                if (sprite.SpriteTag == name)
                {
                    sprite.QueueForDelete();
                }
            }
        }

        /// <summary>
        /// Queues all sprites owned by the specified owner for deletion.
        /// </summary>
        /// <remarks>Use this method to mark all sprites associated with a particular owner for removal.
        /// Sprites will not be deleted immediately, but will be processed according to the deletion queue logic. This
        /// method is useful for bulk removal scenarios, such as when an owner is removed from the system.</remarks>
        /// <param name="ownerUID">The unique identifier of the owner whose sprites should be queued for deletion.</param>
        public void QueueAllForDeletionByOwner(uint ownerUID)
        {
            foreach (var sprite in _collection)
            {
                if (sprite.OwnerUID == ownerUID)
                {
                    sprite.QueueForDelete();
                }
            }
        }

        /// <summary>
        /// Returns all visible sprites in the collection that intersect with the specified sprite using axis-aligned
        /// bounding box (AABB) collision detection.
        /// </summary>
        /// <remarks>The returned array excludes the specified sprite itself. Only sprites marked as
        /// visible are considered for intersection checks.</remarks>
        /// <param name="with">The sprite to check for intersections against other visible sprites in the collection. Cannot be null.</param>
        /// <returns>An array of sprites that are visible and intersect with the specified sprite. The array will be empty if no
        /// intersections are found.</returns>
        public AeSprite[] Intersections(AeSprite with)
        {
            var objects = new List<AeSprite>();

            foreach (var obj in _collection.Where(o => o.IsVisible == true))
            {
                if (obj != with)
                {
                    if (obj.IntersectsAABB(with.Location, new AeVector(with.Size.Width, with.Size.Height)))
                    {
                        objects.Add(obj);
                    }
                }
            }
            return objects.ToArray();
        }

        /// <summary>
        /// Returns an array of sprites that intersect with the specified rectangular area.
        /// </summary>
        /// <param name="x">The X-coordinate of the upper-left corner of the rectangle to test for intersections.</param>
        /// <param name="y">The Y-coordinate of the upper-left corner of the rectangle to test for intersections.</param>
        /// <param name="width">The width of the rectangle to test for intersections. Must be greater than zero.</param>
        /// <param name="height">The height of the rectangle to test for intersections. Must be greater than zero.</param>
        /// <returns>An array of sprites that intersect with the specified rectangle. The array will be empty if no intersections
        /// are found.</returns>
        public AeSprite[] Intersections(float x, float y, float width, float height)
            => Intersections(new AeVector(x, y), new AeVector(width, height));

        /// <summary>
        /// Returns an array of visible sprites that intersect with the specified axis-aligned bounding box.
        /// </summary>
        /// <remarks>Only sprites marked as visible are considered for intersection tests. The returned
        /// array contains references to the intersecting sprites; modifying these may affect the collection.</remarks>
        /// <param name="location">The location of the axis-aligned bounding box to test for intersections. Represents the minimum corner of
        /// the box.</param>
        /// <param name="size">The size of the axis-aligned bounding box to test for intersections. Specifies the width and height of the
        /// box.</param>
        /// <returns>An array of sprites that are visible and intersect with the specified bounding box. The array will be empty
        /// if no intersections are found.</returns>
        public AeSprite[] Intersections(AeVector location, AeVector size)
        {
            var objects = new List<AeSprite>();

            foreach (var obj in _collection.Where(o => o.IsVisible == true))
            {
                if (obj.IntersectsAABB(location, size))
                {
                    objects.Add(obj);
                }
            }
            return objects.ToArray();
        }

        /// <summary>
        /// Returns an array of sprites whose rendered locations intersect with the specified axis-aligned bounding box.
        /// </summary>
        /// <remarks>This method can be used to detect which sprites overlap a given area, such as for hit
        /// testing or selection. The intersection is determined based on the rendered location and size of each
        /// sprite.</remarks>
        /// <param name="location">The center point of the axis-aligned bounding box to test for intersections.</param>
        /// <param name="size">The size of the axis-aligned bounding box, typically representing its width and height.</param>
        /// <param name="includeInvisible">A value indicating whether to include sprites that are not currently visible. If <see langword="true"/>,
        /// invisible sprites are considered; otherwise, only visible sprites are included.</param>
        /// <returns>An array of <see cref="AeSprite"/> objects that intersect with the specified bounding box. The array will be
        /// empty if no intersections are found.</returns>
        public AeSprite[] RenderLocationIntersections(AeVector location, AeVector size, bool includeInvisible = false)
        {
            var objects = new List<AeSprite>();

            foreach (var obj in _collection.Where(o => o.IsVisible == true || includeInvisible))
            {
                if (obj.RenderLocationIntersectsAABB(location, size))
                {
                    objects.Add(obj);
                }
            }
            return objects.ToArray();
        }

        internal void RenderPostScaling(SharpDX.Direct2D1.RenderTarget renderTarget, float epoch)
        {
            foreach (var sprite in _collection.Where(o => o.IsVisible == true && o.RenderScaleOrder == AeRenderScaleOrder.PostScale).OrderBy(o => o.Z))
            {
                sprite.Render(renderTarget, epoch);
            }

            if (RenderRadar)
            {
                var radarBgImage = _engine.Assets.GetBitmap("Sprites/RadarTransparent");

                _engine.Rendering.DrawBitmap(renderTarget, radarBgImage,
                    _engine.Display.NaturalScreenSize.Width - radarBgImage.Size.Width,
                    _engine.Display.NaturalScreenSize.Height - radarBgImage.Size.Height, 0);

                float radarDistance = 8;

                if (_radarScale == null)
                {
                    float radarVisionWidth = _engine.Display.TotalCanvasSize.Width * radarDistance;
                    float radarVisionHeight = _engine.Display.TotalCanvasSize.Height * radarDistance;

                    _radarScale = new AeVector(radarBgImage.Size.Width / radarVisionWidth, radarBgImage.Size.Height / radarVisionHeight);
                    _radarOffset = new AeVector(radarBgImage.Size.Width / 2.0f, radarBgImage.Size.Height / 2.0f); //Best guess until player is visible.
                }

                if (_engine.Player.Sprite is not null && _engine.Player.Sprite.IsVisible)
                {
                    float centerOfRadarX = (int)(radarBgImage.Size.Width / 2.0f) - 2.0f; //Subtract half the dot size.
                    float centerOfRadarY = (int)(radarBgImage.Size.Height / 2.0f) - 2.0f; //Subtract half the dot size.

                    _radarOffset = new AeVector(
                            _engine.Display.NaturalScreenSize.Width - radarBgImage.Size.Width + (centerOfRadarX - _engine.Player.Sprite.X * _radarScale.X),
                            _engine.Display.NaturalScreenSize.Height - radarBgImage.Size.Height + (centerOfRadarY - _engine.Player.Sprite.Y * _radarScale.Y)
                        );

                    //Render radar:
                    foreach (var sprite in _collection.Where(o => o.IsVisible == true))
                    {
                        //SiPoint scale, SiPoint< float > offset
                        int x = (int)(_radarOffset.X + sprite.Location.X * _radarScale.X);
                        int y = (int)(_radarOffset.Y + sprite.Location.Y * _radarScale.Y);

                        if (x > _engine.Display.NaturalScreenSize.Width - radarBgImage.Size.Width
                            && x < _engine.Display.NaturalScreenSize.Width - radarBgImage.Size.Width + radarBgImage.Size.Width
                            && y > _engine.Display.NaturalScreenSize.Height - radarBgImage.Size.Height
                            && y < _engine.Display.NaturalScreenSize.Height - radarBgImage.Size.Height + radarBgImage.Size.Height
                            )
                        {
                            if ((sprite is AeSpritePlayer || sprite is AeSpriteEnemy || sprite is AeSpriteMunition || sprite is AeSpritePowerup) && sprite.IsVisible == true)
                            {
                                sprite.RenderRadar(renderTarget, x, y);
                            }
                        }
                    }

                    //Render player blip:
                    _engine.Rendering.DrawSolidEllipse(
                        renderTarget,
                        _engine.Display.NaturalScreenSize.Width - radarBgImage.Size.Width + centerOfRadarX,
                        _engine.Display.NaturalScreenSize.Height - radarBgImage.Size.Height + centerOfRadarY,
                        2, 2, _engine.Rendering.Materials.Colors.Green);
                }
            }
        }

        /// <summary>
        /// Will render the current game state to a single bitmap. If a lock cannot be acquired
        /// for drawing then the previous frame will be returned.
        /// </summary>
        /// <returns></returns>
        internal void RenderPreScaling(SharpDX.Direct2D1.RenderTarget renderTarget, float epoch)
        {
            foreach (var sprite in _collection.Where(o => o.IsVisible == true && o.RenderScaleOrder == AeRenderScaleOrder.PreScale).OrderBy(o => o.Z))
            {
                if (sprite.IsWithinCurrentScaledScreenBounds)
                {
                    sprite.Render(renderTarget, epoch);
                }
            }

            _engine.Menus.Render(renderTarget, epoch);

            if (_engine.Settings.HighlightNaturalBounds)
            {
                var naturalScreenBounds = _engine.Display.NaturalScreenBounds;
                var rawRectF = new RawRectangleF(naturalScreenBounds.Left, naturalScreenBounds.Top, naturalScreenBounds.Right, naturalScreenBounds.Bottom);

                //Highlight the 1:1 frame
                _engine.Rendering.DrawRectangle(renderTarget, rawRectF, _engine.Rendering.Materials.Colors.Red, 0, 1, 0);
            }
        }

        /// <summary>
        /// Creates particle fragments from the specified sprite's image and adds them to the engine as individual
        /// bitmap sprites.
        /// </summary>
        /// <remarks>This method generates irregular fragments based on the sprite's image and configures
        /// each fragment with randomized properties such as orientation, speed, and fade reduction. The fragments are
        /// added to the engine for further rendering and processing. This is typically used for effects such as sprite
        /// destruction or particle explosions.</remarks>
        /// <param name="sprite">The sprite whose image will be used to generate fragments. Cannot be null; if the sprite's image is null, no
        /// fragments are created.</param>
        public void CreateFragmentsOf(AeSprite sprite)
        {
            var image = sprite.GetImage();
            if (image == null)
            {
                return;
            }

            var fragmentImages = _engine.Rendering.GenerateIrregularFragments(image);

            foreach (var fragmentImage in fragmentImages)
            {
                var fragment = _engine.Sprites.GenericBitmaps.Add(fragmentImage, (o) =>
                {
                    o.Location = sprite.Location.Clone();
                    o.CleanupMode = AeParticleCleanupMode.DistanceOffScreen;
                    o.FadeToBlackReductionAmount = AeRandom.Between(0.001f, 0.01f); //TODO: Can we implement this?
                    o.RotationSpeed = AeRandom.RandomSign(AeRandom.Between(45f, 180f).ToRadians());
                    o.VectorType = AeParticleVectorType.Default;

                    o.Orientation.Degrees = AeRandom.Between(0.0f, 359.0f);
                    o.Speed = AeRandom.Between(100, 350f);
                    o.Throttle = 1;
                });
            }
        }
    }
}
