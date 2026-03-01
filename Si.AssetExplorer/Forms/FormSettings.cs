using NTDLS.Persistence;
using NTDLS.WinFormsHelpers;

namespace Si.AssetExplorer
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var settings = LocalUserApplicationData.LoadFromDisk(Constants.AppName, new Settings());

                //settings.SomeSetting = comboBoxSomeSetting.SelectedItem?.ToString()

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