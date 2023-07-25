namespace CSRApp.forms
{
    partial class CSRFormContainer
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.menu_panel = new System.Windows.Forms.Panel();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.controls_panel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // menu_panel
            // 
            this.menu_panel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.menu_panel.BackColor = System.Drawing.SystemColors.Control;
            this.menu_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.menu_panel.Location = new System.Drawing.Point(2, 2);
            this.menu_panel.Name = "menu_panel";
            this.menu_panel.Size = new System.Drawing.Size(126, 526);
            this.menu_panel.TabIndex = 0;
            // 
            // controls_panel
            // 
            this.controls_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.controls_panel.BackColor = System.Drawing.SystemColors.Control;
            this.controls_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.controls_panel.Location = new System.Drawing.Point(131, 2);
            this.controls_panel.Name = "controls_panel";
            this.controls_panel.Size = new System.Drawing.Size(642, 526);
            this.controls_panel.TabIndex = 1;
            // 
            // CSRFormContainer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 529);
            this.ControlBox = false;
            this.Controls.Add(this.controls_panel);
            this.Controls.Add(this.menu_panel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.IsMdiContainer = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CSRFormContainer";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CSRFormContainer_FormClosing);
            this.Load += new System.EventHandler(this.CSRFormContainer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel menu_panel;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        public System.Windows.Forms.Panel controls_panel;

    }
}

