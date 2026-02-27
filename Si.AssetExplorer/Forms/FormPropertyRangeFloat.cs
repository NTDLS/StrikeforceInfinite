using Krypton.Toolkit;
using Si.Library;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyRangeFloat
        : KryptonForm
    {
        public SiRange<float> Value => new((float)kryptonNumericUpDownWorkingMin.Value, (float)kryptonNumericUpDownWorkingMax.Value);

        public FormPropertyRangeFloat()
        {
            InitializeComponent();
        }

        public FormPropertyRangeFloat(PropertyItem propertyItem)
        {
            InitializeComponent();
            kryptonLabelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            kryptonTextBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            kryptonNumericUpDownWorkingMin.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            kryptonNumericUpDownWorkingMin.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            kryptonNumericUpDownWorkingMin.Value = (decimal?)(propertyItem.WorkingValue as SiRange<float>)?.Min ?? 0;

            kryptonNumericUpDownWorkingMax.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            kryptonNumericUpDownWorkingMax.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            kryptonNumericUpDownWorkingMax.Value = (decimal?)(propertyItem.WorkingValue as SiRange<float>)?.Max ?? 0;

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
