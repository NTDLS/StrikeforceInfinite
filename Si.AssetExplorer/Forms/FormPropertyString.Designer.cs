namespace Si.AssetExplorer.Forms
{
    partial class FormPropertyString
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPropertyString));
            kryptonLabelName = new Krypton.Toolkit.KryptonLabel();
            kryptonButtonCancel = new Krypton.Toolkit.KryptonButton();
            kryptonButtonSave = new Krypton.Toolkit.KryptonButton();
            kryptonTextBoxDescription = new Krypton.Toolkit.KryptonTextBox();
            kryptonLabelDescription = new Krypton.Toolkit.KryptonLabel();
            kryptonLabelDefaultValue = new Krypton.Toolkit.KryptonLabel();
            kryptonTextBoxWorking = new Krypton.Toolkit.KryptonTextBox();
            kryptonTextBoxDefault = new Krypton.Toolkit.KryptonTextBox();
            SuspendLayout();
            // 
            // kryptonLabelName
            // 
            kryptonLabelName.Location = new Point(12, 12);
            kryptonLabelName.Name = "kryptonLabelName";
            kryptonLabelName.Size = new Size(346, 25);
            kryptonLabelName.TabIndex = 1;
            kryptonLabelName.Values.Text = "Property Name";
            // 
            // kryptonButtonCancel
            // 
            kryptonButtonCancel.Location = new Point(268, 246);
            kryptonButtonCancel.Name = "kryptonButtonCancel";
            kryptonButtonCancel.Size = new Size(90, 25);
            kryptonButtonCancel.TabIndex = 6;
            kryptonButtonCancel.Values.DropDownArrowColor = Color.Empty;
            kryptonButtonCancel.Values.Text = "Cancel";
            kryptonButtonCancel.Click += KryptonButtonCancel_Click;
            // 
            // kryptonButtonSave
            // 
            kryptonButtonSave.Location = new Point(172, 246);
            kryptonButtonSave.Name = "kryptonButtonSave";
            kryptonButtonSave.Size = new Size(90, 25);
            kryptonButtonSave.TabIndex = 5;
            kryptonButtonSave.Values.DropDownArrowColor = Color.Empty;
            kryptonButtonSave.Values.Text = "Save";
            kryptonButtonSave.Click += KryptonButtonSave_Click;
            // 
            // kryptonTextBoxDescription
            // 
            kryptonTextBoxDescription.Location = new Point(12, 161);
            kryptonTextBoxDescription.Multiline = true;
            kryptonTextBoxDescription.Name = "kryptonTextBoxDescription";
            kryptonTextBoxDescription.ReadOnly = true;
            kryptonTextBoxDescription.Size = new Size(343, 79);
            kryptonTextBoxDescription.TabIndex = 8;
            kryptonTextBoxDescription.Text = "Description";
            // 
            // kryptonLabelDescription
            // 
            kryptonLabelDescription.Location = new Point(12, 130);
            kryptonLabelDescription.Name = "kryptonLabelDescription";
            kryptonLabelDescription.Size = new Size(90, 25);
            kryptonLabelDescription.TabIndex = 9;
            kryptonLabelDescription.Values.Text = "Description";
            // 
            // kryptonLabelDefaultValue
            // 
            kryptonLabelDefaultValue.Location = new Point(12, 71);
            kryptonLabelDefaultValue.Name = "kryptonLabelDefaultValue";
            kryptonLabelDefaultValue.Size = new Size(90, 25);
            kryptonLabelDefaultValue.TabIndex = 12;
            kryptonLabelDefaultValue.Values.Text = "Default Value";
            // 
            // kryptonTextBoxWorking
            // 
            kryptonTextBoxWorking.Location = new Point(12, 43);
            kryptonTextBoxWorking.Name = "kryptonTextBoxWorking";
            kryptonTextBoxWorking.Size = new Size(343, 23);
            kryptonTextBoxWorking.TabIndex = 14;
            kryptonTextBoxWorking.Text = "kryptonTextBox1";
            // 
            // kryptonTextBoxDefault
            // 
            kryptonTextBoxDefault.Location = new Point(12, 102);
            kryptonTextBoxDefault.Name = "kryptonTextBoxDefault";
            kryptonTextBoxDefault.Size = new Size(346, 23);
            kryptonTextBoxDefault.TabIndex = 15;
            kryptonTextBoxDefault.Text = "kryptonTextBox2";
            // 
            // FormPropertyString
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 295);
            Controls.Add(kryptonTextBoxDefault);
            Controls.Add(kryptonTextBoxWorking);
            Controls.Add(kryptonLabelDefaultValue);
            Controls.Add(kryptonLabelDescription);
            Controls.Add(kryptonTextBoxDescription);
            Controls.Add(kryptonButtonCancel);
            Controls.Add(kryptonButtonSave);
            Controls.Add(kryptonLabelName);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormPropertyString";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Krypton.Toolkit.KryptonLabel kryptonLabelName;
        private Krypton.Toolkit.KryptonButton kryptonButtonCancel;
        private Krypton.Toolkit.KryptonButton kryptonButtonSave;
        private Krypton.Toolkit.KryptonTextBox kryptonTextBoxDescription;
        private Krypton.Toolkit.KryptonLabel kryptonLabelDescription;
        private Krypton.Toolkit.KryptonLabel kryptonLabelDefaultValue;
        private Krypton.Toolkit.KryptonTextBox kryptonTextBoxWorking;
        private Krypton.Toolkit.KryptonTextBox kryptonTextBoxDefault;
    }
}