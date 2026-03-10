using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite;
using System;
using System.Threading;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    public class StarSpriteTickController
        : VectoredTickControllerBase<AeSpriteStar>
    {
        private const int _maxDistance = 1000;
        private readonly Lock _lock = new();


        public StarSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        public void AddRandomStarAt(AeVector position)
        {
            var assetKeys = Engine.Assets.GetAssetKeysInPath("Sprites/Star");

            Engine.Sprites.Add<AeSpriteStar>(assetKeys.OneOf(), (sprite) =>
            {
                sprite.Location = position;
            });
        }

        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            if (Math.Abs(cameraDisplacement.X) > 1 || Math.Abs(cameraDisplacement.Y) > 1)
            {
                #region Add new stars...

                if (SpriteManager.VisibleOfType<AeSpriteStar>().Length < Engine.Settings.DeltaFrameTargetStarCount) //Never wan't more than n stars.
                {
                    if (cameraDisplacement.X > 0)
                    {
                        if (AeRandom.PercentChance(20))
                        {
                            int x = AeRandom.Between(Engine.Display.TotalCanvasSize.Width - (int)cameraDisplacement.X, Engine.Display.TotalCanvasSize.Width);
                            int y = AeRandom.Between(0, Engine.Display.TotalCanvasSize.Height);
                            AddRandomStarAt(new AeVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }

                    }
                    else if (cameraDisplacement.X < 0)
                    {
                        if (AeRandom.PercentChance(20))
                        {
                            int x = AeRandom.Between(0, (int)-cameraDisplacement.X);
                            int y = AeRandom.Between(0, Engine.Display.TotalCanvasSize.Height);
                            AddRandomStarAt(new AeVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }

                    }
                    if (cameraDisplacement.Y > 0)
                    {
                        if (AeRandom.PercentChance(20))
                        {
                            int x = AeRandom.Between(0, Engine.Display.TotalCanvasSize.Width);
                            int y = AeRandom.Between(Engine.Display.TotalCanvasSize.Height - (int)cameraDisplacement.Y, Engine.Display.TotalCanvasSize.Height);
                            AddRandomStarAt(new AeVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
                        }
                    }
                    else if (cameraDisplacement.Y < 0)
                    {
                        if (AeRandom.PercentChance(20))
                        {
                            int x = AeRandom.Between(0, Engine.Display.TotalCanvasSize.Width);
                            int y = AeRandom.Between(0, (int)-cameraDisplacement.Y);
                            AddRandomStarAt(new AeVector(Engine.Display.CameraPosition.X + x, Engine.Display.CameraPosition.Y + y));
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
