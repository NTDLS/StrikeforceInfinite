namespace Si.AssetExplorer.Forms
{
    partial class FormPropertyBoolean
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPropertyBoolean));
            kryptonButtonCancel = new Krypton.Toolkit.KryptonButton();
            kryptonButtonSave = new Krypton.Toolkit.KryptonButton();
            kryptonTextBoxDescription = new Krypton.Toolkit.KryptonTextBox();
            kryptonLabelDescription = new Krypton.Toolkit.KryptonLabel();
            kryptonCheckBoxDefault = new Krypton.Toolkit.KryptonCheckBox();
            kryptonCheckBoxWorking = new Krypton.Toolkit.KryptonCheckBox();
            SuspendLayout();
            // 
            // kryptonButtonCancel
            // 
            kryptonButtonCancel.Location = new Point(266, 190);
            kryptonButtonCancel.Name = "kryptonButtonCancel";
            kryptonButtonCancel.Size = new Size(90, 25);
            kryptonButtonCancel.TabIndex = 6;
            kryptonButtonCancel.Values.DropDownArrowColor = Color.Empty;
            kryptonButtonCancel.Values.Text = "Cancel";
            kryptonButtonCancel.Click += KryptonButtonCancel_Click;
            // 
            // kryptonButtonSave
            // 
            kryptonButtonSave.Location = new Point(170, 190);
            kryptonButtonSave.Name = "kryptonButtonSave";
            kryptonButtonSave.Size = new Size(90, 25);
            kryptonButtonSave.TabIndex = 5;
            kryptonButtonSave.Values.DropDownArrowColor = Color.Empty;
            kryptonButtonSave.Values.Text = "Save";
            kryptonButtonSave.Click += KryptonButtonSave_Click;
            // 
            // kryptonTextBoxDescription
            // 
            kryptonTextBoxDescription.Location = new Point(12, 105);
            kryptonTextBoxDescription.Multiline = true;
            kryptonTextBoxDescription.Name = "kryptonTextBoxDescription";
            kryptonTextBoxDescription.ReadOnly = true;
            kryptonTextBoxDescription.Size = new Size(343, 79);
            kryptonTextBoxDescription.TabIndex = 8;
            kryptonTextBoxDescription.Text = "Description";
            // 
            // kryptonLabelDescription
            // 
            kryptonLabelDescription.Location = new Point(12, 74);
            kryptonLabelDescription.Name = "kryptonLabelDescription";
            kryptonLabelDescription.Size = new Size(90, 25);
            kryptonLabelDescription.TabIndex = 9;
            kryptonLabelDescription.Values.Text = "Description";
            // 
            // kryptonCheckBoxDefault
            // 
            kryptonCheckBoxDefault.Location = new Point(12, 43);
            kryptonCheckBoxDefault.Name = "kryptonCheckBoxDefault";
            kryptonCheckBoxDefault.Size = new Size(97, 25);
            kryptonCheckBoxDefault.TabIndex = 15;
            kryptonCheckBoxDefault.Values.Text = "Default Value";
            // 
            // kryptonCheckBoxWorking
            // 
            kryptonCheckBoxWorking.Location = new Point(12, 12);
            kryptonCheckBoxWorking.Name = "kryptonCheckBoxWorking";
            kryptonCheckBoxWorking.Size = new Size(106, 25);
            kryptonCheckBoxWorking.TabIndex = 16;
            kryptonCheckBoxWorking.Values.Text = "Property Name";
            // 
            // FormPropertyBoolean
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 229);
            Controls.Add(kryptonCheckBoxWorking);
            Controls.Add(kryptonCheckBoxDefault);
            Controls.Add(kryptonLabelDescription);
            Controls.Add(kryptonTextBoxDescription);
            Controls.Add(kryptonButtonCancel);
            Controls.Add(kryptonButtonSave);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormPropertyBoolean";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Krypton.Toolkit.KryptonButton kryptonButtonCancel;
        private Krypton.Toolkit.KryptonButton kryptonButtonSave;
        private Krypton.Toolkit.KryptonTextBox kryptonTextBoxDescription;
        private Krypton.Toolkit.KryptonLabel kryptonLabelDescription;
        private Krypton.Toolkit.KryptonCheckBox kryptonCheckBoxDefault;
        private Krypton.Toolkit.KryptonCheckBox kryptonCheckBoxWorking;
    }
}