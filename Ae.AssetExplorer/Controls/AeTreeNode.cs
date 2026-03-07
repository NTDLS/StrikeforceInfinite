namespace Ae.AssetExplorer.Controls
{
    internal class AeTreeNode : TreeNode
    {
        public SiTreeNodeType NodeType { get; set; } = SiTreeNodeType.Undefined;
        public string AssetKey { get; set; } = string.Empty;

        public AeTreeNode()
        {
        }

        public AeTreeNode(string name, string text, string assetKey, SiTreeNodeType nodeType)
            : base(text)
        {
            Name = name;
            NodeType = nodeType;
            AssetKey = assetKey;
        }
    }
}
