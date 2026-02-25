using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Si.Library;
using Si.Library.Mathematics;
using System.Collections.Generic;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.SupportingClasses
{
    public class SpriteMetadata
    {
        public string @Class { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        #region InteractiveSpriteAttachmentMetadata

        public string Type { get; set; } = string.Empty;
        public float X { get; set; }
        public float Y { get; set; }

        [JsonIgnore]
        public SiVector LocationRelativeToOwner { get => new SiVector(X, Y); }

        #endregion

        #region InteractiveSpriteMetadata

        public AttachmentOrientationType OrientationType { get; set; } = AttachmentOrientationType.FixedToOwner;
        public AttachmentPositionType PositionType { get; set; } = AttachmentPositionType.FixedToOwner;

        public ExplosionType ExplosionType { get; set; } = ExplosionType.MediumFire;
        public Range<int> ParticleBlastOnExplodeAmount { get; set; } = new Range<int>(200, 800);
        public bool FragmentOnExplode { get; set; } = true;
        public Range<int> ScreenShakeOnExplodeAmount { get; set; } = new Range<int>(5, 500);

        public float Speed { get; set; } = 1f;
        public float MaxThrottle { get; set; } = 1.0f;
        public float Throttle { get; set; } = 1.0f;

        /// <summary>
        /// How much does the sprite weigh? Used in physics calculations when MunitionDetection == true.
        /// A higher mass will make the sprite more resistant to acceleration and deceleration,
        /// while a lower mass will make it more agile but also more susceptible to external forces.
        /// 
        /// Mass == 0 is infinite mass, it cannot be moved by ineterial forces.
        /// </summary>
        public float Mass { get; set; } = 0f;

        /// <summary>
        /// How many hit points does the sprite have? When this reaches 0, the sprite is destroyed.
        /// </summary>
        public int Hull { get; set; } = 0;

        /// <summary>
        /// Gets or sets the current shield strength of the entity, representing its defensive capabilities.
        /// </summary>
        public int Shields { get; set; } = 0;

        /// <summary>
        /// Gets or sets the bounty amount associated with the entity. This probably will not survive the early access period.
        /// </summary>
        public int Bounty { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether munition detection is enabled (bullet detection).
        /// </summary>
        public bool MunitionDetection { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether collision detection is enabled.
        /// </summary>
        /// <remarks>When set to <see langword="true"/>, the system checks for collisions between objects.
        /// Enabling collision detection may impact performance depending on the number of objects being
        /// monitored.</remarks>
        public bool CollisionDetection { get; set; } = false;

        /// <summary>
        /// Gets or sets the augmentation factor applied to the collision polygon, affecting its size and shape during collision detection.
        /// </summary>
        /// <remarks>Adjust this value to fine-tune the collision area for the associated sprite.
        /// Increasing the factor enlarges the collision polygon, while decreasing it reduces the area considered for
        /// collisions. This can be useful for accommodating visual effects or gameplay balancing.</remarks>
        public float CollisionPolyAugmentation { get; set; } = 1;

        /// <summary>
        /// Used for the players "primary weapon slot".
        /// </summary>
        public SpriteMetadata? PrimaryWeapon { get; set; }
        public List<SpriteMetadata> Attachments { get; set; } = new();
        public List<SpriteMetadata> Weapons { get; set; } = new();

        #endregion

        #region InteractiveSpriteWeaponMetadata

        public int MunitionCount { get; set; }

        #endregion

        #region SpriteAnimationMetadata

        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public float FramesPerSecond { get; set; }

        public SiAnimationPlayMode PlayMode { get; set; }

        #endregion

        #region WeaponMetadata

        /// <summary>
        /// If the sprite has an image, these are the paths to the bitmaps (be default, they are used at random)..
        /// </summary>
        public string[] SpritePaths { get; set; } = new string[0];

        public string? SoundPath { get; set; }
        public float SoundVolume { get; set; } = 1.0f;

        /// <summary>
        /// The variance in degrees that the loaded munition will use for an initial heading angle.
        /// </summary>
        public float AngleVarianceDegrees { get; set; } = 0;

        /// <summary>
        /// The variance expressed in decimal percentage that determines the loaded munitions initial velocity.
        /// </summary>
        public float SpeedVariancePercent { get; set; } = 0;

        public int FireDelayMilliseconds { get; set; } = 100;
        public int Damage { get; set; } = 1;

        /// <summary>
        /// Gets or sets the maximum angle, in degrees, within which a target can be locked on.
        /// </summary>
        public int MaxLockOnAngle { get; set; } = 10;
        /// <summary>
        /// Gets or sets the maximum number of targets that can be locked on at once.
        /// </summary>
        public int MaxLocks { get; set; } = 1;
        /// <summary>
        /// Gets or sets the minimum distance, in units, required to initiate a lock.
        /// </summary>
        public float MinLockDistance { get; set; } = 50;
        /// <summary>
        /// Gets or sets the maximum distance, in units, required to initiate a lock.
        /// </summary>
        public float MaxLockDistance { get; set; } = 100;
        public bool ExplodesOnImpact { get; set; } = false;

        [JsonConverter(typeof(StringEnumConverter))]
        public MunitionType MunitionType { get; set; }

        /// <summary>
        /// The viewing angle that the munition will use for seeking/locking.
        /// </summary>
        public int SeekingEscapeAngleDegrees { get; set; } = 20;

        /// <summary>
        /// The viewing distance that the munition will use for seeking/locking.
        /// </summary>
        public int SeekingEscapeDistance { get; set; } = 20;

        /// <summary>
        /// Rate in degrees that the munition will rotate towards it target.
        /// </summary>
        public float SeekingRotationRateDegrees { get; set; } = 0.25f;

        #endregion
    }
}
