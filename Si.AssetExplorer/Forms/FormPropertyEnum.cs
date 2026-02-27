using Krypton.Toolkit;
using NTDLS.Helpers;
using Si.Library;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyEnum
        : KryptonForm
    {
        public int Value => ((ComboboxItem)kryptonComboBoxWorking.SelectedItem.EnsureNotNull()).Value;

        public FormPropertyEnum()
        {
            InitializeComponent();
        }

        class ComboboxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }
            public override string ToString() => Text;

            public ComboboxItem(string text, int value)
            {
                Text = text;
                Value = value;
            }
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
                    var item = new ComboboxItem(text, Convert.ToInt32(value));

                    if (text == propertyItem.WorkingValue?.ToString())
                    {
                        selectedItem = item;
                    }
                    kryptonComboBoxWorking.Items.Add(item);
                }
            }
            if (selectedItem != null)
            {
                kryptonComboBoxWorking.SelectedItem = selectedItem;
            }

            AcceptButton = kryptonButtonSave;
            CancelButton = kryptonButtonCancel;
        }

        private void KryptonButtonSave_Click(object sender, EventArgs e)
        {
            if (kryptonComboBoxWorking.SelectedItem == null)
            {
                MessageBox.Show("Please select a value.", SiConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
