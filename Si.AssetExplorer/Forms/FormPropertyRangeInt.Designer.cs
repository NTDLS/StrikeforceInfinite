namespace Si.AssetExplorer.Forms
{
    partial class FormPropertyRangeInt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPropertyRangeInt));
            kryptonNumericUpDownWorkingMin = new Krypton.Toolkit.KryptonNumericUpDown();
            kryptonLabelName = new Krypton.Toolkit.KryptonLabel();
            kryptonButtonCancel = new Krypton.Toolkit.KryptonButton();
            kryptonButtonSave = new Krypton.Toolkit.KryptonButton();
            kryptonTextBoxDescription = new Krypton.Toolkit.KryptonTextBox();
            kryptonLabelDescription = new Krypton.Toolkit.KryptonLabel();
            kryptonLabelDefaultValue = new Krypton.Toolkit.KryptonLabel();
            kryptonNumericUpDownDefaultMin = new Krypton.Toolkit.KryptonNumericUpDown();
            kryptonNumericUpDownWorkingMax = new Krypton.Toolkit.KryptonNumericUpDown();
            kryptonNumericUpDownDefaultMax = new Krypton.Toolkit.KryptonNumericUpDown();
            kryptonLabelWorkingMin = new Krypton.Toolkit.KryptonLabel();
            kryptonLabelWorkingMax = new Krypton.Toolkit.KryptonLabel();
            kryptonLabelDefaultMin = new Krypton.Toolkit.KryptonLabel();
            kryptonLabelDefaultMax = new Krypton.Toolkit.KryptonLabel();
            SuspendLayout();
            // 
            // kryptonNumericUpDownWorkingMin
            // 
            kryptonNumericUpDownWorkingMin.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            kryptonNumericUpDownWorkingMin.Location = new Point(50, 44);
            kryptonNumericUpDownWorkingMin.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            kryptonNumericUpDownWorkingMin.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            kryptonNumericUpDownWorkingMin.Name = "kryptonNumericUpDownWorkingMin";
            kryptonNumericUpDownWorkingMin.Size = new Size(129, 22);
            kryptonNumericUpDownWorkingMin.TabIndex = 0;
            kryptonNumericUpDownWorkingMin.Value = new decimal(new int[] { 0, 0, 0, 0 });
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
            // kryptonNumericUpDownDefaultMin
            // 
            kryptonNumericUpDownDefaultMin.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            kryptonNumericUpDownDefaultMin.Location = new Point(50, 101);
            kryptonNumericUpDownDefaultMin.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            kryptonNumericUpDownDefaultMin.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            kryptonNumericUpDownDefaultMin.Name = "kryptonNumericUpDownDefaultMin";
            kryptonNumericUpDownDefaultMin.ReadOnly = true;
            kryptonNumericUpDownDefaultMin.Size = new Size(129, 22);
            kryptonNumericUpDownDefaultMin.TabIndex = 11;
            kryptonNumericUpDownDefaultMin.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // kryptonNumericUpDownWorkingMax
            // 
            kryptonNumericUpDownWorkingMax.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            kryptonNumericUpDownWorkingMax.Location = new Point(229, 44);
            kryptonNumericUpDownWorkingMax.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            kryptonNumericUpDownWorkingMax.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            kryptonNumericUpDownWorkingMax.Name = "kryptonNumericUpDownWorkingMax";
            kryptonNumericUpDownWorkingMax.Size = new Size(129, 22);
            kryptonNumericUpDownWorkingMax.TabIndex = 14;
            kryptonNumericUpDownWorkingMax.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // kryptonNumericUpDownDefaultMax
            // 
            kryptonNumericUpDownDefaultMax.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            kryptonNumericUpDownDefaultMax.Location = new Point(229, 100);
            kryptonNumericUpDownDefaultMax.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            kryptonNumericUpDownDefaultMax.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            kryptonNumericUpDownDefaultMax.Name = "kryptonNumericUpDownDefaultMax";
            kryptonNumericUpDownDefaultMax.Size = new Size(129, 22);
            kryptonNumericUpDownDefaultMax.TabIndex = 15;
            kryptonNumericUpDownDefaultMax.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // kryptonLabelWorkingMin
            // 
            kryptonLabelWorkingMin.Location = new Point(12, 43);
            kryptonLabelWorkingMin.Name = "kryptonLabelWorkingMin";
            kryptonLabelWorkingMin.Size = new Size(32, 25);
            kryptonLabelWorkingMin.TabIndex = 16;
            kryptonLabelWorkingMin.Values.Text = "Min";
            // 
            // kryptonLabelWorkingMax
            // 
            kryptonLabelWorkingMax.Location = new Point(189, 43);
            kryptonLabelWorkingMax.Name = "kryptonLabelWorkingMax";
            kryptonLabelWorkingMax.Size = new Size(34, 25);
            kryptonLabelWorkingMax.TabIndex = 17;
            kryptonLabelWorkingMax.Values.Text = "Max";
            // 
            // kryptonLabelDefaultMin
            // 
            kryptonLabelDefaultMin.Location = new Point(12, 99);
            kryptonLabelDefaultMin.Name = "kryptonLabelDefaultMin";
            kryptonLabelDefaultMin.Size = new Size(32, 25);
            kryptonLabelDefaultMin.TabIndex = 18;
            kryptonLabelDefaultMin.Values.Text = "Min";
            // 
            // kryptonLabelDefaultMax
            // 
            kryptonLabelDefaultMax.Location = new Point(189, 99);
            kryptonLabelDefaultMax.Name = "kryptonLabelDefaultMax";
            kryptonLabelDefaultMax.Size = new Size(34, 25);
            kryptonLabelDefaultMax.TabIndex = 20;
            kryptonLabelDefaultMax.Values.Text = "Max";
            // 
            // FormPropertyRangeInt
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 289);
            Controls.Add(kryptonLabelDefaultMax);
            Controls.Add(kryptonLabelDefaultMin);
            Controls.Add(kryptonLabelWorkingMax);
            Controls.Add(kryptonLabelWorkingMin);
            Controls.Add(kryptonNumericUpDownDefaultMax);
            Controls.Add(kryptonNumericUpDownWorkingMax);
            Controls.Add(kryptonLabelDefaultValue);
            Controls.Add(kryptonNumericUpDownDefaultMin);
            Controls.Add(kryptonLabelDescription);
            Controls.Add(kryptonTextBoxDescription);
            Controls.Add(kryptonButtonCancel);
            Controls.Add(kryptonButtonSave);
            Controls.Add(kryptonLabelName);
            Controls.Add(kryptonNumericUpDownWorkingMin);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormPropertyRangeInt";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Krypton.Toolkit.KryptonNumericUpDown kryptonNumericUpDownWorkingMin;
        private Krypton.Toolkit.KryptonLabel kryptonLabelName;
        private Krypton.Toolkit.KryptonButton kryptonButtonCancel;
        private Krypton.Toolkit.KryptonButton kryptonButtonSave;
        private Krypton.Toolkit.KryptonTextBox kryptonTextBoxDescription;
        private Krypton.Toolkit.KryptonLabel kryptonLabelDescription;
        private Krypton.Toolkit.KryptonLabel kryptonLabelDefaultValue;
        private Krypton.Toolkit.KryptonNumericUpDown kryptonNumericUpDownDefaultMin;
        private Krypton.Toolkit.KryptonNumericUpDown kryptonNumericUpDownWorkingMax;
        private Krypton.Toolkit.KryptonNumericUpDown kryptonNumericUpDownDefaultMax;
        private Krypton.Toolkit.KryptonLabel kryptonLabelWorkingMin;
        private Krypton.Toolkit.KryptonLabel kryptonLabelWorkingMax;
        private Krypton.Toolkit.KryptonLabel kryptonLabelDefaultMin;
        private Krypton.Toolkit.KryptonLabel kryptonLabelDefaultMax;
    }
}