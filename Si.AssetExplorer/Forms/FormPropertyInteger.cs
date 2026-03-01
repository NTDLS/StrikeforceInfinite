namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyInteger
        : Form
    {
        public int Value => Convert.ToInt32(numericUpDownWorking.Value);

        public FormPropertyInteger()
        {
            InitializeComponent();
        }

        public FormPropertyInteger(PropertyItem propertyItem)
        {
            InitializeComponent();
            labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            numericUpDownWorking.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? int.MinValue;
            numericUpDownWorking.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? int.MaxValue;
            numericUpDownWorking.Value = Convert.ToInt32(propertyItem.WorkingValue ?? 0);

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
