using Ae.Engine;
using Ae.Engine.Helpers;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using System.Xml;

namespace Ae.AssetExplorer.Controls
{
    internal class AeCodeEditor
        : System.Windows.Forms.Integration.ElementHost
    {
        public TextEditor Editor { get; private set; }
        public bool TextHasChanged { get; private set; } = false;

        private readonly FormMain? _formMain;

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

        public AeCodeEditor(Control parent, FormMain? formMain, AeCodeType codeType, string text)
            : this(parent, formMain, codeType)
        {
            Text = text;

            ApplyEditorSettings();
            TextHasChanged = false;
        }

        public AeCodeEditor(Control parent, FormMain? formMain, AeCodeType codeType)
        {
            _formMain = formMain;

            Editor = new TextEditor();

            Editor.TextChanged += (object? sender, EventArgs e) => TextHasChanged = true;

            this.Child = Editor;
            Dock = DockStyle.Fill;

            var highlighterText = codeType switch
            {
                AeCodeType.CSharp => AeEmbeddedResourceReader.LoadText("Highlighters/AeCSharpHighlighter.xshd"),
                AeCodeType.JSON => AeEmbeddedResourceReader.LoadText("Highlighters/AeJsonHighlighter.xshd"),
                AeCodeType.MarkDown => AeEmbeddedResourceReader.LoadText("Highlighters/AeMarkDownHighlighter.xshd"),
                AeCodeType.XML => AeEmbeddedResourceReader.LoadText("Highlighters/AeXmlHighlighter.xshd"),
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

            Editor.KeyUp += Editor_KeyUp;

            ApplyEditorSettings();
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

        public void SetUnmodified()
        {
            TextHasChanged = false;
        }

        private void Editor_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control && e.Key == Key.F)
            {
                _formMain?.ShowFind(Editor.SelectedText);
            }
            else if ((Control.ModifierKeys & Keys.Control) == Keys.Control && e.Key == Key.H)
            {
                _formMain?.ShowReplace(Editor.SelectedText);
            }
            else if (e.Key == Key.F3)
            {
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    _formMain?.ShowFind(Editor.SelectedText);
                }
                else
                {
                    _formMain?.FindNext();
                }
            }
        }

        public void IncreaseCurrentTabIndent()
        {
            var lines = Editor.SelectedText.Split("\r\n");
            if (lines?.Length > 0)
            {
                var newText = new StringBuilder();
                foreach (var line in lines)
                {
                    newText.AppendLine($"\t{line}");
                }
                Editor.SelectedText = newText.ToString();
            }
        }

        public void DecreaseCurrentTabIndent()
        {
            var lines = Editor.SelectedText.Split("\r\n");
            if (lines?.Length > 0)
            {
                var newText = new StringBuilder();
                foreach (var line in lines)
                {
                    if (line.TrimStart(' ').StartsWith('\t'))
                    {
                        var index = line.IndexOf('\t');
                        newText.AppendLine(line.Remove(index, 1));
                    }
                    else if (line.StartsWith("    "))
                    {
                        var index = line.IndexOf("    ");
                        newText.AppendLine(line.Remove(index, 4));
                    }
                    else
                    {
                        newText.AppendLine($"{line}");
                    }
                }
                Editor.SelectedText = newText.ToString();
            }
        }

        public void CommentSelection()
        {
            var lines = Editor.SelectedText.Split("\r\n");
            if (lines?.Length > 0)
            {
                var newText = new StringBuilder();
                foreach (var line in lines)
                {
                    newText.AppendLine($"//{line}");
                }
                Editor.SelectedText = newText.ToString();
            }
        }

        public void UncommentSelection()
        {
            var lines = Editor.SelectedText.Split("\r\n");
            if (lines?.Length > 0)
            {
                var newText = new StringBuilder();
                foreach (var line in lines)
                {
                    if (line.TrimStart().StartsWith("//"))
                    {
                        var index = line.IndexOf("//");
                        newText.AppendLine(line.Remove(index, 2));
                    }
                    else
                    {
                        newText.AppendLine($"{line}");
                    }
                }
                Editor.SelectedText = newText.ToString();
            }
        }

        #region Find.

        private string _lastSearchText = string.Empty;

        public bool FindNext(string searchText, bool caseSensitive)
        {
            var startIndex = Editor.SelectionLength > 0
                    ? Editor.SelectionStart + Editor.SelectionLength
                    : Editor.CaretOffset;

            if (searchText != _lastSearchText)
            {
                startIndex = 0;
            }
            _lastSearchText = searchText;

            startIndex = Editor.Document.IndexOf(searchText, startIndex,
                (Editor.Document.TextLength - startIndex) - 1,
                caseSensitive ? StringComparison.InvariantCulture : StringComparison.CurrentCultureIgnoreCase);

            if (startIndex >= 0)
            {
                Editor.Select(startIndex, searchText.Length);
                Editor.TextArea.Caret.BringCaretToView();
                return true;
            }

            return false;
        }

        public void FindReplace(string searchText, string replaceWith, bool caseSensitive)
        {
            if (Editor.SelectionLength > 0)
            {
                Editor.SelectedText = replaceWith;
            }
            FindNext(searchText, caseSensitive);
        }

        public void FindReplaceAll(string searchText, string replaceWith, bool caseSensitive)
        {
            Editor.Document.Text = Editor.Document.Text.Replace(
                searchText, replaceWith,
                caseSensitive ? StringComparison.InvariantCulture : StringComparison.CurrentCultureIgnoreCase);
        }

        #endregion
    }
}
