namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyPicker
        : Form
    {
        public decimal Value => numericUpDown.Value;

        public FormPropertyPicker()
        {
            InitializeComponent();
        }

        public FormPropertyPicker(PropertyItem propertyItem)
        {
            InitializeComponent();
            //labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            //textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;
            //numericUpDown.Value = Convert.ToDecimal(propertyItem.WorkingValue ?? propertyItem.DefaultValue);

            AcceptButton = buttonSave;
            CancelButton = buttonCancel;

            throw new System.NotImplementedException();
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
