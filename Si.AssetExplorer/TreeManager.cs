using NTDLS.Helpers;
using SharpCompress.Archives;
using SharpCompress.Common;
using Si.AssetExplorer.Controls;
using Si.Engine;
using Si.Engine.Manager;
using Talkster.Client.Controls;

namespace Si.AssetExplorer
{
    internal class TreeManager
    {
        public readonly DoubleBufferedTreeView _treeView;
        private readonly EngineCore _engineCore;
        public readonly Action<string, LoggingLevel?> _writeOutput;
        public readonly Action<SiTreeNode> _loadSelectedTreeNode;

        public TreeManager(DoubleBufferedTreeView treeView, EngineCore engineCore,
            Action<string, LoggingLevel?> writeOutput,
            Action<SiTreeNode> loadSelectedTreeNode)
        {
            _engineCore = engineCore;
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

                WriteOutput("Accessing archive.", LoggingLevel.Verbose);
                var archive = ArchiveFactory.Open(_engineCore.Assets.AssetPackagePath, new SharpCompress.Readers.ReaderOptions()
                {
                    ArchiveEncoding = new ArchiveEncoding()
                    {
                        Default = System.Text.Encoding.Default
                    }
                });

                WriteOutput($"Enumerating {archive.Entries.Count():n0} assets.", LoggingLevel.Verbose);

                foreach (var entry in archive.Entries)
                {
                    if (entry.Key == null) continue;
                    if (Path.GetExtension(entry.Key).Equals(".meta", StringComparison.OrdinalIgnoreCase)) continue;

                    UpsertTreeNodesPath(entry);

                    switch (Path.GetExtension(entry.Key).ToLower())
                    {
                        case ".meta":
                        case ".json":
                        case ".txt":
                            //threadPoolTracker.Enqueue(() => GetText(entry.Key), (QueueItemState<object> o) => Interlocked.Increment(ref statusIndex));
                            break;
                        case ".png":
                        case ".jpg":
                        case ".bmp":
                            //threadPoolTracker.Enqueue(() => GetBitmap(entry.Key), (QueueItemState<object> o) => Interlocked.Increment(ref statusIndex));
                            break;
                        case ".wav":
                            //threadPoolTracker.Enqueue(() => GetAudio(entry.Key), (QueueItemState<object> o) => Interlocked.Increment(ref statusIndex));
                            break;
                        default:
                            //Interlocked.Increment(ref statusIndex);
                            break;
                    }
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

        private void UpsertTreeNodesPath(IArchiveEntry entry)
        {
            try
            {
                if (_treeView.InvokeRequired)
                {
                    _treeView.Invoke(new Action<IArchiveEntry>(UpsertTreeNodesPath), entry);
                    return;
                }

                var path = entry.Key.EnsureNotNull().TrimStart(['\\', '/', '.']).TrimStart(['\\', '/', '.']);
                var parts = path.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries);

                TreeNodeCollection workingLevel = _treeView.Nodes;

                foreach (var part in parts)
                {
                    if (part.StartsWith("@"))
                    {
                        //We use the "@" prefix to denote special nodes that should be ignored in the tree structure.
                        return;
                    }

                    var foundNode = workingLevel.Find(part, false);
                    if (foundNode.Length == 1)
                    {
                        workingLevel = foundNode.First().Nodes;
                    }
                    else
                    {
                        var nodeType = AssetManager.IsDirectoryFromAttrib(entry) ? SiTreeNodeType.Folder : SiTreeNodeType.Asset;

                        var displayName = part;

                        if (nodeType == SiTreeNodeType.Asset)
                        {
                            displayName = Path.GetFileNameWithoutExtension(part);
                        }

                        var newNode = new SiTreeNode(part, displayName, entry.Key, nodeType);
                        workingLevel.Add(newNode);
                        workingLevel = newNode.Nodes;
                    }
                }
            }
            catch (Exception ex)
            {
                _writeOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }
    }
}
