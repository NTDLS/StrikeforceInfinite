using Ae.AssetExplorer.Controls;
using Ae.AssetExplorer.Forms;
using Ae.AssetExplorer.Properties;
using Ae.Engine;
using Ae.Library;
using System.Text.Json;
using Talkster.Client.Controls;

namespace Ae.AssetExplorer
{
    internal class TreeManager
    {
        private readonly DoubleBufferedTreeView _treeView;
        private readonly AeEngine _engine;
        private readonly Action<string, LoggingLevel?> _writeOutput;
        private readonly Action<AeTreeNode> _loadSelectedTreeNode;

        private readonly Dictionary<string, Image> AssetTypeImages = new()
        {
            ["png"] = Resources.AssetTypeImage,
            ["wav"] = Resources.AssetTypeSound,
            ["cs"] = Resources.AssetTypeCode,
            ["json"] = Resources.AssetTypeJson,
            ["xml"] = Resources.AssetTypeXml,
            ["txt"] = Resources.AssetTypeText
        };

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

            _treeView.ImageList = new ImageList()
            {
                ColorDepth = ColorDepth.Depth32Bit
            };

            _treeView.ImageList.Images.Add("folder", Resources.AssetTypeFolder);
            _treeView.ImageList.Images.Add("generic", Resources.AssetTypeGeneric);
            foreach (var item in AssetTypeImages)
            {
                _treeView.ImageList.Images.Add(item.Key, item.Value);
            }
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
                        var createMenu = new ToolStripMenuItem("Create");
                        createMenu.DropDownItems.Add("Folder", Resources.AssetTypeFolder, (s, e) => CreateFolder(node));
                        menu.Items.Add(new ToolStripSeparator());
                        createMenu.DropDownItems.Add("Text file", Resources.AssetTypeText, (s, e) => CreateFile(node, "txt"));
                        createMenu.DropDownItems.Add("JSON file", Resources.AssetTypeJson, (s, e) => CreateFile(node, "json"));
                        createMenu.DropDownItems.Add("XML file", Resources.AssetTypeXml, (s, e) => CreateFile(node, "xml"));
                        createMenu.DropDownItems.Add("Code file", Resources.AssetTypeCode, (s, e) => CreateFile(node, "cs"));
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
            try
            {
                using var form = new FormGetNewAssetName();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var newAssetKey = $"{node.AssetKey}/{form.AssetName}".Trim('/');

                    _engine.Assets.WriteEmptyAsset(newAssetKey, assetBaseType);

                    var asset = _engine.Assets.GetAsset(newAssetKey);

                    var newNode = UpsertTreeNodesPath(asset);
                    _treeView.SelectedNode = newNode;
                }
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void CreateFolder(AeTreeNode node)
        {
            try
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
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
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

        private AeTreeNode? UpsertTreeNodesPath(AssetContainer asset)
        {
            try
            {
                if (_treeView.InvokeRequired)
                {
                    _treeView.Invoke(new Func<AssetContainer, AeTreeNode?>(UpsertTreeNodesPath), asset);
                    return null;
                }

                var parts = asset.Key.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries);

                TreeNodeCollection workingLevel = _treeView.Nodes;

                int depthCounter = 0;

                AeTreeNode? lastNodeCreated = null;

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

                        var newNode = new AeTreeNode(part, displayName,
                            nodeType == AeTreeNodeType.Folder ? part : asset.Key, // For folders, the asset key is the path part. For assets, it's the full asset key.
                            nodeType);

                        switch (nodeType)
                        {
                            case AeTreeNodeType.Folder:
                                newNode.ImageKey = "folder";
                                break;
                            case AeTreeNodeType.Asset:
                                if (AssetTypeImages.ContainsKey(asset.BaseType))
                                {
                                    //If we have a specific image for this asset type, use it.
                                    newNode.ImageKey = asset.BaseType;
                                }
                                else
                                {
                                    newNode.ImageKey = "generic";
                                }
                                break;
                        }

                        newNode.SelectedImageKey = newNode.ImageKey;
                        workingLevel.Add(newNode);
                        workingLevel = newNode.Nodes;

                        lastNodeCreated = newNode;
                    }

                    depthCounter++;
                }

                return lastNodeCreated;
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
            return null;
        }
    }
}
