using static Si.Library.SiConstants;

namespace Si.AssetExplorer.Controls
{
    internal class SiTabPage
        : TabPage
    {
        public SiCodeEditor Editor { get; private set; }

        public string AssetKey { get; private set; }

        public SiTabPage(string assetKey, string codeText, SiCodeType codeType)
        {
            Text = assetKey.Split('/').Last();
            AssetKey = assetKey;
            Editor = new SiCodeEditor(this, codeType, codeText);
        }
    }
}
