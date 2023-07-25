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
using VisionModule.CLASES;
using System.Threading;
using WCFModule;


namespace VisionModule
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    public class VisionModule : csr.modules.CSRFormModule
    {   
        bool                save                    = false;
        bool                Remote_active           = true;
        bool                automatic               = false;
        bool                visualizar              = false;
        bool                forzar_categoria        = false;
        int                 ThreadsL1               = 0; //solo permitimos doce hilos de procesado simultaneosç    
        int                 Numberofprocessors      = Environment.ProcessorCount + 100;
        int                 categoria_forzada       = 0;
        int                 numProductoSave         = 0;
        int                 numProductoTotal        = 0;
        string              pathToSave              = "";
        string              Output_text             = "OK";
        List<string>        classification_names    = null;
        int[]               contadores_epecies      = null;
        ManualResetEvent    StartProcesadoEvent     = new ManualResetEvent(false);
        ManualResetEvent    StopProcesadoEvent      = new ManualResetEvent(false);
        PilaImagenes        pilaImagenes            = new PilaImagenes();         
        Procesado           procesado               = new Procesado();       
        Thread              hiloProcesado           = null;  
        DateTime            Ejecucion               = DateTime.Now;
        DateTime            imagen_anterior_tiempo  = DateTime.Now;
        ImagesToProcess     imagenesCalibracion     = new ImagesToProcess();
        ColorMatrixMachine  ColorMatrixs            = null;
        AdquisitionModule.AdquisitionModule Adquisition     = null;
        WCFModule.WCFModule                 WCFmoduleacces  = null;
        public Thread       ProcesadothreadL1       = null;
        public Thread       ProcesadothreadL2       = null;
        public Thread       TaxisCommthread         = null;
        private Object      thisLock                = new Object();
        public string clasification_path            = "";
        public int Save_images_step                 = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_id"></param>
        public VisionModule(string _id)
            : base(_id)
        { }

       /// <summary>
       /// 
       /// </summary>
       /// <returns></returns>
        public override bool Init()
        {
            bool saveimage                      = false;
            ModuleDelegates module_delegates    = new ModuleDelegates(ChangeSave,ChangeFolderPath,ExecuteFromFolder, ChangeClasificationModel, Entrenarclasificador, SaveImagesProducto, ResetContadores);
            //acceso a los modulos necesarios      
            Adquisition                     = (AdquisitionModule.AdquisitionModule)GetModule("Adquisicion");
            WCFmoduleacces                  = (WCFModule.WCFModule)GetModule("WCF");
            if (Adquisition.ConfiguracionParams.Save_image_vision == 1)
                saveimage = true;
            Save_images_step = Adquisition.ConfiguracionParams.Save_images_step;
            Adquisition.NewImages           += new AdquisitionModule.NewImages(ApilarImagenes);
            StartProcesado();


            WindowForm = new VisionForm(this, module_delegates, saveimage);
            //((VisionForm)WindowForm).Change_clasification_module(1);                
            imagen_anterior_tiempo          = DateTime.Now;                   
            Ejecucion                       = DateTime.Now;
            pathToSave                      = "CapturedProduct\\" + Ejecucion.Day.ToString() + "_" + Ejecucion.Month.ToString() + "_" + Ejecucion.Year.ToString();
            WriteConsole("Módulo cargado correctamente.", true);
            SetGlobalParameter("imagenesCalibracion", imagenesCalibracion);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {

            GC.SuppressFinalize(this);
        }

        public override bool Destroy()
        {
            StopProcesado();
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public  void HandleMessagesThread(csr.com.CSRMessage message)
        {
            string strMessage = message.ToString();
            if (strMessage == "CALIB_IMAGES_LINEALES")
            {
                ColorMatrixs = (ColorMatrixMachine)GetGlobalParameter("ColorMatrix");
                return;
            }
            if (strMessage == "NEW_CLASSIFICATOR_DETECTED")
            {
                ChangeClasificationModel(clasification_path);
                return;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void HandleMessages(csr.com.CSRMessage message)
        {

            TaxisCommthread                 = new Thread(() => HandleMessagesThread(message));
            TaxisCommthread.Priority        = ThreadPriority.Highest;
            TaxisCommthread.IsBackground    = true;
            TaxisCommthread.Start();
           
        }       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img1RGB"></param>
        /// <param name="img2RGB"></param>
        /// <param name="img3RGB"></param>
        /// <param name="img1NIR"></param>
        /// <param name="img2NIR"></param>
        /// <param name="img3NIR"></param>
        /// <param name="imgAnterior"></param>
        /// <param name="imgPosterior"></param>
        public void ProcesarImagenes(HImage img1RGB,HImage img1NIR, double ScaleX, double ScaleY, List<Sample> Samples, ImagesToProcess imagenes)
        {   
            string outputtextsegmentacion   = "Error";
           
            HImage img1RGBdisp              = null;
            HImage img1NIRdisp              = null;  
            HRegion Regions1                = img1RGB.GetDomain();
            HRegion Regions2                = img1NIR.GetDomain();     
           
            //lanzo el mensaje a la pantalla de calibracion para que sepa que está disponible si la recalibracion
            if (imagenesCalibracion.img1RGB != null)
                imagenesCalibracion.img1RGB.Dispose();
            if (imagenesCalibracion.img1NIR != null)
                imagenesCalibracion.img1NIR.Dispose();

            imagenesCalibracion.img1RGB = img1RGB;
            imagenesCalibracion.img1NIR = img1NIR;                        
               
            try
            {         
                //porcesado de las imagenes
                outputtextsegmentacion = procesado.ExecuteSegmentacion(img1RGB, img1NIR, ScaleX, ScaleY,  Samples, out  Regions1, out  Regions2);
                //escribe en la salida el texto de salida del porcesado           
                WriteConsole(outputtextsegmentacion, true);
                //genera una lista de imagenes para enviar al procesar por textura            
                try
                {
                    //escribe el area de las regiones calculadas
                    string areastrng = "Areas: " + Regions1.Area.ToString() + " , " + Regions2.Area.ToString();
                    WriteConsole(areastrng, true);
                    imagenes.img1RGB = img1RGB.ReduceDomain(Regions1);
                    imagenes.img1NIR = imagenes.img1NIR.ReduceDomain(Regions2);
                
                }
                catch
                {               
                    //imagen enviada sin roi            
                    WriteConsole("fallo en los rois", true);
                    Regions1 = img1RGB.GetDomain();
                    Regions2 = img1NIR.GetDomain();             
                }
               
                DateTime    Tinicialremote  = DateTime.Now;
                WCFmoduleacces.EnviarImagenesTexturaCliente(imagenes);
                DateTime    Tfinalremote    = DateTime.Now;
                TimeSpan    timeremote;
                timeremote                  = Tfinalremote - Tinicialremote;
                string outputtextProcess    = "Error";
                outputtextProcess         = procesado.ExecuteProcess(img1RGB, img1NIR, ScaleX, ScaleY, Samples, Regions1,  Regions2);           
                try
                {
                    int ID = numProductoSave + 1;
                    Samples[0].Features.addFeature("ID", ID);
                }
                catch
                { }                             
                //escribe en la salida el texto de salida del porcesado
                WriteConsole(outputtextProcess, true);                       
            }
            catch
            {
                //imagen enviada sin roi            
                WriteConsole("fallo en la segmentacion", true);
                Regions1 = img1RGB.GetDomain();
                Regions2 = img1NIR.GetDomain();         
            }
            imagenesCalibracion.img1RGB = imagenesCalibracion.img1RGB.ReduceDomain(Regions1);       
            imagenesCalibracion.img1NIR = imagenesCalibracion.img1NIR.ReduceDomain(Regions2);
            SendMessage("Calibracion", "CALIB_IMAGES_LINEALES");

            if (visualizar)
            {
                img1RGBdisp = img1RGB.ReduceDomain(Regions1);            
                img1NIRdisp = img1NIR.ReduceDomain(Regions2);          
                try
                {
                    Output_text = imagenes.IDproducto.ToString() + " : " + imagenes.Nombre_variable + imagenes.taxi.ToString();
                    ((VisionForm)WindowForm).DispCounter(numProductoTotal, Output_text);
                    ((VisionForm)WindowForm).DispImages(img1RGBdisp,  img1NIRdisp,  imagenesCalibracion.Linea);  
                }
                catch 
                {
                }            
            }
            Regions1.Dispose();
            Regions2.Dispose();




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
        /// 
        /// </summary>
        /// <param name="img1RGB"></param>
        /// <param name="img2RGB"></param>
        /// <param name="img3RGB"></param>
        /// <param name="img1NIR"></param>
        /// <param name="img2NIR"></param>
        /// <param name="img3NIR"></param>
        /// <param name="imgAnterior"></param>
        /// <param name="imgPosterior"></param>
        public void ProcesarImagenesEnvioRemoto(HImage img1RGB,  HImage img1NIR, double ScaleX, double ScaleY,
            out HImage img1RGBout, List<Sample> Samples, ImagesToProcess imagenes)
        {
            DateTime Tinicial1 = DateTime.Now;

            
            HImage img1RGBdisp  = null;               
            HImage img1NIRdisp  = null;         
            HRegion Regions1    = img1RGB.GetDomain();
            HRegion Regions2    = img1NIR.GetDomain();       
            img1RGBout          = img1RGB;
            //lanzo el mensaje a la pantalla de calibracion para que sepa que está disponible si la recalibracion
            if(imagenesCalibracion.img1RGB != null)
                imagenesCalibracion.img1RGB.Dispose();
            if (imagenesCalibracion.img1NIR != null)
                imagenesCalibracion.img1NIR.Dispose();
            imagenesCalibracion.img1RGB = img1RGB;    
            imagenesCalibracion.img1NIR = img1NIR;      
           
            try
            {
                //porcesado de las imagenes
                string outputtextsegmentacion = procesado.ExecuteSegmentacion(img1RGBout, img1NIR, ScaleX, ScaleY, Samples, out Regions1, out Regions2);
                //escribe en la salida el texto de salida del porcesado
                WriteConsole(outputtextsegmentacion, true);

                DateTime TFinal1 = DateTime.Now;
                TimeSpan timeremote1;
                timeremote1 = TFinal1 - Tinicial1;
                WriteConsole("Segmentacion time:" + timeremote1.TotalMilliseconds.ToString(), true);
                //genera una lista de imagenes para enviar al procesar por textura
                DateTime Tinicialremote = DateTime.Now;
                try
                {
                    //escribe el area de las regiones calculadas
                    string areastrng = "Areas: " + Regions1.Area.ToString() + " , " + Regions2.Area.ToString() ;
                    WriteConsole(areastrng, true);
                    imagenes.img1RGB            = img1RGBout.ReduceDomain(Regions1);                              
                    imagenes.img1NIR            = img1NIR.ReduceDomain(Regions2);
                   
                }
                catch
                {
                    //imagen enviada sin roi            
                    WriteConsole("fallo en los rois", true);
                     Regions1 = img1RGB.GetDomain();
                     Regions2 = img1NIR.GetDomain();                
                }               
               
                WCFmoduleacces.EnviarImagenesTexturaCliente(imagenes);
                DateTime Tfinalremote = DateTime.Now;
                TimeSpan timeremote;
                timeremote = Tfinalremote - Tinicialremote;
                WriteConsole("sending time:" + timeremote.TotalMilliseconds.ToString(), true);    

            }
            catch 
            {
                //imagen enviada sin roi            
                WriteConsole("fallo en la segmentación", true);
                Regions1 = img1RGB.GetDomain();
                Regions2 = img1NIR.GetDomain();              
            }

            DateTime Tinicialtest = DateTime.Now;
            imagenesCalibracion.img1RGB = imagenesCalibracion.img1RGB.ReduceDomain(Regions1);            
            imagenesCalibracion.img1NIR = imagenesCalibracion.img1NIR.ReduceDomain(Regions2);         
            SendMessage("Calibracion", "CALIB_IMAGES_LINEALES");

            if (visualizar)
            {
                img1RGBdisp     = img1RGBout.ReduceDomain(Regions1);                             
                img1NIRdisp     = img1NIR.ReduceDomain(Regions2);                          
                try
                {
                    Output_text = imagenes.IDproducto.ToString() + " : " + imagenes.Nombre_variable + imagenes.taxi.ToString();
                    ((VisionForm)WindowForm).DispCounter(numProductoTotal, Output_text);              
                   ((VisionForm)WindowForm).DispImages(img1RGBdisp, img1NIRdisp,imagenesCalibracion.Linea);
                  
                }
                catch 
                {
                }

            }                   
                      
            DateTime Tfinalremotetest = DateTime.Now;
            TimeSpan timeremotetest;
            timeremotetest = Tfinalremotetest - Tinicialtest;
            WriteConsole("visual time:" + timeremotetest.TotalMilliseconds.ToString(), true);
            Regions1.Dispose();
            Regions2.Dispose();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img1RGB"></param>
        /// <param name="img1NIR"></param>
      
        public void ProcesarImagenesTextura( List<Sample> Samples, ImagesToProcess imagenes)
        {
            
            HImage img1RGBdisp  = null;
            HImage img1NIRdisp  = null;
            HRegion Regions1    = imagenes.img1RGB.GetDomain();
            HRegion Regions2    = imagenes.img1NIR.GetDomain();
               
            try
            {
                 string outputtextProcess = procesado.ExecuteProcess(imagenes.img1RGB, imagenes.img1NIR, 1, 1, Samples,Regions1, Regions2);
                //escribe en la salida el texto de salida del porcesado
                 WriteConsole(outputtextProcess, true);

                try
                {
                    int ID = numProductoSave + 1;
                    Samples[0].Features.addFeature("ID", ID);
                }
                catch 
                { }

            }
            catch 
            {
                //imagen enviada sin roi            
                WriteConsole("fallo en el processo ", true);
                Regions1 = imagenes.img1RGB.GetDomain();
                Regions2 = imagenes.img1NIR.GetDomain();
           
            }
            if (visualizar)
            {

                img1RGBdisp = imagenes.img1RGB.ReduceDomain(Regions1);                   
                img1NIRdisp = imagenes.img1NIR.ReduceDomain(Regions2);                
       
                try
                {
                    Output_text = imagenes.IDproducto.ToString() + " : " + imagenes.Nombre_variable + imagenes.taxi.ToString();
                    ((VisionForm)WindowForm).DispCounter(numProductoTotal, Output_text);
                    ((VisionForm)WindowForm).DispImages(img1RGBdisp,img1NIRdisp, imagenesCalibracion.Linea);              
                }
                catch
                {
                }

            }
            Regions1.Dispose();
            Regions2.Dispose();


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imgRGB1"></param>
        /// <param name="imgNIR1"></param>
        /// 
        /// <summary>
        /// /////////
        /// </summary>
        /// <param name="img"></param>
        /// <param name="modo"></param>
        public int SaveImagesProducto(HImage imgRGB1, HImage imgNIR1, int linea, int categoriaout, Features Instance)
        {

            //instancia a partir de las features obtenidas del procesado
            Features Instancecopied = Instance;
            HImage imgRGB1tmp = null, imgNIR1tmp = null;
            if(imgRGB1 != null)
                imgRGB1tmp = imgRGB1.CopyImage();
            if (imgNIR1 != null)
                imgNIR1tmp = imgNIR1.CopyImage();
            Thread ProcesadothreadL1;
            ProcesadothreadL1 = new Thread(() => this.SaveImagesProductoThread(imgRGB1tmp, imgNIR1tmp,  linea,  categoriaout, Instancecopied));
            ProcesadothreadL1.Name = "SaveImageVision";
            ProcesadothreadL1.Priority = ThreadPriority.BelowNormal;
            ProcesadothreadL1.IsBackground = true;
            ProcesadothreadL1.Start();
            return(0);


        }

        public int SaveImagesProductoThread(HImage imgRGB1,HImage imgNIR1, int linea, int categoriaout, Features Instance)
        {
            int productoID = 0;
            string classtype = "";
            Save_images_step = Adquisition.ConfiguracionParams.Save_images_step;
            lock (thisLock)
            {
                this.numProductoSave++;
                productoID = this.numProductoSave;
                Ejecucion = DateTime.Now;
                pathToSave = "CapturedProduct\\" + Ejecucion.Day.ToString() + "_" + Ejecucion.Month.ToString() + "_" + Ejecucion.Year.ToString();
            }

            if (forzar_categoria == true)
                classtype = "Trained";
            else
                classtype = "Production";

            string categoriatext    = classification_names[categoriaout];
            long Time_long_ID       = Ejecucion.Ticks / TimeSpan.TicksPerMillisecond;
            string timeID           = (Ejecucion.Ticks / TimeSpan.TicksPerMillisecond).ToString();        
            string path             = pathToSave + "\\" + classtype + "\\" + categoriatext + "\\";
            string path10before     = "CapturedProduct\\" + Ejecucion.Day.ToString() + "_" + Ejecucion.Month.ToString() + "_" + Ejecucion.Year.ToString();
            DateTime time10daysbefore;
            DateTime time1daysbefore = new DateTime(1976, 6, 12, 0, 0, 0, 0);
            DateTime time2daysbefore = new DateTime(1976, 6, 22, 0, 0, 0, 0);
            TimeSpan diff10days;
            diff10days = time2daysbefore - time1daysbefore;
           

            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                    time10daysbefore = Ejecucion.Subtract(diff10days);
                    path10before = "CapturedProduct\\" + time10daysbefore.Day.ToString() + "_" + time10daysbefore.Month.ToString() + "_" + time10daysbefore.Year.ToString();
                    //delete the folder of the 10 days before and all the files
                    if (Directory.Exists(path10before))
                        Directory.Delete(path10before, true);
                }
                if (Save_images_step <= 0)
                    Save_images_step = 1;
                int saveImage = numProductoSave % Save_images_step;

                if (imgRGB1 != null && saveImage == 0)
                {
                    imgRGB1 = imgRGB1.FullDomain();
                    imgRGB1.WriteImage("jpg", 0, path  + "RGB_" + timeID);
                }
               
                            
                if (imgNIR1 != null && saveImage == 0)
                {
                    imgNIR1 = imgNIR1.FullDomain();
                    imgNIR1.WriteImage("jpg", 0, path  + "NIR_" + timeID);
                }
                string Path_File_Arff = pathToSave + "\\" + classtype + "\\" + "Instancias_imagenes.arff";
                if (Instance != null && saveImage == 0)
                {
                    Instance.addFeature("ID",0.0, Time_long_ID);
                    Features.StoreToFileArff(Path_File_Arff, Instance, classification_names);
                }



                if (saveImage == 0)
                    WriteConsole("Imagen guardada ID: " + timeID, true);
                //guarda la instancia.
                
                
            }
            catch (Exception e)
            {
                WriteConsole("Error Imagen guardada ID: " + this.numProductoSave.ToString(), true);
                WriteConsole("Error  " + e.ToString(), true);

            }
            if(imgRGB1 != null)
                imgRGB1.Dispose();
            if (imgNIR1 != null)
                imgNIR1.Dispose();
            return (this.numProductoSave);
             
        }
      /// <summary>
      /// 
      /// </summary>
      /// <param name="imagenes"></param>
        public void GestionColaImagenesCalibracion(out ImagesToProcess imagenes)
        {
            try
            {
                #region GestionColaImagenes              
                imagenes                        = null;
                //lectura de encoder              
                //procesa la lista de porcesado
                ImagesToProcess imagenesInit    = pilaImagenes.Last();                                  
                //comprueba que las imagenes tienen encoders dentro de los margenes tolerables de error
                if (imagenesInit != null)
                {               
                    imagenesInit            = pilaImagenes.Desapilar();
                    HTuple MatColorMatrix1  = null;
                    //correccion de color 
                    try
                    {
                       //calibra el color 
                        if (ColorMatrixs != null && Adquisition.CalibrationActive)
                        {
                            MatColorMatrix1 = ColorMatrixs.ColorMatrix1_l1.GetFullMatrix();
                            imagenesInit.img1RGB = imagenesInit.img1RGB.LinearTransColor(MatColorMatrix1).ConvertImageType("byte");
                        }

                    }
                    catch
                    {
                        WriteConsole("Error: color matrix error");
                    }
                   
                    if (WCFmoduleacces.clientActive())
                        imagenes = imagenesInit;
                    else
                        WCFmoduleacces.ApilarImagentextura(imagenesInit);
                }        
                #endregion
            }
            catch(Exception ex)
            {
                WriteConsole("Gestion de colas fallo: " + ex.ToString(), true);
                imagenes = null;
            }

            }
        /// <summary>
        /// 
        /// </summary>
        public void  ProcessVision1(ImagesToProcess imagenes)
        {
                          
            int categoriaout = 0; 
            DateTime Tfinaltwincat      = DateTime.Now;
            DateTime Tinicialtwincat    = DateTime.Now;
            TimeSpan timenocom          = Tfinaltwincat - Tinicialtwincat;
            List<Sample> Samples;
            //comprobar que está todo sincronizado siempre
            //comparo pilas de taxis e imagenes            
            if (imagenes != null)
           {
              // GC.Collect();
                numProductoTotal++;               
                TimeSpan interval;
                DateTime TinicioTotal       = DateTime.Now;
                DateTime TinicioCalibracion = DateTime.Now;                                 
                imagenesCalibracion.Linea   = imagenes.Linea;
                try
                {
                   
                    Samples                     = new List<Sample>();
                    DateTime TfinalCalibracion  = DateTime.Now;
                    DateTime TinicioProcessado  = DateTime.Now;
                    if (Remote_active == true)
                    { 
                       ProcesarImagenesEnvioRemoto(imagenes.img1RGB, imagenes.img1NIR, 1, 1,
                       out  imagenes.img1RGB, Samples, imagenes);
                    }
                    DateTime TfinalProcessado = DateTime.Now;
                    //comunica el resultado al PLC 
                    DateTime TinicioComunicacion = DateTime.Now;
                    if (Remote_active == false)
                    {
                        
                        ComunicaResultado(Samples[0].Features, imagenes.taxi, imagenes.Nombre_variable, out categoriaout, imagenes.parametros);
                    }
                   
                    if (visualizar)
                    {
                        try
                        {
                            if (Samples.Count() > 0)
                            {
                                Features features = Samples[0].Features;
                                ((VisionForm)WindowForm).UpdateDataGridView1(features);
                            }

                        }
                        catch
                        {
                        }
                    }

                    Samples.Clear();

                    DateTime TfinalComunicacion = DateTime.Now;
                    DateTime TfinalTotal        = DateTime.Now;
                    TimeSpan Procesadotime;
                    TimeSpan Comunicaciointime;
                    TimeSpan calibraciontime;
                    interval                    = TfinalTotal - TinicioTotal;                   
                    calibraciontime             = TfinalCalibracion - TinicioCalibracion;                  
                    Procesadotime               = TfinalProcessado - TinicioProcessado;                    
                    Comunicaciointime           = TfinalComunicacion - TinicioComunicacion;
                    //tiempos de procesado
                    string texto_debug;
                    texto_debug = "Calibracion: " + calibraciontime.TotalMilliseconds.ToString() + " Procesado: "
                        + Procesadotime.TotalMilliseconds.ToString() + " Comunicacion: " + Comunicaciointime.TotalMilliseconds.ToString() +
                        " Total: " + interval.TotalMilliseconds.ToString();
                    //saca por consola los textos
                    WriteConsole(texto_debug, true);
                    Output_text = imagenes.IDproducto.ToString() + " : " + imagenes.Nombre_variable + imagenes.taxi.ToString();
                    WriteConsole(Output_text, true);                
                }
                catch
                {         
                }
                //graba la imagenes 
                if (save && numProductoTotal > 1)
                {                   
                    SaveImagesProducto(imagenes.img1RGB, imagenes.img1NIR, imagenes.Linea, categoriaout, null);
                }                                   
                DateTime timepo_captura_imagen  = imagenes.TimeStamp;                        
                TimeSpan tiempo_total_entre_imagen = imagenes.TimeStamp - imagen_anterior_tiempo;
                imagen_anterior_tiempo = imagenes.TimeStamp;

                imagenes.Dispose();
                imagenes = null;
            }
            //parar que le gabage collector elimie  la memoria       
         
        
            ThreadsL1--;
            //GC.Collect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imagenes"></param>
        public void ProcessVision2(ImagesToProcess imagenes)
        {
            if (imagenes == null)
            {
                ThreadsL1--;
                return;
            }
            GC.Collect();
            numProductoTotal++;
            int categoriaout                        = 0;
            DateTime Tfinaltwincat                  = DateTime.Now;
            DateTime Tinicialtwincat                = DateTime.Now;
            TimeSpan interval;
            TimeSpan timenocom                      = Tfinaltwincat - Tinicialtwincat;   
            DateTime TinicioTotal                   = DateTime.Now;
            DateTime TinicioCalibracion             = DateTime.Now;
            List<Sample> Samples                    = new List<Sample>();
           
            try
            {
               
                                          
                DateTime TfinalCalibracion  = DateTime.Now;
                //procesado del imagen mediante procedimientos Halcon
                DateTime TinicioProcessado = DateTime.Now;          
                ProcesarImagenesTextura(Samples, imagenes);
                DateTime TfinalProcessado = DateTime.Now;
                DateTime TinicioComunicacion = DateTime.Now;
             
                ComunicaResultado(Samples[0].Features, imagenes.taxi, imagenes.Nombre_variable, out categoriaout, imagenes.parametros);

                if (visualizar)
                {
                    try
                    {
                        if (Samples.Count() > 0)
                        {
                            Features features = Samples[0].Features;
                            ((VisionForm)WindowForm).UpdateDataGridView1(features);
                        }
                    }
                    catch
                    { }
                }

              
                DateTime TfinalComunicacion = DateTime.Now;
                DateTime TfinalTotal        = DateTime.Now;                             
                TimeSpan Procesadotime;
                TimeSpan Comunicaciointime;
                interval            = TfinalTotal - TinicioTotal;
                Procesadotime       = TfinalProcessado - TinicioProcessado;              
                Comunicaciointime   = TfinalComunicacion - TinicioComunicacion;
                //tiempos de procesado
                string texto_debug;
                texto_debug = " Procesado textura: "
                    + Procesadotime.TotalMilliseconds.ToString() + " Comunicacion: " + Comunicaciointime.TotalMilliseconds.ToString() +
                    " Total: " + interval.TotalMilliseconds.ToString();
                //saca por consola los textos
                WriteConsole(texto_debug, true);
                Output_text = imagenes.IDproducto.ToString() + " : " + imagenes.Nombre_variable + imagenes.taxi.ToString();
                WriteConsole(Output_text, true);
            }catch(Exception ex)
            {
                string textoout = ex.ToString();
            }
            //graba la imagenes 
            if (save && numProductoTotal > 1)
            {

                SaveImagesProducto(imagenes.img1RGB, imagenes.img1NIR, imagenes.Linea, categoriaout, Samples[0].Features);
            }

            ThreadsL1--;
            imagenes.Dispose();      
            imagenes = null;
            Samples.Clear();
            Samples = null;
            GC.Collect();
        }
        /// <summary>
        /// comunica el resultado al ordenador central
        /// </summary>
        /// <param name="FeaturesProducto"></param>
        /// <param name="taxi"></param>
        /// <param name="linea"></param>
        private void ComunicaResultado(Features FeaturesProducto, int taxi, string Nombre_variable, out int categoriaout,  Parametros_Process parametros_result  )
        {
            int         categoria           = 10; //por defecto 10 fallo procesado         
            string      OutputText          = "";
            string      textout             = " ";
            UInt64 timestampprop            = 0;
            UInt64 actualsystemtime         = 0;
            categoriaout                    = 0;
            try
            {
                List<string> namesToSelect = new List<string>();
                List<Feature> FeatureToSelect = new List<Feature>();            
                namesToSelect.Add("Categoria_asignada");
                FeaturesProducto.Select(namesToSelect);
                FeatureToSelect = FeaturesProducto.GetSelected();
                categoria = Convert.ToInt16(FeatureToSelect[0].Value);
                namesToSelect.Clear();       
                              
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
                    categoria = categoria_forzada;

               
            }
            catch
            {
                categoria = 10; //por defecto 10 fallo procesado            
                WriteConsole("fallo en el procesado",true);
                textout = "localization failure: ";
            }
            categoriaout = categoria;

            if (categoria < 10)
            {
                textout = "Type of fish: " + classification_names[categoriaout].ToString();
                contadores_epecies[categoriaout]++;
            }
                
            else
                textout = "fallo en el procesado";

            //Adquisition.PrintOut(textout);
            //imprime en la pantalla de aquisicion los contadores
            textout += "\r\n------------\r\n";
           
            for(int i = 0; i < classification_names.Count(); i++)
            {
                textout += classification_names[i].ToString() + " --> " + contadores_epecies[i].ToString() + "\r\n";
               
            }
            textout += "------------\r\n";
            Adquisition.PrintOut(textout, false);

            //comunicacion si o si        
            int count       = 0;
            //while (comok == false && count < 4)
            { 
                //comunicacion del resultado                           
                count++;
                if (forzar_categoria == true)
                    categoria = categoria_forzada;

               
                try
                {
                    IO_Parameters io = (IO_Parameters)GetGlobalParameter("IO");
                    string Variable_IO = " ";
                    string Variable_IO_Fisrt = " ";
                    string Variable_IO_second = " ";

                    OutputText = "Type of fish: " + classification_names[categoria].ToString(); ;
                    WriteConsole(OutputText,true);                 
                    Variable_IO_Fisrt = "GVL.CSR_LINE_" + taxi.ToString();
                    Variable_IO_second = "_SPECIES_TYPE";
                    Variable_IO = Variable_IO_Fisrt + Variable_IO_second;                        
                    io.Out[Variable_IO].Write(OutputText);  
                          
                    Variable_IO_second = "_SPECIES_CODE";
                    Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                    io.Out[Variable_IO].Write((UInt32)categoria);
                    
                    // se envia en mm y mm2
                    Variable_IO_second = "_LENGTH";
                    Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                    io.Out[Variable_IO].Write((UInt32)parametros_result.fParam1*10);
                  
                    Variable_IO_second = "_WIDTH";
                    Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                    io.Out[Variable_IO].Write((UInt32)parametros_result.fParam2*10);
                   
                    Variable_IO_second = "_AREA";
                    Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                    io.Out[Variable_IO].Write((UInt32)parametros_result.fParam3*100);

                    Variable_IO = "GVL.CSR";
                    Variable_IO += "_TIME_STAMP";
                    actualsystemtime = Convert.ToUInt64(io.Out[Variable_IO].Value);
                    timestampprop = actualsystemtime - Convert.ToUInt64(parametros_result.fParam4);

                    Variable_IO_second = "_PROP_TIME";
                    Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                    io.Out[Variable_IO].Write((UInt32)timestampprop);

                    Variable_IO_second = "_POS";
                    Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                    io.Out[Variable_IO].Write((UInt16)parametros_result.iParam2);

                    Variable_IO_second = "_COUNTER";
                    Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                    io.Out[Variable_IO].Write((UInt16)parametros_result.iParam1);
                   

                }
                catch (Exception e)
                {
                   
                    //SendMessage("Communication", "REINIT_COMM");
                    WriteConsole("fallo en la comunicación del resultado" + e.Message,true);
                    timestampprop = actualsystemtime - (UInt64)Math.Round((double)DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond);
                }   
                
                            
          }
            textout = "Total processing time: " + timestampprop.ToString();
            WriteConsole(textout, true);
            


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
            procesado.CargarImagenes(path);
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
                WriteConsole(output, true);
                classification_names = procesado.Get_Class_names();
                contadores_epecies = new int[classification_names.Count()];
                for (int i = 0; i < classification_names.Count(); i++)
                    contadores_epecies[i] = 0;
                return classification_names;
            }
            catch 
            {
                WriteConsole("Modelo de clasificacion no cargado correctamente",true);
                classification_names = null;
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>        
        private void ExecuteFromFolder(bool calibrate)
        {
            //activar para leer las imagenes              
            bool        test_captura    = false;
            List<Sample> Samples        = null;
            int         linea           = 0;
            HImage Image1RGB = new HImage();      
            HImage Image1NIR = new HImage();
            TimeSpan interval;
            DateTime Tinicio = DateTime.Now;
            double ScaleX = 1 , ScaleY = 1;
            imagenesCalibracion.Linea   = 0;
            int categoriaout            = 0;
            long ID                     = 0;
            //ejecuta todas las imagames a la vez
            for ( int i = 0; i < procesado.numero_imagenes_folder;  i++ )
            {
                int categoriaread = 0;
                bool forzada = false;
                bool lectura_ok = false;
                lectura_ok = procesado.LeerImagenes(out Image1RGB, out Image1NIR,null, out ScaleX, out ScaleY, out linea, out ID, out forzada, out categoriaread);

                //lectura_ok = false;
                if (lectura_ok)
                {
                    //aplicar la calibracion se procede
                    if (calibrate == true)
                    {
                    
                        //calibra el color 
                        if (ColorMatrixs != null)
                        {
                            Image1RGB = Image1RGB.LinearTransColor(ColorMatrixs.ColorMatrix1_l1.GetFullMatrix()).ConvertImageType("byte");
                        }

                    }

                    imagenesCalibracion.Linea = Adquisition.ConfiguracionParams.Line_ID;

                    //calculo de la escala de las imagenes
                    int Dimx, DimY;
                    Image1RGB.GetImageSize(out Dimx, out DimY);                
                    test_captura = false;                                
                    if (test_captura == false)
                    {                                       
                        Samples                         = new List<Sample>();
                        ImagesToProcess imagenestextura = new ImagesToProcess();
                        imagenestextura.img1RGB         = Image1RGB;
                        imagenestextura.img1NIR         = Image1NIR;                
                        imagenestextura.Linea           = imagenesCalibracion.Linea;                   
                        numProductoTotal++;

                        //imagenes procesadas
                               
                        ProcesarImagenes(Image1RGB, Image1NIR, ScaleX, ScaleY, Samples, imagenestextura); //lgcorzo    
                        try
                        {
                            if (numProductoTotal > 0)
                           ComunicaResultado(Samples[0].Features, imagenestextura.Linea, null, out categoriaout, imagenestextura.parametros);
                            if (visualizar)
                            {                         
                                if(Samples.Count() > 0)
                                {
                                    Features features = Samples[0].Features;
                                    ((VisionForm)WindowForm).UpdateDataGridView1(features);
                                }                                                                              
                            }
                        }
                        catch
                        { }

                        if (save && numProductoTotal > 1)
                        {
                            SaveImagesProducto(Image1RGB, Image1NIR, imagenesCalibracion.Linea, categoriaout, Samples[0].Features);
                        }
                    }                    
                    else
                    {
                        ImagesToProcess imagenes = new ImagesToProcess();
                        imagenes.img1NIR = Image1NIR;
                        imagenes.img1RGB = Image1RGB;           
                        imagenes.Linea = 1;
                        ApilarImagenes(imagenes);
                    }                 
                }
                DateTime Tfinal = DateTime.Now;
                interval = Tfinal - Tinicio;
                Output_text = interval.TotalMilliseconds.ToString() + " mseg ";
                Image1RGB.Dispose();
                Image1NIR.Dispose();

                Thread.Sleep(225);
              

                if (this.automatic == false)
                    break;
            }
           

        }

        #region apilado
        public void ApilarImagenes(ImagesToProcess imagenes)
        {
            try
            {              
                pilaImagenes.Apilar(imagenes);
            }
            catch
            {
                throw;
            }
            
        }
        #endregion


        #region HILO PROCESADO

        public void StopProcesado()
        {
            try
            {
                StartProcesadoEvent.Reset();
                StopProcesadoEvent.Set();
            }
            catch 
            {
                throw;
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
                hiloProcesado.Name = "procesado";
                hiloProcesado.Priority = ThreadPriority.Normal;
                hiloProcesado.IsBackground = true;

                hiloProcesado.Start();

            }
            catch 
            {
                throw;
            }
        }

        private void Procesado()
        {
            string textout = "";
           
                StartProcesadoEvent.Set();
                StopProcesadoEvent.Reset();
          
            while (Status != csr.modules.CSRModuleStatus.Closing && Status != csr.modules.CSRModuleStatus.Closed)
            {

                Thread.Sleep(10);
                try
                    {
                    TimeSpan interval;
                    DateTime Tinicio = DateTime.Now;
                    ImagesToProcess imagenesTh1;
                    GestionColaImagenesCalibracion(out imagenesTh1);
                    if (ThreadsL1 < Numberofprocessors && imagenesTh1 != null)
                    {
                        //funcion de gestion de las colas de las imagenes y taxis GestionColasProcess(out imagenesTh1)
                      
                        ThreadsL1++;                     
                        ProcesadothreadL1 = new Thread(() => ProcessVision1(imagenesTh1));
                        ProcesadothreadL1.Name = "ProcessVision1";
                        ProcesadothreadL1.Priority = ThreadPriority.Highest;
                        ProcesadothreadL1.IsBackground = true;
                        ProcesadothreadL1.Start();
                        textout = "ProcessVision1: " + ThreadsL1.ToString();
                        WriteConsole(textout, true);  
                    }
                    ImagesToProcess imagenesTh2 = WCFmoduleacces.DesapilarImagentextura();
                    if (ThreadsL1 < Numberofprocessors && imagenesTh2 != null)
                    {
                        //procesa las lista de textura  lgcorzo
                       
                        if (imagenesTh2 != null && imagenesTh2 != null)
                        {
                            numProductoTotal++;
                        }
                        ThreadsL1++;
                        ProcesadothreadL2 = new Thread(() => ProcessVision2(imagenesTh2));
                        ProcesadothreadL2.Name = "ProcessVision2";
                        ProcesadothreadL2.Priority = ThreadPriority.Highest;
                        ProcesadothreadL2.IsBackground = true;
                        ProcesadothreadL2.Start();                
                        textout = "ProcessVision2: " + ThreadsL1.ToString();
                        WriteConsole(textout, true);


                    }
            
                       if (StopProcesadoEvent.WaitOne(5, true))
                        {
                            StopProcesadoEvent.Reset();
                            StartProcesadoEvent.Reset();
                            break;
                        }
                 
                    DateTime Tfinal = DateTime.Now;
                    interval = Tfinal - Tinicio;
                    string Output_text_debug = interval.TotalMilliseconds.ToString() + " mseg  hilo Vision";            
                }
                catch 
                {
                    throw ;
                }
            }
            
           
        }
        #endregion
    }
    //////////////////////////////////////////////////////////////
    public delegate void ChangeSaveDelegate(bool save, int categoria, bool automatic, bool visualizar, bool forzar_categoria, int categoria_forzada);
    public delegate void ChangeFolderPathDelegate(string path);
    public delegate void ExecuteFromFolderDelegate(bool calibrate);
    public delegate List<string> CargaModeloClasificacion(string path);
    public delegate void GenerarModeloClasificacion(string patharff, string pathmodelo);
    public delegate int SaveImagetrained(HImage imgRGB1, HImage imgNIR1 , int linea, int categoriaout, Features Instancia);
    public delegate void ResetContadores();

    //////////////////////////////////////////////////////////////
    public class ModuleDelegates
    {

        public ChangeSaveDelegate ChangeSave;
        public ChangeFolderPathDelegate ChangeFolderPath;
        public ExecuteFromFolderDelegate ExecuteFromFolder;
        public CargaModeloClasificacion CargaModeloClasificacion;
        public GenerarModeloClasificacion GenerarModeloClasificacion;
        public SaveImagetrained SaveImagetrained;
        public ResetContadores ResetContadores;


        //////////////////////////////////////////////////////////////
        public ModuleDelegates( ChangeSaveDelegate pChangeSave, ChangeFolderPathDelegate pChangeFolderPath, ExecuteFromFolderDelegate pExecuteFromFolder, CargaModeloClasificacion pCargaModeloClasificacion,
            GenerarModeloClasificacion pGenerarModeloClasificacion, SaveImagetrained pSaveImagetrained, ResetContadores pResetContadores)
        {
            ChangeSave                  = pChangeSave;
            ChangeFolderPath            = pChangeFolderPath;
            ExecuteFromFolder           = pExecuteFromFolder;
            CargaModeloClasificacion    = pCargaModeloClasificacion;
            GenerarModeloClasificacion = pGenerarModeloClasificacion;
            SaveImagetrained            = pSaveImagetrained;
            ResetContadores             = pResetContadores;

        }

        //////////////////////////////////////////////////////////////
       


    }

}
