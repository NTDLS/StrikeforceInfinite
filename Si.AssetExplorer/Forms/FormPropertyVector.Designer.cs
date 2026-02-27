namespace Si.AssetExplorer.Forms
{
    partial class FormPropertyVector
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPropertyVector));
            kryptonNumericUpDownWorkingX = new NumericUpDown();
            kryptonLabelName = new Label();
            kryptonButtonCancel = new Button();
            kryptonButtonSave = new Button();
            kryptonTextBoxDescription = new TextBox();
            kryptonLabelDescription = new Label();
            kryptonNumericUpDownWorkingY = new NumericUpDown();
            kryptonLabelWorkingX = new Label();
            kryptonLabelWorkingY = new Label();
            SuspendLayout();
            // 
            // kryptonNumericUpDownWorkingX
            // 
            kryptonNumericUpDownWorkingX.DecimalPlaces = 5;
            kryptonNumericUpDownWorkingX.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            kryptonNumericUpDownWorkingX.Location = new Point(40, 44);
            kryptonNumericUpDownWorkingX.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            kryptonNumericUpDownWorkingX.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            kryptonNumericUpDownWorkingX.Name = "kryptonNumericUpDownWorkingX";
            kryptonNumericUpDownWorkingX.Size = new Size(139, 22);
            kryptonNumericUpDownWorkingX.TabIndex = 2;
            kryptonNumericUpDownWorkingX.Value = new decimal(new int[] { 0, 0, 0, 0 });
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
            // kryptonNumericUpDownWorkingY
            // 
            kryptonNumericUpDownWorkingY.Increment = new decimal(new int[] { 1, 0, 0, 0 });
            kryptonNumericUpDownWorkingY.Location = new Point(213, 44);
            kryptonNumericUpDownWorkingY.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            kryptonNumericUpDownWorkingY.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            kryptonNumericUpDownWorkingY.Name = "kryptonNumericUpDownWorkingY";
            kryptonNumericUpDownWorkingY.Size = new Size(120, 22);
            kryptonNumericUpDownWorkingY.TabIndex = 4;
            kryptonNumericUpDownWorkingY.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // kryptonLabelWorkingX
            // 
            kryptonLabelWorkingX.Location = new Point(12, 43);
            kryptonLabelWorkingX.Name = "kryptonLabelWorkingX";
            kryptonLabelWorkingX.Size = new Size(22, 25);
            kryptonLabelWorkingX.TabIndex = 1;
            kryptonLabelWorkingX.Text = "X";
            // 
            // kryptonLabelWorkingY
            // 
            kryptonLabelWorkingY.Location = new Point(185, 43);
            kryptonLabelWorkingY.Name = "kryptonLabelWorkingY";
            kryptonLabelWorkingY.Size = new Size(22, 25);
            kryptonLabelWorkingY.TabIndex = 3;
            kryptonLabelWorkingY.Text = "Y";
            // 
            // FormPropertyVector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 235);
            Controls.Add(kryptonLabelWorkingY);
            Controls.Add(kryptonLabelWorkingX);
            Controls.Add(kryptonNumericUpDownWorkingY);
            Controls.Add(kryptonLabelDescription);
            Controls.Add(kryptonTextBoxDescription);
            Controls.Add(kryptonButtonCancel);
            Controls.Add(kryptonButtonSave);
            Controls.Add(kryptonLabelName);
            Controls.Add(kryptonNumericUpDownWorkingX);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormPropertyVector";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown kryptonNumericUpDownWorkingX;
        private Label kryptonLabelName;
        private Button kryptonButtonCancel;
        private Button kryptonButtonSave;
        private TextBox kryptonTextBoxDescription;
        private Label kryptonLabelDescription;
        private NumericUpDown kryptonNumericUpDownWorkingY;
        private Label kryptonLabelWorkingX;
        private Label kryptonLabelWorkingY;
    }
}