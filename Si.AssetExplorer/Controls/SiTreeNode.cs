namespace Si.AssetExplorer.Controls
{
    internal class SiTreeNode : TreeNode
    {
        public SiTreeNodeType NodeType { get; set; } = SiTreeNodeType.Undefined;
        public string AssetKey { get; set; } = string.Empty;

        public SiTreeNode()
        {
        }

        public SiTreeNode(string name, string text, string assetKey, SiTreeNodeType nodeType)
            : base(text)
        {
            Name = name;
            NodeType = nodeType;
            AssetKey = assetKey;
        }
    }
}
