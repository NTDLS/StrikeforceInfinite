using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Engine.Sprite.Enemy._Superclass;
using Si.Engine.Sprite.Weapon._Superclass;
using Si.Library;
using Si.Library.ExtensionMethods;
using Si.Library.Mathematics;
using Si.Rendering;
using System;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.Weapon.Munition._Superclass
{
    /// <summary>
    /// The munition base is the base for all bullets/projectiles/etc.
    /// </summary>
    public class MunitionBase
        : SpriteBase
    {
        public SiFiredFromType FiredFromType { get; private set; }
        public WeaponBase Weapon { get; private set; }
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
        public MunitionBase(EngineCore engine, WeaponBase weapon, SpriteInteractiveBase firedFrom, string assetKey, SiVector location, float? angleDegrees = null)
            : base(engine, assetKey)
        {
            Weapon = weapon;
            RadarDotSize = new SiVector(1, 1);
            SceneDistanceLimit = _engine.Settings.MunitionSceneDistanceLimit;

            float headingRadians = angleDegrees == null ? firedFrom.Orientation.RadiansSigned : SiMath.DegToRad(angleDegrees.Value);
            if (Metadata.AngleVarianceDegrees > 0)
            {
                var variance = SiMath.DegToRad(SiRandom.Between(0, Metadata.AngleVarianceDegrees.Value));
                headingRadians += (SiRandom.FlipCoin() ? 1 : -1) * variance;
            }

            Location = location;
            Orientation = new SiVector(headingRadians);
            Speed = SiRandom.Between(Metadata.Speed, 0);
            RecalculateMovementVectorFromOrientation();

            if (firedFrom is SpriteAttachment attachment)
            {
                //If we are firing from an attachment, get the type of the root owner.
                firedFrom = attachment.RootOwner;
            }

            if (firedFrom is SpriteEnemyBase)
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

        public virtual void ApplyIntelligence(float epoch, SiVector displacementVector)
        {
            if (AgeInMilliseconds > MillisecondsToLive)
            {
                Explode();
                return;
            }
        }

        public override void ApplyMotion(float epoch, SiVector displacementVector)
        {
            if (!_engine.Display.TotalCanvasBounds.Balloon(SceneDistanceLimit).IntersectsWith(RenderBounds))
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
