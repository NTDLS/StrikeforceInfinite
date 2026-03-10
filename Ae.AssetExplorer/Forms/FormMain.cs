using Ae.AssetExplorer.Controls;
using Ae.AssetExplorer.Forms;
using Ae.Engine;
using Ae.Engine.ExtensionMethods;
using Ae.Engine.Sprite._Superclass._Root;
using Ae.Engine.Sprite.Animation;
using NTDLS.Helpers;
using NTDLS.WinFormsHelpers;
using static Ae.Engine.AeConstants;

namespace Ae.AssetExplorer
{
    public partial class FormMain : Form
    {
        private readonly AeEngine _engine;
        private bool _firstShown = true;
        private readonly TreeManager _treeManager;
        private readonly PropertyListManager _propertyListManager;
        private readonly TabManager _tabManager;

        public FormMain()
        {
            InitializeComponent();

            WriteOutput("Instanciating EngineCore.", AeLoggingLevel.Verbose);

            drawingSurface.Parent.EnsureNotNull().Resize += Parent_Resize;
            Parent_Resize(null, new());

            drawingSurface.MouseWheel += PictureBoxPreview_MouseWheel;

            _engine = new AeEngine(drawingSurface, AeConstants.SiEngineExecutionMode.Edit, new Size(1000, 1000));
            _engine.Display.ZoomOverride = 0.1f; // Start zoomed out to show the whole sprite.
            _engine.OnInitializationComplete += EngineCore_OnInitializationComplete;

            _treeManager = new TreeManager(treeViewAssets, _engine, WriteOutput, LoadSelectedTreeNode);
            _propertyListManager = new PropertyListManager(listViewProperties, _engine, WriteOutput, PropertiesEdited);
            _tabManager = new TabManager(_engine, tabControlCode, TabSelected);

            _engine.EnableDevelopment(new FormInterrogation(_engine));

            Shown += FormMain_Shown;
            drawingSurface.MouseDown += DrawingSurface_MouseDown;
        }

        private void PictureBoxPreview_MouseWheel(object? sender, MouseEventArgs e)
        {
            float zoom = (_engine.Display.ZoomOverride ?? 0);

            zoom += e.Delta > 0 ? -0.01f : 0.01f;
            zoom = Math.Clamp(zoom, 0.001f, 1);

            _engine.Display.ZoomOverride = zoom.IsNearZero() ? null : zoom;
        }

        #region Debug interactions.

        private void DrawingSurface_MouseDown(object? sender, MouseEventArgs e)
        {
            List<SpriteBase>? sprites = null;

            _engine.Invoke(() =>
            {
                sprites = _engine.Sprites.All().ToList();
            }).Wait();

            if (sprites?.Count > 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    var menu = new ContextMenuStrip();

                    var watchMenu = new ToolStripMenuItem("Watch");
                    watchMenu.DropDownItemClicked += WatchMenu_ItemClicked;
                    menu.Items.Add(watchMenu);
                    foreach (var sprite in sprites)
                    {
                        var label = $"UID: {sprite.UID}, Type: {sprite.GetType().Name}";
                        if (!string.IsNullOrEmpty(sprite.SpriteTag))
                        {
                            label += $", Tag: {sprite.SpriteTag}";
                        }

                        watchMenu.DropDownItems.Add(label).Tag = sprite;
                    }

                    var inspectMenu = new ToolStripMenuItem("Inspect");
                    inspectMenu.DropDownItemClicked += InspectMenu_ItemClicked;
                    menu.Items.Add(inspectMenu);
                    foreach (var sprite in sprites)
                    {
                        var label = $"UID: {sprite.UID}, Type: {sprite.GetType().Name}";
                        if (!string.IsNullOrEmpty(sprite.SpriteTag))
                        {
                            label += $", Tag: {sprite.SpriteTag}";
                        }

                        inspectMenu.DropDownItems.Add(label).Tag = sprite;
                    }

                    var deleteMenu = new ToolStripMenuItem("Delete");
                    deleteMenu.DropDownItemClicked += DeleteMenu_ItemClicked;
                    menu.Items.Add(deleteMenu);
                    foreach (var sprite in sprites)
                    {
                        var label = $"UID: {sprite.UID}, Type: {sprite.GetType().Name}";
                        if (!string.IsNullOrEmpty(sprite.SpriteTag))
                        {
                            label += $", Tag: {sprite.SpriteTag}";
                        }

                        deleteMenu.DropDownItems.Add(label).Tag = sprite;
                    }

                    var location = new Point((int)e.X + 10, (int)e.Y);
                    menu.Show(_engine.Display.DrawingSurface, location);
                }
            }
        }

        private void InspectMenu_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            (sender as ToolStripDropDown)?.Close();
            if (e.ClickedItem?.Tag is not SpriteBase sprite) return;

            _engine.Development?.EnsureVisibility();
            _engine.Development?.EnqueueCommand($"Sprite-Inspect {sprite.UID}");
        }

        private void WatchMenu_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            (sender as ToolStripDropDown)?.Close();
            if (e.ClickedItem?.Tag is not SpriteBase sprite) return;

            Task.Run(() =>
            {
                using var form = new FormInterrogationSpriteWatch(_engine, sprite);
                form.ShowDialog();
            });
        }

        private void DeleteMenu_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            (sender as ToolStripDropDown)?.Close();
            if (e.ClickedItem?.Tag is not SpriteBase sprite) return;

            sprite.QueueForDelete();
        }

        #endregion

        private void Parent_Resize(object? sender, EventArgs e)
        {
            try
            {
                drawingSurface.Parent.EnsureNotNull();

                int margin = 6;
                var boxSize = Math.Min(drawingSurface.Parent.Width, drawingSurface.Parent.Height) - margin;

                if (boxSize > 10)
                {
                    drawingSurface.Width = boxSize;
                    drawingSurface.Height = boxSize;

                    drawingSurface.Left = (drawingSurface.Parent.Width / 2) - (drawingSurface.Width / 2);
                    drawingSurface.Top = (drawingSurface.Parent.Height / 2) - (drawingSurface.Height / 2);
                }
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        private void EngineCore_OnInitializationComplete(AeEngine engine)
        {
            try
            {
                WriteOutput("Engine initialization complete.", AeLoggingLevel.Verbose);

                engine.Events.Once(() =>
                {
                    _engine.Sprites.QueueAllForDeletion();
                    _engine.Sprites.HardDeleteAllQueuedDeletions();
                });

                _treeManager.Repopulate();
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        private void FormMain_Shown(object? sender, EventArgs e)
        {
            try
            {
                if (_firstShown)
                {
                    if (!Program.NoSplash)
                    {
                        var formStartup = new FormStartup();
                        formStartup.ShowDialog();
                    }

                    _firstShown = false;

                    using var progressForm = new ProgressForm(AeConstants.FriendlyName, "Initializing engine...");

                    progressForm.Execute(() =>
                    {
                        WriteOutput("Initializing engine.", AeLoggingLevel.Verbose);

                        progressForm.SeProgressStyle(ProgressBarStyle.Continuous);
                        progressForm.SetProgressMinimum(0);
                        progressForm.SetProgressMaximum(100);

                        void EngineStartupProgressCallback(string message, float progress)
                        {
                            progressForm.SetBodyText($"{message} ({progress:n0}%)");
                            progressForm.SetProgressValue((int)progress);
                        }

                        _engine.StartEngine(EngineStartupProgressCallback, WriteOutput);
                    });
                }
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
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
                _tabManager.AddTab(node);
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
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

                    var sprite = _engine.Sprites.EditorAdd(tab.AssetKey, WriteOutput, (o) =>
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

                    _propertyListManager.PopulateProperties(tab.AssetKey, sprite);
                });
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        private void WriteOutput(string text, AeLoggingLevel? loggingLevel)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, AeLoggingLevel?>(WriteOutput), text, loggingLevel);
                return;
            }

            var color = loggingLevel switch
            {
                AeLoggingLevel.Verbose => AssetExplorerColors.Verbose,
                AeLoggingLevel.Information => AssetExplorerColors.Information,
                AeLoggingLevel.Warning => AssetExplorerColors.Warning,
                AeLoggingLevel.Error => AssetExplorerColors.Error,
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

        #region Toolstrip buttons

        private void ToolStripButtonSettings_Click(object sender, EventArgs e)
        {
            try
            {
                using var formSettings = new FormSettings();
                formSettings.ShowDialog();
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        private void ToolStripButtonDeveloperConsole_Click(object sender, EventArgs e)
            => _engine.Development?.EnsureVisibility();

        private void ToolStripButtonToggleAssets_Click(object sender, EventArgs e)
            => splitContainerLeft.Panel1Collapsed = !splitContainerLeft.Panel1Collapsed;

        private void ToolStripButtonToggleOutput_Click(object sender, EventArgs e)
            => splitContainerBottom.Panel2Collapsed = !splitContainerBottom.Panel2Collapsed;

        private void ToolStripButtonToggleProperties_Click(object sender, EventArgs e)
            => splitContainerRight.Panel2Collapsed = !splitContainerRight.Panel2Collapsed;

        private void ToolStripButtonSave_Click(object sender, EventArgs e)
        {
            _tabManager.SaveCurrentTab();
        }

        private void ToolStripButtonSaveAll_Click(object sender, EventArgs e)
        {
        }

        private void ToolStripButtonClose_Click(object sender, EventArgs e)
            => _tabManager.CloseCurrentTab();

        #endregion

        private void toolStripButtonAbout_Click(object sender, EventArgs e)
        {
            using var formAbout = new FormAbout();
            formAbout.ShowDialog();
        }
    }
}
