namespace Si.AssetExplorer
{
    partial class FormSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            buttonSave = new Button();
            buttonCancel = new Button();
            SuspendLayout();
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(134, 132);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(90, 25);
            buttonSave.TabIndex = 3;
            buttonSave.Text = "Save";
            buttonSave.Click += buttonSave_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(230, 132);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(90, 25);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "Cancel";
            buttonCancel.Click += buttonCancel_Click;
            // 
            // FormSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(332, 169);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            ResumeLayout(false);
        }

        #endregion
        private Button buttonSave;
        private Button buttonCancel;
    }
}