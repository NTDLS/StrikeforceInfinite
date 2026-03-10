using System;
using static Ae.Engine.AeConstants;

namespace Ae.Engine.Metadata
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AssetMetadataAttribute : Attribute
    {
        public string FriendlyName { get; }
        public string Description { get; }
        public AePropertyEditorType EditorType { get; }
        public AePropertyEditorGroup EditorGroup { get; }
        public double? MaxValue { get; } = null;
        public double? MinValue { get; } = null;
        public int? MinLength { get; } = null;
        public Type? EnumType { get; } = null;
        public string[]? PickList { get; } = null;

        /// <summary>
        /// Gets an array of types that this member is applicable to.
        /// </summary>
        /// <remarks>This property provides a list of types that can utilize the functionality defined by
        /// this member. It is useful for determining the context in which this member can be effectively
        /// 
        /// NULL when there are no specific type restrictions, meaning that the member is applicable to all types.
        /// applied.</remarks>
        public Type[]? ApplicableTo { get; }

        /// <summary>
        /// When specified, indicates that the property value must be assignable from the specified type.
        /// This is used for when the property is of an asset type and we want to restrict the types of assets that can be assigned to it.
        /// </summary>
        public Type? RequireAssignableFrom { get; }

        public AssetMetadataAttribute(string friendlyName, string description, AePropertyEditorGroup editorGroup, AePropertyEditorType editorType,
            Type[]? applicableTo, double maxValue = 0, double minValue = 0, int minLength = 0, Type? enumType = null, string[]? pickList = null, Type? requireAssignableFrom = null)
        {
            FriendlyName = friendlyName;
            EditorGroup = editorGroup;
            Description = description;
            EditorType = editorType;
            MaxValue = maxValue > 0 && maxValue > minValue ? MaxValue : null;
            MinValue = minValue >= 0 && maxValue >= 0 && maxValue > minValue ? minValue : null;
            MinLength = minLength > 0 ? minLength : null;
            EnumType = enumType;
            PickList = pickList;
            RequireAssignableFrom = requireAssignableFrom;
            ApplicableTo = applicableTo;
        }
    }
}
