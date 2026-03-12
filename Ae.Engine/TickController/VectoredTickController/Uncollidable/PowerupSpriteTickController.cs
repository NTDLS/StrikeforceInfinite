using Ae.Engine.Manager;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite;
using NTDLS.Helpers;
using System;

namespace Ae.Engine.TickController.VectoredTickController.Uncollidable
{
    /// <summary>
    /// Controls the tick-based updates for power-up sprites within the game world, managing their intelligence, motion,
    /// and multiplayer synchronization.
    /// </summary>
    /// <remarks>This controller is responsible for updating all visible power-up sprites each world clock
    /// tick. It applies AI and movement logic, and records multiplayer actions if a lobby is active. Use this class to
    /// add new power-up sprites at specific locations and to ensure their state is updated consistently during
    /// gameplay.</remarks>
    public class PowerupSpriteTickController
        : VectoredTickControllerBase<AeSpritePowerup>
    {
        /// <summary>
        /// Initializes a new instance of the PowerupSpriteTickController class to manage power-up sprite updates within
        /// the specified engine and sprite manager.
        /// </summary>
        /// <param name="engine">The game engine instance used to coordinate sprite updates and game logic. Cannot be null.</param>
        /// <param name="manager">The sprite manager responsible for handling power-up sprites. Cannot be null.</param>
        public PowerupSpriteTickController(AeEngine engine, SpriteManager manager)
            : base(engine, manager)
        {
        }

        /// <summary>
        /// Updates all visible sprites for the current world clock tick, applying intelligence and motion, and records
        /// multiplayer motion actions.
        /// </summary>
        /// <remarks>This method processes each visible sprite by applying intelligence and motion updates
        /// based on the provided epoch and camera displacement. If multiplayer functionality is enabled, motion actions
        /// are recorded for synchronization.</remarks>
        /// <param name="epoch">The current time value, in seconds, representing the world clock tick to process.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera, used to adjust sprite behavior and motion during the tick.</param>
        public override void ExecuteWorldClockTick(float epoch, AeVector cameraDisplacement)
        {
            foreach (var sprite in Visible())
            {
                sprite.ApplyIntelligence(epoch, cameraDisplacement);
                sprite.ApplyMotion(epoch, cameraDisplacement);

                Engine.MultiplayLobby?.ActionBuffer.RecordMotion(sprite.GetMultiPlayActionVector());
            }
        }

        /// <summary>
        /// Creates and adds a new powerup sprite of the specified type at the given coordinates.
        /// </summary>
        /// <remarks>The created sprite is immediately inserted into the SpriteManager. Use this method to
        /// spawn powerups dynamically at specific locations.</remarks>
        /// <typeparam name="T">The type of powerup sprite to create. Must inherit from AeSpritePowerup.</typeparam>
        /// <param name="x">The X coordinate where the powerup sprite will be placed.</param>
        /// <param name="y">The Y coordinate where the powerup sprite will be placed.</param>
        /// <returns>The newly created powerup sprite instance of type T, positioned at the specified location.</returns>
        public T AddAt<T>(float x, float y) where T : AeSpritePowerup
        {
            object[] param = { Engine };
            var obj = (AeSpritePowerup)Activator.CreateInstance(typeof(T), param).EnsureNotNull();
            obj.Location = new AeVector(x, y);
            SpriteManager.Insert(obj);
            return (T)obj;
        }
    }
}
