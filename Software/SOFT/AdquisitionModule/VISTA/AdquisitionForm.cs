using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HalconDotNet;
using System.Threading;
using System.IO;
using System.Collections; // required for NumericComparer : IComparer only

namespace AdquisitionModule
{
    public partial class AdquisitionForm : Form
    {

        FormInterface                       InterfacetoVisializerParams = new FormInterface();
        private csr.modules.CSRFormModule   parent                  = null;
        private AdquisitionModule           AdquisitionProcesado    = null;
        private HWindow                     WindowRGB0              = null;    // imagen RGB en bruto
        private HWindow                     WindowNIR0              = null;   // imagen NIR en bruto
        private HWindow                     WindowRGB1              = null;   // imagen RGB corregida
        private HWindow                     WindowNIR1              = null;  // imagen NIR corregida
        private HWindow                     WindowRGB2              = null;    // imagen RGB pez completo
        private HWindow                     WindowNIR2              = null;    // imagen NIR pez completo        
          
        public AdquisitionForm(csr.modules.CSRFormModule _parent)
        {
            this.parent             = _parent;
            this.FormBorderStyle    = FormBorderStyle.FixedSingle;
            this.MaximizeBox        = false;
            InitializeComponent();      
            //inicializo los modulos delegados para representacion.
            ModuleDelegates module_delegates = new ModuleDelegates( PrintOutText,       DispImages, DispResultsFormAsync,
                                                                    GetResultsParams,   VisualizaResultado);
            AdquisitionProcesado    = (AdquisitionModule)(this.parent); 
            AdquisitionProcesado.SetAdquisitionProcessDelegates(module_delegates);
            txtPathSEQ.Text         = AdquisitionProcesado.path_simularCaptura;     
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="modo"></param>
        public void DispResultsForm(FormInterface paramsform)
        {
            InterfacetoVisializerParams = paramsform;
            this.Invoke((MethodInvoker)delegate
            {
                lblContImg0.Text        = paramsform.lblContImg0_Text;
                lblContImg1.Text        = paramsform.lblContImg1_Text;
                lblContImg2Wrong.Text   = paramsform.lblContImg2Wrong_Text;
                lblContImg2.Text        = paramsform.lblContImg2_Text;
                textBox_fr.Text         = paramsform.fr.ToString();
               

            });

        }

        /// <summary>
        /// 
        /// </summary>
     
        public void DispResultsFormAsync(FormInterface paramsform)
        {         
           try
            {

                
                Thread ProcesadothreadL1;
                ProcesadothreadL1 = new Thread(() => DispResultsForm(paramsform));
                ProcesadothreadL1.Name = "DispResultsFormAsync";
                ProcesadothreadL1.Priority = ThreadPriority.Lowest;
                ProcesadothreadL1.IsBackground = true;
                ProcesadothreadL1.Start();
            }
            catch
            {

            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramsform"></param>
        public FormInterface GetResultsParams()
        {
            FormInterface paramsOut         = new FormInterface();
            paramsOut.lblContImg0_Text      = lblContImg0.Text;
            paramsOut.lblContImg1_Text      = lblContImg1.Text;
            paramsOut.cbVisualizar0_Checked = cbVisualizar0.Checked;
            paramsOut.cbGuardar0_Checked    = cbGuardar0.Checked;
            paramsOut.cbGuardar1_Checked    = cbGuardar1.Checked;
            paramsOut.cbVisualizar1_Checked = cbVisualizar1.Checked;
            paramsOut.cbVisualizar2_Checked = cbVisualizar2.Checked;     
            paramsOut.cbGuardar2_Checked    = cbGuardar2.Checked;
            paramsOut.lblContImg2_Text      = lblContImg2.Text;
            paramsOut.fr                    = Int32.Parse(textBox_fr.Text);
            paramsOut.lblContImg2Wrong_Text = lblContImg2Wrong.Text;
            return (paramsOut);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="modo"></param>
        /// 
        public void DispImages(ElementosToProcess obj, RegionVisualizar modo)
        {
            try
            {

                HImage imgRGB = obj.imagenes[0].imagenRGB.CopyImage();
                HImage imgNIR = obj.imagenes[0].imagenNIR.CopyImage();
                Thread ProcesadothreadL1;
                ProcesadothreadL1 = new Thread(() => DispImagesThread(imgRGB, imgNIR,  modo));
                ProcesadothreadL1.Name = "DispImage";
                ProcesadothreadL1.Priority = ThreadPriority.Lowest;
                ProcesadothreadL1.IsBackground = true;
                ProcesadothreadL1.Start();
                

            }
            catch
            {

            }
        }
        /// <summary>
        /// ////////////////
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="modo"></param>
        public void DispImagesThread(HImage imgRGB, HImage imgNIR , RegionVisualizar modo)
        {
            try
            {
                ParamConf ConfiguracionParams = AdquisitionProcesado.ConfiguracionParams;
                
                HRegion ROI2 = new HRegion(ConfiguracionParams.ROI2_Row1, ConfiguracionParams.ROI2_Col1, ConfiguracionParams.ROI2_Row2, ConfiguracionParams.ROI2_Col2);
                HRegion ROI1 = new HRegion(ConfiguracionParams.ROI1_Row1, ConfiguracionParams.ROI1_Col1, ConfiguracionParams.ROI1_Row2, ConfiguracionParams.ROI1_Col2);
           


                switch (modo)
                {
                    case RegionVisualizar.IMAGENES_BRUTO:
                        WindowRGB0.ClearWindow();
                        hWinCtrRGB0.SetFullImagePart(imgRGB);
                        WindowRGB0.SetDraw("margin");
                        WindowRGB0.SetColor("red");
                        ROI1.DispRegion(WindowRGB0);
                        WindowRGB0.SetColor("green");
                        ROI2.DispRegion(WindowRGB0);
                        imgRGB.DispObj(WindowRGB0);
                        WindowRGB0.SetColor("red");
                        ROI1.DispRegion(WindowRGB0);
                        WindowRGB0.SetColor("green");
                        ROI2.DispRegion(WindowRGB0);
                        WindowNIR0.ClearWindow();

                        hWinCtrNIR0.SetFullImagePart(imgNIR);
                        WindowNIR0.SetDraw("margin");
                        WindowNIR0.SetColor("red");
                        ROI1.DispRegion(WindowNIR0);
                        WindowNIR0.SetColor("green");
                        ROI2.DispRegion(WindowNIR0);
                        imgNIR.DispObj(WindowNIR0);
                        WindowNIR0.SetColor("red");
                        ROI1.DispRegion(WindowNIR0);
                        WindowNIR0.SetColor("green");
                        ROI2.DispRegion(WindowNIR0);

                        break;

                    case RegionVisualizar.IMAGENES_PREPROCESADAS:
                        WindowRGB1.ClearWindow();                                 
                        hWinCtrRGB1.SetFullImagePart(imgRGB);
                        imgRGB.DispObj(WindowRGB1);
                        WindowRGB1.SetDraw("margin");
                        WindowRGB1.SetColor("red");
                        imgRGB.GetDomain().DispRegion(WindowRGB1);

                        WindowNIR1.ClearWindow();                                         
                        hWinCtrNIR1.SetFullImagePart(imgNIR);
                        imgNIR.DispObj(WindowNIR1);
                        WindowNIR1.SetDraw("margin");
                        WindowNIR1.SetColor("green");
                        imgRGB.GetDomain().DispRegion(WindowNIR1);
                        break;

                    case RegionVisualizar.IMAGENES_PEZ_COMPLETO:

                        WindowRGB2.ClearWindow();                                 
                        hWinCtrRGB2.SetFullImagePart(imgRGB);
                        imgRGB.DispObj(WindowRGB2);
                        WindowRGB2.SetDraw("margin");
                        WindowRGB2.SetColor("red");
                        imgRGB.GetDomain().DispRegion(WindowRGB2);

                        WindowNIR2.ClearWindow();                                   
                        hWinCtrNIR2.SetFullImagePart(imgNIR);
                        imgNIR.DispObj(WindowNIR2);
                        WindowNIR2.SetDraw("margin");
                        WindowNIR2.SetColor("green");
                        imgNIR.GetDomain().DispRegion(WindowNIR2);
                        break;

                }

               
            }
            catch
            {

            }
            imgRGB.Dispose();
            imgNIR.Dispose();

        }
        /// <summary>
        /// 
        /// </summary>     
       
        
        /// <summary>
        /// 
        /// </summary>
        public void VisualizaResultado()
        {
            
            //visualizar resultado                        
            try
            {
                //visualiza el resultado de la clasificacion
                SetTextCallback d = new SetTextCallback(SetResults);
                this.Invoke(d, new object[] { AdquisitionProcesado.especies });

            }
            catch  { }
            //Ver ancho, largo y área.
           
        }
           
        /// <summary>
        /// 
        /// </summary>
        /// <param name="especies"></param>
        private void SetResults(List<Especie> especies)
        {

            lblLargo.Text = InterfacetoVisializerParams.largo.ToString();
            lblAncho.Text = InterfacetoVisializerParams.ancho.ToString();
            lblArea.Text = InterfacetoVisializerParams.area.ToString();
            lblProccesingTime.Text = InterfacetoVisializerParams.time_proccess.ToString();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TextoOut"></param>
        private void SetText(string TextoOut, bool Appendtext)
        {
            if (Appendtext == false)
                f.Text = TextoOut;
            else
                f.Text += TextoOut;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="especies"></param>
        delegate void SetTextCallback(List<Especie> especies);
        delegate void SetTextOutCallback(string textoOut, bool Appendtext);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="textOut"></param>
        public void PrintOutText(string textOut, bool Appendtext = false)
        {
            //visualiza el resultado de la clasificacion
            SetTextOutCallback d = new SetTextOutCallback(SetText);
            this.Invoke(d, new object[] { textOut, Appendtext });
        }
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="opcion"></param>
        private void SaveButtonstate(bool opcion)
        {

            if (opcion)
            {
                //Inhabilitar botones de guardar imágenes
                cbGuardar0.Enabled = false;
                cbGuardar1.Enabled = false;
                cbGuardar2.Enabled = false;
                bttModifyFishPath.Enabled = false;

            }
            else
            {
                //Habilitar botones de guardar imágenes
                cbGuardar0.Enabled = true;
                cbGuardar1.Enabled = true;
                cbGuardar2.Enabled = true;
                bttModifyFishPath.Enabled = true;
            }

        }
        
        /// <summary>
        /// 
        /// </summary>
        public void InitVisualizacion()
        {
            //Imágenes en BRUTO
            WindowRGB0 = hWinCtrRGB0.HalconWindow;
            WindowNIR0 = hWinCtrNIR0.HalconWindow;

            //Imágenes PREPROCESADAS
            WindowRGB1 = hWinCtrRGB1.HalconWindow;
            WindowNIR1 = hWinCtrNIR1.HalconWindow;

            //Imágenes PEZ COMPLETO
            WindowRGB2 = hWinCtrRGB2.HalconWindow;
            WindowNIR2 = hWinCtrNIR2.HalconWindow;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bttIniciar_Click(object sender, EventArgs e)
        {
            //iniciliza los ficheros de configuracion
            PrintOutText(""); //inicializa la salida de texto     
            bttIniciar.Enabled = false;      
            txtCameraDelay.Enabled = false;
            txtVelocidadCinta.Enabled = false;
            bttVelocidadCamara.Enabled = false;
            SaveButtonstate(true);
            //inicializa el modulo de adquisicion
            AdquisitionProcesado.LoadIOConf();            
            //activar la captura simulada
            AdquisitionProcesado.simulated = true;
            if (txtPathSEQ.Text == "")
                AdquisitionProcesado.simulated = false;
            AdquisitionProcesado.CrearPathsGuardar();
            AdquisitionProcesado.IniciarEngineHdev();
            //inicializa los controles de visualizacion
            InitVisualizacion();            
        }
        /// <summary>
        /// 
        /// </summary>
        private void PararTodo()
        {
            //Cuando esté parado, iniciarlo con src.Cancel(false);
            AdquisitionProcesado.PararTodo();
            //Iniciar true
            this.Invoke((MethodInvoker)delegate
            {
                bttIniciar.Enabled = true;           
            });
        }
        /// <summary>
        /// 
        /// </summary>
        private void LimpiarTodo()
        {
            AdquisitionProcesado.LimpiarTodo();

            this.Invoke((MethodInvoker)delegate
            {
                lblContImg0.Text = AdquisitionProcesado.cont_imagenesAdquisicion.ToString();
                lblContImg1.Text = AdquisitionProcesado.cont_imagenesPreprocesadas.ToString();
                lblContImg2.Text = AdquisitionProcesado.cont_imagenesPezCompleto.ToString();
                lblContImg2Wrong.Text = AdquisitionProcesado.cont_imagenesPezbuffer.ToString();
            });         
            //Limpiar las imágenes
            try
            {
                WindowRGB0.ClearWindow();       // imagen RGB en bruto
                WindowNIR0.ClearWindow();       // imagen NIR en bruto
                WindowRGB1.ClearWindow();       // imagen RGB corregida
                WindowNIR1.ClearWindow();       // imagen NIR corregida
                WindowRGB2.ClearWindow();       // imagen RGB pez completo
                WindowNIR2.ClearWindow();       // imagen NIR pez completo
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bttStop_Click(object sender, EventArgs e)
        {
            PararTodo();
            AdquisitionProcesado.StopFramegrabbers();      
            LimpiarTodo();
            AdquisitionProcesado.primera_vez    = true;
            //Habilitar botones de la velocidad de la cinta
            txtVelocidadCinta.Enabled           = true;
            txtCameraDelay.Enabled              = true;
            bttVelocidadCamara.Enabled          = true;
            //Habilitar botones de guardar imágenes
            cbGuardar0.Enabled                  = true;
            cbGuardar1.Enabled                  = true;
            cbGuardar2.Enabled                  = true;
            bttModifyFishPath.Enabled           = true;
            txtPathSEQ.Text = "";
          
        }     
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bttPausar_Click(object sender, EventArgs e)
        {
            if (bttPausar.Text == "Pause")
            {
                bttPausar.Text = "Continue";
                // Pausar Captura
                AdquisitionProcesado._pausarCaptura.Reset();
            }
            else
            {
                bttPausar.Text = "Pause";
                // Reanudar Captura
                AdquisitionProcesado._pausarCaptura.Set();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bttModifyFishPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            DialogResult res = dlg.ShowDialog();

            if (res == DialogResult.OK)
            {
                AdquisitionProcesado.pathCam1 = dlg.SelectedPath + "\\";
                AdquisitionProcesado.pathToSave = "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bttExaminarSEQ_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "seq";
            dlg.ValidateNames = true;

            dlg.Filter = "Halcon Sequence (*.seq)|*.seq";
            DialogResult res = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                string aux1 = dlg.FileName.Substring(dlg.FileName.LastIndexOf('\\') + 1);
                string aux2 = dlg.FileName.Substring(0, dlg.FileName.LastIndexOf('\\'));
                string aux3 = aux2.Substring(aux2.LastIndexOf('\\') + 1);
                //txtPathSEQ.Text = aux3 + "/" + aux1;
                txtPathSEQ.Text = dlg.FileName;
                AdquisitionProcesado.path_simularCaptura = txtPathSEQ.Text;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtVelocidadCinta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            AdquisitionProcesado.velocidadCinta_o_sleepadquisicion = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCameraDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            AdquisitionProcesado.velocidadCinta_o_sleepadquisicion = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bttVelocidadCamara_Click(object sender, EventArgs e)
        {
            if (AdquisitionProcesado.velocidadCinta_o_sleepadquisicion)
            {
                // A 1m/s se sacan fotos saca 225 ms
                if (txtVelocidadCinta.Text.StartsWith("."))
                    txtVelocidadCinta.Text = "0" + txtVelocidadCinta.Text;

                if (Convert.ToDouble(txtVelocidadCinta.Text.Replace('.', ',')) > 5 || Convert.ToDouble(txtVelocidadCinta.Text.Replace('.', ',')) < 0.1)
                    txtVelocidadCinta.Text = "1.0";

                double vel_cinta = Convert.ToDouble(txtVelocidadCinta.Text.Replace('.', ','));
                AdquisitionProcesado.SPEED_ADQUISICION = vel_cinta;
                AdquisitionProcesado.SLEEP_ADQUISICION = (int)Math.Round( 225 / vel_cinta);
                txtCameraDelay.Text = AdquisitionProcesado.SLEEP_ADQUISICION.ToString();
            }
            else
            {
                //Desde 100ms hasta 1000ms
                if (Convert.ToInt32(txtCameraDelay.Text) >= 25 || Convert.ToInt32(txtCameraDelay.Text) <= 1000)
                {
                    AdquisitionProcesado.SLEEP_ADQUISICION = Convert.ToInt32(txtCameraDelay.Text);
                    txtVelocidadCinta.Text = (225.0 / AdquisitionProcesado.SLEEP_ADQUISICION).ToString();
                    AdquisitionProcesado.SPEED_ADQUISICION = (225.0 / AdquisitionProcesado.SLEEP_ADQUISICION);
                }
                else
                {
                    txtVelocidadCinta.Text = "1.0";
                    txtCameraDelay.Text = "225";
                }
            }

        }      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bttGoToPath_Click(object sender, EventArgs e)
        {
            string path = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + AdquisitionProcesado.pathToSave + AdquisitionProcesado.pathCam1;
            System.Diagnostics.Process.Start(path);
        }

        //Parar todos los hilos al cerrar
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!bttIniciar.Enabled)
            {
                string cabecera = "Information";
                string dialogo = "Pleases stop Image Acquisition first.";
                DialogResult result = MessageBox.Show(dialogo, cabecera, MessageBoxButtons.OK, MessageBoxIcon.Information);
                e.Cancel = true;
                return;
            }

            try
            {
                AdquisitionProcesado.threadAdquisicion.Abort();
                AdquisitionProcesado.threadAdquisicion.Join();
                AdquisitionProcesado.threadPreprocesado.Abort();
                AdquisitionProcesado.threadPreprocesado.Join();
                AdquisitionProcesado.threadMosaikingContorno.Abort();
                AdquisitionProcesado.threadMosaikingContorno.Join();
            }
            catch { }
        }      
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //guarda la los resultados en el fichero csv que le corresponde

            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "Contadores_especies.csv";
            // set filters - this can be done in properties as well
            savefile.Filter = "CSV (Comma Separated Values) (*.csv)|*.csv";
            //Guardar características de cada pez
            StreamWriter swcsv = null;
            if (savefile.ShowDialog() == DialogResult.OK)
            {            
                swcsv = new StreamWriter(savefile.FileName);
                //si es cero el numero de especies no hace nada
                int numespecies = AdquisitionProcesado.especies.Count;
                swcsv.WriteLine("Especie; codigo especie; número de unidades especie; numero total ejemplares");
                if (AdquisitionProcesado.cont_imagenesPezCompletoUnknown > 0)
                {
                    string texto = "Unknown" + ";" + "----" + ";" + AdquisitionProcesado.cont_imagenesPezCompletoUnknown.ToString() + ";" + AdquisitionProcesado.cont_imagenesPezCompleto.ToString();
                    swcsv.WriteLine(texto);
                }

                for (int i = 1; i <= numespecies; i++)
                {
                    string texto = AdquisitionProcesado.especies[i - 1].nombre + ";" + AdquisitionProcesado.especies[i - 1].codigo + ";" + AdquisitionProcesado.especies[i - 1].contador.ToString() + ";" + AdquisitionProcesado.cont_imagenesPezCompleto.ToString();
                    swcsv.WriteLine(texto);

                }
                if (swcsv != null)
                    swcsv.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            PrintOutText("");
            //si es cero el numero de especies no hace nada
            int numespecies = AdquisitionProcesado.especies.Count;
            for (int i = 1; i <= numespecies; i++)
            {
                AdquisitionProcesado.especies[i - 1].contador = 0;
            }
            AdquisitionProcesado.cont_imagenesPezCompleto = 0;
            AdquisitionProcesado.cont_imagenesPezCompletoUnknown = 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSimulate_Click(object sender, EventArgs e)
        {         
            OpenFileDialog dlg  = new OpenFileDialog();
            dlg.DefaultExt      = "seq";
            dlg.ValidateNames   = true;
            dlg.Filter          = "Halcon Sequence (*.seq)|*.seq";
            DialogResult res    = dlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                string aux1     = dlg.FileName.Substring(dlg.FileName.LastIndexOf('\\') + 1);
                string aux2     = dlg.FileName.Substring(0, dlg.FileName.LastIndexOf('\\'));
                string aux3     = aux2.Substring(aux2.LastIndexOf('\\') + 1);        
                txtPathSEQ.Text = dlg.FileName;
                AdquisitionProcesado.path_simularCaptura = txtPathSEQ.Text;
            }
        }

        private void AdquisitionForm_Shown(object sender, EventArgs e)
        {
            bttIniciar_Click(sender, e);
        }
        //crea el fichero seq para la simulacio a partir de las imagens que tiene en el path
        private void button_create_Seq_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            DialogResult        result = folderBrowserDialog1.ShowDialog();
            string              folderName = "";
           
            if (result == DialogResult.OK)
            {
                folderName = folderBrowserDialog1.SelectedPath;
                StoreToFileSeq(folderName);

            }
        }


        /// <summary>
        /// adaptarlo para escribir arff
        /// https://weka.wikispaces.com/ARFF
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="features"></param>
        /// <returns></returns>
        public static bool StoreToFileSeq(string folderName)
        {
           
            HTuple      RutasImagenes       = "";
            HTuple      RutasImagenesBase   = "";        
            string      filename            = "";
            string      PathRGBFile         = "";
            string      PathNIRFile         = "";


            try
            {

               
                string[] filePaths = Directory.GetFiles(folderName,"*_RGB_*",SearchOption.TopDirectoryOnly);
                if (filePaths.Length == 0)
                    filePaths = Directory.GetFiles(folderName, "RGB_*", SearchOption.TopDirectoryOnly);

                NumericComparer ns = new NumericComparer();
                Array.Sort(filePaths, ns);
        
                filename = folderName + "\\imagenes.seq";
                //si existe lo borrar  
                if (File.Exists(filename))
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(filename);                   
                     fi.Delete();
                  
                }

                //escribe los datos
                string line = "";
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filename, true))
                {
                    for ( int i = 0; i < filePaths.Length; i++ )
                    {
                        PathRGBFile = filePaths[i];
                        PathNIRFile = PathRGBFile.Replace("RGB_", "NIR_");
                        line = PathNIRFile;
                        file.WriteLine(line);
                        line = PathRGBFile;
                        file.WriteLine(line);
                    }
                    file.Close();
                }       
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("There was an error storing " + filename + " file. [" + e.Message + "]");
            }
        
         }
        /// <summary>
        /// 
        /// </summary>
        public class NumericComparer : IComparer
        {
            public NumericComparer()
            { }

            public int Compare(object x, object y)
            {
                if ((x is string) && (y is string))
                {
                    return StringLogicalComparer.Compare((string)x, (string)y);
                }
                return -1;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public class StringLogicalComparer
        {
            public static int Compare(string s1, string s2)
            {
                //get rid of special cases
                if ((s1 == null) && (s2 == null)) return 0;
                else if (s1 == null) return -1;
                else if (s2 == null) return 1;

                if ((s1.Equals(string.Empty) && (s2.Equals(string.Empty)))) return 0;
                else if (s1.Equals(string.Empty)) return -1;
                else if (s2.Equals(string.Empty)) return -1;

                //WE style, special case
                bool sp1 = Char.IsLetterOrDigit(s1, 0);
                bool sp2 = Char.IsLetterOrDigit(s2, 0);
                if (sp1 && !sp2) return 1;
                if (!sp1 && sp2) return -1;

                int i1 = 0, i2 = 0; //current index
                int r = 0; // temp result
                while (true)
                {
                    bool c1 = Char.IsDigit(s1, i1);
                    bool c2 = Char.IsDigit(s2, i2);
                    if (!c1 && !c2)
                    {
                        bool letter1 = Char.IsLetter(s1, i1);
                        bool letter2 = Char.IsLetter(s2, i2);
                        if ((letter1 && letter2) || (!letter1 && !letter2))
                        {
                            if (letter1 && letter2)
                            {
                                r = Char.ToLower(s1[i1]).CompareTo(Char.ToLower(s2[i2]));
                            }
                            else
                            {
                                r = s1[i1].CompareTo(s2[i2]);
                            }
                            if (r != 0) return r;
                        }
                        else if (!letter1 && letter2) return -1;
                        else if (letter1 && !letter2) return 1;
                    }
                    else if (c1 && c2)
                    {
                        r = CompareNum(s1, ref i1, s2, ref i2);
                        if (r != 0) return r;
                    }
                    else if (c1)
                    {
                        return -1;
                    }
                    else if (c2)
                    {
                        return 1;
                    }
                    i1++;
                    i2++;
                    if ((i1 >= s1.Length) && (i2 >= s2.Length))
                    {
                        return 0;
                    }
                    else if (i1 >= s1.Length)
                    {
                        return -1;
                    }
                    else if (i2 >= s2.Length)
                    {
                        return -1;
                    }
                }
            }

            private static int CompareNum(string s1, ref int i1, string s2, ref int i2)
            {
                int nzStart1 = i1, nzStart2 = i2; // nz = non zero
                int end1 = i1, end2 = i2;

                ScanNumEnd(s1, i1, ref end1, ref nzStart1);
                ScanNumEnd(s2, i2, ref end2, ref nzStart2);
                int start1 = i1; i1 = end1 - 1;
                int start2 = i2; i2 = end2 - 1;

                int nzLength1 = end1 - nzStart1;
                int nzLength2 = end2 - nzStart2;

                if (nzLength1 < nzLength2) return -1;
                else if (nzLength1 > nzLength2) return 1;

                for (int j1 = nzStart1, j2 = nzStart2; j1 <= i1; j1++, j2++)
                {
                    int r = s1[j1].CompareTo(s2[j2]);
                    if (r != 0) return r;
                }
                // the nz parts are equal
                int length1 = end1 - start1;
                int length2 = end2 - start2;
                if (length1 == length2) return 0;
                if (length1 > length2) return -1;
                return 1;
            }

            //lookahead
            private static void ScanNumEnd(string s, int start, ref int end, ref int nzStart)
            {
                nzStart = start;
                end = start;
                bool countZeros = true;
                while (Char.IsDigit(s, end))
                {
                    if (countZeros && s[end].Equals('0'))
                    {
                        nzStart++;
                    }
                    else countZeros = false;
                    end++;
                    if (end >= s.Length) break;
                }
            }

        }

        ///

    }

}
