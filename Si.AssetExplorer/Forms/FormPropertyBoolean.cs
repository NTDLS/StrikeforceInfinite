using Krypton.Toolkit;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyBoolean
        : KryptonForm
    {
        //DONE!

        public bool Value => kryptonCheckBoxWorking.Checked;

        public FormPropertyBoolean()
        {
            InitializeComponent();
        }

        public FormPropertyBoolean(PropertyItem propertyItem)
        {
            InitializeComponent();
            kryptonCheckBoxWorking.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            kryptonTextBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;
            kryptonCheckBoxWorking.Checked = Convert.ToBoolean(propertyItem.WorkingValue ?? propertyItem.DefaultValue);
            kryptonCheckBoxDefault.Checked = Convert.ToBoolean(propertyItem.DefaultValue);

            AcceptButton = kryptonButtonSave;
            CancelButton = kryptonButtonCancel;
        }

        private void KryptonButtonSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void KryptonButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
