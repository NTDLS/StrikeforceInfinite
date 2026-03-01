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
            numericUpDown = new NumericUpDown();
            labelName = new Label();
            buttonCancel = new Button();
            buttonSave = new Button();
            textBoxDescription = new TextBox();
            labelDescription = new Label();
            ((System.ComponentModel.ISupportInitialize)numericUpDown).BeginInit();
            SuspendLayout();
            // 
            // numericUpDown
            // 
            numericUpDown.DecimalPlaces = 5;
            numericUpDown.Location = new Point(12, 43);
            numericUpDown.Name = "numericUpDown";
            numericUpDown.Size = new Size(139, 23);
            numericUpDown.TabIndex = 1;
            // 
            // labelName
            // 
            labelName.Location = new Point(12, 15);
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
            buttonCancel.TabIndex = 5;
            buttonCancel.Text = "Cancel";
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(169, 187);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(90, 25);
            buttonSave.TabIndex = 4;
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
            textBoxDescription.TabIndex = 3;
            textBoxDescription.Text = "Description";
            // 
            // labelDescription
            // 
            labelDescription.Location = new Point(12, 74);
            labelDescription.Name = "labelDescription";
            labelDescription.Size = new Size(90, 25);
            labelDescription.TabIndex = 2;
            labelDescription.Text = "Description";
            // 
            // FormPropertyPicker
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 224);
            Controls.Add(labelDescription);
            Controls.Add(textBoxDescription);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            Controls.Add(labelName);
            Controls.Add(numericUpDown);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormPropertyPicker";
            Opacity = 0.95D;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ((System.ComponentModel.ISupportInitialize)numericUpDown).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private NumericUpDown numericUpDown;
        private Label labelName;
        private Button buttonCancel;
        private Button buttonSave;
        private TextBox textBoxDescription;
        private Label labelDescription;
    }
}