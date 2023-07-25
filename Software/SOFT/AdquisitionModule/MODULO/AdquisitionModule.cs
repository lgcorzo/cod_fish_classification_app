using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Camera;
using HalconDotNet;
using System.Timers;
using AdquisitionModule.CLASES;
using System.Xml.Serialization;
using System.Collections.Concurrent;
using TwincatModule;
using ProcesadoEspecies;


namespace AdquisitionModule
{
    //funciones delegadas
    public delegate void                    PrintOutTextDelegate(string Texto, bool append = false);
    public delegate void                    DispImagesDelegate(ElementosToProcess obj, RegionVisualizar modo);
    public delegate void                    DispResultsDelegate(FormInterface paramsform);
    public delegate FormInterface           GetResultsParamsDelegate();
    public delegate void                    VisualizaResultadoDelegate();
    public delegate void                    VisualizarLabelsDelegate();
    //evento de imagen capturada
    public delegate void NewImages(ImagesToProcess images);
    public delegate void NewImagesRaw(ImagesToProcess images);

    public class ModuleDelegates
    {
        public PrintOutTextDelegate         PrintOutText;
        public DispImagesDelegate           DispImages;
        public DispResultsDelegate          DispResults;
        public GetResultsParamsDelegate     GetResultsParams;
        public VisualizaResultadoDelegate   VisualizaResultado;
     
        //////////////////////////////////////////////////////////////
        public ModuleDelegates(PrintOutTextDelegate pPrintOutText, DispImagesDelegate pDispImages, DispResultsDelegate pDispResults,
            GetResultsParamsDelegate pGetResultsParams, VisualizaResultadoDelegate pVisualizaResultado)
        {
            PrintOutText        = pPrintOutText;
            DispImages          = pDispImages;
            DispResults         = pDispResults;
            GetResultsParams    = pGetResultsParams;
            VisualizaResultado  = pVisualizaResultado;
          
        }
    }

    public class AdquisitionModule : csr.modules.CSRFormModule
    {
        //variables para el control de las camaras
        private HDevelopExport HalconProcedures             = new HDevelopExport();
        private ulong   time_last_image                     = 0;
        private FormInterface InterfacetoVisializerParams = new FormInterface();
        public bool     CalibrationActive                   = true;
        public bool     OneShootImage                       = false;
        public bool     simulated                           = false;     
        public bool     clasificadorCargado                 = false;
        public bool     HDevelopEngineIniciado              = false;
        public bool     soloRGB                             = false;    //Si la cámara es "JAI BB-500GE" (RGB) o es "JAI AD-130GE" (RGB + NIR)                    
        public bool     primera_vez                         = true;
        public bool     velocidadCinta_o_sleepadquisicion   = true;
        public bool     guardar_imagen_white                = false;
        public string   pathToSave                          = "Camera\\";
        public string   pathCam1                            = "Adquisition\\";
        public string   pathSimulation_images               = "conf\\Patrones\\RAW_IMAGE\\imagenes.seq";
        public string   path_simularCaptura                 = "";
        public string   path_halcon                         = "";
        public string   PATH_IMAGEN_RGB_WHITE               = "";
        public string   PATH_IMAGEN_NIR_WHITE               = "";
        public string   pathImagen0, pathImagen1, pathImagen2;
        public List<string> clasifier_types                 = new List<string>();       
        //Guardar imágenes
        public int      cont_imagenesAdquisicion            = 0;
        public int      cont_imagenesPreprocesadas          = 0;
        public int      cont_imagenesPezCompleto            = 0;
        public int      cont_imagenesPezCompletoUnknown     = 0;
        public int      cont_imagenesPezbuffer              = 0;
        public int      SLEEP_ADQUISICION                   = 225; //ms
        public double   SPEED_ADQUISICION                   = 1.0; //ms
        public int      SALTO                               = 1;     // se coge para preprocesar una de cada SALTO imágenes 
        public int      BUFFERSIZE                          = 50;     // se coge para preprocesar una de cada SALTO imágenes     
        UInt16          counter_fish                        = 0;
        //ADQUISICION
        public HTuple   hv_AcqHandleRGB                     = null;
        public HTuple   hv_AcqHandleNIR                     = null;
        public HTuple   hv_CameraParameters                 = null;
        public HTuple   hv_CameraPose                       = null;
        public HObject  ho_ImageNIR                         = null;
        public HObject  ho_ImageRGB                         = null;     
        //Factores de corrección para el ancho y largo real (relacción: px/cm)
        public double   FACTOR_LARGO                        = 0.0785;
        public double   FACTOR_ANCHO                        = 0.0785;
        public event NewImages          NewImages;
        public event NewImagesRaw       NewImagesRaw;
        public ParamConf                ConfiguracionParams;
       
        //funciones delegadas
        ModuleDelegates     module_delegates;
        //Tiempo de procesado del pez, desde que se coge la primera foto hasta que se clasifica o clacula medidas                 
        public int          PosanteriorhayAlgo              = 9999999;
        //Guardar características de cada pez
       
        public List<object> clasificadores                  = new List<object>();    
        public List<Especie> especies                       = new List<Especie>();
        public List<Especie> especies_aux                   = new List<Especie>();
        public readonly Mutex mVisualizar                   = new Mutex();
        public readonly Mutex mGuardar                      = new Mutex();
        //Mutex para sacar contorno en identificacion de pez y en sacar el pez completo sin fondo
        public readonly Mutex mContorno                     = new Mutex();     
        public Camera.Camera camera1;
        public ImagesToAdquitionProcess imagenes            = new ImagesToAdquitionProcess(); 
        //Preprocesado: imagen del blanco para eliminar el defecto porducido por la iluminación
        public HImage ImagenRGBWhite                        = new HImage();
        public HImage ImagenNIRWhite                        = new HImage();
        public static HDevEngine Engine                     = new HDevEngine();
     
      
        public Procesado procesado = null;

        //Colas FIFO
        public System.Collections.Concurrent.BlockingCollection<ImagesToAdquitionProcess> fifoSalidaAdquisicon = new BlockingCollection<ImagesToAdquitionProcess>(new ConcurrentQueue<ImagesToAdquitionProcess>());
        public System.Collections.Concurrent.BlockingCollection<ElementosToProcess> fifoSalidaProcesado = new BlockingCollection<ElementosToProcess>(new ConcurrentQueue<ElementosToProcess>());
        //Hilos
        public Thread threadAdquisicionTrigger;
        public Thread threadAdquisicion;
        public Thread threadPreprocesado;
        public Thread threadMosaikingContorno;
        public ManualResetEvent _pausarCaptura              = new ManualResetEvent(true);
        public ManualResetEvent _pararCaptura               = new ManualResetEvent(true);
        public CancellationTokenSource src1                 = new CancellationTokenSource();
        public bool First_round_done                        = false;


        // Initialize local and output iconic variables 
        //imagen del blanco para eliminar el defecto porducido por la iluminación


        public AdquisitionModule(string _id)
            : base(_id)
        { }
        //////////////////////////////////////////////////////////////////////////////////
        public override bool Init()
        {
            base.Init();
            WindowForm = new AdquisitionForm(this);
            IniciarHDevelopEngine();
           
            WriteConsole("Módulo cargado correctamente.", true);        
            return true;
        }
       
        public void SetAdquisitionProcessDelegates(ModuleDelegates pdelegate)
        {
            module_delegates = pdelegate;
        }
        public override bool Destroy()
        {
            PararTodo();
            GC.Collect();
            return true;
        }
        //////////////////////////////////////////////////////////////////////////////////
        public override void HandleMessages(csr.com.CSRMessage message)
        {
           
        }
        /// <summary>
        /// Fuerza la captura de una imagen tal cual sale de la camara
        /// </summary>
        public void SoftTrigger()
        {
            OneShootImage = true;
        }

        public void First_round_precompilation_Hdev()
        {
            HImage imgRGB = new HImage();
            HImage imgNIR = new HImage();
            HFramegrabber hfEmulationAcqHandle = null;
            int counter_images = 0;
            //cargar imagenes para lanzar un pez desde la captura
            if (First_round_done == false)
            {
                try
                {
                    //ARRANCA EN EMULACION
                    // para que fucione he tenido que cambiar al framework 2.0 y cerrarlo. Luego cambiar otra vez al Framework 4.5.1, porque sino no compila.
                    hfEmulationAcqHandle = new HFramegrabber("File", 1, 1, 0, 0, 0, 0, "default", -1, "default", -1, "default", pathSimulation_images, "default", -1, -1);
                    // Pasar las nuevas imágenes a la cola
                    while (counter_images < 10)
                    {
                        HImage imgNIRsimul = hfEmulationAcqHandle.GrabImage();
                        HImage imgRGBsimul = hfEmulationAcqHandle.GrabImage();
                       
                        NewImagesFunction(imgRGBsimul, imgNIRsimul);
                        counter_images++;
                        Thread.Sleep(50);
                    }
                    Thread.Sleep(1000);
                    First_round_done = true;

                }
                catch (Exception ex)
                {
                    string textosalida = ex.ToString();
                    WriteConsole("failed to load simulation images", true);
                }
                
                First_round_done = true;
            }
 
           
        }

        public void Activatecalibration(bool activate)
        {
            CalibrationActive = activate;
        }
        /// <summary>
        /// carga el fichero de configuración
        /// </summary>
        /// <returns></returns>
        public bool LoadIOConf()
        {
            //carga el filtro
            string          filenamePath                = "conf\\Adquisition\\";
            string          filenamefullPath            = "conf\\Adquisition\\config.xml";
            ParamConf       ComParam                    = new ParamConf();
            XmlSerializer   serializerConf              = new XmlSerializer(typeof(ParamConf));
            FileStream filestreamFilter;
            if (!Directory.Exists(filenamePath))
                Directory.CreateDirectory(filenamePath);
            try
            {
                filestreamFilter = new FileStream(filenamefullPath, FileMode.Open);
                ComParam = (ParamConf)serializerConf.Deserialize(filestreamFilter);
                filestreamFilter.Close();
            }
            catch 
            {
                //no existe el fichero genera unos con todas las variables
                filestreamFilter = new FileStream(filenamefullPath, FileMode.Create);
                ComParam = new ParamConf();
                serializerConf.Serialize(filestreamFilter, ComParam);
                filestreamFilter.Close();
            }


            ConfiguracionParams = ComParam;

            string Messagetosend = "SYSTEMID_" + ConfiguracionParams.Line_ID.ToString();
            SendMessage("Communication", Messagetosend);

            HRegion ROI1 = new HRegion(ConfiguracionParams.ROI1_Row1, ConfiguracionParams.ROI1_Col1, ConfiguracionParams.ROI1_Row2, ConfiguracionParams.ROI1_Col2);
            ROI1.WriteRegion("conf\\Patrones\\Cam1\\region1.hobj");

            HRegion ROI2 = new HRegion(ConfiguracionParams.ROI2_Row1, ConfiguracionParams.ROI2_Col1, ConfiguracionParams.ROI2_Row2, ConfiguracionParams.ROI2_Col2);
            ROI2.WriteRegion("conf\\Patrones\\Cam1\\region2.hobj");
            //parametros de la camara
            try
            {
                HOperatorSet.ReadCamPar("conf\\Patrones\\Cam1\\CameraParametersRGB.cal", out hv_CameraParameters);
                HOperatorSet.ReadPose("conf\\Patrones\\Cam1\\CameraPoseRGB.dat", out hv_CameraPose);
            }
            catch
            {
                WriteConsole("Error en la carga de los ficheros de camara", true);
            }

           

            if (Directory.GetFiles("conf\\Adquisition\\", "config.xml").Select(path => Path.GetFileName(path)).ToArray().Length > 0)
                return true;



            return false;
        }

        /// <summary>
        /// crea las carpetas para guardar las imagenes
        /// </summary>
        /// <param name="opcion"></param>
        public void CrearPathsGuardar()
        {
            
            try
                {
                    if (!Directory.Exists(pathToSave))
                        Directory.CreateDirectory(pathToSave);

                    if (!Directory.Exists(pathToSave + pathCam1))
                        Directory.CreateDirectory(pathToSave + pathCam1);

                    string diaHora = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                    pathImagen0 =  diaHora + "_RAW\\";                 
                    pathImagen1 =  diaHora + "_PROCESSED\\";                
                    if (especies.Count == 0)
                    {
                        pathImagen2 =  diaHora + "_FISHES\\";                    
                    }
                    
                }
                catch { }

           

        }
        /// <summary>
        /// 
        /// </summary>
        public void IniciarHDevelopEngine()
        {
               
            //Cargar el path de Halcon
            //HALCON
            path_halcon = Path.GetDirectoryName(Application.ExecutablePath) + "\\Procedimientos";
            PATH_IMAGEN_RGB_WHITE = Path.GetDirectoryName(Application.ExecutablePath) + "\\conf\\Patrones\\Cam1\\RGB_blanco.jpg";
            PATH_IMAGEN_NIR_WHITE = Path.GetDirectoryName(Application.ExecutablePath) + "\\conf\\Patrones\\Cam1\\NIR_blanco.jpg";
            string ProcedurePath = path_halcon;
            if (!HalconAPI.isWindows)
            {
                // Unix-based systems (Mono)
                ProcedurePath = ProcedurePath.Replace('\\', '/');
            }
            Engine.SetProcedurePath("Procedimientos//");        
            // enable or disable execution of compiled procedures
            Engine.SetEngineAttribute("execute_procedures_jit_compiled", "true");                
            HOperatorSet.SetSystem("clip_region", "false");

            // CARGAR LOS PROCEDIMIENTOS
            Procesado procesado = new Procesado();
            //carga las imagnes de los blancos
            CargaBlancos();
            HDevelopEngineIniciado = true;
            // inicializa el procesado
            
        }
        /// <summary>
        /// 
        /// </summary>
        public void CargaBlancos()
        {
            LoadIOConf();
            //CARGAR LA IMAGEN ImagenRgbWhite: imagen del blanco para eliminar el defecto porducido por la iluminación
            try
            {
                ImagenRGBWhite.ReadImage(PATH_IMAGEN_RGB_WHITE);
                ImagenNIRWhite.ReadImage(PATH_IMAGEN_NIR_WHITE);

                ImagenNIRWhite = ImagenNIRWhite.ZoomImageFactor(ConfiguracionParams.Image_scale_fr, ConfiguracionParams.Image_scale_fr, "constant");
                ImagenRGBWhite = ImagenRGBWhite.ZoomImageFactor(ConfiguracionParams.Image_scale_fr, ConfiguracionParams.Image_scale_fr, "constant");

            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message, "The white reference is not present, please capture one");
                ImagenRGBWhite.GenEmptyObj();
                ImagenNIRWhite.GenEmptyObj();
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void IniciarEngineHdev()
        {

            // Iniciar el HDevelop Engine
            if (!HDevelopEngineIniciado)
            {
                IniciarHDevelopEngine();
               
            }

            //Iniciar los hilos de adquisicion, preprocesado y procesado (Mosaiking + sacar el contorno)
            threadAdquisicion                       = new Thread(new ThreadStart(InitAdquisition));
            threadPreprocesado                      = new Thread(new ThreadStart(InitPreprocesado));
            threadMosaikingContorno                 = new Thread(new ThreadStart(InitMosaikingContorno));
            threadAdquisicion.Priority              = ThreadPriority.Highest;
            threadAdquisicion.IsBackground          = true;
            threadPreprocesado.Priority             = ThreadPriority.Highest;
            threadPreprocesado.IsBackground         = true;
            threadMosaikingContorno.Priority        = ThreadPriority.Highest;
            threadMosaikingContorno.IsBackground    = true;
            threadAdquisicion.Start();
            threadPreprocesado.Start();
            threadMosaikingContorno.Start();

        }
        /// <summary>
        /// 
        /// </summary>
        private void InitAdquisition()
        {
            //Iniciar los eventos de parada de hilos
            fifoSalidaAdquisicon = new BlockingCollection<ImagesToAdquitionProcess>();
            fifoSalidaProcesado = new BlockingCollection<ElementosToProcess>();
            src1 = new CancellationTokenSource();

            if (simulated == false)
            {
                if (primera_vez)
                {
                    InitFrameGrabber();
                    primera_vez = false;
                }
                SetFrameGabberParameters(SLEEP_ADQUISICION * 3);
                StartAdquisition();
               
            }
            else
            {
                InitFrameGrabberSimulacion();
                SetCameraParametersSimulacion();
                StartAdquisitionSimulacion();
            }

        }
      
        /// <summary>
        /// /////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>

        public bool InitFrameGrabber()
        {
            try
            {
                

                //Parchear la adquisición para que la "JAI BB-500GE"(sólo RGB) en "JAI AD-130GE"(RGB y NIR)
                if (soloRGB)
                {
                    //Abrir el framegrabber de la cámara JAI BB-500GE (RGB)
                    HOperatorSet.OpenFramegrabber("GigEVision", 0, 0, 0, 0, 0, 0, "default", -1,
                    "default", -1, "false", "default", ConfiguracionParams.RGB_name, 0,
                    -1, out hv_AcqHandleRGB);
                }
                else
                {
                    //Abrir el framegrabber de la cámara JAI AD-130GE (RGB)
                    HOperatorSet.OpenFramegrabber("GigEVision", 0, 0, 0, 0, 0, 0, "default", -1,
                    "default", -1, "false", "default", ConfiguracionParams.RGB_name, 0,
                    -1, out hv_AcqHandleRGB);

                    //Abrir el framegrabber de la cámara JAI AD-130GE (NIR)
                    HOperatorSet.OpenFramegrabber("GigEVision", 0, 0, 0, 0, 0, 0, "default", -1,
                    "default", -1, "false", "default", ConfiguracionParams.NIR_name, 0,
                    -1, out hv_AcqHandleNIR);
                }

                return true;
            }
            catch (Exception e)
            {
                //string cabecera = "Information";
                string textout = e.ToString();
                string dialogo = "error in the camera configuration";
                //DialogResult result = MessageBox.Show(dialogo, cabecera, MessageBoxButtons.OK, MessageBoxIcon.Information);
                WriteConsole(dialogo, true);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool InitFrameGrabberSimulacion()
        {
            try
            {
                if (camera1 != null)
                {
                    if (camera1.IsConnected())
                    {
                        camera1.DisConnect();
                    }
                }

                camera1 = new Camera.Camera("Xcelera-CL+_PX8_1", "SaperaLT", "T_P4_CC_02K07N_51_R_Infaimon_Master.ccf");
                camera1.path = path_simularCaptura;
                camera1.AdquisitionMode = CameraAdquisitionMode.EMULATION;
                camera1.Connect();
                SetCameraParametersSimulacion();
                camera1.AcquireImage(false);

                return true;
            }
            catch 
            {
                return false;
            }
        }
       

        /// <summary>
        /// 
        /// </summary>
        public void StopFramegrabbers()
        {

            string textoOut = "Stop frame grabber. \r\n";
            //visualiza el resultado de la clasificacion
            module_delegates.PrintOutText(textoOut, true);

            try
            {
                if (hv_AcqHandleRGB != null)
                {
                    HOperatorSet.CloseFramegrabber(hv_AcqHandleRGB);
                    hv_AcqHandleRGB = null;
                }
                    
                if (hv_AcqHandleNIR != null)
                {
                    HOperatorSet.CloseFramegrabber(hv_AcqHandleNIR);
                    hv_AcqHandleNIR = null;

                }
                   
            }
            catch {
                string textoOuterror = "Error Stopping frame grabber. \r\n";
                //visualiza el resultado de la clasificacion
                module_delegates.PrintOutText(textoOuterror, true);
            }
        }
        ///

        /// <summary>
        ///  Inicializa la captura el trigger software de las camaras
        /// </summary>
        public void StartAdquisitionTrigger()
        {
            while (Status != csr.modules.CSRModuleStatus.Closing && Status != csr.modules.CSRModuleStatus.Closed && !(_pararCaptura.WaitOne(SLEEP_ADQUISICION-5, true)))            
            {
                _pausarCaptura.WaitOne(Timeout.Infinite);
                //añadir  software trigger para capturar a la vez
                try
                {
                    //genero un pulso de 5 mseg
                    HOperatorSet.SetFramegrabberParam(hv_AcqHandleRGB, "TriggerSoftware", 1);
                    //añadir  software trigger para capturar a la vez si no hay sincronizacion
                    if (ConfiguracionParams.Sync == false)
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandleNIR, "TriggerSoftware", 1);
                    Thread.Sleep(5);
                    //genero un pulso de 5 mseg
                    HOperatorSet.SetFramegrabberParam(hv_AcqHandleRGB, "TriggerSoftware", 0);
                    //añadir  software trigger para capturar a la vez si no hay sincronizacion
                    if (ConfiguracionParams.Sync == false)
                        HOperatorSet.SetFramegrabberParam(hv_AcqHandleNIR, "TriggerSoftware", 0);


                }
                catch
                {
                    //solo  para DEBUG LUIS
                    string textoOut = "trigger process stopped \r\n";
                    //visualiza el resultado de la clasificacion
                    module_delegates.PrintOutText(textoOut, true);
                    break;
                }

            }

        }
        /// <summary>
        /// Inicializa la captura continua de las imagenes
        /// </summary>
        public void PrintOut( string texto, bool append = false)
        {
           
            module_delegates.PrintOutText(texto, append);
          
        }
        /// <summary>
        /// Inicializa la captura continua de las imagenes
        /// </summary>
        public void StartAdquisition()
        {
            First_round_precompilation_Hdev();
           
            try
            {
                module_delegates.PrintOutText("Adquisition Started. \r\n");

                HOperatorSet.GrabImageStart(hv_AcqHandleRGB, -1);
                HOperatorSet.GrabImageStart(hv_AcqHandleNIR, -1);
                _pararCaptura.Reset();
                // lanza el hilo de trigger para la captura
                threadAdquisicionTrigger = new Thread(new ThreadStart(StartAdquisitionTrigger));
                threadAdquisicionTrigger.Name = "triguer software";
                threadAdquisicionTrigger.Priority = ThreadPriority.Highest;
                threadAdquisicionTrigger.IsBackground = true;
                threadAdquisicionTrigger.Start();
               
                while (Status != csr.modules.CSRModuleStatus.Closing && Status != csr.modules.CSRModuleStatus.Closed && !(_pararCaptura.WaitOne(SLEEP_ADQUISICION/2, true)))
                {
             
                    _pausarCaptura.WaitOne(Timeout.Infinite);
                    //Console.WriteLine("Captura");
                    GrabImages();
                }
            }
            catch 
            {
                module_delegates.PrintOutText("Capture: error timeout \r\n", true);
                //Parar hilos              
            }
            //solo  para DEBUG LUIS
            string textoOut = "Capture process stopped. \r\n";
            //visualiza el resultado de la clasificacion
            module_delegates.PrintOutText(textoOut, true);
            PararTodo();
        }
        /// <summary>
        /// 
        /// </summary>
        public void PararTodo()
        {
            //Cuando esté parado, iniciarlo con src.Cancel(false);
            src1.Cancel(false);                    
            //para la captura de las imagenes
            _pararCaptura.Set();
           
           
        }
        /// <summary>
        /// 
        /// </summary>
        public void LimpiarTodo()
        {
            cont_imagenesAdquisicion = 0;
            cont_imagenesPreprocesadas = 0;
        }


        /// <summary>
        /// 
        /// </summary>
        public void StartAdquisitionSimulacion()
        {
            try
            {
                if (camera1.AdquisitionMode != CameraAdquisitionMode.EMULATION)
                    camera1.hfAcqHandle.GrabImageStart(-1);

                _pararCaptura.Reset();
                while (Status != csr.modules.CSRModuleStatus.Closing && Status != csr.modules.CSRModuleStatus.Closed && !(_pararCaptura.WaitOne(SLEEP_ADQUISICION, true)))
                {
                   
                    _pausarCaptura.WaitOne(Timeout.Infinite);
                    //Console.WriteLine("Captura");
                    GrabImagesSimulacion(CameraAdquisitionMode.EMULATION);
                }
            }
            catch 
            {

            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void GrabImagesRGB()
        {
            try
            {
                if(hv_AcqHandleRGB != null)
                {
                    HOperatorSet.GrabImageAsync(out ho_ImageRGB, hv_AcqHandleRGB, SLEEP_ADQUISICION * 3);
                    //rotar imagen 90 o 270
                    HOperatorSet.RotateImage(ho_ImageRGB,out  ho_ImageRGB, ConfiguracionParams.Rotation_camera,"constant");
                    
                }

            }
            catch
            {
                ho_ImageRGB = null;
                module_delegates.PrintOutText("Capture: timeout error RGB \r\n");
                
            }

        }
        /// <summary>
        /// 
        /// </summary>
        public void GrabImagesNIR()
        {
            try
            {
                if (hv_AcqHandleNIR != null)
                {
                    HOperatorSet.GrabImageAsync(out ho_ImageNIR, hv_AcqHandleNIR, SLEEP_ADQUISICION * 3);
                    HOperatorSet.RotateImage(ho_ImageNIR, out ho_ImageNIR, ConfiguracionParams.Rotation_camera, "constant");
                }
                   
            }
            catch 
            {
                ho_ImageNIR = null;
                module_delegates.PrintOutText("Capture: timeout error NIR \r\n");
                
            }


        }
        /// <summary>
        ///  //limpio los buffer RGB y NIR
        /// </summary>
        private void DoClearringCapture()
        {
            try
            {
               
                bool connectionok = false;
                module_delegates.PrintOutText("Capture: reinicio captura por timeout \r\n", true);

                StopFramegrabbers();
                connectionok = InitFrameGrabber();
                Thread.Sleep(200);
            }
            catch
            {
                module_delegates.PrintOutText("Capture: reinicio error \r\n", true);
            }


        }


        /// <summary>
        /// 
        /// </summary>
        public void GrabImages()
        {
            try
            {
                //Adquirir imagen RGB
                if (ho_ImageRGB != null)
                    ho_ImageRGB.Dispose();
                //Adquirir imagen NIR
                if (ho_ImageNIR != null)
                    ho_ImageNIR.Dispose();
                //lanzar en hilos independientes y esperar

                Thread threadAdquisicion1 = new Thread(() => GrabImagesRGB());
                Thread threadAdquisicion2 = new Thread(() => GrabImagesNIR());
                threadAdquisicion1.Priority = ThreadPriority.Highest;
                threadAdquisicion1.IsBackground = true;
                threadAdquisicion2.Priority = ThreadPriority.Highest;
                threadAdquisicion2.IsBackground = true;
                threadAdquisicion1.Start();
                threadAdquisicion2.Start();
                threadAdquisicion1.Join();
                threadAdquisicion2.Join();
              
                if (ho_ImageRGB == null || ho_ImageNIR == null)
                {
                    //limpio los buffer RGB y NIR y reinicio los frame grabber
                    DoClearringCapture();
                    return;
                }
                    

                //Parchear la adquisición para que la "JAI BB-500GE"(sólo RGB) en "JAI AD-130GE"(RGB y NIR)
                if (soloRGB)
                {
                    //Falsear la NIR y coger el canal R como si fuera NIR
                    HObject ho_Rgb1, ho_R, ho_G, ho_B;
                    HOperatorSet.GenEmptyObj(out ho_Rgb1);
                    HOperatorSet.GenEmptyObj(out ho_R);
                    HOperatorSet.GenEmptyObj(out ho_G);
                    HOperatorSet.GenEmptyObj(out ho_B);
                    HOperatorSet.Decompose3(ho_ImageRGB, out ho_R, out ho_G, out ho_B);
                    ho_ImageNIR = ho_R;
                }

                HalconDotNet.HImage imgRGB = new HImage(ho_ImageRGB);
                HalconDotNet.HImage imgNIR = new HImage(ho_ImageNIR);
             
                // Pasar las nuevas imágenes a la cola
                NewImagesFunction(imgRGB, imgNIR);
                


            }
            catch (Exception ex)
            {
                string textoOut = ex.ToString();
                module_delegates.PrintOutText(textoOut);

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modo"></param>
        /// <param name="async"></param>
        public void GrabImagesSimulacion(CameraAdquisitionMode modo, bool async = false)
        {
            try
            {
                if (modo == CameraAdquisitionMode.FROM_CAMERA)
                {
                    try
                    {
                        camera1.AcquireImage(async);
                    }
                    catch
                    {
                        //Patron(e.ToString(), true);
                    }
                }
                else
                {
                    
                    //lectura de la imagen RGB
                    camera1.AcquireImage(async);
                    //camera1.hiLastImageNIR = camera1.hiLastImage.RotateImage(ConfiguracionParams.Rotation_camera,"constant");
                    
                    camera1.hiLastImageNIR = camera1.hiLastImage;
                    //lecatura de la imagen NIR
                    camera1.AcquireImage(async);
                    //camera1.hiLastImageRGB = camera1.hiLastImage.RotateImage(ConfiguracionParams.Rotation_camera, "constant");
                    camera1.hiLastImageRGB = camera1.hiLastImage;
                }

                NewImagesFunction(camera1.hiLastImageRGB, camera1.hiLastImageNIR);
                

            }
            catch 
            {
                _pararCaptura.Set();
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool SetCameraParametersSimulacion()
        {
            //Espera tiempo infinito a que haya una imagen en el framegrabber
            if (camera1.AdquisitionMode == CameraAdquisitionMode.FROM_CAMERA)
                camera1.hfAcqHandle.SetFramegrabberParam("grab_timeout", -1);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sleep"></param>
        /// <returns></returns>
        public bool SetFrameGabberParameters(int sleep)
        {
            try
            {
                HOperatorSet.SetFramegrabberParam(hv_AcqHandleRGB, "'do_clear_ring_buffer'", -1);
                HOperatorSet.SetFramegrabberParam(hv_AcqHandleNIR, "'do_clear_ring_buffer'", -1);
                HOperatorSet.SetFramegrabberParam(hv_AcqHandleRGB, "grab_timeout", sleep);
                HOperatorSet.SetFramegrabberParam(hv_AcqHandleNIR, "grab_timeout", sleep);
            }
            catch
            {
            }

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imgRGB"></param>
        /// <param name="imgNIR"></param>
        public void NewImagesFunction(HImage imgRGB, HImage imgNIR)
        {
            imagenes                        = new ImagesToAdquitionProcess();
           
            imagenes.imagenRGB              = imgRGB;
            imagenes.imagenNIR              = imgNIR;
            //actualiza las variables de control
           

            try
            {
                string Variable_IO = "";
                IO_Parameters io = (IO_Parameters)GetGlobalParameter("IO");
                Variable_IO = "GVL.CSR";
                Variable_IO += "_TIME_STAMP";
                imagenes.milliseconds = Convert.ToUInt64(io.Out[Variable_IO].Value);
                imagenes.millisecondslocal = (UInt64)Math.Round((double)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
            }
            catch
            {
                imagenes.milliseconds = (UInt64)Math.Round((double)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
                imagenes.millisecondslocal = (UInt64)Math.Round((double)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);

            }
            
            //sends messages to the twincat control statur
            string Messagetosend = "CAMERASTATUS_" + imagenes.millisecondslocal.ToString();
            SendMessage("Communication", Messagetosend);

            //envio de la imagen a la calibracion

            ImagesToProcess imagenesCalib = new ImagesToProcess();
            imagenesCalib.img1NIR = imgNIR.CopyImage();
            imagenesCalib.img1RGB = imgRGB.CopyImage();
            imagenesCalib.TimeStamp = DateTime.Now;
            imagenesCalib.Linea = ConfiguracionParams.Line_ID;
            imagenesCalib.Nombre_variable = "LINE_";
            imagenesCalib.taxi = ConfiguracionParams.Line_ID;
            if (OneShootImage == true)
            {
                OneShootImage = false;
                if (NewImagesRaw != null)
                    this.NewImagesRaw(imagenesCalib);
            }
            

            cont_imagenesAdquisicion = cont_imagenesAdquisicion + 1;

            InterfacetoVisializerParams = module_delegates.GetResultsParams();
            //Guardar imágenes
            if (InterfacetoVisializerParams.cbGuardar0_Checked)
            {
                SaveImages(imagenes, RegionVisualizar.IMAGENES_BRUTO);
            }
            
            
            
            //visualiza imagenes BRUTO
            if (InterfacetoVisializerParams.cbVisualizar0_Checked)
            {
                ElementosToProcess elementos = new ElementosToProcess();
                elementos.imagenes = new List<ImagesToAdquitionProcess>();
                elementos.imagenes.Add(imagenes);        
                module_delegates.DispImages(elementos, RegionVisualizar.IMAGENES_BRUTO);
                elementos.imagenes.Clear();

            }
            InterfacetoVisializerParams.lblContImg2Wrong_Text = cont_imagenesPezbuffer.ToString();
            InterfacetoVisializerParams.lblContImg0_Text = cont_imagenesAdquisicion.ToString();
            InterfacetoVisializerParams.fr = (int)(imagenes.millisecondslocal - time_last_image);
            time_last_image = imagenes.millisecondslocal;
            module_delegates.DispResults(InterfacetoVisializerParams);

            //Poner las imágenes en una cola FIFO limito el buffer a 50 imagenes para no acabar con la memoria
            if (cont_imagenesAdquisicion % SALTO == 0 && fifoSalidaAdquisicon.Count() < BUFFERSIZE)
            {
                fifoSalidaAdquisicon.Add(imagenes);
                imagenes.num = cont_imagenesAdquisicion;
               
            }

                     
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img"></param>
        /// <param name="modo"></param>
        /// 
        private void SaveImagesThread(ImagesToAdquitionProcess img, RegionVisualizar modo)
        {
           
            string pathImagen = "";
            int cont = 0;
            switch (modo)
            {
                case RegionVisualizar.IMAGENES_BRUTO:
                    pathImagen = pathToSave + pathCam1 + pathImagen0;
                    cont = cont_imagenesAdquisicion;
                    break;

                case RegionVisualizar.IMAGENES_PREPROCESADAS:

                    pathImagen = pathToSave + pathCam1 + pathImagen1;
                    cont = cont_imagenesPreprocesadas;
                    break;

                case RegionVisualizar.IMAGENES_PEZ_COMPLETO:

                    // no se ha gargado el clasificador
                    pathImagen = pathToSave + pathCam1 + pathImagen2;
                    cont = cont_imagenesPezCompleto;
                    break;
            }

            try
            {
                Directory.CreateDirectory(pathImagen);
                img.imagenRGB.WriteImage("jpg", 0, pathImagen + img.milliseconds.ToString() + "_RGB_" + cont.ToString());
                img.imagenNIR.WriteImage("jpg", 0, pathImagen + img.milliseconds.ToString() + "_NIR_" + cont.ToString());

            }
            catch
            {
                
            }
            if(img.imagenNIR != null)
                img.imagenNIR.Dispose();
            if (img.imagenRGB != null)
                img.imagenRGB.Dispose();
            img = null;



        }
        /// <summary>
        /// /////////
        /// </summary>
        /// <param name="img"></param>
        /// <param name="modo"></param>
        private void SaveImages(ImagesToAdquitionProcess img, RegionVisualizar modo)
        {
            ImagesToAdquitionProcess imgTemp = new ImagesToAdquitionProcess();
            imgTemp.imagenNIR       = img.imagenNIR.CopyImage();
            imgTemp.imagenRGB       = img.imagenRGB.CopyImage();
            imgTemp.milliseconds    = img.milliseconds;
            imgTemp.nombre          = img.nombre;
            imgTemp.num             = img.num;
            imgTemp.position        = img.position;

            Thread ProcesadothreadL1;
            ProcesadothreadL1 = new Thread(() => this.SaveImagesThread(imgTemp, modo));
            ProcesadothreadL1.Name = "SaveImage";
            ProcesadothreadL1.Priority = ThreadPriority.BelowNormal;
            ProcesadothreadL1.IsBackground = true;
            ProcesadothreadL1.Start();



        }
        //*******************************************************************************************************//
        //******************************************** PREPROCESADO *********************************************//      
        private void InitPreprocesado()
        {
            // variables para la identificación del pez  
            int hayAlgo                             = 0;
            int pos_ini                             = 0;
            int iniPez                              = 0;
            int finPez                              = 0;   
            bool enviado_mosaicing                  = false;
            uint posicion_inicio_pez                = 0;
            HImage TempImage                        = null;
            int    hayalgo_seguido_cont             = 0;
            List<ImagesToAdquitionProcess> pezImg   = new List<ImagesToAdquitionProcess>();
            List<MascarasToProcess> pezMask         = new List<MascarasToProcess>();
            UInt64                   t_inicio       = 0;
            ImagesToAdquitionProcess newImg         = null;
            DateTime TinicioProcessadoCalib;
      
            DateTime Final1 = DateTime.Now;
            DateTime Final2 = DateTime.Now;
            while (Status != csr.modules.CSRModuleStatus.Closing && Status != csr.modules.CSRModuleStatus.Closed)
              {         
                // esperar a que haya una nueva imagen (RGB + NIR)
                try
                {              
                    newImg = fifoSalidaAdquisicon.Take(src1.Token);                
                    cont_imagenesPezbuffer  = fifoSalidaAdquisicon.Count();
                    TinicioProcessadoCalib = DateTime.Now;
                    //reescala las images para reducir el tiempo de procesadon//RGB
                    TempImage = newImg.imagenRGB.ZoomImageFactor(ConfiguracionParams.Image_scale_fr, ConfiguracionParams.Image_scale_fr, "constant");                 
                    newImg.imagenRGB.Dispose();
                    newImg.imagenRGB = TempImage;
                     //NIR        
                    TempImage = newImg.imagenNIR.ZoomImageFactor(ConfiguracionParams.Image_scale_fr, ConfiguracionParams.Image_scale_fr, "constant");                
                    newImg.imagenNIR.Dispose();
                    newImg.imagenNIR = TempImage;
                    //para que la primera imagen capturada sea todo negro y no detecte nada
                    if(cont_imagenesAdquisicion == 1)
                    {
                        newImg.imagenNIR = newImg.imagenNIR.ScaleImage(0.0, 0.0);
                        newImg.imagenRGB = newImg.imagenRGB.ScaleImage(0.0, 0.0);
                    }                 
                }
                catch 
                {
                    break;
                }
                //region de procesado de la imagen
                try
                {
                    #region Procesado_imagen                
                   // sacar contorno NIR rápido para no preprocesar todas las imágenes, sólo en las que haya pez, porque cada
                   //  preprocesado tarde 400ms aprox (100ms NIR + 300ms RGB).
                    HRegion ROI             = new HRegion(ConfiguracionParams.ROI1_Row1, ConfiguracionParams.ROI1_Col1, ConfiguracionParams.ROI1_Row2, ConfiguracionParams.ROI1_Col2);
                    ROI                     = ROI.ZoomRegion(ConfiguracionParams.Image_scale_fr, ConfiguracionParams.Image_scale_fr);
                    /*
                    //Parámetros de entrada:                   
                     ProcCallDeteccionRapida.SetInputIconicParamObject("Imagen", newImg.imagenRGB);
                     ProcCallDeteccionRapida.SetInputIconicParamObject("ImagenRgbWhite", ImagenRGBWhite);
                     ProcCallDeteccionRapida.SetInputIconicParamObject("ROI_0", ROI);
                     ProcCallDeteccionRapida.SetInputCtrlParamTuple("scale_1_1", ConfiguracionParams.X_Scale);

                    // Ejecutar procedimiento
                    ProcCallDeteccionRapida.Execute();
                     //ver las imágenes en las que hay algo               
                     hayAlgo = (int)ProcCallDeteccionRapida.GetOutputCtrlParamTuple("result").D;
                     pos_ini = (int)ProcCallDeteccionRapida.GetOutputCtrlParamTuple("Y_start").D;
                    */
                    ////////////////////////////////////////////////////////////////////////////////////
                    HTuple h_hayAlgo;
                    HTuple h_pos_ini;
                    HalconProcedures.RC_optimar_identificar_rapido(newImg.imagenRGB, ImagenRGBWhite, ROI, ConfiguracionParams.X_Scale, out h_hayAlgo, out h_pos_ini);
                    hayAlgo =(int)(h_hayAlgo.D);
                    pos_ini =(int)(h_pos_ini.D);

                    /////////////////////////////////////////////////////////////////////////////////////
                    if (hayAlgo == -1)
                        SendMessage("alarm", "1004"); //fallo en la iluminacion

                    if (hayAlgo > ConfiguracionParams.Area_min_pix && hayalgo_seguido_cont < 15)
                     {
                        SendMessage("alarm", "-1004");
                        hayalgo_seguido_cont++;
                                            
                        if (pezImg == null)
                             pezImg = new List<ImagesToAdquitionProcess>();

                         posicion_inicio_pez   = (uint)((pos_ini)*ConfiguracionParams.Y_Scale);
                         t_inicio              = newImg.milliseconds;
                        // preprocesado correccion de la deformacion y resclado a 1:1
                        double factorScalex = ConfiguracionParams.X_Scale_W / ConfiguracionParams.X_Scale;
                        double factorScaley = ConfiguracionParams.Y_Scale_W / ConfiguracionParams.Y_Scale;
                        
                        HImage imgNIR           = newImg.imagenNIR;
                        HImage imgRGB           = newImg.imagenRGB;
                         // preprocesar NIR con hdevelopengine
                         HRegion ROI2            = new HRegion(ConfiguracionParams.ROI2_Row1, ConfiguracionParams.ROI2_Col1, ConfiguracionParams.ROI2_Row2, ConfiguracionParams.ROI2_Col2);
                         ROI2                    = ROI2.ZoomRegion(ConfiguracionParams.Image_scale_fr, ConfiguracionParams.Image_scale_fr);

                      
                        HImage ImagenNIRCorregida = null;
                        HObject ho_ImageOrg;
                        HalconProcedures.RC_optimar_preprocesar_imagenes(imgNIR, ImagenNIRWhite, ROI2, out ho_ImageOrg, hv_CameraParameters, hv_CameraPose, ConfiguracionParams.Angle_belt, factorScalex, factorScaley);
                        ImagenNIRCorregida = new HImage(ho_ImageOrg);

                        ///////////////////////////////////////////////////////////////////
                        HImage ImagenRGBCorregida = null;
                        HObject ho_ImageOrg_RGB = null;
                        HalconProcedures.RC_optimar_preprocesar_imagenes(imgRGB, ImagenRGBWhite, ROI2, out ho_ImageOrg_RGB, hv_CameraParameters, hv_CameraPose, ConfiguracionParams.Angle_belt, factorScalex, factorScaley);
                        ImagenRGBCorregida = new HImage(ho_ImageOrg_RGB);


                        
                        //Almacenar estructuras imágenes y máscaras
                        UInt64 milliseconds        = newImg.milliseconds;                                    
                         newImg.imagenNIR           = ImagenNIRCorregida;
                         newImg.imagenRGB           = ImagenRGBCorregida;
                         newImg.milliseconds        = milliseconds;                   
                         cont_imagenesPreprocesadas = cont_imagenesPreprocesadas + 1 * SALTO;
                         //actualiza las variables de control
                         InterfacetoVisializerParams                    = module_delegates.GetResultsParams();
                         InterfacetoVisializerParams.lblContImg1_Text   = cont_imagenesPreprocesadas.ToString();
                         module_delegates.DispResults(InterfacetoVisializerParams);
                         //Guardar imagen corregida
                         if (InterfacetoVisializerParams.cbGuardar1_Checked)
                         {
                           
                            this.SaveImages(newImg, RegionVisualizar.IMAGENES_PREPROCESADAS);
                          
                         }

                        HImage  ImagenRGBsinFondo   = null;
                        HImage  ImagenNIRsinFondo   = null;
                        int     iniPezF             = 0;
                        int     finPezF             = 0;
                        int     SinFondo = 0;
                   

                        ///////////////////////////////////////////////////////////////////////////////////////
                        HObject ho_ImagenSinFondoNIR, ho_ImagenSinFondoRGB;
                        HObject ho_Mascara, ho_Contorno;
                        HTuple hv_iniPez, hv_finPez, hv_Largo, hv_Ancho, hv_Area, hv_Fila1, hv_Fila2;
                        HalconProcedures.RC_optimar_contorno_imagen(newImg.imagenRGB, newImg.imagenNIR,
                        out ho_Mascara, out ho_Contorno, out ho_ImagenSinFondoRGB,out ho_ImagenSinFondoNIR, SinFondo, out hv_iniPez,
                        out hv_finPez, out hv_Largo, out hv_Ancho, out hv_Area,out hv_Fila1, out hv_Fila2);

                        ImagenRGBsinFondo = new HImage(ho_ImagenSinFondoRGB);
                        ImagenNIRsinFondo = new HImage(ho_ImagenSinFondoNIR);

                        iniPezF = (int)hv_Fila1.D;
                        finPezF = (int)hv_Fila2.D;                        
                        
                        ///////////////////////////////////////////////////////////////////////////////////

                        //Almacenar estructuras imágenes y máscaras
                        newImg.imagenNIR = ImagenNIRsinFondo;
                        newImg.imagenRGB = ImagenRGBsinFondo;
                         
                        //visualizar imagen corregida    
                       
                         InterfacetoVisializerParams = module_delegates.GetResultsParams();
                         if (InterfacetoVisializerParams.cbVisualizar1_Checked)
                         {
                            ElementosToProcess elementos = new ElementosToProcess();
                            elementos.imagenes = new List<ImagesToAdquitionProcess>();
                            elementos.imagenes.Add(newImg);
                            module_delegates.DispImages(elementos, RegionVisualizar.IMAGENES_PREPROCESADAS);
                            elementos.imagenes.Clear();

                        }
                        
                         //añado la imagen a la lista
                         if (enviado_mosaicing == false)
                         {
                             newImg.position = posicion_inicio_pez;

                             pezImg.Add(newImg);
                             //comprueba la imagen para ver su localizacion
                             iniPez = (int)hv_iniPez.D;
                             finPez = (int)hv_finPez.D;
                             //pez contenido por completo elimino y emvio solo esa imagen
                             if (iniPez == 1 && finPez == 1)
                             {

                                 pezImg.Clear();
                                 pezImg.Add(newImg);
                             }

                             if (finPez == 1 && pezImg.Count() > 0)
                             {
                                 enviado_mosaicing       = true;
                                 //envio la imagen a procesar                                              
                                 ElementosToProcess pez  = new ElementosToProcess();
                                 pez.t_ini               = pezImg[0].milliseconds;
                                 pez.imagenes            = pezImg;                            
                                 pez.pos_init            = pezImg[0].position;
                                 // pez terminado, añadir a la cola 
                                 fifoSalidaProcesado.Add(pez);
                                WriteConsole("Piece send finih detected", true);
                            }
                         }
                        
                         PosanteriorhayAlgo             = iniPezF;
                        //escribe el tiempo de procesado
                        DateTime TFinalProcessadoCalib  = DateTime.Now;
                        TimeSpan CalibTime              = TFinalProcessadoCalib - TinicioProcessadoCalib;
                        string texto_debug;
                        texto_debug = " Process fish detection: " + CalibTime.TotalMilliseconds.ToString();
                        //saca por consola los textos
                        WriteConsole(texto_debug, true);
                        SendMessage("Communication", "SYSTEMERROR_00000000");//desactivo el ereror
                    }
                     else
                     {

                        if (hayalgo_seguido_cont >= 15 )
                        {
                            if (pezImg != null)
                                pezImg.Clear();
                            WriteConsole("Obstacle in vision detected", true);
                            SendMessage("Communication", "SYSTEMERROR_00000001");//activo error
                        } 
                        if (pezImg != null && pezImg.Count() > 0 && enviado_mosaicing == false)
                         {
                             enviado_mosaicing       = true;
                             //envio la imagen a procesar                                              
                             ElementosToProcess pez = new ElementosToProcess();
                             pez.t_ini               = pezImg[0].milliseconds;
                             pez.imagenes            = pezImg;
                             pez.pos_init            = pezImg[0].position;
                             // pez terminado, añadir a la cola 
                             fifoSalidaProcesado.Add(pez);
                             WriteConsole("Piece send no finih detected", true);
                        }


                        hayalgo_seguido_cont = 0;

                        enviado_mosaicing  = false;
                         pezImg             = null;
                         iniPez             = 0;
                         finPez             = 0;
                       
                     }

                    #endregion
                }
                catch (Exception ex)
                {
                    string textoout = ex.ToString();
                }

            }
          
        }


        //*******************************************************************************************************//
        //********************************************** MOSAIKING **********************************************//
        //*******************************************************************************************************//


        private void InitMosaikingContorno()
        {
            bool errorPatron = false;
           
            

            while (Status != csr.modules.CSRModuleStatus.Closing && Status != csr.modules.CSRModuleStatus.Closed)
            {
                ImagesToAdquitionProcess newImg     = new ImagesToAdquitionProcess();
                MascarasToProcess newMask           = new MascarasToProcess();
                ElementosToProcess pez              = null;
                HTuple width                        = 0;
                HTuple heigth                       = 0;
                try
                {
                    pez = fifoSalidaProcesado.Take(src1.Token);
                    
                }
                catch
                {
                    break;
                }
                DateTime TinicioProcessadoMosiacking = DateTime.Now;
                try
                {
                    #region Procesado_mosaicking               
                    errorPatron                 = false;
                    HImage mosRGB               = pez.imagenes[0].imagenRGB;
                    HImage mosNIR               = pez.imagenes[0].imagenNIR;
                    mosNIR.GetImageSize(out width, out heigth);
                    bool error_mosaiking        = false;
                    double timestampIniciopez   = 0;
                    if (pez.imagenes.Count > 1) // comprobar que haya más de una imagen para hacer el mosaiking
                    {               
                        int IndexDecr           = pez.imagenes.Count - 1;
                        HImage imgsSampleRGB    = pez.imagenes[IndexDecr].imagenRGB;
                        HImage imgsSampleNIR    = pez.imagenes[IndexDecr].imagenNIR;
                        //LGCORZO: mejorar para que sea automtico
                        int desplazamiento_Y    = (int) (heigth - ConfiguracionParams.Mosaick_desp_y);
                        int desplazamiento_X    = (int) (ConfiguracionParams.Mosaick_desp_x);
                        UInt64 timeINI          = pez.imagenes[IndexDecr - 1].milliseconds;
                        UInt64 timeFIN          = pez.imagenes[IndexDecr].milliseconds;
                        timestampIniciopez      = pez.t_ini;
                        //diferencia en milisegundos entre las capturas 
                        UInt64 Diff_timeFIN_INI = timeFIN - timeINI;
                        HTuple DespxArray       = Diff_timeFIN_INI;
                        for (int i = 1; i < pez.imagenes.Count; i++)
                        {
                            //se ordenan segun LIFO
                            IndexDecr--;
                            imgsSampleRGB        = imgsSampleRGB.ConcatObj(pez.imagenes[IndexDecr].imagenRGB);
                            imgsSampleNIR        = imgsSampleNIR.ConcatObj(pez.imagenes[IndexDecr].imagenNIR);
                            if(IndexDecr > 0)
                            {
                                timeINI             = pez.imagenes[IndexDecr - 1].milliseconds;
                                timeFIN             = pez.imagenes[IndexDecr].milliseconds;
                                //diferencia en milisegundos entre las capturas 
                                Diff_timeFIN_INI    = timeFIN - timeINI;
                                DespxArray          = DespxArray.TupleConcat(Diff_timeFIN_INI);
                            }
                                          
                            timestampIniciopez  = pez.t_ini;                          
                            //conocida la velocidad y la resulucion de las imagenes podemos calcular el avance teórico del pez
                            double row_desplazamiento =   Diff_timeFIN_INI* SPEED_ADQUISICION / ConfiguracionParams.Y_Scale_W; //mseg*mm/mseg*pix/mmm  (m/seg = mm/mseg)
                        }
                    
                        // Ejecutar procedimiento
                        try
                        {
                           
                            HObject ho_ImagenMosaickingRGB;
                            HTuple hv_error_mosaiking;
                            HalconProcedures.Tile_images_roboconcept(imgsSampleRGB, out ho_ImagenMosaickingRGB,
                            desplazamiento_Y, desplazamiento_X, ConfiguracionParams.Mosaick_Correl_activ, out hv_error_mosaiking);
                            mosRGB          = new HImage(ho_ImagenMosaickingRGB);
                            int error       = (int)hv_error_mosaiking.D;

                            
                            HObject ho_ImagenMosaickingNIR;
                            HTuple hv_error_mosaiking_NIR;
                            HalconProcedures.Tile_images_roboconcept(imgsSampleNIR, out ho_ImagenMosaickingNIR,
                            desplazamiento_Y, desplazamiento_X, ConfiguracionParams.Mosaick_Correl_activ, out hv_error_mosaiking_NIR);
                            mosNIR = new HImage(ho_ImagenMosaickingNIR);


                            if (error == 1)
                            {
                                error_mosaiking = true;
                            }
                            //Limpiar procedimiento
                            
                            imgsSampleRGB.Dispose();
                            imgsSampleNIR.Dispose();
                        }
                        catch
                        {
                            //No se ha encontrado patrón
                            errorPatron = true;
                            pez.imagenes.Clear();
                            imgsSampleRGB.Dispose();
                            imgsSampleNIR.Dispose();
                        }
                    }

                    if (!errorPatron)
                    {
                        counter_fish++;
                        newImg.imagenNIR = mosNIR;
                        newImg.imagenRGB = mosRGB;
                       
                        double largo = 0;
                        double ancho = 0;
                        double area = 0;
                        try
                        {
                           
                            //Imagen del pez recortada
                           
                            int SinFondo = 1;
                            HObject ho_Mascara, ho_Contorno, ho_ImagenSinFondoRGB, ho_ImagenSinFondoNIR;
                            HTuple hv_iniPez, hv_finPez, hv_Largo, hv_Ancho, hv_Area, hv_Fila1, hv_Fila2;
                            HalconProcedures.RC_optimar_contorno_imagen(mosRGB, mosNIR,out  ho_Mascara, out  ho_Contorno, out  ho_ImagenSinFondoRGB,out ho_ImagenSinFondoNIR,
                                SinFondo, out  hv_iniPez,out  hv_finPez, out  hv_Largo, out  hv_Ancho, out  hv_Area,out  hv_Fila1, out  hv_Fila2);

                            largo   = hv_Largo.D;
                            ancho   = hv_Ancho.D;
                            area    = hv_Area.D;
                            HImage ImagenNIRPez = new HImage(ho_ImagenSinFondoNIR);
                            HImage ImagenRGBPez = new HImage(ho_ImagenSinFondoRGB);

                            newImg.imagenNIR = ImagenNIRPez;
                            newImg.imagenRGB = ImagenRGBPez;
                        }
                        catch
                        {
                            WriteConsole("fallo en mosaicking contorno", true);
                        }

                       
                        //Ver cuanto a tardado
                        UInt64 time_proccess = 0;
                        
                         time_proccess = (UInt64)Math.Round((double)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond) - pez.imagenes[0].millisecondslocal;
                           
                     
                        //Almacenar estructuras imágenes
                        cont_imagenesPezCompleto                        = cont_imagenesPezCompleto + 1;
                        FACTOR_LARGO                                    = (0.1 * ConfiguracionParams.Y_Scale_pix_cm) / ConfiguracionParams.Y_Scale_W;
                        FACTOR_ANCHO                                    = (0.1 * ConfiguracionParams.X_Scale_pix_cm) / ConfiguracionParams.X_Scale_W;
                        largo                                           = largo * FACTOR_LARGO;
                        ancho                                           = ancho * FACTOR_ANCHO;
                        area                                            = area * FACTOR_LARGO * FACTOR_ANCHO;
                        InterfacetoVisializerParams                     = module_delegates.GetResultsParams();
                        InterfacetoVisializerParams.lblContImg2_Text    = cont_imagenesPezCompleto.ToString();
                        InterfacetoVisializerParams.largo               = largo;
                        InterfacetoVisializerParams.ancho               = ancho;
                        InterfacetoVisializerParams.area                = area;
                        InterfacetoVisializerParams.time_proccess       = time_proccess;
                        InterfacetoVisializerParams.error_mosaiking     = error_mosaiking;

                        module_delegates.DispResults(InterfacetoVisializerParams);
                       
                        if (InterfacetoVisializerParams.cbVisualizar2_Checked)
                        {
                            ElementosToProcess elementos = new ElementosToProcess();
                            elementos.imagenes = new List<ImagesToAdquitionProcess>();
                            elementos.imagenes.Add(newImg);
                            module_delegates.DispImages(elementos, RegionVisualizar.IMAGENES_PEZ_COMPLETO);
                            elementos.imagenes.Clear();
                        }

                       

                       //evento de imagen compuesta lista para procesar
                       ImagesToProcess imagenes         = new ImagesToProcess();
                        newImg.nombre                   = "";
                        imagenes.img1NIR                = newImg.imagenNIR;
                        imagenes.img1RGB                = newImg.imagenRGB;
                        imagenes.TimeStamp              = DateTime.Now;
                        imagenes.Linea                  = ConfiguracionParams.Line_ID;
                        imagenes.Nombre_variable        = "LINE_";
                        imagenes.taxi                   = ConfiguracionParams.Line_ID;
                        imagenes.parametros.fParam1     = (float)largo;
                        imagenes.parametros.fParam2     = (float)ancho;
                        imagenes.parametros.fParam3     = (float)area;
                        double desplazamiento_deteccion =  (pez.pos_init*ConfiguracionParams.Y_Scale_pix_cm);
                        imagenes.parametros.fParam4     = (float)pez.t_ini - (float)(desplazamiento_deteccion/ SPEED_ADQUISICION);
                        imagenes.parametros.iParam1     = (int)counter_fish;
                        imagenes.parametros.iParam2     = (int)pez.pos_init;
                        imagenes.IDproducto             = counter_fish;


                        if (area > ConfiguracionParams.Area_min_fish)
                        {
                            //asignacion de parametros de medida para envio
                            if (NewImages != null && imagenes.img1NIR != null && imagenes.img1RGB != null)
                                this.NewImages(imagenes);

                            //Guardar imagen corregida
                            if (InterfacetoVisializerParams.cbGuardar2_Checked)
                            {
                                this.SaveImages(newImg, RegionVisualizar.IMAGENES_PEZ_COMPLETO);
                            }

                            module_delegates.VisualizaResultado();
                            
                        }
                        else
                        {
                            imagenes.Dispose();
                            WriteConsole("fish detected min area excluded", true);
                        }

                        


                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    string textoout = ex.ToString();
                    WriteConsole(textoout, true);
                }

                DateTime TFinalProcessadoMosiacking = DateTime.Now;
                TimeSpan MosaickingTime = TFinalProcessadoMosiacking - TinicioProcessadoMosiacking;
                string texto_debug;
                texto_debug = " Procesado Mosaicking: " + MosaickingTime.TotalMilliseconds.ToString();
                //saca por consola los textos
                WriteConsole(texto_debug, true);

            }
           
        }
        //////////////////////////////////////////////////////////////
    }
    /// <summary>
    /// 
    /// </summary>
    public class ImagesToProcess
    {
        public HImage       img1RGB             = null;
        public HImage       img2RGB             = null;
        public HImage       img3RGB             = null;
        public HImage       img1NIR             = null;
        public HImage       img2NIR             = null;
        public HImage       img3NIR             = null;
        public HImage       imgAnteriorRGB      = null;
        public HImage       imgPosteriorRGB     = null;
        public DateTime     TimeStamp           = DateTime.Now;
        public int          Linea               = -1;
        public int          taxi                = -1;
        public int          IDproducto          = 0;
        public int          IDproductoCSR       = 0;
        public long         Encoder             = 0;
        public string       Nombre_variable     = "";
        public Parametros_Process parametros    = new Parametros_Process();

        public void Dispose()
        {
            if (img1RGB != null)
                img1RGB.Dispose();
            if (img2RGB != null)
                img2RGB.Dispose();
            if (img3RGB != null)
                img3RGB.Dispose();
            if (img1NIR != null)
                img1NIR.Dispose();
            if (img2NIR != null)
                img2NIR.Dispose();
            if (img3NIR != null)
                img3NIR.Dispose();
            if (imgAnteriorRGB != null)
                imgAnteriorRGB.Dispose();
            if (imgPosteriorRGB != null)
                imgPosteriorRGB.Dispose();
            img1RGB = null;
            img2RGB = null;
            img3RGB = null;
            img1NIR = null;
            img2NIR = null;
            img3NIR = null;
            imgAnteriorRGB = null;
            imgPosteriorRGB = null;
            parametros = null;
        }

    }
    public class Parametros_Process
    {
        public String sParam1   = "";
        public float fParam1    = 0; //lago
        public float fParam2    = 0; //ancho
        public float fParam3    = 0; //area
        public float fParam4    = 0;
        public int iParam1      = 0; //numproducto linea
        public int iParam2      = 0;
        public int iParam3      = 0;
        public int iParam4      = 0;      
    }

    public class  ColorMatrixMachine
    {
        public  HMatrix     ColorMatrix1_l1     = null;    
    }


    public class ImagesToAdquitionProcess
    {
        public HImage       imagenRGB           = null;
        public HImage       imagenNIR           = null;
        public int          num                 = 0;
        public UInt64       milliseconds        = 0;
        public UInt64       millisecondslocal   = 0;
        public string       nombre              = "";
        public uint         position            = 0;
    }
    public class MascarasToProcess
    {
        public HRegion mascaraRGB = null;
        public HRegion mascaraNIR = null;
    }

    public class ElementosToProcess
    {
        public List<ImagesToAdquitionProcess>   imagenes    = null;
        public List<MascarasToProcess>          mascaras    = null;
        public UInt64                           t_ini       = 0;
        public uint                             pos_init    = 0;
    }
    public class ParamConf
    {
        [XmlElement("Area_min_pix")]
        public int Area_min_pix = 2000;
        [XmlElement("Area_min_fish")]
        public int Area_min_fish = 8000;     
        [XmlElement("ROI1_Row1")]
        public double ROI1_Row1 = 178.389;
        [XmlElement("ROI1_Row2")]
        public double ROI1_Row2 = 891.082;
        [XmlElement("ROI1_Col1")]
        public double ROI1_Col1 = 272.687;
        [XmlElement("ROI1_Col2")]
        public double ROI1_Col2 = 858.401;
        [XmlElement("ROI2_Row1")]
        public double ROI2_Row1 = 46.668;
        [XmlElement("ROI2_Row2")]
        public double ROI2_Row2 = 891.918;
        [XmlElement("ROI2_Col1")]
        public double ROI2_Col1 = 214.656;
        [XmlElement("ROI2_Col2")]
        public double ROI2_Col2 = 867.719;
        [XmlElement("Angle_belt")]
        public double Angle_belt = -4;
        [XmlElement("Image_scale_fr")]
        public double Image_scale_fr = 0.5;
        [XmlElement("Rotation_camera")]
        public double Rotation_camera = 0;
        [XmlElement("X_Scale")]
        public double X_Scale = 1.0;//pix/mm
        [XmlElement("Y_Scale")]
        public double Y_Scale = 1.0;
        [XmlElement("X_Scale_W")]
        public double X_Scale_W = 2.0;//pix/mm
        [XmlElement("Y_Scale_W")]
        public double Y_Scale_W = 2.0;
        [XmlElement("X_Scale_pix_cm")]
        public double X_Scale_pix_cm = 1.0;//pix/mm
        [XmlElement("Y_Scale_pix_cm")]
        public double Y_Scale_pix_cm = 1.0;
        [XmlElement("RGB_name")]
        public string RGB_name = "000cdf080268_JAILtdJapan_AD130GE0";
        [XmlElement("NIR_name")]
        public string NIR_name = "000cdf081268_JAILtdJapan_AD130GE1";
        [XmlElement("Sync")]
        public bool Sync = true;
        [XmlElement("Line_ID")]
        public int Line_ID = 0;
        [XmlElement("Mosaick_desp_x")]
        public int Mosaick_desp_x = 300;
        [XmlElement("Mosaick_desp_y")]
        public int Mosaick_desp_y = 0;
        [XmlElement("Mosaick_Correl_activ")]
        public int Mosaick_Correl_activ = 0;
        [XmlElement("Save_image_vision")]
        public int Save_image_vision = 0;
        [XmlElement("Save_images_step")]
        public int Save_images_step = 1;
    }

    public enum RegionVisualizar
    {
        IMAGENES_BRUTO,
        IMAGENES_PREPROCESADAS,
        IMAGENES_PEZ_COMPLETO
    }

    public class FormInterface
    {
        public string       lblContImg0_Text;
        public string       lblContImg1_Text;
        public string       lblContImg2Wrong_Text;
        public string       lblContImg2_Text;
        public bool         cbVisualizar0_Checked;
        public bool         cbGuardar0_Checked;
        public bool         cbGuardar1_Checked;
        public bool         cbVisualizar1_Checked;
        public double       largo;
        public double       ancho;
        public double       area;
        public UInt64       time_proccess;
        public bool         error_mosaiking;
        public bool         cbVisualizar2_Checked;
        public bool         cbClasificar_Checked;
        public string       pezActual;
        public string       codigoActual;
        public int          especie_num;
        public bool         cbGuardar2_Checked;
        public int          fr;

    }

    public class Especie
    {
        public string       nombre;
        public string       codigo;
        public double       a;
        public double       b;
        public string       path_guardar_imagenes;
        public int          contador = 0;
    }

   
}