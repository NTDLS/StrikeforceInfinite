using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;
using System;

namespace Ae.Engine.Sprite
{
    /// <summary>
    /// Represents a "power-up" that the player can pick up to gain some ability / stat-improvement.
    /// </summary>
    [AssetClass("Powerup", "", AeBaseAssetType.Image, true)]
    public class AeSpritePowerup
        : AeSprite
    {
        /// <summary>
        /// The power up amount (number of boost points, shield points, repair, etc.).
        /// </summary>
        public int PowerupAmount { get; set; } = 1;

        /// <summary>
        /// Time until the powerup exploded on its own.
        /// </summary>
        public float TimeToLive { get; set; } = 30000;

        /// <summary>
        /// Gets or sets the UTC date and time when the object was created.
        /// </summary>
        public DateTime CreationUTC { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets the age of the object, in milliseconds, since its creation in Coordinated Universal Time (UTC).
        /// </summary>
        public float AgeInMilliseconds
        {
            get
            {
                return (float)(DateTime.UtcNow - CreationUTC).TotalMilliseconds;
            }
        }

        /// <summary>
        /// Initializes a new instance of the AeSpritePowerup class using the specified engine and asset key.
        /// </summary>
        /// <param name="engine">The engine instance used to manage and render the sprite powerup.</param>
        /// <param name="assetKey">The key identifying the asset to be used for the sprite powerup.</param>
        public AeSpritePowerup(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            RadarDotSize = new AeVector(4, 4);
        }

        /// <summary>
        /// Performs cleanup operations for the derived class instance.
        /// </summary>
        /// <remarks>Overrides the base class cleanup method to release resources or reset state as
        /// needed. Call this method when the instance is no longer required to ensure proper resource
        /// management.</remarks>
        public override void Cleanup()
        {
            base.Cleanup();
        }

        /// <summary>
        /// Triggers the explosion effect for the object, including playing the associated audio and marking the object
        /// for deletion.
        /// </summary>
        /// <remarks>This method is typically called when the object is destroyed or needs to be removed
        /// from the game. The explosion effect includes playing a power-up sound and scheduling the object for removal.
        /// Ensure that the object is in a valid state before calling this method, as it will no longer be available
        /// after execution.</remarks>
        public override void Explode()
        {
            Engine.Assets.GetAudio("Sounds/Powerup/PowerUp1").Play();
            QueueForDelete();
        }

        /// <summary>
        /// Applies intelligence logic to the object for the current simulation epoch, determining whether it should
        /// trigger an explosion based on its state and position.
        /// </summary>
        /// <remarks>This method evaluates the object's state and position to decide if it should explode.
        /// It is typically called once per simulation update.</remarks>
        /// <param name="epoch">The current simulation epoch, typically representing the elapsed time or frame in the simulation.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera, used to adjust object behavior relative to camera movement.</param>
        public virtual void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
        {
            if (IntersectsAABB(Engine.Player.Sprite))
            {
                Explode();
            }
            else if (AgeInMilliseconds > TimeToLive)
            {
                Explode();
            }
        }
    }
}
