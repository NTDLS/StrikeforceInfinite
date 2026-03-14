using Ae.AssetExplorer.Controls;
using Ae.AssetExplorer.Forms;
using Ae.Engine;
using Ae.Engine.Compiler;
using Ae.Engine.ExtensionMethods;
using Ae.Engine.Sprite;
using Ae.Engine.Sprite.Base;
using NTDLS.Helpers;
using NTDLS.WinFormsHelpers;
using System.Diagnostics;
using System.Management;

namespace Ae.AssetExplorer
{
    public partial class FormMain : Form
    {
        private bool _firstShown = true;
        private readonly AeEngine _engine;
        private readonly TreeManager _treeManager;
        private readonly PropertyListManager _propertyListManager;
        internal TabManager TabManager { get; private set; }
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
            TabManager = new TabManager(this, _engine, tabControlCode);

            TabManager.TabSelected += TabManager_TabSelected;
            TabManager.TabCollectionModified += (TabManager tabManager, AeTabPage tabPage) => UpdateToolBarStates(tabManager.TabControl.TabPages.Count > 0); ;

            _engine.EnableDevelopment(new FormInterrogation(_engine));

            Shown += FormMain_Shown;
            drawingSurface.MouseDown += DrawingSurface_MouseDown;

            _codeViewer = new AeCodeEditor(this, null, AeCodeType.CSharp);
            tabPageCode.Controls.Add(_codeViewer);


            listViewOutput.MouseDoubleClick += ListViewOutput_MouseDoubleClick;

            panelSearch.Height = textBoxSearch.Height + 2;
            textBoxSearch.KeyUp += (object? sender, KeyEventArgs e) => _treeManager.SearchTextChange(textBoxSearch.Text);
            buttonClearSearch.Click += (object? sender, EventArgs e) =>
            {
                textBoxSearch.Text = string.Empty;
                _treeManager.SearchTextChange(string.Empty);
            };

            UpdateToolBarStates(false);
        }

        private void UpdateToolBarStates(bool hasTabs)
        {
            toolStripButtonSave.Enabled = hasTabs;
            toolStripButtonSaveAll.Enabled = hasTabs;
            toolStripButtonClose.Enabled = hasTabs;
            toolStripButtonUndo.Enabled = hasTabs;
            toolStripButtonRedo.Enabled = hasTabs;
            toolStripButtonBuild.Enabled = hasTabs;
            toolStripButtonRun.Enabled = true;
            toolStripButtonDebug.Enabled = true;
            toolStripButtonBreak.Enabled = hasTabs;
            toolStripButtonComment.Enabled = hasTabs;
            toolStripButtonUncomment.Enabled = hasTabs;
            toolStripButtonCopy.Enabled = hasTabs;
            toolStripButtonCut.Enabled = hasTabs;
            toolStripButtonPaste.Enabled = hasTabs;
            toolStripButtonDecreaseIndent.Enabled = hasTabs;
            toolStripButtonIncreaseIndent.Enabled = hasTabs;
            toolStripButtonFind.Enabled = hasTabs;
            toolStripButtonReplace.Enabled = hasTabs;
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
                TabManager.AddTab(node);
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
        private void TabManager_TabSelected(TabManager tabManager, AeTabPage? tabPage)
        {
            try
            {
                if (tabPage == null)
                {
                    _propertyListManager.Clear();
                }

                _engine.Events.Once(() =>
            {
                try
                {
                    _engine.Sprites.QueueAllForDeletion();
                    _engine.Sprites.HardDeleteAllQueuedDeletions();

                    if (tabPage != null)
                    {
                        var sprite = _engine.Sprites.EditorAdd(tabPage.AssetKey, WriteLog, (o) =>
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

                        _propertyListManager.PopulateProperties(tabPage.AssetKey, sprite);
                    }
                }
                catch (Exception ex)
                {
                    WriteLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
                }
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

        #region Menu items.

        private void IngestProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using var dialog = new FolderBrowserDialog
                {
                    Description = "Select a previously extracted Visual Studio project.",
                    UseDescriptionForTitle = true,
                    ShowNewFolderButton = true
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (ProjectMerger.IngestVsProject(_engine, dialog.SelectedPath, WriteLog).Count > 0)
                    {
                        _treeManager.Repopulate();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        private void ExtractProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using var dialog = new FolderBrowserDialog
                {
                    Description = "Select a folder for the extracted Visual Studio project.",
                    UseDescriptionForTitle = true,
                    ShowNewFolderButton = true
                };

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    ProjectMerger.ExtractVsProject(_engine, Path.Combine(dialog.SelectedPath, "Ae.Engine.Debug"), WriteLog);
                }
            }
            catch (Exception ex)
            {
                WriteLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        #endregion

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
            TabManager.SaveCurrentTab();
        }

        private void ToolStripButtonSaveAll_Click(object sender, EventArgs e)
        {
            TabManager.SaveAllTabs();
        }

        private void ToolStripButtonClose_Click(object sender, EventArgs e)
            => TabManager.CloseCurrentTab();

        private void ToolStripButtonAbout_Click(object sender, EventArgs e)
        {
            using var formAbout = new FormAbout();
            formAbout.ShowDialog();
        }

        private void ToolStripButtonBuild_Click(object sender, EventArgs e)
        {
            if (!TabManager.SaveCurrentTab())
            {
                return;
            }
            var tab = TabManager.CurrentTab();
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
                        WriteLog($"Failed to compile asset controller for asset with key: {tab.AssetKey}. Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
                    }
                }
            }
        }

        private void ToolStripButtonComment_Click(object sender, EventArgs e)
            => TabManager.CommentSelection();

        private void ToolStripButtonUncomment_Click(object sender, EventArgs e)
            => TabManager.UncommentSelection();

        private void ToolStripButtonCopy_Click(object sender, EventArgs e)
            => TabManager.Copy();

        private void ToolStripButtonCut_Click(object sender, EventArgs e)
            => TabManager.Cut();

        private void ToolStripButtonPaste_Click(object sender, EventArgs e)
            => TabManager.Paste();

        private void ToolStripButtonDecreaseIndent_Click(object sender, EventArgs e)
            => TabManager.DecreaseCurrentTabIndent();

        private void ToolStripButtonIncreaseIndent_Click(object sender, EventArgs e)
            => TabManager.IncreaseCurrentTabIndent();

        private void ToolStripButtonFind_Click(object sender, EventArgs e)
            => ShowFind(TabManager.CurrentTab()?.EditorHost.Editor.SelectedText);

        private void ToolStripButtonReplace_Click(object sender, EventArgs e)
            => ShowReplace(TabManager.CurrentTab()?.EditorHost.Editor.SelectedText);

        private void ToolStripButtonDebug_Click(object sender, EventArgs e)
        {
            try
            {
                TabManager.SaveAllTabs();

                string tempPath = Path.Combine(Path.GetTempPath(), AeConstants.FriendlyName, "ProjectExtract", Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempPath);
                var projectFile = ProjectMerger.ExtractVsProject(_engine, tempPath, WriteLog)
                    ?? throw new Exception("Project extraction failed.");

                _ = Process.Start(new ProcessStartInfo
                {
                    FileName = "devenv.exe",
                    Arguments = $"\"{projectFile}\"",
                    UseShellExecute = true
                });

                Task.Run(() =>
                {
                    Task.Delay(1000);

                    //Find the instance of VS that we launched by searching for one with our extracted project in the commandline.
                    //We do this in case VS used a previously launched instance instead of opening a new one, or used the VS version selector dialog.
                    var searcher = new ManagementObjectSearcher(
                    "SELECT ProcessId, CommandLine FROM Win32_Process WHERE Name = 'devenv.exe'");

                    foreach (var obj in searcher.Get().Cast<ManagementObject>())
                    {
                        if (obj["CommandLine"]?.ToString()?.Contains(projectFile, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            int pid = Convert.ToInt32(obj["ProcessId"]);
                            var process = Process.GetProcessById(pid);

                            process.WaitForExit();

                            Invoke(() =>
                            {
                                if (MessageBox.Show(this, "Would you like to merge any changes made back into this library? If not, any changes you made in Visual Studio will be lost.", AeConstants.FriendlyName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                {
                                    if (ProjectMerger.IngestVsProject(_engine, tempPath, WriteLog).Count > 0)
                                    {
                                        _treeManager.Repopulate();
                                    }
                                }
                            });

                            Directory.Delete(tempPath, true);
                            return;
                        }
                    }
                    Directory.Delete(tempPath, true);

                    WriteLog("Could not find the Visual Studio process that was launched for debugging.", AeLoggingLevel.Warning);
                });
            }
            catch (Exception ex)
            {
                WriteLog($"Error: {ex.GetBaseException().Message}", AeLoggingLevel.Error);
            }
        }

        private void ToolStripButtonRun_Click(object sender, EventArgs e)
        {
        }

        private void ToolStripButtonBreak_Click(object sender, EventArgs e)
        {
        }

        #endregion

        #region Find and Replace.

        private FormFindReplace? _findReplaceForm;
        private string _lastSearchText = string.Empty;
        private bool _lastSearchCaseSensitive = false;
        private string _lastReplaceText = string.Empty;

        public void ShowFind(string? selectedText)
        {
            if (_findReplaceForm == null || _findReplaceForm.IsDisposed)
            {
                _findReplaceForm = new FormFindReplace(this, _lastSearchText, _lastReplaceText);
            }

            _findReplaceForm.Show(FormFindReplace.FindType.Find, selectedText);
            _findReplaceForm.BringToFront();
        }

        public void ShowReplace(string? selectedText)
        {
            if (_findReplaceForm == null || _findReplaceForm.IsDisposed)
            {
                _findReplaceForm = new FormFindReplace(this, _lastSearchText, _lastReplaceText);
            }

            _findReplaceForm.Show(FormFindReplace.FindType.Replace, selectedText);
            _findReplaceForm.BringToFront();
        }

        public void FindNext()
        {
            var info = TabManager.CurrentTab();
            if (info != null)
            {
                if (string.IsNullOrEmpty(_lastSearchText))
                {
                    ShowFind(TabManager.CurrentTab()?.EditorHost.Editor.SelectedText);
                }
                else
                {
                    TabManager.CurrentTab()?.EditorHost.FindNext(_lastSearchText, _lastSearchCaseSensitive);
                }
            }
        }

        public void FindNext(string searchText, bool caseSensitive)
        {
            _lastSearchText = searchText;
            _lastSearchCaseSensitive = caseSensitive;
            TabManager.CurrentTab()?.EditorHost.FindNext(searchText, caseSensitive);
        }

        public void FindReplace(string searchText, string replaceWith, bool caseSensitive)
        {
            _lastSearchText = searchText;
            _lastReplaceText = replaceWith;
            _lastSearchCaseSensitive = caseSensitive;
            TabManager.CurrentTab()?.EditorHost.FindReplace(searchText, replaceWith, caseSensitive);
        }

        public void FindReplaceAll(string searchText, string replaceWith, bool caseSensitive)
        {
            _lastSearchText = searchText;
            _lastReplaceText = replaceWith;
            _lastSearchCaseSensitive = caseSensitive;

            TabManager.CurrentTab()?.EditorHost.FindReplaceAll(searchText, replaceWith, caseSensitive);
        }

        #endregion

    }
}
