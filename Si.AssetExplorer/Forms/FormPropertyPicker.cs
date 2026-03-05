using Si.Library;

namespace Si.AssetExplorer.Forms
{
    public partial class FormPropertyPicker
        : Form
    {
        public string Value
        {
            get
            {
                if (comboBoxWorking.SelectedItem is ComboboxItem selectedItem)
                {
                    return selectedItem.Text;
                }
                throw new Exception("No value selected.");
            }
        }

        public FormPropertyPicker()
        {
            InitializeComponent();
        }

        class ComboboxItem
        {
            public string Text { get; set; }
            public override string ToString() => Text;

            public ComboboxItem(string text)
            {
                Text = text;
            }
        }

        public FormPropertyPicker(PropertyItem propertyItem)
        {
            InitializeComponent();

            Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            if (propertyItem.Attributes?.EnumType == null)
                throw new Exception("EnumType must be specified for enum properties.");

            var values = propertyItem.Attributes.PickList ?? [];

            object? selectedItem = null;

            foreach (var value in values)
            {
                var text = value.ToString();
                if (text != null && text != string.Empty)
                {
                    var item = new ComboboxItem(text);

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
