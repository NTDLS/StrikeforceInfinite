namespace Ae.Engine.DataModels
{
    /// <summary>
    /// Model used for deserialization of the Assets table from the database.
    /// </summary>
    public class AssetDatabaseModel
    {
        /// <summary>
        /// AssetKey is the unique identifier for an asset. It is used to retrieve the asset from the database and to reference it in code.
        /// </summary>
        public string Key { get; set; } = string.Empty;
        /// <summary>
        /// The file extension of the asset. This is used to determine how to interpret the bytes of the asset and what type of asset it is.
        /// </summary>
        public string BaseType { get; set; } = string.Empty;
        /// <summary>
        /// Bytes for the asset.
        /// </summary>
        public byte[] Bytes { get; set; } = [];
        /// <summary>
        /// Serialized metadata for the asset.
        /// </summary>
        public string Metadata { get; set; } = string.Empty;
        /// <summary>
        /// Denotes whether the asset is stored in a compressed format. If true, the asset bytes should be decompressed before use.
        /// </summary>
        public bool IsCompressed { get; set; }
        /// <summary>
        /// For sprites, this is C# code that can be used to control the sprite's behavior. It is compiled and executed at runtime.
        /// </summary>
        public string? Controller { get; set; }
    }
}
