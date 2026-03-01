using Si.Library;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyRangeFloat
        : Form
    {
        public SiRange<float> Value => new((float)numericUpDownWorkingMin.Value, (float)numericUpDownWorkingMax.Value);

        public FormPropertyRangeFloat()
        {
            InitializeComponent();
        }

        public FormPropertyRangeFloat(PropertyItem propertyItem)
        {
            InitializeComponent();
            labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            numericUpDownWorkingMin.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            numericUpDownWorkingMin.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            numericUpDownWorkingMin.Value = (decimal?)(propertyItem.WorkingValue as SiRange<float>)?.Min ?? 0;

            numericUpDownWorkingMax.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            numericUpDownWorkingMax.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            numericUpDownWorkingMax.Value = (decimal?)(propertyItem.WorkingValue as SiRange<float>)?.Max ?? 0;

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
