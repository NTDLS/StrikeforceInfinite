using Si.Library;

namespace Si.AssetExplorer.Forms
{
    public partial class FormCreateFolder
        : Form
    {
        public string FolderName => textBoxFolderName.Text;

        public FormCreateFolder()
        {
            InitializeComponent();
            AcceptButton = buttonCreate;
            CancelButton = buttonCancel;
        }

        private void ButtonCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxFolderName.Text))
            {
                MessageBox.Show("Folder name cannot be empty.", SiConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
