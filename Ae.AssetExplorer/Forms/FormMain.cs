using Ae.AssetExplorer.Controls;
using Ae.AssetExplorer.Forms;
using Ae.Engine;
using Ae.Engine.Compiler;
using Ae.Engine.ExtensionMethods;
using Ae.Engine.Sprite;
using Ae.Engine.Sprite.Base;
using NTDLS.Helpers;
using NTDLS.WinFormsHelpers;
using System.Reflection;

namespace Ae.AssetExplorer
{
    public partial class FormMain : Form
    {
        private bool _firstShown = true;
        private readonly AeEngine _engine;
        private readonly TreeManager _treeManager;
        private readonly PropertyListManager _propertyListManager;
        private readonly TabManager _tabManager;
        private readonly AeCodeEditor _codeViewer;

        public FormMain()
        {
            InitializeComponent();

            WriteLog("Instantiating EngineCore.", AeLoggingLevel.Verbose);

            drawingSurface.Parent.EnsureNotNull().Resize += Parent_Resize;
            Parent_Resize(null, new());

            drawingSurface.MouseWheel += PictureBoxPreview_MouseWheel;

            _engine = new AeEngine(drawingSurface, AeEngineExecutionMode.Edit, new Size(1000, 1000));
            _engine.Display.ZoomOverride = 0.1f; // Start zoomed out to show the whole sprite.
            _engine.OnInitializationComplete += EngineCore_OnInitializationComplete;

            _treeManager = new TreeManager(treeViewAssets, _engine, WriteLog, LoadSelectedTreeNode);
            _propertyListManager = new PropertyListManager(listViewProperties, _engine, WriteLog, PropertiesEdited);
            _tabManager = new TabManager(_engine, tabControlCode, TabSelected);

            _engine.EnableDevelopment(new FormInterrogation(_engine));

            Shown += FormMain_Shown;
            drawingSurface.MouseDown += DrawingSurface_MouseDown;

            _codeViewer = new AeCodeEditor(this, AeCodeType.CSharp);
            tabPageCode.Controls.Add(_codeViewer);


            listViewOutput.MouseDoubleClick += ListViewOutput_MouseDoubleClick;

        }

        private void ListViewOutput_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            var item = listViewOutput.HitTest(e.Location)?.Item as AeLogListViewItem;
            if (item != null && string.IsNullOrEmpty(item.AssetKey) == false)
            {
                _treeManager.HighlightItem(item.AssetKey);
            }
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
            List<AeSprite>? sprites = null;

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
            if (e.ClickedItem?.Tag is not AeSprite sprite) return;

            _engine.Development?.EnsureVisibility();
            _engine.Development?.EnqueueCommand($"Sprite-Inspect {sprite.UID}");
        }

        private void WatchMenu_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            (sender as ToolStripDropDown)?.Close();
            if (e.ClickedItem?.Tag is not AeSprite sprite) return;

            Task.Run(() =>
            {
                using var form = new FormInterrogationSpriteWatch(_engine, sprite);
                form.ShowDialog();
            });
        }

        private void DeleteMenu_ItemClicked(object? sender, ToolStripItemClickedEventArgs e)
        {
            (sender as ToolStripDropDown)?.Close();
            if (e.ClickedItem?.Tag is not AeSprite sprite) return;

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
                WriteLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        private void EngineCore_OnInitializationComplete(AeEngine engine)
        {
            try
            {
                WriteLog("Engine initialization complete.", AeLoggingLevel.Verbose);

                engine.Events.Once(() =>
                {
                    _engine.Sprites.QueueAllForDeletion();
                    _engine.Sprites.HardDeleteAllQueuedDeletions();
                });

                _treeManager.Repopulate();
            }
            catch (Exception ex)
            {
                WriteLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
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
                        WriteLog("Initializing engine.", AeLoggingLevel.Verbose);

                        progressForm.SeProgressStyle(ProgressBarStyle.Continuous);
                        progressForm.SetProgressMinimum(0);
                        progressForm.SetProgressMaximum(100);

                        void EngineStartupProgressCallback(string message, float progress)
                        {
                            progressForm.SetBodyText($"{message} ({progress:n0}%)");
                            progressForm.SetProgressValue((int)progress);
                        }

                        _engine.StartEngine(EngineStartupProgressCallback, WriteLog);
                    });
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        /// <summary>
        /// An attribute property was edited in the property list.
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="propertyItem"></param>
        private void PropertiesEdited(AeSprite sprite, PropertyItem propertyItem)
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
                WriteLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
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

                    var sprite = _engine.Sprites.EditorAdd(tab.AssetKey, WriteLog, (o) =>
                    {
                        if (o is AeSpriteAnimation spriteAnimation)
                        {
                            spriteAnimation.PlayMode = AeAnimationPlayMode.Infinite;
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
                WriteLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        private void WriteLog(string message, AeLoggingLevel level, string? assetKey = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, AeLoggingLevel, string?>(WriteLog), message, level, assetKey);
                return;
            }

            listViewOutput.Items.Add(new AeLogListViewItem(level, message, assetKey));
            listViewOutput.EnsureVisible(listViewOutput.Items.Count - 1);
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
                WriteLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
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
            _tabManager.SaveAllTabs();
        }

        private void ToolStripButtonClose_Click(object sender, EventArgs e)
            => _tabManager.CloseCurrentTab();

        private void ToolStripButtonAbout_Click(object sender, EventArgs e)
        {
            using var formAbout = new FormAbout();
            formAbout.ShowDialog();
        }

        private void ToolStripButtonBuild_Click(object sender, EventArgs e)
        {
            if (!_tabManager.SaveCurrentTab())
            {
                return;
            }
            var tab = _tabManager.CurrentTab();
            if (tab != null)
            {
                var codeToCompile = _engine.Assets.GetAssetCodeForCompilation(tab.AssetKey, WriteLog);
                _codeViewer.Text = codeToCompile ?? $"No code available for asset {tab.AssetKey}";

                if (string.IsNullOrEmpty(codeToCompile) == false)
                {
                    try
                    {
                        if (AeRuntimeCompiler.CompileToAssembly(tab.AssetKey, codeToCompile, false, WriteLog))
                        {
                            WriteLog("Successfully compiled.", AeLoggingLevel.Information, tab.AssetKey);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog($"Failed to compile asset controller for asset with key: {tab.AssetKey}. Error: {ex.Message}", AeLoggingLevel.Error);
                    }
                }
            }
        }

        #endregion

        #region Menu items.

        private void ExtractProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using var dialog = new SaveFileDialog
                {
                    Title = "Save Asset",
                    Filter = $"C# Project File (*.csproj)|*.csproj|All Files (*.*)|*.*",
                    FileName = $"extractedProject.csproj",
                    DefaultExt = "csproj",
                    AddExtension = true,
                    OverwritePrompt = true
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var selectedFileName = dialog.FileName;

                    var fileName = Path.GetFileNameWithoutExtension(selectedFileName)
                        ?? throw new Exception("Could not determine project name from file path.");

                    var directory = Path.GetDirectoryName(selectedFileName)
                        ?? throw new Exception("Could not determine directory from file path.");

                    //Create a directory with the same name as the project to extract the files to, to avoid cluttering the user-selected directory with multiple files.
                    Directory.CreateDirectory(Path.Combine(directory, fileName));

                    selectedFileName = Path.Combine(directory, fileName, Path.GetFileName(selectedFileName));

                    directory = Path.GetDirectoryName(selectedFileName)
                        ?? throw new Exception("Could not determine directory from file path.");

                    var projectName = AeRuntimeCompiler.AssetKeyToClassName(fileName);
                    var version = string.Join('.', (Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0").Split('.').Take(3));

                    var projectFileText = $@"<Project Sdk=""Microsoft.NET.Sdk"">
                              <PropertyGroup>
                                <TargetFramework>net10.0-windows</TargetFramework>
                                <ImplicitUsings>enable</ImplicitUsings>
                                <Nullable>enable</Nullable>
                                <RootNamespace>Ae.Engine</RootNamespace>
                                <AssemblyName>Ae.Engine</AssemblyName>
                              </PropertyGroup>
                              <ItemGroup>
                                <PackageReference Include=""Ae.Engine"" Version=""{version}"" />
                              </ItemGroup>
                            </Project>";

                    File.WriteAllText(selectedFileName, projectFileText);

                    _engine.Assets.ExtractProject(directory, WriteLog);
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        #endregion
    }
}
