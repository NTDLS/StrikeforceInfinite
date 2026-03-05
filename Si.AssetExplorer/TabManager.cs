using Si.AssetExplorer.Controls;
using Si.AssetExplorer.Properties;
using Si.Engine;
using Si.Library;
using static Si.Library.SiConstants;

namespace Si.AssetExplorer
{
    internal class TabManager
    {
        public TabControl TabControl { get; private set; }
        private readonly Action<SiTabPage> _tabSelected;
        private readonly EngineCore _engine;
        private SiTabPage? _lastSelectedTab; //Just so that we dont keep reloading the same tab on selection.

        public TabManager(EngineCore engine, TabControl tabControl, Action<SiTabPage> tabSelected)
        {
            _tabSelected = tabSelected;
            TabControl = tabControl;
            _engine = engine;

            TabControl.MouseUp += TabControl_MouseUp;
            tabControl.Selected += (object? sender, TabControlEventArgs e) => InvokeTabChanged(tabControl.SelectedTab as SiTabPage);
        }

        private void InvokeTabChanged(SiTabPage? tabPage)
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
                if (GetClickedTab(e.Location) is SiTabPage clickedTab)
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

        private SiTabPage? GetClickedTab(Point mouseLocation)
        {
            for (int i = 0; i < TabControl.TabCount; i++)
            {
                if (TabControl.GetTabRect(i).Contains(mouseLocation))
                {
                    return TabControl.TabPages[i] as SiTabPage;
                }
            }
            return null;
        }

        public SiTabPage AddTab(string assetKey)
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
            var codeType = SiCodeType.CSharp; // Default to C# for now, but this should be determined dynamically.

            var tabPage = new SiTabPage(assetKey, codeText, codeType);
            TabControl.TabPages.Add(tabPage);
            TabControl.SelectedTab = tabPage;
            InvokeTabChanged(tabPage);
            return tabPage;
        }

        private SiTabPage? FindTabByFileName(string assetKey)
        {
            foreach (var tab in TabControl.TabPages.OfType<SiTabPage>())
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
                if (CloseTab((SiTabPage)TabControl.TabPages[0]) == false)
                {
                    break;
                }
            }
        }

        public bool CloseTab(SiTabPage tabPage)
        {
            if (tabPage.Editor.TextHasChanged)
            {
                if (MessageBox.Show($"The file '{tabPage.AssetKey}' has unsaved changes. Save before closing?",
                    SiConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Question) != DialogResult.OK)
                {
                    return false;
                }

                //TODO: Save the changes here:
                //_engine....
            }

            TabControl.TabPages.Remove(tabPage);
            return true;
        }

        public void CloseAllButThisTab(SiTabPage tabPage)
        {
            var tabsToClose = new List<SiTabPage>();

            foreach (var tabFilePage in TabControl.TabPages.OfType<SiTabPage>())
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

        public void RemoveTab(SiTabPage tabPage)
        {
            TabControl.TabPages.Remove(tabPage);
        }

        public void ClearTabs()
        {
            TabControl.TabPages.Clear();
        }
    }
}
