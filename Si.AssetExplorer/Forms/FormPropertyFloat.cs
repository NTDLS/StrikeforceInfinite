using Krypton.Toolkit;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyFloat
        : KryptonForm
    {
        //DONE!

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

            kryptonNumericUpDownDefault.Minimum = decimal.MinValue;
            kryptonNumericUpDownDefault.Maximum = decimal.MaxValue;
            kryptonNumericUpDownDefault.Value = Convert.ToDecimal(propertyItem.DefaultValue);

            kryptonNumericUpDownWorking.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            kryptonNumericUpDownWorking.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            kryptonNumericUpDownWorking.Value = Convert.ToDecimal(propertyItem.WorkingValue ?? propertyItem.DefaultValue);

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
