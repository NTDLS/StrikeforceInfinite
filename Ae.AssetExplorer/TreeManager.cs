using Ae.AssetExplorer.Controls;
using Ae.AssetExplorer.Forms;
using Ae.Engine;
using Ae.Library;
using System.Text;
using System.Text.Json;
using Talkster.Client.Controls;

namespace Ae.AssetExplorer
{
    internal class TreeManager
    {
        public readonly DoubleBufferedTreeView _treeView;
        private readonly AeEngine _engine;
        public readonly Action<string, LoggingLevel?> _writeOutput;
        public readonly Action<AeTreeNode> _loadSelectedTreeNode;

        public TreeManager(DoubleBufferedTreeView treeView, AeEngine engine,
            Action<string, LoggingLevel?> writeOutput,
            Action<AeTreeNode> loadSelectedTreeNode)
        {
            _engine = engine;
            _treeView = treeView;
            _writeOutput = writeOutput;
            _loadSelectedTreeNode = loadSelectedTreeNode;

            _treeView.NodeMouseDoubleClick += TreeView_NodeMouseDoubleClick;
            _treeView.NodeMouseClick += TreeView_NodeMouseClick;
        }

        private void TreeView_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right && e.Node is AeTreeNode node)
                {
                    _treeView.SelectedNode = node;

                    var menu = new ContextMenuStrip();

                    if (node.NodeType == AeTreeNodeType.Asset)
                    {
                        menu.Items.Add("Replace", null, (s, e) => ReplaceAsset(node));
                        menu.Items.Add("Export", null, (s, e) => ExportAsset(node, false));
                        menu.Items.Add("Export with Metadata", null, (s, e) => ExportAsset(node, true));
                        menu.Items.Add("Delete", null, (s, e) => DeleteAsset(node));
                    }
                    else if (node.NodeType == AeTreeNodeType.Folder)
                    {
                        menu.Items.Add("Create", null, (s, e) => CreateFolder(node));

                        var createMenu = new ToolStripMenuItem("Create");
                        createMenu.DropDownItems.Add("Folder", null, (s, e) => CreateFolder(node));
                        menu.Items.Add(new ToolStripSeparator());
                        createMenu.DropDownItems.Add("Text file", null, (s, e) => CreateFile(node, "txt"));
                        createMenu.DropDownItems.Add("JSON file", null, (s, e) => CreateFile(node, "json"));
                        createMenu.DropDownItems.Add("XML file", null, (s, e) => CreateFile(node, "xml"));
                        createMenu.DropDownItems.Add("Code file", null, (s, e) => CreateFile(node, "cs"));
                        //menu.Items.Add(new ToolStripSeparator());
                        //createMenu.DropDownItems.Add("Sprite", null, (s, e) => CreateFile(node, "cs"));
                        menu.Items.Add(createMenu);
                    }

                    menu.Show(_treeView, e.Location);
                }
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }
        private void CreateFile(AeTreeNode node, string assetBaseType)
        {
            using var form = new FormGetNewAssetName();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var newAssetKey = $"{node.AssetKey}/{form.AssetName}".Trim('/');

                _engine.Assets.WriteEmptyAsset(newAssetKey, assetBaseType);

                var asset = _engine.Assets.GetAsset(newAssetKey);

                UpsertTreeNodesPath(asset);


                var newNode = new AeTreeNode(form.AssetName, form.AssetName, newAssetKey, AeTreeNodeType.Asset);
                node.Nodes.Add(newNode);
                node.Expand();
                _treeView.SelectedNode = newNode;
            }

            //WriteEmptyAsset

        }

        private void CreateFolder(AeTreeNode node)
        {
            using var form = new FormCreateFolder();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var newAssetKey = $"{node.AssetKey}/{form.FolderName}".Trim('/');

                var newNode = new AeTreeNode(form.FolderName, form.FolderName, newAssetKey, AeTreeNodeType.Folder);
                node.Nodes.Add(newNode);
                node.Expand();
                _treeView.SelectedNode = newNode;
            }
        }

        private void DeleteAsset(AeTreeNode node)
        {
            if (MessageBox.Show("Are you sure you want to delete this asset?",
                AeConstants.FriendlyName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _engine.Assets.DeleteAsset(node.AssetKey);
            }
        }

        private void ExportAsset(AeTreeNode node, bool exportMetadata)
        {
            try
            {
                var asset = _engine.Assets.GetAsset(node.AssetKey);
                var assetBytes = _engine.Assets.ReadAssetBytes(node.AssetKey);

                var asstKeyName = node.AssetKey.Split('/').Last();

                using var dialog = new SaveFileDialog
                {
                    Title = "Save Asset",
                    Filter = $"{asset.BaseType} File (*.{asset.BaseType})|*.{asset.BaseType}|All Files (*.*)|*.*",
                    FileName = $"{asstKeyName}.{asset.BaseType}",
                    DefaultExt = $"{asset.BaseType}",
                    AddExtension = true,
                    OverwritePrompt = true
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(dialog.FileName, assetBytes);
                    if (exportMetadata)
                    {
                        var metadataJson = JsonSerializer.Serialize(asset.Metadata, AeConstants.JsonSerializerOptions);
                        File.WriteAllText($"{dialog.FileName}.meta", metadataJson);
                    }
                }
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void ReplaceAsset(AeTreeNode node)
        {
            try
            {
                using var dialog = new OpenFileDialog
                {
                    Title = "Select File",
                    Filter = AeConstants.GetSupportedOpenFileFilterString(),
                    Multiselect = false
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    _engine.Assets.WriteAssetBytes(node.AssetKey, dialog.FileName);
                    _loadSelectedTreeNode(node);
                }
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void TreeView_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                var node = e.Node as AeTreeNode ?? throw new InvalidOperationException("Expected SiTreeNode type.");
                if (node.NodeType == AeTreeNodeType.Asset)
                {
                    _loadSelectedTreeNode(node);
                }
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void WriteOutput(string text, LoggingLevel? color = null)
            => _writeOutput(text, color);

        public void Repopulate()
        {
            try
            {
                WriteOutput("Populating assets.", LoggingLevel.Verbose);

                //Files and paths that contain "#" are for internal purposes and should not be shown in the editor.
                var assets = _engine.Assets.GetAssets()
                    .Where(o => o.Key.Contains('#') == false).ToList();

                WriteOutput($"Enumerating {assets.Count:n0} assets.", LoggingLevel.Verbose);

                foreach (var asset in assets)
                {
                    UpsertTreeNodesPath(asset);
                }

                WriteOutput($"Assets enumeration complete.", LoggingLevel.Verbose);

                ExpandRootNodes();
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void ExpandRootNodes()
        {
            try
            {
                if (_treeView.InvokeRequired)
                {
                    _treeView.Invoke(new Action(ExpandRootNodes));
                    return;
                }

                foreach (AeTreeNode node in _treeView.Nodes)
                {
                    node.Expand();
                }
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void UpsertTreeNodesPath(AssetContainer asset)
        {
            try
            {
                if (_treeView.InvokeRequired)
                {
                    _treeView.Invoke(new Action<AssetContainer>(UpsertTreeNodesPath), asset);
                    return;
                }

                var parts = asset.Key.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries);

                TreeNodeCollection workingLevel = _treeView.Nodes;

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
                        var nodeType = depthCounter == parts.Length - 1 ? AeTreeNodeType.Asset : AeTreeNodeType.Folder;

                        var displayName = part;

                        if (nodeType == AeTreeNodeType.Asset)
                        {
                            displayName = Path.GetFileNameWithoutExtension(part);
                        }

                        var newNode = new AeTreeNode(part, displayName, asset.Key, nodeType);
                        workingLevel.Add(newNode);
                        workingLevel = newNode.Nodes;
                    }

                    depthCounter++;
                }
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }
    }
}
