namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyFloat
        : Form
    {
        public float Value => Convert.ToSingle(numericUpDownWorking.Value);

        public FormPropertyFloat()
        {
            InitializeComponent();
        }

        public FormPropertyFloat(PropertyItem propertyItem)
        {
            InitializeComponent();
            labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            numericUpDownWorking.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            numericUpDownWorking.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            numericUpDownWorking.Value = Convert.ToDecimal(propertyItem.WorkingValue ?? 0f);

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
