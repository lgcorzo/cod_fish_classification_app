namespace CSRApp.forms
{
    partial class Carga
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lStatusInfo = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::CSRApp.Properties.Resources.LOGO_ROBOCONCEPT;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(703, 184);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // lStatusInfo
            // 
            this.lStatusInfo.AutoSize = true;
            this.lStatusInfo.BackColor = System.Drawing.Color.WhiteSmoke;
            this.lStatusInfo.Location = new System.Drawing.Point(303, 133);
            this.lStatusInfo.Name = "lStatusInfo";
            this.lStatusInfo.Size = new System.Drawing.Size(104, 13);
            this.lStatusInfo.TabIndex = 1;
            this.lStatusInfo.Text = "Loading, please wait";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(127, 149);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(458, 23);
            this.progressBar.TabIndex = 2;
            // 
            // Carga
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(703, 184);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lStatusInfo);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Carga";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Carga";
            this.TransparencyKey = System.Drawing.Color.Gray;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lStatusInfo;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}