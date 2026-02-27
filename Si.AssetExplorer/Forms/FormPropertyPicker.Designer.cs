namespace Si.AssetExplorer.Forms
{
    partial class FormPropertyPicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPropertyPicker));
            kryptonNumericUpDown = new Krypton.Toolkit.KryptonNumericUpDown();
            kryptonLabelName = new Krypton.Toolkit.KryptonLabel();
            kryptonButtonCancel = new Krypton.Toolkit.KryptonButton();
            kryptonButtonSave = new Krypton.Toolkit.KryptonButton();
            kryptonTextBoxDescription = new Krypton.Toolkit.KryptonTextBox();
            kryptonLabelDescription = new Krypton.Toolkit.KryptonLabel();
            kryptonLabelDefaultValue = new Krypton.Toolkit.KryptonLabel();
            kryptonNumericUpDownDefaultValue = new Krypton.Toolkit.KryptonNumericUpDown();
            SuspendLayout();
            // 
            // kryptonNumericUpDown
            // 
            kryptonNumericUpDown.AllowDecimals = true;
            kryptonNumericUpDown.DecimalPlaces = 5;
            kryptonNumericUpDown.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            kryptonNumericUpDown.Location = new Point(12, 43);
            kryptonNumericUpDown.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            kryptonNumericUpDown.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            kryptonNumericUpDown.Name = "kryptonNumericUpDown";
            kryptonNumericUpDown.Size = new Size(139, 22);
            kryptonNumericUpDown.TabIndex = 0;
            kryptonNumericUpDown.Value = new decimal(new int[] { 0, 0, 0, 0 });
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
            kryptonTextBoxDescription.ScrollBars = ScrollBars.Vertical;
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
            // kryptonNumericUpDownDefaultValue
            // 
            kryptonNumericUpDownDefaultValue.AllowDecimals = true;
            kryptonNumericUpDownDefaultValue.DecimalPlaces = 5;
            kryptonNumericUpDownDefaultValue.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            kryptonNumericUpDownDefaultValue.Location = new Point(12, 102);
            kryptonNumericUpDownDefaultValue.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            kryptonNumericUpDownDefaultValue.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            kryptonNumericUpDownDefaultValue.Name = "kryptonNumericUpDownDefaultValue";
            kryptonNumericUpDownDefaultValue.ReadOnly = true;
            kryptonNumericUpDownDefaultValue.Size = new Size(139, 22);
            kryptonNumericUpDownDefaultValue.TabIndex = 11;
            kryptonNumericUpDownDefaultValue.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // FormPropertyPicker
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 295);
            Controls.Add(kryptonLabelDefaultValue);
            Controls.Add(kryptonNumericUpDownDefaultValue);
            Controls.Add(kryptonLabelDescription);
            Controls.Add(kryptonTextBoxDescription);
            Controls.Add(kryptonButtonCancel);
            Controls.Add(kryptonButtonSave);
            Controls.Add(kryptonLabelName);
            Controls.Add(kryptonNumericUpDown);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormPropertyPicker";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Krypton.Toolkit.KryptonNumericUpDown kryptonNumericUpDown;
        private Krypton.Toolkit.KryptonLabel kryptonLabelName;
        private Krypton.Toolkit.KryptonButton kryptonButtonCancel;
        private Krypton.Toolkit.KryptonButton kryptonButtonSave;
        private Krypton.Toolkit.KryptonTextBox kryptonTextBoxDescription;
        private Krypton.Toolkit.KryptonLabel kryptonLabelDescription;
        private Krypton.Toolkit.KryptonLabel kryptonLabelDefaultValue;
        private Krypton.Toolkit.KryptonNumericUpDown kryptonNumericUpDownDefaultValue;
    }
}