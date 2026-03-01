using NTDLS.Helpers;
using Si.AssetExplorer.Controls;
using Si.Engine;
using Si.Engine.Manager;
using Si.Library;
using Talkster.Client.Controls;

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

        public static void CreateMetaFiles(string rootDirectory)
        {
            if (!Directory.Exists(rootDirectory))
                throw new DirectoryNotFoundException(rootDirectory);

            foreach (var file in Directory.EnumerateFiles(rootDirectory, "*", SearchOption.AllDirectories))
            {
                try
                {
                    // Skip files that are already meta files
                    if (file.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
                        continue;

                    string metaPath = file + ".meta";

                    if (!File.Exists(metaPath))
                    {
                        // Create empty JSON file
                        using (File.Create(metaPath)) { }
                    }
                }
                catch (Exception ex)
                {
                    // Optional: log and continue
                    Console.WriteLine($"Failed: {file} - {ex.Message}");
                }
            }
        }

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
