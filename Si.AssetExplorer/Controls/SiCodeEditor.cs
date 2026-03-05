using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Si.Library;
using System.ComponentModel;
using System.Xml;

namespace Si.AssetExplorer.Controls
{
    internal class SiCodeEditor
        : System.Windows.Forms.Integration.ElementHost
    {
        public TextEditor Editor { get; private set; }

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

        public SiCodeEditor(Control parent, string text)
            : this(parent)
        {
            Text = text;
        }

        public SiCodeEditor(Control parent)
        {
            var highlighterText = EmbeddedResource.Load("Highlighters/SiCSharpHighlighter.xshd");

            Editor = new TextEditor();

            this.Child = Editor;
            Dock = DockStyle.Fill;

            using var stringReader = new StringReader(highlighterText);
            using var reader = XmlReader.Create(stringReader);
            Editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
            reader.Close();
            stringReader.Close();

            parent.Controls.Add(this);
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
