using Si.AssetExplorer.Controls;
using Si.Engine;
using Si.Library;
using Talkster.Client.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Si.AssetExplorer
{
    internal class TreeManager
    {
        public readonly DoubleBufferedTreeView _treeView;
        private readonly EngineCore _engine;
        public readonly Action<string, LoggingLevel?> _writeOutput;
        public readonly Action<SiTreeNode> _loadSelectedTreeNode;

        public TreeManager(DoubleBufferedTreeView treeView, EngineCore engineCore,
            Action<string, LoggingLevel?> writeOutput,
            Action<SiTreeNode> loadSelectedTreeNode)
        {
            _engine = engineCore;
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
                if (e.Button == MouseButtons.Right && e.Node is SiTreeNode node)
                {
                    _treeView.SelectedNode = node;

                    var menu = new ContextMenuStrip();
                    menu.Items.Add("Replace Asset", null, (s, e) => ReplaceAsset(node));
                    menu.Items.Add("Export Asset", null, (s, e) => ExportAsset(node));
                    menu.Show(_treeView, e.Location);
                }
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void ExportAsset(SiTreeNode node)
        {
            try
            {
                var asset = _engine.Assets.GetAsset(node.AssetKey);
                var assetBytes = _engine.Assets.ReadRowAssetBytes(node.AssetKey);

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
                }
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void ReplaceAsset(SiTreeNode node)
        {
            try
            {
                using var dialog = new OpenFileDialog
                {
                    Title = "Select File",
                    Filter =
                    "Supported Files (*.wav;*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.txt;*.json)|*.wav;*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.txt;*.json|" +
                    "Wave Audio (*.wav)|*.wav|" +
                    "Images (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|" +
                    "Text Files (*.txt)|*.txt|" +
                    "JSON Files (*.json)|*.json|" +
                    "All Files (*.*)|*.*",
                    Multiselect = false
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //var pngBytes = EasyImage.LoadAnyToPngBytes(dialog.FileName);
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
                var node = e.Node as SiTreeNode ?? throw new InvalidOperationException("Expected SiTreeNode type.");
                if (node.NodeType == SiTreeNodeType.Asset)
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

                foreach (SiTreeNode node in _treeView.Nodes)
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
                        var nodeType = depthCounter == parts.Length - 1 ? SiTreeNodeType.Asset : SiTreeNodeType.Folder;

                        var displayName = part;

                        if (nodeType == SiTreeNodeType.Asset)
                        {
                            displayName = Path.GetFileNameWithoutExtension(part);
                        }

                        var newNode = new SiTreeNode(part, displayName, asset.Key, nodeType);
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
