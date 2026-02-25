using SharpCompress.Archives;
using SharpCompress.Common;
using Si.Engine;

namespace Si.AssetExplorer
{
    internal class TreeManager
    {
        public readonly TreeView _treeView;
        private readonly EngineCore _engineCore;
        public readonly Action<string, LoggingLevel?> _writeAction;

        public TreeManager(TreeView treeView, EngineCore engineCore, Action<string, LoggingLevel?> writeAction)
        {
            _engineCore = engineCore;
            _treeView = treeView;
            _writeAction = writeAction;
        }

        private void WriteOutput(string text, LoggingLevel? color = null)
            => _writeAction(text, color);

        public void Repopulate()
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

                //if (AssetManager.IsDirectoryFromAttrib(entry))
                {
                    UpsertTreeNodesPath(entry.Key);
                }

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

        private void ExpandRootNodes()
        {
            if (_treeView.InvokeRequired)
            {
                _treeView.Invoke(new Action(ExpandRootNodes));
                return;
            }

            foreach (TreeNode node in _treeView.Nodes)
            {
                node.Expand();
            }
        }

        private void UpsertTreeNodesPath(string path)
        {
            if (_treeView.InvokeRequired)
            {
                _treeView.Invoke(new Action<string>(UpsertTreeNodesPath), path);
                return;
            }

            path = path.TrimStart(['\\', '/', '.']).TrimStart(['\\', '/', '.']);
            var parts = path.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries);

            TreeNodeCollection workingLevel = _treeView.Nodes;

            foreach (var part in parts)
            {
                var foundNode = workingLevel.Find(part, false);
                if (foundNode.Length == 1)
                {
                    workingLevel = foundNode.First().Nodes;
                }
                else
                {
                    var newNode = new TreeNode(part) { Name = part };
                    workingLevel.Add(newNode);
                    workingLevel = newNode.Nodes;
                }
            }
        }
    }
}
