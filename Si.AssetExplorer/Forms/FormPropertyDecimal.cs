using Krypton.Toolkit;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyDecimal
        : KryptonForm
    {
        public decimal Value => kryptonNumericUpDown.Value;

        public FormPropertyDecimal()
        {
            InitializeComponent();
        }

        public FormPropertyDecimal(PropertyItem propertyItem)
        {
            InitializeComponent();
            kryptonLabelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            kryptonTextBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;
            kryptonNumericUpDown.Value = Convert.ToDecimal(propertyItem.WorkingValue ?? propertyItem.DefaultValue);
            kryptonNumericUpDownDefaultValue.Value = Convert.ToDecimal(propertyItem.DefaultValue);
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
