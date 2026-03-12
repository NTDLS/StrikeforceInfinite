namespace Ae.Engine.Metadata
{
    /// <summary>
    /// Represents a container for an asset, including its key, metadata, data, and associated information.
    /// </summary>
    /// <remarks>The asset data can represent various file types, such as images, text files, or audio files.
    /// The container also stores metadata and information used for asset handling and display. Use this class to
    /// encapsulate asset-related details for storage or processing.</remarks>
    public class AssetContainer
    {
        /// <summary>
        /// Gets or sets the unique identifier associated with the current instance.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the metadata associated with the asset.
        /// </summary>
        public AssetMetadata Metadata { get; set; }

        /// <summary>
        /// The byte data of the asset (png, txt, wav, etc.).
        /// </summary>
        public object Object { get; set; }

        /// <summary>
        /// The extension of the file that was added as an asset. (png, txt, wav, etc.)
        /// This is used to determine how the asset should be handled and displayed in the UI.
        /// </summary>
        public string BaseType { get; set; } = string.Empty;

        /// <summary>
        /// Name of the dynamically compiled controller class, if applicable.
        /// </summary>
        public string? ControllerName { get; set; }

        /// <summary>
        /// Initializes a new instance of the AssetContainer class with the specified asset key, base type, metadata,
        /// and object reference.
        /// </summary>
        /// <param name="key">The unique identifier for the asset. Cannot be null or empty.</param>
        /// <param name="baseType">The base type name associated with the asset. Used to categorize or identify the asset's type.</param>
        /// <param name="metadata">The metadata describing the asset. Provides additional information such as version, tags, or custom
        /// attributes.</param>
        /// <param name="obj">The object instance representing the asset's content. Can be any type relevant to the asset.</param>
        public AssetContainer(string key, string baseType, AssetMetadata metadata, object obj)
        {
            Key = key;
            BaseType = baseType;
            Metadata = metadata;
            Object = obj;
        }
    }
}
