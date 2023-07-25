namespace RDPModule
{
    partial class RDPForm
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
            this.button3 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(797, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(265, 35);
            this.button3.TabIndex = 8;
            this.button3.Text = "Connect client";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(7, 71);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(888, 833);
            this.panel1.TabIndex = 11;
            this.panel1.Visible = false;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(891, 71);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(888, 833);
            this.panel2.TabIndex = 12;
            this.panel2.Visible = false;
            // 
            // RDPForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1817, 961);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.panel1);
            this.Name = "RDPForm";
            this.Text = "Form1";
            this.Enter += new System.EventHandler(this.RDPForm_Enter);
            this.Leave += new System.EventHandler(this.RDPForm_Leave);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button3;
        private AxMSTSCLib.AxMsTscAxNotSafeForScripting rdp2;
        private AxMSTSCLib.AxMsTscAxNotSafeForScripting rdp;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}

