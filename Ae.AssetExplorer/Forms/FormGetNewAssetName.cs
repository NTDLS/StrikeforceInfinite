using Ae.Library;
using static Ae.Library.AeConstants;

namespace Ae.AssetExplorer.Forms
{
    public partial class FormGetNewAssetName
        : Form
    {
        public string AssetName => textBoxAssetName.Text.Trim();

        public FormGetNewAssetName(Action<string, AeLoggingLevel?>? writeOutput)
        {
            InitializeComponent();
            AcceptButton = buttonCreate;
            CancelButton = buttonCancel;
        }

        private void ButtonCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxAssetName.Text.Trim()))
            {
                MessageBox.Show("Asset name cannot be empty.", AeConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (textBoxAssetName.Text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                MessageBox.Show("Asset name contains invalid characters.", AeConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
