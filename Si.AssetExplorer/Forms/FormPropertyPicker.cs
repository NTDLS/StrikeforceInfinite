using Krypton.Toolkit;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyPicker
        : KryptonForm
    {
        public decimal Value => kryptonNumericUpDown.Value;

        public FormPropertyPicker()
        {
            InitializeComponent();
        }

        public FormPropertyPicker(PropertyItem propertyItem)
        {
            InitializeComponent();
            kryptonLabelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            kryptonTextBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;
            kryptonNumericUpDown.Value = Convert.ToDecimal(propertyItem.WorkingValue ?? propertyItem.DefaultValue);
            kryptonNumericUpDownDefaultValue.Value = Convert.ToDecimal(propertyItem.DefaultValue);

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
