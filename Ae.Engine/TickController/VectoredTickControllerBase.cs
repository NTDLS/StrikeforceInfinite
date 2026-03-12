using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite.Base;
using NTDLS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ae.Engine.TickController
{
    /// <summary>
    /// Tick managers which update their sprites using the supplied 2D vector.
    /// Also contains various factory methods.
    /// </summary>
    public class VectoredTickControllerBase<T>
        : ITickController<T> where T : AeSprite
    {
        /// <summary>
        /// Gets the engine instance used to execute automation tasks.
        /// </summary>
        public AeEngine Engine { get; private set; }

        /// <summary>
        /// Gets the manager responsible for handling sprite operations within the application.
        /// </summary>
        public SpriteManager SpriteManager { get; private set; }

        /// <summary>
        /// Returns an array containing all visible sprites of the specified subtype.
        /// </summary>
        /// <typeparam name="subType">The type of sprite to filter and return. Must inherit from T.</typeparam>
        /// <returns>An array of visible sprites of type subType. The array will be empty if no sprites of the specified type are
        /// visible.</returns>
        public subType[] VisibleOfType<subType>() where subType : T => SpriteManager.VisibleOfType<subType>();

        /// <summary>
        /// Returns an array of visible sprites of type T managed by the SpriteManager.
        /// </summary>
        /// <returns>An array of sprites of type T that are currently visible. The array will be empty if no visible sprites of
        /// the specified type exist.</returns>
        public T[] Visible() => SpriteManager.VisibleOfType<T>();

        /// <summary>
        /// Returns an array containing all sprites of type T managed by the SpriteManager.
        /// </summary>
        /// <returns>An array of type T containing all sprites currently managed. The array will be empty if no sprites of type T
        /// are present.</returns>
        public T[] All() => SpriteManager.OfType<T>();

        /// <summary>
        /// Returns an array containing all elements of the specified subtype from the collection.
        /// </summary>
        /// <remarks>Use this method to retrieve only elements of a specific subtype from a heterogeneous
        /// collection. The returned array contains only those elements that are assignable to subType.</remarks>
        /// <typeparam name="subType">The subtype of T to filter and return from the collection.</typeparam>
        /// <returns>An array of elements of type subType found in the collection. The array will be empty if no elements of the
        /// specified subtype are present.</returns>
        public subType[] OfType<subType>() where subType : T => SpriteManager.OfType<subType>();

        /// <summary>
        /// Returns the first sprite of type T that has the specified tag name, or null if no matching sprite is found.
        /// </summary>
        /// <param name="name">The tag name used to identify the sprite. Cannot be null.</param>
        /// <returns>The first sprite of type T with the specified tag name, or null if no such sprite exists.</returns>
        public T? FirstByTag(string name) => SpriteManager.OfType<T>().FirstOrDefault(o => o.SpriteTag == name);

        /// <summary>
        /// Returns all items of type T that have the specified tag name.
        /// </summary>
        /// <param name="name">The tag name used to filter items. Cannot be null.</param>
        /// <returns>An enumerable collection of items of type T with the specified tag name. The collection will be empty if no
        /// items match.</returns>
        public IEnumerable<T> AllByTag(string name) => SpriteManager.OfType<T>().Where(o => o.SpriteTag == name);

        /// <summary>
        /// Advances the world clock by the specified epoch and applies the given camera displacement.
        /// </summary>
        /// <param name="epoch">The amount of time, in seconds, by which to advance the world clock. Must be a positive value.</param>
        /// <param name="cameraDisplacement">The vector representing the displacement to apply to the camera during the tick.</param>
        public virtual void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement) { }

        /// <summary>
        /// Initializes a new instance of the VectoredTickControllerBase class with the specified engine and sprite
        /// manager.
        /// </summary>
        /// <param name="engine">The engine instance used to drive the controller's operations. Cannot be null.</param>
        /// <param name="manager">The sprite manager responsible for managing sprites associated with the controller. Cannot be null.</param>
        public VectoredTickControllerBase(AeEngine engine, SpriteManager manager)
        {
            Engine = engine;
            SpriteManager = manager;
        }

        /// <summary>
        /// Queues all instances of the specified type for deletion from the sprite manager.
        /// </summary>
        /// <remarks>This method marks all sprites of type <typeparamref name="T"/> for removal. Actual
        /// deletion occurs according to the sprite manager's processing cycle. Use this method to efficiently clear all
        /// sprites of a given type without manually iterating over them.</remarks>
        public void QueueAllForDeletion()
            => SpriteManager.QueueAllForDeletionOfType<T>();

        #region Tightly-typed Pass through factory methods to SpriteManager.

        /// <summary>
        /// Creates a new instance of type T using the specified asset key and optional initialization procedure.
        /// </summary>
        /// <param name="assetKey">The unique identifier for the asset to be used when creating the instance. Cannot be null or empty.</param>
        /// <param name="initializationProc">An optional delegate that is invoked to initialize the created instance. If null, no additional
        /// initialization is performed.</param>
        /// <returns>A new instance of type T initialized with the specified asset key and, if provided, the initialization
        /// procedure.</returns>
        public T Create(string assetKey, Action<T>? initializationProc = null)
            => SpriteManager.Create<T>(assetKey, initializationProc);

        /// <summary>
        /// Adds a sprite of type T to the manager using the specified asset key and optional initialization procedure.
        /// </summary>
        /// <param name="assetKey">The unique key identifying the asset to be added. Cannot be null or empty.</param>
        /// <param name="initializationProc">An optional procedure to initialize the sprite after it is created. If provided, this action will be invoked
        /// with the new sprite instance.</param>
        /// <returns>The newly added sprite instance of type T.</returns>
        public T Add(string assetKey, Action<T>? initializationProc = null)
            => SpriteManager.Add<T>(assetKey, initializationProc);

        /// <summary>
        /// Inserts the specified sprite into the sprite manager.
        /// </summary>
        /// <param name="sprite">The sprite to insert. Cannot be null.</param>
        public void Insert(T sprite)
             => SpriteManager.Insert(sprite);

        /// <summary>
        /// Creates and adds a new sprite of type T using the specified bitmap, optionally applying an initialization
        /// procedure.
        /// </summary>
        /// <param name="bitmap">The bitmap to use for the sprite's visual representation. Cannot be null.</param>
        /// <param name="initializationProc">An optional delegate that initializes the sprite after creation. If provided, it will be invoked with the
        /// new sprite instance.</param>
        /// <returns>The newly created sprite of type T.</returns>
        public T Add(SharpDX.Direct2D1.Bitmap bitmap, Action<T>? initializationProc = null)
            => SpriteManager.Add<T>(bitmap, initializationProc);

        #endregion

        /// <summary>
        /// Creates a new instance of type T using the configured engine as a constructor parameter.
        /// </summary>
        /// <remarks>This method uses reflection to instantiate the object. Ensure that type T has a
        /// constructor accepting the engine parameter; otherwise, an exception will be thrown.</remarks>
        /// <returns>A new instance of type T initialized with the engine. The instance is guaranteed to be non-null.</returns>
        public T Create()
        {
            return (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
        }

        /// <summary>
        /// Creates a new instance of type T using the specified bitmap file path.
        /// </summary>
        /// <remarks>The created instance is constructed with the current Engine and the specified bitmap
        /// path. Ensure that the bitmap file exists and is accessible to avoid initialization errors.</remarks>
        /// <param name="bitmapPath">The file path to the bitmap image used to initialize the instance. Cannot be null.</param>
        /// <returns>A new instance of type T initialized with the provided bitmap file path.</returns>
        public T Create(string bitmapPath)
        {
            return (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
        }
    }
}
