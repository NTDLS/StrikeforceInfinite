using Krypton.Toolkit;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyMultipleSpritePicker
        : KryptonForm
    {
        public decimal Value => kryptonNumericUpDown.Value;

        public FormPropertyMultipleSpritePicker()
        {
            InitializeComponent();
        }

        public FormPropertyMultipleSpritePicker(PropertyItem propertyItem)
        {
            InitializeComponent();

            //kryptonLabelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            //kryptonTextBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;
            //kryptonNumericUpDown.Value = Convert.ToDecimal(propertyItem.WorkingValue ?? 0);

            AcceptButton = kryptonButtonSave;
            CancelButton = kryptonButtonCancel;

            throw new System.NotImplementedException();
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
