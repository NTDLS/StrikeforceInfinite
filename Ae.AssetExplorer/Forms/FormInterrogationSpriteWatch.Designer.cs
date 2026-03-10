using Ae.AssetExplorer.Controls;

namespace Ae.AssetExplorer.Forms
{
    partial class FormInterrogationSpriteWatch
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInterrogationSpriteWatch));
            splitContainerBody = new SplitContainer();
            richTexLog = new RichTextBox();
            columnHeaderName = new ColumnHeader();
            columnHeaderValue = new ColumnHeader();
            listViewVariables = new BufferedListView();
            ((System.ComponentModel.ISupportInitialize)splitContainerBody).BeginInit();
            splitContainerBody.Panel1.SuspendLayout();
            splitContainerBody.Panel2.SuspendLayout();
            splitContainerBody.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainerBody
            // 
            splitContainerBody.Dock = DockStyle.Fill;
            splitContainerBody.Location = new Point(0, 0);
            splitContainerBody.Name = "splitContainerBody";
            // 
            // splitContainerBody.Panel1
            // 
            splitContainerBody.Panel1.Controls.Add(listViewVariables);
            // 
            // splitContainerBody.Panel2
            // 
            splitContainerBody.Panel2.Controls.Add(richTexLog);
            splitContainerBody.Size = new Size(659, 563);
            splitContainerBody.SplitterDistance = 400;
            splitContainerBody.TabIndex = 1;
            // 
            // richTexLog
            // 
            richTexLog.Dock = DockStyle.Fill;
            richTexLog.Location = new Point(0, 0);
            richTexLog.Name = "richTexLog";
            richTexLog.Size = new Size(255, 563);
            richTexLog.TabIndex = 0;
            richTexLog.Text = "";
            // 
            // columnHeaderName
            // 
            columnHeaderName.Text = "Name";
            columnHeaderName.Width = 200;
            // 
            // columnHeaderValue
            // 
            columnHeaderValue.Text = "Value";
            columnHeaderValue.Width = 400;
            // 
            // listViewVariables
            // 
            listViewVariables.Dock = DockStyle.Fill;
            listViewVariables.Location = new Point(0, 0);
            listViewVariables.Name = "listViewVariables";
            listViewVariables.Size = new Size(400, 563);
            listViewVariables.TabIndex = 0;
            listViewVariables.UseCompatibleStateImageBehavior = false;
            listViewVariables.Columns.AddRange(new ColumnHeader[] { columnHeaderName, columnHeaderValue });
            listViewVariables.GridLines = true;
            listViewVariables.Location = new Point(0, 0);
            listViewVariables.Name = "listViewVariables";
            listViewVariables.Size = new Size(400, 563);
            listViewVariables.View = View.Details;

            // 
            // FormInterrogationSpriteWatch
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(659, 563);
            Controls.Add(splitContainerBody);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormInterrogationSpriteWatch";
            Text = "Axis Engine : Sprite Watch";
            splitContainerBody.Panel1.ResumeLayout(false);
            splitContainerBody.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerBody).EndInit();
            splitContainerBody.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.RichTextBox richTexLog;
        private System.Windows.Forms.SplitContainer splitContainerBody;
        private System.Windows.Forms.ColumnHeader columnHeaderName;
        private System.Windows.Forms.ColumnHeader columnHeaderValue;
        private BufferedListView listViewVariables;
    }
}