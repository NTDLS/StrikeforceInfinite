namespace Si.AssetExplorer.Forms
{
    partial class FormPropertyRangeFloat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPropertyRangeFloat));
            kryptonNumericUpDownWorkingMin = new NumericUpDown();
            kryptonLabelName = new Label();
            kryptonButtonCancel = new Button();
            kryptonButtonSave = new Button();
            kryptonTextBoxDescription = new TextBox();
            kryptonLabelDescription = new Label();
            kryptonNumericUpDownWorkingMax = new NumericUpDown();
            kryptonLabelWorkingMin = new Label();
            kryptonLabelWorkingMax = new Label();
            ((System.ComponentModel.ISupportInitialize)kryptonNumericUpDownWorkingMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)kryptonNumericUpDownWorkingMax).BeginInit();
            SuspendLayout();
            // 
            // kryptonNumericUpDownWorkingMin
            // 
            kryptonNumericUpDownWorkingMin.DecimalPlaces = 5;
            kryptonNumericUpDownWorkingMin.Location = new Point(50, 40);
            kryptonNumericUpDownWorkingMin.Name = "kryptonNumericUpDownWorkingMin";
            kryptonNumericUpDownWorkingMin.Size = new Size(129, 23);
            kryptonNumericUpDownWorkingMin.TabIndex = 2;
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
            kryptonButtonCancel.Location = new Point(265, 187);
            kryptonButtonCancel.Name = "kryptonButtonCancel";
            kryptonButtonCancel.Size = new Size(90, 25);
            kryptonButtonCancel.TabIndex = 8;
            kryptonButtonCancel.Text = "Cancel";
            kryptonButtonCancel.Click += ButtonCancel_Click;
            // 
            // kryptonButtonSave
            // 
            kryptonButtonSave.Location = new Point(169, 187);
            kryptonButtonSave.Name = "kryptonButtonSave";
            kryptonButtonSave.Size = new Size(90, 25);
            kryptonButtonSave.TabIndex = 7;
            kryptonButtonSave.Text = "Save";
            kryptonButtonSave.Click += ButtonSave_Click;
            // 
            // kryptonTextBoxDescription
            // 
            kryptonTextBoxDescription.Location = new Point(12, 102);
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
            kryptonLabelDescription.Location = new Point(12, 71);
            kryptonLabelDescription.Name = "kryptonLabelDescription";
            kryptonLabelDescription.Size = new Size(90, 25);
            kryptonLabelDescription.TabIndex = 5;
            kryptonLabelDescription.Text = "Description";
            // 
            // kryptonNumericUpDownWorkingMax
            // 
            kryptonNumericUpDownWorkingMax.DecimalPlaces = 5;
            kryptonNumericUpDownWorkingMax.Location = new Point(229, 40);
            kryptonNumericUpDownWorkingMax.Name = "kryptonNumericUpDownWorkingMax";
            kryptonNumericUpDownWorkingMax.Size = new Size(129, 23);
            kryptonNumericUpDownWorkingMax.TabIndex = 4;
            // 
            // kryptonLabelWorkingMin
            // 
            kryptonLabelWorkingMin.Location = new Point(12, 43);
            kryptonLabelWorkingMin.Name = "kryptonLabelWorkingMin";
            kryptonLabelWorkingMin.Size = new Size(32, 20);
            kryptonLabelWorkingMin.TabIndex = 1;
            kryptonLabelWorkingMin.Text = "Min";
            // 
            // kryptonLabelWorkingMax
            // 
            kryptonLabelWorkingMax.Location = new Point(189, 42);
            kryptonLabelWorkingMax.Name = "kryptonLabelWorkingMax";
            kryptonLabelWorkingMax.Size = new Size(34, 21);
            kryptonLabelWorkingMax.TabIndex = 3;
            kryptonLabelWorkingMax.Text = "Max";
            // 
            // FormPropertyRangeFloat
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 224);
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
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormPropertyRangeFloat";
            Opacity = 0.95D;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ((System.ComponentModel.ISupportInitialize)kryptonNumericUpDownWorkingMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)kryptonNumericUpDownWorkingMax).EndInit();
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