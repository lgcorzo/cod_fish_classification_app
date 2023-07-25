namespace CalibracionModule
{
    partial class CalibracionForm
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
            this.tbVisualizacion = new System.Windows.Forms.TabControl();
            this.tpArea = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxScaleY = new System.Windows.Forms.TextBox();
            this.textBoxScaleX = new System.Windows.Forms.TextBox();
            this.gbCam5 = new System.Windows.Forms.GroupBox();
            this.hWindowControl5 = new HalconDotNet.HWindowControl();
            this.gbCam4 = new System.Windows.Forms.GroupBox();
            this.hWindowControl4 = new HalconDotNet.HWindowControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Act = new System.Windows.Forms.Button();
            this.btn_Forzar = new System.Windows.Forms.Button();
            this.Btn_Correccion = new System.Windows.Forms.Button();
            this.btn_Calibrar = new System.Windows.Forms.Button();
            this.gbArea = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_BlancosArea = new System.Windows.Forms.Button();
            this.btn_MacbechArea = new System.Windows.Forms.Button();
            this.btn_FondoArea = new System.Windows.Forms.Button();
            this.Reset_button = new System.Windows.Forms.Button();
            this.checkBoxsaveimages = new System.Windows.Forms.CheckBox();
            this.buttonActualizar = new System.Windows.Forms.Button();
            this.tbVisualizacion.SuspendLayout();
            this.tpArea.SuspendLayout();
            this.gbCam5.SuspendLayout();
            this.gbCam4.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbArea.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbVisualizacion
            // 
            this.tbVisualizacion.Controls.Add(this.tpArea);
            this.tbVisualizacion.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbVisualizacion.Location = new System.Drawing.Point(12, 42);
            this.tbVisualizacion.Name = "tbVisualizacion";
            this.tbVisualizacion.SelectedIndex = 0;
            this.tbVisualizacion.Size = new System.Drawing.Size(803, 518);
            this.tbVisualizacion.TabIndex = 10;
            // 
            // tpArea
            // 
            this.tpArea.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tpArea.Controls.Add(this.label3);
            this.tpArea.Controls.Add(this.label2);
            this.tpArea.Controls.Add(this.textBoxScaleY);
            this.tpArea.Controls.Add(this.textBoxScaleX);
            this.tpArea.Controls.Add(this.gbCam5);
            this.tpArea.Controls.Add(this.gbCam4);
            this.tpArea.Controls.Add(this.groupBox1);
            this.tpArea.Location = new System.Drawing.Point(4, 32);
            this.tpArea.Name = "tpArea";
            this.tpArea.Padding = new System.Windows.Forms.Padding(3);
            this.tpArea.Size = new System.Drawing.Size(795, 482);
            this.tpArea.TabIndex = 1;
            this.tpArea.Text = "Cameras";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(563, 444);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 23);
            this.label3.TabIndex = 17;
            this.label3.Text = "ScaleY";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(366, 444);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 23);
            this.label2.TabIndex = 16;
            this.label2.Text = "ScaleX";
            // 
            // textBoxScaleY
            // 
            this.textBoxScaleY.Location = new System.Drawing.Point(627, 436);
            this.textBoxScaleY.Name = "textBoxScaleY";
            this.textBoxScaleY.ReadOnly = true;
            this.textBoxScaleY.Size = new System.Drawing.Size(100, 31);
            this.textBoxScaleY.TabIndex = 15;
            // 
            // textBoxScaleX
            // 
            this.textBoxScaleX.Location = new System.Drawing.Point(431, 436);
            this.textBoxScaleX.Name = "textBoxScaleX";
            this.textBoxScaleX.ReadOnly = true;
            this.textBoxScaleX.Size = new System.Drawing.Size(100, 31);
            this.textBoxScaleX.TabIndex = 14;
            // 
            // gbCam5
            // 
            this.gbCam5.Controls.Add(this.hWindowControl5);
            this.gbCam5.Location = new System.Drawing.Point(11, 258);
            this.gbCam5.Name = "gbCam5";
            this.gbCam5.Size = new System.Drawing.Size(317, 222);
            this.gbCam5.TabIndex = 10;
            this.gbCam5.TabStop = false;
            this.gbCam5.Text = "NIR camera";
            // 
            // hWindowControl5
            // 
            this.hWindowControl5.BackColor = System.Drawing.Color.Black;
            this.hWindowControl5.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl5.ImagePart = new System.Drawing.Rectangle(0, 0, 1600, 1200);
            this.hWindowControl5.Location = new System.Drawing.Point(6, 23);
            this.hWindowControl5.Name = "hWindowControl5";
            this.hWindowControl5.Size = new System.Drawing.Size(305, 193);
            this.hWindowControl5.TabIndex = 2;
            this.hWindowControl5.WindowSize = new System.Drawing.Size(305, 193);
            // 
            // gbCam4
            // 
            this.gbCam4.Controls.Add(this.hWindowControl4);
            this.gbCam4.Location = new System.Drawing.Point(11, 19);
            this.gbCam4.Name = "gbCam4";
            this.gbCam4.Size = new System.Drawing.Size(317, 233);
            this.gbCam4.TabIndex = 9;
            this.gbCam4.TabStop = false;
            this.gbCam4.Text = "Color camera";
            // 
            // hWindowControl4
            // 
            this.hWindowControl4.BackColor = System.Drawing.Color.Black;
            this.hWindowControl4.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl4.ImagePart = new System.Drawing.Rectangle(0, 0, 1600, 1200);
            this.hWindowControl4.Location = new System.Drawing.Point(6, 23);
            this.hWindowControl4.Name = "hWindowControl4";
            this.hWindowControl4.Size = new System.Drawing.Size(305, 204);
            this.hWindowControl4.TabIndex = 2;
            this.hWindowControl4.WindowSize = new System.Drawing.Size(305, 204);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.hWindowControl1);
            this.groupBox1.Location = new System.Drawing.Point(347, 83);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(423, 340);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Result color";
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 1600, 1200);
            this.hWindowControl1.Location = new System.Drawing.Point(6, 20);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(411, 314);
            this.hWindowControl1.TabIndex = 3;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(411, 314);
            this.hWindowControl1.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl1_HMouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(195, 29);
            this.label1.TabIndex = 11;
            this.label1.Text = "ACTIVE IMAGES";
            // 
            // btn_Act
            // 
            this.btn_Act.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Act.Location = new System.Drawing.Point(819, 74);
            this.btn_Act.Name = "btn_Act";
            this.btn_Act.Size = new System.Drawing.Size(160, 66);
            this.btn_Act.TabIndex = 12;
            this.btn_Act.Text = "Activate";
            this.btn_Act.UseVisualStyleBackColor = true;
            this.btn_Act.Click += new System.EventHandler(this.btn_Act_Click);
            // 
            // btn_Forzar
            // 
            this.btn_Forzar.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Forzar.Location = new System.Drawing.Point(819, 150);
            this.btn_Forzar.Name = "btn_Forzar";
            this.btn_Forzar.Size = new System.Drawing.Size(160, 66);
            this.btn_Forzar.TabIndex = 13;
            this.btn_Forzar.Text = "Take Image RAW";
            this.btn_Forzar.UseVisualStyleBackColor = true;
            this.btn_Forzar.Click += new System.EventHandler(this.btn_Forzar_Click);
            // 
            // Btn_Correccion
            // 
            this.Btn_Correccion.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Btn_Correccion.Location = new System.Drawing.Point(504, 46);
            this.Btn_Correccion.Name = "Btn_Correccion";
            this.Btn_Correccion.Size = new System.Drawing.Size(160, 66);
            this.Btn_Correccion.TabIndex = 17;
            this.Btn_Correccion.Text = "Lens correction";
            this.Btn_Correccion.UseVisualStyleBackColor = true;
            this.Btn_Correccion.Click += new System.EventHandler(this.Btn_Correccion_Click);
            // 
            // btn_Calibrar
            // 
            this.btn_Calibrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_Calibrar.Location = new System.Drawing.Point(821, 278);
            this.btn_Calibrar.Name = "btn_Calibrar";
            this.btn_Calibrar.Size = new System.Drawing.Size(160, 66);
            this.btn_Calibrar.TabIndex = 20;
            this.btn_Calibrar.Text = "Calibrate";
            this.btn_Calibrar.UseVisualStyleBackColor = true;
            this.btn_Calibrar.Click += new System.EventHandler(this.btn_Calibrar_Click);
            // 
            // gbArea
            // 
            this.gbArea.Controls.Add(this.button1);
            this.gbArea.Controls.Add(this.btn_BlancosArea);
            this.gbArea.Controls.Add(this.btn_MacbechArea);
            this.gbArea.Controls.Add(this.Btn_Correccion);
            this.gbArea.Controls.Add(this.btn_FondoArea);
            this.gbArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbArea.Location = new System.Drawing.Point(12, 575);
            this.gbArea.Name = "gbArea";
            this.gbArea.Size = new System.Drawing.Size(956, 120);
            this.gbArea.TabIndex = 22;
            this.gbArea.TabStop = false;
            this.gbArea.Text = "Cameras configuration parameters";
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(670, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(160, 66);
            this.button1.TabIndex = 18;
            this.button1.Text = "White Pattern";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // btn_BlancosArea
            // 
            this.btn_BlancosArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_BlancosArea.Location = new System.Drawing.Point(6, 46);
            this.btn_BlancosArea.Name = "btn_BlancosArea";
            this.btn_BlancosArea.Size = new System.Drawing.Size(160, 66);
            this.btn_BlancosArea.TabIndex = 14;
            this.btn_BlancosArea.Text = "White";
            this.btn_BlancosArea.UseVisualStyleBackColor = true;
            this.btn_BlancosArea.Click += new System.EventHandler(this.button1_Click);
            // 
            // btn_MacbechArea
            // 
            this.btn_MacbechArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_MacbechArea.Location = new System.Drawing.Point(172, 46);
            this.btn_MacbechArea.Name = "btn_MacbechArea";
            this.btn_MacbechArea.Size = new System.Drawing.Size(160, 66);
            this.btn_MacbechArea.TabIndex = 15;
            this.btn_MacbechArea.Text = "Macbech";
            this.btn_MacbechArea.UseVisualStyleBackColor = true;
            this.btn_MacbechArea.Click += new System.EventHandler(this.btn_MacbechArea_Click);
            // 
            // btn_FondoArea
            // 
            this.btn_FondoArea.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_FondoArea.Location = new System.Drawing.Point(338, 46);
            this.btn_FondoArea.Name = "btn_FondoArea";
            this.btn_FondoArea.Size = new System.Drawing.Size(160, 66);
            this.btn_FondoArea.TabIndex = 16;
            this.btn_FondoArea.Text = "Background";
            this.btn_FondoArea.UseVisualStyleBackColor = true;
            this.btn_FondoArea.Click += new System.EventHandler(this.btn_FondoArea_Click);
            // 
            // Reset_button
            // 
            this.Reset_button.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Reset_button.Location = new System.Drawing.Point(821, 364);
            this.Reset_button.Name = "Reset_button";
            this.Reset_button.Size = new System.Drawing.Size(160, 66);
            this.Reset_button.TabIndex = 24;
            this.Reset_button.Text = "Reset calibration";
            this.Reset_button.UseVisualStyleBackColor = true;
            this.Reset_button.Click += new System.EventHandler(this.Reset_button_Click);
            // 
            // checkBoxsaveimages
            // 
            this.checkBoxsaveimages.AutoSize = true;
            this.checkBoxsaveimages.Checked = true;
            this.checkBoxsaveimages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxsaveimages.Location = new System.Drawing.Point(821, 245);
            this.checkBoxsaveimages.Name = "checkBoxsaveimages";
            this.checkBoxsaveimages.Size = new System.Drawing.Size(87, 17);
            this.checkBoxsaveimages.TabIndex = 25;
            this.checkBoxsaveimages.Text = "Save images";
            this.checkBoxsaveimages.UseVisualStyleBackColor = true;
            // 
            // buttonActualizar
            // 
            this.buttonActualizar.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonActualizar.Location = new System.Drawing.Point(821, 447);
            this.buttonActualizar.Name = "buttonActualizar";
            this.buttonActualizar.Size = new System.Drawing.Size(160, 66);
            this.buttonActualizar.TabIndex = 26;
            this.buttonActualizar.Text = "Actualize calibration";
            this.buttonActualizar.UseVisualStyleBackColor = true;
            this.buttonActualizar.Click += new System.EventHandler(this.buttonActualizar_Click);
            // 
            // CalibracionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1186, 707);
            this.Controls.Add(this.buttonActualizar);
            this.Controls.Add(this.checkBoxsaveimages);
            this.Controls.Add(this.Reset_button);
            this.Controls.Add(this.gbArea);
            this.Controls.Add(this.btn_Calibrar);
            this.Controls.Add(this.btn_Forzar);
            this.Controls.Add(this.btn_Act);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbVisualizacion);
            this.Name = "CalibracionForm";
            this.Text = "Form1";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CalibracionForm_Paint);
            this.tbVisualizacion.ResumeLayout(false);
            this.tpArea.ResumeLayout(false);
            this.tpArea.PerformLayout();
            this.gbCam5.ResumeLayout(false);
            this.gbCam4.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.gbArea.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
      
        private System.Windows.Forms.TabControl tbVisualizacion;
        private System.Windows.Forms.TabPage tpArea;
        private System.Windows.Forms.GroupBox gbCam5;
        private HalconDotNet.HWindowControl hWindowControl5;
        private System.Windows.Forms.GroupBox gbCam4;
        private HalconDotNet.HWindowControl hWindowControl4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Act;
        private System.Windows.Forms.Button btn_Forzar;
        private System.Windows.Forms.Button Btn_Correccion;
        private System.Windows.Forms.Button btn_Calibrar;
        private System.Windows.Forms.GroupBox gbArea;
        private System.Windows.Forms.Button btn_BlancosArea;
        private System.Windows.Forms.Button btn_MacbechArea;
        private System.Windows.Forms.Button btn_FondoArea;
        private System.Windows.Forms.Button Reset_button;
        private HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.CheckBox checkBoxsaveimages;
        private System.Windows.Forms.Button buttonActualizar;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxScaleY;
        private System.Windows.Forms.TextBox textBoxScaleX;
        private System.Windows.Forms.Button button1;
    }
}

