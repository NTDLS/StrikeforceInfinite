namespace Si.AssetExplorer.Forms
{
    partial class FormPropertyFloat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPropertyFloat));
            kryptonNumericUpDownWorking = new NumericUpDown();
            kryptonLabelName = new Label();
            kryptonButtonCancel = new Button();
            kryptonButtonSave = new Button();
            kryptonTextBoxDescription = new TextBox();
            kryptonLabelDescription = new Label();
            ((System.ComponentModel.ISupportInitialize)kryptonNumericUpDownWorking).BeginInit();
            SuspendLayout();
            // 
            // kryptonNumericUpDownWorking
            // 
            kryptonNumericUpDownWorking.DecimalPlaces = 5;
            kryptonNumericUpDownWorking.Location = new Point(12, 43);
            kryptonNumericUpDownWorking.Name = "kryptonNumericUpDownWorking";
            kryptonNumericUpDownWorking.Size = new Size(139, 23);
            kryptonNumericUpDownWorking.TabIndex = 1;
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
            kryptonButtonCancel.TabIndex = 5;
            kryptonButtonCancel.Text = "Cancel";
            kryptonButtonCancel.Click += ButtonCancel_Click;
            // 
            // kryptonButtonSave
            // 
            kryptonButtonSave.Location = new Point(169, 187);
            kryptonButtonSave.Name = "kryptonButtonSave";
            kryptonButtonSave.Size = new Size(90, 25);
            kryptonButtonSave.TabIndex = 4;
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
            kryptonTextBoxDescription.TabIndex = 3;
            kryptonTextBoxDescription.Text = "Description";
            // 
            // kryptonLabelDescription
            // 
            kryptonLabelDescription.Location = new Point(12, 71);
            kryptonLabelDescription.Name = "kryptonLabelDescription";
            kryptonLabelDescription.Size = new Size(90, 25);
            kryptonLabelDescription.TabIndex = 2;
            kryptonLabelDescription.Text = "Description";
            // 
            // FormPropertyFloat
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 224);
            Controls.Add(kryptonLabelDescription);
            Controls.Add(kryptonTextBoxDescription);
            Controls.Add(kryptonButtonCancel);
            Controls.Add(kryptonButtonSave);
            Controls.Add(kryptonLabelName);
            Controls.Add(kryptonNumericUpDownWorking);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormPropertyFloat";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ((System.ComponentModel.ISupportInitialize)kryptonNumericUpDownWorking).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown kryptonNumericUpDownWorking;
        private Label kryptonLabelName;
        private Button kryptonButtonCancel;
        private Button kryptonButtonSave;
        private TextBox kryptonTextBoxDescription;
        private Label kryptonLabelDescription;
    }
}