using Krypton.Toolkit;
using NTDLS.Helpers;
using Si.Engine;
using Si.Engine.Sprite.Enemy.Peon;

namespace Si.AssetExplorer
{
    public partial class FormMain : KryptonForm
    {
        private readonly EngineCore _engine;
        private bool _firstShown = true;
        private readonly TreeManager _treeManager;
        private readonly RichTextBox _richTextBox;

        public FormMain()
        {
            InitializeComponent();

            // KryptonRichTextBox is a composite control; the real editor is inside.
            _richTextBox = richTextBoxOutput.Controls.OfType<RichTextBox>().FirstOrDefault()
                ?? throw new InvalidOperationException("Could not find inner RichTextBox inside KryptonRichTextBox.");

            WriteOutput("Instanciating EngineCore.", LoggingLevel.Verbose);

            pictureBoxPreview.Parent.EnsureNotNull().Resize += Parent_Resize;
            Parent_Resize(null, new());

            _engine = new EngineCore(pictureBoxPreview, Library.SiConstants.SiEngineExecutionMode.Edit);
            _engine.OnInitializationComplete += EngineCore_OnInitializationComplete;
            _treeManager = new TreeManager(treeViewAssets, _engine, WriteOutput);

            Shown += FormMain_Shown;
        }

        private void Parent_Resize(object? sender, EventArgs e)
        {
            pictureBoxPreview.Parent.EnsureNotNull();
            int margin = 6;

            var boxSize = Math.Min(pictureBoxPreview.Parent.Width, pictureBoxPreview.Parent.Height) - margin;

            if (boxSize > 10)
            {
                pictureBoxPreview.Width = boxSize;
                pictureBoxPreview.Height = boxSize;

                pictureBoxPreview.Left = (pictureBoxPreview.Parent.Width / 2) - (pictureBoxPreview.Width / 2);
                pictureBoxPreview.Top = (pictureBoxPreview.Parent.Height / 2) - (pictureBoxPreview.Height / 2);
            }
        }

        private void EngineCore_OnInitializationComplete(EngineCore engine)
        {
            WriteOutput("Engine initialization complete.", LoggingLevel.Verbose);

            _treeManager.Repopulate();

            _engine.Events.Once(() =>
            {
                var sprite = _engine.Sprites.Enemies.AddTypeOf<SpriteEnemyPhoenix>();
                sprite.Location = _engine.Display.CenterCanvas;
                sprite.Speed = 0;
                sprite.Throttle = 0;
            });
        }

        private void FormMain_Shown(object? sender, EventArgs e)
        {
            if (_firstShown)
            {
                _firstShown = false;

                WriteOutput("Starting engine.", LoggingLevel.Verbose);
                _engine.StartEngine();
            }
        }

        private void WriteOutput(string text, LoggingLevel? loggingLevel)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, LoggingLevel?>(WriteOutput), text, loggingLevel);
                return;
            }

            var color = loggingLevel switch
            {
                LoggingLevel.Verbose => AssetExplorerColors.Verbose,
                LoggingLevel.Information => AssetExplorerColors.Information,
                LoggingLevel.Warning => AssetExplorerColors.Warning,
                LoggingLevel.Error => AssetExplorerColors.Error,
                _ => AssetExplorerColors.Default
            };

            _richTextBox.SelectionStart = _richTextBox.TextLength;
            _richTextBox.SelectionLength = 0;
            _richTextBox.SelectionColor = color;
            _richTextBox.AppendText(text + Environment.NewLine);
            _richTextBox.SelectionColor = _richTextBox.ForeColor;

            // Reset selection to end.
            _richTextBox.Select(_richTextBox.TextLength, 0);
            _richTextBox.SelectionColor = _richTextBox.ForeColor;

            _richTextBox.ScrollToCaret();
        }

        #region Tooklstrip buttons

        private void ToolStripButtonSettings_Click(object sender, EventArgs e)
        {
            using var formSettings = new FormSettings();
            formSettings.ShowDialog();
        }

        #endregion
    }
}
