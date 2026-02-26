using Krypton.Toolkit;
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
            splitContainerLeft = new KryptonSplitContainer();
            treeViewAssets = new DoubleBufferedTreeView();
            splitContainerRight = new KryptonSplitContainer();
            splitContainerProperties = new KryptonSplitContainer();
            pictureBoxPreview = new KryptonPictureBox();
            listViewProperties = new KryptonListView();
            columnHeaderName = new ColumnHeader();
            columnHeaderValue = new ColumnHeader();
            columnHeaderDefault = new ColumnHeader();
            splitContainerBottom = new KryptonSplitContainer();
            richTextBoxOutput = new KryptonRichTextBox();
            kryptonToolStrip1 = new KryptonToolStrip();
            toolStripButtonSettings = new ToolStripButton();
            toolStripButtonDevelopmentConsole = new ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)splitContainerLeft).BeginInit();
            (splitContainerLeft.Panel1).BeginInit();
            splitContainerLeft.Panel1.SuspendLayout();
            (splitContainerLeft.Panel2).BeginInit();
            splitContainerLeft.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).BeginInit();
            (splitContainerRight.Panel1).BeginInit();
            (splitContainerRight.Panel2).BeginInit();
            splitContainerRight.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerProperties).BeginInit();
            (splitContainerProperties.Panel1).BeginInit();
            splitContainerProperties.Panel1.SuspendLayout();
            (splitContainerProperties.Panel2).BeginInit();
            splitContainerProperties.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPreview).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerBottom).BeginInit();
            (splitContainerBottom.Panel1).BeginInit();
            splitContainerBottom.Panel1.SuspendLayout();
            (splitContainerBottom.Panel2).BeginInit();
            splitContainerBottom.Panel2.SuspendLayout();
            kryptonToolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerLeft
            // 
            splitContainerLeft.Dock = DockStyle.Fill;
            splitContainerLeft.Location = new Point(0, 0);
            // 
            // 
            // 
            splitContainerLeft.Panel1.Controls.Add(treeViewAssets);
            // 
            // 
            // 
            splitContainerLeft.Panel2.Controls.Add(splitContainerRight);
            splitContainerLeft.Size = new Size(800, 429);
            splitContainerLeft.SplitterDistance = 224;
            splitContainerLeft.TabIndex = 0;
            // 
            // treeViewAssets
            // 
            treeViewAssets.Dock = DockStyle.Fill;
            treeViewAssets.Location = new Point(0, 0);
            treeViewAssets.Name = "treeViewAssets";
            treeViewAssets.Size = new Size(224, 429);
            treeViewAssets.TabIndex = 0;
            // 
            // splitContainerRight
            // 
            splitContainerRight.Dock = DockStyle.Fill;
            splitContainerRight.Location = new Point(0, 0);
            // 
            // 
            // 
            splitContainerRight.Panel1.PanelBackStyle = PaletteBackStyle.ControlToolTip;
            // 
            // 
            // 
            splitContainerRight.Panel2.Controls.Add(splitContainerProperties);
            splitContainerRight.Size = new Size(571, 429);
            splitContainerRight.SplitterDistance = 401;
            splitContainerRight.TabIndex = 0;
            // 
            // splitContainerProperties
            // 
            splitContainerProperties.Dock = DockStyle.Fill;
            splitContainerProperties.Location = new Point(0, 0);
            splitContainerProperties.Orientation = Orientation.Horizontal;
            // 
            // 
            // 
            splitContainerProperties.Panel1.Controls.Add(pictureBoxPreview);
            // 
            // 
            // 
            splitContainerProperties.Panel2.Controls.Add(listViewProperties);
            splitContainerProperties.Size = new Size(165, 429);
            splitContainerProperties.SplitterDistance = 168;
            splitContainerProperties.TabIndex = 0;
            // 
            // pictureBoxPreview
            // 
            pictureBoxPreview.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxPreview.Location = new Point(36, 34);
            pictureBoxPreview.Name = "pictureBoxPreview";
            pictureBoxPreview.Size = new Size(100, 100);
            pictureBoxPreview.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBoxPreview.TabIndex = 0;
            pictureBoxPreview.TabStop = false;
            // 
            // listViewProperties
            // 
            listViewProperties.Columns.AddRange(new ColumnHeader[] { columnHeaderName, columnHeaderValue, columnHeaderDefault });
            listViewProperties.Dock = DockStyle.Fill;
            listViewProperties.HideSelection = false;
            listViewProperties.Location = new Point(0, 0);
            listViewProperties.Name = "listViewProperties";
            listViewProperties.Size = new Size(165, 256);
            listViewProperties.TabIndex = 0;
            listViewProperties.View = View.Details;
            // 
            // columnHeaderName
            // 
            columnHeaderName.Text = "Name";
            // 
            // columnHeaderValue
            // 
            columnHeaderValue.Text = "Value";
            // 
            // columnHeaderDefault
            // 
            columnHeaderDefault.Text = "Default";
            // 
            // splitContainerBottom
            // 
            splitContainerBottom.Dock = DockStyle.Fill;
            splitContainerBottom.Location = new Point(0, 25);
            splitContainerBottom.Orientation = Orientation.Horizontal;
            // 
            // 
            // 
            splitContainerBottom.Panel1.Controls.Add(splitContainerLeft);
            // 
            // 
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
            richTextBoxOutput.Size = new Size(800, 166);
            richTextBoxOutput.TabIndex = 0;
            richTextBoxOutput.Text = "";
            // 
            // kryptonToolStrip1
            // 
            kryptonToolStrip1.Font = new Font("Segoe UI", 9F);
            kryptonToolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButtonSettings, toolStripButtonDevelopmentConsole });
            kryptonToolStrip1.Location = new Point(0, 0);
            kryptonToolStrip1.Name = "kryptonToolStrip1";
            kryptonToolStrip1.Size = new Size(800, 25);
            kryptonToolStrip1.TabIndex = 3;
            kryptonToolStrip1.Text = "kryptonToolStrip1";
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
            // toolStripButtonDevelopmentConsole
            // 
            toolStripButtonDevelopmentConsole.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonDevelopmentConsole.Image = (Image)resources.GetObject("toolStripButtonDevelopmentConsole.Image");
            toolStripButtonDevelopmentConsole.ImageTransparentColor = Color.Magenta;
            toolStripButtonDevelopmentConsole.Name = "toolStripButtonDevelopmentConsole";
            toolStripButtonDevelopmentConsole.Size = new Size(23, 22);
            toolStripButtonDevelopmentConsole.Text = "Development Console";
            toolStripButtonDevelopmentConsole.ToolTipText = "Development Console";
            toolStripButtonDevelopmentConsole.Click += ToolStripButtonDevelopmentConsole_Click;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 625);
            Controls.Add(splitContainerBottom);
            Controls.Add(kryptonToolStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormMain";
            Text = "Asset Manager";
            (splitContainerLeft.Panel1).EndInit();
            splitContainerLeft.Panel1.ResumeLayout(false);
            (splitContainerLeft.Panel2).EndInit();
            splitContainerLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerLeft).EndInit();
            (splitContainerRight.Panel1).EndInit();
            (splitContainerRight.Panel2).EndInit();
            splitContainerRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).EndInit();
            (splitContainerProperties.Panel1).EndInit();
            splitContainerProperties.Panel1.ResumeLayout(false);
            splitContainerProperties.Panel1.PerformLayout();
            (splitContainerProperties.Panel2).EndInit();
            splitContainerProperties.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerProperties).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxPreview).EndInit();
            (splitContainerBottom.Panel1).EndInit();
            splitContainerBottom.Panel1.ResumeLayout(false);
            (splitContainerBottom.Panel2).EndInit();
            splitContainerBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerBottom).EndInit();
            kryptonToolStrip1.ResumeLayout(false);
            kryptonToolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private KryptonSplitContainer splitContainerLeft;
        private DoubleBufferedTreeView treeViewAssets;
        private KryptonSplitContainer splitContainerRight;
        private KryptonSplitContainer splitContainerBottom;
        private KryptonSplitContainer splitContainerProperties;
        private KryptonPictureBox pictureBoxPreview;
        private KryptonListView listViewProperties;
        private KryptonRichTextBox richTextBoxOutput;
        private KryptonToolStrip kryptonToolStrip1;
        private ToolStripButton toolStripButtonSettings;
        private ToolStripButton toolStripButtonDevelopmentConsole;
        private ColumnHeader columnHeaderName;
        private ColumnHeader columnHeaderValue;
        private ColumnHeader columnHeaderDefault;
    }
}
