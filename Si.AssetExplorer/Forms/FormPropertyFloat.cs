namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyFloat
        : Form
    {
        public float Value => Convert.ToSingle(kryptonNumericUpDownWorking.Value);

        public FormPropertyFloat()
        {
            InitializeComponent();
        }

        public FormPropertyFloat(PropertyItem propertyItem)
        {
            InitializeComponent();
            kryptonLabelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            kryptonTextBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            kryptonNumericUpDownWorking.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            kryptonNumericUpDownWorking.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            kryptonNumericUpDownWorking.Value = Convert.ToDecimal(propertyItem.WorkingValue ?? 0f);

            AcceptButton = kryptonButtonSave;
            CancelButton = kryptonButtonCancel;
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
