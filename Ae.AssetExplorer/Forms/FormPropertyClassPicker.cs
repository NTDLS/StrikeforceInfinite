using Ae.Library;
using Ae.Library.Compiler;
using Ae.Library.Metadata;
using System.Reflection;
using static Ae.Library.AeConstants;

namespace Ae.AssetExplorer.Forms
{
    public partial class FormPropertyClassPicker
        : Form
    {
        public string Value
        {
            get
            {
                if (comboBoxWorking.SelectedItem is ComboboxItem selectedItem)
                {
                    return selectedItem.Value;
                }
                throw new Exception("No value selected.");
            }
        }

        public FormPropertyClassPicker()
        {
            InitializeComponent();
        }

        class ComboboxItem
        {
            public string Value { get; set; }
            public string Text { get; set; }
            public override string ToString() => Text;

            public ComboboxItem(string text, string value)
            {
                Value = value;
                Text = text;
            }
        }

        public FormPropertyClassPicker(Action<string, AeLoggingLevel?>? writeOutput, PropertyItem propertyItem)
        {
            InitializeComponent();

            Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            labelName.Text = propertyItem.Attributes?.FriendlyName ?? propertyItem.Name;
            textBoxDescription.Text = propertyItem.Attributes?.Description ?? string.Empty;

            var results = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Select(t => new
                {
                    Type = t,
                    Attribute = t.GetCustomAttribute<AssetClassAttribute>()
                })
                .Where(x => x.Attribute != null
                    &&
                        //We do not show the dynamically compiled asset classes. These are the classes from the "Controller" field of an asset.
                        x.Type.IsAssignableTo(typeof(IAeRuntimeCompiledSpriteAsset)) == false
                    )
                .Distinct()
                .ToList();

            object? selectedItem = null;

            foreach (var value in results)
            {
                if (value.Attribute == null)
                {
                    continue;
                }

                var text = $"{value.Attribute.FriendlyName} ({value.Type.Name})";

                if (value.Type.IsAssignableTo(typeof(IAeRuntimeCompiledCodeAsset)))
                {
                    //This is compiled user code, so we want to show the user friendly name of the asset instead of the class name.
                    text = $"{value.Attribute.FriendlyName} ({AeReflection.GetStaticPropertyValue(value.Type.Name, "AeFriendlyName")})";
                }

                if (text != null && text != string.Empty)
                {
                    var item = new ComboboxItem(text, value.Type.Name);

                    if (value.Type.Name == propertyItem.WorkingValue?.ToString())
                    {
                        selectedItem = item;
                    }
                    comboBoxWorking.Items.Add(item);
                }
            }

            comboBoxWorking.Sorted = true;

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
                MessageBox.Show("Please select a value.", AeConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
