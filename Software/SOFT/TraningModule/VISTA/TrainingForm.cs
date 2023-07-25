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

namespace TrainingModule
{
    public partial class TrainingForm : Form
    {      
        int                 contador_imagen         = 0;
        int                 num_param_visualice     = 600;
        int                 categoria               = 0;
        int                 linea                   = 0;
        int                 rowselected             = -1;      
        string              folderName              = "";
        string              pathModelo              = "modelos\\";
        string              file_modelo             = "";
        string              Path_File_Arff          = "";
        string              Initial_date            = "";
        List<string>        m_class_names           = null;
        HWindow             Window1                 = null;
        HWindow             Window2                 = null;
        HImage              img1RGB                 = null; 
        HImage              img1NIR                 = null;
        HImage              img1                    = null;
        HImage              img2                    = null;
        HRegion             Defectosdisp            = null;      
        List<Features>      Instancias              = null;             
        ModuleDelegates     moduleDelegates         = null;
        bool                reset_counter           = false;
        private csr.modules.CSRFormModule parent    = null;
        private Features    Caracteristica_grid     = null;
        HImage knnimage                             = new HImage();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="module_delegates"></param>
        public TrainingForm(csr.modules.CSRFormModule _parent, ModuleDelegates module_delegates)
        {
            this.moduleDelegates    = module_delegates;
            this.parent             = _parent;          
            m_class_names           = new List<string>();          
            m_class_names.Add("No name available");           
            InitializeComponent();
           
            Init();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Init()
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
            comboBox_Modelo.Items.Insert(0, "no seleccionado");
            comboBox_Modelo.Items.Insert(1, "A");
            comboBox_Modelo.Items.Insert(2, "B");
            comboBox_Modelo.Items.Insert(3, "C");
            comboBox_Modelo.Items.Insert(4, "D");
            comboBox_Modelo.Items.Insert(5, "E");
            comboBox_Modelo.Items.Insert(6, "F");
            comboBox_Modelo.SelectedIndex = 1;
            DateTime  timepo_inicio = DateTime.Now;
            Initial_date = timepo_inicio.Second.ToString() + "_" + timepo_inicio.Minute.ToString() + "_" + timepo_inicio.Hour.ToString() + "_" + timepo_inicio.Day.ToString() + "_" + timepo_inicio.Month.ToString() + "_" + timepo_inicio.Year.ToString();
            hWindowControl8.HalconWindow.ClearWindow();
            knnimage.ReadImage("Procedimientos\\88_colortable.bmp");
            knnimage = knnimage.RotateImage(90.0, "constant");
            hWindowControl8.SetFullImagePart(knnimage);
            hWindowControl8.HalconWindow.DispColor(knnimage);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img1"></param>
        /// <param name="img2"></param> 
        /// <param name="img3"></param>
        /// <param name="img4"></param>
        /// <param name="img5"></param>

        public void DispImages(HImage img1RGB,HImage img1NIR, int linea, HRegion Defectosdisp)
        {        
            this.Window1.ClearWindow();
            this.Window2.ClearWindow();    
            this.img1RGB        = img1RGB;
            this.img1NIR        = img1NIR;     
            this.linea          = linea;
            this.Defectosdisp   = Defectosdisp;        
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
            Window2.SetDraw("margin");
            Window2.SetColor("red");
            img2.GetDomain().DispRegion(Window2);

            Window1.SetDraw("margin");
            Window1.SetColor("red");
            img1.GetDomain().DispRegion(Window1);

        }
        public delegate void Change_clasification_module_delegate(int modelo);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelo"></param>
        /// <param name="class_names"></param>
        public void Change_clasification_module(int modelo)
        { //0 Berenjena
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
        public void setListInstancias(List<Features> Instancias)
        {
            this.Instancias = Instancias;
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
                comboCategoria.Enabled              = true;
                buttonInstancia.Enabled             = true;
                Category_assignation_active.Enabled = true;
                button1.Enabled                     = true;
                button_borrar.Enabled               = true;
                comboBox_Modelo.Enabled             = true;
            }else
            {
                comboCategoria.Enabled              = false;
                buttonInstancia.Enabled             = false;
                Category_assignation_active.Enabled = false;
                button1.Enabled                     = false;
                button_borrar.Enabled               = false;
                comboBox_Modelo.Enabled             = false;

            }
        }

        public delegate void cargardatosgridDelegate(Features caracteristicas);
        public delegate void ActualizalistaInstancas();
   
        /// 
        /// </summary>
        /// <param name="caracteristicas"></param>
        public void UpdateDataGridView1(Features caracteristicas)
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
                List<string> namesToSelect      = new List<string>();
                List<Feature> FeatureToSelect   = new List<Feature>();
                namesToSelect.Add("Categoria_asignada");
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
                        FeatureToSelect                 = caracteristicas.GetSelected();
                        comboCategoria.SelectedIndex    = (int)FeatureToSelect[0].Value;
                        int numero_elementos            = FeatureToSelect.Count();         
                    }
                    catch
                    {
                       
                    }
                }
                #region dataGridView1
               
                num_param_visualice = (int)numericUpDowListaPropiedades.Value;
                numericUpDowListaPropiedades.Minimum    = 3;
                numericUpDowListaPropiedades.Maximum    = caracteristicas.Data.Values.Count;
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
                   
                    
                    param1      = feature.Name;
                    if (param1 == "ID")
                        param2 = feature.lValue.ToString();
                    else
                        param2 = feature.Value.ToString();
                    param3      = "No";
                    row         = new string[] { param1, param2, param3 };
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

                #endregion //dataGridView1
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caracteristicas"></param>
        public void UpdateDataGridView2()
        {
            
            List<string> namesToSelect = new List<string>();
            List<Feature> FeatureToSelect = new List<Feature>();
            int contador = 0;

            //actualiza el grid
            if (this.InvokeRequired)
            {
                ActualizalistaInstancas refrescoDelegate = new ActualizalistaInstancas(UpdateDataGridView2);
                this.Invoke(refrescoDelegate, new Object[] {  });
            }
            else //1
            {              
                #region dataGridView2
                if (Instancias != null && Instancias.Count > 0)
                {
                    this.dataGridView2.Rows.Clear();
                    this.dataGridView2.ColumnCount = 4;
                    this.dataGridView2.Columns[0].Name = "pos";
                    this.dataGridView2.Columns[1].Name = "ID";
                    this.dataGridView2.Columns[2].Name = "Classified";
                    this.dataGridView2.Columns[3].Name = "Forced";
                    this.dataGridView2.Columns[0].Width = 40;
                    this.dataGridView2.Columns[1].Width = 40;

                    string param21, param22, param23, param0;
                    object[] row2;
                    int param1;
                    long param2;
                    foreach (Features Instancia in Instancias)
                    {
                        try
                        {
                            contador++;
                            namesToSelect.Clear();
                            namesToSelect.Add("ID");
                            Instancia.Select(namesToSelect);
                            FeatureToSelect = Instancia.GetSelected();
                            namesToSelect.Clear();
                            param2 = (long)(FeatureToSelect[0].lValue);
                            param21 = FeatureToSelect[0].Value.ToString();

                            namesToSelect.Clear();
                            namesToSelect.Add("Categoria_Forzada");
                            Instancia.Select(namesToSelect);
                            FeatureToSelect = Instancia.GetSelected();
                            namesToSelect.Clear();
                            int valor = (int)(FeatureToSelect[0].Value);
                            param23 = m_class_names[valor];

                            namesToSelect.Clear();
                            namesToSelect.Add("Categoria_asignada");
                            Instancia.Select(namesToSelect);
                            FeatureToSelect = Instancia.GetSelected();
                            namesToSelect.Clear();
                            int valor1 = (int)(FeatureToSelect[0].Value);
                            param22 = m_class_names[valor1];
                            param0= contador.ToString();
                            param1 = contador;                       
                            row2 = new object[] { param1, param2, param22, param23 };
                            this.dataGridView2.Rows.Add(row2);
                        }
                        catch 
                        { }

                    }
                }
                #endregion //dataGridView2
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCargar_Click(object sender, EventArgs e)
        {
          
            FolderBrowserDialog folderBrowserDialog1    = new FolderBrowserDialog();
            DialogResult        result                  = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                folderName = folderBrowserDialog1.SelectedPath;
                if (moduleDelegates.ChangeFolderPath != null)
                {
                    moduleDelegates.ChangeFolderPath(folderName);             
                    this.lblPath.Text   = Path.GetFileName(folderName);              
                    Path_File_Arff      = folderName + "\\Instancias_imagenes.arff";
                }

                if( Instancias.Count == 0 )
                {
                    Instancias.Clear();
                    this.dataGridView2.Rows.Clear();
                    this.dataGridView2.Update();
                    this.dataGridView2.Refresh();
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
           
            Thread hiloProcesado        = new Thread(() => moduleDelegates.ExecuteFromFolder());
            hiloProcesado.Priority      = ThreadPriority.Normal;
            hiloProcesado.IsBackground  = true;
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
                this.lblContador.Text       = count.ToString();
                this.textBox_output.Text    = output_text;
                reset_counter               = false;
                contador_imagen             = count;
                this.dataGridView1.Rows.Clear();
                this.dataGridView1.ColumnCount      = 3;
                this.dataGridView1.Columns[0].Name  = "Propierty Name";
                this.dataGridView1.Columns[1].Name  = "Value";
                this.dataGridView1.Columns[2].Name  = "Error";
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
        private void numbreList_Changed(object sender, EventArgs e)
        {
            if (Caracteristica_grid != null)
            {
                List<string> namesToSelect = new List<string>();
                List<Feature> FeatureToSelect = new List<Feature>();
                namesToSelect.Add("Categoria_asignada");
                num_param_visualice                     = (int)numericUpDowListaPropiedades.Value;
                numericUpDowListaPropiedades.Minimum    = 1;
                numericUpDowListaPropiedades.Maximum    = Caracteristica_grid.Data.Values.Count;

                this.dataGridView1.Rows.Clear();
                this.dataGridView1.ColumnCount      = 3;
                this.dataGridView1.Columns[0].Name  = "Propierty Name";
                this.dataGridView1.Columns[1].Name  = "Value";
                this.dataGridView1.Columns[2].Name  = "Error";
                int contador = 0;
                string param1, param2, param3;
                string[] row;
                foreach (Feature feature in Caracteristica_grid.Data.Values)
                {

                    param1 = feature.Name;
                    param2 = feature.Value.ToString();
                    param3 = "No";
                    row    = new string[] { param1, param2, param3 };
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
                    row     = new string[] { param1, param2, param3 };
                    this.dataGridView1.Rows.Add(row);
                }
                catch(Exception)
                { }
                
            }
            
        }
        /// <summary>
        /// añade una instancia al fichero para el entrenamiento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonInstancia_Click(object sender, EventArgs e)
        {
            if (Caracteristica_grid != null)
            {          
               
                //si no esta asignada categoria  se añade
                try
                {                
                    Caracteristica_grid.addFeature("Categoria_Forzada", comboCategoria.SelectedIndex);
                    //modifico la proiedad del gridview2 y de la lista
                  
                }
                catch { }              
            }

            if (Instancias != null && rowselected > -1)
            {
                List<string> namesToSelect = new List<string>();
                List<Feature> FeatureToSelect = new List<Feature>();
                int selectedPos = Int16.Parse(this.dataGridView2.Rows[rowselected].Cells[0].Value.ToString())-1;
                //si no esta asignada categoria  se añade
                try
                {                                
                    Instancias[selectedPos].addFeature("Categoria_Forzada", comboCategoria.SelectedIndex);                
                    this.dataGridView2.Rows[rowselected].Cells[3].Value = m_class_names[comboCategoria.SelectedIndex];
                    this.dataGridView2.Update();
                    this.dataGridView2.Refresh();
                }
                catch { }
            }
        }

        /// <summary>
        /// añade una instancia al fichero para el entrenamiento
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_eliminar_Instancia_Click(object sender, EventArgs e)
        {
           
            if (Instancias != null && rowselected > -1)
            {
                List<string> namesToSelect = new List<string>();
                List<Feature> FeatureToSelect = new List<Feature>();
                int selectedPos =  Int16.Parse(this.dataGridView2.Rows[rowselected].Cells[0].Value.ToString())-1;
                //si no esta asignada categoria  se añade

                try
                {
                    Instancias.RemoveAt(selectedPos);              
                    UpdateDataGridView2();
                    rowselected = -1;


                }
                catch { }
            }
        }
        //
        //
        //
        private void Eliminar_arff()
        {
            // ...or by using FileInfo instance method.
            System.IO.FileInfo fi = new System.IO.FileInfo(Path_File_Arff);
            try
            {
                fi.Delete();
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

           


        }
        /// <summary>
        /// genera el modelo apartir del fichero arff
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {

          
            if(Instancias != null)
            {
                Eliminar_arff();
                for (int i = 0; i < Instancias.Count(); i++)
                {
                    Features.StoreToFileArff(Path_File_Arff, Instancias[i], m_class_names);
                }
              
            }
            
        }
      
        //seleccionar la imagen y visualiarla ejecutar el procesado
        private void dataGridView2_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            cb_SaveImage.Checked        = false;
            checkBoxCargaAut.Checked    = false;
            cb_SaveImage_CheckedChanged( sender,  e);

            DataGridViewRow row_selected    = dataGridView2.CurrentRow;
            rowselected                     = this.dataGridView2.CurrentCell.RowIndex;
            int selectedPos                 = Int16.Parse(this.dataGridView2.Rows[rowselected].Cells[0].Value.ToString())-1;

            if (Instancias != null && Instancias.Count > 0)
            {
                Features Instancia              = Instancias[selectedPos];           
                List<string> namesToSelect      = new List<string>();
                namesToSelect.Clear();
                namesToSelect.Add("ID");
                Instancia.Select(namesToSelect);
                List <Feature> Attributos  = Instancia.GetSelected();
              
                long ID =(long) Attributos[0].lValue;
                if (moduleDelegates.ChangeImagePath != null)
                {
                    moduleDelegates.ChangeImagePath(ID);
                }
                //llamo a la funcion de carga de la imagen con el ID
                btnExecute_Click(sender, e);
            }            

        }

        private void hWindowControl1_HMouseDown(object sender, HMouseEventArgs e)
        {            
            ManualselectionFrom ImageInspection = new ManualselectionFrom();
            ImageInspection.VisualizaImagen(img1, false);
            ImageInspection.ShowDialog();
        }

        private void button_borrar_Click(object sender, EventArgs e)
        {
            string message = "Desea borrar el fichero arff?";
            string caption = "Precaución:";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            // Displays the MessageBox.

            result = MessageBox.Show(message, caption, buttons);

            if (result == System.Windows.Forms.DialogResult.Yes)
            {          
                Eliminar_arff();
                Instancias.Clear();
                this.dataGridView2.Rows.Clear();
                this.dataGridView2.Update();
                this.dataGridView2.Refresh();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Instancias != null )
            {
                List<string> namesToSelect = new List<string>();
                List<Feature> FeatureToSelect = new List<Feature>();

                //si no esta asignada categoria  se añade

                try
                {
                   
                    for (int rowselected = 0; rowselected < Instancias.Count; rowselected++ )
                    {                 
                        namesToSelect.Add("Categoria_asignada");
                        Instancias[rowselected].Select(namesToSelect);
                        FeatureToSelect = Instancias[rowselected].GetSelected();
                        int valor_asignado = Convert.ToInt16(FeatureToSelect[0].Value);
                        namesToSelect.Clear();                           
                        Instancias[rowselected].addFeature("Categoria_Forzada", valor_asignado);
                        this.dataGridView2.Rows[rowselected].Cells[2].Value = m_class_names[valor_asignado];
                     

                    }
                    UpdateDataGridView2();
                }
                catch { }
            }
        }
        //funcionalidad para eliminar las no asignadas
        private void button4_Click(object sender, EventArgs e)
        {
            if (Instancias != null )
            {
                List<string> namesToSelect = new List<string>();
                List<Feature> FeatureToSelect = new List<Feature>();

                for  (int index = 0; index < Instancias.Count; index++)
                {
                    try
                    {
                        namesToSelect.Add("Categoria_Forzada");
                        Instancias[index].Select(namesToSelect);
                        FeatureToSelect = Instancias[index].GetSelected();
                        int valor_asignado = Convert.ToInt16(FeatureToSelect[0].Value);
                       
                        if (valor_asignado < 1)
                        {
                            Instancias.RemoveAt(rowselected);
                            this.dataGridView2.Rows.RemoveAt(rowselected);
                            this.dataGridView2.Update();
                            this.dataGridView2.Refresh();
                        }            
                    }
                    catch { }
                }
                
            }
        }

        private void hWindowControl2_HMouseDown(object sender, HMouseEventArgs e)
        {
            ManualselectionFrom ImageInspection = new ManualselectionFrom();
            ImageInspection.VisualizaImagen(img2, false);
            ImageInspection.ShowDialog();
        }
        //genera modelo 
        private void Model_gen_Click(object sender, EventArgs e)
        {

        }

        private void gbVisualizacion_Paint(object sender, PaintEventArgs e)
        {
            hWindowControl8.HalconWindow.DispColor(knnimage);
        }
    }
}
