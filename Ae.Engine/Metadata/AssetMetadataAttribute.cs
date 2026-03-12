using System;

namespace Ae.Engine.Metadata
{
    /// <summary>
    /// Specifies metadata for asset properties to support editor configuration, validation, and display customization.
    /// </summary>
    /// <remarks>Apply this attribute to a property to provide additional information such as a friendly name,
    /// description, editor type, value constraints, and pick lists. This metadata is used by asset editors and tools to
    /// determine how the property should be presented and validated. Only one instance can be applied to a property.
    /// Typical usage includes specifying value ranges, allowed types, or custom editors for asset-related
    /// properties.</remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AssetMetadataAttribute : Attribute
    {
        /// <summary>
        /// Gets the user-friendly display name associated with the object.
        /// </summary>
        public string FriendlyName { get; }

        /// <summary>
        /// Gets the textual description associated with the current instance.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the editor type associated with the property.
        /// </summary>
        public AePropertyEditorType EditorType { get; }

        /// <summary>
        /// Gets the editor group associated with the property.
        /// </summary>
        public AePropertyEditorGroup EditorGroup { get; }

        /// <summary>
        /// Gets the maximum allowable value for the operation or calculation, if specified.
        /// </summary>
        public double? MaxValue { get; } = null;

        /// <summary>
        /// Gets the minimum allowable value for the associated setting or parameter.
        /// </summary>
        public double? MinValue { get; } = null;

        /// <summary>
        /// Gets the maximum allowed length for the value, or null if no limit is specified.
        /// </summary>
        public int? MaxLength { get; } = null;

        /// <summary>
        /// Gets the type of the enumeration associated with this instance.
        /// </summary>
        public Type? EnumType { get; } = null;

        /// <summary>
        /// Gets the list of selectable items for the pick operation.
        /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the AssetMetadataAttribute class with metadata describing asset properties,
        /// including display name, description, editor configuration, and applicable constraints.
        /// </summary>
        /// <remarks>If constraints such as maxValue, minValue, or maxLength are not valid or not
        /// specified, they will be ignored. Use this constructor to provide detailed metadata for asset properties to
        /// support UI editing and validation scenarios.</remarks>
        /// <param name="friendlyName">The user-friendly name to display for the asset property.</param>
        /// <param name="description">A description of the asset property, providing additional context or usage information.</param>
        /// <param name="editorGroup">The editor group that categorizes the property for UI organization.</param>
        /// <param name="editorType">The editor type used to render the property in the UI.</param>
        /// <param name="applicableTo">An array of types to which this metadata applies. Can be null if not restricted.</param>
        /// <param name="maxValue">The maximum allowed value for the property, if applicable. Must be greater than minValue and positive to be
        /// used.</param>
        /// <param name="minValue">The minimum allowed value for the property, if applicable. Must be non-negative and less than maxValue to be
        /// used.</param>
        /// <param name="maxLength">The maximum length allowed for the property value, if applicable. Must be positive to be used.</param>
        /// <param name="enumType">The enum type to use for the property, if the property represents an enumeration. Can be null if not
        /// applicable.</param>
        /// <param name="pickList">A list of allowed values for the property, used for pick-list editors. Can be null if not applicable.</param>
        /// <param name="requireAssignableFrom">A type that the property value must be assignable from, if applicable. Can be null if not required.</param>
        public AssetMetadataAttribute(string friendlyName, string description, AePropertyEditorGroup editorGroup, AePropertyEditorType editorType,
            Type[]? applicableTo, double maxValue = 0, double minValue = 0, int maxLength = 0, Type? enumType = null, string[]? pickList = null, Type? requireAssignableFrom = null)
        {
            FriendlyName = friendlyName;
            EditorGroup = editorGroup;
            Description = description;
            EditorType = editorType;
            MaxValue = maxValue > 0 && maxValue > minValue ? MaxValue : null;
            MinValue = minValue >= 0 && maxValue >= 0 && maxValue > minValue ? minValue : null;
            MaxLength = maxLength > 0 ? maxLength : null;
            EnumType = enumType;
            PickList = pickList;
            RequireAssignableFrom = requireAssignableFrom;
            ApplicableTo = applicableTo;
        }
    }
}
