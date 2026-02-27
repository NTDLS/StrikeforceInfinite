namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyInteger
        : Form
    {
        public int Value => Convert.ToInt32(kryptonNumericUpDownWorking.Value);

        public FormPropertyInteger()
        {
            InitializeComponent();
        }

        public FormPropertyInteger(PropertyItem propertyItem)
        {
            InitializeComponent();
            kryptonLabelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            kryptonTextBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            kryptonNumericUpDownWorking.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? int.MinValue;
            kryptonNumericUpDownWorking.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? int.MaxValue;
            kryptonNumericUpDownWorking.Value = Convert.ToInt32(propertyItem.WorkingValue ?? 0);

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
