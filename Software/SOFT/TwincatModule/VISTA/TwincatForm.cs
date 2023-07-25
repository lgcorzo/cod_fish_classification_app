using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TwincatModule.VISTA
{
    public partial class TwincatForm : Form
    {
        private csr.modules.CSRFormModule parent;    
        public event EventHandler SelectedInChanged;
        public event EventHandler SelectedOutChanged;

        public TwincatForm(csr.modules.CSRFormModule _parent)
        {
            this.parent = _parent;
            InitializeComponent();
        }

        private delegate void ReconnectDelegate();
        public void Reconnect()
        {
            if (this.InvokeRequired)
            {
                try
                {
                    ReconnectDelegate delegado = new ReconnectDelegate(Reconnect);
                    this.Invoke(delegado, new object[] {  });
                }
                catch (Exception e)
                {
                    string textoout = e.ToString();
                }
            }
            else
            {
                ((TwincatModule)parent).ResetMaquina();
            }


        }

        private delegate void SetSourceInDelegate(List<IO_Parameter> values);
        public void SetSourceIn(List<IO_Parameter> values)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    SetSourceInDelegate delegado = new SetSourceInDelegate(SetSourceIn);
                    this.Invoke(delegado, new object[] { values });
                }
                catch (Exception e)
                {
                    string textoout = e.ToString();
                }
            }else
            {
                this.dataGridViewIn.DataSource = values.ToList();
            }

            
        }
        private delegate void SetSourceOutDelegate(List<IO_Parameter> values);
        public void SetSourceOut(List<IO_Parameter> values)
        {
            if (this.InvokeRequired)
            {
                try
            {
                SetSourceOutDelegate delegado = new SetSourceOutDelegate(SetSourceOut);
                this.Invoke(delegado, new object[] { values });
            }
            catch (Exception e)
            {
                    string textoout = e.ToString();
             }
        }else
        {
                  this.dataGridViewOut.DataSource = values.ToList();
        }
         
    }

        private delegate void IOUpdateDelegate();
        public void IOUpdate()
        {
            
            if (this.InvokeRequired)
            {
                try
                {
                    IOUpdateDelegate delegado = new IOUpdateDelegate(IOUpdate);
                    this.Invoke(delegado, new object[] {});
                }
                catch(Exception e)
                {
                    string textoout = e.ToString();
                    parent.WriteConsole("ADVERTENCIA: la visualización de las variables de TC ha sufrido un fallo.");
                }
            }
            else
            {
                try
                {
                    //Si el modulo esta activo actualizar, sino no gastar recursos del PC
                    if (CanFocus && !IsDisposed)
                    {
                        dataGridViewIn.Enabled = true;
                        dataGridViewOut.Enabled = true;

                        dataGridViewIn.Refresh();
                        dataGridViewIn.Update();
                        dataGridViewOut.Refresh();
                        dataGridViewOut.Update();
                        dataGridViewIn_SelectionChanged(this, null);
                        dataGridViewOut_SelectionChanged(this, null);
                    }
                }
                catch (Exception e)
                {
                    string textoout = e.ToString();
                    //parent.Error("Error: la visualización de las variables de TC ha sufrido un fallo.");
                }
                
            }
        }

        private void dataGridViewIn_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            bool activo, booleano;
            booleano = Boolean.TryParse(dataGridViewIn.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out activo);
            if (booleano)
            {
                if (activo)
                    dataGridViewIn.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = new DataGridViewCellStyle { ForeColor = Color.Black, BackColor = Color.Green };
                else
                    dataGridViewIn.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = new DataGridViewCellStyle { ForeColor = Color.Black, BackColor = Color.Gray };
            
            }
            else
            {
                dataGridViewIn.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = dataGridViewIn.DefaultCellStyle;
            }
        }

        private void dataGridViewOut_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            bool activo, booleano;
            booleano = Boolean.TryParse(dataGridViewOut.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out activo);
            if (booleano)
            {
                if (activo)
                    dataGridViewOut.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = new DataGridViewCellStyle { ForeColor = Color.Black, BackColor = Color.Green };
                else
                    dataGridViewOut.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = new DataGridViewCellStyle { ForeColor = Color.Black, BackColor = Color.Gray };

            }
            else
            {
                dataGridViewOut.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = dataGridViewOut.DefaultCellStyle;
            }
        }

        private void dataGridViewIn_SelectionChanged(object sender, EventArgs e)
        {
            string selectedName="";
            if (dataGridViewIn.SelectedRows.Count > 0)
            {
                selectedName = dataGridViewIn.SelectedRows[0].Cells[0].Value.ToString();
                gbIn.Text = selectedName;
                if (SelectedInChanged != null)
                    SelectedInChanged(selectedName, null);
            }
        }

        private void dataGridViewOut_SelectionChanged(object sender, EventArgs e)
        {
            string selectedName = "";
            if (dataGridViewOut.SelectedRows.Count > 0)
            {
                selectedName = dataGridViewOut.SelectedRows[0].Cells[0].Value.ToString();
                gbOut.Text = selectedName;
                if (SelectedOutChanged != null)
                    SelectedOutChanged(selectedName, null);
            }
        }

        private delegate void DisplayIODelegate(bool input, string comment, string value);
        public void DisplayIO(bool input, string comment,string value)
        {
            if (this.InvokeRequired)
            {
                DisplayIODelegate delegado = new DisplayIODelegate(DisplayIO);
                this.Invoke(delegado, new object[] {input,comment,value });
            }
            else
            {
                if(input)
                {
                    lblComentarioIn.Text = comment;
                    lblValorIn.Text = value;
                }else
                {
                    lblComentarioOut.Text = comment;
                    lblValorOut.Text = value;
                }
                
            }
        }

        private delegate void ErrorDisplayDelegate(bool input);
        public void ErrorDisplaying(bool input)
        {
            if (this.InvokeRequired)
            {
                ErrorDisplayDelegate delegado = new ErrorDisplayDelegate(ErrorDisplaying);
                this.Invoke(delegado, new object[] { input});
            }
            else
            {
                if (input)
                {
                    this.dataGridViewIn.Enabled = false;
                }
                else
                {
                    this.dataGridViewOut.Enabled = false;
                }

            }
        }
    }
}

