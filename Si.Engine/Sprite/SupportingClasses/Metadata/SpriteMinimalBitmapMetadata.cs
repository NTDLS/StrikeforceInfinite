using Si.Engine.Sprite.SupportingClasses.Metadata._Superclass;

namespace Si.Engine.Sprite.SupportingClasses.Metadata
{
    internal class SpriteMinimalBitmapMetadata
        : MetadataBase
    {
        public SpriteMinimalBitmapMetadata() { }

        public float Speed { get; set; } = 1f;
        public float MaxThrottle { get; set; } = 0f;
        public float Throttle { get; set; } = 1f;
    }
}
