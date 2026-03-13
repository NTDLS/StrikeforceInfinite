using Ae.Engine;

namespace Ae.AssetExplorer.Controls
{
    internal class AeTabPage
        : TabPage
    {
        public AeCodeEditor EditorHost { get; private set; }

        public string AssetKey { get; private set; }

        public AeBaseAssetType BaseType { get; private set; }

        private readonly FormMain _formMain;

        public AeTabPage(FormMain formMain, string assetKey, string codeText, AeBaseAssetType baseType, AeCodeType codeType)
        {
            _formMain = formMain;
            Text = assetKey.Split('/').Last();
            AssetKey = assetKey;
            BaseType = baseType;
            EditorHost = new AeCodeEditor(this, formMain, codeType, codeText);
        }
    }
}
