using Si.Library.Mathematics;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyVector
        : Form
    {
        public SiVector Value => new((float)numericUpDownWorkingX.Value, (float)numericUpDownWorkingY.Value);

        public FormPropertyVector()
        {
            InitializeComponent();
        }

        public FormPropertyVector(PropertyItem propertyItem)
        {
            InitializeComponent();
            labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            numericUpDownWorkingX.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            numericUpDownWorkingX.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            numericUpDownWorkingX.Value = (decimal?)(propertyItem.WorkingValue as SiVector)?.X ?? 0;

            numericUpDownWorkingY.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            numericUpDownWorkingY.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            numericUpDownWorkingY.Value = (decimal?)(propertyItem.WorkingValue as SiVector)?.Y ?? 0;

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
