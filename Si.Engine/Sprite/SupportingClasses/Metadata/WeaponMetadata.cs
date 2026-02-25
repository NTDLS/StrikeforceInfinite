using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Si.Engine.Sprite.SupportingClasses.Metadata._Superclass;
using static Si.Library.SiConstants;

namespace Si.Engine.Sprite.SupportingClasses.Metadata
{
    /// <summary>
    /// Contains sprite metadata.
    /// </summary>
    public class WeaponMetadata
        : MetadataBase
    {
        public WeaponMetadata() { }

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

        public float Speed { get; set; } = 25;

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
    }
}
