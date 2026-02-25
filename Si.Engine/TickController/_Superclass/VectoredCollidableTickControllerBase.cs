using NTDLS.Helpers;
using Si.Engine.Manager;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library;
using Si.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Si.Engine.TickController._Superclass
{
    /// <summary>
    /// Tick managers which update their sprites using the supplied 2D vector.
    /// Also contains various factory methods.
    /// </summary>
    public class VectoredCollidableTickControllerBase<T>
        : ITickController<T> where T : SpriteBase
    {
        public EngineCore Engine { get; private set; }
        public SpriteManager SpriteManager { get; private set; }

        public subType[] VisibleOfType<subType>() where subType : T => SpriteManager.VisibleOfType<subType>();
        public T[] Visible() => SpriteManager.VisibleOfType<T>();
        public T[] All() => SpriteManager.OfType<T>();
        public subType[] OfType<subType>() where subType : T => SpriteManager.OfType<subType>();
        public T? FirstByTag(string name) => SpriteManager.OfType<T>().FirstOrDefault(o => o.SpriteTag == name);
        public IEnumerable<T> AllByTag(string name) => SpriteManager.OfType<T>().Where(o => o.SpriteTag == name);

        public virtual void ExecuteWorldClockTick(float epoch, SiVector displacementVector) { }

        public VectoredCollidableTickControllerBase(EngineCore engine, SpriteManager manager)
        {
            Engine = engine;
            SpriteManager = manager;
        }

        public void QueueAllForDeletion() => SpriteManager.QueueAllForDeletionOfType<T>();

        public void Add(T obj) => SpriteManager.Insert(obj);


        public T Add(string spritePath)
        {
            var metadata = Engine.Assets.GetMetadata(spritePath)
                ?? throw new Exception($"No metadata found for bitmap path: {spritePath}");

            var metadataBaseType = SiReflection.GetTypeByName(metadata.Class);

            var obj = (T)Activator.CreateInstance(metadataBaseType, Engine, spritePath).EnsureNotNull();
            Add(obj);
            return obj;
        }

        public T AddAt(string spritePath, SpriteBase locationOf)
        {
            var obj = Add(spritePath);
            obj.Location = locationOf.Location.Clone();
            Add(obj);
            return obj;
        }

        public T Add(SharpDX.Direct2D1.Bitmap bitmap)
        {
            T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmap).EnsureNotNull();
            SpriteManager.Insert(obj);
            return obj;
        }

        public T AddAt(SharpDX.Direct2D1.Bitmap bitmap, SpriteBase locationOf)
        {
            T obj = Add(bitmap);
            obj.Location = locationOf.Location.Clone();
            return obj;
        }

        public T AddAtCenterUniverse(string spritePath)
        {
            var obj = Add(spritePath);
            obj.Location = new SiVector(Engine.Display.TotalCanvasSize.Width / 2, Engine.Display.TotalCanvasSize.Height / 2);
            Add(obj);
            return obj;
        }

        ///// <summary>
        ///// Adds a sprite with the specified bitmap.
        ///// </summary>
        ///// <param name="bitmapPath"></param>
        ///// <param name="location"></param>
        ///// <returns></returns>
        //public T Add(string bitmapPath, SiVector location)
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
        //    obj.Location = location.Clone();
        //    SpriteManager.Add(obj);
        //    return obj;
        //}

        ///// <summary>
        ///// Adds a sprite with the specified bitmap.
        ///// </summary>
        ///// <param name="bitmapPath"></param>
        ///// <param name="x"></param>
        ///// <param name="y"></param>
        ///// <returns></returns>
        //public T Add(string bitmapPath, float x, float y)
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
        //    obj.X = x;
        //    obj.Y = y;
        //    SpriteManager.Add(obj);
        //    return obj;
        //}

        //public T AddAtCenterScreen()
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
        //    obj.Location = Engine.Display.CenterOfCurrentScreen;
        //    SpriteManager.Add(obj);
        //    return obj;
        //}

        ///// <summary>
        ///// Adds a sprite with the specified bitmap.
        ///// </summary>
        ///// <param name="bitmapPath"></param>
        ///// <returns></returns>
        //public T AddAtCenterScreen(string bitmapPath)
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
        //    obj.Location = Engine.Display.CenterOfCurrentScreen;
        //    SpriteManager.Add(obj);
        //    return obj;
        //}

        //public T AddAtCenterUniverse()
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
        //    obj.X = Engine.Display.TotalCanvasSize.Width / 2;
        //    obj.Y = Engine.Display.TotalCanvasSize.Height / 2;

        //    SpriteManager.Add(obj);
        //    return obj;
        //}

        ///// <summary>
        ///// Adds a sprite with the specified bitmap.
        ///// </summary>
        ///// <param name="bitmapPath"></param>
        ///// <returns></returns>
        //public T AddAtCenterUniverse(string bitmapPath)
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
        //    obj.X = Engine.Display.TotalCanvasSize.Width / 2;
        //    obj.Y = Engine.Display.TotalCanvasSize.Height / 2;

        //    SpriteManager.Add(obj);
        //    return obj;
        //}

        //public T Add(float x, float y)
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
        //    obj.X = x;
        //    obj.Y = y;
        //    SpriteManager.Add(obj);
        //    return obj;
        //}

        //public T Add(SiVector location)
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
        //    obj.Location = location.Clone();
        //    SpriteManager.Add(obj);
        //    return obj;
        //}

        //public T AddAt(SpriteBase locationOf)
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
        //    obj.Location = locationOf.Location.Clone();
        //    SpriteManager.Add(obj);
        //    return obj;
        //}

        //public T AddAt(SiVector location)
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
        //    obj.Location = location.Clone();
        //    SpriteManager.Add(obj);
        //    return obj;
        //}

        //public T Add()
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine).EnsureNotNull();
        //    SpriteManager.Add(obj);
        //    return obj;
        //}

        ///// <summary>
        ///// Adds a sprite with the specified bitmap.
        ///// </summary>
        ///// <param name="bitmapPath"></param>
        ///// <returns></returns>
        //public T Add(string bitmapPath)
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmapPath).EnsureNotNull();
        //    SpriteManager.Add(obj);
        //    return obj;
        //}

        //public T Add(SharpDX.Direct2D1.Bitmap bitmap)
        //{
        //    T obj = (T)Activator.CreateInstance(typeof(T), Engine, bitmap).EnsureNotNull();
        //    SpriteManager.Add(obj);
        //    return obj;
        //}

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
