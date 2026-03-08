using Ae.Library;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.ComponentModel;
using System.Xml;
using static Ae.Library.AeConstants;

namespace Ae.AssetExplorer.Controls
{
    internal class AeCodeEditor
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

        public AeCodeEditor(Control parent, AeCodeType codeType, string text)
            : this(parent, codeType)
        {
            Text = text;
        }

        public AeCodeEditor(Control parent, AeCodeType codeType)
        {
            Editor = new TextEditor();

            Editor.TextChanged += (object? sender, EventArgs e) => TextHasChanged = true;

            this.Child = Editor;
            Dock = DockStyle.Fill;

            var highlighterText = codeType switch
            {
                AeCodeType.CSharp => EmbeddedResource.Load("Highlighters/AeCSharpHighlighter.xshd"),
                AeCodeType.JSON => EmbeddedResource.Load("Highlighters/AeJsonHighlighter.xshd"),
                AeCodeType.MarkDown => EmbeddedResource.Load("Highlighters/AeMarkDownHighlighter.xshd"),
                AeCodeType.XML => EmbeddedResource.Load("Highlighters/AeXmlHighlighter.xshd"),
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
