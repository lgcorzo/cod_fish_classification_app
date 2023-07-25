namespace TrainingModule
{
    partial class TrainingForm
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
            this.gbVisualizacion = new System.Windows.Forms.GroupBox();
            this.Model_gen = new System.Windows.Forms.Button();
            this.textBox_output = new System.Windows.Forms.TextBox();
            this.button_reset = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblContador = new System.Windows.Forms.Label();
            this.gbFromFile = new System.Windows.Forms.GroupBox();
            this.checkBoxVisualizar = new System.Windows.Forms.CheckBox();
            this.checkBoxCargaAut = new System.Windows.Forms.CheckBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnCargar = new System.Windows.Forms.Button();
            this.lblPath = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Propiedades = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.numericUpDowListaPropiedades = new System.Windows.Forms.NumericUpDown();
            this.cb_SaveImage = new System.Windows.Forms.CheckBox();
            this.comboBox_Modelo = new System.Windows.Forms.ComboBox();
            this.gbCam1 = new System.Windows.Forms.GroupBox();
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.gbCam2 = new System.Windows.Forms.GroupBox();
            this.hWindowControl2 = new HalconDotNet.HWindowControl();
            this.Category_assignation_active = new System.Windows.Forms.CheckBox();
            this.comboCategoria = new System.Windows.Forms.ComboBox();
            this.buttonInstancia = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_borrar = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.hWindowControl8 = new HalconDotNet.HWindowControl();
            this.gbVisualizacion.SuspendLayout();
            this.gbFromFile.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.Propiedades.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDowListaPropiedades)).BeginInit();
            this.gbCam1.SuspendLayout();
            this.gbCam2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbVisualizacion
            // 
            this.gbVisualizacion.Controls.Add(this.hWindowControl8);
            this.gbVisualizacion.Controls.Add(this.Model_gen);
            this.gbVisualizacion.Controls.Add(this.textBox_output);
            this.gbVisualizacion.Controls.Add(this.button_reset);
            this.gbVisualizacion.Controls.Add(this.label2);
            this.gbVisualizacion.Controls.Add(this.lblContador);
            this.gbVisualizacion.Controls.Add(this.gbFromFile);
            this.gbVisualizacion.Controls.Add(this.tabControl1);
            this.gbVisualizacion.Controls.Add(this.cb_SaveImage);
            this.gbVisualizacion.Controls.Add(this.comboBox_Modelo);
            this.gbVisualizacion.Controls.Add(this.gbCam1);
            this.gbVisualizacion.Controls.Add(this.gbCam2);
            this.gbVisualizacion.Controls.Add(this.Category_assignation_active);
            this.gbVisualizacion.Controls.Add(this.comboCategoria);
            this.gbVisualizacion.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbVisualizacion.Location = new System.Drawing.Point(1, 0);
            this.gbVisualizacion.Name = "gbVisualizacion";
            this.gbVisualizacion.Size = new System.Drawing.Size(1011, 771);
            this.gbVisualizacion.TabIndex = 9;
            this.gbVisualizacion.TabStop = false;
            this.gbVisualizacion.Paint += new System.Windows.Forms.PaintEventHandler(this.gbVisualizacion_Paint);
            // 
            // Model_gen
            // 
            this.Model_gen.Location = new System.Drawing.Point(894, 682);
            this.Model_gen.Name = "Model_gen";
            this.Model_gen.Size = new System.Drawing.Size(101, 71);
            this.Model_gen.TabIndex = 24;
            this.Model_gen.Text = "Generate Model";
            this.Model_gen.UseVisualStyleBackColor = true;
            this.Model_gen.Click += new System.EventHandler(this.Model_gen_Click);
            // 
            // textBox_output
            // 
            this.textBox_output.Location = new System.Drawing.Point(11, 632);
            this.textBox_output.Name = "textBox_output";
            this.textBox_output.Size = new System.Drawing.Size(287, 31);
            this.textBox_output.TabIndex = 14;
            // 
            // button_reset
            // 
            this.button_reset.Location = new System.Drawing.Point(6, 16);
            this.button_reset.Name = "button_reset";
            this.button_reset.Size = new System.Drawing.Size(33, 28);
            this.button_reset.TabIndex = 13;
            this.button_reset.Text = "R";
            this.button_reset.UseVisualStyleBackColor = true;
            this.button_reset.Click += new System.EventHandler(this.button_reset_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 71);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 23);
            this.label2.TabIndex = 11;
            this.label2.Text = "Num_images";
            // 
            // lblContador
            // 
            this.lblContador.AutoSize = true;
            this.lblContador.Location = new System.Drawing.Point(128, 71);
            this.lblContador.Name = "lblContador";
            this.lblContador.Size = new System.Drawing.Size(20, 23);
            this.lblContador.TabIndex = 10;
            this.lblContador.Text = "0";
            // 
            // gbFromFile
            // 
            this.gbFromFile.Controls.Add(this.checkBoxVisualizar);
            this.gbFromFile.Controls.Add(this.checkBoxCargaAut);
            this.gbFromFile.Controls.Add(this.btnExecute);
            this.gbFromFile.Controls.Add(this.btnCargar);
            this.gbFromFile.Controls.Add(this.lblPath);
            this.gbFromFile.Controls.Add(this.label1);
            this.gbFromFile.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFromFile.Location = new System.Drawing.Point(195, 16);
            this.gbFromFile.Name = "gbFromFile";
            this.gbFromFile.Size = new System.Drawing.Size(575, 85);
            this.gbFromFile.TabIndex = 9;
            this.gbFromFile.TabStop = false;
            this.gbFromFile.Text = "Load from folder";
            // 
            // checkBoxVisualizar
            // 
            this.checkBoxVisualizar.AutoSize = true;
            this.checkBoxVisualizar.Checked = true;
            this.checkBoxVisualizar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxVisualizar.Location = new System.Drawing.Point(251, 31);
            this.checkBoxVisualizar.Name = "checkBoxVisualizar";
            this.checkBoxVisualizar.Size = new System.Drawing.Size(87, 23);
            this.checkBoxVisualizar.TabIndex = 13;
            this.checkBoxVisualizar.Text = "Visualice";
            this.checkBoxVisualizar.UseVisualStyleBackColor = true;
            this.checkBoxVisualizar.CheckedChanged += new System.EventHandler(this.cb_SaveImage_CheckedChanged);
            // 
            // checkBoxCargaAut
            // 
            this.checkBoxCargaAut.AutoSize = true;
            this.checkBoxCargaAut.Checked = true;
            this.checkBoxCargaAut.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxCargaAut.Location = new System.Drawing.Point(251, 14);
            this.checkBoxCargaAut.Name = "checkBoxCargaAut";
            this.checkBoxCargaAut.Size = new System.Drawing.Size(126, 23);
            this.checkBoxCargaAut.TabIndex = 12;
            this.checkBoxCargaAut.Text = "Automatic load";
            this.checkBoxCargaAut.UseVisualStyleBackColor = true;
            this.checkBoxCargaAut.CheckedChanged += new System.EventHandler(this.cb_SaveImage_CheckedChanged);
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(494, 14);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 40);
            this.btnExecute.TabIndex = 3;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnCargar
            // 
            this.btnCargar.Location = new System.Drawing.Point(416, 14);
            this.btnCargar.Name = "btnCargar";
            this.btnCargar.Size = new System.Drawing.Size(75, 40);
            this.btnCargar.TabIndex = 2;
            this.btnCargar.Text = "Load";
            this.btnCargar.UseVisualStyleBackColor = true;
            this.btnCargar.Click += new System.EventHandler(this.btnCargar_Click);
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(62, 59);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(43, 19);
            this.lblPath.TabIndex = 1;
            this.lblPath.Text = "None";
            this.lblPath.UseWaitCursor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "PATH: ";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Propiedades);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.tabControl1.Location = new System.Drawing.Point(491, 177);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(388, 594);
            this.tabControl1.TabIndex = 18;
            // 
            // Propiedades
            // 
            this.Propiedades.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Propiedades.Controls.Add(this.dataGridView1);
            this.Propiedades.Controls.Add(this.numericUpDowListaPropiedades);
            this.Propiedades.Location = new System.Drawing.Point(4, 22);
            this.Propiedades.Name = "Propiedades";
            this.Propiedades.Padding = new System.Windows.Forms.Padding(3);
            this.Propiedades.Size = new System.Drawing.Size(380, 568);
            this.Propiedades.TabIndex = 0;
            this.Propiedades.Text = "Propierties";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(360, 530);
            this.dataGridView1.TabIndex = 10;
            // 
            // numericUpDowListaPropiedades
            // 
            this.numericUpDowListaPropiedades.Location = new System.Drawing.Point(3, 542);
            this.numericUpDowListaPropiedades.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDowListaPropiedades.Name = "numericUpDowListaPropiedades";
            this.numericUpDowListaPropiedades.Size = new System.Drawing.Size(69, 20);
            this.numericUpDowListaPropiedades.TabIndex = 13;
            this.numericUpDowListaPropiedades.ValueChanged += new System.EventHandler(this.numbreList_Changed);
            // 
            // cb_SaveImage
            // 
            this.cb_SaveImage.AutoSize = true;
            this.cb_SaveImage.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_SaveImage.Location = new System.Drawing.Point(16, 47);
            this.cb_SaveImage.Name = "cb_SaveImage";
            this.cb_SaveImage.Size = new System.Drawing.Size(124, 23);
            this.cb_SaveImage.TabIndex = 8;
            this.cb_SaveImage.Text = "Save instances";
            this.cb_SaveImage.UseVisualStyleBackColor = true;
            this.cb_SaveImage.CheckedChanged += new System.EventHandler(this.cb_SaveImage_CheckedChanged);
            // 
            // comboBox_Modelo
            // 
            this.comboBox_Modelo.FormattingEnabled = true;
            this.comboBox_Modelo.Location = new System.Drawing.Point(647, 107);
            this.comboBox_Modelo.Name = "comboBox_Modelo";
            this.comboBox_Modelo.Size = new System.Drawing.Size(121, 31);
            this.comboBox_Modelo.TabIndex = 15;
            this.comboBox_Modelo.SelectedIndexChanged += new System.EventHandler(this.Cargar_modelo_Click);
            // 
            // gbCam1
            // 
            this.gbCam1.Controls.Add(this.hWindowControl1);
            this.gbCam1.Controls.Add(this.groupBox8);
            this.gbCam1.Controls.Add(this.groupBox7);
            this.gbCam1.Location = new System.Drawing.Point(6, 107);
            this.gbCam1.Name = "gbCam1";
            this.gbCam1.Size = new System.Drawing.Size(227, 506);
            this.gbCam1.TabIndex = 0;
            this.gbCam1.TabStop = false;
            this.gbCam1.Text = "Cámara 1";
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(8, 25);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(213, 475);
            this.hWindowControl1.TabIndex = 4;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(213, 475);
            this.hWindowControl1.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl1_HMouseDown);
            // 
            // groupBox8
            // 
            this.groupBox8.Location = new System.Drawing.Point(532, 0);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(260, 370);
            this.groupBox8.TabIndex = 3;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Cámara 3";
            // 
            // groupBox7
            // 
            this.groupBox7.Location = new System.Drawing.Point(266, 0);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(260, 370);
            this.groupBox7.TabIndex = 3;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Cámara 2";
            // 
            // gbCam2
            // 
            this.gbCam2.Controls.Add(this.hWindowControl2);
            this.gbCam2.Location = new System.Drawing.Point(239, 107);
            this.gbCam2.Name = "gbCam2";
            this.gbCam2.Size = new System.Drawing.Size(234, 506);
            this.gbCam2.TabIndex = 3;
            this.gbCam2.TabStop = false;
            this.gbCam2.Text = "Cámara 2";
            // 
            // hWindowControl2
            // 
            this.hWindowControl2.BackColor = System.Drawing.Color.Black;
            this.hWindowControl2.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl2.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl2.Location = new System.Drawing.Point(8, 25);
            this.hWindowControl2.Name = "hWindowControl2";
            this.hWindowControl2.Size = new System.Drawing.Size(213, 475);
            this.hWindowControl2.TabIndex = 5;
            this.hWindowControl2.WindowSize = new System.Drawing.Size(213, 475);
            this.hWindowControl2.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl2_HMouseDown);
            // 
            // Category_assignation_active
            // 
            this.Category_assignation_active.AutoSize = true;
            this.Category_assignation_active.Location = new System.Drawing.Point(504, 144);
            this.Category_assignation_active.Name = "Category_assignation_active";
            this.Category_assignation_active.Size = new System.Drawing.Size(166, 27);
            this.Category_assignation_active.TabIndex = 11;
            this.Category_assignation_active.Text = "Force Assignment";
            this.Category_assignation_active.UseVisualStyleBackColor = true;
            this.Category_assignation_active.CheckedChanged += new System.EventHandler(this.cb_SaveImage_CheckedChanged);
            // 
            // comboCategoria
            // 
            this.comboCategoria.FormattingEnabled = true;
            this.comboCategoria.Location = new System.Drawing.Point(504, 107);
            this.comboCategoria.Name = "comboCategoria";
            this.comboCategoria.Size = new System.Drawing.Size(114, 31);
            this.comboCategoria.TabIndex = 10;
            this.comboCategoria.SelectedIndexChanged += new System.EventHandler(this.cb_SaveImage_CheckedChanged);
            // 
            // buttonInstancia
            // 
            this.buttonInstancia.Location = new System.Drawing.Point(895, 16);
            this.buttonInstancia.Name = "buttonInstancia";
            this.buttonInstancia.Size = new System.Drawing.Size(101, 40);
            this.buttonInstancia.TabIndex = 14;
            this.buttonInstancia.Text = "Change assignment";
            this.buttonInstancia.UseVisualStyleBackColor = true;
            this.buttonInstancia.Click += new System.EventHandler(this.buttonInstancia_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 715);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 40);
            this.button1.TabIndex = 16;
            this.button1.Text = "Save all";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AllowUserToResizeColumns = false;
            this.dataGridView2.AllowUserToResizeRows = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(6, 19);
            this.dataGridView2.MultiSelect = false;
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView2.Size = new System.Drawing.Size(373, 681);
            this.dataGridView2.TabIndex = 20;
            this.dataGridView2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView2_MouseDoubleClick);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(895, 62);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(101, 40);
            this.button2.TabIndex = 21;
            this.button2.Text = "Delete instance";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button_eliminar_Instancia_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_borrar);
            this.groupBox1.Controls.Add(this.dataGridView2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(1015, -2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(376, 773);
            this.groupBox1.TabIndex = 22;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Instances";
            // 
            // button_borrar
            // 
            this.button_borrar.Location = new System.Drawing.Point(275, 715);
            this.button_borrar.Name = "button_borrar";
            this.button_borrar.Size = new System.Drawing.Size(101, 40);
            this.button_borrar.TabIndex = 23;
            this.button_borrar.Text = "Delete all";
            this.button_borrar.UseVisualStyleBackColor = true;
            this.button_borrar.Click += new System.EventHandler(this.button_borrar_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(895, 182);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(101, 40);
            this.button3.TabIndex = 23;
            this.button3.Text = "Force all";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(895, 111);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(101, 53);
            this.button4.TabIndex = 24;
            this.button4.Text = "Delete Unknwom";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // hWindowControl8
            // 
            this.hWindowControl8.BackColor = System.Drawing.Color.Black;
            this.hWindowControl8.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl8.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl8.Location = new System.Drawing.Point(11, 682);
            this.hWindowControl8.Name = "hWindowControl8";
            this.hWindowControl8.Size = new System.Drawing.Size(467, 78);
            this.hWindowControl8.TabIndex = 25;
            this.hWindowControl8.WindowSize = new System.Drawing.Size(467, 78);
            // 
            // TrainingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1425, 813);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonInstancia);
            this.Controls.Add(this.gbVisualizacion);
            this.Name = "TrainingForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.VisionForm_Load);
            this.gbVisualizacion.ResumeLayout(false);
            this.gbVisualizacion.PerformLayout();
            this.gbFromFile.ResumeLayout(false);
            this.gbFromFile.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.Propiedades.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDowListaPropiedades)).EndInit();
            this.gbCam1.ResumeLayout(false);
            this.gbCam2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbVisualizacion;
        private System.Windows.Forms.GroupBox gbCam1;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.GroupBox gbCam2;
        private HalconDotNet.HWindowControl hWindowControl1;
        private HalconDotNet.HWindowControl hWindowControl2;
        private System.Windows.Forms.CheckBox cb_SaveImage;
        private System.Windows.Forms.GroupBox gbFromFile;
        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.Button btnCargar;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboCategoria;
        private System.Windows.Forms.CheckBox Category_assignation_active;
        private System.Windows.Forms.Label lblContador;
        private System.Windows.Forms.Button button_reset;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_output;
        private System.Windows.Forms.Button buttonInstancia;
        private System.Windows.Forms.CheckBox checkBoxCargaAut;
        private System.Windows.Forms.CheckBox checkBoxVisualizar;
        private System.Windows.Forms.ComboBox comboBox_Modelo;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_borrar;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TabPage Propiedades;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.NumericUpDown numericUpDowListaPropiedades;
        private System.Windows.Forms.Button Model_gen;
        private HalconDotNet.HWindowControl hWindowControl8;
    }
}

