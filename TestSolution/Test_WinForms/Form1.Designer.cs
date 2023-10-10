namespace Test_WinForms
{
    partial class Form1
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
            textBox = new TextBox();
            clearButton = new Button();
            SuspendLayout();
            // 
            // textBox
            // 
            textBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox.BackColor = SystemColors.Window;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            textBox.Location = new Point(21, 21);
            textBox.MaxLength = 10240;
            textBox.Multiline = true;
            textBox.Name = "textBox";
            textBox.ReadOnly = true;
            textBox.ScrollBars = ScrollBars.Both;
            textBox.Size = new Size(641, 417);
            textBox.TabIndex = 0;
            // 
            // clearButton
            // 
            clearButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            clearButton.Location = new Point(682, 393);
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(94, 29);
            clearButton.TabIndex = 1;
            clearButton.Text = "Clear";
            clearButton.UseVisualStyleBackColor = true;
            clearButton.Click += ClearButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(clearButton);
            Controls.Add(textBox);
            Name = "Form1";
            Text = "WinMsg Test_WinForms";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox;
        private Button clearButton;
    }
}