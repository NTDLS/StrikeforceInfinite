using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;
using System;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite
{
    /// <summary>
    /// Represents a "power-up" that the player can pick up to gain some ability / stat-improvement.
    /// </summary>
    [AssetClass("Powerup", "", AeBaseAssetType.Image, true)]
    public class SpritePowerup
        : SpriteBase
    {
        /// <summary>
        /// The power up amount (number of boost points, shield points, repair, etc.).
        /// </summary>
        public int PowerupAmount { get; set; } = 1;

        /// <summary>
        /// Time until the powerup exploded on its own.
        /// </summary>
        public float TimeToLive { get; set; } = 30000;
        public DateTime CreationTime { get; set; } = DateTime.UtcNow;
        public float AgeInMilliseconds
        {
            get
            {
                return (float)(DateTime.UtcNow - CreationTime).TotalMilliseconds;
            }
        }

        public SpritePowerup(AeEngine engine, string assetKey)
            : base(engine, assetKey)
        {
            RadarDotSize = new AeVector(4, 4);
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public override void Explode()
        {
            Engine.Assets.GetAudio("Sounds/Powerup/PowerUp1").Play();
            QueueForDelete();
        }

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
