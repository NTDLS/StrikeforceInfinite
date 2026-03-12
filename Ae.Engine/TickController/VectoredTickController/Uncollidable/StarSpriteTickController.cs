using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite;
using System;
using System.Threading;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    /// <summary>
    /// Controls the tick-based behavior and lifecycle of star sprites within the game world.
    /// </summary>
    /// <remarks>This controller manages the addition, motion, and removal of star sprites in response to
    /// camera movement and world clock ticks. It ensures that the number of visible stars remains within the target
    /// count specified by engine settings, and automatically removes stars that move too far off-screen. Use this class
    /// to integrate star sprite management into the game's update loop.</remarks>
    public class StarSpriteTickController
        : VectoredTickControllerBase<AeSpriteStar>
    {
        private const int _maxDistance = 1000;
        private readonly Lock _lock = new();

        /// <summary>
        /// Initializes a new instance of the StarSpriteTickController class using the specified engine and sprite
        /// manager.
        /// </summary>
        /// <param name="engine">The engine instance that provides core functionality and services for sprite operations. Cannot be null.</param>
        /// <param name="manager">The sprite manager responsible for managing sprite objects within the engine. Cannot be null.</param>
        public StarSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        /// <summary>
        /// Adds a randomly selected star sprite at the specified position.
        /// </summary>
        /// <param name="position">The location where the star sprite will be placed. Must be a valid vector representing a position in the
        /// scene.</param>
        public void AddRandomStarAt(AeVector position)
        {
            var assetKeys = Engine.Assets.GetAssetKeysInPath("Sprites/Star");

            Engine.Sprites.Add<AeSpriteStar>(assetKeys.OneOf(), (sprite) =>
            {
                sprite.Location = position;
            });
        }

        /// <summary>
        /// Updates the state of stars in the world based on the current epoch and camera displacement. Adds new stars
        /// and removes those that move too far off-screen as the camera moves.
        /// </summary>
        /// <remarks>This method manages the dynamic addition and removal of stars to maintain visual
        /// consistency as the camera moves. Stars are only added if the camera displacement exceeds a threshold and the
        /// current star count is below the target. Stars outside the visible bounds are queued for deletion.</remarks>
        /// <param name="epoch">The current time value, typically representing the simulation or world clock tick, used to update star
        /// motion.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera since the last tick. Determines how stars are added or removed based
        /// on camera movement.</param>
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
