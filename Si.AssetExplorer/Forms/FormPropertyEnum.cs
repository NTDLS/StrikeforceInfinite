using Krypton.Toolkit;
using static Si.Library.SiConstants;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyEnum
        : KryptonForm
    {
        public decimal Value => 0;// kryptonDropButtonWorking.Value;

        public FormPropertyEnum()
        {
            InitializeComponent();
        }

        public FormPropertyEnum(PropertyItem propertyItem)
        {
            InitializeComponent();
            kryptonLabelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            kryptonTextBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            if (propertyItem.Attributes?.EnumType == null)
                throw new Exception("EnumType must be specified for enum properties.");

            var values = Enum.GetValues(propertyItem.Attributes.EnumType);

            object? selectedItem = null;

            foreach (var value in values)
            {
                var text = value.ToString();
                if (text != null && text != string.Empty)
                {
                    if (text == propertyItem.WorkingValue?.ToString())
                    {
                        selectedItem = value;
                    }
                        kryptonComboBoxWorking.Items.Add(text);
                }
            }
            if (selectedItem != null)
            {
                kryptonComboBoxWorking.SelectedItem = selectedItem;
            }

            kryptonTextBoxDefault.Text = propertyItem.DefaultValue?.ToString();

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
