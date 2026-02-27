using Krypton.Toolkit;
using Si.Library;
using Si.Library.Mathematics;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyRangeInt
        : KryptonForm
    {
        //DONE!

        public SiRange<int> Value => new((int)kryptonNumericUpDownWorkingMin.Value, (int)kryptonNumericUpDownWorkingMax.Value);

        public FormPropertyRangeInt()
        {
            InitializeComponent();
        }

        public FormPropertyRangeInt(PropertyItem propertyItem)
        {
            InitializeComponent();
            kryptonLabelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            kryptonTextBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            kryptonNumericUpDownDefaultMin.Minimum = decimal.MinValue;
            kryptonNumericUpDownDefaultMin.Maximum = decimal.MaxValue;
            kryptonNumericUpDownDefaultMin.Value = (decimal?)(propertyItem.DefaultValue as SiRange<int>)?.Min ?? 0;

            kryptonNumericUpDownDefaultMax.Minimum = decimal.MinValue;
            kryptonNumericUpDownDefaultMax.Maximum = decimal.MaxValue;
            kryptonNumericUpDownDefaultMax.Value = (decimal?)(propertyItem.DefaultValue as SiRange<int>)?.Max ?? 0;

            kryptonNumericUpDownWorkingMin.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? int.MinValue;
            kryptonNumericUpDownWorkingMin.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? int.MaxValue;
            kryptonNumericUpDownWorkingMin.Value = (decimal?)(propertyItem.WorkingValue as SiRange<int>)?.Min ?? 0;

            kryptonNumericUpDownWorkingMax.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? int.MinValue;
            kryptonNumericUpDownWorkingMax.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? int.MaxValue;
            kryptonNumericUpDownWorkingMax.Value = (decimal?)(propertyItem.WorkingValue as SiRange<int>)?.Max ?? 0;

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
