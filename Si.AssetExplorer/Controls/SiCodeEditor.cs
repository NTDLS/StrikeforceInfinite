using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Si.Library;
using System.ComponentModel;
using System.Xml;
using static Si.Library.SiConstants;

namespace Si.AssetExplorer.Controls
{
    internal class SiCodeEditor
        : System.Windows.Forms.Integration.ElementHost
    {
        public TextEditor Editor { get; private set; }
        public bool TextHasChanged { get; private set; } = false;

        #region Passthrough properties.

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new string Text
        {
            get => Editor.Text;
            set => Editor.Text = value;
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool WordWrap
        {
            get => Editor.WordWrap;
            set => Editor.WordWrap = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowLineNumbers
        {
            get => Editor.ShowLineNumbers;
            set => Editor.ShowLineNumbers = value;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Windows.Media.FontFamily FontFamily
        {
            get => Editor.FontFamily;
            set => Editor.FontFamily = value;
        }


        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public double FontSize
        {
            get => Editor.FontSize;
            set => Editor.FontSize = value;
        }

        #endregion

        public SiCodeEditor(Control parent, SiCodeType codeType, string text)
            : this(parent, codeType)
        {
            Text = text;
        }

        public SiCodeEditor(Control parent, SiCodeType codeType)
        {
            Editor = new TextEditor();

            Editor.TextChanged += (object? sender, EventArgs e) => TextHasChanged = true;

            this.Child = Editor;
            Dock = DockStyle.Fill;

            var highlighterText = codeType switch
            {
                SiCodeType.CSharp => EmbeddedResource.Load("Highlighters/SiCSharpHighlighter.xshd"),
                SiCodeType.JSON => EmbeddedResource.Load("Highlighters/SiJsonHighlighter.xshd"),
                SiCodeType.MarkDown => EmbeddedResource.Load("Highlighters/SiMarkDownHighlighter.xshd"),
                SiCodeType.XML => EmbeddedResource.Load("Highlighters/SiXmlHighlighter.xshd"),
                _ => null
            };

            if (highlighterText != null)
            {
                using var stringReader = new StringReader(highlighterText);
                using var reader = XmlReader.Create(stringReader);
                Editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                reader.Close();
                stringReader.Close();
            }

            parent.Controls.Add(this);

            TextHasChanged = false;
        }

        /// <summary>
        /// Rereads and applies the editor settings.
        /// </summary>
        /// <param name="editor"></param>
        public void ApplyEditorSettings()
        {
            Editor.ShowLineNumbers = Settings.Instance.EditorShowLineNumbers;
            Editor.FontFamily = new System.Windows.Media.FontFamily(Settings.Instance.EditorFontFamily);
            Editor.FontSize = Settings.Instance.EditorFontSize;
            Editor.WordWrap = Settings.Instance.EditorWordWrap;
        }
    }
}
