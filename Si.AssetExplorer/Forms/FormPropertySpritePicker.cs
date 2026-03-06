using Si.AssetExplorer.Controls;
using Si.Engine;
using Si.Library;
using Si.Library.Metadata;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertySpritePicker
        : Form
    {
        private readonly EngineCore? _engine;
        private readonly bool _multiSelect;
        public List<string> Value => _selectedAssetKeys;
        private List<string> _selectedAssetKeys = new List<string>();

        public FormPropertySpritePicker()
        {
            InitializeComponent();
        }

        public FormPropertySpritePicker(EngineCore engine, PropertyItem propertyItem, bool multiSelect)
        {
            InitializeComponent();
            _engine = engine;
            _multiSelect = multiSelect;

            Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            treeViewAssets.CheckBoxes = _multiSelect;

            var selectedAssetKeys = new List<string>();

            if (propertyItem.WorkingValue is AssetMetadata asset && asset.AssetKey != null)
            {
                selectedAssetKeys.Add(asset.AssetKey);
            }
            else if (propertyItem.WorkingValue is List<AssetMetadata> assets)
            {
                foreach (var selectedAsset in assets)
                {
                    if (selectedAsset.AssetKey != null)
                    {
                        selectedAssetKeys.Add(selectedAsset.AssetKey);
                    }
                }
            }

            Repopulate(selectedAssetKeys);

            AcceptButton = buttonSave;
            CancelButton = buttonCancel;
        }

        public void Repopulate(List<string> selectedAssetKeys)
        {
            if (_engine == null) return;

            try
            {
                //Files and paths that contain "#" are for internal purposes and should not be shown in the editor.
                var assets = _engine.Assets.GetAssets()
                    .Where(o => o.Key.Contains('#') == false).ToList();

                foreach (var asset in assets)
                {
                    UpsertTreeNodesPath(asset, selectedAssetKeys);
                }

                TreeNode? firstSelected = null;

                if (selectedAssetKeys.Count > 0)
                {
                    GetCheckedNodes(treeViewAssets).ToList().ForEach(o =>
                    {
                        var node = o;

                        firstSelected ??= o;

                        //Expand all parent nodes of checked nodes to ensure visibility of checked nodes.
                        while (node != null)
                        {
                            node.Expand();
                            node = node.Parent;
                        }
                    });

                    treeViewAssets.SelectedNode = firstSelected;
                    firstSelected?.EnsureVisible();
                }
                else
                {
                    ExpandRootNodes();
                }
            }
            catch (Exception ex)
            {
                //_writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void UpsertTreeNodesPath(AssetContainer asset, List<string> selectedAssetKeys)
        {
            try
            {
                if (treeViewAssets.InvokeRequired)
                {
                    treeViewAssets.Invoke(new Action<AssetContainer, List<string>>(UpsertTreeNodesPath), asset, selectedAssetKeys);
                    return;
                }

                var parts = asset.Key.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries);

                TreeNodeCollection workingLevel = treeViewAssets.Nodes;

                int depthCounter = 0;

                foreach (var part in parts)
                {
                    var foundNode = workingLevel.Find(part, false);
                    if (foundNode.Length == 1)
                    {
                        workingLevel = foundNode.First().Nodes;
                    }
                    else
                    {
                        var nodeType = depthCounter == parts.Length - 1 ? SiTreeNodeType.Asset : SiTreeNodeType.Folder;

                        var displayName = part;

                        if (nodeType == SiTreeNodeType.Asset)
                        {
                            displayName = Path.GetFileNameWithoutExtension(part);
                        }

                        var newNode = new SiTreeNode(part, displayName, asset.Key, nodeType);
                        workingLevel.Add(newNode);
                        workingLevel = newNode.Nodes;

                        if (nodeType == SiTreeNodeType.Asset)
                        {
                            newNode.Checked = selectedAssetKeys.Contains(asset.Key);
                        }
                    }

                    depthCounter++;
                }
            }
            catch (Exception ex)
            {
                //_writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void ExpandRootNodes()
        {
            try
            {
                if (treeViewAssets.InvokeRequired)
                {
                    treeViewAssets.Invoke(new Action(ExpandRootNodes));
                    return;
                }

                foreach (SiTreeNode node in treeViewAssets.Nodes)
                {
                    node.Expand();
                }
            }
            catch (Exception ex)
            {
                //_writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        public static List<TreeNode> GetCheckedNodes(TreeView treeView)
        {
            var result = new List<TreeNode>();

            foreach (TreeNode node in treeView.Nodes)
            {
                CollectCheckedNodes(node, result);
            }

            static void CollectCheckedNodes(TreeNode node, List<TreeNode> result)
            {
                if (node.Checked)
                    result.Add(node);

                foreach (TreeNode child in node.Nodes)
                {
                    CollectCheckedNodes(child, result);
                }
            }

            return result;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (_multiSelect == true)
            {
                GetCheckedNodes(treeViewAssets).ToList().ForEach(o =>
                {
                    if (o is SiTreeNode node)
                    {
                        _selectedAssetKeys.Add(node.AssetKey);
                    }
                });
            }
            else
            {
                if (treeViewAssets.SelectedNode is not SiTreeNode node)
                {
                    MessageBox.Show("Please select an asset.", SiConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _selectedAssetKeys.Clear();
                _selectedAssetKeys.Add(node.AssetKey);
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
