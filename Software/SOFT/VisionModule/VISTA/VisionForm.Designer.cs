namespace VisionModule
{
    partial class VisionForm
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
            this.button_reset = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lblContador = new System.Windows.Forms.Label();
            this.gbFromFile = new System.Windows.Forms.GroupBox();
            this.checkBoxApplyCalibrationFolder = new System.Windows.Forms.CheckBox();
            this.checkBoxVisualizar = new System.Windows.Forms.CheckBox();
            this.checkBoxCargaAut = new System.Windows.Forms.CheckBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.btnCargar = new System.Windows.Forms.Button();
            this.lblPath = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_SaveImage = new System.Windows.Forms.CheckBox();
            this.gbCam1 = new System.Windows.Forms.GroupBox();
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.gbCam2 = new System.Windows.Forms.GroupBox();
            this.hWindowControl2 = new HalconDotNet.HWindowControl();
            this.textBox_output = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.comboCategoria = new System.Windows.Forms.ComboBox();
            this.Category_assignation_active = new System.Windows.Forms.CheckBox();
            this.numericUpDowListaPropiedades = new System.Windows.Forms.NumericUpDown();
            this.comboBox_Modelo = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Propiedades = new System.Windows.Forms.TabPage();
            this.gbVisualizacion.SuspendLayout();
            this.gbFromFile.SuspendLayout();
            this.gbCam1.SuspendLayout();
            this.gbCam2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDowListaPropiedades)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.Propiedades.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbVisualizacion
            // 
            this.gbVisualizacion.Controls.Add(this.button_reset);
            this.gbVisualizacion.Controls.Add(this.label2);
            this.gbVisualizacion.Controls.Add(this.lblContador);
            this.gbVisualizacion.Controls.Add(this.gbFromFile);
            this.gbVisualizacion.Controls.Add(this.cb_SaveImage);
            this.gbVisualizacion.Controls.Add(this.gbCam1);
            this.gbVisualizacion.Controls.Add(this.gbCam2);
            this.gbVisualizacion.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbVisualizacion.Location = new System.Drawing.Point(1, 0);
            this.gbVisualizacion.Name = "gbVisualizacion";
            this.gbVisualizacion.Size = new System.Drawing.Size(803, 1011);
            this.gbVisualizacion.TabIndex = 9;
            this.gbVisualizacion.TabStop = false;
            this.gbVisualizacion.Enter += new System.EventHandler(this.gbVisualizacion_Enter);
            // 
            // button_reset
            // 
            this.button_reset.Location = new System.Drawing.Point(11, 16);
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
            this.gbFromFile.Controls.Add(this.checkBoxApplyCalibrationFolder);
            this.gbFromFile.Controls.Add(this.checkBoxVisualizar);
            this.gbFromFile.Controls.Add(this.checkBoxCargaAut);
            this.gbFromFile.Controls.Add(this.btnExecute);
            this.gbFromFile.Controls.Add(this.btnCargar);
            this.gbFromFile.Controls.Add(this.lblPath);
            this.gbFromFile.Controls.Add(this.label1);
            this.gbFromFile.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFromFile.Location = new System.Drawing.Point(212, 16);
            this.gbFromFile.Name = "gbFromFile";
            this.gbFromFile.Size = new System.Drawing.Size(575, 85);
            this.gbFromFile.TabIndex = 9;
            this.gbFromFile.TabStop = false;
            this.gbFromFile.Text = "Load from folder";
            // 
            // checkBoxApplyCalibrationFolder
            // 
            this.checkBoxApplyCalibrationFolder.AutoSize = true;
            this.checkBoxApplyCalibrationFolder.Location = new System.Drawing.Point(13, 31);
            this.checkBoxApplyCalibrationFolder.Name = "checkBoxApplyCalibrationFolder";
            this.checkBoxApplyCalibrationFolder.Size = new System.Drawing.Size(137, 23);
            this.checkBoxApplyCalibrationFolder.TabIndex = 14;
            this.checkBoxApplyCalibrationFolder.Text = "Apply calibration";
            this.checkBoxApplyCalibrationFolder.UseVisualStyleBackColor = true;
            // 
            // checkBoxVisualizar
            // 
            this.checkBoxVisualizar.AutoSize = true;
            this.checkBoxVisualizar.Location = new System.Drawing.Point(251, 31);
            this.checkBoxVisualizar.Name = "checkBoxVisualizar";
            this.checkBoxVisualizar.Size = new System.Drawing.Size(137, 23);
            this.checkBoxVisualizar.TabIndex = 13;
            this.checkBoxVisualizar.Text = "Visualize images";
            this.checkBoxVisualizar.UseVisualStyleBackColor = true;
            this.checkBoxVisualizar.CheckedChanged += new System.EventHandler(this.cb_SaveImage_CheckedChanged);
            // 
            // checkBoxCargaAut
            // 
            this.checkBoxCargaAut.AutoSize = true;
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
            // cb_SaveImage
            // 
            this.cb_SaveImage.AutoSize = true;
            this.cb_SaveImage.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_SaveImage.Location = new System.Drawing.Point(16, 47);
            this.cb_SaveImage.Name = "cb_SaveImage";
            this.cb_SaveImage.Size = new System.Drawing.Size(109, 23);
            this.cb_SaveImage.TabIndex = 8;
            this.cb_SaveImage.Text = "Save images";
            this.cb_SaveImage.UseVisualStyleBackColor = true;
            this.cb_SaveImage.CheckedChanged += new System.EventHandler(this.cb_SaveImage_CheckedChanged);
            // 
            // gbCam1
            // 
            this.gbCam1.Controls.Add(this.hWindowControl1);
            this.gbCam1.Controls.Add(this.groupBox8);
            this.gbCam1.Location = new System.Drawing.Point(11, 129);
            this.gbCam1.Name = "gbCam1";
            this.gbCam1.Size = new System.Drawing.Size(383, 870);
            this.gbCam1.TabIndex = 0;
            this.gbCam1.TabStop = false;
            this.gbCam1.Text = "RGB";
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(8, 25);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(369, 851);
            this.hWindowControl1.TabIndex = 4;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(369, 851);
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
            // gbCam2
            // 
            this.gbCam2.Controls.Add(this.hWindowControl2);
            this.gbCam2.Location = new System.Drawing.Point(412, 129);
            this.gbCam2.Name = "gbCam2";
            this.gbCam2.Size = new System.Drawing.Size(385, 876);
            this.gbCam2.TabIndex = 3;
            this.gbCam2.TabStop = false;
            this.gbCam2.Text = "NIR";
            // 
            // hWindowControl2
            // 
            this.hWindowControl2.BackColor = System.Drawing.Color.Black;
            this.hWindowControl2.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl2.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl2.Location = new System.Drawing.Point(11, 25);
            this.hWindowControl2.Name = "hWindowControl2";
            this.hWindowControl2.Size = new System.Drawing.Size(369, 845);
            this.hWindowControl2.TabIndex = 5;
            this.hWindowControl2.WindowSize = new System.Drawing.Size(369, 845);
            // 
            // textBox_output
            // 
            this.textBox_output.Location = new System.Drawing.Point(810, 12);
            this.textBox_output.Name = "textBox_output";
            this.textBox_output.Size = new System.Drawing.Size(287, 20);
            this.textBox_output.TabIndex = 14;
            this.textBox_output.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 6);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(543, 840);
            this.dataGridView1.TabIndex = 10;
            // 
            // comboCategoria
            // 
            this.comboCategoria.FormattingEnabled = true;
            this.comboCategoria.Location = new System.Drawing.Point(814, 42);
            this.comboCategoria.Name = "comboCategoria";
            this.comboCategoria.Size = new System.Drawing.Size(114, 21);
            this.comboCategoria.TabIndex = 10;
            this.comboCategoria.SelectedIndexChanged += new System.EventHandler(this.cb_SaveImage_CheckedChanged);
            // 
            // Category_assignation_active
            // 
            this.Category_assignation_active.AutoSize = true;
            this.Category_assignation_active.Location = new System.Drawing.Point(814, 69);
            this.Category_assignation_active.Name = "Category_assignation_active";
            this.Category_assignation_active.Size = new System.Drawing.Size(109, 17);
            this.Category_assignation_active.TabIndex = 11;
            this.Category_assignation_active.Text = "Force assignment";
            this.Category_assignation_active.UseVisualStyleBackColor = true;
            this.Category_assignation_active.CheckedChanged += new System.EventHandler(this.cb_SaveImage_CheckedChanged);
            // 
            // numericUpDowListaPropiedades
            // 
            this.numericUpDowListaPropiedades.Location = new System.Drawing.Point(6, 852);
            this.numericUpDowListaPropiedades.Name = "numericUpDowListaPropiedades";
            this.numericUpDowListaPropiedades.Size = new System.Drawing.Size(120, 20);
            this.numericUpDowListaPropiedades.TabIndex = 13;
            this.numericUpDowListaPropiedades.ValueChanged += new System.EventHandler(this.numbreList_Changed);
            // 
            // comboBox_Modelo
            // 
            this.comboBox_Modelo.FormattingEnabled = true;
            this.comboBox_Modelo.Location = new System.Drawing.Point(947, 42);
            this.comboBox_Modelo.Name = "comboBox_Modelo";
            this.comboBox_Modelo.Size = new System.Drawing.Size(121, 21);
            this.comboBox_Modelo.TabIndex = 15;
            this.comboBox_Modelo.SelectedIndexChanged += new System.EventHandler(this.Cargar_modelo_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Propiedades);
            this.tabControl1.Location = new System.Drawing.Point(810, 101);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(561, 904);
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
            this.Propiedades.Size = new System.Drawing.Size(553, 878);
            this.Propiedades.TabIndex = 0;
            this.Propiedades.Text = "Propierties";
            // 
            // VisionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1382, 1015);
            this.Controls.Add(this.textBox_output);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.comboBox_Modelo);
            this.Controls.Add(this.Category_assignation_active);
            this.Controls.Add(this.comboCategoria);
            this.Controls.Add(this.gbVisualizacion);
            this.Name = "VisionForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.VisionForm_Load);
            this.gbVisualizacion.ResumeLayout(false);
            this.gbVisualizacion.PerformLayout();
            this.gbFromFile.ResumeLayout(false);
            this.gbFromFile.PerformLayout();
            this.gbCam1.ResumeLayout(false);
            this.gbCam2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDowListaPropiedades)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.Propiedades.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbVisualizacion;
        private System.Windows.Forms.GroupBox gbCam1;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.GroupBox gbCam2;
        private System.Windows.Forms.DataGridView dataGridView1;
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
        private System.Windows.Forms.NumericUpDown numericUpDowListaPropiedades;
        private System.Windows.Forms.CheckBox checkBoxCargaAut;
        private System.Windows.Forms.CheckBox checkBoxVisualizar;
        private System.Windows.Forms.ComboBox comboBox_Modelo;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage Propiedades;
        private System.Windows.Forms.CheckBox checkBoxApplyCalibrationFolder;
    }
}

