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
            kryptonComboBoxTheme = new Krypton.Toolkit.KryptonComboBox();
            kryptonLabelTheme = new Krypton.Toolkit.KryptonLabel();
            kryptonButtonSave = new Krypton.Toolkit.KryptonButton();
            kryptonButtonCancel = new Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)kryptonComboBoxTheme).BeginInit();
            SuspendLayout();
            // 
            // kryptonComboBoxTheme
            // 
            kryptonComboBoxTheme.DropDownWidth = 284;
            kryptonComboBoxTheme.Location = new Point(12, 43);
            kryptonComboBoxTheme.Name = "kryptonComboBoxTheme";
            kryptonComboBoxTheme.Size = new Size(284, 22);
            kryptonComboBoxTheme.StateCommon.ComboBox.Content.TextH = Krypton.Toolkit.PaletteRelativeAlign.Near;
            kryptonComboBoxTheme.TabIndex = 0;
            kryptonComboBoxTheme.Text = "Theme";
            // 
            // kryptonLabelTheme
            // 
            kryptonLabelTheme.Location = new Point(12, 12);
            kryptonLabelTheme.Name = "kryptonLabelTheme";
            kryptonLabelTheme.Size = new Size(90, 25);
            kryptonLabelTheme.TabIndex = 2;
            kryptonLabelTheme.Values.Text = "Theme";
            // 
            // kryptonButtonSave
            // 
            kryptonButtonSave.Location = new Point(448, 116);
            kryptonButtonSave.Name = "kryptonButtonSave";
            kryptonButtonSave.Size = new Size(90, 25);
            kryptonButtonSave.TabIndex = 3;
            kryptonButtonSave.Values.DropDownArrowColor = Color.Empty;
            kryptonButtonSave.Values.Text = "Save";
            kryptonButtonSave.Click += kryptonButtonSave_Click;
            // 
            // kryptonButtonCancel
            // 
            kryptonButtonCancel.Location = new Point(544, 116);
            kryptonButtonCancel.Name = "kryptonButtonCancel";
            kryptonButtonCancel.Size = new Size(90, 25);
            kryptonButtonCancel.TabIndex = 4;
            kryptonButtonCancel.Values.DropDownArrowColor = Color.Empty;
            kryptonButtonCancel.Values.Text = "Cancel";
            kryptonButtonCancel.Click += kryptonButtonCancel_Click;
            // 
            // FormSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(649, 155);
            Controls.Add(kryptonButtonCancel);
            Controls.Add(kryptonButtonSave);
            Controls.Add(kryptonLabelTheme);
            Controls.Add(kryptonComboBoxTheme);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormSettings";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)kryptonComboBoxTheme).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Krypton.Toolkit.KryptonComboBox kryptonComboBoxTheme;
        private Krypton.Toolkit.KryptonLabel kryptonLabelTheme;
        private Krypton.Toolkit.KryptonButton kryptonButtonSave;
        private Krypton.Toolkit.KryptonButton kryptonButtonCancel;
    }
}