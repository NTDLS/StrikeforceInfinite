using static Ae.Library.AeConstants;

namespace Ae.AssetExplorer.Controls
{
    internal class AeTabPage
        : TabPage
    {
        public AeCodeEditor Editor { get; private set; }

        public string AssetKey { get; private set; }

        public AeTabPage(string assetKey, string codeText, AeCodeType codeType)
        {
            Text = assetKey.Split('/').Last();
            AssetKey = assetKey;
            Editor = new AeCodeEditor(this, codeType, codeText);
        }
    }
}
