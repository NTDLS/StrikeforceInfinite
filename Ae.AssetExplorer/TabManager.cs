using Ae.AssetExplorer.Controls;
using Ae.AssetExplorer.Properties;
using Ae.Engine;

namespace Ae.AssetExplorer
{
    internal class TabManager
    {
        public TabControl TabControl { get; private set; }
        private readonly AeEngine _engine;
        private AeTabPage? _lastSelectedTab; //Just so that we don't keep reloading the same tab on selection.
        private readonly FormMain _formMain;

        public delegate void TabSelectedEventHandler(TabManager tabManager, AeTabPage? tabPage);
        /// <summary>
        /// Tells the owner form that a tab has been selected, so that it can update the property grid and other UI elements accordingly.
        /// Guards against re-invoking the event if the same tab is selected again, to avoid unnecessary UI updates.
        /// </summary>
        public event TabSelectedEventHandler? TabSelected;

        public TabManager(FormMain formMain, AeEngine engine, TabControl tabControl)
        {
            _formMain = formMain;
            TabControl = tabControl;
            _engine = engine;

            TabControl.MouseUp += TabControl_MouseUp;
            tabControl.Selected += (object? sender, TabControlEventArgs e)
                => TabSelected?.Invoke(this, tabControl.SelectedTab as AeTabPage);
        }

        private void TabControl_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (GetClickedTab(e.Location) is AeTabPage clickedTab)
                {
                    var popupMenu = new ContextMenuStrip();

                    popupMenu.Items.Add("Close", FormUtility.TransparentImage(Resources.ToolCloseFile)).Click += (s, e) => CloseTab(clickedTab);
                    popupMenu.Items.Add("-");
                    popupMenu.Items.Add("Close All but This", FormUtility.TransparentImage(Resources.ToolCloseFile)).Click += (s, e) => CloseAllButThisTab(clickedTab);
                    popupMenu.Items.Add("Close All", FormUtility.TransparentImage(Resources.ToolCloseFile)).Click += (s, e) => CloseAllTabs();
                    popupMenu.Show(TabControl, e.Location);
                }
            }
        }

        private AeTabPage? GetClickedTab(Point mouseLocation)
        {
            for (int i = 0; i < TabControl.TabCount; i++)
            {
                if (TabControl.GetTabRect(i).Contains(mouseLocation))
                {
                    return TabControl.TabPages[i] as AeTabPage;
                }
            }
            return null;
        }

        public AeTabPage AddTab(AeTreeNode node)
        {
            var existingTab = FindTabByFileName(node.AssetKey);
            if (existingTab != null)
            {
                TabControl.SelectedTab = existingTab;
                TabSelected?.Invoke(this, existingTab);
                return existingTab;
            }

            var textContent = string.Empty;

            var asset = _engine.Assets.GetAsset(node.AssetKey);

            if (!AeConstants.BaseAssetTypes.TryGetValue(asset.BaseType, out var baseType))
            {
                throw new Exception("Unsupported asset type: " + asset.BaseType);
            }

            var codeType = AeCodeType.Text;

            switch (baseType)
            {
                case AeBaseAssetType.Image:
                    textContent = _engine.Assets.ReadAssetController(node.AssetKey);
                    codeType = AeCodeType.CSharp;
                    break;
                case AeBaseAssetType.Text:
                    textContent = asset.Object as string; //Text assets are stored as strings, so we can just cast the object to a string.
                    switch (asset.BaseType.ToLower())
                    {
                        case "txt":
                            codeType = AeCodeType.Text;
                            break;
                        case "json":
                            codeType = AeCodeType.Text;
                            break;
                        case "xml":
                            codeType = AeCodeType.XML;
                            break;
                    }
                    break;
                case AeBaseAssetType.Code:
                    textContent = asset.Object as string; //Code assets are stored as strings, so we can just cast the object to a string.
                    codeType = AeCodeType.CSharp;
                    break;
                case AeBaseAssetType.Sound:
                    textContent = string.Empty; //TODO: We should probably have a different editor for sound assets, but for now we'll just show an empty editor.
                    break;
            }

            var tabPage = new AeTabPage(_formMain, node.AssetKey, textContent ?? string.Empty, baseType, codeType);
            TabControl.TabPages.Add(tabPage);
            TabControl.SelectedTab = tabPage;
            TabSelected?.Invoke(this, tabPage);
            return tabPage;
        }

        private AeTabPage? FindTabByFileName(string assetKey)
        {
            foreach (var tab in TabControl.TabPages.OfType<AeTabPage>())
            {
                if (tab.AssetKey.Equals(assetKey, StringComparison.InvariantCultureIgnoreCase))
                {
                    return tab;
                }
            }
            return null;
        }

        public AeTabPage? CurrentTab()
        {
            if (TabControl.SelectedTab is AeTabPage tabPage)
            {
                return tabPage;
            }
            return null;
        }

        public bool SaveCurrentTab()
        {
            if (TabControl.SelectedTab is AeTabPage tabPage)
            {
                return SaveTab(tabPage);
            }
            return false;
        }

        public void SaveAllTabs()
        {
            foreach (AeTabPage tab in TabControl.TabPages)
            {
                if (SaveTab(tab) == false)
                {
                    break;
                }
            }
        }

        public bool SaveTab(AeTabPage tabPage)
        {
            switch (tabPage.BaseType)
            {
                case AeBaseAssetType.Image:
                    //Images are sprites, and when we edit those we are editing their controller - not their bytes.
                    _engine.Assets.WriteAssetControllerFromText(tabPage.AssetKey, tabPage.EditorHost.Text);
                    tabPage.EditorHost.SetUnmodified();
                    break;
                case AeBaseAssetType.Text:
                case AeBaseAssetType.Code:
                    //Text type assets are written to the bytes as that is their native type.
                    _engine.Assets.WriteAssetBytesFromText(tabPage.AssetKey, tabPage.EditorHost.Text);
                    tabPage.EditorHost.SetUnmodified();
                    break;
                case AeBaseAssetType.Sound:
                    break;
                default:
                    throw new Exception("Unsupported asset type: " + tabPage.BaseType);
            }

            TabSelected?.Invoke(this, tabPage);

            return true;
        }

        public void CloseAllTabs()
        {
            while (TabControl.TabCount > 0)
            {
                if (CloseTab((AeTabPage)TabControl.TabPages[0]) == false)
                {
                    break;
                }
            }
        }

        public bool CloseCurrentTab()
        {
            if (TabControl.SelectedTab is AeTabPage tabPage)
            {
                return CloseTab(tabPage);
            }
            return false;
        }

        public bool CloseTab(AeTabPage tabPage)
        {
            if (tabPage.EditorHost.TextHasChanged)
            {
                var saveAnswer = MessageBox.Show($"Save '{tabPage.AssetKey}' before closing?",
                    AeConstants.FriendlyName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (saveAnswer == DialogResult.Cancel)
                {
                    return false;
                }
                else if (saveAnswer == DialogResult.Yes)
                {
                    if (!SaveTab(tabPage))
                    {
                        return false;
                    }
                }
                else if (saveAnswer == DialogResult.No)
                {
                    //Continue to close.
                }
            }

            TabControl.TabPages.Remove(tabPage);
            TabSelected?.Invoke(this, null);
            return true;
        }

        public void CloseAllButThisTab(AeTabPage tabPage)
        {
            var tabsToClose = new List<AeTabPage>();

            foreach (var tabFilePage in TabControl.TabPages.OfType<AeTabPage>())
            {
                if (tabFilePage != tabPage)
                {
                    tabsToClose.Add(tabFilePage);
                }
            }

            foreach (var tabFilePage in tabsToClose)
            {
                if (CloseTab(tabFilePage) == false)
                {
                    break;
                }
            }
        }

        #region Text editor stuff.

        public void IncreaseCurrentTabIndent()
            => CurrentTab()?.EditorHost.IncreaseCurrentTabIndent();

        public void DecreaseCurrentTabIndent()
            => CurrentTab()?.EditorHost.DecreaseCurrentTabIndent();

        public void CommentSelection()
            => CurrentTab()?.EditorHost.CommentSelection();

        public void UncommentSelection()
            => CurrentTab()?.EditorHost.UncommentSelection();

        public void Redo()
            => CurrentTab()?.EditorHost.Editor.Redo();

        public void Undo()
            => CurrentTab()?.EditorHost.Editor.Undo();

        public void Cut()
            => CurrentTab()?.EditorHost.Editor.Cut();

        public void Copy()
            => CurrentTab()?.EditorHost.Editor.Copy();

        public void Paste()
            => CurrentTab()?.EditorHost.Editor.Paste();

        public bool FindNext(string searchText, bool caseSensitive)
            => CurrentTab()?.EditorHost.FindNext(searchText, caseSensitive) == true;

        public void FindReplace(string searchText, string replaceWith, bool caseSensitive)
            => CurrentTab()?.EditorHost.FindReplace(searchText, replaceWith, caseSensitive);

        public void FindReplaceAll(string searchText, string replaceWith, bool caseSensitive)
            => CurrentTab()?.EditorHost.FindReplaceAll(searchText, replaceWith, caseSensitive);

        #endregion
    }
}
