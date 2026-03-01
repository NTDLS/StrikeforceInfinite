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
            numericUpDownWorkingX = new NumericUpDown();
            labelName = new Label();
            buttonCancel = new Button();
            buttonSave = new Button();
            textBoxDescription = new TextBox();
            labelDescription = new Label();
            numericUpDownWorkingY = new NumericUpDown();
            labelWorkingX = new Label();
            labelWorkingY = new Label();
            ((System.ComponentModel.ISupportInitialize)numericUpDownWorkingX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownWorkingY).BeginInit();
            SuspendLayout();
            // 
            // numericUpDownWorkingX
            // 
            numericUpDownWorkingX.DecimalPlaces = 5;
            numericUpDownWorkingX.Location = new Point(40, 40);
            numericUpDownWorkingX.Name = "numericUpDownWorkingX";
            numericUpDownWorkingX.Size = new Size(139, 23);
            numericUpDownWorkingX.TabIndex = 2;
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
            // numericUpDownWorkingY
            // 
            numericUpDownWorkingY.Location = new Point(213, 40);
            numericUpDownWorkingY.Name = "numericUpDownWorkingY";
            numericUpDownWorkingY.Size = new Size(120, 23);
            numericUpDownWorkingY.TabIndex = 4;
            // 
            // labelWorkingX
            // 
            labelWorkingX.Location = new Point(12, 43);
            labelWorkingX.Name = "labelWorkingX";
            labelWorkingX.Size = new Size(22, 20);
            labelWorkingX.TabIndex = 1;
            labelWorkingX.Text = "X";
            // 
            // labelWorkingY
            // 
            labelWorkingY.Location = new Point(185, 43);
            labelWorkingY.Name = "labelWorkingY";
            labelWorkingY.Size = new Size(22, 20);
            labelWorkingY.TabIndex = 3;
            labelWorkingY.Text = "Y";
            // 
            // FormPropertyVector
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 224);
            Controls.Add(labelWorkingY);
            Controls.Add(labelWorkingX);
            Controls.Add(numericUpDownWorkingY);
            Controls.Add(labelDescription);
            Controls.Add(textBoxDescription);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(labelName);
            Controls.Add(numericUpDownWorkingX);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormPropertyVector";
            Opacity = 0.95D;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ((System.ComponentModel.ISupportInitialize)numericUpDownWorkingX).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownWorkingY).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown numericUpDownWorkingX;
        private Label labelName;
        private Button buttonCancel;
        private Button buttonSave;
        private TextBox textBoxDescription;
        private Label labelDescription;
        private NumericUpDown numericUpDownWorkingY;
        private Label labelWorkingX;
        private Label labelWorkingY;
    }
}