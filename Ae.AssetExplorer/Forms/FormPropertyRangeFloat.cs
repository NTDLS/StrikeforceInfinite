using Ae.Engine;
using Ae.Engine.Types;

namespace Ae.AssetExplorer.Forms
{
    public partial class FormPropertyRangeFloat
        : Form
    {
        public AeRange<float> Value => new((float)numericUpDownWorkingMin.Value, (float)numericUpDownWorkingMax.Value);

        public FormPropertyRangeFloat()
        {
            InitializeComponent();
        }

        public FormPropertyRangeFloat(WriteLogDelegate writeLog, PropertyItem propertyItem)
        {
            InitializeComponent();

            Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            numericUpDownWorkingMin.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            numericUpDownWorkingMin.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            numericUpDownWorkingMin.Value = (decimal?)(propertyItem.WorkingValue as AeRange<float>)?.Min ?? 0;

            numericUpDownWorkingMax.Minimum = (decimal?)propertyItem.Attributes?.MinValue ?? decimal.MinValue;
            numericUpDownWorkingMax.Maximum = (decimal?)propertyItem.Attributes?.MaxValue ?? decimal.MaxValue;
            numericUpDownWorkingMax.Value = (decimal?)(propertyItem.WorkingValue as AeRange<float>)?.Max ?? 0;

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
