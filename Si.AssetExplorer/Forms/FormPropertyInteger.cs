using Krypton.Toolkit;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyInteger
        : KryptonForm
    {
        //DONE!

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

            kryptonNumericUpDownDefault.Minimum = int.MinValue;
            kryptonNumericUpDownDefault.Maximum = int.MaxValue;
            kryptonNumericUpDownDefault.Value = Convert.ToInt32(propertyItem.DefaultValue);

            kryptonNumericUpDownWorking.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? int.MinValue;
            kryptonNumericUpDownWorking.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? int.MaxValue;
            kryptonNumericUpDownWorking.Value = Convert.ToInt32(propertyItem.WorkingValue ?? propertyItem.DefaultValue);

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
