namespace KMeans_Image_Clustering
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
            buttonLoad = new Button();
            buttonStart = new Button();
            numericUpDownK = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)numericUpDownK).BeginInit();
            SuspendLayout();
            // 
            // buttonLoad
            // 
            buttonLoad.Location = new Point(93, 163);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Size = new Size(142, 35);
            buttonLoad.TabIndex = 0;
            buttonLoad.Text = "Load Image";
            buttonLoad.UseVisualStyleBackColor = true;
            // 
            // buttonStart
            // 
            buttonStart.Location = new Point(93, 217);
            buttonStart.Name = "buttonStart";
            buttonStart.Size = new Size(143, 39);
            buttonStart.TabIndex = 1;
            buttonStart.Text = "Start Clustering";
            buttonStart.UseVisualStyleBackColor = true;
            // 
            // numericUpDownK
            // 
            numericUpDownK.Location = new Point(359, 163);
            numericUpDownK.Name = "numericUpDownK";
            numericUpDownK.Size = new Size(150, 27);
            numericUpDownK.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(numericUpDownK);
            Controls.Add(buttonStart);
            Controls.Add(buttonLoad);
            Name = "Form1";
            Text = "KMeans Image Clustering";
            ((System.ComponentModel.ISupportInitialize)numericUpDownK).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button buttonLoad;
        private Button buttonStart;
        private NumericUpDown numericUpDownK;
    }
}