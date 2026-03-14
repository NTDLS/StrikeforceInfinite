using Ae.AssetExplorer.Controls;
using Ae.AssetExplorer.Forms;
using Ae.AssetExplorer.Properties;
using Ae.Engine;
using Ae.Engine.Metadata;
using System.Text;
using System.Text.Json;
using Talkster.Client.Controls;

namespace Ae.AssetExplorer
{
    internal class TreeManager
    {
        private readonly DoubleBufferedTreeView _treeView;
        private readonly AeEngine _engine;
        private readonly WriteLogDelegate _writeLog;
        private readonly Action<AeTreeNode> _loadSelectedTreeNode;
        private readonly AeTreeNode _rootNode;

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
            WriteLogDelegate writeLog,
            Action<AeTreeNode> loadSelectedTreeNode)
        {
            _engine = engine;
            _treeView = treeView;
            _writeLog = writeLog;
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

            _rootNode = new AeTreeNode("Assets", "Assets", "", AeTreeNodeType.Folder)
            {
                ImageKey = "folder",
                SelectedImageKey = "folder"
            };
            _treeView.Nodes.Add(_rootNode);

            _debounceTimer.Interval = 300; // ms
            _debounceTimer.Tick += (object? sender, EventArgs e) =>
            {
                _debounceTimer.Stop();
                PerformSearch();
            };
        }

        #region Search Functionality.

        private readonly System.Windows.Forms.Timer _debounceTimer = new();
        private readonly List<AeTreeNode> _detachedRoots = new();
        private string _currentSearchText = string.Empty;

        private void PerformSearch()
        {
            if (string.IsNullOrWhiteSpace(_currentSearchText))
            {
                ReattachRootNodes();
                return;
            }

            DetachRootNodes();

            _treeView.Nodes.Clear();

            var matches = FindMatchingNodes(_detachedRoots, _currentSearchText).ToList();

            foreach (var match in matches)
            {
                UpsertSearchTreeNode(match);
            }

            foreach (AeTreeNode node in _treeView.Nodes)
            {
                node.ExpandAll();
            }
        }

        private IEnumerable<AeTreeNode> FindMatchingNodes(IEnumerable<AeTreeNode> nodes, string searchText)
        {
            foreach (var node in nodes)
            {
                if (node.Text.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return node;
                }

                foreach (AeTreeNode child in node.Nodes)
                {
                    foreach (var match in FindMatchingNodes([child], searchText))
                    {
                        yield return match;
                    }
                }
            }
        }

        private AeTreeNode? FindNodeByUID(TreeNodeCollection nodes, Guid uid)
        {
            foreach (AeTreeNode node in nodes)
            {
                if (node.UID == uid)
                    return node;

                var found = FindNodeByUID(node.Nodes, uid);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private void UpsertSearchTreeNode(AeTreeNode originalNode)
        {
            try
            {
                if (_treeView.InvokeRequired)
                {
                    _treeView.Invoke(new Action<AeTreeNode>(UpsertSearchTreeNode), originalNode);
                    return;
                }

                var flatOriginalNodes = new List<AeTreeNode>();

                var node = originalNode as AeTreeNode;
                while (node != null)
                {
                    flatOriginalNodes.Add(node);
                    node = node.Parent as AeTreeNode;
                }

                flatOriginalNodes.Reverse();

                var nodeCollection = _treeView.Nodes;

                var touchedNodes = new List<AeTreeNode>();

                foreach (var flatNode in flatOriginalNodes)
                {
                    //Search on UID.
                    var foundNode = FindNodeByUID(nodeCollection, flatNode.UID);
                    if (foundNode == null)
                    {
                        var newNode = new AeTreeNode(flatNode.Name, flatNode.Text, flatNode.AssetKey, flatNode.NodeType)
                        {
                            ImageKey = flatNode.ImageKey,
                            SelectedImageKey = flatNode.SelectedImageKey,
                            UID = flatNode.UID
                        };
                        nodeCollection.Add(newNode);
                        nodeCollection = newNode.Nodes;
                        touchedNodes.Add(newNode);
                    }
                    else
                    {
                        touchedNodes.Add(foundNode);
                        nodeCollection = foundNode.Nodes;
                    }
                }
            }
            catch (Exception ex)
            {
                _writeLog?.Invoke($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        void DetachRootNodes()
        {
            if (_detachedRoots.Count > 0)
            {
                //If we already have detached nodes, we shouldn't try to detach again without reattaching first,
                //otherwise we might lose references to the detached nodes and end up with an empty tree view
                //with no way to get the nodes back.
                return;
            }

            _detachedRoots.Clear();

            foreach (AeTreeNode node in _treeView.Nodes)
            {
                _detachedRoots.Add(node);
            }

            _treeView.Nodes.Clear();
        }

        void ReattachRootNodes()
        {
            if (_detachedRoots.Count == 0)
            {
                //Make sure we have something to reattach before trying to do so,
                //otherwise we might end up with an empty tree view and no way to get the nodes back.
                return;
            }

            _treeView.BeginUpdate();
            _treeView.Nodes.Clear();

            foreach (var node in _detachedRoots)
            {
                _treeView.Nodes.Add(node);
            }

            _detachedRoots.Clear();

            _treeView.EndUpdate();
        }

        public void SearchTextChange(string text)
        {
            _currentSearchText = text;
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        #endregion

        public void HighlightItem(string assetKey)
        {
            if (_treeView.InvokeRequired)
            {
                _treeView.Invoke(new Action<string>(HighlightItem), assetKey);
                return;
            }

            var parts = assetKey.Split('/', StringSplitOptions.RemoveEmptyEntries);
            AeTreeNode? lastFoundNode = null;

            var workingLevel = _rootNode.Nodes;

            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                var foundNode = workingLevel.Find(part, false)?.FirstOrDefault() as AeTreeNode;

                if (foundNode == null)
                {
                    break;
                }

                sb.Append($"{part}/");

                lastFoundNode = foundNode;
                workingLevel = foundNode.Nodes;
            }

            if (lastFoundNode != null && sb.ToString().Trim('/').Equals(assetKey.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                _treeView.SelectedNode = lastFoundNode;
                lastFoundNode.EnsureVisible();
                _treeView.Focus();
                _loadSelectedTreeNode(lastFoundNode);
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

                    var enableAlterActions = _detachedRoots.Count == 0;

                    if (node.NodeType == AeTreeNodeType.Asset)
                    {
                        menu.Items.Add("Replace", null, (s, e) => ReplaceAsset(node));
                        menu.Items.Add("Export", null, (s, e) => ExportAsset(node, false));
                        menu.Items.Add("Export with Metadata", null, (s, e) => ExportAsset(node, true));
                        menu.Items.Add("Delete", null, (s, e) => DeleteAsset(node)).Enabled = enableAlterActions;
                    }
                    else if (node.NodeType == AeTreeNodeType.Folder)
                    {
                        var createMenu = new ToolStripMenuItem("Create");
                        createMenu.DropDownItems.Add("Folder", Resources.AssetTypeFolder, (s, e) => CreateFolder(node)).Enabled = enableAlterActions;
                        menu.Items.Add(new ToolStripSeparator());
                        createMenu.DropDownItems.Add("Text file", Resources.AssetTypeText, (s, e) => CreateFile(node, "txt")).Enabled = enableAlterActions;
                        createMenu.DropDownItems.Add("JSON file", Resources.AssetTypeJson, (s, e) => CreateFile(node, "json")).Enabled = enableAlterActions;
                        createMenu.DropDownItems.Add("XML file", Resources.AssetTypeXml, (s, e) => CreateFile(node, "xml")).Enabled = enableAlterActions;
                        createMenu.DropDownItems.Add("Code file", Resources.AssetTypeCode, (s, e) => CreateFile(node, "cs")).Enabled = enableAlterActions;
                        //menu.Items.Add(new ToolStripSeparator());
                        //createMenu.DropDownItems.Add("Sprite", null, (s, e) => CreateFile(node, "cs"));
                        menu.Items.Add(createMenu);
                    }

                    menu.Show(_treeView, e.Location);
                }
            }
            catch (Exception ex)
            {
                _writeLog?.Invoke($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        /// <summary>
        /// Used to get the folder path for a given node by recursively traversing up the tree and concatenating the asset keys.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string GetNodeAssetDirectory(AeTreeNode? node)
        {
            List<string> parts = new List<string>();

            while (node != null)
            {
                parts.Add(node.AssetKey);
                node = node.Parent as AeTreeNode;
            }

            parts.Reverse();
            return string.Join('/', parts.Where(p => string.IsNullOrEmpty(p) == false));
        }


        private void CreateFile(AeTreeNode node, string assetBaseType)
        {
            try
            {
                using var form = new FormGetNewAssetName(_writeLog);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var newAssetKey = $"{GetNodeAssetDirectory(node)}/{form.AssetName}".Trim('/');

                    _engine.Assets.WriteEmptyAsset(newAssetKey, assetBaseType);

                    var asset = _engine.Assets.GetAsset(newAssetKey);

                    var newNode = UpsertTreeNodesPath(asset);
                    _treeView.SelectedNode = newNode;
                }
            }
            catch (Exception ex)
            {
                _writeLog?.Invoke($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        private void CreateFolder(AeTreeNode node)
        {
            try
            {
                using var form = new FormCreateFolder(_writeLog);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var newAssetKey = $"{GetNodeAssetDirectory(node)}/{form.FolderName}".Trim('/');

                    var newNode = new AeTreeNode(form.FolderName, form.FolderName, newAssetKey, AeTreeNodeType.Folder);
                    node.Nodes.Add(newNode);
                    node.Expand();
                    _treeView.SelectedNode = newNode;
                }
            }
            catch (Exception ex)
            {
                _writeLog?.Invoke($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        private void DeleteAsset(AeTreeNode node)
        {
            if (MessageBox.Show("Are you sure you want to delete this asset?",
                AeConstants.FriendlyName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _engine.Assets.DeleteAsset(node.AssetKey);
                _treeView.Nodes.Remove(node);
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
                _writeLog?.Invoke($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
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
                    _engine.Assets.WriteAssetBytesFromFile(node.AssetKey, dialog.FileName);
                    _loadSelectedTreeNode(node);
                }
            }
            catch (Exception ex)
            {
                _writeLog?.Invoke($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
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
                _writeLog?.Invoke($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        public void Repopulate()
        {
            try
            {
                _writeLog?.Invoke("Populating assets.", AeLoggingLevel.Verbose);

                //Files and paths that contain "#" are for internal purposes and should not be shown in the editor.
                var assets = _engine.Assets.GetAssets()
                    .Where(o => o.Key.Contains('#') == false).ToList();

                _writeLog?.Invoke($"Enumerating {assets.Count:n0} assets.", AeLoggingLevel.Verbose);

                foreach (var asset in assets)
                {
                    UpsertTreeNodesPath(asset);
                }

                _writeLog?.Invoke($"Assets enumeration complete.", AeLoggingLevel.Verbose);

                _treeView.Invoke(() => _rootNode.Expand());
            }
            catch (Exception ex)
            {
                _writeLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
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

                TreeNodeCollection workingLevel = _rootNode.Nodes;

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
                _writeLog?.Invoke($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
            return null;
        }
    }
}
