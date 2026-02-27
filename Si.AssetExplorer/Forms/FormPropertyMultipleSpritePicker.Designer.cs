namespace Si.AssetExplorer.Forms
{
    partial class FormPropertyMultipleSpritePicker
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPropertyMultipleSpritePicker));
            kryptonNumericUpDown = new Krypton.Toolkit.KryptonNumericUpDown();
            kryptonLabelName = new Krypton.Toolkit.KryptonLabel();
            kryptonButtonCancel = new Krypton.Toolkit.KryptonButton();
            kryptonButtonSave = new Krypton.Toolkit.KryptonButton();
            kryptonTextBoxDescription = new Krypton.Toolkit.KryptonTextBox();
            kryptonLabelDescription = new Krypton.Toolkit.KryptonLabel();
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
            kryptonNumericUpDown.TabIndex = 1;
            kryptonNumericUpDown.Value = new decimal(new int[] { 0, 0, 0, 0 });
            // 
            // kryptonLabelName
            // 
            kryptonLabelName.Location = new Point(12, 12);
            kryptonLabelName.Name = "kryptonLabelName";
            kryptonLabelName.Size = new Size(346, 25);
            kryptonLabelName.TabIndex = 0;
            kryptonLabelName.Values.Text = "Property Name";
            // 
            // kryptonButtonCancel
            // 
            kryptonButtonCancel.Location = new Point(265, 187);
            kryptonButtonCancel.Name = "kryptonButtonCancel";
            kryptonButtonCancel.Size = new Size(90, 25);
            kryptonButtonCancel.TabIndex = 5;
            kryptonButtonCancel.Values.DropDownArrowColor = Color.Empty;
            kryptonButtonCancel.Values.Text = "Cancel";
            kryptonButtonCancel.Click += KryptonButtonCancel_Click;
            // 
            // kryptonButtonSave
            // 
            kryptonButtonSave.Location = new Point(169, 187);
            kryptonButtonSave.Name = "kryptonButtonSave";
            kryptonButtonSave.Size = new Size(90, 25);
            kryptonButtonSave.TabIndex = 4;
            kryptonButtonSave.Values.DropDownArrowColor = Color.Empty;
            kryptonButtonSave.Values.Text = "Save";
            kryptonButtonSave.Click += KryptonButtonSave_Click;
            // 
            // kryptonTextBoxDescription
            // 
            kryptonTextBoxDescription.Location = new Point(12, 102);
            kryptonTextBoxDescription.Multiline = true;
            kryptonTextBoxDescription.Name = "kryptonTextBoxDescription";
            kryptonTextBoxDescription.ReadOnly = true;
            kryptonTextBoxDescription.ScrollBars = ScrollBars.Vertical;
            kryptonTextBoxDescription.Size = new Size(343, 79);
            kryptonTextBoxDescription.TabIndex = 3;
            kryptonTextBoxDescription.Text = "Description";
            // 
            // kryptonLabelDescription
            // 
            kryptonLabelDescription.Location = new Point(12, 71);
            kryptonLabelDescription.Name = "kryptonLabelDescription";
            kryptonLabelDescription.Size = new Size(90, 25);
            kryptonLabelDescription.TabIndex = 2;
            kryptonLabelDescription.Values.Text = "Description";
            // 
            // FormPropertyMultipleSpritePicker
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 231);
            Controls.Add(kryptonLabelDescription);
            Controls.Add(kryptonTextBoxDescription);
            Controls.Add(kryptonButtonCancel);
            Controls.Add(kryptonButtonSave);
            Controls.Add(kryptonLabelName);
            Controls.Add(kryptonNumericUpDown);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormPropertyMultipleSpritePicker";
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
    }
}