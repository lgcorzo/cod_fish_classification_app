using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ProcesadoEspecies;
using AdquisitionModule;
using HalconDotNet;
using System.IO;
using TwincatModule;

namespace CalibracionModule
{
    public class CalibracionModule : csr.modules.CSRFormModule
    {
        bool                capturaActivada = false;
        string              path            = "conf\\Patrones\\";
        string              pathCam1        = "Cam1\\";  
        ImagesToProcess     imagenes        = null;
        ColorMatrixMachine  ColorMatrixs    = null;
        HTuple              Matrix1         = null;   
        AdquisitionModule.AdquisitionModule Adquisition         = null;
        HDevProcedureCall   ProcedureCallCalibracion    = null;
        HDevProcedure       HalconProcedureCalibracionLineales  = null;
     

        private static HDevEngine HalconEngine = new HDevEngine();  
        //////////////////////////////////////////////////////////////////////////////////
        public CalibracionModule(string _id)
            : base(_id)
        { }
        //////////////////////////////////////////////////////////////////////////////////
        public override bool Init()
        {
            base.Init();
            ModuleDelegates module_delegates    = new ModuleDelegates(Blancos, Macbech, Fondo, Correccion,ForzarCaptura,ActivarCaptura,Calibracion, Reset_calibracion, CorrecionRGB, Actualizar, ActualizarMatriz,  ActualizaTexto, BlancosPatron);
            WindowForm                          = new CalibracionForm(this, module_delegates);         
            Adquisition                         = (AdquisitionModule.AdquisitionModule)GetModule("Adquisicion");
            HalconEngine.SetProcedurePath("Procedimientos//");
            HalconProcedureCalibracionLineales  = new HDevProcedure("SegmentImageMacBeth");
            ProcedureCallCalibracion    = new HDevProcedureCall(HalconProcedureCalibracionLineales);
          
            //crea los directorios si no estan creados
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            if (!Directory.Exists(path + pathCam1))
                Directory.CreateDirectory(path + pathCam1);       
            ColorMatrixs                    = new ColorMatrixMachine();
            ColorMatrixs.ColorMatrix1_l1    = new HMatrix();
          
            //lectura de los ficheros de matrices si existen
            try
            {
                ColorMatrixs.ColorMatrix1_l1.ReadMatrix(path + "ColorMatrix1_l1");              
            }
            catch 
            {
                HTuple valuesInitMatrix = new HTuple(1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
                ColorMatrixs.ColorMatrix1_l1.CreateMatrix(3, 4, 0.0);               
                ColorMatrixs.ColorMatrix1_l1.SetFullMatrix(valuesInitMatrix);              
            }          
            SetGlobalParameter("ColorMatrix", ColorMatrixs);      
            //Envia el mensaje para notificar que la matriz de calibracion ya está lista
            SendMessage("Vision", "CALIB_IMAGES_LINEALES");
            //actualiza el texto en las pantallas
            ActualizaTexto();
            WriteConsole("Módulo cargado correctamente.", true);
            return true;
        }

        public void ActualizaTexto()
        {
            string texto    = "RoffsetL1: " + Math.Round(ColorMatrixs.ColorMatrix1_l1.GetValueMatrix(0, 3), 1).ToString();
            texto           += " GoffsetL1: " + Math.Round(ColorMatrixs.ColorMatrix1_l1.GetValueMatrix(1, 3), 1).ToString();
            texto           += " BoffsetL1: " + Math.Round(ColorMatrixs.ColorMatrix1_l1.GetValueMatrix(2, 3), 1).ToString();
            //mostrar los resultados en la pantalla
            ((CalibracionForm)WindowForm).WritetxtoResultados(texto, 1, true);
            texto = "R11: " + Math.Round(ColorMatrixs.ColorMatrix1_l1.GetValueMatrix(0, 0), 1).ToString();
            texto += " R12: " + Math.Round(ColorMatrixs.ColorMatrix1_l1.GetValueMatrix(0, 1), 1).ToString();
            texto += " R13: " + Math.Round(ColorMatrixs.ColorMatrix1_l1.GetValueMatrix(0, 2), 1).ToString();
            ((CalibracionForm)WindowForm).WritetxtoResultados(texto, 1, false);
            texto = "R21: " + Math.Round(ColorMatrixs.ColorMatrix1_l1.GetValueMatrix(1, 0), 1).ToString();
            texto += " R22: " + Math.Round(ColorMatrixs.ColorMatrix1_l1.GetValueMatrix(1, 1), 1).ToString();
            texto += " R23: " + Math.Round(ColorMatrixs.ColorMatrix1_l1.GetValueMatrix(1, 2), 1).ToString();
            ((CalibracionForm)WindowForm).WritetxtoResultados(texto, 1, false);
            texto = "R31: " + Math.Round(ColorMatrixs.ColorMatrix1_l1.GetValueMatrix(2, 0), 1).ToString();
            texto += " R32: " + Math.Round(ColorMatrixs.ColorMatrix1_l1.GetValueMatrix(2, 1), 1).ToString();
            texto += " R33: " + Math.Round(ColorMatrixs.ColorMatrix1_l1.GetValueMatrix(2, 2), 1).ToString();
            ((CalibracionForm)WindowForm).WritetxtoResultados(texto, 1, false);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="images"></param>
        public void NuevasImagenes(ImagesToProcess images)
        {           
            this.imagenes = images;
            ((CalibracionForm)WindowForm).CleanDispImages();
            ((CalibracionForm)WindowForm).DispImagesArea(imagenes.img1RGB, imagenes.img1NIR, imagenes.Linea);
            //desactiva la notificacion del evento            
            Adquisition.NewImagesRaw -= NuevasImagenes;   

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public override void HandleMessages(csr.com.CSRMessage message)
        {
            bool calibracionrequerida = capturaActivada;

            if (message.ToString() == "CALIB_IMAGES_LINEALES" && calibracionrequerida)
            {
                ((CalibracionForm)WindowForm).CleanDispImages();
                imagenes = (ImagesToProcess)GetGlobalParameter("imagenesCalibracion");        
                //guarda la imagenes de calibracion
                if (((CalibracionForm)WindowForm).GetSaveImage() == true)
                    SaveImagesProducto(imagenes.img1RGB, imagenes.img1NIR, imagenes.Linea);      
                ((CalibracionForm)WindowForm).DispImagesArea(imagenes.img1RGB, imagenes.img1NIR, imagenes.Linea);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void ForzarCaptura()
        {
            Adquisition.SoftTrigger();
            //activa la notificacion del evento         
            Adquisition.NewImagesRaw += new AdquisitionModule.NewImagesRaw(NuevasImagenes);            

        }
        /// <summary>
        /// 
        /// </summary>
        public void ActivarCaptura()
        {
            capturaActivada = !capturaActivada;
            if (capturaActivada  == true)
            {

                Adquisition.Activatecalibration(false);                          
            }         
            else
            {
                Adquisition.Activatecalibration(true);
                                                                 
            }
           
            ((CalibracionForm)WindowForm).CapturaActivada(capturaActivada);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public bool Blancos(int cam)
        {
            const string message =
            "Are you sure that you want to  replace the files?";
            const string caption = "Form Closing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            // If the no button was pressed ...
            if (result == DialogResult.No)
                return (true);         
            //GUARDAR IMÁGENES BLANCO LINEALES
            HTuple a, b;
            imagenes.img1NIR.GetImageSize(out a, out b);
            imagenes.img1RGB.WriteImage("jpg", 0, path + "Cam1\\RGB_blanco");
            imagenes.img1NIR.WriteImage("jpg", 0, path + "Cam1\\NIR_blanco");
            // recarga los blancos para la aquisicion
            Adquisition.CargaBlancos();
            return true;  
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public bool BlancosPatron(int cam)
        {
            const string message =
            "Are you sure that you want to  replace the files?";
            const string caption = "Form Closing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            // If the no button was pressed ...
            if (result == DialogResult.No)
                return (true);
            //GUARDAR IMÁGENES BLANCO LINEALES
            HTuple a, b;
            imagenes.img1NIR.GetImageSize(out a, out b);
            imagenes.img1RGB.WriteImage("jpg", 0, path + "Cam1\\RGB_blanco_patron");
            imagenes.img1NIR.WriteImage("jpg", 0, path + "Cam1\\NIR_blanco_patron");
            // recarga los blancos para la aquisicion
            //Adquisition.CargaBlancos();
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="linea"></param>
        /// <returns></returns>
        public bool Macbech(int linea)
        {
            const string message =
            "Are you sure that you want to  replace the files?";
            const string caption = "Form Closing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            // If the no button was pressed ...
            if (result == DialogResult.No)
                return (true);          
            //GUARDAR IMÁGENES MACBECH LINEALES
            imagenes.img1RGB.WriteImage("jpg", 0, path + "Cam1\\RGB_L" + linea.ToString());          
            imagenes.img1NIR.WriteImage("jpg", 0, path + "Cam1\\NIR_L" + linea.ToString());
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public bool CorrecionRGB(int cam)
        {

            const string message =
           "Are you sure that you want to  replace the files?";
            const string caption = "Form Closing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            // If the no button was pressed ...
            if (result == DialogResult.No)
                return (true);
            if ((cam == 1))
            {
                //GUARDAR IMÁGENES BLANCO LINEALES
                HTuple a, b;
                imagenes.img1NIR.GetImageSize(out a, out b);
                imagenes.img1RGB.WriteImage("jpg", 0, path + "Cam1\\RGB_Pixel");           
                imagenes.img1NIR.WriteImage("jpg", 0, path + "Cam1\\NIR_Pixel");              
            }            
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public bool Fondo(int cam)
        {

            const string message =
            "Are you sure that you want to  replace the files?";
            const string caption = "Form Closing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            // If the no button was pressed ...
            if (result == DialogResult.No)
                return (true);
            if (cam == 1)
            {
                //GUARDAR IMÁGENES FONDO              
                imagenes.img1RGB.WriteImage("jpg", 0, path + "Cam1\\RGB_fondo");          
                imagenes.img1NIR.WriteImage("jpg", 0, path + "Cam1\\NIR_fondo");       
            }
           

            return true;
        }
        /// <summary>
        /// resetea la calibracion y la inicializa a 1
        /// </summary>
        public void  Reset_calibracion()
        {

            HTuple valuesInitMatrix = new HTuple(1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
            ColorMatrixs.ColorMatrix1_l1.CreateMatrix(3, 4, 0.0);
            ColorMatrixs.ColorMatrix1_l1.SetFullMatrix(valuesInitMatrix);
            //Envia el mensaje para notificar que la matriz de calibracion ya está lista
            SendMessage("Vision", "CALIB_IMAGES_LINEALES");
            const string message =
            "Are you sure that you want to  replace the files?";
            const string caption = "Form Closing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
            // If the no button was pressed ...
            if (result == DialogResult.No)
                return;
            ColorMatrixs.ColorMatrix1_l1.WriteMatrix("ascii", path + "\\ColorMatrix1_l1");          
        }
        /// <summary>
        /// 
        /// </summary>
        public void Actualizar()
        {                
            string message = "Are the Macbeth  images well corrected?";
            string caption = "Form Closing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
        
            // If the no button was pressed ...
            if (result != DialogResult.Yes)
                return;
            
            if (Matrix1 != null)
            {
                ColorMatrixs.ColorMatrix1_l1.SetFullMatrix(Matrix1);
                ColorMatrixs.ColorMatrix1_l1.WriteMatrix("ascii", path + "\\ColorMatrix1_l1");
            }
            //Envia el mensaje para notificar que la matriz de calibracion ya está lista
            SendMessage("Vision", "CALIB_IMAGES_LINEALES");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="Matriz"></param>
        public void ActualizarMatriz(int ID, HTuple Matriz)
        {

            if (ID == 1)
                Matrix1 = Matriz.Clone();
                    
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        public bool Correccion(int cam)
        {
            const string message =
            "Are you sure that you want to  replace the files?";
            const string caption = "Form Closing";
            var result = MessageBox.Show(message, caption,
                                         MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);

            // If the no button was pressed ...
            if (result == DialogResult.No)
                return (true);      
            //GUARDAR IMÁGENES CORRECCIÓN LINEALES
            imagenes.img1RGB.WriteImage("jpg", 0, path + "Cam1\\RGB_patron");         
            imagenes.img1NIR.WriteImage("jpg", 0, path + "Cam1\\NIR_patron");           
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Calibracion()
        {

            if (imagenes == null)
                return false;
            HImage  images          = imagenes.img1RGB;
            HRegion Regions1        = null;      
            HImage  imagesReult1    = null;        
            HTuple  output          = 0;
            HTuple  valuesInitMatrix = new HTuple(1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0);
            images                   = images.ConcatObj(imagenes.img1NIR);
            double ScaleX1           = 0;
            double ScaleY1           = 0;
            // execute procedure
            //imagen1
            ProcedureCallCalibracion.SetInputIconicParamObject("Image", imagenes.img1RGB);
            ProcedureCallCalibracion.SetInputCtrlParamTuple("path_image",  path );
            ProcedureCallCalibracion.SetInputCtrlParamTuple("Linea", imagenes.Linea);
            ProcedureCallCalibracion.SetInputCtrlParamTuple("cameratype", 0);
            HTuple text = HalconEngine.GetLoadedProcedureNames();
            ProcedureCallCalibracion.Execute();
            int error1  = ProcedureCallCalibracion.GetOutputCtrlParamTuple("error");
            output      = ProcedureCallCalibracion.GetOutputCtrlParamTuple("Texto_salida");

            if (error1 == 0)
            {
                      
                Regions1                = ProcedureCallCalibracion.GetOutputIconicParamRegion("Region1");
                HTuple outputHandle1    = ProcedureCallCalibracion.GetOutputCtrlParamTuple("MatrixTransLCam1");
                HOperatorSet.GetFullMatrix(outputHandle1, out Matrix1);
                imagesReult1            = ProcedureCallCalibracion.GetOutputIconicParamImage("Image1");
                ScaleX1                 = ProcedureCallCalibracion.GetOutputCtrlParamTuple("ScaleX1");
                ScaleY1                 = ProcedureCallCalibracion.GetOutputCtrlParamTuple("ScaleY1");
                ScaleX1                 = Math.Round(ScaleX1, 2);
                ScaleY1                 = Math.Round(ScaleY1, 2);


            }
            else
                Matrix1 = valuesInitMatrix.Clone();
          

            ((CalibracionForm)WindowForm).CleanDispImages();
            ((CalibracionForm)WindowForm).DispImagesResultados(imagesReult1, imagesReult1, imagenes.Linea, ScaleX1, ScaleY1);
     
           
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imgRGB1"></param>
        /// <param name="imgRGB2"></param>
        /// <param name="imgRGB3"></param>
        /// <param name="imgNIR1"></param>
        /// <param name="imgNIR2"></param>
        /// <param name="imgNIR3"></param>
        /// <param name="imgAnterior"></param>
        /// <param name="imgPosterior"></param>
        private void SaveImagesProducto(HImage imgRGB1,HImage imgNIR1, int linea)
        {
            DateTime Ejecucion = DateTime.Now;
            string savepath = path + Ejecucion.Day.ToString() + "_" + Ejecucion.Month.ToString() + "_" + Ejecucion.Year.ToString() + "\\" + "linea_" + linea.ToString() + "\\";

            try
            {
                if (!Directory.Exists(savepath))
                    Directory.CreateDirectory(savepath);
                if (!Directory.Exists(savepath + pathCam1))
                    Directory.CreateDirectory(savepath + pathCam1);
             
                imgRGB1 = imgRGB1.FullDomain();             
                imgNIR1 = imgNIR1.FullDomain();            
                imgRGB1.WriteImage("jpg", 0, savepath + pathCam1  + "RGB_" + Ejecucion.Ticks.ToString());              
                imgNIR1.WriteImage("jpg", 0, savepath + pathCam1  + "NIR_" + Ejecucion.Ticks.ToString());
            

            }
            catch 
            { }


        }
    }

    //////////////////////////////////////////////////////////////

    public delegate bool CalibracionDelegate(int cam);
    public delegate bool LineaDelegate(int linea);
    public delegate bool CalibrarDelegate();
    public delegate void VoidDelegate();
    public delegate void Reset_calibracion();
    public delegate bool CalibrarRGBDelegate(int cam);
    public delegate void ActualizaMatriz(int ID, HTuple Matriz);
    public delegate void ActualizaTexto();


    //////////////////////////////////////////////////////////////
    public class ModuleDelegates
    {
        public VoidDelegate ForzarCaptura;
        public VoidDelegate ActivarCaptura;
        public CalibracionDelegate Blancos;
        public CalibracionDelegate Blancos_patron;   
        public LineaDelegate Macbech;
        public CalibracionDelegate Fondo;
        public CalibracionDelegate Correccion;
        public CalibrarDelegate Calibrar;
        public Reset_calibracion Resetear;
        public CalibrarRGBDelegate CorrecionRGB;
        public VoidDelegate Actualizar;
        public ActualizaMatriz ActualizarMatriz;
        public ActualizaTexto ActualizaTexto;

        //////////////////////////////////////////////////////////////
        public ModuleDelegates(CalibracionDelegate pBlancos, LineaDelegate pMacbech, CalibracionDelegate pFondo, CalibracionDelegate pCorreccion, VoidDelegate pForzarCaptura, VoidDelegate pActivarCaptura, CalibrarDelegate pCalibrar, Reset_calibracion pResetear, CalibrarRGBDelegate pCorrecionRGB, VoidDelegate  pActualizar, ActualizaMatriz pActualizarMatriz,
             ActualizaTexto pActualizaTexto, CalibracionDelegate pBlancos_patron)
        {
            Blancos_patron = pBlancos_patron;
            Blancos = pBlancos;
            Macbech = pMacbech;
            Fondo = pFondo;
            Correccion = pCorreccion;
            ForzarCaptura = pForzarCaptura;
            ActivarCaptura = pActivarCaptura;
            Calibrar = pCalibrar;
            Resetear = pResetear;
            CorrecionRGB = pCorrecionRGB;
            Actualizar = pActualizar;
            ActualizarMatriz = pActualizarMatriz;
            ActualizaTexto = pActualizaTexto;
        }

        //////////////////////////////////////////////////////////////
    }

}
