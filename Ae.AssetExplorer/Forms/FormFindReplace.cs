namespace Ae.AssetExplorer.Forms
{
    public partial class FormFindReplace : Form
    {
        private readonly FormMain? _formMain;
        private bool _isFirstLoad = true;

        public enum FindType
        {
            Find,
            Replace
        }

        public FormFindReplace()
        {
            InitializeComponent();
        }

        public FormFindReplace(FormMain formMain, string searchText, string replaceText)
        {
            InitializeComponent();
            _formMain = formMain;
            textBoxFindText.Text = searchText;
            textBoxFindReplaceText.Text = replaceText;
            Owner = formMain;

            Activated += (object? sender, EventArgs e) => Opacity = 1.0;
            Deactivate += (object? sender, EventArgs e) =>
            {
                if (!Disposing)
                {
                    Opacity = 0.75;
                }
            };

            tabControlBody.SelectedIndexChanged += TabControlBody_SelectedIndexChanged;
        }

        public void Show(FindType findType, string? defaultFindText)
        {
            Show();

            if (findType == FindType.Find)
            {
                Text = "Find";

                AcceptButton = buttonFind_FindNext;
                CancelButton = buttonFind_Close;
                tabControlBody.SelectedTab = tabPageFind;

                if (string.IsNullOrEmpty(defaultFindText) == false)
                {
                    textBoxFindText.Text = defaultFindText;
                }
                textBoxFindText.Focus();
            }
            if (findType == FindType.Replace)
            {
                Text = "Replace";

                AcceptButton = buttonReplace_FindNext;
                CancelButton = buttonReplace_Close;
                tabControlBody.SelectedTab = tabPageReplace;

                if (string.IsNullOrEmpty(defaultFindText) == false)
                {
                    textBoxFindReplaceText.Text = defaultFindText;
                }
                textBoxFindReplaceText.Focus();
            }
        }

        private void FormFind_Load(object sender, EventArgs e)
        {
            if (Owner != null && _isFirstLoad)
            {
                var currentTab = _formMain?.TabManager.CurrentTab();
                if (currentTab != null)
                {
                    var absolutePoint = currentTab.Parent?.PointToScreen(currentTab.Location);
                    if (absolutePoint != null && absolutePoint.HasValue)
                    {
                        //Place the find form in a reasonable location.
                        Location = new Point(
                            (absolutePoint.Value.X + currentTab.Width) - (Width + 50),
                            absolutePoint.Value.Y + 50);
                    }
                }
            }
            _isFirstLoad = false;
        }

        private void TabControlBody_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControlBody.SelectedTab == tabPageFind)
            {
                AcceptButton = buttonFind_FindNext;
                CancelButton = buttonFind_Close;
                textBoxFindText.Focus();
                Text = "Find";
            }
            else if (tabControlBody.SelectedTab == tabPageReplace)
            {
                AcceptButton = buttonReplace_FindNext;
                CancelButton = buttonReplace_Close;
                textBoxFindReplaceText.Focus();
                Text = "Replace";
            }
        }

        private void FormFind_FormClosing(object? sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void ButtonFindNext_Click(object sender, EventArgs e)
            => _formMain?.FindNext(textBoxFindText.Text, checkBoxFindCaseSensitive.Checked);

        private void ButtonReplace_FindNext_Click(object sender, EventArgs e)
            => _formMain?.FindNext(textBoxFindReplaceText.Text, checkBoxFindReplaceCaseSensitive.Checked);

        private void ButtonReplace_Replace_Click(object sender, EventArgs e)
            => _formMain?.FindReplace(textBoxFindReplaceText.Text, textBoxFindReplaceWithText.Text, checkBoxFindReplaceCaseSensitive.Checked);

        private void ButtonReplace_ReplaceAll_Click(object sender, EventArgs e)
            => _formMain?.FindReplaceAll(textBoxFindReplaceText.Text, textBoxFindReplaceWithText.Text, checkBoxFindReplaceCaseSensitive.Checked);

        private void ButtonClose_Click(object sender, EventArgs e)
            => Hide();
        private void ButtonReplace_Close_Click(object sender, EventArgs e)
            => Hide();
    }
}
