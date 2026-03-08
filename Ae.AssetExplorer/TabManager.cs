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

        public AeTabPage AddTab(string assetKey)
        {
            var existingTab = FindTabByFileName(assetKey);
            if (existingTab != null)
            {
                TabControl.SelectedTab = existingTab;
                InvokeTabChanged(existingTab);
                return existingTab;
            }

            var asset = _engine.Assets.ReadAssetController(assetKey);

            //TODO: We should probably determine the code type based on the asset's base type or metadata.
            string codeText = asset.Controller ?? string.Empty;
            var codeType = AeCodeType.CSharp; // Default to C# for now, but this should be determined dynamically.

            var tabPage = new AeTabPage(assetKey, codeText, codeType);
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

        public bool CloseTab(AeTabPage tabPage)
        {
            if (tabPage.Editor.TextHasChanged)
            {
                if (MessageBox.Show($"The file '{tabPage.AssetKey}' has unsaved changes. Save before closing?",
                    AeConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Question) != DialogResult.OK)
                {
                    return false;
                }

                //TODO: Save the changes here:
                //_engine....
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
