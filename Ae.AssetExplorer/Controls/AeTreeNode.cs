namespace Ae.AssetExplorer.Controls
{
    internal class AeTreeNode : TreeNode
    {
        public Guid UID { get; set; } = Guid.NewGuid();
        public AeTreeNodeType NodeType { get; set; } = AeTreeNodeType.Undefined;
        public string AssetKey { get; set; } = string.Empty;

        public AeTreeNode()
        {
        }

        public AeTreeNode(string name, string text, string assetKey, AeTreeNodeType nodeType)
            : base(text)
        {

            Name = name;
            NodeType = nodeType;
            AssetKey = assetKey;
        }
    }
}
