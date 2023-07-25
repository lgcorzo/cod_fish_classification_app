namespace TrainingModule
{
    partial class ManualselectionFrom
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
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.button_salir = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 1600, 1200);
            this.hWindowControl1.Location = new System.Drawing.Point(12, 22);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(597, 418);
            this.hWindowControl1.TabIndex = 4;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(597, 418);
            // 
            // button_salir
            // 
            this.button_salir.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_salir.Location = new System.Drawing.Point(682, 402);
            this.button_salir.Name = "button_salir";
            this.button_salir.Size = new System.Drawing.Size(72, 38);
            this.button_salir.TabIndex = 18;
            this.button_salir.Text = "OK";
            this.button_salir.UseVisualStyleBackColor = true;
            this.button_salir.Click += new System.EventHandler(this.button_salir_Click);
            // 
            // ManualselectionFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(766, 452);
            this.Controls.Add(this.button_salir);
            this.Controls.Add(this.hWindowControl1);
            this.Name = "ManualselectionFrom";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.Button button_salir;
    }
}