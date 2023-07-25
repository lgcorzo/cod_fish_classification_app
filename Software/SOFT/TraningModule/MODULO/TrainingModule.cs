using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AdquisitionModule;
using HalconDotNet;
using System.IO;
using ProcesadoEspecies;
using TwincatModule;
using TrainingModule.CLASES;
using System.Threading;
using WCFModule;

namespace TrainingModule
{
    /// <summary>
    /// 
    /// </summary>
    /// 
   

    public class TrainingModule : csr.modules.CSRFormModule
    {
      
        bool                save                    = false;     
        bool                automatic               = false;
        bool                visualizar              = false;
        bool                forzar_categoria        = false;
        int                 Numberofprocessors      = Environment.ProcessorCount;
        int                 numProductoTotal        = 0;
        int                 categoria_forzada       = 0;
        string              pathToSave              = "Modelos\\";
        string              pathToModeloA           = "Modelos\\clasificador_A\\";
        string              Output_text             = "OK";
        string              clasification_path      = "";
        List<Features>      Instancias              = null;
        Thread              hiloProcesado           = null;
        ManualResetEvent    StartProcesadoEvent     = new ManualResetEvent(false);
        ManualResetEvent    StopProcesadoEvent      = new ManualResetEvent(false);    
        Procesado           procesado               = new Procesado();   
        DateTime            Ejecucion               = DateTime.Now;     
        DateTime            imagen_anterior_tiempo  = DateTime.Now;
        private Object      thisLock                = new Object();
        int                 counter_change          = 0;
        //check if the clasification files has changed
        private System.IO.FileSystemWatcher         m_Watcher;
       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_id"></param>
        public TrainingModule(string _id)
            : base(_id)
        { }

       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>
        public override bool Init()
        {         
            base.Init();
            //acceso a los modulos necesarios
            AdquisitionModule.AdquisitionModule Adquisition = (AdquisitionModule.AdquisitionModule)GetModule("Adquisicion");
          

            ModuleDelegates module_delegates    = new ModuleDelegates(ChangeSave,              ChangeFolderPath,       ExecuteFromFolder,  ChangeClasificationModel,
                                                                    Entrenarclasificador,     ResetContadores,    ChangeImagePath);
            WindowForm                          = new TrainingForm(this,    module_delegates);               
            
            ((TrainingForm)WindowForm).Change_clasification_module(1);

            imagen_anterior_tiempo  = DateTime.Now;       
            Ejecucion               = DateTime.Now;
            startWatcher();
            WriteConsole("Módulo cargado correctamente.", true);

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Destroy()
        {
            m_Watcher.EnableRaisingEvents = false;
            m_Watcher.Dispose();
            StopProcesado();
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public void startWatcher()
        {
          
            m_Watcher       = new System.IO.FileSystemWatcher();
            m_Watcher.Filter = "append_Appended_1_sets.arff";
            m_Watcher.Path = pathToModeloA;
            m_Watcher.NotifyFilter =  NotifyFilters.LastWrite;
            m_Watcher.Changed += new FileSystemEventHandler(OnChanged);
            //m_Watcher.Created += new FileSystemEventHandler(OnChanged);
            //m_Watcher.Deleted += new FileSystemEventHandler(OnChanged);
            //m_Watcher.Renamed += new RenamedEventHandler(OnRenamed);
            m_Watcher.EnableRaisingEvents = true;
          
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            counter_change++;
            //espera 10 segundos a que copie todo
            if (counter_change == 3)
            {
                Thread.Sleep(5000);
                SendMessage("Vision", "NEW_CLASSIFICATOR_DETECTED");
                //Thread.Sleep(5000);
                //ChangeClasificationModel(clasification_path);
                counter_change = 0;
            }



        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        public  List<string> Get_Class_names()
        {        
            return procesado.Get_Class_names();
        }       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public  void HandleMessagesThread(csr.com.CSRMessage message)
        {
            string strMessage = message.ToString();
            
        }
        /// <summary>
        /// Process the messages sended by the CSR modules
        /// </summary>
        /// <param name="message"></param>
        public override void HandleMessages(csr.com.CSRMessage message)
        {                    
        }       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img1RGB"></param>  
        /// <param name="img1NIR"></param> 
        public void ProcesarImagenes(HImage img1RGB, HImage img1NIR, double ScaleX, double ScaleY, List<Sample> Samples, ImagesToProcess imagenes, long ID)
        {
            HRegion Defectosdisp    = null;           
            HImage img1RGBdisp      = null;       
            HImage img1NIRdisp      = null;
            HRegion Regions1        = img1RGB.GetDomain();
            HRegion Regions2        = img1NIR.GetDomain();  
         
       
            try
            {           
                //porcesado de las imagenes
                string outputtextsegmentacion = procesado.ExecuteSegmentacion(img1RGB, img1NIR,  ScaleX, ScaleY,  Samples,out  Regions1, out  Regions2);
                //escribe en la salida el texto de salida del porcesado
                //WriteConsole(outputtextsegmentacion, true);
                //genera una lista de imagenes para enviar al procesar por textura              
                try
                {
                    //escribe el area de las regiones calculadas
                    string areastrng = "Areas: " + Regions1.Area.ToString() + " , " + Regions2.Area.ToString();
                    //WriteConsole(areastrng, true);
                    imagenes.img1RGB = img1RGB.ReduceDomain(Regions1);           
                    imagenes.img1NIR = img1NIR.ReduceDomain(Regions2);  
                }
                catch
                {                 
                    //imagen enviada sin roi            
                    WriteConsole("fallo en los rois", true);
                    Regions1 = img1RGB.GetDomain();
                    Regions2 = img1NIR.GetDomain();              
                }         
                DateTime    Tinicialremote      = DateTime.Now;           
                DateTime    Tfinalremote        = DateTime.Now;
                TimeSpan    timeremote;
                timeremote                  = Tfinalremote - Tinicialremote;          
                string outputtextProcess    = procesado.ExecuteProcess(img1RGB, img1NIR, ScaleX, ScaleY, Samples, Regions1,  Regions2);           
                try
                {
                   
                    Samples[0].Features.addFeature("ID",0.0, ID);
                }
                catch
                { }                               
                //escribe en la salida el texto de salida del porcesado
                //WriteConsole(outputtextProcess, true);                       
            }
            catch
            {
                //imagen enviada sin roi            
                WriteConsole("fallo en la segmentacion", true);
                Regions1 = img1RGB.GetDomain();
                Regions2 = img1NIR.GetDomain();
         
            }       
            if (visualizar)
            {
                img1RGBdisp = img1RGB.ReduceDomain(Regions1);          
                img1NIRdisp = img1NIR.ReduceDomain(Regions2);            
                try
                {
                    ((TrainingForm)WindowForm).DispCounter(numProductoTotal, Output_text);
                    ((TrainingForm)WindowForm).DispImages(img1RGBdisp, img1NIRdisp , 0, Defectosdisp);                 
                }
                catch 
                {
                }              
            }

            if (Defectosdisp != null)
                Defectosdisp.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        public void ResetContadores()
        {          
                numProductoTotal                        = 0;
                procesado.NumProcesamientos             = 0;
                procesado.NumProcesamientosTextura      = 0;
                procesado.NumProcesamientosSegmentacion = 0;    
        }               
       
    
        /// <summary>
        /// comunica el resultado al ordenador central
        /// </summary>
        /// <param name="FeaturesProducto"></param>
        /// <param name="taxi"></param>
        /// <param name="linea"></param>
        private void ComunicaResultado(Features FeaturesProducto, int taxi, string Nombre_variable, out int categoriaout)
        {
            int     categoria                   = 10; //por defecto 10 fallo procesado             
                    categoriaout                = 0;
            try
            {
                List<string> namesToSelect = new List<string>();
                List<Feature> FeatureToSelect = new List<Feature>();            
                namesToSelect.Add("Categoria_asignada");
                FeaturesProducto.Select(namesToSelect);
                FeatureToSelect = FeaturesProducto.GetSelected();
                categoria = Convert.ToInt16(FeatureToSelect[0].Value);            
                FeaturesProducto.addFeature("Categoria_asignada", categoria);

                //escribe el resultado
                switch (categoria)
                {
                    case -2:
                        categoria = 10; //por defecto 10 fallo procesado
                        WriteConsole("fallo en la decision modelo no correcto", true);
                        break;
                    case -1:
                        categoria = 10; //por defecto 10 fallo procesado
                        WriteConsole("fallo en el procesado", true);
                        break;
                    default:
                        break;
                }

                if (forzar_categoria == true)
                {
                    FeaturesProducto.addFeature("Categoria_Forzada", categoria_forzada);
                    categoria = categoria_forzada;
                }
                   
            }
            catch
            {
                categoria = 10; //por defecto 10 fallo procesado       
                WriteConsole("fallo en el procesado",true);
            }

            categoriaout = categoria;
       
            //comunicacion si o si      
            int count = 0;
           
                //comunicacion del resultado
                
             count++;
            if (forzar_categoria == true)
                categoria = categoria_forzada;
       }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="save"></param>
        private void ChangeSave(bool save, int categoria, bool automatic, bool visualizar, bool forzar_categoria, int categoria_forzada)
        {
            this.save = save;
            //this.categoria = categoria;
            this.automatic = automatic;
            this.visualizar = visualizar;
            this.categoria_forzada = categoria_forzada;
            this.forzar_categoria = forzar_categoria;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private void ChangeFolderPath(string path)
        {
            pathToSave = path;
            procesado.CargarImagenes(path);
            string pathArff = pathToSave + "\\Instancias_imagenes.arff";
            Features.LoadArff(pathArff, out Instancias);
            ((TrainingForm)WindowForm).setListInstancias(Instancias);
            ((TrainingForm)WindowForm).UpdateDataGridView2();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        private HTuple ChangeImagePath(long ID)
        {        
            HTuple PathImagenselected = procesado.SetPathImagenen_ID(ID);
            return PathImagenselected;

        }
        /// <summary>
        /// Carga el modelo de clasifacion
        /// </summary>
        /// <param name="path"></param>
        private void Entrenarclasificador(string trainingDatasetFilePath, string savePath)
        {
            try { procesado.Entrenarclasificador( trainingDatasetFilePath,  savePath);
                WriteConsole("Modelo de clasificacion generado correctamente",true);
            }
            catch
            {
                WriteConsole("Modelo de clasificacion no generado correctamente",true);
            }
            
        }

        /// <summary>
        /// Carga el modelo de clasifacion
        /// </summary>
        /// <param name="path"></param>
        private List<string> ChangeClasificationModel(string path)
        {
            string output;
            try
            {
                clasification_path = path;
                output =  procesado.CargarModeloClasificacion(path);
               // WriteConsole(output, true);
                return procesado.Get_Class_names();


            }
            catch 
            {
                WriteConsole("Modelo de clasificacion no cargado correctamente",true);
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void ExecuteFromFolder()
        {

            StartProcesado();

        }

        /// <summary>
        /// 
        /// </summary>
        private void ExecuteFromFolderThread()
        {
            //activar para leer las imagenes  
            bool    test_captura            = false;
            int     linea                   = 0;
            double  ScaleX, ScaleY;
            HImage Image1RGB                = null;      
            HImage  Image1NIR               = null;
            TimeSpan interval;
            DateTime Tinicio                = DateTime.Now;         
            long     ID                     = 0;
            int categoriaout                = 0;
      
            //ejecuta todas las imagames a la vez
            for (int i = 0; i < procesado.numero_imagenes_folder; i++)
            {


                if (StopProcesadoEvent.WaitOne(5, true))
                {
                    StopProcesadoEvent.Reset();
                    StartProcesadoEvent.Reset();
                    return;
                }
                    int categoriaread   = 0;
                    bool forzada        = false;
                    bool lectura_ok     = false;

                if (save)
                     lectura_ok = procesado.LeerImagenes(out Image1RGB, out Image1NIR, Instancias, out ScaleX, out ScaleY, out linea, out ID, out forzada, out categoriaread);
                else
                    lectura_ok = procesado.LeerImagenes(out Image1RGB, out Image1NIR, null, out ScaleX, out ScaleY, out linea, out ID, out forzada, out categoriaread);

                if (lectura_ok)
                    {

                        if (forzada == true)
                        {
                            categoria_forzada = categoriaread;
                        }
                        forzar_categoria = forzada;

                        //calculo de la escala de las imagenes             
                        test_captura = false;
                        if (test_captura == false)
                        {
                            List<Sample> Samples;
                            Samples = new List<Sample>();
                            ImagesToProcess imagenestextura = new ImagesToProcess();
                            imagenestextura.img1NIR = Image1NIR;
                            imagenestextura.img1RGB = Image1RGB;
                            imagenestextura.Linea   = 0;
                            numProductoTotal++;
                            //imagenes procesadas
                            ProcesarImagenes(Image1RGB, Image1NIR, ScaleX, ScaleY, Samples, imagenestextura, ID);
                            try
                            {
                                ComunicaResultado(Samples[0].Features, 0, null, out categoriaout);
                                if (visualizar)
                                {
                                    if (Samples.Count() > 0)
                                    {
                                        Features features = Samples[0].Features;
                                        ((TrainingForm)WindowForm).UpdateDataGridView1(features);
                                    }
                                }
                            }
                            catch
                            { }
                            if (save)
                            {
                                try
                                {
                                    string PathArff = pathToSave + "\\Instancias_imagenes.arff";
                                    //escribo el fichero solo si ha funcionado
                                    Features.StoreToFileArff(PathArff, Samples[0].Features, procesado.Get_Class_names());
                                    Instancias.Add(Samples[0].Features);
                                    ((TrainingForm)WindowForm).setListInstancias(Instancias);
                                    ((TrainingForm)WindowForm).UpdateDataGridView2();
                                }
                                catch { }
                            }
                            Samples.Clear();
                            Samples = null;
                            imagenestextura.Dispose();
                        }
                    }
                             
                DateTime Tfinal = DateTime.Now;
                interval        = Tfinal - Tinicio;
                Output_text     = interval.TotalMilliseconds.ToString() + " mseg ";
                if (this.automatic == false)
                    break;
            }


        }
        #region HILO PROCESADO
        public void StopProcesado()
        {
            try
            {
                StartProcesadoEvent.Reset();
                StopProcesadoEvent.Set();
            }
            catch (Exception e)
            {
                throw (e);
            }
        }

        public void StartProcesado()
        {
            try
            {

                //HILO
                if ((hiloProcesado != null) && (hiloProcesado.ThreadState == ThreadState.Running))
                {
                    hiloProcesado.Abort();
                }
                hiloProcesado = new Thread(() => Procesado());
                hiloProcesado.Priority = ThreadPriority.Normal;
                hiloProcesado.IsBackground = true;

                hiloProcesado.Start();

            }
            catch 
            {
                throw ;
            }
        }

        private void Procesado()
        {
           
                StartProcesadoEvent.Set();
                StopProcesadoEvent.Reset();


            try
            {
                TimeSpan interval;
                DateTime Tinicio = DateTime.Now;

                ExecuteFromFolderThread();


                DateTime Tfinal = DateTime.Now;
                interval = Tfinal - Tinicio;
            }

            catch 
            {
                throw;
            }
       
            
           
        }
        #endregion
    }

    //////////////////////////////////////////////////////////////

    public delegate void ChangeSaveDelegate(bool save, int categoria, bool automatic, bool visualizar, bool forzar_categoria, int categoria_forzada);
    public delegate void ChangeFolderPathDelegate(string path);
    public delegate void ExecuteFromFolderDelegate();
    public delegate List<string>  CargaModeloClasificacion(string path);
    public delegate void GenerarModeloClasificacion(string patharff, string pathmodelo);
    public delegate void ResetContadores();
    public delegate HTuple ChangeImagePath(long ID);
    //////////////////////////////////////////////////////////////
    public class ModuleDelegates
    {

        public ChangeSaveDelegate ChangeSave;
        public ChangeFolderPathDelegate ChangeFolderPath;
        public ExecuteFromFolderDelegate ExecuteFromFolder;
        public CargaModeloClasificacion CargaModeloClasificacion;
        public GenerarModeloClasificacion GenerarModeloClasificacion;
        public ResetContadores ResetContadores;
        public ChangeImagePath ChangeImagePath;


        //////////////////////////////////////////////////////////////
        public ModuleDelegates( ChangeSaveDelegate pChangeSave, ChangeFolderPathDelegate pChangeFolderPath, ExecuteFromFolderDelegate pExecuteFromFolder, CargaModeloClasificacion pCargaModeloClasificacion,
            GenerarModeloClasificacion pGenerarModeloClasificacion, ResetContadores pResetContadores, ChangeImagePath pChangeImagePath)
        {
            ChangeSave                  = pChangeSave;
            ChangeFolderPath            = pChangeFolderPath;
            ExecuteFromFolder           = pExecuteFromFolder;
            CargaModeloClasificacion    = pCargaModeloClasificacion;
            GenerarModeloClasificacion = pGenerarModeloClasificacion;
            ResetContadores             = pResetContadores;
            ChangeImagePath             = pChangeImagePath;

        }

        //////////////////////////////////////////////////////////////
       


    }

}
