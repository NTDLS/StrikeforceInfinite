namespace Si.AssetExplorer.Forms
{
    partial class FormPropertyBoolean
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPropertyBoolean));
            buttonCancel = new Button();
            buttonSave = new Button();
            textBoxDescription = new TextBox();
            labelDescription = new Label();
            checkBoxWorking = new CheckBox();
            SuspendLayout();
            // 
            // buttonCancel
            // 
            buttonCancel.Location = new Point(266, 159);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(90, 25);
            buttonCancel.TabIndex = 4;
            buttonCancel.Text = "Cancel";
            buttonCancel.Click += ButtonCancel_Click;
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(170, 159);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(90, 25);
            buttonSave.TabIndex = 3;
            buttonSave.Text = "Save";
            buttonSave.Click += ButtonSave_Click;
            // 
            // textBoxDescription
            // 
            textBoxDescription.Location = new Point(13, 74);
            textBoxDescription.Multiline = true;
            textBoxDescription.Name = "textBoxDescription";
            textBoxDescription.ReadOnly = true;
            textBoxDescription.ScrollBars = ScrollBars.Vertical;
            textBoxDescription.Size = new Size(343, 79);
            textBoxDescription.TabIndex = 2;
            textBoxDescription.Text = "Description";
            // 
            // labelDescription
            // 
            labelDescription.Location = new Point(13, 46);
            labelDescription.Name = "labelDescription";
            labelDescription.Size = new Size(90, 25);
            labelDescription.TabIndex = 1;
            labelDescription.Text = "Description";
            // 
            // checkBoxWorking
            // 
            checkBoxWorking.Location = new Point(12, 12);
            checkBoxWorking.Name = "checkBoxWorking";
            checkBoxWorking.Size = new Size(344, 25);
            checkBoxWorking.TabIndex = 0;
            checkBoxWorking.Text = "Property Name";
            // 
            // FormPropertyBoolean
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(368, 206);
            Controls.Add(checkBoxWorking);
            Controls.Add(labelDescription);
            Controls.Add(textBoxDescription);
            Controls.Add(buttonCancel);
            Controls.Add(buttonSave);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FormPropertyBoolean";
            Opacity = 0.95D;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Property Edit";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button buttonCancel;
        private Button buttonSave;
        private TextBox textBoxDescription;
        private Label labelDescription;
        private CheckBox checkBoxWorking;
    }
}