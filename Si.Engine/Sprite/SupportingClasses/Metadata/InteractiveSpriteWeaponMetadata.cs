using Si.Engine.Sprite.SupportingClasses.Metadata._Superclass;
using System;

namespace Si.Engine.Sprite.SupportingClasses.Metadata
{
    public class InteractiveSpriteWeaponMetadata
        : MetadataBase
    {
        public InteractiveSpriteWeaponMetadata() { }

        public string? Type { get; set; }
        public int MunitionCount { get; set; }

        public InteractiveSpriteWeaponMetadata(Type type, int munitionCount)
        {
            Type = type.Name;
            MunitionCount = munitionCount;
        }
    }
}
