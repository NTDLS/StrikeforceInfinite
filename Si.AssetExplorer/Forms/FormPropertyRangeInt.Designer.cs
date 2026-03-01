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
            numericUpDownWorkingMin = new NumericUpDown();
            labelName = new Label();
            buttonCancel = new Button();
            buttonSave = new Button();
            textBoxDescription = new TextBox();
            labelDescription = new Label();
            numericUpDownWorkingMax = new NumericUpDown();
            labelWorkingMin = new Label();
            labelWorkingMax = new Label();
            ((System.ComponentModel.ISupportInitialize)numericUpDownWorkingMin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownWorkingMax).BeginInit();
            SuspendLayout();
            // 
            // numericUpDownWorkingMin
            // 
            numericUpDownWorkingMin.Location = new Point(50, 40);
            numericUpDownWorkingMin.Name = "numericUpDownWorkingMin";
            numericUpDownWorkingMin.Size = new Size(129, 23);
            numericUpDownWorkingMin.TabIndex = 2;
            // 
            // labelName
            // 
            labelName.Location = new Point(12, 12);
            labelName.Name = "labelName";
            labelName.Size = new Size(346, 25);
            labelName.TabIndex = 0;
            labelName.Text = "Property Name";
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(265, 187);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(90, 25);
            buttonCancel.TabIndex = 8;
            buttonCancel.Text = "Cancel";
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(169, 187);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(90, 25);
            buttonSave.TabIndex = 7;
            buttonSave.Text = "Save";
            buttonSave.Click += ButtonSave_Click;
            // 
            // textBoxDescription
            // 
            textBoxDescription.Location = new Point(12, 102);
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.ReadOnly = true;
            textBoxDescription.ScrollBars = ScrollBars.Vertical;
            textBoxDescription.Size = new Size(343, 79);
            textBoxDescription.TabIndex = 6;
            textBoxDescription.Text = "Description";
            // 
            // labelDescription
            // 
            labelDescription.Location = new Point(12, 74);
            labelDescription.Name = "labelDescription";
            labelDescription.Size = new Size(90, 25);
            labelDescription.TabIndex = 5;
            labelDescription.Text = "Description";
            // 
            // numericUpDownWorkingMax
            // 
            numericUpDownWorkingMax.Location = new Point(229, 40);
            numericUpDownWorkingMax.Name = "numericUpDownWorkingMax";
            numericUpDownWorkingMax.Size = new Size(129, 23);
            numericUpDownWorkingMax.TabIndex = 4;
            // 
            // labelWorkingMin
            // 
            labelWorkingMin.Location = new Point(12, 43);
            labelWorkingMin.Name = "labelWorkingMin";
            labelWorkingMin.Size = new Size(32, 20);
            labelWorkingMin.TabIndex = 1;
            labelWorkingMin.Text = "Min";
            // 
            // labelWorkingMax
            // 
            labelWorkingMax.Location = new Point(189, 43);
            labelWorkingMax.Name = "labelWorkingMax";
            labelWorkingMax.Size = new Size(34, 20);
            labelWorkingMax.TabIndex = 3;
            labelWorkingMax.Text = "Max";
            // 
            // FormPropertyRangeInt
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 224);
            Controls.Add(labelWorkingMax);
            Controls.Add(labelWorkingMin);
            Controls.Add(numericUpDownWorkingMax);
            Controls.Add(labelDescription);
            Controls.Add(textBoxDescription);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(labelName);
            Controls.Add(numericUpDownWorkingMin);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormPropertyRangeInt";
            Opacity = 0.95D;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ((System.ComponentModel.ISupportInitialize)numericUpDownWorkingMin).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownWorkingMax).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown numericUpDownWorkingMin;
        private Label labelName;
        private Button buttonCancel;
        private Button buttonSave;
        private TextBox textBoxDescription;
        private Label labelDescription;
        private NumericUpDown numericUpDownWorkingMax;
        private Label labelWorkingMin;
        private Label labelWorkingMax;
    }
}