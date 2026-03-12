using Ae.Engine.ExtensionMethods;
using Ae.Engine.Helpers;
using Ae.Engine.Mathematics;
using Ae.Engine.Metadata;
using Ae.Engine.Sprite.Base;
using Ae.Engine.Sprite.Interactive;
using Ae.Engine.Sprite.Interactive.Ship;
using System;

namespace Ae.Engine.Sprite.Munition
{
    /// <summary>
    /// The munition base is the base for all bullets/projectiles/etc.
    /// </summary>
    [AssetClass("Munition", "", AeBaseAssetType.Image, true)]
    public class AeSpriteMunition
        : AeSprite
    {
        /// <summary>
        /// Gets the type that triggered the event.
        /// </summary>
        public AeFiredFromType FiredFromType { get; private set; }

        /// <summary>
        /// Gets the weapon associated with the sprite.
        /// </summary>
        public AeSpriteWeapon Weapon { get; private set; }

        /// <summary>
        /// Gets the date and time when the object was created.
        /// </summary>
        public DateTime CreatedUTC { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the duration, in milliseconds, that the object remains active before it is considered expired.
        /// </summary>
        public float MillisecondsToLive { get; set; } = 4000;

        /// <summary>
        /// Gets the age of the object, in milliseconds, since its creation.
        /// </summary>
        public float AgeInMilliseconds => (float)(DateTime.UtcNow - CreatedUTC).TotalMilliseconds;

        /// <summary>
        /// Gets or sets the maximum distance, in units, at which scenes are considered for loading or interaction.
        /// </summary>
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
        public AeSpriteMunition(AeEngine engine, AeSpriteWeapon weapon, AeSpriteInteractive firedFrom, string assetKey, AeVector location, float? angleDegrees = null)
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

            if (firedFrom is AeSpriteAttachment attachment)
            {
                //If we are firing from an attachment, get the type of the root owner.
                firedFrom = attachment.RootOwner;
            }

            if (firedFrom is AeSpriteEnemy)
            {
                FiredFromType = AeFiredFromType.Enemy;
            }
            else if (firedFrom is AeSpritePlayer)
            {
                FiredFromType = AeFiredFromType.Player;
            }
            else
            {
                throw new Exception($"Munitions for {firedFrom.GetType().Name} are not implemented.");
            }
        }

        /// <summary>
        /// Applies intelligence logic to the entity for the specified epoch, potentially triggering state changes based
        /// on its lifetime and camera displacement.
        /// </summary>
        /// <remarks>If the entity's age exceeds its configured lifetime, this method triggers an
        /// explosion and halts further processing for the current epoch.</remarks>
        /// <param name="epoch">The current epoch value representing the simulation or game time at which intelligence logic is applied.</param>
        /// <param name="cameraDisplacement">The displacement vector of the camera, used to inform intelligence decisions based on camera movement or
        /// position.</param>
        public virtual void ApplyIntelligence(float epoch, AeVector cameraDisplacement)
        {
            if (AgeInMilliseconds > MillisecondsToLive)
            {
                Explode();
                return;
            }
        }

        /// <summary>
        /// Applies motion to the object based on the specified time interval and camera displacement.
        /// </summary>
        /// <remarks>If the object is outside the scene distance limit, it will be queued for deletion and
        /// no motion will be applied.</remarks>
        /// <param name="epoch">The time interval, in seconds, over which to apply the motion. Must be positive.</param>
        /// <param name="cameraDisplacement">The displacement vector representing the camera's movement during the epoch. Used to adjust the object's
        /// motion relative to the camera.</param>
        public override void ApplyMotion(float epoch, AeVector cameraDisplacement)
        {
            if (!Engine.Display.TotalCanvasBounds.Balloon(SceneDistanceLimit).IntersectsWith(RenderBounds))
            {
                QueueForDelete();
                return;
            }

            Location += MovementVector * epoch;
        }

        /// <summary>
        /// Triggers the explosion behavior for the object, handling any impact-related effects and marking the object
        /// for deletion.
        /// </summary>
        /// <remarks>If the object is associated with a weapon and is configured to explode on impact, an
        /// explosion effect is executed before the object is queued for deletion. This method is typically called when
        /// the object should be removed from the game world due to an explosion event.</remarks>
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
