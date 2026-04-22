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
            StartButton = new Button();
            FromComboBox = new ComboBox();
            ToComboBox = new ComboBox();
            label1 = new Label();
            panel1 = new Panel();
            FastestWayCheckBox = new CheckBox();
            AltButton = new Button();
            ((System.ComponentModel.ISupportInitialize)GraphPictureBox).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // GraphPictureBox
            // 
            GraphPictureBox.Dock = DockStyle.Fill;
            GraphPictureBox.Location = new Point(0, 0);
            GraphPictureBox.Name = "GraphPictureBox";
            GraphPictureBox.Size = new Size(1494, 841);
            GraphPictureBox.TabIndex = 0;
            GraphPictureBox.TabStop = false;
            // 
            // StartButton
            // 
            StartButton.Font = new Font("Segoe UI", 14F);
            StartButton.Location = new Point(552, 5);
            StartButton.Name = "StartButton";
            StartButton.Size = new Size(265, 43);
            StartButton.TabIndex = 1;
            StartButton.Text = "Построить маршрут";
            StartButton.UseVisualStyleBackColor = true;
            StartButton.Click += StartButton_Click;
            // 
            // FromComboBox
            // 
            FromComboBox.FormattingEnabled = true;
            FromComboBox.Location = new Point(208, 17);
            FromComboBox.Name = "FromComboBox";
            FromComboBox.Size = new Size(132, 28);
            FromComboBox.TabIndex = 2;
            // 
            // ToComboBox
            // 
            ToComboBox.FormattingEnabled = true;
            ToComboBox.Location = new Point(414, 18);
            ToComboBox.Name = "ToComboBox";
            ToComboBox.Size = new Size(132, 28);
            ToComboBox.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 18F);
            label1.Location = new Point(357, 4);
            label1.Name = "label1";
            label1.Size = new Size(51, 41);
            label1.TabIndex = 4;
            label1.Text = "->";
            // 
            // panel1
            // 
            panel1.Controls.Add(AltButton);
            panel1.Controls.Add(FastestWayCheckBox);
            panel1.Controls.Add(StartButton);
            panel1.Controls.Add(FromComboBox);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(ToComboBox);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 783);
            panel1.Name = "panel1";
            panel1.Size = new Size(1494, 58);
            panel1.TabIndex = 5;
            // 
            // FastestWayCheckBox
            // 
            FastestWayCheckBox.AutoSize = true;
            FastestWayCheckBox.Location = new Point(10, 17);
            FastestWayCheckBox.Name = "FastestWayCheckBox";
            FastestWayCheckBox.Size = new Size(179, 24);
            FastestWayCheckBox.TabIndex = 5;
            FastestWayCheckBox.Text = "Самый быстрый путь";
            FastestWayCheckBox.UseVisualStyleBackColor = true;
            // 
            // AltButton
            // 
            AltButton.Font = new Font("Segoe UI", 14F);
            AltButton.Location = new Point(852, 5);
            AltButton.Name = "AltButton";
            AltButton.Size = new Size(169, 43);
            AltButton.TabIndex = 6;
            AltButton.Text = "Другой путь";
            AltButton.UseVisualStyleBackColor = true;
            AltButton.Click += AltButton_Click;
            // 
            // KiMapsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1494, 841);
            Controls.Add(panel1);
            Controls.Add(GraphPictureBox);
            Name = "KiMapsForm";
            Text = "KiMaps";
            Resize += KiMapsForm_Resize;
            ((System.ComponentModel.ISupportInitialize)GraphPictureBox).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox GraphPictureBox;
        private Button StartButton;
        private ComboBox FromComboBox;
        private ComboBox ToComboBox;
        private Label label1;
        private Panel panel1;
        private CheckBox FastestWayCheckBox;
        private Button AltButton;
    }
}
