namespace CalibracionModule
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
            this.btn_UL = new System.Windows.Forms.Button();
            this.btn_UR = new System.Windows.Forms.Button();
            this.btn_DL = new System.Windows.Forms.Button();
            this.btn_DR = new System.Windows.Forms.Button();
            this.button_calibrar = new System.Windows.Forms.Button();
            this.button_salir = new System.Windows.Forms.Button();
            this.checkBoxCorrection = new System.Windows.Forms.CheckBox();
            this.numericUpDownAngle = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAngle)).BeginInit();
            this.SuspendLayout();
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 1600, 1200);
            this.hWindowControl1.Location = new System.Drawing.Point(12, 22);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(508, 418);
            this.hWindowControl1.TabIndex = 4;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(508, 418);
            this.hWindowControl1.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl1_HMouseDown);
            // 
            // btn_UL
            // 
            this.btn_UL.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_UL.Location = new System.Drawing.Point(526, 22);
            this.btn_UL.Name = "btn_UL";
            this.btn_UL.Size = new System.Drawing.Size(97, 38);
            this.btn_UL.TabIndex = 13;
            this.btn_UL.Text = "Brown";
            this.btn_UL.UseVisualStyleBackColor = true;
            this.btn_UL.Click += new System.EventHandler(this.btn_UL_Click);
            // 
            // btn_UR
            // 
            this.btn_UR.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_UR.Location = new System.Drawing.Point(526, 66);
            this.btn_UR.Name = "btn_UR";
            this.btn_UR.Size = new System.Drawing.Size(97, 38);
            this.btn_UR.TabIndex = 14;
            this.btn_UR.Text = "Blue";
            this.btn_UR.UseVisualStyleBackColor = true;
            this.btn_UR.Click += new System.EventHandler(this.btn_UR_Click);
            // 
            // btn_DL
            // 
            this.btn_DL.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_DL.Location = new System.Drawing.Point(526, 110);
            this.btn_DL.Name = "btn_DL";
            this.btn_DL.Size = new System.Drawing.Size(97, 38);
            this.btn_DL.TabIndex = 15;
            this.btn_DL.Text = "Black";
            this.btn_DL.UseVisualStyleBackColor = true;
            this.btn_DL.Click += new System.EventHandler(this.btn_DL_Click);
            // 
            // btn_DR
            // 
            this.btn_DR.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_DR.Location = new System.Drawing.Point(526, 154);
            this.btn_DR.Name = "btn_DR";
            this.btn_DR.Size = new System.Drawing.Size(97, 38);
            this.btn_DR.TabIndex = 16;
            this.btn_DR.Text = "White";
            this.btn_DR.UseVisualStyleBackColor = true;
            this.btn_DR.Click += new System.EventHandler(this.btn_DR_Click);
            // 
            // button_calibrar
            // 
            this.button_calibrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_calibrar.Location = new System.Drawing.Point(551, 358);
            this.button_calibrar.Name = "button_calibrar";
            this.button_calibrar.Size = new System.Drawing.Size(72, 38);
            this.button_calibrar.TabIndex = 17;
            this.button_calibrar.Text = "Calibrar";
            this.button_calibrar.UseVisualStyleBackColor = true;
            this.button_calibrar.Click += new System.EventHandler(this.button_Calibrar_Click);
            // 
            // button_salir
            // 
            this.button_salir.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_salir.Location = new System.Drawing.Point(551, 402);
            this.button_salir.Name = "button_salir";
            this.button_salir.Size = new System.Drawing.Size(72, 38);
            this.button_salir.TabIndex = 18;
            this.button_salir.Text = "Ok";
            this.button_salir.UseVisualStyleBackColor = true;
            this.button_salir.Click += new System.EventHandler(this.button_salir_Click);
            // 
            // checkBoxCorrection
            // 
            this.checkBoxCorrection.AutoSize = true;
            this.checkBoxCorrection.Checked = true;
            this.checkBoxCorrection.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCorrection.Location = new System.Drawing.Point(533, 207);
            this.checkBoxCorrection.Name = "checkBoxCorrection";
            this.checkBoxCorrection.Size = new System.Drawing.Size(72, 17);
            this.checkBoxCorrection.TabIndex = 26;
            this.checkBoxCorrection.Text = "Corrected";
            this.checkBoxCorrection.UseVisualStyleBackColor = true;
            this.checkBoxCorrection.CheckedChanged += new System.EventHandler(this.checkBoxCorrection_CheckedChanged);
            // 
            // numericUpDownAngle
            // 
            this.numericUpDownAngle.Location = new System.Drawing.Point(533, 260);
            this.numericUpDownAngle.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.numericUpDownAngle.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.numericUpDownAngle.Name = "numericUpDownAngle";
            this.numericUpDownAngle.Size = new System.Drawing.Size(81, 20);
            this.numericUpDownAngle.TabIndex = 27;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(530, 244);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 28;
            this.label1.Text = "Belt angle";
            // 
            // ManualselectionFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(635, 452);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownAngle);
            this.Controls.Add(this.checkBoxCorrection);
            this.Controls.Add(this.button_salir);
            this.Controls.Add(this.button_calibrar);
            this.Controls.Add(this.btn_DR);
            this.Controls.Add(this.btn_DL);
            this.Controls.Add(this.btn_UR);
            this.Controls.Add(this.btn_UL);
            this.Controls.Add(this.hWindowControl1);
            this.Name = "ManualselectionFrom";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownAngle)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.Button btn_UL;
        private System.Windows.Forms.Button btn_UR;
        private System.Windows.Forms.Button btn_DL;
        private System.Windows.Forms.Button btn_DR;
        private System.Windows.Forms.Button button_calibrar;
        private System.Windows.Forms.Button button_salir;
        private System.Windows.Forms.CheckBox checkBoxCorrection;
        private System.Windows.Forms.NumericUpDown numericUpDownAngle;
        private System.Windows.Forms.Label label1;
    }
}