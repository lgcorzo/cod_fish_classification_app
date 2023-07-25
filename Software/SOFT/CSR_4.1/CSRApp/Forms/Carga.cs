using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSRApp.forms
{    
    public partial class Carga : Form
    {

        private bool _shown;
        private string _StatusInfo;
        private int _progress;
        

        public Carga()
        {
            InitializeComponent();
            this.Shown += new System.EventHandler(this.Form1_Shown);
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this._shown = true;
        }


        public string StatusInfo
        {
            set
            {
                _StatusInfo = value;
                ChangeStatusText();
            }
            get
            {
                return _StatusInfo;
            }
        }

        public int Progress
        {
            set
            {
                _progress = value;
                ChangeProgress();
            }
            get
            {
                return _progress;
            }
        }

        public bool Loaded
        {
            get
            {
                return this._shown;
            }
        }

        public void ChangeStatusText()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(this.ChangeStatusText));
                    return;
                }
                
                lStatusInfo.Text = _StatusInfo;
            }
            catch (Exception e)
            {
                string textoOuterror = e.ToString();
                //	do something here...
            }
        }

        public void ChangeProgress()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(this.ChangeProgress));
                    return;
                }

                progressBar.Value = _progress;
            }
            catch (Exception e)
            {
                string textoOuterror = e.ToString();
                //	do something here...
            }
        }
    }
}
