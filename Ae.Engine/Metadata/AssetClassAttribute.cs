using System;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Metadata
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class AssetClassAttribute : Attribute
    {
        public string FriendlyName { get; }
        public bool UserSelectable { get; }
        public string Description { get; }
        public AeBaseAssetType ApplicableTo { get; set; }

        public AssetClassAttribute(string friendlyName, string description, AeBaseAssetType applicableTo, bool userSelectable)
        {
            FriendlyName = friendlyName;
            Description = description;
            ApplicableTo = applicableTo;
            UserSelectable = userSelectable;
        }
    }
}
