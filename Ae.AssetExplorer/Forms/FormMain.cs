using Ae.AssetExplorer.Controls;
using Ae.AssetExplorer.Forms;
using Ae.Engine;
using Ae.Engine.Sprite._Superclass._Root;
using Ae.Engine.Sprite._Superclass.Animation;
using Ae.Library;
using Ae.Library.ExtensionMethods;
using NTDLS.Helpers;
using NTDLS.WinFormsHelpers;
using static Ae.Library.AeConstants;

namespace Ae.AssetExplorer
{
    public partial class FormMain : Form
    {
        private readonly AeEngine _engine;
        private bool _firstShown = true;
        private readonly TreeManager _treeManager;
        private readonly PropertyListManager _propertListManager;
        private readonly TabManager _tabManager;

        public FormMain()
        {
            InitializeComponent();

            WriteOutput("Instanciating EngineCore.", LoggingLevel.Verbose);

            pictureBoxPreview.Parent.EnsureNotNull().Resize += Parent_Resize;
            Parent_Resize(null, new());

            pictureBoxPreview.MouseWheel += PictureBoxPreview_MouseWheel;

            _engine = new AeEngine(pictureBoxPreview, AeConstants.SiEngineExecutionMode.Edit, new Size(1000, 1000));
            _engine.Display.ZoomOverride = 0.1f; // Start zoomed out to show the whole sprite.
            _engine.OnInitializationComplete += EngineCore_OnInitializationComplete;

            _treeManager = new TreeManager(treeViewAssets, _engine, WriteOutput, LoadSelectedTreeNode);
            _propertListManager = new PropertyListManager(listViewProperties, _engine, WriteOutput, PropertiesEdited);
            _tabManager = new TabManager(_engine, tabControlCode, TabSelected);

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

        private void EngineCore_OnInitializationComplete(AeEngine engine)
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
                    var formStartup = new FormStartup();
                    formStartup.ShowDialog();

                    _firstShown = false;

                    using var progressForm = new ProgressForm(AeConstants.FriendlyName, "Initializing engine...");

                    progressForm.Execute(() =>
                    {
                        WriteOutput("Initializing engine.", LoggingLevel.Verbose);

                        progressForm.SeProgressStyle(ProgressBarStyle.Continuous);
                        progressForm.SetProgressMinimum(0);
                        progressForm.SetProgressMaximum(100);

                        void EngineStartupProgressCallback(string message, float progress)
                        {
                            progressForm.SetBodyText($"{message} ({progress:n0}%)");
                            progressForm.SetProgressValue((int)progress);
                        }

                        _engine.StartEngine(EngineStartupProgressCallback);
                    });
                }
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        /// <summary>
        /// An attribute property was edited in the property list.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="propertyItem"></param>
        private void PropertiesEdited(SpriteBase sprite, PropertyItem propertyItem)
        {

        }

        /// <summary>
        /// A tree node was double-clicked.
        /// </summary>
        /// <param name="node"></param>
        private void LoadSelectedTreeNode(AeTreeNode node)
        {
            try
            {
                _tabManager.AddTab(node.AssetKey);
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        /// <summary>
        /// A tab page was selected.
        /// </summary>
        /// <param name="tab"></param>
        private void TabSelected(AeTabPage tab)
        {
            try
            {
                _engine.Events.Once(() =>
                {
                    _engine.Sprites.QueueAllForDeletion();
                    _engine.Sprites.HardDeleteAllQueuedDeletions();

                    var sprite = _engine.Sprites.EditorAdd(tab.AssetKey, (o) =>
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

                    _propertListManager.PopulateProperties(tab.AssetKey, sprite);
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

        private void ToolStripButtonDeveloperConsole_Click(object sender, EventArgs e)
        {
            _engine.Development?.EnsureVisibility();
        }

        #endregion
    }
}
