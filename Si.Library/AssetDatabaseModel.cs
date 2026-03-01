namespace Si.Library
{
    public class AssetDatabaseModel
    {
        public string Key { get; set; } = string.Empty;
        public string BaseType { get; set; } = string.Empty;
        public byte[] Bytes { get; set; } = [];
        public bool IsCompressed { get; set; }
        public string Metadata { get; set; } = string.Empty;
    }
}
