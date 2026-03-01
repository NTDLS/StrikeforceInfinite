namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyMultipleSpritePicker
        : Form
    {
        public decimal Value => numericUpDown.Value;

        public FormPropertyMultipleSpritePicker()
        {
            InitializeComponent();
        }

        public FormPropertyMultipleSpritePicker(PropertyItem propertyItem)
        {
            InitializeComponent();

            //labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            //textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;
            //numericUpDown.Value = Convert.ToDecimal(propertyItem.WorkingValue ?? 0);

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
