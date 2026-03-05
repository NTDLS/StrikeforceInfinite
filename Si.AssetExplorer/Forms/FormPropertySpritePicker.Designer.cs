namespace Si.AssetExplorer.Forms
{
    partial class FormPropertySpritePicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPropertySpritePicker));
            labelName = new Label();
            buttonCancel = new Button();
            buttonSave = new Button();
            textBoxDescription = new TextBox();
            labelDescription = new Label();
            treeViewAssets = new TreeView();
            SuspendLayout();
            // 
            // labelName
            // 
            labelName.Location = new Point(12, 15);
            labelName.Name = "labelName";
            labelName.Size = new Size(346, 25);
            labelName.TabIndex = 0;
            labelName.Text = "Property Name";
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(268, 407);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(90, 25);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "Cancel";
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(172, 407);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(90, 25);
            buttonSave.TabIndex = 3;
            buttonSave.Text = "Save";
            buttonSave.Click += ButtonSave_Click;
            // 
            // textBoxDescription
            // 
            textBoxDescription.Location = new Point(12, 322);
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.ReadOnly = true;
            textBoxDescription.ScrollBars = ScrollBars.Vertical;
            textBoxDescription.Size = new Size(346, 79);
            textBoxDescription.TabIndex = 2;
            textBoxDescription.Text = "Description";
            // 
            // labelDescription
            // 
            labelDescription.Location = new Point(12, 304);
            labelDescription.Name = "labelDescription";
            labelDescription.Size = new Size(90, 15);
            labelDescription.TabIndex = 1;
            labelDescription.Text = "Description";
            // 
            // treeViewAssets
            // 
            treeViewAssets.Location = new Point(12, 43);
            treeViewAssets.Name = "treeViewAssets";
            treeViewAssets.Size = new Size(346, 256);
            treeViewAssets.TabIndex = 0;
            // 
            // FormPropertySpritePicker
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(372, 444);
            Controls.Add(treeViewAssets);
            Controls.Add(labelDescription);
            Controls.Add(textBoxDescription);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(labelName);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormPropertySpritePicker";
            Opacity = 0.95D;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label labelName;
        private Button buttonCancel;
        private Button buttonSave;
        private TextBox textBoxDescription;
        private Label labelDescription;
        private TreeView treeViewAssets;
    }
}