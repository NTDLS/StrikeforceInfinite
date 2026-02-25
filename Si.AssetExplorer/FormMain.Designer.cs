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
            splitContainerLeft = new SplitContainer();
            treeViewAssets = new TreeView();
            splitContainerRight = new SplitContainer();
            splitContainerProperties = new SplitContainer();
            pictureBoxPreview = new PictureBox();
            listViewProperties = new ListView();
            splitContainerBottom = new SplitContainer();
            richTextBoxOutput = new RichTextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainerLeft).BeginInit();
            splitContainerLeft.Panel1.SuspendLayout();
            splitContainerLeft.Panel2.SuspendLayout();
            splitContainerLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerRight).BeginInit();
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
            SuspendLayout();
            // 
            // splitContainerLeft
            // 
            splitContainerLeft.Dock = DockStyle.Fill;
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
            splitContainerLeft.Size = new Size(794, 443);
            splitContainerLeft.SplitterDistance = 223;
            splitContainerLeft.TabIndex = 0;
            // 
            // treeViewAssets
            // 
            treeViewAssets.Dock = DockStyle.Fill;
            treeViewAssets.Location = new Point(0, 0);
            treeViewAssets.Name = "treeViewAssets";
            treeViewAssets.Size = new Size(223, 443);
            treeViewAssets.TabIndex = 0;
            // 
            // splitContainerRight
            // 
            splitContainerRight.Dock = DockStyle.Fill;
            splitContainerRight.Location = new Point(0, 0);
            splitContainerRight.Name = "splitContainerRight";
            // 
            // splitContainerRight.Panel2
            // 
            splitContainerRight.Panel2.Controls.Add(splitContainerProperties);
            splitContainerRight.Size = new Size(567, 443);
            splitContainerRight.SplitterDistance = 399;
            splitContainerRight.TabIndex = 0;
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
            splitContainerProperties.Panel1.BackColor = Color.Black;
            splitContainerProperties.Panel1.Controls.Add(pictureBoxPreview);
            // 
            // splitContainerProperties.Panel2
            // 
            splitContainerProperties.Panel2.Controls.Add(listViewProperties);
            splitContainerProperties.Size = new Size(164, 443);
            splitContainerProperties.SplitterDistance = 175;
            splitContainerProperties.TabIndex = 0;
            // 
            // pictureBoxPreview
            // 
            pictureBoxPreview.BackColor = Color.CadetBlue;
            pictureBoxPreview.Location = new Point(36, 34);
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
            listViewProperties.Size = new Size(164, 264);
            listViewProperties.TabIndex = 0;
            listViewProperties.UseCompatibleStateImageBehavior = false;
            // 
            // splitContainerBottom
            // 
            splitContainerBottom.Dock = DockStyle.Fill;
            splitContainerBottom.Location = new Point(3, 3);
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
            splitContainerBottom.Size = new Size(794, 619);
            splitContainerBottom.SplitterDistance = 443;
            splitContainerBottom.TabIndex = 1;
            // 
            // richTextBoxOutput
            // 
            richTextBoxOutput.Dock = DockStyle.Fill;
            richTextBoxOutput.Location = new Point(0, 0);
            richTextBoxOutput.Name = "richTextBoxOutput";
            richTextBoxOutput.Size = new Size(794, 172);
            richTextBoxOutput.TabIndex = 0;
            richTextBoxOutput.Text = "";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 625);
            Controls.Add(splitContainerBottom);
            Name = "FormMain";
            Padding = new Padding(3);
            Text = "Asset Manager";
            splitContainerLeft.Panel1.ResumeLayout(false);
            splitContainerLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerLeft).EndInit();
            splitContainerLeft.ResumeLayout(false);
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
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainerLeft;
        private TreeView treeViewAssets;
        private SplitContainer splitContainerRight;
        private SplitContainer splitContainerBottom;
        private SplitContainer splitContainerProperties;
        private PictureBox pictureBoxPreview;
        private ListView listViewProperties;
        private RichTextBox richTextBoxOutput;
    }
}
