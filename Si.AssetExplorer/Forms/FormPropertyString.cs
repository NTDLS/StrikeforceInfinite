namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyString
        : Form
    {
        public string Value => textBoxWorking.Text;

        public FormPropertyString()
        {
            InitializeComponent();
        }

        public FormPropertyString(PropertyItem propertyItem)
        {
            InitializeComponent();
            labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            textBoxWorking.MaxLength = (int?)propertyItem.Attributes?.MaxValue ?? int.MaxValue;
            textBoxWorking.Text = propertyItem.WorkingValue?.ToString() ?? string.Empty;

            AcceptButton = buttonSave;
            CancelButton = buttonCancel;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
