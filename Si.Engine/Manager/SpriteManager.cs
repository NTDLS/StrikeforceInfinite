﻿using NTDLS.Helpers;
using SharpCompress;
using SharpDX.Mathematics.Interop;
using Si.Engine.Menu;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Player._Superclass;
using Si.Engine.Sprite.PowerUp._Superclass;
using Si.Engine.Sprite.Weapon.Munition._Superclass;
using Si.GameEngine.Sprite.SupportingClasses;
using Si.GameEngine.TickController.UnvectoredTickController;
using Si.GameEngine.TickController.VectoredTickController.Collidable;
using Si.GameEngine.TickController.VectoredTickController.Uncollidable;
using Si.Library;
using Si.Library.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static Si.Library.SiConstants;

namespace Si.Engine.Manager
{
    /// <summary>
    /// Contains the collection of all sprites and their factories. This class stringently controls access to the internal collection
    ///     only allowing insertion and deletions from it to occur within events so that it can be safely assumes that the collection
    ///     can be enumerated in the world clock controllers without fear of collection modification during enumeration.
    /// </summary>
    public class SpriteManager : IDisposable
    {
        public delegate void CollectionAccessor(SpriteBase[] sprites);
        public delegate T CollectionAccessorT<T>(SpriteBase[] sprites);

        private readonly EngineCore _engine;
        private SiVector? _radarScale;
        private SiVector? _radarOffset;

        public bool RenderRadar { get; set; } = false;

        private readonly List<SpriteBase> _collection = new();

        #region Sprite Tick Controllerss.

        public AnimationSpriteTickController Animations { get; private set; }
        public AttachmentSpriteTickController Attachments { get; private set; }
        public InteractiveBitmapSpriteTickController InteractiveBitmaps { get; private set; }
        public MinimalBitmapSpriteTickController GenericBitmaps { get; private set; }
        public MunitionSpriteTickController Munitions { get; private set; }
        public DebugSpriteTickController Debugs { get; private set; }
        public EnemySpriteTickController Enemies { get; private set; }
        public ParticleSpriteTickController Particles { get; private set; }
        public PowerupSpriteTickController Powerups { get; private set; }
        public RadarPositionsSpriteTickController RadarPositions { get; set; }
        public StarSpriteTickController Stars { get; private set; }
        public TextBlocksSpriteTickController TextBlocks { get; private set; }
        public SkyBoxSpriteTickController SkyBoxes { get; private set; }

        #endregion

        public SpriteManager(EngineCore engine)
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

        public SpriteBase[] Visible() => _collection.Where(o => o.Visible == true).ToArray();

        public SpriteBase[] All() => _collection.ToArray();

        public List<SpritePlayerBase> AllVisiblePlayers
        {
            get
            {
                var players = VisibleOfType<SpritePlayerBase>().ToList();
                players.Add(_engine.Player.Sprite);
                return players;
            }
        }

        /// <summary>
        /// This is to be used ONLY for the debugger to access the collection. Otherwise, this class managed all access to the internal collection,
        /// </summary>
        /// <param name="collectionAccessor"></param>
        public void DebugOnlyAccess(CollectionAccessor collectionAccessor)
            => collectionAccessor(All());

        public void QueueAllForDeletionOfType<T>() where T : SpriteBase
            => OfType<T>().ForEach(c => c.QueueForDelete());

        public void Dispose()
        {
        }

        public T CreateByType<T>() where T : SpriteBase
        {
            return (T)Activator.CreateInstance(typeof(T), [_engine]).EnsureNotNull();
        }

        public void Add(SpriteBase item)
        {
            if (_engine.IsInitializing == true)
            {
                //When the engine is initializing, we do all kinds of pre-caching.
                //We want to make sure that none of these new classes make it to the sprite collection.
                return;
            }

            if (item == null)
            {
                throw new Exception("NULL sprites cannot be added to the manager.");
            }
            _engine.Events.Add(() => _collection.Add(item));
        }

        public void HardDelete(SpriteBase item)
        {
            item.Cleanup();
            _collection.Remove(item);
        }

        public void HardDeleteAllQueuedDeletions()
        {
            _collection.Where(o => o.IsQueuedForDeletion).ToList().ForEach(p => p.Cleanup());
            _collection.RemoveAll(o => o.IsQueuedForDeletion);

            _engine.Events.CleanupQueuedForDeletion();

            if (_engine.Player.Sprite.IsDeadOrExploded)
            {
                _engine.Player.Sprite.Visible = false;
                _engine.Player.Sprite.ReviveDeadOrExploded();
                _engine.Menus.Show(new MenuStartNewGame(_engine));
            }
        }

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

        public T[]? GetSpritesByTag<T>(string name) where T : SpriteBase
            => _collection.Where(o => o.SpriteTag == name).ToArray() as T[];

        public T? GetSingleSpriteByTag<T>(string name) where T : SpriteBase
            => _collection.Where(o => o.SpriteTag == name).SingleOrDefault() as T;

        public T? GetSpriteByOwner<T>(uint ownerUID) where T : SpriteBase
            => _collection.Where(o => o.UID == ownerUID).SingleOrDefault() as T;

        public T[] OfType<T>() where T : SpriteBase
            => _collection.OfType<T>().ToArray();

        public T[] VisibleOfType<T>() where T : SpriteBase
            => _collection.OfType<T>().Where(o => o.Visible).ToArray();

        public T?[] VisibleDamageable<T>() where T : class
            => _collection.OfType<SpriteInteractiveBase>().Where(o => o.Visible && o.Metadata.MunitionDetection).Select(o => o as T).ToArray();

        //Probably faster than VisibleDamageable<T>().
        public SpriteInteractiveBase[] VisibleDamageable()
            => _collection.OfType<SpriteInteractiveBase>().Where(o => o.Visible && o.Metadata.MunitionDetection == true).ToArray();

        public T?[] VisibleCollidable<T>() where T : class
            => _collection.OfType<SpriteInteractiveBase>().Where(o => o.Visible && o.Metadata.CollisionDetection).Select(o => o as T).ToArray();

        //Probably faster than VisibleCollidable<T>().
        public SpriteInteractiveBase[] VisibleCollidable()
            => _collection.OfType<SpriteInteractiveBase>().Where(o => o.Visible && o.Metadata.CollisionDetection == true).ToArray();

        public PredictedKinematicBody[] VisibleCollidablePredictiveMove(float epoch)
            => _engine.Sprites.VisibleCollidable().Select(o => new PredictedKinematicBody(o, _engine.Display.RenderWindowPosition, epoch)).ToArray();

        public SpriteBase[] VisibleOfTypes(Type[] types)
        {
            var result = new List<SpriteBase>();
            foreach (var type in types)
            {
                result.AddRange(_collection.Where(o => o.Visible == true && type.IsAssignableFrom(o.GetType())));
            }

            return result.ToArray();
        }

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

        public SpriteBase[] Intersections(SpriteBase with)
        {
            var objects = new List<SpriteBase>();

            foreach (var obj in _collection.Where(o => o.Visible == true))
            {
                if (obj != with)
                {
                    if (obj.IntersectsAABB(with.Location, new SiVector(with.Size.Width, with.Size.Height)))
                    {
                        objects.Add(obj);
                    }
                }
            }
            return objects.ToArray();
        }

        public SpriteBase[] Intersections(float x, float y, float width, float height)
            => Intersections(new SiVector(x, y), new SiVector(width, height));

        public SpriteBase[] Intersections(SiVector location, SiVector size)
        {
            var objects = new List<SpriteBase>();

            foreach (var obj in _collection.Where(o => o.Visible == true))
            {
                if (obj.IntersectsAABB(location, size))
                {
                    objects.Add(obj);
                }
            }
            return objects.ToArray();
        }

        public SpriteBase[] RenderLocationIntersectionsEvenInvisible(SiVector location, SiVector size)
        {
            var objects = new List<SpriteBase>();

            foreach (var obj in _collection)
            {
                if (obj.RenderLocationIntersectsAABB(location, size))
                {
                    objects.Add(obj);
                }
            }
            return objects.ToArray();
        }

        public SpriteBase[] RenderLocationIntersections(SiVector location, SiVector size)
        {
            var objects = new List<SpriteBase>();

            foreach (var obj in _collection.Where(o => o.Visible == true))
            {
                if (obj.RenderLocationIntersectsAABB(location, size))
                {
                    objects.Add(obj);
                }
            }
            return objects.ToArray();
        }

        public SpritePlayerBase AddPlayer(SpritePlayerBase sprite)
        {
            Add(sprite);
            return sprite;
        }

        public void RenderPostScaling(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            foreach (var sprite in _collection.Where(o => o.Visible == true && o.RenderScaleOrder == SiRenderScaleOrder.PostScale).OrderBy(o => o.Z))
            {
                sprite.Render(renderTarget);
            }

            if (RenderRadar)
            {
                var radarBgImage = _engine.Assets.GetBitmap(@"Sprites\RadarTransparent.png");

                _engine.Rendering.DrawBitmap(renderTarget, radarBgImage,
                    _engine.Display.NaturalScreenSize.Width - radarBgImage.Size.Width,
                    _engine.Display.NaturalScreenSize.Height - radarBgImage.Size.Height, 0);

                float radarDistance = 8;

                if (_radarScale == null)
                {
                    float radarVisionWidth = _engine.Display.TotalCanvasSize.Width * radarDistance;
                    float radarVisionHeight = _engine.Display.TotalCanvasSize.Height * radarDistance;

                    _radarScale = new SiVector(radarBgImage.Size.Width / radarVisionWidth, radarBgImage.Size.Height / radarVisionHeight);
                    _radarOffset = new SiVector(radarBgImage.Size.Width / 2.0f, radarBgImage.Size.Height / 2.0f); //Best guess until player is visible.
                }

                if (_engine.Player.Sprite is not null && _engine.Player.Sprite.Visible)
                {
                    float centerOfRadarX = (int)(radarBgImage.Size.Width / 2.0f) - 2.0f; //Subtract half the dot size.
                    float centerOfRadarY = (int)(radarBgImage.Size.Height / 2.0f) - 2.0f; //Subtract half the dot size.

                    _radarOffset = new SiVector(
                            _engine.Display.NaturalScreenSize.Width - radarBgImage.Size.Width + (centerOfRadarX - _engine.Player.Sprite.X * _radarScale.X),
                            _engine.Display.NaturalScreenSize.Height - radarBgImage.Size.Height + (centerOfRadarY - _engine.Player.Sprite.Y * _radarScale.Y)
                        );

                    //Render radar:
                    foreach (var sprite in _collection.Where(o => o.Visible == true))
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
                            if ((sprite is SpritePlayerBase || sprite is SpriteEnemyBase || sprite is MunitionBase || sprite is SpritePowerupBase) && sprite.Visible == true)
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
        public void RenderPreScaling(SharpDX.Direct2D1.RenderTarget renderTarget)
        {
            foreach (var sprite in _collection.Where(o => o.Visible == true && o.RenderScaleOrder == SiRenderScaleOrder.PreScale).OrderBy(o => o.Z))
            {
                if (sprite.IsWithinCurrentScaledScreenBounds)
                {
                    sprite.Render(renderTarget);
                }
            }

            _engine.Player.Sprite?.Render(renderTarget);
            _engine.Menus.Render(renderTarget);

            if (_engine.Settings.HighlightNaturalBounds)
            {
                var naturalScreenBounds = _engine.Display.NaturalScreenBounds;
                var rawRectF = new RawRectangleF(naturalScreenBounds.Left, naturalScreenBounds.Top, naturalScreenBounds.Right, naturalScreenBounds.Bottom);

                //Highlight the 1:1 frame
                _engine.Rendering.DrawRectangle(renderTarget, rawRectF, _engine.Rendering.Materials.Colors.Red, 0, 1, 0);
            }
        }

        public void CreateFragmentsOf(SpriteBase sprite)
        {
            var image = sprite.GetImage();
            if (image == null)
            {
                return;
            }

            var fragmentImages = _engine.Rendering.GenerateIrregularFragments(image);

            foreach (var fragmentImage in fragmentImages)
            {
                var fragment = _engine.Sprites.GenericBitmaps.AddAt(fragmentImage, sprite);
                fragment.CleanupMode = ParticleCleanupMode.DistanceOffScreen;
                fragment.FadeToBlackReductionAmount = SiRandom.Between(0.001f, 0.01f); //TODO: Can we implement this?
                fragment.RotationSpeed = SiRandom.FlipCoin() ? SiRandom.Between(-0.05f, -0.02f) : SiRandom.Between(0.02f, 0.05f);
                fragment.VectorType = ParticleVectorType.Default;

                fragment.Orientation.Degrees = SiRandom.Between(0.0f, 359.0f);
                fragment.Speed = SiRandom.Between(1, 3.5f);
                fragment.Throttle = 1;
                fragment.RecalculateMovementVector();
            }
        }

        public void HydrateCache(SpriteTextBlock loadingHeader, SpriteTextBlock loadingDetail)
        {
            float statusIndex = 0;
            loadingHeader.SetTextAndCenterX("Hydrating sprite cache...");

            var assembly = Assembly.GetExecutingAssembly();
            var baseType = typeof(SpriteBase);
            var derivedTypes = new List<Type>();

            var allTypes = assembly.GetTypes();

            foreach (var type in allTypes)
            {
                loadingDetail.SetTextAndCenterX($"{statusIndex++ / allTypes.Length * 100.0:n0}%");

                if (baseType.IsAssignableFrom(type) && type != baseType)
                {
                    // Check if the constructor parameter is of type EngineCore
                    var constructor = type.GetConstructor([typeof(EngineCore)]);
                    if (constructor != null)
                    {
                        derivedTypes.Add(type);
                    }
                }
            }

            statusIndex = 0;
            loadingHeader.SetTextAndCenterX("Hydrating animation cache...");

            // Create instances of derived types
            foreach (var type in derivedTypes)
            {
                //Creating the instance of the sprite loads and caches the metadata and images.
                dynamic instance = Activator.CreateInstance(type, _engine).EnsureNotNull();

                loadingDetail.SetTextAndCenterX($"{statusIndex++ / derivedTypes.Count * 100.0:n0}%");
                instance.QueueForDelete();
            }

            //Pre-cache animations:
            //Animations do not have their own classes, so we need to look for them in the assets and load them.
            var animations = _engine.Assets.Entries.Select(o => o.Value.Key.EnsureNotNull())
                .Where(o => o != null && o.StartsWith(@"sprites/animation/", StringComparison.CurrentCultureIgnoreCase)
                && o.EndsWith(@".png", StringComparison.CurrentCultureIgnoreCase)).ToList();

            animations.ForEach(o => Animations.Add(o).QueueForDelete());
        }
    }
}
