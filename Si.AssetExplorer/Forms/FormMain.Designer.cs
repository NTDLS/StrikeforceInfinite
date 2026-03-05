using Talkster.Client.Controls;

namespace Si.AssetExplorer
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
            pictureBoxPreview = new PictureBox();
            listViewProperties = new ListView();
            splitContainerBottom = new SplitContainer();
            richTextBoxOutput = new RichTextBox();
            toolStrip1 = new ToolStrip();
            toolStripButtonSettings = new ToolStripButton();
            toolStripButtonDeveloperConsole = new ToolStripButton();
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
            ((System.ComponentModel.ISupportInitialize)pictureBoxPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerBottom).BeginInit();
            splitContainerBottom.Panel1.SuspendLayout();
            splitContainerBottom.Panel2.SuspendLayout();
            splitContainerBottom.SuspendLayout();
            toolStrip1.SuspendLayout();
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
            splitContainerLeft.Size = new Size(800, 429);
            splitContainerLeft.SplitterDistance = 273;
            splitContainerLeft.TabIndex = 0;
            // 
            // treeViewAssets
            // 
            treeViewAssets.Dock = DockStyle.Fill;
            treeViewAssets.Location = new Point(0, 0);
            treeViewAssets.Name = "treeViewAssets";
            treeViewAssets.Size = new Size(273, 429);
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
            splitContainerRight.Size = new Size(523, 429);
            splitContainerRight.SplitterDistance = 259;
            splitContainerRight.TabIndex = 0;
            // 
            // tabControlCode
            // 
            tabControlCode.Dock = DockStyle.Fill;
            tabControlCode.Location = new Point(0, 0);
            tabControlCode.Name = "tabControlCode";
            tabControlCode.SelectedIndex = 0;
            tabControlCode.Size = new Size(259, 429);
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
            splitContainerProperties.Panel1.Controls.Add(pictureBoxPreview);
            // 
            // splitContainerProperties.Panel2
            // 
            splitContainerProperties.Panel2.Controls.Add(listViewProperties);
            splitContainerProperties.Size = new Size(260, 429);
            splitContainerProperties.SplitterDistance = 168;
            splitContainerProperties.TabIndex = 0;
            // 
            // pictureBoxPreview
            // 
            pictureBoxPreview.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxPreview.Location = new Point(83, 28);
            pictureBoxPreview.Name = "pictureBoxPreview";
            pictureBoxPreview.Size = new Size(100, 100);
            pictureBoxPreview.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBoxPreview.TabIndex = 0;
            pictureBoxPreview.TabStop = false;
            // 
            // listViewProperties
            // 
            listViewProperties.Dock = DockStyle.Fill;
            listViewProperties.Location = new Point(0, 0);
            listViewProperties.Name = "listViewProperties";
            listViewProperties.Size = new Size(260, 257);
            listViewProperties.TabIndex = 0;
            listViewProperties.UseCompatibleStateImageBehavior = false;
            listViewProperties.View = View.Details;
            // 
            // splitContainerBottom
            // 
            splitContainerBottom.Dock = DockStyle.Fill;
            splitContainerBottom.Location = new Point(0, 25);
            splitContainerBottom.Name = "splitContainerBottom";
            splitContainerBottom.Orientation = Orientation.Horizontal;
            // 
            // splitContainerBottom.Panel1
            // 
            splitContainerBottom.Panel1.Controls.Add(splitContainerLeft);
            // 
            // splitContainerBottom.Panel2
            // 
            splitContainerBottom.Panel2.Controls.Add(richTextBoxOutput);
            splitContainerBottom.Size = new Size(800, 600);
            splitContainerBottom.SplitterDistance = 429;
            splitContainerBottom.TabIndex = 1;
            // 
            // richTextBoxOutput
            // 
            richTextBoxOutput.Dock = DockStyle.Fill;
            richTextBoxOutput.Location = new Point(0, 0);
            richTextBoxOutput.Name = "richTextBoxOutput";
            richTextBoxOutput.Size = new Size(800, 167);
            richTextBoxOutput.TabIndex = 0;
            richTextBoxOutput.Text = "";
            // 
            // toolStrip1
            // 
            toolStrip1.Font = new Font("Segoe UI", 9F);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButtonSettings, toolStripButtonDeveloperConsole });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 3;
            toolStrip1.Text = "toolStrip1";
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
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 625);
            Controls.Add(splitContainerBottom);
            Controls.Add(toolStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormMain";
            Text = "Asset Manager";
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
            ((System.ComponentModel.ISupportInitialize)pictureBoxPreview).EndInit();
            splitContainerBottom.Panel1.ResumeLayout(false);
            splitContainerBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerBottom).EndInit();
            splitContainerBottom.ResumeLayout(false);
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer splitContainerLeft;
        private DoubleBufferedTreeView treeViewAssets;
        private SplitContainer splitContainerRight;
        private SplitContainer splitContainerBottom;
        private SplitContainer splitContainerProperties;
        private PictureBox pictureBoxPreview;
        private ListView listViewProperties;
        private RichTextBox richTextBoxOutput;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonSettings;
        private ToolStripButton toolStripButtonDeveloperConsole;
        private TabControl tabControlCode;
    }
}
