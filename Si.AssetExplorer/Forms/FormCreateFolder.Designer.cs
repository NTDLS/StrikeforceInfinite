namespace Si.AssetExplorer.Forms
{
    partial class FormCreateFolder
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCreateFolder));
            buttonCancel = new Button();
            buttonCreate = new Button();
            textBoxFolderName = new TextBox();
            labelFolderName = new Label();
            SuspendLayout();
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(266, 56);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(90, 25);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "Cancel";
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // buttonCreate
            // 
            buttonCreate.Location = new Point(170, 56);
            buttonCreate.Name = "buttonCreate";
            buttonCreate.Size = new Size(90, 25);
            buttonCreate.TabIndex = 2;
            buttonCreate.Text = "Create";
            buttonCreate.Click += ButtonCreate_Click;
            // 
            // textBoxFolderName
            // 
            textBoxFolderName.Location = new Point(12, 27);
            textBoxFolderName.Name = "textBoxFolderName";
            textBoxFolderName.Size = new Size(344, 23);
            textBoxFolderName.TabIndex = 1;
            // 
            // labelFolderName
            // 
            labelFolderName.AutoSize = true;
            labelFolderName.Location = new Point(12, 9);
            labelFolderName.Name = "labelFolderName";
            labelFolderName.Size = new Size(75, 15);
            labelFolderName.TabIndex = 0;
            labelFolderName.Text = "Folder Name";
            // 
            // FormCreateFolder
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 95);
            Controls.Add(labelFolderName);
            Controls.Add(textBoxFolderName);
            Controls.Add(buttonCancel);
            Controls.Add(buttonCreate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormCreateFolder";
            Opacity = 0.95D;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button buttonCancel;
        private Button buttonCreate;
        private TextBox textBoxFolderName;
        private Label labelFolderName;
    }
}