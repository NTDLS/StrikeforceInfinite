using static Si.Library.SiConstants;

namespace Si.Library.Metadata
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AssetMetadataAttribute : Attribute
    {
        public string FriendlyName { get; }
        public string Description { get; }
        public PropertyEditorType EditorType { get; }
        public PropertyEditorGroup EditorGroup { get; }
        public double? MaxValue { get; } = null;
        public double? MinValue { get; } = null;
        public int? MinLength { get; } = null;
        public Type? EnumType { get; } = null;
        public string[]? PickList { get; } = null;
        //public Type[] ApplicipableTo { get; }

        public AssetMetadataAttribute(string friendlyName, string description, PropertyEditorGroup editorGroup, PropertyEditorType editorType,
            /*Type[] applicableTo,*/ double maxValue = 0, double minValue = 0, int minLength = 0, Type? enumType = null, string[]? pickList = null)
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
            //ApplicipableTo = applicableTo;
        }
    }
}
