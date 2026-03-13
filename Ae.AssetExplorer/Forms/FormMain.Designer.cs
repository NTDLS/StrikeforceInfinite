using Talkster.Client.Controls;

namespace Ae.AssetExplorer
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            splitContainerLeft = new SplitContainer();
            treeViewAssets = new DoubleBufferedTreeView();
            splitContainerRight = new SplitContainer();
            tabControlCode = new TabControl();
            splitContainerProperties = new SplitContainer();
            drawingSurface = new PictureBox();
            listViewProperties = new ListView();
            splitContainerBottom = new SplitContainer();
            tabControlOutput = new TabControl();
            tabPageOutput = new TabPage();
            listViewOutput = new Ae.AssetExplorer.Controls.BufferedListView();
            columnHeaderSeverity = new ColumnHeader();
            columnHeaderAsset = new ColumnHeader();
            columnHeaderText = new ColumnHeader();
            tabPageCode = new TabPage();
            toolStrip = new ToolStrip();
            toolStripButtonSettings = new ToolStripButton();
            toolStripButtonDeveloperConsole = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripButtonSave = new ToolStripButton();
            toolStripButtonSaveAll = new ToolStripButton();
            toolStripButtonClose = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripButtonUndo = new ToolStripButton();
            toolStripButtonRedo = new ToolStripButton();
            toolStripSeparator5 = new ToolStripSeparator();
            toolStripButtonBuild = new ToolStripButton();
            toolStripButtonRun = new ToolStripButton();
            toolStripButtonDebug = new ToolStripButton();
            toolStripButtonBreak = new ToolStripButton();
            toolStripSeparator6 = new ToolStripSeparator();
            toolStripButtonComment = new ToolStripButton();
            toolStripButtonUncomment = new ToolStripButton();
            toolStripSeparator7 = new ToolStripSeparator();
            toolStripButtonCopy = new ToolStripButton();
            toolStripButtonCut = new ToolStripButton();
            toolStripButtonPaste = new ToolStripButton();
            toolStripSeparator8 = new ToolStripSeparator();
            toolStripButtonDecreaseIndent = new ToolStripButton();
            toolStripButtonIncreaseIndent = new ToolStripButton();
            toolStripSeparator9 = new ToolStripSeparator();
            toolStripButtonFind = new ToolStripButton();
            toolStripButtonReplace = new ToolStripButton();
            toolStripButtonGoToLine = new ToolStripButton();
            toolStripSeparator10 = new ToolStripSeparator();
            toolStripButtonToggleAssets = new ToolStripButton();
            toolStripButtonToggleOutput = new ToolStripButton();
            toolStripButtonToggleProperties = new ToolStripButton();
            toolStripSeparator4 = new ToolStripSeparator();
            toolStripButtonAbout = new ToolStripButton();
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            extractProjectToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainerLeft).BeginInit();
            splitContainerLeft.Panel1.SuspendLayout();
            splitContainerLeft.Panel2.SuspendLayout();
            splitContainerLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).BeginInit();
            splitContainerRight.Panel1.SuspendLayout();
            splitContainerRight.Panel2.SuspendLayout();
            splitContainerRight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerProperties).BeginInit();
            splitContainerProperties.Panel1.SuspendLayout();
            splitContainerProperties.Panel2.SuspendLayout();
            splitContainerProperties.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)drawingSurface).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerBottom).BeginInit();
            splitContainerBottom.Panel1.SuspendLayout();
            splitContainerBottom.Panel2.SuspendLayout();
            splitContainerBottom.SuspendLayout();
            tabControlOutput.SuspendLayout();
            tabPageOutput.SuspendLayout();
            toolStrip.SuspendLayout();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerLeft
            // 
            splitContainerLeft.Dock = DockStyle.Fill;
            splitContainerLeft.FixedPanel = FixedPanel.Panel1;
            splitContainerLeft.Location = new Point(0, 0);
            splitContainerLeft.Name = "splitContainerLeft";
            // 
            // splitContainerLeft.Panel1
            // 
            splitContainerLeft.Panel1.Controls.Add(treeViewAssets);
            // 
            // splitContainerLeft.Panel2
            // 
            splitContainerLeft.Panel2.Controls.Add(splitContainerRight);
            splitContainerLeft.Size = new Size(800, 411);
            splitContainerLeft.SplitterDistance = 273;
            splitContainerLeft.TabIndex = 0;
            // 
            // treeViewAssets
            // 
            treeViewAssets.Dock = DockStyle.Fill;
            treeViewAssets.Location = new Point(0, 0);
            treeViewAssets.Name = "treeViewAssets";
            treeViewAssets.Size = new Size(273, 411);
            treeViewAssets.TabIndex = 0;
            // 
            // splitContainerRight
            // 
            splitContainerRight.Dock = DockStyle.Fill;
            splitContainerRight.FixedPanel = FixedPanel.Panel2;
            splitContainerRight.Location = new Point(0, 0);
            splitContainerRight.Name = "splitContainerRight";
            // 
            // splitContainerRight.Panel1
            // 
            splitContainerRight.Panel1.Controls.Add(tabControlCode);
            // 
            // splitContainerRight.Panel2
            // 
            splitContainerRight.Panel2.Controls.Add(splitContainerProperties);
            splitContainerRight.Size = new Size(523, 411);
            splitContainerRight.SplitterDistance = 259;
            splitContainerRight.TabIndex = 0;
            // 
            // tabControlCode
            // 
            tabControlCode.Dock = DockStyle.Fill;
            tabControlCode.Location = new Point(0, 0);
            tabControlCode.Name = "tabControlCode";
            tabControlCode.SelectedIndex = 0;
            tabControlCode.Size = new Size(259, 411);
            tabControlCode.TabIndex = 0;
            // 
            // splitContainerProperties
            // 
            splitContainerProperties.Dock = DockStyle.Fill;
            splitContainerProperties.Location = new Point(0, 0);
            splitContainerProperties.Name = "splitContainerProperties";
            splitContainerProperties.Orientation = Orientation.Horizontal;
            // 
            // splitContainerProperties.Panel1
            // 
            splitContainerProperties.Panel1.Controls.Add(drawingSurface);
            // 
            // splitContainerProperties.Panel2
            // 
            splitContainerProperties.Panel2.Controls.Add(listViewProperties);
            splitContainerProperties.Size = new Size(260, 411);
            splitContainerProperties.SplitterDistance = 160;
            splitContainerProperties.TabIndex = 0;
            // 
            // drawingSurface
            // 
            drawingSurface.BorderStyle = BorderStyle.FixedSingle;
            drawingSurface.Location = new Point(83, 28);
            drawingSurface.Name = "drawingSurface";
            drawingSurface.Size = new Size(100, 100);
            drawingSurface.SizeMode = PictureBoxSizeMode.AutoSize;
            drawingSurface.TabIndex = 0;
            drawingSurface.TabStop = false;
            // 
            // listViewProperties
            // 
            listViewProperties.Dock = DockStyle.Fill;
            listViewProperties.Location = new Point(0, 0);
            listViewProperties.Name = "listViewProperties";
            listViewProperties.Size = new Size(260, 247);
            listViewProperties.TabIndex = 0;
            listViewProperties.UseCompatibleStateImageBehavior = false;
            listViewProperties.View = View.Details;
            // 
            // splitContainerBottom
            // 
            splitContainerBottom.Dock = DockStyle.Fill;
            splitContainerBottom.Location = new Point(0, 49);
            splitContainerBottom.Name = "splitContainerBottom";
            splitContainerBottom.Orientation = Orientation.Horizontal;
            // 
            // splitContainerBottom.Panel1
            // 
            splitContainerBottom.Panel1.Controls.Add(splitContainerLeft);
            // 
            // splitContainerBottom.Panel2
            // 
            splitContainerBottom.Panel2.Controls.Add(tabControlOutput);
            splitContainerBottom.Size = new Size(800, 576);
            splitContainerBottom.SplitterDistance = 411;
            splitContainerBottom.TabIndex = 1;
            // 
            // tabControlOutput
            // 
            tabControlOutput.Controls.Add(tabPageOutput);
            tabControlOutput.Controls.Add(tabPageCode);
            tabControlOutput.Dock = DockStyle.Fill;
            tabControlOutput.Location = new Point(0, 0);
            tabControlOutput.Name = "tabControlOutput";
            tabControlOutput.SelectedIndex = 0;
            tabControlOutput.Size = new Size(800, 161);
            tabControlOutput.TabIndex = 1;
            // 
            // tabPageOutput
            // 
            tabPageOutput.Controls.Add(listViewOutput);
            tabPageOutput.Location = new Point(4, 24);
            tabPageOutput.Name = "tabPageOutput";
            tabPageOutput.Padding = new Padding(3);
            tabPageOutput.Size = new Size(792, 133);
            tabPageOutput.TabIndex = 0;
            tabPageOutput.Text = "Output";
            tabPageOutput.UseVisualStyleBackColor = true;
            // 
            // listViewOutput
            // 
            listViewOutput.Columns.AddRange(new ColumnHeader[] { columnHeaderSeverity, columnHeaderAsset, columnHeaderText });
            listViewOutput.Dock = DockStyle.Fill;
            listViewOutput.FullRowSelect = true;
            listViewOutput.Location = new Point(3, 3);
            listViewOutput.MultiSelect = false;
            listViewOutput.Name = "listViewOutput";
            listViewOutput.ShowGroups = false;
            listViewOutput.Size = new Size(786, 127);
            listViewOutput.TabIndex = 0;
            listViewOutput.UseCompatibleStateImageBehavior = false;
            listViewOutput.View = View.Details;
            // 
            // columnHeaderSeverity
            // 
            columnHeaderSeverity.Text = "Severity";
            columnHeaderSeverity.Width = 100;
            // 
            // columnHeaderAsset
            // 
            columnHeaderAsset.Text = "Asset";
            columnHeaderAsset.Width = 100;
            // 
            // columnHeaderText
            // 
            columnHeaderText.Text = "Text";
            columnHeaderText.Width = 500;
            // 
            // tabPageCode
            // 
            tabPageCode.Location = new Point(4, 24);
            tabPageCode.Name = "tabPageCode";
            tabPageCode.Padding = new Padding(3);
            tabPageCode.Size = new Size(792, 133);
            tabPageCode.TabIndex = 1;
            tabPageCode.Text = "Code";
            tabPageCode.UseVisualStyleBackColor = true;
            // 
            // toolStrip
            // 
            toolStrip.Font = new Font("Segoe UI", 9F);
            toolStrip.Items.AddRange(new ToolStripItem[] { toolStripButtonSettings, toolStripButtonDeveloperConsole, toolStripSeparator1, toolStripButtonSave, toolStripButtonSaveAll, toolStripButtonClose, toolStripSeparator2, toolStripButtonUndo, toolStripButtonRedo, toolStripSeparator5, toolStripButtonBuild, toolStripButtonRun, toolStripButtonDebug, toolStripButtonBreak, toolStripSeparator6, toolStripButtonComment, toolStripButtonUncomment, toolStripSeparator7, toolStripButtonCopy, toolStripButtonCut, toolStripButtonPaste, toolStripSeparator8, toolStripButtonDecreaseIndent, toolStripButtonIncreaseIndent, toolStripSeparator9, toolStripButtonFind, toolStripButtonReplace, toolStripButtonGoToLine, toolStripSeparator10, toolStripButtonToggleAssets, toolStripButtonToggleOutput, toolStripButtonToggleProperties, toolStripSeparator4, toolStripButtonAbout });
            toolStrip.Location = new Point(0, 24);
            toolStrip.Name = "toolStrip";
            toolStrip.Size = new Size(800, 25);
            toolStrip.TabIndex = 3;
            toolStrip.Text = "toolStrip1";
            // 
            // toolStripButtonSettings
            // 
            toolStripButtonSettings.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonSettings.Image = (Image)resources.GetObject("toolStripButtonSettings.Image");
            toolStripButtonSettings.ImageTransparentColor = Color.Magenta;
            toolStripButtonSettings.Name = "toolStripButtonSettings";
            toolStripButtonSettings.Size = new Size(23, 22);
            toolStripButtonSettings.Text = "Settings";
            toolStripButtonSettings.ToolTipText = "Settings";
            toolStripButtonSettings.Click += ToolStripButtonSettings_Click;
            // 
            // toolStripButtonDeveloperConsole
            // 
            toolStripButtonDeveloperConsole.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonDeveloperConsole.Image = (Image)resources.GetObject("toolStripButtonDeveloperConsole.Image");
            toolStripButtonDeveloperConsole.ImageTransparentColor = Color.Magenta;
            toolStripButtonDeveloperConsole.Name = "toolStripButtonDeveloperConsole";
            toolStripButtonDeveloperConsole.Size = new Size(23, 22);
            toolStripButtonDeveloperConsole.Text = "Developer Console";
            toolStripButtonDeveloperConsole.ToolTipText = "Developer Console";
            toolStripButtonDeveloperConsole.Click += ToolStripButtonDeveloperConsole_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // toolStripButtonSave
            // 
            toolStripButtonSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonSave.Image = (Image)resources.GetObject("toolStripButtonSave.Image");
            toolStripButtonSave.ImageTransparentColor = Color.Magenta;
            toolStripButtonSave.Name = "toolStripButtonSave";
            toolStripButtonSave.Size = new Size(23, 22);
            toolStripButtonSave.Text = "Save";
            toolStripButtonSave.Click += ToolStripButtonSave_Click;
            // 
            // toolStripButtonSaveAll
            // 
            toolStripButtonSaveAll.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonSaveAll.Image = (Image)resources.GetObject("toolStripButtonSaveAll.Image");
            toolStripButtonSaveAll.ImageTransparentColor = Color.Magenta;
            toolStripButtonSaveAll.Name = "toolStripButtonSaveAll";
            toolStripButtonSaveAll.Size = new Size(23, 22);
            toolStripButtonSaveAll.Text = "Save All";
            toolStripButtonSaveAll.Click += ToolStripButtonSaveAll_Click;
            // 
            // toolStripButtonClose
            // 
            toolStripButtonClose.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonClose.Image = (Image)resources.GetObject("toolStripButtonClose.Image");
            toolStripButtonClose.ImageTransparentColor = Color.Magenta;
            toolStripButtonClose.Name = "toolStripButtonClose";
            toolStripButtonClose.Size = new Size(23, 22);
            toolStripButtonClose.Text = "Close";
            toolStripButtonClose.Click += ToolStripButtonClose_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // toolStripButtonUndo
            // 
            toolStripButtonUndo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonUndo.Image = (Image)resources.GetObject("toolStripButtonUndo.Image");
            toolStripButtonUndo.ImageTransparentColor = Color.Magenta;
            toolStripButtonUndo.Name = "toolStripButtonUndo";
            toolStripButtonUndo.Size = new Size(23, 22);
            toolStripButtonUndo.Text = "Undo";
            // 
            // toolStripButtonRedo
            // 
            toolStripButtonRedo.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonRedo.Image = (Image)resources.GetObject("toolStripButtonRedo.Image");
            toolStripButtonRedo.ImageTransparentColor = Color.Magenta;
            toolStripButtonRedo.Name = "toolStripButtonRedo";
            toolStripButtonRedo.Size = new Size(23, 22);
            toolStripButtonRedo.Text = "Redo";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(6, 25);
            // 
            // toolStripButtonBuild
            // 
            toolStripButtonBuild.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonBuild.Image = (Image)resources.GetObject("toolStripButtonBuild.Image");
            toolStripButtonBuild.ImageTransparentColor = Color.Magenta;
            toolStripButtonBuild.Name = "toolStripButtonBuild";
            toolStripButtonBuild.Size = new Size(23, 22);
            toolStripButtonBuild.Text = "Build";
            toolStripButtonBuild.ToolTipText = "Build";
            toolStripButtonBuild.Click += ToolStripButtonBuild_Click;
            // 
            // toolStripButtonRun
            // 
            toolStripButtonRun.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonRun.Image = (Image)resources.GetObject("toolStripButtonRun.Image");
            toolStripButtonRun.ImageTransparentColor = Color.Magenta;
            toolStripButtonRun.Name = "toolStripButtonRun";
            toolStripButtonRun.Size = new Size(23, 22);
            toolStripButtonRun.Text = "Run";
            toolStripButtonRun.Click += ToolStripButtonRun_Click;
            // 
            // toolStripButtonDebug
            // 
            toolStripButtonDebug.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonDebug.Image = (Image)resources.GetObject("toolStripButtonDebug.Image");
            toolStripButtonDebug.ImageTransparentColor = Color.Magenta;
            toolStripButtonDebug.Name = "toolStripButtonDebug";
            toolStripButtonDebug.Size = new Size(23, 22);
            toolStripButtonDebug.Text = "Debug";
            toolStripButtonDebug.Click += ToolStripButtonDebug_Click;
            // 
            // toolStripButtonBreak
            // 
            toolStripButtonBreak.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonBreak.Image = (Image)resources.GetObject("toolStripButtonBreak.Image");
            toolStripButtonBreak.ImageTransparentColor = Color.Magenta;
            toolStripButtonBreak.Name = "toolStripButtonBreak";
            toolStripButtonBreak.Size = new Size(23, 22);
            toolStripButtonBreak.Text = "Break";
            toolStripButtonBreak.Click += ToolStripButtonBreak_Click;
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 25);
            // 
            // toolStripButtonComment
            // 
            toolStripButtonComment.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonComment.Image = (Image)resources.GetObject("toolStripButtonComment.Image");
            toolStripButtonComment.ImageTransparentColor = Color.Magenta;
            toolStripButtonComment.Name = "toolStripButtonComment";
            toolStripButtonComment.Size = new Size(23, 22);
            toolStripButtonComment.Text = "Comment";
            toolStripButtonComment.Click += ToolStripButtonComment_Click;
            // 
            // toolStripButtonUncomment
            // 
            toolStripButtonUncomment.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonUncomment.Image = (Image)resources.GetObject("toolStripButtonUncomment.Image");
            toolStripButtonUncomment.ImageTransparentColor = Color.Magenta;
            toolStripButtonUncomment.Name = "toolStripButtonUncomment";
            toolStripButtonUncomment.Size = new Size(23, 22);
            toolStripButtonUncomment.Text = "Uncomment";
            toolStripButtonUncomment.Click += ToolStripButtonUncomment_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(6, 25);
            // 
            // toolStripButtonCopy
            // 
            toolStripButtonCopy.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonCopy.Image = (Image)resources.GetObject("toolStripButtonCopy.Image");
            toolStripButtonCopy.ImageTransparentColor = Color.Magenta;
            toolStripButtonCopy.Name = "toolStripButtonCopy";
            toolStripButtonCopy.Size = new Size(23, 22);
            toolStripButtonCopy.Text = "Copy";
            toolStripButtonCopy.Click += ToolStripButtonCopy_Click;
            // 
            // toolStripButtonCut
            // 
            toolStripButtonCut.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonCut.Image = (Image)resources.GetObject("toolStripButtonCut.Image");
            toolStripButtonCut.ImageTransparentColor = Color.Magenta;
            toolStripButtonCut.Name = "toolStripButtonCut";
            toolStripButtonCut.Size = new Size(23, 22);
            toolStripButtonCut.Text = "Cut";
            toolStripButtonCut.Click += ToolStripButtonCut_Click;
            // 
            // toolStripButtonPaste
            // 
            toolStripButtonPaste.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonPaste.Image = (Image)resources.GetObject("toolStripButtonPaste.Image");
            toolStripButtonPaste.ImageTransparentColor = Color.Magenta;
            toolStripButtonPaste.Name = "toolStripButtonPaste";
            toolStripButtonPaste.Size = new Size(23, 22);
            toolStripButtonPaste.Text = "Paste";
            toolStripButtonPaste.Click += ToolStripButtonPaste_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(6, 25);
            // 
            // toolStripButtonDecreaseIndent
            // 
            toolStripButtonDecreaseIndent.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonDecreaseIndent.Image = (Image)resources.GetObject("toolStripButtonDecreaseIndent.Image");
            toolStripButtonDecreaseIndent.ImageTransparentColor = Color.Magenta;
            toolStripButtonDecreaseIndent.Name = "toolStripButtonDecreaseIndent";
            toolStripButtonDecreaseIndent.Size = new Size(23, 22);
            toolStripButtonDecreaseIndent.Text = "Decrease Indent";
            toolStripButtonDecreaseIndent.Click += ToolStripButtonDecreaseIndent_Click;
            // 
            // toolStripButtonIncreaseIndent
            // 
            toolStripButtonIncreaseIndent.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonIncreaseIndent.Image = (Image)resources.GetObject("toolStripButtonIncreaseIndent.Image");
            toolStripButtonIncreaseIndent.ImageTransparentColor = Color.Magenta;
            toolStripButtonIncreaseIndent.Name = "toolStripButtonIncreaseIndent";
            toolStripButtonIncreaseIndent.Size = new Size(23, 22);
            toolStripButtonIncreaseIndent.Text = "Increase Indent";
            toolStripButtonIncreaseIndent.Click += ToolStripButtonIncreaseIndent_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(6, 25);
            // 
            // toolStripButtonFind
            // 
            toolStripButtonFind.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonFind.Image = (Image)resources.GetObject("toolStripButtonFind.Image");
            toolStripButtonFind.ImageTransparentColor = Color.Magenta;
            toolStripButtonFind.Name = "toolStripButtonFind";
            toolStripButtonFind.Size = new Size(23, 22);
            toolStripButtonFind.Text = "Find";
            toolStripButtonFind.Click += ToolStripButtonFind_Click;
            // 
            // toolStripButtonReplace
            // 
            toolStripButtonReplace.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonReplace.Image = (Image)resources.GetObject("toolStripButtonReplace.Image");
            toolStripButtonReplace.ImageTransparentColor = Color.Magenta;
            toolStripButtonReplace.Name = "toolStripButtonReplace";
            toolStripButtonReplace.Size = new Size(23, 22);
            toolStripButtonReplace.Text = "Replace";
            toolStripButtonReplace.Click += ToolStripButtonReplace_Click;
            // 
            // toolStripButtonGoToLine
            // 
            toolStripButtonGoToLine.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonGoToLine.Image = (Image)resources.GetObject("toolStripButtonGoToLine.Image");
            toolStripButtonGoToLine.ImageTransparentColor = Color.Magenta;
            toolStripButtonGoToLine.Name = "toolStripButtonGoToLine";
            toolStripButtonGoToLine.Size = new Size(23, 22);
            toolStripButtonGoToLine.Text = "Go to Line";
            toolStripButtonGoToLine.Click += ToolStripButtonGoToLine_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new Size(6, 25);
            // 
            // toolStripButtonToggleAssets
            // 
            toolStripButtonToggleAssets.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonToggleAssets.Image = (Image)resources.GetObject("toolStripButtonToggleAssets.Image");
            toolStripButtonToggleAssets.ImageTransparentColor = Color.Magenta;
            toolStripButtonToggleAssets.Name = "toolStripButtonToggleAssets";
            toolStripButtonToggleAssets.Size = new Size(23, 22);
            toolStripButtonToggleAssets.Text = "Toggle Assets";
            toolStripButtonToggleAssets.Click += ToolStripButtonToggleAssets_Click;
            // 
            // toolStripButtonToggleOutput
            // 
            toolStripButtonToggleOutput.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonToggleOutput.Image = (Image)resources.GetObject("toolStripButtonToggleOutput.Image");
            toolStripButtonToggleOutput.ImageTransparentColor = Color.Magenta;
            toolStripButtonToggleOutput.Name = "toolStripButtonToggleOutput";
            toolStripButtonToggleOutput.Size = new Size(23, 22);
            toolStripButtonToggleOutput.Text = "Toggle Output";
            toolStripButtonToggleOutput.Click += ToolStripButtonToggleOutput_Click;
            // 
            // toolStripButtonToggleProperties
            // 
            toolStripButtonToggleProperties.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonToggleProperties.Image = (Image)resources.GetObject("toolStripButtonToggleProperties.Image");
            toolStripButtonToggleProperties.ImageTransparentColor = Color.Magenta;
            toolStripButtonToggleProperties.Name = "toolStripButtonToggleProperties";
            toolStripButtonToggleProperties.Size = new Size(23, 22);
            toolStripButtonToggleProperties.Text = "Toggle Properties";
            toolStripButtonToggleProperties.Click += ToolStripButtonToggleProperties_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(6, 25);
            // 
            // toolStripButtonAbout
            // 
            toolStripButtonAbout.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonAbout.Image = (Image)resources.GetObject("toolStripButtonAbout.Image");
            toolStripButtonAbout.ImageTransparentColor = Color.Magenta;
            toolStripButtonAbout.Name = "toolStripButtonAbout";
            toolStripButtonAbout.Size = new Size(23, 22);
            toolStripButtonAbout.Text = "About";
            toolStripButtonAbout.Click += ToolStripButtonAbout_Click;
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(800, 24);
            menuStrip.TabIndex = 4;
            menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { extractProjectToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // extractProjectToolStripMenuItem
            // 
            extractProjectToolStripMenuItem.Name = "extractProjectToolStripMenuItem";
            extractProjectToolStripMenuItem.Size = new Size(149, 22);
            extractProjectToolStripMenuItem.Text = "Extract Project";
            extractProjectToolStripMenuItem.Click += ExtractProjectToolStripMenuItem_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 625);
            Controls.Add(splitContainerBottom);
            Controls.Add(toolStrip);
            Controls.Add(menuStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            Name = "FormMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Asset Explorer";
            splitContainerLeft.Panel1.ResumeLayout(false);
            splitContainerLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerLeft).EndInit();
            splitContainerLeft.ResumeLayout(false);
            splitContainerRight.Panel1.ResumeLayout(false);
            splitContainerRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).EndInit();
            splitContainerRight.ResumeLayout(false);
            splitContainerProperties.Panel1.ResumeLayout(false);
            splitContainerProperties.Panel1.PerformLayout();
            splitContainerProperties.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerProperties).EndInit();
            splitContainerProperties.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)drawingSurface).EndInit();
            splitContainerBottom.Panel1.ResumeLayout(false);
            splitContainerBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerBottom).EndInit();
            splitContainerBottom.ResumeLayout(false);
            tabControlOutput.ResumeLayout(false);
            tabPageOutput.ResumeLayout(false);
            toolStrip.ResumeLayout(false);
            toolStrip.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainerLeft;
        private SplitContainer splitContainerRight;
        private SplitContainer splitContainerBottom;
        private SplitContainer splitContainerProperties;
        private PictureBox drawingSurface;
        private ListView listViewProperties;
        private ToolStrip toolStrip;
        private ToolStripButton toolStripButtonSettings;
        private ToolStripButton toolStripButtonDeveloperConsole;
        private TabControl tabControlCode;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonToggleAssets;
        private ToolStripButton toolStripButtonToggleOutput;
        private ToolStripButton toolStripButtonToggleProperties;
        private ToolStripButton toolStripButtonSave;
        private ToolStripButton toolStripButtonSaveAll;
        private ToolStripButton toolStripButtonClose;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonAbout;
        private DoubleBufferedTreeView treeViewAssets;
        private ToolStripButton toolStripButtonBuild;
        private ToolStripSeparator toolStripSeparator4;
        private TabControl tabControlOutput;
        private TabPage tabPageOutput;
        private TabPage tabPageCode;
        private Controls.BufferedListView listViewOutput;
        private ColumnHeader columnHeaderSeverity;
        private ColumnHeader columnHeaderAsset;
        private ColumnHeader columnHeaderText;
        private MenuStrip menuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem extractProjectToolStripMenuItem;
        private ToolStripButton toolStripButtonRun;
        private ToolStripButton toolStripButtonDebug;
        private ToolStripButton toolStripButtonBreak;
        private ToolStripButton toolStripButtonComment;
        private ToolStripButton toolStripButtonUncomment;
        private ToolStripButton toolStripButtonCopy;
        private ToolStripButton toolStripButtonCut;
        private ToolStripButton toolStripButtonPaste;
        private ToolStripButton toolStripButtonDecreaseIndent;
        private ToolStripButton toolStripButtonIncreaseIndent;
        private ToolStripButton toolStripButtonFind;
        private ToolStripButton toolStripButtonReplace;
        private ToolStripButton toolStripButtonGoToLine;
        private ToolStripButton toolStripButtonRedo;
        private ToolStripButton toolStripButtonUndo;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripSeparator toolStripSeparator10;
    }
}
