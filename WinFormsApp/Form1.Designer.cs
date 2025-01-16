namespace WinFormsApp3
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
            radioButtonC = new RadioButton();
            radioButtonAsm = new RadioButton();
            processImageButton = new Button();
            selectImageButton = new Button();
            threadsNumberTextBox = new TextBox();
            label1 = new Label();
            label3 = new Label();
            processingTimeLabel = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            originalImagePictureBox = new PictureBox();
            processedImagePictureBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)originalImagePictureBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)processedImagePictureBox).BeginInit();
            SuspendLayout();
            // 
            // radioButtonC
            // 
            radioButtonC.AutoSize = true;
            radioButtonC.Location = new Point(27, 290);
            radioButtonC.Name = "radioButtonC";
            radioButtonC.Size = new Size(39, 24);
            radioButtonC.TabIndex = 0;
            radioButtonC.TabStop = true;
            radioButtonC.Text = "C";
            radioButtonC.UseVisualStyleBackColor = true;
            radioButtonC.CheckedChanged += radioButtonC_CheckedChanged;
            // 
            // radioButtonAsm
            // 
            radioButtonAsm.AutoSize = true;
            radioButtonAsm.Location = new Point(27, 336);
            radioButtonAsm.Name = "radioButtonAsm";
            radioButtonAsm.Size = new Size(61, 24);
            radioButtonAsm.TabIndex = 1;
            radioButtonAsm.TabStop = true;
            radioButtonAsm.Text = "ASM";
            radioButtonAsm.UseVisualStyleBackColor = true;
            radioButtonAsm.CheckedChanged += radioButtonAsm_CheckedChanged;
            // 
            // processImageButton
            // 
            processImageButton.Location = new Point(496, 485);
            processImageButton.Name = "processImageButton";
            processImageButton.Size = new Size(140, 29);
            processImageButton.TabIndex = 2;
            processImageButton.Text = "Process Image";
            processImageButton.UseVisualStyleBackColor = true;
            processImageButton.Click += processImageButton_Click;
            // 
            // selectImageButton
            // 
            selectImageButton.Location = new Point(50, 58);
            selectImageButton.Name = "selectImageButton";
            selectImageButton.Size = new Size(157, 29);
            selectImageButton.TabIndex = 3;
            selectImageButton.Text = "Select Image File";
            selectImageButton.UseVisualStyleBackColor = true;
            selectImageButton.Click += selectImageButton_Click;
            // 
            // threadsNumberTextBox
            // 
            threadsNumberTextBox.Location = new Point(51, 530);
            threadsNumberTextBox.Name = "threadsNumberTextBox";
            threadsNumberTextBox.Size = new Size(125, 27);
            threadsNumberTextBox.TabIndex = 4;
            threadsNumberTextBox.TextChanged += threadsNumberTextBox_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(50, 494);
            label1.Name = "label1";
            label1.Size = new Size(157, 20);
            label1.TabIndex = 5;
            label1.Text = "Enter Threads Number";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(50, 247);
            label3.Name = "label3";
            label3.Size = new Size(200, 20);
            label3.TabIndex = 7;
            label3.Text = "Select Implemetation Library";
            // 
            // processingTimeLabel
            // 
            processingTimeLabel.AutoSize = true;
            processingTimeLabel.Location = new Point(51, 836);
            processingTimeLabel.Name = "processingTimeLabel";
            processingTimeLabel.Size = new Size(15, 20);
            processingTimeLabel.TabIndex = 8;
            processingTimeLabel.Text = "-";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(50, 784);
            label5.Name = "label5";
            label5.Size = new Size(116, 20);
            label5.TabIndex = 9;
            label5.Text = "Processing Time";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(496, 18);
            label6.Name = "label6";
            label6.Size = new Size(51, 20);
            label6.TabIndex = 10;
            label6.Text = "Image";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(496, 530);
            label7.Name = "label7";
            label7.Size = new Size(121, 20);
            label7.TabIndex = 11;
            label7.Text = "Processed Image";
            // 
            // originalImagePictureBox
            // 
            originalImagePictureBox.Location = new Point(496, 41);
            originalImagePictureBox.Name = "originalImagePictureBox";
            originalImagePictureBox.Size = new Size(746, 371);
            originalImagePictureBox.TabIndex = 12;
            originalImagePictureBox.TabStop = false;
            // 
            // processedImagePictureBox
            // 
            processedImagePictureBox.Location = new Point(496, 558);
            processedImagePictureBox.Name = "processedImagePictureBox";
            processedImagePictureBox.Size = new Size(746, 371);
            processedImagePictureBox.TabIndex = 13;
            processedImagePictureBox.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1359, 947);
            Controls.Add(processedImagePictureBox);
            Controls.Add(originalImagePictureBox);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(processingTimeLabel);
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(threadsNumberTextBox);
            Controls.Add(selectImageButton);
            Controls.Add(processImageButton);
            Controls.Add(radioButtonAsm);
            Controls.Add(radioButtonC);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)originalImagePictureBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)processedImagePictureBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RadioButton radioButtonC;
        private RadioButton radioButtonAsm;
        private Button processImageButton;
        private Button selectImageButton;
        private TextBox threadsNumberTextBox;
        private Label label1;
        private Label label3;
        private Label processingTimeLabel;
        private Label label5;
        private Label label6;
        private Label label7;
        private PictureBox originalImagePictureBox;
        private PictureBox processedImagePictureBox;
    }
}
