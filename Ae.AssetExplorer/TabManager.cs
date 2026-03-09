using Ae.AssetExplorer.Controls;
using Ae.AssetExplorer.Properties;
using Ae.Engine;
using Ae.Library;
using static Ae.Library.AeConstants;

namespace Ae.AssetExplorer
{
    internal class TabManager
    {
        public TabControl TabControl { get; private set; }
        private readonly Action<AeTabPage> _tabSelected;
        private readonly AeEngine _engine;
        private AeTabPage? _lastSelectedTab; //Just so that we dont keep reloading the same tab on selection.

        public TabManager(AeEngine engine, TabControl tabControl, Action<AeTabPage> tabSelected)
        {
            _tabSelected = tabSelected;
            TabControl = tabControl;
            _engine = engine;

            TabControl.MouseUp += TabControl_MouseUp;
            tabControl.Selected += (object? sender, TabControlEventArgs e) => InvokeTabChanged(tabControl.SelectedTab as AeTabPage);
        }

        /// <summary>
        /// Tells the owner form that a tab has been selected, so that it can update the property grid and other UI elements accordingly.
        /// Guards against re-invoking the event if the same tab is selected again, to avoid unnecessary UI updates.
        /// </summary>
        private void InvokeTabChanged(AeTabPage? tabPage)
        {
            if (tabPage != null && tabPage != _lastSelectedTab)
            {
                _lastSelectedTab = tabPage;
                _tabSelected.Invoke(tabPage);
            }
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
                InvokeTabChanged(existingTab);
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

            var tabPage = new AeTabPage(node.AssetKey, textContent ?? string.Empty, baseType, codeType);
            TabControl.TabPages.Add(tabPage);
            TabControl.SelectedTab = tabPage;
            InvokeTabChanged(tabPage);
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

        public bool SaveCurrentTab()
        {
            if (TabControl.SelectedTab is AeTabPage tabPage)
            {
                return SaveTab(tabPage);
            }
            return false;
        }

        public bool SaveTab(AeTabPage tabPage)
        {
            switch (tabPage.BaseType)
            {
                case AeBaseAssetType.Image:
                    //Images are sprites, and when we edit those we are editing their controller - not their bytes.
                    _engine.Assets.WriteAssetControllerFromText(tabPage.AssetKey, tabPage.Editor.Text);
                    tabPage.Editor.SetUnmodified();
                    break;
                case AeBaseAssetType.Text:
                case AeBaseAssetType.Code:
                    //Text type assets are written to the bytes as that is their native type.
                    _engine.Assets.WriteAssetBytesFromText(tabPage.AssetKey, tabPage.Editor.Text);
                    tabPage.Editor.SetUnmodified();
                    break;
                case AeBaseAssetType.Sound:
                    break;
                default:
                    throw new Exception("Unsupported asset type: " + tabPage.BaseType);
            }

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
            if (tabPage.Editor.TextHasChanged)
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

        public void RemoveTab(AeTabPage tabPage)
        {
            TabControl.TabPages.Remove(tabPage);
        }

        public void ClearTabs()
        {
            TabControl.TabPages.Clear();
        }
    }
}
