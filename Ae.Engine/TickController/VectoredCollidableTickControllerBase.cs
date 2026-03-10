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
    public class VectoredCollidableTickControllerBase<T>
        : ITickController<T> where T : SpriteBase
    {
        public AeEngine Engine { get; private set; }
        public SpriteManager SpriteManager { get; private set; }

        public subType[] VisibleOfType<subType>() where subType : T => SpriteManager.VisibleOfType<subType>();
        public T[] Visible() => SpriteManager.VisibleOfType<T>();
        public T[] All() => SpriteManager.OfType<T>();
        public subType[] OfType<subType>() where subType : T => SpriteManager.OfType<subType>();
        public T? FirstByTag(string name) => SpriteManager.OfType<T>().FirstOrDefault(o => o.SpriteTag == name);
        public IEnumerable<T> AllByTag(string name) => SpriteManager.OfType<T>().Where(o => o.SpriteTag == name);

        public virtual void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement) { }

        public VectoredCollidableTickControllerBase(AeEngine engine, SpriteManager manager)
        {
            Engine = engine;
            SpriteManager = manager;
        }

        public void QueueAllForDeletion() => SpriteManager.QueueAllForDeletionOfType<T>();

        #region Tightly-typed Pass through factory methods to SpriteManager.

        public T Create(string assetKey, Action<T>? initializationProc = null)
            => SpriteManager.Create<T>(assetKey, initializationProc);

        public T Add(string assetKey, Action<T>? initializationProc = null)
            => SpriteManager.Add<T>(assetKey, initializationProc);

        public void Insert(T sprite)
             => SpriteManager.Insert(sprite);

        public T Add(SharpDX.Direct2D1.Bitmap bitmap, Action<T>? initializationProc = null)
            => SpriteManager.Add<T>(bitmap, initializationProc);

        #endregion

        public T Create()
        {
            return (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
        }

        public T Create(string bitmapPath)
        {
            return (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
        }
    }
}
