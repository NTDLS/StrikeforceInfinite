using Si.Library;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyEnum
        : Form
    {
        public object Value
        {
            get
            {
                if (comboBoxWorking.SelectedItem is ComboboxItem selectedItem)
                {
                    return Enum.ToObject(_propertyItem?.Attributes?.EnumType ?? throw new Exception("EnumType must be specified for enum properties."),
                        selectedItem.Value);
                }
                throw new Exception("No value selected.");
            }
        }

        private PropertyItem? _propertyItem;

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

            _propertyItem = propertyItem;

            labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

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
                    comboBoxWorking.Items.Add(item);
                }
            }
            if (selectedItem != null)
            {
                comboBoxWorking.SelectedItem = selectedItem;
            }

            AcceptButton = buttonSave;
            CancelButton = buttonCancel;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (comboBoxWorking.SelectedItem == null)
            {
                MessageBox.Show("Please select a value.", SiConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

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
