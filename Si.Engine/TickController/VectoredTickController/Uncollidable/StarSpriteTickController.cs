using Si.Engine.Manager;
using Si.Engine.Sprite;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Rendering;
using System;
using System.Collections.Generic;
using static Si.Engine.Manager.AssetManager;

namespace Si.Engine.TickController.VectoredTickController.Uncollidable
{
    public class StarSpriteTickController
        : VectoredTickControllerBase<SpriteStar>
    {
        List<MetadataContainer> _starAssets = new();

        private List<MetadataContainer> StarAssets
        {
            get
            {
                if (_starAssets.Count == 0)
                {
                    lock (this)
                    {
                        if (_starAssets.Count == 0)
                            _starAssets = Engine.Assets.GetMetadataInDirectory(@"Sprites\Star");
                    }
                }
                return _starAssets;
            }
        }

        public StarSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public MetadataContainer? GetRandomStar()
        {
            if (StarAssets.Count == 0)
            {
                return null;
            }
            var index = SiRandom.Between(0, StarAssets.Count - 1);
            return StarAssets[index];
        }

        public void AddAt(SiVector position)
        {
            var randomStarSpritePath = GetRandomStar()?.Asset.SpritePath;
            if (randomStarSpritePath != null)
            {
                var starSprite = Engine.Sprites.Add<SpriteStar>(randomStarSpritePath);
                starSprite.Location = position;
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
                            AddAt(new SiVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }

                    }
                    else if (displacementVector.X < 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, (int)-displacementVector.X);
                            int y = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Height);
                            AddAt(new SiVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }

                    }
                    if (displacementVector.Y > 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(Engine.Display.TotalCanvasSize.Height - (int)displacementVector.Y, Engine.Display.TotalCanvasSize.Height);
                            AddAt(new SiVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }
                    }
                    else if (displacementVector.Y < 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(0, (int)-displacementVector.Y);
                            AddAt(new SiVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }
                    }
                }

                #endregion

                foreach (var star in All())
                {
                    star.ApplyMotion(epoch, displacementVector);

                    //Remove stars that are too far off-screen.
                    if (Engine.Display.TotalCanvasBounds.Balloon(1000).IntersectsWith(star.RenderBounds) == false)
                    {
                        star.QueueForDelete();
                    }
                }
            }
        }
    }
}
