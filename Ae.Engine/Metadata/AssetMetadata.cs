using Ae.Engine.AI;
using Ae.Engine.Audio;
using Ae.Engine.Compiler;
using Ae.Engine.Mathematics;
using Ae.Engine.Sprite;
using Ae.Engine.Sprite.Base;
using Ae.Engine.Sprite.Interactive;
using Ae.Engine.Sprite.Munition;
using Ae.Engine.Types;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Ae.Engine.Metadata
{
    /// <summary>
    /// Represents metadata for an asset, including properties that describe its characteristics, behaviors, and
    /// configuration for use in the game engine. This class provides a flexible structure for defining asset-specific
    /// information such as type, appearance, physics, AI, audio, animation, and weapon attributes.
    /// </summary>
    /// <remarks>Asset metadata is used to configure and control various aspects of sprites, audio clips,
    /// animations, weapons, and attachments within the game. Properties are grouped by functionality and may be
    /// applicable to different asset types. Many properties are nullable to allow for optional configuration. Some
    /// properties, such as attachments and AI controllers, reference other assets or classes to enable extensible
    /// behaviors. When editing metadata, ensure that values meet any documented constraints or requirements for correct
    /// operation. Thread safety is not guaranteed; concurrent modifications should be managed externally.</remarks>
    public class AssetMetadata
    {
        /// <summary>
        /// The name of the type that was dynamically compiled for this asset.
        /// </summary>
        [JsonIgnore]
        private string? _dynamicTypeName;

        /// <summary>
        /// The name of the type that was dynamically compiled for this asset.
        /// </summary>
        [JsonIgnore]
        public string DynamicTypeName
        {
            get
            {
                if (_dynamicTypeName == null)
                {
                    lock (this)
                    {
                        _dynamicTypeName ??= AeRuntimeCompiler.AssetKeyToClassName(AssetKey);
                    }
                }
                return _dynamicTypeName;
            }
            set
            {
                _dynamicTypeName = value;
            }
        }

        /// <summary>
        /// Gets or sets the asset key associated with the attachment sprite.
        /// </summary>
        [AssetMetadata("Asset Key", "The asset key of attachment sprite.", AePropertyEditorGroup.Base, AePropertyEditorType.Readonly,
            applicableTo: null)]
        public string? AssetKey { get; set; }

        /// <summary>
        /// Gets or sets the volume level of the sound, expressed as a percentage between 0 and 1.
        /// </summary>
        [AssetMetadata("Sound Volume", "Volume of the sound expressed in percentages.", AePropertyEditorGroup.Audio, AePropertyEditorType.Float,
            applicableTo: [typeof(AeAudioClip)], minValue: 0, maxValue: 1)]
        public float? SoundVolume { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the sound should loop when played.
        /// </summary>
        [AssetMetadata("Loop Sound", "Indicates whether the sound should loop when played.", AePropertyEditorGroup.Audio, AePropertyEditorType.Boolean,
            applicableTo: [typeof(AeAudioClip)])]
        public bool? LoopSound { get; set; }

        /// <summary>
        /// Gets or sets the class name used to control the sprite.
        /// </summary>
        /// <remarks>The class determines the behavior and control logic applied to the sprite. Assign a
        /// valid class name to enable appropriate functionality. If the value is null or empty, default behavior may be
        /// applied.</remarks>
        [AssetMetadata("Class", "The class of the sprite which will be used to control the sprite.", AePropertyEditorGroup.Base, AePropertyEditorType.Class,
            applicableTo: null)]
        public string? Class { get; set; }

        /// <summary>
        /// Gets or sets the name of the sprite used for identification and display purposes.
        /// </summary>
        [AssetMetadata("Name", "The name of the sprite, used for identification and display purposes.", AePropertyEditorGroup.Base, AePropertyEditorType.String,
            applicableTo: null)]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets a brief description of the sprite.
        /// </summary>
        [AssetMetadata("Description", "A brief description of the sprite.", AePropertyEditorGroup.Base, AePropertyEditorType.String,
            applicableTo: null)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the coordinate of the sprite's attachment position relative to its owner.
        /// </summary>
        /// <remarks>This property should be set in the owning sprite's metadata rather than in the
        /// attachment's own metadata. The value represents the position where the attachment is anchored relative to
        /// the owner sprite, which can affect rendering and positioning behaviors.</remarks>
        //TODO: THIS NEED A NEW EDITOR.
        //Note that these are meant to be set in the owning sprite's metadata, not in the attachment's own metadata.
        [AssetMetadata("Attachment Position", "The coordinate of the sprite's attachment position relative to its owner.",
            AePropertyEditorGroup.Attachment, AePropertyEditorType.Vector,
            applicableTo: [typeof(AeSprite)])]
        public AeVector? AttachmentPosition { get; set; }

        /// <summary>
        /// Gets or sets the collection of AI controller class names available to this sprite.
        /// </summary>
        /// <remarks>Each entry represents a class that implements AI behavior for the sprite. Only
        /// classes assignable from AeAIStateMachine are valid. This property is typically used to configure which AI
        /// controllers can be selected or applied to interactive sprites.</remarks>
        [AssetMetadata("AI Controllers", "The AI controller classes that will be available to this sprite.",
            AePropertyEditorGroup.AI, AePropertyEditorType.MultipleSpritePicker,
            applicableTo: [typeof(AeSpriteInteractive)], requireAssignableFrom: typeof(AeAIStateMachine))]
        public string[]? AIControllers { get; set; }

        /// <summary>
        /// Gets or sets the default AI controller class for the sprite.
        /// </summary>
        /// <remarks>The specified controller must be assignable from the type AeAIStateMachine and is
        /// applicable to interactive sprites. Use this property to define the AI behavior for sprites that require
        /// automated control.</remarks>
        [AssetMetadata("Default AI Controller", "The default AI controller class for the sprite.",
            AePropertyEditorGroup.AI, AePropertyEditorType.SingleSpritePicker,
            applicableTo: [typeof(AeSpriteInteractive)], requireAssignableFrom: typeof(AeAIStateMachine))]
        public string? DefaultAIController { get; set; }

        #region InteractiveSpriteMetadata

        /// <summary>
        /// Gets or sets the orientation behavior for the attached sprite relative to its owner.
        /// </summary>
        /// <remarks>Set this property in the owning sprite's metadata to control whether the attachment
        /// maintains a fixed orientation relative to its owner or operates independently. Use 'FixedToOwner' to keep
        /// the attachment aligned with the owner's rotation, or 'Independent' to allow the attachment its own
        /// orientation.</remarks>
        //TODO: THIS NEED A NEW EDITOR.
        //Note that these are meant to be set in the owning sprite's metadata, not in the attachment's own metadata.
        [AssetMetadata("Orientation Type", "Determines how the attached sprite orientation is affected by its owner. 'FixedToOwner' means the sprite will maintain a constant orientation relative to its owner, while 'Independent' allows the sprite to have its own orientation regardless of the owner's rotation.",
            AePropertyEditorGroup.Attachment, AePropertyEditorType.Enum,
            applicableTo: [typeof(AeSprite)], enumType: typeof(AeAttachmentOrientationType))]
        public AeAttachmentOrientationType? AttachmentOrientationType { get; set; }

        /// <summary>
        /// Gets or sets the position type that determines how the attached sprite's position is affected by its owner.
        /// </summary>
        /// <remarks>Set this property in the owning sprite's metadata to control whether the attachment
        /// maintains a fixed position relative to its owner or operates independently. Use 'FixedToOwner' to keep the
        /// attachment aligned with the owner's movement, or 'Independent' to allow the attachment to move
        /// separately.</remarks>
        //TODO: THIS NEED A NEW EDITOR.
        //Note that these are meant to be set in the owning sprite's metadata, not in the attachment's own metadata.
        [AssetMetadata("Position Type", "Determines how the attached sprite position is affected by its owner. 'FixedToOwner' means the sprite will maintain a constant position relative to its owner, while 'Independent' allows the sprite to have its own position regardless of the owner's movement.",
            AePropertyEditorGroup.Attachment, AePropertyEditorType.Enum,
            applicableTo: [typeof(AeSprite)], enumType: typeof(AeAttachmentPositionType))]
        public AeAttachmentPositionType? AttachmentPositionType { get; set; }

        /// <summary>
        /// Gets or sets the type of explosion effect applied to the sprite.
        /// </summary>
        [AssetMetadata("Explosion Type", "Determines the type of explosion effect for the sprite.",
            AePropertyEditorGroup.Destroy, AePropertyEditorType.Enum,
            applicableTo: [typeof(AeSprite)], enumType: typeof(AeExplosionType))]
        public AeExplosionType? ExplosionType { get; set; }

        /// <summary>
        /// Gets or sets the range specifying the number of particles generated when the sprite explodes.
        /// </summary>
        /// <remarks>If the value is null, no particle blast will occur on explosion. The range determines
        /// the minimum and maximum amount of particles created, allowing for variability in explosion
        /// effects.</remarks>
        [AssetMetadata("Particle Blast On Explode Amount", "Specifies the amount of particles generated when the sprite explodes.",
            AePropertyEditorGroup.Destroy, AePropertyEditorType.RangeInt,
            applicableTo: [typeof(AeSprite)])]
        public AeRange<int>? ParticleBlastOnExplodeAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the sprite should fragment when it explodes.
        /// </summary>
        [AssetMetadata("Fragment On Explode", "Indicates whether the sprite should fragment upon explosion.",
            AePropertyEditorGroup.Destroy, AePropertyEditorType.Boolean,
            applicableTo: [typeof(AeSprite)])]
        public bool? FragmentOnExplode { get; set; }

        /// <summary>
        /// Gets or sets the intensity of the screen shake effect triggered when the sprite explodes.
        /// </summary>
        /// <remarks>A higher value results in a more pronounced screen shake. This property is applicable
        /// only to sprites that support explosion effects.</remarks>
        [AssetMetadata("Screen Shake On Explode Amount", "Specifies the intensity of screen shake when the sprite explodes.",
            AePropertyEditorGroup.Destroy, AePropertyEditorType.RangeInt,
            applicableTo: [typeof(AeSprite)])]
        public AeRange<int>? ScreenShakeOnExplodeAmount { get; set; }

        /// <summary>
        /// Gets or sets the speed range for the sprite.
        /// </summary>
        [AssetMetadata("Speed", "The speed of the sprite.", AePropertyEditorGroup.Momentum, AePropertyEditorType.RangeFloat,
            applicableTo: [typeof(AeSprite)])]
        public AeRange<float>? Speed { get; set; }

        /// <summary>
        /// Gets or sets the maximum throttle value for the sprite.
        /// </summary>
        [AssetMetadata("Max Throttle", "The maximum throttle of the sprite.", AePropertyEditorGroup.Momentum, AePropertyEditorType.Float,
            applicableTo: [typeof(AeSprite)])]
        public float? MaxThrottle { get; set; }

        /// <summary>
        /// Gets or sets the throttle of the sprite, representing its current speed as a percentage of its maximum
        /// speed.
        /// </summary>
        [AssetMetadata("Throttle", "The throttle of the sprite, which determines its current speed as a percentage of its maximum speed.",
            AePropertyEditorGroup.Momentum, AePropertyEditorType.Float,
            applicableTo: [typeof(AeSprite)])]
        public float? Throttle { get; set; }

        /// <summary>
        /// How much does the sprite weigh? Used in physics calculations when MunitionDetection == true.
        /// A higher mass will make the sprite more resistant to acceleration and deceleration,
        /// while a lower mass will make it more agile but also more susceptible to external forces.
        /// 
        /// Mass == 0 is infinite mass, it cannot be moved by inertial forces.
        /// </summary>
        [AssetMetadata("Mass", "The mass of the sprite, which affects its resistance to acceleration and deceleration in physics calculations when munition detection is enabled. A higher mass makes the sprite more resistant to external forces, while a lower mass makes it more agile but also more susceptible to being moved by such forces. A mass of 0 is considered infinite mass, meaning the sprite cannot be moved by inertial forces.",
            AePropertyEditorGroup.Momentum, AePropertyEditorType.RangeFloat,
            applicableTo: [typeof(AeSpriteInteractive)])]
        public AeRange<float>? Mass { get; set; }

        /// <summary>
        /// How many hit points does the sprite have? When this reaches 0, the sprite is destroyed.
        /// </summary>
        [AssetMetadata("Hull", "The hull strength of the sprite, representing its hit points. When this reaches 0, the sprite is destroyed.",
            AePropertyEditorGroup.Health, AePropertyEditorType.Integer,
            applicableTo: [typeof(AeSprite)])]
        public int? Hull { get; set; }

        /// <summary>
        /// Gets or sets the current shield strength of the entity, representing its defensive capabilities.
        /// </summary>
        [AssetMetadata("Shields", "The shield strength of the sprite, representing its defensive capabilities.",
            AePropertyEditorGroup.Health, AePropertyEditorType.Integer,
            applicableTo: [typeof(AeSprite)])]
        public int? Shields { get; set; }

        /// <summary>
        /// Gets or sets the bounty amount associated with the entity. This probably will not survive the early access period.
        /// </summary>
        [AssetMetadata("Bounty", "The bounty amount associated with the sprite. This probably will not survive the early access period.",
            AePropertyEditorGroup.Health, AePropertyEditorType.Integer,
            applicableTo: [typeof(AeSprite)])]
        public int? Bounty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether munition detection is enabled (bullet detection).
        /// </summary>
        [AssetMetadata("Munition Detection", "Indicates whether munition detection (bullet detection) is enabled for the sprite.",
            AePropertyEditorGroup.Collision, AePropertyEditorType.Boolean,
            applicableTo: [typeof(AeSprite)])]
        public bool? MunitionDetection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether collision detection is enabled.
        /// </summary>
        /// <remarks>When set to <see langword="true"/>, the system checks for collisions between objects.
        /// Enabling collision detection may impact performance depending on the number of objects being
        /// monitored.</remarks>
        [AssetMetadata("Collision Detection", "Indicates whether collision detection is enabled for the sprite.",
            AePropertyEditorGroup.Collision, AePropertyEditorType.Boolean,
            applicableTo: [typeof(AeSpriteInteractive)])]
        public bool? CollisionDetection { get; set; }

        /// <summary>
        /// Gets or sets the augmentation factor applied to the collision polygon, affecting its size and shape during collision detection.
        /// </summary>
        /// <remarks>Adjust this value to fine-tune the collision area for the associated sprite.
        /// Increasing the factor enlarges the collision polygon, while decreasing it reduces the area considered for
        /// collisions. This can be useful for accommodating visual effects or gameplay balancing.</remarks>
        [AssetMetadata("Collision Polygon Augmentation", "The augmentation factor applied to the collision polygon, affecting its size and shape during collision detection.",
            AePropertyEditorGroup.Collision, AePropertyEditorType.Float,
            applicableTo: [typeof(AeSpriteInteractive)])]
        public float? CollisionPolyAugmentation { get; set; }

        /// <summary>
        /// Used for the players "primary weapon slot".
        /// </summary>
        [AssetMetadata("Primary Weapon", "The primary weapon assigned to the sprite.",
            AePropertyEditorGroup.Weapons, AePropertyEditorType.SingleSpritePicker,
            applicableTo: [typeof(AeSpriteInteractive)], requireAssignableFrom: typeof(AeSpriteWeapon))]
        public string? PrimaryWeaponAssetKey { get; set; }

        /// <summary>
        /// Gets or sets the collection of attachments associated with the sprite.
        /// </summary>
        /// <remarks>Attachments are intended to be managed within the sprite's metadata. Use this
        /// property to access or modify the list of attachments for a given sprite.</remarks>
        //TODO: THIS NEED A NEW EDITOR.
        //Note that these are meant to be set in the owning sprite's metadata, not in the attachment's own metadata.
        [AssetMetadata("Attachments", "The list of attachments for the sprite.",
            AePropertyEditorGroup.Attachment, AePropertyEditorType.MultipleSpritePicker,
            applicableTo: [typeof(AeSprite)], requireAssignableFrom: typeof(AeSpriteAttachment))]
        public List<AssetMetadata>? Attachments { get; set; }

        /// <summary>
        /// Gets or sets the collection of asset keys representing the available weapons for the sprite.
        /// </summary>
        /// <remarks>Each asset key corresponds to a weapon that can be assigned to the sprite. The list
        /// may be empty or null if no weapons are available.</remarks>
        [AssetMetadata("Weapon Assets", "The list of weapons for the sprite.",
            AePropertyEditorGroup.Weapons, AePropertyEditorType.MultipleSpritePicker,
            applicableTo: [typeof(AeSpriteInteractive)], requireAssignableFrom: typeof(AeSpriteWeapon))]
        public List<string>? WeaponAssetKeys { get; set; }

        #endregion

        #region InteractiveSpriteWeaponMetadata

        /// <summary>
        /// Gets or sets the number of munitions available for the weapon.
        /// </summary>
        [AssetMetadata("Munition Count", "The number of munitions available for the weapon.",
            AePropertyEditorGroup.Munitions, AePropertyEditorType.Integer,
            applicableTo: [typeof(AeSpriteWeapon)])]
        public int? MunitionCount { get; set; }

        #endregion

        #region SpriteAnimationMetadata

        /// <summary>
        /// Gets or sets the width, in pixels, of each frame in the sprite animation.
        /// </summary>
        [AssetMetadata("Frame Width", "The width of each frame in the sprite animation.",
            AePropertyEditorGroup.Animation, AePropertyEditorType.Integer,
            applicableTo: [typeof(AeSpriteAnimation)])]
        public int? FrameWidth { get; set; }

        /// <summary>
        /// Gets or sets the height, in pixels, of each frame in the sprite animation.
        /// </summary>
        [AssetMetadata("Frame Height", "The height of each frame in the sprite animation.",
            AePropertyEditorGroup.Animation, AePropertyEditorType.Integer,
            applicableTo: [typeof(AeSpriteAnimation)])]
        public int? FrameHeight { get; set; }

        /// <summary>
        /// Gets or sets the number of frames displayed per second in the sprite animation.
        /// </summary>
        [AssetMetadata("Frames Per Second", "The number of frames displayed per second in the sprite animation.",
            AePropertyEditorGroup.Animation, AePropertyEditorType.RangeFloat,
            applicableTo: [typeof(AeSpriteAnimation)])]
        public AeRange<float>? FramesPerSecond { get; set; }

        /// <summary>
        /// Gets or sets the play mode for the sprite animation.
        /// </summary>
        [AssetMetadata("Play Mode", "The play mode of the sprite animation.",
            AePropertyEditorGroup.Animation, AePropertyEditorType.Enum,
            applicableTo: [typeof(AeSpriteAnimation)], enumType: typeof(AeAnimationPlayMode))]
        public AeAnimationPlayMode? PlayMode { get; set; }

        #endregion

        #region WeaponMetadata

        /// <summary>
        /// If the sprite has an image, these are the paths to the bitmaps (be default, they are used at random)..
        /// </summary>
        [AssetMetadata("Munition Assets", "The munitions assets for the weapon.",
            AePropertyEditorGroup.Munitions, AePropertyEditorType.MultipleSpritePicker,
            applicableTo: [typeof(AeSpriteWeapon)],
            requireAssignableFrom: typeof(AeSpriteMunition))]
        public string[]? MunitionAssetKeys { get; set; }

        /// <summary>
        /// Gets or sets the keys of sound assets associated with the sprite.
        /// </summary>
        /// <remarks>Each key corresponds to a sound asset file used for audio playback with the sprite.
        /// The array may be null or empty if no sound assets are assigned.</remarks>
        [AssetMetadata("Sound Asset", "The sound asset file for the sprite.",
            AePropertyEditorGroup.Audio, AePropertyEditorType.MultipleSpritePicker,
            applicableTo: [typeof(AeSprite)], requireAssignableFrom: typeof(AeAudioClip))]
        public string[]? SoundAssetKeys { get; set; }

        /// <summary>
        /// The variance in degrees that the loaded munition will use for an initial heading angle.
        /// </summary>
        [AssetMetadata("Angle Variance", "The variance in degrees that the loaded munition will use for an initial heading angle.",
            AePropertyEditorGroup.Munitions, AePropertyEditorType.Float,
            applicableTo: [typeof(AeSpriteMunition)])]
        public float? AngleVarianceDegrees { get; set; }

        /// <summary>
        /// Gets or sets the delay, in milliseconds, between each shot fired by the weapon.
        /// </summary>
        [AssetMetadata("Fire Delay", "The delay in milliseconds between each shot fired by the weapon.",
            AePropertyEditorGroup.Munitions, AePropertyEditorType.Integer,
            applicableTo: [typeof(AeSpriteWeapon)])]
        public int? FireDelayMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets the range of damage dealt by the munition.
        /// </summary>
        [AssetMetadata("Damage", "The amount of damage dealt by the munition.",
            AePropertyEditorGroup.Munitions, AePropertyEditorType.RangeInt,
            applicableTo: [typeof(AeSpriteMunition)])]
        public AeRange<int>? Damage { get; set; }

        /// <summary>
        /// Gets or sets the maximum angle, in degrees, within which a target can be locked on.
        /// </summary>
        [AssetMetadata("Max Lock-On Angle", "The maximum angle, in degrees, within which a target can be locked on.",
            AePropertyEditorGroup.Weapons, AePropertyEditorType.Integer,
            applicableTo: [typeof(AeSpriteWeapon)])]
        public int? MaxLockOnAngle { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of targets that can be locked on at once.
        /// </summary>
        [AssetMetadata("Max Locks", "The maximum number of targets that can be locked on at once.",
            AePropertyEditorGroup.Weapons, AePropertyEditorType.Integer,
            applicableTo: [typeof(AeSpriteWeapon)])]
        public int? MaxLocks { get; set; }

        /// <summary>
        /// Gets or sets the minimum distance, in units, required to initiate a lock.
        /// </summary>
        [AssetMetadata("Min Lock Distance", "The minimum distance, in units, required to initiate a lock.",
            AePropertyEditorGroup.Weapons, AePropertyEditorType.Float,
            applicableTo: [typeof(AeSpriteWeapon)])]
        public float? MinLockDistance { get; set; }

        /// <summary>
        /// Gets or sets the maximum distance, in units, required to initiate a lock.
        /// </summary>
        [AssetMetadata("Max Lock Distance", "The maximum distance, in units, required to initiate a lock.",
            AePropertyEditorGroup.Weapons, AePropertyEditorType.Float,
            applicableTo: [typeof(AeSpriteWeapon)])]
        public float? MaxLockDistance { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the munition explodes upon impact.
        /// </summary>
        [AssetMetadata("Explodes On Impact", "Indicates whether the munition explodes on impact.",
            AePropertyEditorGroup.Munitions, AePropertyEditorType.Boolean,
            applicableTo: [typeof(AeSpriteMunition)])]
        public bool? ExplodesOnImpact { get; set; }

        /// <summary>
        /// The viewing angle that the munition will use for seeking/locking.
        /// </summary>
        [AssetMetadata("Seeking Escape Angle", "The viewing angle that the munition will use for seeking/locking.",
            AePropertyEditorGroup.Munitions, AePropertyEditorType.Integer,
            applicableTo: [typeof(AeSpriteMunition)])]
        public int? SeekingEscapeAngleDegrees { get; set; }

        /// <summary>
        /// The viewing distance that the munition will use for seeking/locking.
        /// </summary>
        [AssetMetadata("Seeking Escape Distance", "The viewing distance that the munition will use for seeking/locking.",
            AePropertyEditorGroup.Munitions, AePropertyEditorType.Integer,
            applicableTo: [typeof(AeSpriteMunition)])]
        public int? SeekingEscapeDistance { get; set; }

        /// <summary>
        /// Rate in degrees that the munition will rotate towards it target.
        /// </summary>
        [AssetMetadata("Seeking Rotation Rate", "Rate in degrees that the munition will rotate towards it target.",
            AePropertyEditorGroup.Munitions, AePropertyEditorType.Float,
            applicableTo: [typeof(AeSpriteMunition)])]
        public float? SeekingRotationRateDegrees { get; set; }

        #endregion
    }
}
