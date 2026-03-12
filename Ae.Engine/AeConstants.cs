using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace Ae.Engine
{
    /// <summary>
    /// Represents a method that handles writing a log message with a specified severity level and optional asset key.
    /// </summary>
    /// <remarks>Use this delegate to provide custom logging behavior for messages at various severity levels.
    /// The asset key can be used to correlate log entries with specific assets when applicable.</remarks>
    /// <param name="message">The log message to write. Cannot be null.</param>
    /// <param name="level">The severity level of the log message.</param>
    /// <param name="assetKey">An optional key identifying the related asset. If null, the log entry is not associated with a specific asset.</param>
    public delegate void WriteLogDelegate(string message, AeLoggingLevel level, string? assetKey = null);

    /// <summary>
    /// Provides application-wide constants and utility members for the Axis Engine.
    /// </summary>
    /// <remarks>This class contains string, numeric, and type constants, as well as helper methods and
    /// configuration options used throughout the Axis Engine. It is intended for use by consumers who need access to
    /// standard values or supported asset types. The class cannot be instantiated.</remarks>
    public static class AeConstants
    {
        /// <summary>
        /// Represents the user-friendly display name for the Axis Engine.
        /// </summary>
        public const string FriendlyName = "Axis Engine";

        internal static Lock SharedLock { get; private set; } = new Lock();
        internal const string MultiplayServerAddress = "127.0.0.1";
        internal const int MultiplayServerTCPPort = 6785;
        internal const int MinimumCompressionRatio = 1;
        internal static readonly string[] ImageTypes = ["png", "bmp"];

        /// <summary>
        /// Provides a mapping of common file extensions to their corresponding base asset types.
        /// </summary>
        /// <remarks>This dictionary can be used to determine the base asset type for a given file
        /// extension, such as "png" for images or "wav" for sounds. The mapping is case-sensitive and only includes a
        /// predefined set of extensions.</remarks>
        public static readonly Dictionary<string, AeBaseAssetType> BaseAssetTypes = new()
        {
            ["png"] = AeBaseAssetType.Image,
            ["wav"] = AeBaseAssetType.Sound,
            ["cs"] = AeBaseAssetType.Code,
            ["json"] = AeBaseAssetType.Text,
            ["xml"] = AeBaseAssetType.Text,
            ["txt"] = AeBaseAssetType.Text
        };

        /// <summary>
        /// Builds a file filter string suitable for use with file open dialogs, including filters for all supported
        /// asset types and a general 'All Files' option.
        /// </summary>
        /// <remarks>The returned filter string can be assigned to the Filter property of an
        /// OpenFileDialog to allow users to select files of supported types. Each filter entry groups files by asset
        /// type and extension.</remarks>
        /// <returns>A filter string that lists supported asset types by file extension, formatted for use with file open
        /// dialogs. The string includes an 'All Files' filter as the final option.</returns>
        public static string GetSupportedOpenFileFilterString()
        {
            var filter = new StringBuilder();

            foreach (var assetTypes in AeConstants.BaseAssetTypes.GroupBy(o => o.Value))
            {
                filter.Append($"{assetTypes.Key.ToString()} files");
                filter.Append(" (" + string.Join(", ", assetTypes.Select(t => $"*.{t.Key}")) + ")|");
                filter.Append(string.Join(";", assetTypes.Select(t => $"*.{t.Key}")) + "|");
            }
            filter.Append("All Files (*.*)|*.*");

            return filter.ToString();
        }

        private static JsonSerializerOptions? _JsonSerializationOptions;
        /// <summary>
        /// Gets the default options used for JSON serialization and deserialization throughout the application.
        /// </summary>
        /// <remarks>The returned options configure serialization to ignore null values, write indented
        /// JSON, and serialize enums as strings. The same instance is reused for all operations, ensuring consistent
        /// behavior.</remarks>
        public static JsonSerializerOptions JsonSerializerOptions
        {
            get
            {
                if (_JsonSerializationOptions == null)
                {
                    lock (SharedLock)
                    {
                        if (_JsonSerializationOptions == null)
                        {
                            _JsonSerializationOptions = new()
                            {
                                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                                WriteIndented = true,
                            };
                            _JsonSerializationOptions.Converters.Add(new JsonStringEnumConverter());
                        }
                    }
                }
                return _JsonSerializationOptions;
            }
        }

        /// <summary>
        /// Provides predefined mass constants representing commonly used mass categories in arbitrary units.
        /// </summary>
        /// <remarks>These constants can be used to standardize mass values across the application, such
        /// as for categorizing objects or specifying default mass values. The units are application-defined and
        /// intended for relative comparison rather than representing a specific measurement system.</remarks>
        public static class AeMass
        {
            /// <summary>
            /// Very small mass, suitable for lightweight objects or particles. Represents a mass that is negligible in most calculations.
            /// </summary>
            public const float Minuscule = 0.1f;
            /// <summary>
            /// Represents a small constant value of 1.0.
            /// </summary>
            public const float Tiny = 1f;
            /// <summary>
            /// Represents a Small constant value of 10.0.
            /// </summary>
            public const float Small = 10f;
            /// <summary>
            /// Represents a Medium constant value of 10.0.
            /// </summary>
            public const float Medium = 100f;
            /// <summary>
            /// Represents a Large constant value of 10.0.
            /// </summary>
            public const float Large = 1000f;
            /// <summary>
            /// Represents a Huge constant value of 10.0.
            /// </summary>
            public const float Huge = 10000f;
        }
    }
}
