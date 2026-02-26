using Krypton.Toolkit;
using NTDLS.Helpers;
using Si.AssetExplorer.Controls;
using Si.AssetExplorer.Forms;
using Si.Engine;
using Si.Engine.Sprite;
using Si.Engine.Sprite._Superclass;
using Si.Engine.Sprite._Superclass._Root;
using Si.Library;
using Si.Library.ExtensionMethods;
using static Si.Library.SiConstants;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

            pictureBoxPreview.MouseWheel += PictureBoxPreview_MouseWheel;

            _engine = new EngineCore(pictureBoxPreview, Library.SiConstants.SiEngineExecutionMode.Edit, new Size(1000, 1000));
            _engine.Display.ZoomOverride = 0.1f; // Start zoomed out to show the whole sprite.
            _engine.OnInitializationComplete += EngineCore_OnInitializationComplete;
            _treeManager = new TreeManager(treeViewAssets, _engine, WriteOutput, LoadSelectedTreeNode);
            _engine.EnableDevelopment(new FormInterrogation(_engine));

            Shown += FormMain_Shown;
        }

        private void PictureBoxPreview_MouseWheel(object? sender, MouseEventArgs e)
        {
            float zoom = (_engine.Display.ZoomOverride ?? 0);

            zoom += e.Delta > 0 ? -0.01f : 0.01f;
            zoom = Math.Clamp(zoom, 0.001f, 1);

            _engine.Display.ZoomOverride = zoom.IsNearZero() ? null : zoom;
        }

        private void Parent_Resize(object? sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void EngineCore_OnInitializationComplete(EngineCore engine)
        {
            try
            {
                WriteOutput("Engine initialization complete.", LoggingLevel.Verbose);

                _engine.Sprites.QueueAllForDeletion();
                _engine.Sprites.HardDeleteAllQueuedDeletions();

                _treeManager.Repopulate();

                //_engine.Events.Once(() =>
                //{
                //    _engine.Sprites.QueueAllForDeletion();
                //    _engine.Sprites.HardDeleteAllQueuedDeletions();

                //    var sprite = _engine.Sprites.Add<SpriteBase>(@"Sprites\Enemy\Debug\Hull.png", (o) =>
                //    {
                //        o.Location = _engine.Display.CenterCanvas;
                //        o.Speed = 0;
                //        o.Throttle = 0;
                //    });
                //});
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void FormMain_Shown(object? sender, EventArgs e)
        {
            try
            {
                if (_firstShown)
                {
                    _firstShown = false;

                    WriteOutput("Starting engine.", LoggingLevel.Verbose);
                    _engine.StartEngine();
                }
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void LoadSelectedTreeNode(SiTreeNode node)
        {
            try
            {
                _engine.Events.Once(() =>
                {
                    _engine.Sprites.QueueAllForDeletion();
                    _engine.Sprites.HardDeleteAllQueuedDeletions();

                    var sprite = _engine.Sprites.Add(node.AssetKey, (o) =>
                    {
                        if (o is SpriteAnimation spriteAnimation)
                        {
                            spriteAnimation.PlayMode = SiAnimationPlayMode.Infinite ;
                        }

                        o.Orientation.Degrees = 0;
                        o.IsVisible = true;
                        o.Location = _engine.Display.CenterCanvas;
                        o.RotationSpeed = 0f;
                        o.Speed = 0;
                        o.Throttle = 0;
                    });

                    PopulateProperties(sprite);
                });
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private class PropertyItem : ListViewItem
        {
            private readonly static Metadata _defaults = new Metadata();

            public PropertyItem(Metadata metaData, string propertyName, ListViewGroup? group, PropertyEditorType editorType)
            {
                Group = group;

                var workingValue = SiReflection.GetPropertyValue(metaData, propertyName)?.ToString() ?? string.Empty;
                var defaultValue = SiReflection.GetPropertyValue(_defaults, propertyName)?.ToString() ?? string.Empty;

                Text = NTDLS.Helpers.Text.SeparateCamelCase(propertyName);
                SubItems.Add(workingValue);
                SubItems.Add(defaultValue);
            }
        }

        private void PopulateProperties(SpriteBase sprite)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<SpriteBase>(PopulateProperties), sprite);
                return;
            }

            listViewProperties.Items.Clear();
            listViewProperties.View = View.Details;
            listViewProperties.FullRowSelect = true;
            listViewProperties.GridLines = true;
            listViewProperties.ShowGroups = true;

            listViewProperties.Groups.Clear();

            var defaults = new Metadata();

            if (sprite is SpriteInteractiveBase concrete)
            {
                var groupBase = new ListViewGroup("Base", HorizontalAlignment.Left);
                listViewProperties.Groups.Add(groupBase);
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "Class", groupBase, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "Name", groupBase, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "Description", groupBase, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "Type", groupBase, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "X", groupBase, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "Y", groupBase, PropertyEditorType.Readonly));

                var groupAttachment = new ListViewGroup("Attachment", HorizontalAlignment.Left);
                listViewProperties.Groups.Add(groupAttachment);


                var groupDestroy = new ListViewGroup("Destroy", HorizontalAlignment.Left);
                listViewProperties.Groups.Add(groupDestroy);
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "ExplosionType", groupDestroy, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "ParticleBlastOnExplodeAmount", groupDestroy, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "FragmentOnExplode", groupDestroy, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "ScreenShakeOnExplodeAmount", groupDestroy, PropertyEditorType.Readonly));

                //OrientationType
                //PositionType

                var groupHealth = new ListViewGroup("Health", HorizontalAlignment.Left);
                listViewProperties.Groups.Add(groupHealth);
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "Hull", groupHealth, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "Shields", groupHealth, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "Bounty", groupHealth, PropertyEditorType.Readonly));

                var groupMomentum = new ListViewGroup("Momentum", HorizontalAlignment.Left);
                listViewProperties.Groups.Add(groupMomentum);
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "Speed", groupMomentum, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "Throttle", groupMomentum, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "MaxThrottle", groupMomentum, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "Mass", groupMomentum, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "MunitionDetection", groupMomentum, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "CollisionDetection", groupMomentum, PropertyEditorType.Readonly));
                listViewProperties.Items.Add(new PropertyItem(concrete.Metadata, "CollisionPolyAugmentation", groupMomentum, PropertyEditorType.Readonly));

                //PrimaryWeapon
                //Attachments
                //Weapons

                //listViewProperties.Items.Add(new PropertyItem(["Description", concrete.Metadata.Description, defaults.Description], groupAttachment));
            }

            //listViewProperties.Invalidate();
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
            try
            {
                using var formSettings = new FormSettings();
                formSettings.ShowDialog();
            }
            catch (Exception ex)
            {
                WriteOutput($"Error: {ex.GetBaseException().Message}", LoggingLevel.Error);
            }
        }

        private void ToolStripButtonDevelopmentConsole_Click(object sender, EventArgs e)
        {
            _engine.Development?.EnsureVisibility();
        }

        #endregion
    }
}
