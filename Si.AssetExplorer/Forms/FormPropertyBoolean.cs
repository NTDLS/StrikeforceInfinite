namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyBoolean
        : Form
    {
        public bool Value => checkBoxWorking.Checked;

        public FormPropertyBoolean()
        {
            InitializeComponent();
        }

        public FormPropertyBoolean(PropertyItem propertyItem)
        {
            InitializeComponent();
            checkBoxWorking.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;
            checkBoxWorking.Checked = Convert.ToBoolean(propertyItem.WorkingValue ?? false);

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
