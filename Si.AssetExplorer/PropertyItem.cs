using Si.Library;
using Si.Library.Mathematics;
using System.Globalization;
using System.Reflection;

namespace Si.AssetExplorer
{
    public class PropertyItem
        : ListViewItem
    {
        /// <summary>
        /// Gets or sets the metadata attributes associated with the object.
        /// </summary>
        /// <remarks>This property allows for the specification of additional metadata that can influence
        /// the behavior of the object. It can be null if no attributes are assigned.</remarks>
        public AssetMetadataAttribute? Attributes { get; set; }

        /// <summary>
        /// Actual value of this meta data attribute.
        /// </summary>
        public object? WorkingValue { get; set; }
        public AssetMetadata MetaData { get; set; }
        public string PropertyName { get; set; }

        public PropertyItem(AssetMetadata metaData, string propertyName, ListViewGroup? group)
        {
            var propertyInfo = metaData.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.Name == propertyName)
                .Select(p => new
                {
                    Property = p,
                    MetadataAttribute = p.GetCustomAttribute<AssetMetadataAttribute>()
                })
                .Where(x => x.MetadataAttribute != null)
                .FirstOrDefault() ?? throw new ArgumentException($"Property '{propertyName}' not found in {metaData.GetType().Name} or does not have AssetMetadataAttribute.");

            if (propertyInfo.MetadataAttribute == null)
                throw new ArgumentException($"Property '{propertyName}' does not have AssetMetadataAttribute.");

            Group = group;
            MetaData = metaData;
            PropertyName = propertyName;
            WorkingValue = SiReflection.GetPropertyValue(metaData, propertyName);
            Attributes = propertyInfo.MetadataAttribute;
            Text = propertyInfo.MetadataAttribute.FriendlyName;
            SubItems.Add(PropertyStringifier(WorkingValue));
        }

        public static string? PropertyStringifier(object? value)
        {
            if (value is float f)
                return string.Format(CultureInfo.InvariantCulture, "{0:#,##0.#####}", f);
            else if (value is double d)
                return string.Format(CultureInfo.InvariantCulture, "{0:#,##0.#####}", d);
            else if (value is int i)
                return string.Format(CultureInfo.InvariantCulture, "{0:#,##0}", i);
            else if (value is long l)
                return string.Format(CultureInfo.InvariantCulture, "{0:#,##0}", l);
            else if (value is ulong r)
                return string.Format(CultureInfo.InvariantCulture, "{0:#,##0}", r);
            else if (value is string s)
                return s;
            else if (value is bool b)
                return b.ToString();
            else if (value is SiVector v)
                return v.ToString();
            else if (value is SiRange<int> ri)
                return ri.ToString();
            else if (value is SiRange<float> rf)
                return rf.ToString();
            else if (value is SiRange<double> rd)
                return rd.ToString();

            return value?.ToString();
        }
    }
}
