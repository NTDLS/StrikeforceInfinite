using Ae.Library.Metadata;

namespace Ae.Library
{
    public class AssetContainer
    {
        public string Key { get; set; }
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

        public AssetContainer(string key, string baseType, AssetMetadata metadata, object obj)
        {
            Key = key;
            BaseType = baseType;
            Metadata = metadata;
            Object = obj;
        }
    }
}
