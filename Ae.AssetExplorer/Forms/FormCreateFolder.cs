using Ae.Engine;
using static Ae.Engine.AeConstants;

namespace Ae.AssetExplorer.Forms
{
    public partial class FormCreateFolder
        : Form
    {
        public string FolderName => textBoxFolderName.Text.Trim();

        public FormCreateFolder(Action<string, AeLoggingLevel?>? writeOutput)
        {
            InitializeComponent();
            AcceptButton = buttonCreate;
            CancelButton = buttonCancel;
        }

        private void ButtonCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxFolderName.Text.Trim()))
            {
                MessageBox.Show("Folder name cannot be empty.", AeConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (textBoxFolderName.Text.Trim().IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                MessageBox.Show("Folder name contains invalid characters.", AeConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
