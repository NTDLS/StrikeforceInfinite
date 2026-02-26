using Krypton.Toolkit;
using NTDLS.Persistence;
using NTDLS.WinFormsHelpers;
using Si.AssetExplorer.Controls;

namespace Si.AssetExplorer
{
    public partial class FormSettings : KryptonForm
    {
        public FormSettings()
        {
            InitializeComponent();

            #region Themes.

            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Cloud Black (Dark)", PaletteMode.Microsoft365BlackDarkMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Cloud Blue (Light)", PaletteMode.Microsoft365Blue));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Cloud Gray (Dark)", PaletteMode.Microsoft365Black));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Cloud Gray (Light)", PaletteMode.Microsoft365Silver));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Cloud Refuge (Dark)", PaletteMode.Microsoft365BlackDarkModeAlternate));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Cloud Silver (Dark)", PaletteMode.Microsoft365SilverDarkMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Cloud Silver (Light)", PaletteMode.Microsoft365SilverLightMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Cloud Stark (Light)", PaletteMode.Microsoft365BlueDarkMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Cloud Vibrant (Light)", PaletteMode.Microsoft365BlueLightMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Cloud White (Light)", PaletteMode.Microsoft365White));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Classic (Light)", PaletteMode.ProfessionalOffice2003));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Metro Azure (Light)", PaletteMode.Office2010Blue));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Metro Black (Dark)", PaletteMode.Office2010BlackDarkMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Metro Blue (Light)", PaletteMode.Office2010BlueDarkMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Metro Blue (Light)", PaletteMode.Office2010BlueLightMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Metro Gray (Dark)", PaletteMode.Office2010Black));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Metro Silver (Dark)", PaletteMode.Office2010SilverDarkMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Metro Silver (Light)", PaletteMode.Office2010SilverLightMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Metro Vibrant (Light)", PaletteMode.Office2010Silver));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Metro White (Light)", PaletteMode.Office2010White));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Modern (Light)", PaletteMode.Office2013White));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Slate Azure (Light)", PaletteMode.Office2007Blue));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Slate Black (Dark)", PaletteMode.Office2007BlackDarkMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Slate Blue (Light)", PaletteMode.Office2007BlueLightMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Slate Gray (Dark)", PaletteMode.Office2007Black));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Slate Gray (Light)", PaletteMode.Office2007Silver));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Slate Silver (Dark)", PaletteMode.Office2007SilverDarkMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Slate Silver (Light)", PaletteMode.Office2007SilverLightMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Slate Vibrant (Light)", PaletteMode.Office2007BlueDarkMode));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Office Slate White (Light)", PaletteMode.Office2007White));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Professional (Light)", PaletteMode.ProfessionalSystem));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Sparkle Blue (Dark)", PaletteMode.SparkleBlue));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Sparkle Orange (Dark)", PaletteMode.SparkleOrange));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Sparkle Purple (Dark)", PaletteMode.SparklePurple));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Studio Render Cloud (Light)", PaletteMode.VisualStudio2010Render365));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Studio Render Metro (Light)", PaletteMode.VisualStudio2010Render2010));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Studio Render Modern (Light)", PaletteMode.VisualStudio2010Render2013));
            kryptonComboBoxTheme.Items.Add(new ThemeComboItem("Studio Render Slate (Light)", PaletteMode.VisualStudio2010Render2007));

            foreach (var item in kryptonComboBoxTheme.Items)
            {
                if (item is ThemeComboItem themeItem && themeItem.Mode == Settings.Instance.Theme)
                {
                    kryptonComboBoxTheme.SelectedItem = item;
                    break;
                }
            }

            kryptonComboBoxTheme.SelectedIndexChanged += kryptonComboBoxTheme_SelectedIndexChanged;

            #endregion
        }

        private void kryptonComboBoxTheme_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (kryptonComboBoxTheme.SelectedItem is ThemeComboItem item)
            {
                Program.ThemeManager.GlobalPaletteMode = item.Mode;
                try
                {
                    foreach (Form form in Application.OpenForms)
                    {
                        form.BackColor = KryptonManager.CurrentGlobalPalette.GetBackColor1(PaletteBackStyle.PanelClient, PaletteState.Normal);
                    }
                }
                catch
                {
                }
            }
        }

        private void kryptonButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void kryptonButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var settings = LocalUserApplicationData.LoadFromDisk(Constants.AppName, new Settings());

                settings.Theme = (kryptonComboBoxTheme.SelectedItem as ThemeComboItem)?.Mode
                    ?? throw new ArgumentNullException("Theme must be selected.");

                Settings.Instance = settings;

                this.InvokeClose(DialogResult.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Constants.AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}