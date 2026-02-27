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
            kryptonButtonSave = new Button();
            kryptonButtonCancel = new Button();
            SuspendLayout();
            // 
            // kryptonButtonSave
            // 
            kryptonButtonSave.Location = new Point(134, 132);
            kryptonButtonSave.Name = "kryptonButtonSave";
            kryptonButtonSave.Size = new Size(90, 25);
            kryptonButtonSave.TabIndex = 3;
            kryptonButtonSave.Text = "Save";
            kryptonButtonSave.Click += kryptonButtonSave_Click;
            // 
            // kryptonButtonCancel
            // 
            kryptonButtonCancel.Location = new Point(230, 132);
            kryptonButtonCancel.Name = "kryptonButtonCancel";
            kryptonButtonCancel.Size = new Size(90, 25);
            kryptonButtonCancel.TabIndex = 4;
            kryptonButtonCancel.Text = "Cancel";
            kryptonButtonCancel.Click += kryptonButtonCancel_Click;
            // 
            // FormSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(332, 169);
            Controls.Add(kryptonButtonCancel);
            Controls.Add(kryptonButtonSave);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            ResumeLayout(false);
        }

        #endregion
        private Button kryptonButtonSave;
        private Button kryptonButtonCancel;
    }
}