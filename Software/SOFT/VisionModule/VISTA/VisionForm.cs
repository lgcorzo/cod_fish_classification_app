using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using ProcesadoEspecies;
using System.Threading;

namespace VisionModule
{
    public partial class VisionForm : Form
    {
        bool            reset_counter       = false;
        int             contador_imagen     = 0;
        int             num_param_visualice = 7;
        int             categoria           = 0;
        int             linea               = 0;         
        string          pathModelo          = "modelos\\";
        string          file_modelo         = "";    
        string          Initial_date        = "";
        List<string>    m_class_names       = null;
        HWindow         Window1             = null;
        HWindow         Window2             = null;
        HImage          img1RGB             = null;  
        HImage          img1NIR             = null;      
        ModuleDelegates moduleDelegates     = null;
        //variables privadas
        private csr.modules.CSRFormModule parent    = null;
        private Features Caracteristica_grid        = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="module_delegates"></param>
        public VisionForm(csr.modules.CSRFormModule _parent, ModuleDelegates module_delegates, bool saveimage)
        {
            m_class_names = new List<string>();
            m_class_names.Add("No name available");
            this.parent = _parent;
            InitializeComponent();
            this.moduleDelegates = module_delegates;
            Init(saveimage);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Init(bool saveimage)
        {
               
            Window1 = hWindowControl1.HalconWindow;
            Window2 = hWindowControl2.HalconWindow;

            int index = 0;
            foreach (string text in m_class_names)
            {
                comboCategoria.Items.Insert(index, text);
                index++;
            }
            comboCategoria.SelectedIndex = 0;
            numericUpDowListaPropiedades.Value = num_param_visualice;
            //posibles valores de asignacion 
            comboBox_Modelo.Items.Insert(0, "0");
            comboBox_Modelo.Items.Insert(1, "A");
            comboBox_Modelo.Items.Insert(2, "B");
            comboBox_Modelo.Items.Insert(3, "C");
            comboBox_Modelo.Items.Insert(4, "D");
            comboBox_Modelo.Items.Insert(5, "E");
            comboBox_Modelo.Items.Insert(6, "F");
            comboBox_Modelo.SelectedIndex = 1;
            cb_SaveImage.Checked = saveimage;
            DateTime  timepo_inicio = DateTime.Now;
            Initial_date = timepo_inicio.Second.ToString() + "_" + timepo_inicio.Minute.ToString() + "_" + timepo_inicio.Hour.ToString() + "_" + timepo_inicio.Day.ToString() + "_" + timepo_inicio.Month.ToString() + "_" + timepo_inicio.Year.ToString();
        }


        public void DispImages(HImage img1RGB, HImage img1NIR, int linea)
        {
            try
            {

                HImage imgRGB = img1RGB.CopyImage();
                HImage imgNIR = img1NIR.CopyImage();
              
                Thread ProcesadothreadL1;
                ProcesadothreadL1 = new Thread(() => DispImagesThread(imgRGB, imgNIR,  linea));
                ProcesadothreadL1.Name = "DispImage";
                ProcesadothreadL1.Priority = ThreadPriority.Lowest;
                ProcesadothreadL1.IsBackground = true;
                ProcesadothreadL1.Start();
                //ProcesadothreadL1.Join();


            }
            catch
            {

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param> 
        /// <param name="img3"></param>
        /// <param name="img4"></param>
        /// <param name="img5"></param>

        public void DispImagesThread(HImage img1RGB,HImage img1NIR, int linea)
        {

            this.Window1.ClearWindow();
            this.Window2.ClearWindow();
     
            //para la visualizacion resto 65 a todos los canales
            HImage img1 = null, img2 = null;
            this.img1RGB = img1RGB; 
            this.img1NIR = img1NIR;     
            this.linea = linea;
         
                
            try
            {
                img1 = img1RGB.CropDomain();
            }
            catch 
            {
            }
            try
            {
                img2 = img1NIR.CropDomain();
            }
            catch 
            {
            }
               
                
        hWindowControl1.SetFullImagePart(img1);
        hWindowControl2.SetFullImagePart(img2);
             

            if (img1 != null)
            {
                try
                {
                   if (img1.CountChannels() == 3)
                        this.Window1.DispColor(img1);
                    else
                        this.Window1.DispImage(img1);
                   
                }
                catch 
                { }
            }

            if (img2 != null)
            {
                try
                {
                    if (img2.CountChannels() == 3)
                        this.Window2.DispColor(img2);
                    else
                        this.Window2.DispImage(img2);
                }
                catch 
                { }
            }
                          
        }

        public delegate void Change_clasification_module_delegate(int modelo);
        public void Change_clasification_module(int modelo)
        { 
            //actualiza el grid
            if (this.InvokeRequired)
            {
                Change_clasification_module_delegate refrescoDelegate = new Change_clasification_module_delegate(Change_clasification_module);
                this.Invoke(refrescoDelegate, new Object[] { modelo });
            }
            else //1
            {
                comboBox_Modelo.SelectedIndex = modelo;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gbVisualizacion_Enter(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VisionForm_Load(object sender, EventArgs e)
        {
            Window1 = hWindowControl1.HalconWindow;
            Window2 = hWindowControl2.HalconWindow;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_SaveImage_CheckedChanged(object sender, EventArgs e)
        {
           

            categoria = comboCategoria.SelectedIndex;
            moduleDelegates.ChangeSave(cb_SaveImage.Checked, categoria, checkBoxCargaAut.Checked, checkBoxVisualizar.Checked,
               Category_assignation_active.Checked, comboCategoria.SelectedIndex);
           
            if (checkBoxVisualizar.Checked)
            {
                comboCategoria.Enabled  = true;            
                Category_assignation_active.Enabled = true;             
                comboBox_Modelo.Enabled = true;
            }else
            {
                comboCategoria.Enabled = false;            
                Category_assignation_active.Enabled = false;            
                comboBox_Modelo.Enabled = false;

            }
        }

        public delegate void cargardatosgridDelegate(Features caracteristicas);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caracteristicas"></param>
        public void UpdateDataGridView1(Features caracteristicas)
        {
            try
            {
                //actualiza el grid
                if (this.InvokeRequired)
                {
                    cargardatosgridDelegate refrescoDelegate = new cargardatosgridDelegate(UpdateDataGridView1);
                    this.Invoke(refrescoDelegate, new Object[] { caracteristicas });
                }
                else //1
                {

                    Caracteristica_grid = caracteristicas;
                    List<string> namesToSelect = new List<string>();
                    List<Feature> FeatureToSelect = new List<Feature>();
                    namesToSelect.Add("Categoria_asignada");
                    ////
                    //actualiza los datos de categoria o asigna en caso de forzar la categoria
                    if (Category_assignation_active.Checked == true)
                    {
                        //si no tiene categoria asignada no se añade a la lista
                        caracteristicas.addFeature("Categoria_Forzada", comboCategoria.SelectedIndex);


                    }
                    else //2
                    {
                        try
                        {
                            caracteristicas.Select(namesToSelect);
                            FeatureToSelect = caracteristicas.GetSelected();
                            comboCategoria.SelectedIndex = (int)FeatureToSelect[0].Value;
                            int numero_elementos = FeatureToSelect.Count();
                        }
                        catch
                        {

                        }
                    }
                    //////
                    num_param_visualice = (int)numericUpDowListaPropiedades.Value;
                    numericUpDowListaPropiedades.Minimum = 3;
                    numericUpDowListaPropiedades.Maximum = caracteristicas.Data.Values.Count;

                    this.dataGridView1.Rows.Clear();
                    this.dataGridView1.ColumnCount = 3;
                    this.dataGridView1.Columns[0].Name = "Propierty Name";
                    this.dataGridView1.Columns[1].Name = "Value";
                    this.dataGridView1.Columns[2].Name = "Error";
                    int contador = 0;
                    string param1, param2, param3;
                    string[] row;
                    foreach (Feature feature in caracteristicas.Data.Values)
                    {

                        param1 = feature.Name;
                        param2 = feature.Value.ToString();
                        param3 = "No";
                        row = new string[] { param1, param2, param3 };
                        this.dataGridView1.Rows.Add(row);
                        contador++;
                        if (contador > num_param_visualice)
                            break;
                    }
                    namesToSelect.Clear();
                    namesToSelect.Add("Categoria_asignada");
                    try
                    {
                        caracteristicas.Select(namesToSelect);
                        FeatureToSelect = caracteristicas.GetSelected();
                        param1 = FeatureToSelect[0].Name;
                        param2 = FeatureToSelect[0].Value.ToString();
                        param3 = "No";
                        row = new string[] { param1, param2, param3 };
                        this.dataGridView1.Rows.Add(row);
                    }
                    catch
                    { }

                    //escribe en fichero si ha funionado el algoritmo

                }


            }
            catch
            { }
            
             
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCargar_Click(object sender, EventArgs e)
        {
            string folderName;
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderName = folderBrowserDialog1.SelectedPath;
                if (moduleDelegates.ChangeFolderPath != null)
                {
                    moduleDelegates.ChangeFolderPath(folderName);
                    this.lblPath.Text = folderName;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExecute_Click(object sender, EventArgs e)
        {
            Thread hiloProcesado = new Thread(() => moduleDelegates.ExecuteFromFolder(checkBoxApplyCalibrationFolder.Checked));
            hiloProcesado.Priority = ThreadPriority.Normal;
            hiloProcesado.IsBackground = true;
            hiloProcesado.Start();
       
        }
        /// <summary>
        /// carga modelo de clasificacion
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cargar_modelo_Click(object sender, EventArgs e)
        {

            //carga el modelo según el tipo de producto selecconado
            
            switch (comboBox_Modelo.SelectedIndex)
            {
                case 0:
                    file_modelo = pathModelo + "clasificador_default";
               
                    break;
                case 1: //Berenjena
                    file_modelo = pathModelo + "clasificador_A";
                  
                    break;
                case 2: //Pimiento Rojo
                    file_modelo = pathModelo + "clasificador_B";
                 
                    break;
                case 3: //Pimineto Verde
                    file_modelo = pathModelo + "clasificador_C";
                  
                    break;
                case 4: //Pepino corto
                    file_modelo = pathModelo + "clasificador_D";
                 
                    break;
                case 5: //Pepino Frances
                    file_modelo = pathModelo + "clasificador_E";
                  
                    break;
                case 6: //Berenjena Rayada
                    file_modelo = pathModelo + "clasificador_F";
                   
                    break;
                default:
                    file_modelo = pathModelo + "clasificador_default";
                  
                    break;

            }
            //carga el modelo que le corresponda  
            if (comboBox_Modelo.SelectedIndex > 0)
            {           
                int index = 0;
                m_class_names = moduleDelegates.CargaModeloClasificacion(file_modelo);
                comboCategoria.Items.Clear();
                foreach (string text in m_class_names)
                {
                    comboCategoria.Items.Insert(index, text);
                    index++;
                }
                comboCategoria.SelectedIndex = 0;
            }
                


        }

        public delegate bool DispCounterDelegate(int count, string output_text);
        public bool DispCounter(int count, string output_text)
        {
            bool output = reset_counter;
            
            if (this.InvokeRequired)
            {
                DispCounterDelegate function = new DispCounterDelegate(DispCounter);
                this.Invoke(function, new Object[] { count, output_text });
            }
            else //1
            {
                this.lblContador.Text = count.ToString();
                this.textBox_output.Text = output_text;
                reset_counter = false;
                contador_imagen = count;

                this.dataGridView1.Rows.Clear();
                this.dataGridView1.ColumnCount = 3;
                this.dataGridView1.Columns[0].Name = "Propierty Name";
                this.dataGridView1.Columns[1].Name = "Value";
                this.dataGridView1.Columns[2].Name = "Error";

            }
            return (output);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_reset_Click(object sender, EventArgs e)
        {
            moduleDelegates.ResetContadores();
            reset_counter = true;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdRGB_CheckedChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rdNIR_CheckedChanged(object sender, EventArgs e)
        {
            HImage img1 = null, img2 = null;

            try
            {
                img1 = img1RGB.CropDomain();
            }
            catch 
            {
            }
            try
            {
                img2 = img1NIR.CropDomain();
            }
            catch 
            {
            }

            hWindowControl1.SetFullImagePart(img1);
            hWindowControl2.SetFullImagePart(img2);
      
            if (img1 != null)
            {
                try
                {
                    if (img1.CountChannels() == 3)
                        this.Window1.DispColor(img1);
                    else
                        this.Window1.DispImage(img1);

                }
                catch 
                { }
            }

            if (img2 != null)
            {
                try
                {
                    if (img2.CountChannels() == 3)
                        this.Window2.DispColor(img2);
                    else
                        this.Window2.DispImage(img2);
                }
                catch 
                { }
            }
        
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numbreList_Changed(object sender, EventArgs e)
        {
            try
            {

                if (Caracteristica_grid != null)
                {
                    List<string> namesToSelect = new List<string>();
                    List<Feature> FeatureToSelect = new List<Feature>();
                    namesToSelect.Add("Categoria_asignada");
                    num_param_visualice = (int)numericUpDowListaPropiedades.Value;
                    numericUpDowListaPropiedades.Minimum = 1;
                    numericUpDowListaPropiedades.Maximum = Caracteristica_grid.Data.Values.Count;

                    this.dataGridView1.Rows.Clear();
                    this.dataGridView1.ColumnCount = 3;
                    this.dataGridView1.Columns[0].Name = "Propierty Name";
                    this.dataGridView1.Columns[1].Name = "Value";
                    this.dataGridView1.Columns[2].Name = "Error";
                    int contador = 0;
                    string param1, param2, param3;
                    string[] row;
                    foreach (Feature feature in Caracteristica_grid.Data.Values)
                    {

                        param1 = feature.Name;
                        param2 = feature.Value.ToString();
                        param3 = "No";
                        row = new string[] { param1, param2, param3 };
                        this.dataGridView1.Rows.Add(row);
                        contador++;
                        if (contador > num_param_visualice)
                            break;
                    }

                    try
                    {
                        Caracteristica_grid.Select(namesToSelect);
                        FeatureToSelect = Caracteristica_grid.GetSelected();
                        param1 = FeatureToSelect[0].Name;
                        param2 = FeatureToSelect[0].Value.ToString();
                        param3 = "No";
                        row = new string[] { param1, param2, param3 };
                        this.dataGridView1.Rows.Add(row);
                    }
                    catch (Exception)
                    { }

                }

            }
            catch{ }
            
        }
        ///////////////
    }
}
