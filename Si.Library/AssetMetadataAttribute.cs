using static Si.Library.SiConstants;

namespace Si.Library
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AssetMetadataAttribute : Attribute
    {
        public string FriendlyName { get; }
        public string Description { get; }
        public PropertyEditorType EditorType { get; }
        public double? MaxValue { get; } = null;
        public double? MinValue { get; } = null;
        public int? MinLength { get; } = null;
        public Type? EnumType { get; } = null;


        public AssetMetadataAttribute(string friendlyName, string description, PropertyEditorType editorType,
            double maxValue = 0, double minValue = 0, int minLength = 0, Type? enumType = null)
        {
            FriendlyName = friendlyName;
            Description = description;
            EditorType = editorType;
            MaxValue = maxValue > 0 && maxValue > minValue ? MaxValue : null;
            MinValue = minValue >= 0 && maxValue >= 0 && maxValue > minValue ? minValue : null;
            MinLength = minLength > 0 ? minLength : null;
            EnumType = enumType;
        }
    }
}
