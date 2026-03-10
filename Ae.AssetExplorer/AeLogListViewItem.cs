using static Ae.Engine.AeConstants;

namespace Ae.AssetExplorer
{
    internal class AeLogListViewItem
        : ListViewItem
    {
        public DateTime OccurrenceUtc { get; set; } = DateTime.UtcNow;
        public AeLoggingLevel LoggingLevel { get; }
        public string? AssetKey { get; set; }

        public AeLogListViewItem(AeLoggingLevel loggingLevel, string text, string? assetKey = null)
            : base([loggingLevel.ToString(), assetKey?.Split('/').Last() ?? string.Empty, text])
        {
            AssetKey = assetKey;
            LoggingLevel = loggingLevel;

            ForeColor = loggingLevel switch
            {
                AeLoggingLevel.Verbose => AssetExplorerColors.Verbose,
                AeLoggingLevel.Information => AssetExplorerColors.Information,
                AeLoggingLevel.Warning => AssetExplorerColors.Warning,
                AeLoggingLevel.Error => AssetExplorerColors.Error,
                _ => AssetExplorerColors.Default
            };
        }
    }
}
