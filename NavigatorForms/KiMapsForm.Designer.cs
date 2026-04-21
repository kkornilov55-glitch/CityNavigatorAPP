namespace NavigatorForms
{
    partial class KiMapsForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            GraphPictureBox = new PictureBox();
            button1 = new Button();
            FromComboBox = new ComboBox();
            ToComboBox = new ComboBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)GraphPictureBox).BeginInit();
            SuspendLayout();
            // 
            // GraphPictureBox
            // 
            GraphPictureBox.Dock = DockStyle.Top;
            GraphPictureBox.Location = new Point(0, 0);
            GraphPictureBox.Name = "GraphPictureBox";
            GraphPictureBox.Size = new Size(1039, 417);
            GraphPictureBox.TabIndex = 0;
            GraphPictureBox.TabStop = false;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 18F);
            button1.Location = new Point(298, 559);
            button1.Name = "button1";
            button1.Size = new Size(387, 52);
            button1.TabIndex = 1;
            button1.Text = "Найти";
            button1.UseVisualStyleBackColor = true;
            // 
            // FromComboBox
            // 
            FromComboBox.FormattingEnabled = true;
            FromComboBox.Location = new Point(298, 508);
            FromComboBox.Name = "FromComboBox";
            FromComboBox.Size = new Size(151, 28);
            FromComboBox.TabIndex = 2;
            // 
            // ToComboBox
            // 
            ToComboBox.FormattingEnabled = true;
            ToComboBox.Location = new Point(534, 508);
            ToComboBox.Name = "ToComboBox";
            ToComboBox.Size = new Size(151, 28);
            ToComboBox.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F);
            label1.Location = new Point(467, 495);
            label1.Name = "label1";
            label1.Size = new Size(51, 41);
            label1.TabIndex = 4;
            label1.Text = "->";
            // 
            // KiMapsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1039, 651);
            Controls.Add(label1);
            Controls.Add(ToComboBox);
            Controls.Add(FromComboBox);
            Controls.Add(button1);
            Controls.Add(GraphPictureBox);
            Name = "KiMapsForm";
            Text = "KiMaps";
            ((System.ComponentModel.ISupportInitialize)GraphPictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox GraphPictureBox;
        private Button button1;
        private ComboBox FromComboBox;
        private ComboBox ToComboBox;
        private Label label1;
    }
}
