using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Si.Library;
using Si.Library.Mathematics;
using System.Collections.Generic;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite
{
    public class Metadata
    {
        [MetaData("Class", "The class of the sprite, used for categorization and behavior assignment.", PropertyEditorType.String)]
        public string @Class { get; set; } = string.Empty;

        [MetaData("Name", "The name of the sprite, used for identification and display purposes.", PropertyEditorType.String)]
        public string Name { get; set; } = string.Empty;
        [MetaData("Description", "A brief description of the sprite.", PropertyEditorType.String)]
        public string Description { get; set; } = string.Empty;

        //TODO: I think this can and should be eliminated.
        [MetaData("Type", "THIS SHOULD BE ELIMINATED IN FAVOR OF A SPRITE PATH.", PropertyEditorType.String)]
        public string Type { get; set; } = string.Empty;

        [MetaData("X", "The X coordinate of the sprite's location relative to its owner.", PropertyEditorType.Float)]
        public float X { get; set; }
        [MetaData("Y", "The Y coordinate of the sprite's location relative to its owner.", PropertyEditorType.Float)]
        public float Y { get; set; }

        #region InteractiveSpriteAttachmentMetadata

        [JsonIgnore]
        [MetaData("Location Relative To Owner", "The location of the sprite relative to its owner, represented as a vector with X and Y coordinates.", PropertyEditorType.Vector)]
        public SiVector LocationRelativeToOwner { get => new SiVector(X, Y); }

        #endregion

        #region InteractiveSpriteMetadata

        [MetaData("Orientation Type", "Determines how the sprite's orientation is affected by its owner. 'FixedToOwner' means the sprite will maintain a constant orientation relative to its owner, while 'Independent' allows the sprite to have its own orientation regardless of the owner's rotation.", PropertyEditorType.Enum, enumType: typeof(AttachmentOrientationType))]
        public AttachmentOrientationType OrientationType { get; set; } = AttachmentOrientationType.FixedToOwner;
        [MetaData("Position Type", "Determines how the sprite's position is affected by its owner. 'FixedToOwner' means the sprite will maintain a constant position relative to its owner, while 'Independent' allows the sprite to have its own position regardless of the owner's movement.", PropertyEditorType.Enum, enumType: typeof(AttachmentPositionType))]
        public AttachmentPositionType PositionType { get; set; } = AttachmentPositionType.FixedToOwner;

        [MetaData("Explosion Type", "Determines the type of explosion effect for the sprite.", PropertyEditorType.Enum, enumType: typeof(ExplosionType))]
        public ExplosionType ExplosionType { get; set; } = ExplosionType.MediumFire;
        [MetaData("Particle Blast On Explode Amount", "Specifies the amount of particles generated when the sprite explodes.", PropertyEditorType.RangeInt)]
        public SiRange<int> ParticleBlastOnExplodeAmount { get; set; } = new SiRange<int>(1, 5000);
        [MetaData("Fragment On Explode", "Indicates whether the sprite should fragment upon explosion.", PropertyEditorType.Boolean)]
        public bool FragmentOnExplode { get; set; } = true;
        [MetaData("Screen Shake On Explode Amount", "Specifies the intensity of screen shake when the sprite explodes.", PropertyEditorType.RangeInt)]
        public SiRange<int> ScreenShakeOnExplodeAmount { get; set; } = new SiRange<int>(1, 1000);

        [MetaData("Speed", "The speed of the sprite.", PropertyEditorType.Float)]
        public float Speed { get; set; } = 1f;
        [MetaData("Max Throttle", "The maximum throttle of the sprite.", PropertyEditorType.Float)]
        public float MaxThrottle { get; set; } = 1.0f;
        [MetaData("Throttle", "The throttle of the sprite, which determines its current speed as a percentage of its maximum speed.", PropertyEditorType.Float)]
        public float Throttle { get; set; } = 1.0f;

        /// <summary>
        /// How much does the sprite weigh? Used in physics calculations when MunitionDetection == true.
        /// A higher mass will make the sprite more resistant to acceleration and deceleration,
        /// while a lower mass will make it more agile but also more susceptible to external forces.
        /// 
        /// Mass == 0 is infinite mass, it cannot be moved by ineterial forces.
        /// </summary>
        [MetaData("Mass", "The mass of the sprite, which affects its resistance to acceleration and deceleration in physics calculations when munition detection is enabled. A higher mass makes the sprite more resistant to external forces, while a lower mass makes it more agile but also more susceptible to being moved by such forces. A mass of 0 is considered infinite mass, meaning the sprite cannot be moved by inertial forces.", PropertyEditorType.Float)]
        public float Mass { get; set; }

        /// <summary>
        /// How many hit points does the sprite have? When this reaches 0, the sprite is destroyed.
        /// </summary>
        [MetaData("Hull", "The hull strength of the sprite, representing its hit points. When this reaches 0, the sprite is destroyed.", PropertyEditorType.Integer)]
        public int Hull { get; set; }

        /// <summary>
        /// Gets or sets the current shield strength of the entity, representing its defensive capabilities.
        /// </summary>
        [MetaData("Shields", "The shield strength of the sprite, representing its defensive capabilities.", PropertyEditorType.Integer)]
        public int Shields { get; set; }

        /// <summary>
        /// Gets or sets the bounty amount associated with the entity. This probably will not survive the early access period.
        /// </summary>
        [MetaData("Bounty", "The bounty amount associated with the sprite. This probably will not survive the early access period.", PropertyEditorType.Integer)]
        public int Bounty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether munition detection is enabled (bullet detection).
        /// </summary>
        [MetaData("Munition Detection", "Indicates whether munition detection (bullet detection) is enabled for the sprite.", PropertyEditorType.Boolean)]
        public bool MunitionDetection { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether collision detection is enabled.
        /// </summary>
        /// <remarks>When set to <see langword="true"/>, the system checks for collisions between objects.
        /// Enabling collision detection may impact performance depending on the number of objects being
        /// monitored.</remarks>
        [MetaData("Collision Detection", "Indicates whether collision detection is enabled for the sprite.", PropertyEditorType.Boolean)]
        public bool CollisionDetection { get; set; } = false;

        /// <summary>
        /// Gets or sets the augmentation factor applied to the collision polygon, affecting its size and shape during collision detection.
        /// </summary>
        /// <remarks>Adjust this value to fine-tune the collision area for the associated sprite.
        /// Increasing the factor enlarges the collision polygon, while decreasing it reduces the area considered for
        /// collisions. This can be useful for accommodating visual effects or gameplay balancing.</remarks>
        [MetaData("Collision Polygon Augmentation", "The augmentation factor applied to the collision polygon, affecting its size and shape during collision detection.", PropertyEditorType.Float)]
        public float CollisionPolyAugmentation { get; set; } = 0;

        /// <summary>
        /// Used for the players "primary weapon slot".
        /// </summary>
        [MetaData("Primary Weapon", "The primary weapon assigned to the sprite.", PropertyEditorType.SingleSpritePicker)]
        public Metadata? PrimaryWeapon { get; set; }
        [MetaData("Attachments", "The list of attachments for the sprite.", PropertyEditorType.MultipleSpritePicker)]
        public List<Metadata> Attachments { get; set; } = new();
        [MetaData("Weapons", "The list of weapons for the sprite.", PropertyEditorType.MultipleSpritePicker)]
        public List<Metadata> Weapons { get; set; } = new();

        #endregion

        #region InteractiveSpriteWeaponMetadata

        [MetaData("Munition Count", "The number of munitions available for the weapon.", PropertyEditorType.Integer)]
        public int MunitionCount { get; set; }

        #endregion

        #region SpriteAnimationMetadata

        [MetaData("Frame Width", "The width of each frame in the sprite animation.", PropertyEditorType.Integer)]
        public int FrameWidth { get; set; }
        [MetaData("Frame Height", "The height of each frame in the sprite animation.", PropertyEditorType.Integer)]
        public int FrameHeight { get; set; }
        [MetaData("Frames Per Second", "The number of frames displayed per second in the sprite animation.", PropertyEditorType.Float)]
        public float FramesPerSecond { get; set; }
        [MetaData("Play Mode", "The play mode of the sprite animation.", PropertyEditorType.Enum, enumType: typeof(SiAnimationPlayMode))]
        public SiAnimationPlayMode PlayMode { get; set; }

        #endregion

        #region WeaponMetadata

        /// <summary>
        /// If the sprite has an image, these are the paths to the bitmaps (be default, they are used at random)..
        /// </summary>
        [MetaData("Sprite Paths", "The paths to the bitmaps for the sprite.", PropertyEditorType.MultipleSpritePicker)]
        public string[] SpritePaths { get; set; } = [];

        [MetaData("Sound Path", "The path to the sound file for the sprite.", PropertyEditorType.String)]
        public string? SoundPath { get; set; }
        [MetaData("Sound Volume", "The volume of the sound for the sprite.", PropertyEditorType.Float)]
        public float SoundVolume { get; set; } = 1.0f;

        /// <summary>
        /// The variance in degrees that the loaded munition will use for an initial heading angle.
        /// </summary>
        [MetaData("Angle Variance", "The variance in degrees that the loaded munition will use for an initial heading angle.", PropertyEditorType.Float)]
        public float AngleVarianceDegrees { get; set; }

        /// <summary>
        /// The variance expressed in decimal percentage that determines the loaded munitions initial velocity.
        /// </summary>
        [MetaData("Speed Variance", "The variance expressed in decimal percentage that determines the loaded munitions initial velocity.", PropertyEditorType.Float)]
        public float SpeedVariancePercent { get; set; }

        [MetaData("Fire Delay", "The delay in milliseconds between each shot fired by the weapon.", PropertyEditorType.Integer)]
        public int FireDelayMilliseconds { get; set; } = 100;

        [MetaData("Damage", "The amount of damage dealt by the weapon.", PropertyEditorType.Integer)]
        public int Damage { get; set; } = 1;

        /// <summary>
        /// Gets or sets the maximum angle, in degrees, within which a target can be locked on.
        /// </summary>
        [MetaData("Max Lock-On Angle", "The maximum angle, in degrees, within which a target can be locked on.", PropertyEditorType.Integer)]
        public int MaxLockOnAngle { get; set; } = 10;
        /// <summary>
        /// Gets or sets the maximum number of targets that can be locked on at once.
        /// </summary>
        [MetaData("Max Locks", "The maximum number of targets that can be locked on at once.", PropertyEditorType.Integer)]
        public int MaxLocks { get; set; } = 1;
        /// <summary>
        /// Gets or sets the minimum distance, in units, required to initiate a lock.
        /// </summary>
        [MetaData("Min Lock Distance", "The minimum distance, in units, required to initiate a lock.", PropertyEditorType.Float)]
        public float MinLockDistance { get; set; } = 50;
        /// <summary>
        /// Gets or sets the maximum distance, in units, required to initiate a lock.
        /// </summary>
        [MetaData("Max Lock Distance", "The maximum distance, in units, required to initiate a lock.", PropertyEditorType.Float)]
        public float MaxLockDistance { get; set; } = 100;
        [MetaData("Explodes On Impact", "Indicates whether the munition explodes on impact.", PropertyEditorType.Boolean)]
        public bool ExplodesOnImpact { get; set; } = false;

        [MetaData("Munition Type", "The type of munition.", PropertyEditorType.Enum)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MunitionType MunitionType { get; set; }

        /// <summary>
        /// The viewing angle that the munition will use for seeking/locking.
        /// </summary>
        [MetaData("Seeking Escape Angle", "The viewing angle that the munition will use for seeking/locking.", PropertyEditorType.Integer)]
        public int SeekingEscapeAngleDegrees { get; set; } = 20;

        /// <summary>
        /// The viewing distance that the munition will use for seeking/locking.
        /// </summary>
        [MetaData("Seeking Escape Distance", "The viewing distance that the munition will use for seeking/locking.", PropertyEditorType.Integer)]
        public int SeekingEscapeDistance { get; set; } = 20;

        /// <summary>
        /// Rate in degrees that the munition will rotate towards it target.
        /// </summary>
        [MetaData("Seeking Rotation Rate", "Rate in degrees that the munition will rotate towards it target.", PropertyEditorType.Float)]
        public float SeekingRotationRateDegrees { get; set; } = 0.25f;

        #endregion
    }
}
