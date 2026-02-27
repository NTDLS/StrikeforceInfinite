using Si.Library.Mathematics;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyVector
        : Form
    {
        public SiVector Value => new((float)kryptonNumericUpDownWorkingX.Value, (float)kryptonNumericUpDownWorkingY.Value);

        public FormPropertyVector()
        {
            InitializeComponent();
        }

        public FormPropertyVector(PropertyItem propertyItem)
        {
            InitializeComponent();
            kryptonLabelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            kryptonTextBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            kryptonNumericUpDownWorkingX.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            kryptonNumericUpDownWorkingX.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            kryptonNumericUpDownWorkingX.Value = (decimal?)(propertyItem.WorkingValue as SiVector)?.X ?? 0;

            kryptonNumericUpDownWorkingY.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            kryptonNumericUpDownWorkingY.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            kryptonNumericUpDownWorkingY.Value = (decimal?)(propertyItem.WorkingValue as SiVector)?.Y ?? 0;

            AcceptButton = kryptonButtonSave;
            CancelButton = kryptonButtonCancel;
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
