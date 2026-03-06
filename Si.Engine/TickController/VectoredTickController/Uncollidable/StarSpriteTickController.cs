using Si.Engine.Manager;
using Si.Engine.Sprite._Superclass;
using Si.Engine.TickController._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Rendering;
using System;
using System.Threading;

namespace Si.Engine.TickController.VectoredTickController.Uncollidable
{
    public class StarSpriteTickController
        : VectoredTickControllerBase<SpriteStar>
    {
        private const int _maxDistance = 1000;
        private readonly Lock _lock = new();


        public StarSpriteTickController(EngineCore engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public void AddRandomStarAt(SiVector position)
        {
            var assetKeys = Engine.Assets.GetAssetKeysInPath("Sprites/Star");

            Engine.Sprites.Add<SpriteStar>(SiRandom.OneOf(assetKeys), (sprite) =>
            {
                sprite.Location = position;
            });
        }

        public override void ExecuteWorldClockTick(float epoch, SiVector cameraDisplacement)
        {
            if (Math.Abs(cameraDisplacement.X) > 1 || Math.Abs(cameraDisplacement.Y) > 1)
            {
                #region Add new stars...

                if (SpriteManager.VisibleOfType<SpriteStar>().Length < Engine.Settings.DeltaFrameTargetStarCount) //Never wan't more than n stars.
                {
                    if (cameraDisplacement.X > 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(Engine.Display.TotalCanvasSize.Width - (int)cameraDisplacement.X, Engine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Height);
                            AddRandomStarAt(new SiVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }

                    }
                    else if (cameraDisplacement.X < 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, (int)-cameraDisplacement.X);
                            int y = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Height);
                            AddRandomStarAt(new SiVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }

                    }
                    if (cameraDisplacement.Y > 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(Engine.Display.TotalCanvasSize.Height - (int)cameraDisplacement.Y, Engine.Display.TotalCanvasSize.Height);
                            AddRandomStarAt(new SiVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }
                    }
                    else if (cameraDisplacement.Y < 0)
                    {
                        if (SiRandom.PercentChance(20))
                        {
                            int x = SiRandom.Between(0, Engine.Display.TotalCanvasSize.Width);
                            int y = SiRandom.Between(0, (int)-cameraDisplacement.Y);
                            AddRandomStarAt(new SiVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }
                    }
                }

                #endregion

                foreach (var star in All())
                {
                    star.ApplyMotion(epoch, cameraDisplacement);

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
