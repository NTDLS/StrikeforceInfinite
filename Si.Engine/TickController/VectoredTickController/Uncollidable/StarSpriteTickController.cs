using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Rendering;
using System;
using System.Collections.Generic;
using System.Threading;
using static Si.Engine.Manager.AssetManager;

namespace Si.Engine.TickController.VectoredTickController.Uncollidable
{
    public class StarSpriteTickController
        : VectoredTickControllerBase<SpriteStar>
    {
        private const int _maxDistance = 1000;
        private readonly Lock _lock = new();

        private List<AssetContainer>? _starAssets = null;
        private List<AssetContainer> StarAssets
        {
            get
            {
                if (_starAssets == null)
                {
                    lock (_lock)
                    {
                        //We lazy load these because the assets arent cached untill initilization.
                        _starAssets ??= Engine.Assets.GetAssetsInPath(@"Sprites\Star");
                    }
                }
                return _starAssets;
            }
        }

        public StarSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public AssetContainer? GetRandomStar()
        {
            if (StarAssets.Count == 0)
            {
                return null;
            }
            var index = SiRandom.Between(0, StarAssets.Count - 1);
            return StarAssets[index];
        }

        public void AddRandomStarAt(SiVector position)
        {
            var randomStarSpritePath = GetRandomStar()?.Key;
            if (randomStarSpritePath != null)
            {
                var starSprite = Engine.Sprites.Add<SpriteStar>(randomStarSpritePath, (o) =>
                {
                    o.Location = position;
                });
            }
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector displacementVector)
        {
            if (Math.Abs(displacementVector.X) > 1 || Math.Abs(displacementVector.Y) > 1)
            {
                #region Add new stars...

                if (SpriteManager.VisibleOfType<SpriteStar>().Length < Engine.Settings.DeltaFrameTargetStarCount) //Never wan't more than n stars.
                {
                    if (displacementVector.X > 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(Engine.Display.TotalCanvasSize.Width - (int)displacementVector.X, Engine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Height);
                            AddRandomStarAt(new SiVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }

                    }
                    else if (displacementVector.X < 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, (int)-displacementVector.X);
                            int y = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Height);
                            AddRandomStarAt(new SiVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }

                    }
                    if (displacementVector.Y > 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(Engine.Display.TotalCanvasSize.Height - (int)displacementVector.Y, Engine.Display.TotalCanvasSize.Height);
                            AddRandomStarAt(new SiVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }
                    }
                    else if (displacementVector.Y < 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(0, (int)-displacementVector.Y);
                            AddRandomStarAt(new SiVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }
                    }
                }

                #endregion

                foreach (var star in All())
                {
                    star.ApplyMotion(epoch, displacementVector);

                    //Remove stars that are too far off-screen.
                    if (Engine.Display.TotalCanvasBounds.Balloon(_maxDistance).IntersectsWith(star.RenderBounds) == false)
                    {
                        star.QueueForDelete();
                    }
                }
            }
        }
    }
}
