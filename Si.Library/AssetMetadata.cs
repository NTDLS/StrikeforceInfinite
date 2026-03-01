using Si.Library.Mathematics;
using static Si.Library.SiConstants;

namespace Si.Library
{
    public class AssetMetadata
    {
        [AssetMetadata("Sound Volume", "Volumne of the sound expressed in percentages.", PropertyEditorType.Float, minValue: 0, maxValue: 1)]
        public float? SoundVolume { get; set; }

        [AssetMetadata("Loop Sound", "Indicates whether the sound should loop when played.", PropertyEditorType.Boolean)]
        public bool? LoopSound { get; set; }

        [AssetMetadata("Class", "The class of the sprite which will be used to control the sprite.", PropertyEditorType.String)]
        public string? @Class { get; set; }

        [AssetMetadata("Name", "The name of the sprite, used for identification and display purposes.", PropertyEditorType.String)]
        public string? Name { get; set; }

        [AssetMetadata("Description", "A brief description of the sprite.", PropertyEditorType.String)]
        public string? Description { get; set; }

        [AssetMetadata("Attachment Asset", "The asset key of the attachment sprite.", PropertyEditorType.String)]
        public string? AttachmentAssetKey { get; set; }

        [AssetMetadata("Attachment Position", "The coordinate of the sprite's attachment position relative to its owner.", PropertyEditorType.Vector)]
        public SiVector? AttachmentPosition { get; set; }

        #region InteractiveSpriteMetadata

        [AssetMetadata("Orientation Type", "Determines how the attached sprite orientation is affected by its owner. 'FixedToOwner' means the sprite will maintain a constant orientation relative to its owner, while 'Independent' allows the sprite to have its own orientation regardless of the owner's rotation.", PropertyEditorType.Enum, enumType: typeof(AttachmentOrientationType))]
        public AttachmentOrientationType? AttachmentOrientationType { get; set; }

        [AssetMetadata("Position Type", "Determines how the attached sprite position is affected by its owner. 'FixedToOwner' means the sprite will maintain a constant position relative to its owner, while 'Independent' allows the sprite to have its own position regardless of the owner's movement.", PropertyEditorType.Enum, enumType: typeof(AttachmentPositionType))]
        public AttachmentPositionType? AttachmentPositionType { get; set; }

        [AssetMetadata("Explosion Type", "Determines the type of explosion effect for the sprite.", PropertyEditorType.Enum, enumType: typeof(ExplosionType))]
        public ExplosionType? ExplosionType { get; set; }

        [AssetMetadata("Particle Blast On Explode Amount", "Specifies the amount of particles generated when the sprite explodes.", PropertyEditorType.RangeInt)]
        public SiRange<int>? ParticleBlastOnExplodeAmount { get; set; }

        [AssetMetadata("Fragment On Explode", "Indicates whether the sprite should fragment upon explosion.", PropertyEditorType.Boolean)]
        public bool? FragmentOnExplode { get; set; }

        [AssetMetadata("Screen Shake On Explode Amount", "Specifies the intensity of screen shake when the sprite explodes.", PropertyEditorType.RangeInt)]
        public SiRange<int>? ScreenShakeOnExplodeAmount { get; set; }

        [AssetMetadata("Speed", "The speed of the sprite.", PropertyEditorType.RangeFloat)]
        public SiRange<float>? Speed { get; set; }

        [AssetMetadata("Max Throttle", "The maximum throttle of the sprite.", PropertyEditorType.Float)]
        public float? MaxThrottle { get; set; }

        [AssetMetadata("Throttle", "The throttle of the sprite, which determines its current speed as a percentage of its maximum speed.", PropertyEditorType.Float)]
        public float? Throttle { get; set; }

        /// <summary>
        /// How much does the sprite weigh? Used in physics calculations when MunitionDetection == true.
        /// A higher mass will make the sprite more resistant to acceleration and deceleration,
        /// while a lower mass will make it more agile but also more susceptible to external forces.
        /// 
        /// Mass == 0 is infinite mass, it cannot be moved by ineterial forces.
        /// </summary>
        [AssetMetadata("Mass", "The mass of the sprite, which affects its resistance to acceleration and deceleration in physics calculations when munition detection is enabled. A higher mass makes the sprite more resistant to external forces, while a lower mass makes it more agile but also more susceptible to being moved by such forces. A mass of 0 is considered infinite mass, meaning the sprite cannot be moved by inertial forces.", PropertyEditorType.RangeFloat)]
        public SiRange<float>? Mass { get; set; }

        /// <summary>
        /// How many hit points does the sprite have? When this reaches 0, the sprite is destroyed.
        /// </summary>
        [AssetMetadata("Hull", "The hull strength of the sprite, representing its hit points. When this reaches 0, the sprite is destroyed.", PropertyEditorType.Integer)]
        public int? Hull { get; set; }

        /// <summary>
        /// Gets or sets the current shield strength of the entity, representing its defensive capabilities.
        /// </summary>
        [AssetMetadata("Shields", "The shield strength of the sprite, representing its defensive capabilities.", PropertyEditorType.Integer)]
        public int? Shields { get; set; }

        /// <summary>
        /// Gets or sets the bounty amount associated with the entity. This probably will not survive the early access period.
        /// </summary>
        [AssetMetadata("Bounty", "The bounty amount associated with the sprite. This probably will not survive the early access period.", PropertyEditorType.Integer)]
        public int? Bounty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether munition detection is enabled (bullet detection).
        /// </summary>
        [AssetMetadata("Munition Detection", "Indicates whether munition detection (bullet detection) is enabled for the sprite.", PropertyEditorType.Boolean)]
        public bool? MunitionDetection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether collision detection is enabled.
        /// </summary>
        /// <remarks>When set to <see langword="true"/>, the system checks for collisions between objects.
        /// Enabling collision detection may impact performance depending on the number of objects being
        /// monitored.</remarks>
        [AssetMetadata("Collision Detection", "Indicates whether collision detection is enabled for the sprite.", PropertyEditorType.Boolean)]
        public bool? CollisionDetection { get; set; }

        /// <summary>
        /// Gets or sets the augmentation factor applied to the collision polygon, affecting its size and shape during collision detection.
        /// </summary>
        /// <remarks>Adjust this value to fine-tune the collision area for the associated sprite.
        /// Increasing the factor enlarges the collision polygon, while decreasing it reduces the area considered for
        /// collisions. This can be useful for accommodating visual effects or gameplay balancing.</remarks>
        [AssetMetadata("Collision Polygon Augmentation", "The augmentation factor applied to the collision polygon, affecting its size and shape during collision detection.", PropertyEditorType.Float)]
        public float? CollisionPolyAugmentation { get; set; }

        /// <summary>
        /// Used for the players "primary weapon slot".
        /// </summary>
        [AssetMetadata("Primary Weapon", "The primary weapon assigned to the sprite.", PropertyEditorType.SingleSpritePicker)]
        public string? PrimaryWeaponAssetKey { get; set; }

        [AssetMetadata("Attachments", "The list of attachments for the sprite.", PropertyEditorType.MultipleSpritePicker)]
        public List<AssetMetadata>? Attachments { get; set; }

        [AssetMetadata("Weapons", "The list of weapons for the sprite.", PropertyEditorType.MultipleSpritePicker)]
        public List<string>? WeaponAssetKeys { get; set; }

        #endregion

        #region InteractiveSpriteWeaponMetadata

        [AssetMetadata("Munition Count", "The number of munitions available for the weapon.", PropertyEditorType.Integer)]
        public int? MunitionCount { get; set; }

        #endregion

        #region SpriteAnimationMetadata

        [AssetMetadata("Frame Width", "The width of each frame in the sprite animation.", PropertyEditorType.Integer)]
        public int? FrameWidth { get; set; }

        [AssetMetadata("Frame Height", "The height of each frame in the sprite animation.", PropertyEditorType.Integer)]
        public int? FrameHeight { get; set; }

        [AssetMetadata("Frames Per Second", "The number of frames displayed per second in the sprite animation.", PropertyEditorType.RangeFloat)]
        public SiRange<float>? FramesPerSecond { get; set; }

        [AssetMetadata("Play Mode", "The play mode of the sprite animation.", PropertyEditorType.Enum, enumType: typeof(SiAnimationPlayMode))]
        public SiAnimationPlayMode? PlayMode { get; set; }

        #endregion

        #region WeaponMetadata

        /// <summary>
        /// If the sprite has an image, these are the paths to the bitmaps (be default, they are used at random)..
        /// </summary>
        [AssetMetadata("Sprite Assets", "The munitions assets for the weapon.", PropertyEditorType.MultipleSpritePicker)]
        public string[]? MunitionAssetKeys { get; set; }

        [AssetMetadata("Sound Asset", "The sound asset file for the sprite.", PropertyEditorType.String)]
        public string? SoundAssetKey { get; set; }

        /// <summary>
        /// The variance in degrees that the loaded munition will use for an initial heading angle.
        /// </summary>
        [AssetMetadata("Angle Variance", "The variance in degrees that the loaded munition will use for an initial heading angle.", PropertyEditorType.Float)]
        public float? AngleVarianceDegrees { get; set; }


        [AssetMetadata("Fire Delay", "The delay in milliseconds between each shot fired by the weapon.", PropertyEditorType.Integer)]
        public int? FireDelayMilliseconds { get; set; }

        [AssetMetadata("Damage", "The amount of damage dealt by the weapon.", PropertyEditorType.RangeInt)]
        public SiRange<int>? Damage { get; set; }

        /// <summary>
        /// Gets or sets the maximum angle, in degrees, within which a target can be locked on.
        /// </summary>
        [AssetMetadata("Max Lock-On Angle", "The maximum angle, in degrees, within which a target can be locked on.", PropertyEditorType.Integer)]
        public int? MaxLockOnAngle { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of targets that can be locked on at once.
        /// </summary>
        [AssetMetadata("Max Locks", "The maximum number of targets that can be locked on at once.", PropertyEditorType.Integer)]
        public int? MaxLocks { get; set; }

        /// <summary>
        /// Gets or sets the minimum distance, in units, required to initiate a lock.
        /// </summary>
        [AssetMetadata("Min Lock Distance", "The minimum distance, in units, required to initiate a lock.", PropertyEditorType.Float)]
        public float? MinLockDistance { get; set; }

        /// <summary>
        /// Gets or sets the maximum distance, in units, required to initiate a lock.
        /// </summary>
        [AssetMetadata("Max Lock Distance", "The maximum distance, in units, required to initiate a lock.", PropertyEditorType.Float)]
        public float? MaxLockDistance { get; set; }

        [AssetMetadata("Explodes On Impact", "Indicates whether the munition explodes on impact.", PropertyEditorType.Boolean)]
        public bool? ExplodesOnImpact { get; set; }

        /// <summary>
        /// The viewing angle that the munition will use for seeking/locking.
        /// </summary>
        [AssetMetadata("Seeking Escape Angle", "The viewing angle that the munition will use for seeking/locking.", PropertyEditorType.Integer)]
        public int? SeekingEscapeAngleDegrees { get; set; }

        /// <summary>
        /// The viewing distance that the munition will use for seeking/locking.
        /// </summary>
        [AssetMetadata("Seeking Escape Distance", "The viewing distance that the munition will use for seeking/locking.", PropertyEditorType.Integer)]
        public int? SeekingEscapeDistance { get; set; }

        /// <summary>
        /// Rate in degrees that the munition will rotate towards it target.
        /// </summary>
        [AssetMetadata("Seeking Rotation Rate", "Rate in degrees that the munition will rotate towards it target.", PropertyEditorType.Float)]
        public float? SeekingRotationRateDegrees { get; set; }

        #endregion
    }
}
