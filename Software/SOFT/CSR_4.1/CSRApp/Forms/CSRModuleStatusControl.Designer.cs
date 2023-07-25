namespace CSRApp
{
    partial class CSRModuleStatusControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtNombre = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtErrores = new System.Windows.Forms.Label();
            this.txtConectado = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtNombre
            // 
            this.txtNombre.AutoSize = true;
            this.txtNombre.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNombre.Location = new System.Drawing.Point(3, 3);
            this.txtNombre.Name = "txtNombre";
            this.txtNombre.Size = new System.Drawing.Size(95, 13);
            this.txtNombre.TabIndex = 0;
            this.txtNombre.Text = "Nombre Módulo";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Errores:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(104, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Conectado:";
            // 
            // txtErrores
            // 
            this.txtErrores.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtErrores.Location = new System.Drawing.Point(51, 18);
            this.txtErrores.Name = "txtErrores";
            this.txtErrores.Size = new System.Drawing.Size(28, 13);
            this.txtErrores.TabIndex = 3;
            this.txtErrores.Text = "No";
            // 
            // txtConectado
            // 
            this.txtConectado.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConectado.Location = new System.Drawing.Point(172, 18);
            this.txtConectado.Name = "txtConectado";
            this.txtConectado.Size = new System.Drawing.Size(28, 13);
            this.txtConectado.TabIndex = 4;
            this.txtConectado.Text = "No";
            // 
            // CSRModuleStatusControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.txtConectado);
            this.Controls.Add(this.txtErrores);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtNombre);
            this.Name = "CSRModuleStatusControl";
            this.Size = new System.Drawing.Size(215, 35);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label txtNombre;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label txtErrores;
        private System.Windows.Forms.Label txtConectado;
    }
}
