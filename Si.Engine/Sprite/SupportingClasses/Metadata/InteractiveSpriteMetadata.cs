using Si.Library;
using System.Collections.Generic;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.SupportingClasses.Metadata
{
    /// <summary>
    /// Contains sprite metadata.
    /// </summary>
    public class InteractiveSpriteMetadata
    {
        public InteractiveSpriteMetadata() { }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

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
        public InteractiveSpriteWeapon? PrimaryWeapon { get; set; }
        public List<InteractiveSpriteAttachment> Attachments { get; set; } = new();
        public List<InteractiveSpriteWeapon> Weapons { get; set; } = new();
    }
}
