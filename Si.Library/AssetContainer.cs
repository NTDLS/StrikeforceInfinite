namespace Si.Library
{
    public class AssetContainer
    {
        public string Key { get; set; }
        public AssetMetadata Metadata { get; set; }
        public object Object { get; set; }
        public string BaseType { get; set; } = string.Empty;

        public AssetContainer(string key, string baseType, AssetMetadata metadata, object obj)
        {
            Key = key;
            BaseType = baseType;
            Metadata = metadata;
            Object = obj;
        }
    }
}
