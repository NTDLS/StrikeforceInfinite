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
            kryptonNumericUpDownWorkingMin = new NumericUpDown();
            kryptonLabelName = new Label();
            kryptonButtonCancel = new Button();
            kryptonButtonSave = new Button();
            kryptonTextBoxDescription = new TextBox();
            kryptonLabelDescription = new Label();
            kryptonNumericUpDownWorkingMax = new NumericUpDown();
            kryptonLabelWorkingMin = new Label();
            kryptonLabelWorkingMax = new Label();
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
            kryptonNumericUpDownWorkingMin.TabIndex = 2;
            kryptonNumericUpDownWorkingMin.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // kryptonLabelName
            // 
            kryptonLabelName.Location = new Point(12, 12);
            kryptonLabelName.Name = "kryptonLabelName";
            kryptonLabelName.Size = new Size(346, 25);
            kryptonLabelName.TabIndex = 0;
            kryptonLabelName.Text = "Property Name";
            // 
            // kryptonButtonCancel
            // 
            kryptonButtonCancel.Location = new Point(265, 190);
            kryptonButtonCancel.Name = "kryptonButtonCancel";
            kryptonButtonCancel.Size = new Size(90, 25);
            kryptonButtonCancel.TabIndex = 8;
            
            kryptonButtonCancel.Text = "Cancel";
            kryptonButtonCancel.Click += ButtonCancel_Click;
            // 
            // kryptonButtonSave
            // 
            kryptonButtonSave.Location = new Point(169, 190);
            kryptonButtonSave.Name = "kryptonButtonSave";
            kryptonButtonSave.Size = new Size(90, 25);
            kryptonButtonSave.TabIndex = 7;
            
            kryptonButtonSave.Text = "Save";
            kryptonButtonSave.Click += ButtonSave_Click;
            // 
            // kryptonTextBoxDescription
            // 
            kryptonTextBoxDescription.Location = new Point(12, 105);
            kryptonTextBoxDescription.Multiline = true;
            kryptonTextBoxDescription.Name = "kryptonTextBoxDescription";
            kryptonTextBoxDescription.ReadOnly = true;
            kryptonTextBoxDescription.ScrollBars = ScrollBars.Vertical;
            kryptonTextBoxDescription.Size = new Size(343, 79);
            kryptonTextBoxDescription.TabIndex = 6;
            kryptonTextBoxDescription.Text = "Description";
            // 
            // kryptonLabelDescription
            // 
            kryptonLabelDescription.Location = new Point(12, 74);
            kryptonLabelDescription.Name = "kryptonLabelDescription";
            kryptonLabelDescription.Size = new Size(90, 25);
            kryptonLabelDescription.TabIndex = 5;
            kryptonLabelDescription.Text = "Description";
            // 
            // kryptonNumericUpDownWorkingMax
            // 
            kryptonNumericUpDownWorkingMax.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            kryptonNumericUpDownWorkingMax.Location = new Point(229, 44);
            kryptonNumericUpDownWorkingMax.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            kryptonNumericUpDownWorkingMax.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            kryptonNumericUpDownWorkingMax.Name = "kryptonNumericUpDownWorkingMax";
            kryptonNumericUpDownWorkingMax.Size = new Size(129, 22);
            kryptonNumericUpDownWorkingMax.TabIndex = 4;
            kryptonNumericUpDownWorkingMax.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // kryptonLabelWorkingMin
            // 
            kryptonLabelWorkingMin.Location = new Point(12, 43);
            kryptonLabelWorkingMin.Name = "kryptonLabelWorkingMin";
            kryptonLabelWorkingMin.Size = new Size(32, 25);
            kryptonLabelWorkingMin.TabIndex = 1;
            kryptonLabelWorkingMin.Text = "Min";
            // 
            // kryptonLabelWorkingMax
            // 
            kryptonLabelWorkingMax.Location = new Point(189, 43);
            kryptonLabelWorkingMax.Name = "kryptonLabelWorkingMax";
            kryptonLabelWorkingMax.Size = new Size(34, 25);
            kryptonLabelWorkingMax.TabIndex = 3;
            kryptonLabelWorkingMax.Text = "Max";
            // 
            // FormPropertyRangeInt
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 236);
            Controls.Add(kryptonLabelWorkingMax);
            Controls.Add(kryptonLabelWorkingMin);
            Controls.Add(kryptonNumericUpDownWorkingMax);
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

        private NumericUpDown kryptonNumericUpDownWorkingMin;
        private Label kryptonLabelName;
        private Button kryptonButtonCancel;
        private Button kryptonButtonSave;
        private TextBox kryptonTextBoxDescription;
        private Label kryptonLabelDescription;
        private NumericUpDown kryptonNumericUpDownWorkingMax;
        private Label kryptonLabelWorkingMin;
        private Label kryptonLabelWorkingMax;
    }
}