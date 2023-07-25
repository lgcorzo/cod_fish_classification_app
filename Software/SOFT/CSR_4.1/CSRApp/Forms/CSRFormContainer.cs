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
    public partial class CSRFormContainer : Form
    {

        ///////////////////////////////////////////////////////////
        private CSRModuleStatusForm status_form;
        private string formActual = "";
        private Dictionary<string, Form> form_dict;
        private int last_button_index = 0;

        ///////////////////////////////////////////////////////////
        private int margen = 2; //Tamaño en pixeles de los margenes que se utilizaran entre paneles.
        private int maxMenu = 126;  //Tamaño horizontal maximo en pixeles que ocupara el menu de los modulos.
        ///////////////////////////////////////////////////////////

        public CSRFormContainer()
        {
            InitializeComponent();
            this.IsMdiContainer = true;

            this.MainMenuStrip = new MenuStrip();
            this.MainMenuStrip.Visible = false;

            status_form = new CSRModuleStatusForm();
        }

        ///////////////////////////////////////////////////////////
        public void Init()
        {

        }

        ///////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////
        public void AttachModules(Dictionary<string, csr.modules.CSRModule> module_list)
        {
            form_dict = new Dictionary<string, Form>();
            foreach (csr.modules.CSRFormModule module in module_list.Values.OfType<csr.modules.CSRFormModule>())
            {
                form_dict.Add(module.Id, module.WindowForm);
                AddMenuButton(module.Id, module.Name);
            }

            form_dict.Add("__status__", status_form);
            AddMenuButton("__status__", "Status");

            GoToModule(form_dict.First().Key);
        }

        ///////////////////////////////////////////////////////////
        public void AddMenuButton(string id, string name)
        {
            Button btnModule = new Button();
            btnModule.Name = id;
            btnModule.Parent = menu_panel;
            btnModule.Size = new System.Drawing.Size(menu_panel.Width - 2, 64);
            btnModule.Location = new System.Drawing.Point(0, last_button_index++ * 64);
            btnModule.Text = name.ToString();
            btnModule.Font = new System.Drawing.Font(btnModule.Font.FontFamily, 12.0f, FontStyle.Bold);
            btnModule.Click += MenuButtonClick;
            menu_panel.Controls.Add(btnModule);
        }

        ////////////////////////////////////////////////////////
        private void MenuButtonClick(Object o, EventArgs e)
        {
            if (o is Button)
                GoToModule(((Button)o).Name);
        }

        ////////////////////////////////////////////////////////
        private Button GetButton(string moduleId)
        {
            if (!form_dict.ContainsKey(moduleId))
                return null;

            foreach (Button bt in menu_panel.Controls)
            {
                if (bt.Name == moduleId)
                    return bt;
            }

            return null;
        }

        ////////////////////////////////////////////////////////
        public bool GoToModule(string moduleId)
        {
            if (!form_dict.ContainsKey(moduleId))
                return false;

            if (formActual != moduleId)
            {
                foreach (Button bt in menu_panel.Controls)
                {
                    if (bt.Name == formActual)
                    {
                        bt.BackColor = default(Color);
                        bt.UseVisualStyleBackColor = true;
                    }

                    if (bt.Name == moduleId)
                        bt.BackColor = Color.Gray;
                }
            }

            formActual = moduleId;
            ShowForm(form_dict[moduleId]);
            return true;
        }

        ///////////////////////////////////////////////////////////
        private void ShowForm(System.Windows.Forms.Form form)
        {
            form.MdiParent = this;

            form.Dock = DockStyle.Fill;

            form.FormBorderStyle = FormBorderStyle.None;

            controls_panel.Controls.Clear();
            controls_panel.Controls.Add(form);
            form.Show();
        }

        ///////////////////////////////////////////////////////////
        public void AddModuleToStatus(string _id, string _name)
        {
            status_form.AddModule(_id, _name);
        }

        ///////////////////////////////////////////////////////////
        public void WriteConsole(string id, string message)
        {
            if (status_form != null)
                status_form.WriteConsole(id, message);
        }

        ///////////////////////////////////////////////////////////
        public void ModuleError(string id, bool hasError)
        {
            if (status_form != null)
                status_form.SetModuleError(id, hasError);
        }

        ///////////////////////////////////////////////////////////
        public void ModuleConnection(string id, bool isConnected)
        {
            if (status_form != null)
                status_form.SetModuleConnection(id, isConnected);
        }

        ///////////////////////////////////////////////////////////

        public void Scale(Size size)
        {
            if (size.Height == 0 && size.Width == 0)
                throw new NotImplementedException("Implementar pantalla completa.");

            this.Size = size;

            this.status_form.Size = new Size(this.ClientSize.Width - (margen * 3) - menu_panel.Size.Width, this.ClientSize.Height - (margen * 2));

            //this.status_form.Scale(size);

            //((Control)this).Scale(size);

            //Arreglo para que el panel de los modulos no sobrepase de un maximo de tamaño a lo horizontal
            /*if (menu_panel.Size.Width > maxMenu)
            {
                menu_panel.Location = new Point(margen, margen);
                menu_panel.Size = new Size(maxMenu, this.ClientSize.Height - (margen * 2));
                controls_panel.Location = new Point(margen + menu_panel.Size.Width + margen, margen);
                controls_panel.Size = new Size(this.ClientSize.Width - (margen * 3) - menu_panel.Size.Width, this.ClientSize.Height - (margen * 2));
            }*/

        }

        private void CSRFormContainer_Load(object sender, EventArgs e)
        {
            RefreshForm();
            foreach (Control control in this.Controls)
            {
                MdiClient client = control as MdiClient;
                if (!(client == null))
                {
                    client.BackColor = SystemColors.Control;
                    break;
                }
            }
        }

        public void RefreshForm()
        {
            ShowForm(form_dict[formActual]);
        }

        public bool ButtonColor(string ModuleId, Color? color, bool isPermanent)
        {
            try
            {
                //TODO: implementar que sea permanente el color.
                if (!form_dict.ContainsKey(ModuleId))
                    return false;

                if (formActual != ModuleId)
                {
                    Button bt = GetButton(ModuleId);
                    if (color == null)
                    {
                        bt.BackColor = default(Color);
                        bt.UseVisualStyleBackColor = true;
                    }
                    else
                        bt.BackColor = (Color)color;
                }
                return true;
            }
            catch (Exception e)
            {
                string textoOuterror = e.ToString();
                //throw (e);
                return false;
            }
        }

        private void CSRFormContainer_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                status_form.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cerrar el StatusForm. " + ex.Message);
            }
        }
    }
}
