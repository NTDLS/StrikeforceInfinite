using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;
using Ae.Engine.Sprite.Interactive;
using Ae.Engine.Sprite.Interactive.Ship;
using System;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Sprite.Munition
{
    /// <summary>
    /// The munition base is the base for all bullets/projectiles/etc.
    /// </summary>
    [AssetClass("Munition", "", AeBaseAssetType.Image, true)]
    public class SpriteMunition
        : SpriteBase
    {
        public SiFiredFromType FiredFromType { get; private set; }
        public SpriteWeapon Weapon { get; private set; }
        public DateTime CreatedDate { get; private set; } = DateTime.UtcNow;
        public float MillisecondsToLive { get; set; } = 4000;
        public float AgeInMilliseconds => (float)(DateTime.UtcNow - CreatedDate).TotalMilliseconds;
        public float SceneDistanceLimit { get; set; }

        /// <summary>
        /// Creates a munition for the given weapon.
        /// </summary>
        /// <param name="engine">Reference to the engine.</param>
        /// <param name="weapon">The weapon to create a munition for.</param>
        /// <param name="firedFrom">The sprite that is firing the weapon.</param>
        /// <param name="assetKey">The image for the munition.</param>
        /// <param name="location">The optional location for the munition to originate from (if not specified, we'll use the location of the firedFrom sprite).</param>
        /// <param name="angleDegrees">>The optional angle for the munition to travel on (if not specified, we'll use the angle of the firedFrom sprite).</param>
        public SpriteMunition(AeEngine engine, SpriteWeapon weapon, SpriteInteractive firedFrom, string assetKey, AeVector location, float? angleDegrees = null)
            : base(engine, assetKey)
        {
            Weapon = weapon;
            RadarDotSize = new AeVector(1, 1);
            SceneDistanceLimit = Engine.Settings.MunitionSceneDistanceLimit;

            float headingRadians = angleDegrees == null ? firedFrom.Orientation.RadiansSigned : AeMath.DegToRad(angleDegrees.Value);
            if (Metadata.AngleVarianceDegrees > 0)
            {
                var variance = AeMath.DegToRad(AeRandom.Between(0, Metadata.AngleVarianceDegrees.Value));
                headingRadians += (AeRandom.FlipCoin() ? 1 : -1) * variance;
            }

            Location = location;
            Orientation = new AeVector(headingRadians);
            Speed = AeRandom.Between(Metadata.Speed, 0);
            RecalculateMovementVectorFromOrientation();

            if (firedFrom is SpriteAttachment attachment)
            {
                //If we are firing from an attachment, get the type of the root owner.
                firedFrom = attachment.RootOwner;
            }

            if (firedFrom is SpriteEnemy)
            {
                FiredFromType = SiFiredFromType.Enemy;
            }
            else if (firedFrom is SpritePlayer)
            {
                FiredFromType = SiFiredFromType.Player;
            }
            else
            {
                throw new Exception($"Munitions for {firedFrom.GetType().Name} are not implemented.");
            }
        }

        public virtual void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
        {
            if (AgeInMilliseconds > MillisecondsToLive)
            {
                Explode();
                return;
            }
        }

        public override void ApplyMotion(float epoch, AeVector cameraDisplacement)
        {
            if (!Engine.Display.TotalCanvasBounds.Balloon(SceneDistanceLimit).IntersectsWith(RenderBounds))
            {
                QueueForDelete();
                return;
            }

            Location += MovementVector * epoch;
        }

        public override void Explode()
        {
            if (Weapon != null && Metadata.ExplodesOnImpact == true)
            {
                HitExplosion();
            }
            QueueForDelete();
        }
    }
}
