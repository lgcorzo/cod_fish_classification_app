using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlarmModule
{
    public partial class AlarmForm : Form
    {
        private csr.modules.CSRFormModule parent;
        ModuleDelegates moduleDelegates;

        ///////////////////////////////////
        private DescripcionAlarmas m_alarmas;

        ///////////////////////////////////
        public AlarmForm(csr.modules.CSRFormModule _parent, ModuleDelegates moduleDelegates)
        {
            this.parent = _parent;
            InitializeComponent();
            this.moduleDelegates = moduleDelegates;
            Init();
            listBox1.Size = new Size(this.splitContainer1.Panel1.Width - 100, this.splitContainer1.Panel1.Height - 50);
            listBox1.Location = new Point(25, 25);
            listBox1.Font = new Font("Arial", 16, FontStyle.Bold);
            this.splitContainer1.SplitterDistance = this.splitContainer1.Height / 2;
        }

        ///////////////////////////////////
        private void Init()
        {
            m_alarmas = new DescripcionAlarmas();
        }

        ///////////////////////////////////
        delegate bool AddList(DescripcionAlarma da);
        public bool AñadirALista(DescripcionAlarma da)
        {
            if (listBox1.InvokeRequired)
            {
                AddList d = new AddList(AñadirALista);
                //this.Invoke(d, new object[] { da });
                return (bool)this.Invoke(new Func<bool>(() => d(da)));
            }
            else
            {
                if (!listBox1.Items.Contains(da))
                {
                    listBox1.Items.Add(da);
                    listBox1.DisplayMember = "Titulo";
                    return true;
                }
                else
                    return false;
            }
        }

        public Task<T> BeginInvokeExWithReturnValue<T>(Func<T> actionFunction)
        {
            var task = new Task<T>(actionFunction);
            task.Start();
            return task;
        }

        ///////////////////////////////////
        delegate bool RemoveList(DescripcionAlarma da);
        public bool BorrarDeLista(DescripcionAlarma da)
        {
            if (listBox1.InvokeRequired)
            {
                RemoveList d = new RemoveList(BorrarDeLista);
                return (bool)this.Invoke(new Func<bool>(() => d(da)));
            }
            else
            {
                try
                {
                    if (listBox1.Items.Contains(da))
                    {
                        listBox1.Items.Remove(da);
                        return true;
                    }
                    return false;
                }
                catch (Exception e)
                {
                    string text = e.ToString();
                    //parent.Error("Error al borrar la alarma de la lista. " + e.Message, true, false);
                    return false;
                }
            }
        }

        ///////////////////////////////////
        public bool ListaVacia()
        {
            return listBox1.Items.Count == 0;
        }

        ///////////////////////////////////
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
                textBox1.Text = ((DescripcionAlarma)listBox1.SelectedItem).Titulo + ": " + ((DescripcionAlarma)listBox1.SelectedItem).Descripcion;
            else
                textBox1.Text = "";
        }

        ///////////////////////////////////
        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > 0)
                listBox1.SelectedIndex--;
            else
            {
                if (listBox1.SelectedIndex == 0)
                    listBox1.SelectedIndex = 0;
            }
                
        }

        ///////////////////////////////////
        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < (listBox1.Items.Count - 1))
                listBox1.SelectedIndex++;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            moduleDelegates.ExecuteScript();
        }
    }
}
