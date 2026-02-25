using Newtonsoft.Json;
using Si.Engine.Sprite.SupportingClasses.Metadata._Superclass;
using Si.Library.Mathematics;
using System.Collections.Generic;

namespace Si.Engine.Sprite.SupportingClasses.Metadata
{
    public class InteractiveSpriteAttachmentMetadata
        : MetadataBase
    {
        public InteractiveSpriteAttachmentMetadata() { }

        public string Type { get; set; } = string.Empty;
        public float X { get; set; }
        public float Y { get; set; }

        [JsonIgnore]
        public SiVector LocationRelativeToOwner { get => new SiVector(X, Y); }

        public List<InteractiveSpriteWeaponMetadata> Weapons { get; set; } = new();
    }
}
