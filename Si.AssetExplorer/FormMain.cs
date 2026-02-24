using NTDLS.WinFormsHelpers;
using SharpCompress.Archives;
using SharpCompress.Common;
using Si.Engine;
using Si.Engine.Manager;
using Si.Engine.Sprite.Enemy.Peon;

namespace Si.AssetExplorer
{
    public partial class FormMain : Form
    {
        private readonly EngineCore _engine;
        private bool _firstShown = true;

        public FormMain()
        {
            InitializeComponent();
            _engine = new EngineCore(pictureBoxPreview, Library.SiConstants.SiEngineExecutionMode.Edit);
            _engine.OnInitializationComplete += _engineCore_OnInitializationComplete;
            Shown += FormMain_Shown;
        }

        private void _engineCore_OnInitializationComplete(EngineCore engine)
        {
            _engine.Events.Once(() =>
            {
                var sprite = _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyPhoenix>();
                sprite.Location = _engine.Display.CenterCanvas;
                sprite.Speed = 0;
                sprite.Throttle = 0;
            });
        }

        private void ExpandRootNodes()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ExpandRootNodes));
                return;
            }

            foreach (TreeNode node in treeViewAssets.Nodes)
            {
                node.Expand();
            }
        }

        private void UpsertTreeNodesPath(string path)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpsertTreeNodesPath), path);
                return;
            }

            path = path.TrimStart(['\\', '/', '.']).TrimStart(['\\', '/', '.']);
            var parts = path.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries);

            TreeNodeCollection workingLevel = treeViewAssets.Nodes;

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

        private void FormMain_Shown(object? sender, EventArgs e)
        {
            if (_firstShown)
            {
                _firstShown = false;

                using var pf = new ProgressForm("Asset Explorer", "Loading engine...");

                pf.Execute(() =>
                {
                    _engine.StartEngine();

                    var archive = ArchiveFactory.Open(_engine.Assets.AssetPackagePath, new SharpCompress.Readers.ReaderOptions()
                    {
                        ArchiveEncoding = new ArchiveEncoding()
                        {
                            Default = System.Text.Encoding.Default
                        }
                    });

                    foreach (var entry in archive.Entries)
                    {
                        if (entry.Key == null) continue;
                        if(Path.GetExtension(entry.Key).Equals(".meta", StringComparison.OrdinalIgnoreCase)) continue;

                        //if (AssetManager.IsDirectoryFromAttrib(entry))
                        {
                            WriteOutput(entry.Key);
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

                    ExpandRootNodes();
                });
            }
        }


        private void WriteOutput(string text, Color? color = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color?>(WriteOutput), text, color);
                return;
            }

            richTextBoxOutput.SelectionStart = richTextBoxOutput.TextLength;
            richTextBoxOutput.SelectionLength = 0;

            richTextBoxOutput.SelectionColor = color ?? Color.Black;
            richTextBoxOutput.AppendText($"{text}\r\n");
            richTextBoxOutput.SelectionColor = richTextBoxOutput.ForeColor;

            richTextBoxOutput.SelectionStart = richTextBoxOutput.Text.Length;
            //richTextBoxOutput.ScrollToCaret();
        }

    }
}
