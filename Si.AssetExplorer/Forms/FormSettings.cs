using NTDLS.Persistence;
using NTDLS.WinFormsHelpers;
using Si.AssetExplorer.Controls;
using Si.Library;

namespace Si.AssetExplorer.Forms
{
    public partial class FormSettings : Form
    {
        private readonly SiCodeEditor _fontSampleTextbox;
        private readonly Graphics _graphics;

        public FormSettings()
        {
            InitializeComponent();

            _graphics = CreateGraphics();

            AcceptButton = buttonSave;
            CancelButton = buttonCancel;

            checkBoxLineNumbers.Checked = Settings.Instance.EditorShowLineNumbers;
            checkBoxWordWrap.Checked = Settings.Instance.EditorWordWrap;

            var sampleText = EmbeddedResource.Load("Samples/CSharpTextSample.txt");

            _fontSampleTextbox = new SiCodeEditor(panelFontSampleParent, sampleText);

            //FullyFeaturedCodeEditor.ApplyEditorSettings(_fontSampleTextbox);

            foreach (var font in FontFamily.Families)
            {
                if (IsMonospacedFont(font))
                {
                    comboBoxFont.Items.Add(font.Name);
                }
            }
            comboBoxFont.Text = Settings.Instance.EditorFontFamily;
            numericUpDownFontSize.Value = (decimal)Settings.Instance.EditorFontSize;

            numericUpDownFontSize.ValueChanged += (object? sender, EventArgs e) => UpdateFontSample();
            comboBoxFont.SelectedIndexChanged += (object? sender, EventArgs e) => UpdateFontSample();
            checkBoxLineNumbers.CheckedChanged += (object? sender, EventArgs e) => UpdateFontSample();
            checkBoxWordWrap.CheckedChanged += (object? sender, EventArgs e) => UpdateFontSample();

            Disposed += (object? sender, EventArgs e) => _graphics.Dispose();

            UpdateFontSample();
        }

        private bool IsMonospacedFont(FontFamily fontFamily)
        {
            using var font = new Font(fontFamily, 12);
            return _graphics.MeasureString("i", font).Width == _graphics.MeasureString("W", font).Width;
        }

        private void UpdateFontSample()
        {
            var selectedFontName = comboBoxFont.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedFontName) == false)
            {
                try
                {
                    var fontSize = numericUpDownFontSize.Value;
                    if (fontSize > 0)
                    {
                        _fontSampleTextbox.WordWrap = checkBoxWordWrap.Checked;
                        _fontSampleTextbox.ShowLineNumbers = checkBoxLineNumbers.Checked;
                        _fontSampleTextbox.FontFamily = new System.Windows.Media.FontFamily(selectedFontName);
                        _fontSampleTextbox.FontSize = (double)fontSize;
                    }
                }
                catch { }
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            try
            {
                var settings = new Settings()
                {
                    EditorFontSize = (double)numericUpDownFontSize.Value,
                    EditorShowLineNumbers = checkBoxLineNumbers.Checked,
                    EditorWordWrap = checkBoxWordWrap.Checked,
                    EditorFontFamily = comboBoxFont.Text
                };

                LocalUserApplicationData.SaveToDisk($"{SiConstants.FriendlyName}\\Si.AssetExplorer", settings);
                Settings.Instance = settings;

                this.InvokeClose(DialogResult.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, SiConstants.FriendlyName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.InvokeClose(DialogResult.Cancel);
        }
    }
}
