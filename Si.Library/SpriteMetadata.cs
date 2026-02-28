using Si.Library.Mathematics;
using static Si.Library.SiConstants;

namespace Si.Library
{
    public class SpriteMetadata
    {
        [SpriteMetadata("Sound Volume", "Volumne of the sound expressed in percentages.", PropertyEditorType.Float, minValue: 0, maxValue: 1)]
        public float? SoundVolume { get; set; }

        [SpriteMetadata("Loop Sound", "Indicates whether the sound should loop when played.", PropertyEditorType.Boolean)]
        public bool? LoopSound { get; set; }

        [SpriteMetadata("Class", "The class of the sprite, used for categorization and behavior assignment.", PropertyEditorType.String)]
        public string @Class { get; set; } = string.Empty;

        [SpriteMetadata("Name", "The name of the sprite, used for identification and display purposes.", PropertyEditorType.String)]
        public string? Name { get; set; }

        [SpriteMetadata("Description", "A brief description of the sprite.", PropertyEditorType.String)]
        public string? Description { get; set; }

        //TODO: I think this can and should be eliminated.
        [SpriteMetadata("Type", "THIS SHOULD BE ELIMINATED IN FAVOR OF A SPRITE PATH.", PropertyEditorType.String)]
        public string? Type { get; set; }

        [SpriteMetadata("Attachment Position", "The coordinate of the sprite's attachment position relative to its owner.", PropertyEditorType.Vector)]
        public SiVector? AttachmentPosition { get; set; }

        #region InteractiveSpriteAttachmentMetadata

        #endregion

        #region InteractiveSpriteMetadata

        [SpriteMetadata("Orientation Type", "Determines how the sprite's orientation is affected by its owner. 'FixedToOwner' means the sprite will maintain a constant orientation relative to its owner, while 'Independent' allows the sprite to have its own orientation regardless of the owner's rotation.", PropertyEditorType.Enum, enumType: typeof(AttachmentOrientationType))]
        public AttachmentOrientationType? OrientationType { get; set; }

        [SpriteMetadata("Position Type", "Determines how the sprite's position is affected by its owner. 'FixedToOwner' means the sprite will maintain a constant position relative to its owner, while 'Independent' allows the sprite to have its own position regardless of the owner's movement.", PropertyEditorType.Enum, enumType: typeof(AttachmentPositionType))]
        public AttachmentPositionType? PositionType { get; set; }

        [SpriteMetadata("Explosion Type", "Determines the type of explosion effect for the sprite.", PropertyEditorType.Enum, enumType: typeof(ExplosionType))]
        public ExplosionType? ExplosionType { get; set; }

        [SpriteMetadata("Particle Blast On Explode Amount", "Specifies the amount of particles generated when the sprite explodes.", PropertyEditorType.RangeInt)]
        public SiRange<int>? ParticleBlastOnExplodeAmount { get; set; }

        [SpriteMetadata("Fragment On Explode", "Indicates whether the sprite should fragment upon explosion.", PropertyEditorType.Boolean)]
        public bool? FragmentOnExplode { get; set; }

        [SpriteMetadata("Screen Shake On Explode Amount", "Specifies the intensity of screen shake when the sprite explodes.", PropertyEditorType.RangeInt)]
        public SiRange<int>? ScreenShakeOnExplodeAmount { get; set; }

        [SpriteMetadata("Speed", "The speed of the sprite.", PropertyEditorType.RangeFloat)]
        public SiRange<float>? Speed { get; set; }

        [SpriteMetadata("Max Throttle", "The maximum throttle of the sprite.", PropertyEditorType.Float)]
        public float? MaxThrottle { get; set; }

        [SpriteMetadata("Throttle", "The throttle of the sprite, which determines its current speed as a percentage of its maximum speed.", PropertyEditorType.Float)]
        public float? Throttle { get; set; }

        /// <summary>
        /// How much does the sprite weigh? Used in physics calculations when MunitionDetection == true.
        /// A higher mass will make the sprite more resistant to acceleration and deceleration,
        /// while a lower mass will make it more agile but also more susceptible to external forces.
        /// 
        /// Mass == 0 is infinite mass, it cannot be moved by ineterial forces.
        /// </summary>
        [SpriteMetadata("Mass", "The mass of the sprite, which affects its resistance to acceleration and deceleration in physics calculations when munition detection is enabled. A higher mass makes the sprite more resistant to external forces, while a lower mass makes it more agile but also more susceptible to being moved by such forces. A mass of 0 is considered infinite mass, meaning the sprite cannot be moved by inertial forces.", PropertyEditorType.RangeFloat)]
        public SiRange<float>? Mass { get; set; }

        /// <summary>
        /// How many hit points does the sprite have? When this reaches 0, the sprite is destroyed.
        /// </summary>
        [SpriteMetadata("Hull", "The hull strength of the sprite, representing its hit points. When this reaches 0, the sprite is destroyed.", PropertyEditorType.Integer)]
        public int? Hull { get; set; }

        /// <summary>
        /// Gets or sets the current shield strength of the entity, representing its defensive capabilities.
        /// </summary>
        [SpriteMetadata("Shields", "The shield strength of the sprite, representing its defensive capabilities.", PropertyEditorType.Integer)]
        public int? Shields { get; set; }

        /// <summary>
        /// Gets or sets the bounty amount associated with the entity. This probably will not survive the early access period.
        /// </summary>
        [SpriteMetadata("Bounty", "The bounty amount associated with the sprite. This probably will not survive the early access period.", PropertyEditorType.Integer)]
        public int? Bounty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether munition detection is enabled (bullet detection).
        /// </summary>
        [SpriteMetadata("Munition Detection", "Indicates whether munition detection (bullet detection) is enabled for the sprite.", PropertyEditorType.Boolean)]
        public bool? MunitionDetection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether collision detection is enabled.
        /// </summary>
        /// <remarks>When set to <see langword="true"/>, the system checks for collisions between objects.
        /// Enabling collision detection may impact performance depending on the number of objects being
        /// monitored.</remarks>
        [SpriteMetadata("Collision Detection", "Indicates whether collision detection is enabled for the sprite.", PropertyEditorType.Boolean)]
        public bool? CollisionDetection { get; set; }

        /// <summary>
        /// Gets or sets the augmentation factor applied to the collision polygon, affecting its size and shape during collision detection.
        /// </summary>
        /// <remarks>Adjust this value to fine-tune the collision area for the associated sprite.
        /// Increasing the factor enlarges the collision polygon, while decreasing it reduces the area considered for
        /// collisions. This can be useful for accommodating visual effects or gameplay balancing.</remarks>
        [SpriteMetadata("Collision Polygon Augmentation", "The augmentation factor applied to the collision polygon, affecting its size and shape during collision detection.", PropertyEditorType.Float)]
        public float? CollisionPolyAugmentation { get; set; }

        /// <summary>
        /// Used for the players "primary weapon slot".
        /// </summary>
        [SpriteMetadata("Primary Weapon", "The primary weapon assigned to the sprite.", PropertyEditorType.SingleSpritePicker)]
        public SpriteMetadata? PrimaryWeapon { get; set; }

        [SpriteMetadata("Attachments", "The list of attachments for the sprite.", PropertyEditorType.MultipleSpritePicker)]
        public List<SpriteMetadata>? Attachments { get; set; }

        [SpriteMetadata("Weapons", "The list of weapons for the sprite.", PropertyEditorType.MultipleSpritePicker)]
        public List<SpriteMetadata>? Weapons { get; set; }

        #endregion

        #region InteractiveSpriteWeaponMetadata

        [SpriteMetadata("Munition Count", "The number of munitions available for the weapon.", PropertyEditorType.Integer)]
        public int? MunitionCount { get; set; }

        #endregion

        #region SpriteAnimationMetadata

        [SpriteMetadata("Frame Width", "The width of each frame in the sprite animation.", PropertyEditorType.Integer)]
        public int? FrameWidth { get; set; }

        [SpriteMetadata("Frame Height", "The height of each frame in the sprite animation.", PropertyEditorType.Integer)]
        public int? FrameHeight { get; set; }

        [SpriteMetadata("Frames Per Second", "The number of frames displayed per second in the sprite animation.", PropertyEditorType.RangeFloat)]
        public SiRange<float>? FramesPerSecond { get; set; }

        [SpriteMetadata("Play Mode", "The play mode of the sprite animation.", PropertyEditorType.Enum, enumType: typeof(SiAnimationPlayMode))]
        public SiAnimationPlayMode? PlayMode { get; set; }

        #endregion

        #region WeaponMetadata

        /// <summary>
        /// If the sprite has an image, these are the paths to the bitmaps (be default, they are used at random)..
        /// </summary>
        [SpriteMetadata("Sprite Paths", "The paths to the munitions for the weapon.", PropertyEditorType.MultipleSpritePicker)]
        public string[]? MunitionSpritePaths { get; set; }

        [SpriteMetadata("Sound Path", "The path to the sound file for the sprite.", PropertyEditorType.String)]
        public string? SoundPath { get; set; }

        /// <summary>
        /// The variance in degrees that the loaded munition will use for an initial heading angle.
        /// </summary>
        [SpriteMetadata("Angle Variance", "The variance in degrees that the loaded munition will use for an initial heading angle.", PropertyEditorType.Float)]
        public float? AngleVarianceDegrees { get; set; }


        [SpriteMetadata("Fire Delay", "The delay in milliseconds between each shot fired by the weapon.", PropertyEditorType.Integer)]
        public int? FireDelayMilliseconds { get; set; }

        [SpriteMetadata("Damage", "The amount of damage dealt by the weapon.", PropertyEditorType.RangeInt)]
        public SiRange<int>? Damage { get; set; }

        /// <summary>
        /// Gets or sets the maximum angle, in degrees, within which a target can be locked on.
        /// </summary>
        [SpriteMetadata("Max Lock-On Angle", "The maximum angle, in degrees, within which a target can be locked on.", PropertyEditorType.Integer)]
        public int? MaxLockOnAngle { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of targets that can be locked on at once.
        /// </summary>
        [SpriteMetadata("Max Locks", "The maximum number of targets that can be locked on at once.", PropertyEditorType.Integer)]
        public int? MaxLocks { get; set; }

        /// <summary>
        /// Gets or sets the minimum distance, in units, required to initiate a lock.
        /// </summary>
        [SpriteMetadata("Min Lock Distance", "The minimum distance, in units, required to initiate a lock.", PropertyEditorType.Float)]
        public float? MinLockDistance { get; set; }

        /// <summary>
        /// Gets or sets the maximum distance, in units, required to initiate a lock.
        /// </summary>
        [SpriteMetadata("Max Lock Distance", "The maximum distance, in units, required to initiate a lock.", PropertyEditorType.Float)]
        public float? MaxLockDistance { get; set; }

        [SpriteMetadata("Explodes On Impact", "Indicates whether the munition explodes on impact.", PropertyEditorType.Boolean)]
        public bool? ExplodesOnImpact { get; set; }

        /// <summary>
        /// The viewing angle that the munition will use for seeking/locking.
        /// </summary>
        [SpriteMetadata("Seeking Escape Angle", "The viewing angle that the munition will use for seeking/locking.", PropertyEditorType.Integer)]
        public int? SeekingEscapeAngleDegrees { get; set; }

        /// <summary>
        /// The viewing distance that the munition will use for seeking/locking.
        /// </summary>
        [SpriteMetadata("Seeking Escape Distance", "The viewing distance that the munition will use for seeking/locking.", PropertyEditorType.Integer)]
        public int? SeekingEscapeDistance { get; set; }

        /// <summary>
        /// Rate in degrees that the munition will rotate towards it target.
        /// </summary>
        [SpriteMetadata("Seeking Rotation Rate", "Rate in degrees that the munition will rotate towards it target.", PropertyEditorType.Float)]
        public float? SeekingRotationRateDegrees { get; set; }

        #endregion
    }
}
