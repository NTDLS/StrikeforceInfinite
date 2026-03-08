namespace Ae.AssetExplorer.Forms
{
    partial class FormGetNewAssetName
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGetNewAssetName));
            buttonCancel = new Button();
            buttonCreate = new Button();
            textBoxAssetName = new TextBox();
            labelAssetName = new Label();
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
            // textBoxAssetName
            // 
            textBoxAssetName.Location = new Point(12, 27);
            textBoxAssetName.Name = "textBoxAssetName";
            textBoxAssetName.Size = new Size(344, 23);
            textBoxAssetName.TabIndex = 1;
            // 
            // labelAssetName
            // 
            labelAssetName.AutoSize = true;
            labelAssetName.Location = new Point(12, 9);
            labelAssetName.Name = "labelAssetName";
            labelAssetName.Size = new Size(97, 15);
            labelAssetName.TabIndex = 0;
            labelAssetName.Text = "New Asset Name";
            // 
            // FormGetNewAssetName
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 95);
            Controls.Add(labelAssetName);
            Controls.Add(textBoxAssetName);
            Controls.Add(buttonCancel);
            Controls.Add(buttonCreate);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormGetNewAssetName";
            Opacity = 0.95D;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Create Asset";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button buttonCancel;
        private Button buttonCreate;
        private TextBox textBoxAssetName;
        private Label labelAssetName;
    }
}