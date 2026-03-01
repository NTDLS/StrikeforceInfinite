using NTDLS.Helpers;
using NTDLS.WinFormsHelpers;
using Si.AssetExplorer.Controls;
using Si.AssetExplorer.Forms;
using Si.Engine;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library;
using Si.Library.ExtensionMethods;
using static Si.Library.SiConstants;

namespace Si.AssetExplorer
{
    public partial class FormMain : Form
    {
        private readonly EngineCore _engine;
        private bool _firstShown = true;
        private readonly TreeManager _treeManager;
        private readonly PropertListManager _propertListManager;

        public FormMain()
        {
            InitializeComponent();

            WriteOutput("Instanciating EngineCore.", LoggingLevel.Verbose);

            pictureBoxPreview.Parent.EnsureNotNull().Resize += Parent_Resize;
            Parent_Resize(null, new());

            pictureBoxPreview.MouseWheel += PictureBoxPreview_MouseWheel;

            _engine = new EngineCore(pictureBoxPreview, Library.SiConstants.SiEngineExecutionMode.Edit, new Size(1000, 1000));
            _engine.Display.ZoomOverride = 0.1f; // Start zoomed out to show the whole sprite.
            _engine.OnInitializationComplete += EngineCore_OnInitializationComplete;
            _treeManager = new TreeManager(treeViewAssets, _engine, WriteOutput, LoadSelectedTreeNode);
            _propertListManager = new PropertListManager(listViewProperties, _engine, WriteOutput, PropertiesEdited);
            _engine.EnableDevelopment(new FormInterrogation(_engine));

            Shown += FormMain_Shown;
        }

        private void PictureBoxPreview_MouseWheel(object? sender, MouseEventArgs e)
        {
            float zoom = (_engine.Display.ZoomOverride ?? 0);

            zoom += e.Delta > 0 ? -0.01f : 0.01f;
            zoom = Math.Clamp(zoom, 0.001f, 1);

            _engine.Display.ZoomOverride = zoom.IsNearZero() ? null : zoom;
        }

        private void Parent_Resize(object? sender, EventArgs e)
        {
            try
            {
                pictureBoxPreview.Parent.EnsureNotNull();

                int margin = 6;
                var boxSize = Math.Min(pictureBoxPreview.Parent.Width, pictureBoxPreview.Parent.Height) - margin;

                if (boxSize > 10)
                {
                    pictureBoxPreview.Width = boxSize;
                    pictureBoxPreview.Height = boxSize;

                    pictureBoxPreview.Left = (pictureBoxPreview.Parent.Width / 2) - (pictureBoxPreview.Width / 2);
                    pictureBoxPreview.Top = (pictureBoxPreview.Parent.Height / 2) - (pictureBoxPreview.Height / 2);
                }
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void EngineCore_OnInitializationComplete(EngineCore engine)
        {
            try
            {
                WriteOutput("Engine initialization complete.", LoggingLevel.Verbose);

                _engine.Sprites.QueueAllForDeletion();
                _engine.Sprites.HardDeleteAllQueuedDeletions();

                _treeManager.Repopulate();
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void FormMain_Shown(object? sender, EventArgs e)
        {
            try
            {
                if (_firstShown)
                {
                    _firstShown = false;

                    using var progress = new ProgressForm(SiConstants.FriendlyName, "Initializing engine...");

                    progress.Execute(() =>
                    {
                        WriteOutput("Initializing engine.", LoggingLevel.Verbose);
                        _engine.StartEngine();
                    });
                }
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void PropertiesEdited(SpriteBase sprite, PropertyItem propertyItem)
        {

        }

        private void LoadSelectedTreeNode(SiTreeNode node)
        {
            try
            {
                _engine.Events.Once(() =>
                {
                    _engine.Sprites.QueueAllForDeletion();
                    _engine.Sprites.HardDeleteAllQueuedDeletions();

                    var sprite = _engine.Sprites.EditorAdd(node.AssetKey, (o) =>
                    {
                        if (o is SpriteAnimation spriteAnimation)
                        {
                            spriteAnimation.PlayMode = SiAnimationPlayMode.Infinite;
                        }

                        o.Orientation.Degrees = 0;
                        o.IsVisible = true;
                        o.Location = _engine.Display.CenterCanvas;
                        o.RotationSpeed = 0f;
                        o.Speed = 0;
                        o.Throttle = 0;
                    });

                    _propertListManager.PopulateProperties(sprite);
                });
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void WriteOutput(string text, LoggingLevel? loggingLevel)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, LoggingLevel?>(WriteOutput), text, loggingLevel);
                return;
            }

            var color = loggingLevel switch
            {
                LoggingLevel.Verbose => AssetExplorerColors.Verbose,
                LoggingLevel.Information => AssetExplorerColors.Information,
                LoggingLevel.Warning => AssetExplorerColors.Warning,
                LoggingLevel.Error => AssetExplorerColors.Error,
                _ => AssetExplorerColors.Default
            };

            richTextBoxOutput.SelectionStart = richTextBoxOutput.TextLength;
            richTextBoxOutput.SelectionLength = 0;
            richTextBoxOutput.SelectionColor = color;
            richTextBoxOutput.AppendText(text + Environment.NewLine);
            richTextBoxOutput.SelectionColor = richTextBoxOutput.ForeColor;

            // Reset selection to end.
            richTextBoxOutput.Select(richTextBoxOutput.TextLength, 0);
            richTextBoxOutput.SelectionColor = richTextBoxOutput.ForeColor;

            richTextBoxOutput.ScrollToCaret();
        }

        #region Tooklstrip buttons

        private void ToolStripButtonSettings_Click(object sender, EventArgs e)
        {
            try
            {
                using var formSettings = new FormSettings();
                formSettings.ShowDialog();
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void ToolStripButtonDevelopmentConsole_Click(object sender, EventArgs e)
        {
            _engine.Development?.EnsureVisibility();
        }

        #endregion
    }
}
