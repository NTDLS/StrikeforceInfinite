using Krypton.Toolkit;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyString
        : KryptonForm
    {
        public string Value => kryptonTextBoxWorking.Text;

        public FormPropertyString()
        {
            InitializeComponent();
        }

        public FormPropertyString(PropertyItem propertyItem)
        {
            InitializeComponent();
            kryptonLabelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            kryptonTextBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            kryptonTextBoxWorking.MaxLength = (int?)propertyItem.Attributes?.MaxValue ?? int.MaxValue;
            kryptonTextBoxWorking.Text = propertyItem.WorkingValue?.ToString() ?? string.Empty;

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
