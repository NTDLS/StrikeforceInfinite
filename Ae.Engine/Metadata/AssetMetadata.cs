using Ae.Engine.AI;
using Ae.Engine.Audio;
using Ae.Engine.Sprite._Superclass;
using Ae.Engine.Sprite._Superclass._Root;
using Ae.Engine.Sprite._Superclass.Animation;
using Ae.Engine.Sprite._Superclass.Interactive;
using Ae.Engine.Sprite._Superclass.Munition;
using Ae.Library;
using Ae.Library.Compiler;
using Ae.Library.Mathematics;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static Ae.Library.AeConstants;

namespace Ae.Engine.Metadata
{
    public class AssetMetadata
    {
        /// <summary>
        /// The name of the type that was dynamically compiled for this asset.
        /// </summary>
        [JsonIgnore]
        public string DynamicTypeName => AeRuntimeCompiler.AssetKeyToClassName(AssetKey);


        [AssetMetadata("Asset Key", "The asset key of attachment sprite.", PropertyEditorGroup.Base, PropertyEditorType.Readonly,
            applicableTo: null)]
        public string? AssetKey { get; set; }

        [AssetMetadata("Sound Volume", "Volume of the sound expressed in percentages.", PropertyEditorGroup.Audio, PropertyEditorType.Float,
            applicableTo: [typeof(AudioClip)], minValue: 0, maxValue: 1)]
        public float? SoundVolume { get; set; }

        [AssetMetadata("Loop Sound", "Indicates whether the sound should loop when played.", PropertyEditorGroup.Audio, PropertyEditorType.Boolean,
            applicableTo: [typeof(AudioClip)])]
        public bool? LoopSound { get; set; }

        [AssetMetadata("Class", "The class of the sprite which will be used to control the sprite.", PropertyEditorGroup.Base, PropertyEditorType.Class,
            applicableTo: null)]
        public string? Class { get; set; }

        [AssetMetadata("Name", "The name of the sprite, used for identification and display purposes.", PropertyEditorGroup.Base, PropertyEditorType.String,
            applicableTo: null)]
        public string? Name { get; set; }

        [AssetMetadata("Description", "A brief description of the sprite.", PropertyEditorGroup.Base, PropertyEditorType.String,
            applicableTo: null)]
        public string? Description { get; set; }

        //TODO: THIS NEED A NEW EDITOR.
        //Note that these are meant to be set in the owning sprite's metadata, not in the attachment's own metadata.
        [AssetMetadata("Attachment Position", "The coordinate of the sprite's attachment position relative to its owner.",
            PropertyEditorGroup.Attachment, PropertyEditorType.Vector,
            applicableTo: [typeof(SpriteBase)])]
        public AeVector? AttachmentPosition { get; set; }

        [AssetMetadata("AI Controllers", "The AI controller classes that will be available to this sprite.",
            PropertyEditorGroup.AI, PropertyEditorType.MultipleSpritePicker,
            applicableTo: [typeof(SpriteInteractive)], requireAssignableFrom: typeof(IAIController))]
        public string[]? AIControllers { get; set; }

        [AssetMetadata("Default AI Controller", "The default AI controller class for the sprite.",
            PropertyEditorGroup.AI, PropertyEditorType.SingleSpritePicker,
            applicableTo: [typeof(SpriteInteractive)], requireAssignableFrom: typeof(IAIController))]
        public string? DefaultAIController { get; set; }

        #region InteractiveSpriteMetadata

        //TODO: THIS NEED A NEW EDITOR.
        //Note that these are meant to be set in the owning sprite's metadata, not in the attachment's own metadata.
        [AssetMetadata("Orientation Type", "Determines how the attached sprite orientation is affected by its owner. 'FixedToOwner' means the sprite will maintain a constant orientation relative to its owner, while 'Independent' allows the sprite to have its own orientation regardless of the owner's rotation.",
            PropertyEditorGroup.Attachment, PropertyEditorType.Enum,
            applicableTo: [typeof(SpriteBase)], enumType: typeof(AttachmentOrientationType))]
        public AttachmentOrientationType? AttachmentOrientationType { get; set; }

        //TODO: THIS NEED A NEW EDITOR.
        //Note that these are meant to be set in the owning sprite's metadata, not in the attachment's own metadata.
        [AssetMetadata("Position Type", "Determines how the attached sprite position is affected by its owner. 'FixedToOwner' means the sprite will maintain a constant position relative to its owner, while 'Independent' allows the sprite to have its own position regardless of the owner's movement.",
            PropertyEditorGroup.Attachment, PropertyEditorType.Enum,
            applicableTo: [typeof(SpriteBase)], enumType: typeof(AttachmentPositionType))]
        public AttachmentPositionType? AttachmentPositionType { get; set; }

        [AssetMetadata("Explosion Type", "Determines the type of explosion effect for the sprite.",
            PropertyEditorGroup.Destroy, PropertyEditorType.Enum,
            applicableTo: [typeof(SpriteBase)], enumType: typeof(ExplosionType))]
        public ExplosionType? ExplosionType { get; set; }

        [AssetMetadata("Particle Blast On Explode Amount", "Specifies the amount of particles generated when the sprite explodes.",
            PropertyEditorGroup.Destroy, PropertyEditorType.RangeInt,
            applicableTo: [typeof(SpriteBase)])]
        public AeRange<int>? ParticleBlastOnExplodeAmount { get; set; }

        [AssetMetadata("Fragment On Explode", "Indicates whether the sprite should fragment upon explosion.",
            PropertyEditorGroup.Destroy, PropertyEditorType.Boolean,
            applicableTo: [typeof(SpriteBase)])]
        public bool? FragmentOnExplode { get; set; }

        [AssetMetadata("Screen Shake On Explode Amount", "Specifies the intensity of screen shake when the sprite explodes.",
            PropertyEditorGroup.Destroy, PropertyEditorType.RangeInt,
            applicableTo: [typeof(SpriteBase)])]
        public AeRange<int>? ScreenShakeOnExplodeAmount { get; set; }

        [AssetMetadata("Speed", "The speed of the sprite.", PropertyEditorGroup.Momentum, PropertyEditorType.RangeFloat,
            applicableTo: [typeof(SpriteBase)])]
        public AeRange<float>? Speed { get; set; }

        [AssetMetadata("Max Throttle", "The maximum throttle of the sprite.", PropertyEditorGroup.Momentum, PropertyEditorType.Float,
            applicableTo: [typeof(SpriteBase)])]
        public float? MaxThrottle { get; set; }

        [AssetMetadata("Throttle", "The throttle of the sprite, which determines its current speed as a percentage of its maximum speed.",
            PropertyEditorGroup.Momentum, PropertyEditorType.Float,
            applicableTo: [typeof(SpriteBase)])]
        public float? Throttle { get; set; }

        /// <summary>
        /// How much does the sprite weigh? Used in physics calculations when MunitionDetection == true.
        /// A higher mass will make the sprite more resistant to acceleration and deceleration,
        /// while a lower mass will make it more agile but also more susceptible to external forces.
        /// 
        /// Mass == 0 is infinite mass, it cannot be moved by inertial forces.
        /// </summary>
        [AssetMetadata("Mass", "The mass of the sprite, which affects its resistance to acceleration and deceleration in physics calculations when munition detection is enabled. A higher mass makes the sprite more resistant to external forces, while a lower mass makes it more agile but also more susceptible to being moved by such forces. A mass of 0 is considered infinite mass, meaning the sprite cannot be moved by inertial forces.",
            PropertyEditorGroup.Momentum, PropertyEditorType.RangeFloat,
            applicableTo: [typeof(SpriteInteractive)])]
        public AeRange<float>? Mass { get; set; }

        /// <summary>
        /// How many hit points does the sprite have? When this reaches 0, the sprite is destroyed.
        /// </summary>
        [AssetMetadata("Hull", "The hull strength of the sprite, representing its hit points. When this reaches 0, the sprite is destroyed.",
            PropertyEditorGroup.Health, PropertyEditorType.Integer,
            applicableTo: [typeof(SpriteBase)])]
        public int? Hull { get; set; }

        /// <summary>
        /// Gets or sets the current shield strength of the entity, representing its defensive capabilities.
        /// </summary>
        [AssetMetadata("Shields", "The shield strength of the sprite, representing its defensive capabilities.",
            PropertyEditorGroup.Health, PropertyEditorType.Integer,
            applicableTo: [typeof(SpriteBase)])]
        public int? Shields { get; set; }

        /// <summary>
        /// Gets or sets the bounty amount associated with the entity. This probably will not survive the early access period.
        /// </summary>
        [AssetMetadata("Bounty", "The bounty amount associated with the sprite. This probably will not survive the early access period.",
            PropertyEditorGroup.Health, PropertyEditorType.Integer,
            applicableTo: [typeof(SpriteBase)])]
        public int? Bounty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether munition detection is enabled (bullet detection).
        /// </summary>
        [AssetMetadata("Munition Detection", "Indicates whether munition detection (bullet detection) is enabled for the sprite.",
            PropertyEditorGroup.Collision, PropertyEditorType.Boolean,
            applicableTo: [typeof(SpriteBase)])]
        public bool? MunitionDetection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether collision detection is enabled.
        /// </summary>
        /// <remarks>When set to <see langword="true"/>, the system checks for collisions between objects.
        /// Enabling collision detection may impact performance depending on the number of objects being
        /// monitored.</remarks>
        [AssetMetadata("Collision Detection", "Indicates whether collision detection is enabled for the sprite.",
            PropertyEditorGroup.Collision, PropertyEditorType.Boolean,
            applicableTo: [typeof(SpriteInteractive)])]
        public bool? CollisionDetection { get; set; }

        /// <summary>
        /// Gets or sets the augmentation factor applied to the collision polygon, affecting its size and shape during collision detection.
        /// </summary>
        /// <remarks>Adjust this value to fine-tune the collision area for the associated sprite.
        /// Increasing the factor enlarges the collision polygon, while decreasing it reduces the area considered for
        /// collisions. This can be useful for accommodating visual effects or gameplay balancing.</remarks>
        [AssetMetadata("Collision Polygon Augmentation", "The augmentation factor applied to the collision polygon, affecting its size and shape during collision detection.",
            PropertyEditorGroup.Collision, PropertyEditorType.Float,
            applicableTo: [typeof(SpriteInteractive)])]
        public float? CollisionPolyAugmentation { get; set; }

        /// <summary>
        /// Used for the players "primary weapon slot".
        /// </summary>
        [AssetMetadata("Primary Weapon", "The primary weapon assigned to the sprite.",
            PropertyEditorGroup.Weapons, PropertyEditorType.SingleSpritePicker,
            applicableTo: [typeof(SpriteInteractive)], requireAssignableFrom: typeof(SpriteWeapon))]
        public string? PrimaryWeaponAssetKey { get; set; }

        //TODO: THIS NEED A NEW EDITOR.
        //Note that these are meant to be set in the owning sprite's metadata, not in the attachment's own metadata.
        [AssetMetadata("Attachments", "The list of attachments for the sprite.",
            PropertyEditorGroup.Attachment, PropertyEditorType.MultipleSpritePicker,
            applicableTo: [typeof(SpriteBase)], requireAssignableFrom: typeof(SpriteAttachment))]
        public List<AssetMetadata>? Attachments { get; set; }

        [AssetMetadata("Weapon Assets", "The list of weapons for the sprite.",
            PropertyEditorGroup.Weapons, PropertyEditorType.MultipleSpritePicker,
            applicableTo: [typeof(SpriteInteractive)], requireAssignableFrom: typeof(SpriteWeapon))]
        public List<string>? WeaponAssetKeys { get; set; }

        #endregion

        #region InteractiveSpriteWeaponMetadata

        [AssetMetadata("Munition Count", "The number of munitions available for the weapon.",
            PropertyEditorGroup.Munitions, PropertyEditorType.Integer,
            applicableTo: [typeof(SpriteWeapon)])]
        public int? MunitionCount { get; set; }

        #endregion

        #region SpriteAnimationMetadata

        [AssetMetadata("Frame Width", "The width of each frame in the sprite animation.",
            PropertyEditorGroup.Animation, PropertyEditorType.Integer,
            applicableTo: [typeof(SpriteAnimation)])]
        public int? FrameWidth { get; set; }

        [AssetMetadata("Frame Height", "The height of each frame in the sprite animation.",
            PropertyEditorGroup.Animation, PropertyEditorType.Integer,
            applicableTo: [typeof(SpriteAnimation)])]
        public int? FrameHeight { get; set; }

        [AssetMetadata("Frames Per Second", "The number of frames displayed per second in the sprite animation.",
            PropertyEditorGroup.Animation, PropertyEditorType.RangeFloat,
            applicableTo: [typeof(SpriteAnimation)])]
        public AeRange<float>? FramesPerSecond { get; set; }

        [AssetMetadata("Play Mode", "The play mode of the sprite animation.",
            PropertyEditorGroup.Animation, PropertyEditorType.Enum,
            applicableTo: [typeof(SpriteAnimation)], enumType: typeof(SiAnimationPlayMode))]
        public SiAnimationPlayMode? PlayMode { get; set; }

        #endregion

        #region WeaponMetadata

        /// <summary>
        /// If the sprite has an image, these are the paths to the bitmaps (be default, they are used at random)..
        /// </summary>
        [AssetMetadata("Munition Assets", "The munitions assets for the weapon.",
            PropertyEditorGroup.Munitions, PropertyEditorType.MultipleSpritePicker,
            applicableTo: [typeof(SpriteWeapon)],
            requireAssignableFrom: typeof(SpriteMunition))]
        public string[]? MunitionAssetKeys { get; set; }

        [AssetMetadata("Sound Asset", "The sound asset file for the sprite.",
            PropertyEditorGroup.Audio, PropertyEditorType.String,
            applicableTo: [typeof(SpriteBase)]/*, requireAssignableFrom: typeof(SpriteSound)*/)]
        public string[]? SoundAssetKeys { get; set; }

        /// <summary>
        /// The variance in degrees that the loaded munition will use for an initial heading angle.
        /// </summary>
        [AssetMetadata("Angle Variance", "The variance in degrees that the loaded munition will use for an initial heading angle.",
            PropertyEditorGroup.Munitions, PropertyEditorType.Float,
            applicableTo: [typeof(SpriteMunition)])]
        public float? AngleVarianceDegrees { get; set; }


        [AssetMetadata("Fire Delay", "The delay in milliseconds between each shot fired by the weapon.",
            PropertyEditorGroup.Munitions, PropertyEditorType.Integer,
            applicableTo: [typeof(SpriteWeapon)])]
        public int? FireDelayMilliseconds { get; set; }

        [AssetMetadata("Damage", "The amount of damage dealt by the munition.",
            PropertyEditorGroup.Munitions, PropertyEditorType.RangeInt,
            applicableTo: [typeof(SpriteMunition)])]
        public AeRange<int>? Damage { get; set; }

        /// <summary>
        /// Gets or sets the maximum angle, in degrees, within which a target can be locked on.
        /// </summary>
        [AssetMetadata("Max Lock-On Angle", "The maximum angle, in degrees, within which a target can be locked on.",
            PropertyEditorGroup.Weapons, PropertyEditorType.Integer,
            applicableTo: [typeof(SpriteWeapon)])]
        public int? MaxLockOnAngle { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of targets that can be locked on at once.
        /// </summary>
        [AssetMetadata("Max Locks", "The maximum number of targets that can be locked on at once.",
            PropertyEditorGroup.Weapons, PropertyEditorType.Integer,
            applicableTo: [typeof(SpriteWeapon)])]
        public int? MaxLocks { get; set; }

        /// <summary>
        /// Gets or sets the minimum distance, in units, required to initiate a lock.
        /// </summary>
        [AssetMetadata("Min Lock Distance", "The minimum distance, in units, required to initiate a lock.",
            PropertyEditorGroup.Weapons, PropertyEditorType.Float,
            applicableTo: [typeof(SpriteWeapon)])]
        public float? MinLockDistance { get; set; }

        /// <summary>
        /// Gets or sets the maximum distance, in units, required to initiate a lock.
        /// </summary>
        [AssetMetadata("Max Lock Distance", "The maximum distance, in units, required to initiate a lock.",
            PropertyEditorGroup.Weapons, PropertyEditorType.Float,
            applicableTo: [typeof(SpriteWeapon)])]
        public float? MaxLockDistance { get; set; }

        [AssetMetadata("Explodes On Impact", "Indicates whether the munition explodes on impact.",
            PropertyEditorGroup.Munitions, PropertyEditorType.Boolean,
            applicableTo: [typeof(SpriteMunition)])]
        public bool? ExplodesOnImpact { get; set; }

        /// <summary>
        /// The viewing angle that the munition will use for seeking/locking.
        /// </summary>
        [AssetMetadata("Seeking Escape Angle", "The viewing angle that the munition will use for seeking/locking.",
            PropertyEditorGroup.Munitions, PropertyEditorType.Integer,
            applicableTo: [typeof(SpriteMunition)])]
        public int? SeekingEscapeAngleDegrees { get; set; }

        /// <summary>
        /// The viewing distance that the munition will use for seeking/locking.
        /// </summary>
        [AssetMetadata("Seeking Escape Distance", "The viewing distance that the munition will use for seeking/locking.",
            PropertyEditorGroup.Munitions, PropertyEditorType.Integer,
            applicableTo: [typeof(SpriteMunition)])]
        public int? SeekingEscapeDistance { get; set; }

        /// <summary>
        /// Rate in degrees that the munition will rotate towards it target.
        /// </summary>
        [AssetMetadata("Seeking Rotation Rate", "Rate in degrees that the munition will rotate towards it target.",
            PropertyEditorGroup.Munitions, PropertyEditorType.Float,
            applicableTo: [typeof(SpriteMunition)])]
        public float? SeekingRotationRateDegrees { get; set; }

        #endregion
    }
}
