using static Ae.Library.AeConstants;

namespace Ae.AssetExplorer.Controls
{
    internal class AeTabPage
        : TabPage
    {
        public AeCodeEditor Editor { get; private set; }

        public string AssetKey { get; private set; }

        public AeBaseAssetType BaseType { get; private set; }

        public AeTabPage(string assetKey, string codeText, AeBaseAssetType baseType, AeCodeType codeType)
        {
            Text = assetKey.Split('/').Last();
            AssetKey = assetKey;
            BaseType = baseType;
            Editor = new AeCodeEditor(this, codeType, codeText);
        }
    }
}
