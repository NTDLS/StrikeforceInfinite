using Si.Library;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyRangeInt
        : Form
    {
        public SiRange<int> Value => new((int)numericUpDownWorkingMin.Value, (int)numericUpDownWorkingMax.Value);

        public FormPropertyRangeInt()
        {
            InitializeComponent();
        }

        public FormPropertyRangeInt(PropertyItem propertyItem)
        {
            InitializeComponent();
            labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            numericUpDownWorkingMin.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? int.MinValue;
            numericUpDownWorkingMin.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? int.MaxValue;
            numericUpDownWorkingMin.Value = (decimal?)(propertyItem.WorkingValue as SiRange<int>)?.Min ?? 0;

            numericUpDownWorkingMax.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? int.MinValue;
            numericUpDownWorkingMax.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? int.MaxValue;
            numericUpDownWorkingMax.Value = (decimal?)(propertyItem.WorkingValue as SiRange<int>)?.Max ?? 0;

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
