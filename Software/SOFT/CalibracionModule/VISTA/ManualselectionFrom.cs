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
using System.IO;

namespace CalibracionModule
{
    public partial class ManualselectionFrom : Form
    {
        public int                  calibracion_ok;
        int                         camera_type;
        HWindow                     Window1;
        HImage                      Image1;
        HImage                      Imageorg;
        public HImage               imagesReult1;
        HRegion                     UL;
        HRegion                     UR;
        HRegion                     DL;
        HRegion                     DR;
        HRegion                     Regions1;
        HMouseEventArgs             eposition;
        public  HTuple              Matrix1;
        private static HDevEngine   HalconEngine = new HDevEngine();
        HDevProcedureCall           ProcedureCallCalibracion;
        HDevProcedure               HalconProcedureCalibracion;
        HDevProcedureCall           ProcedureCallCorreccion;
        HDevProcedure               HalconProcedureCorreccion;
        public string PATH_IMAGEN_RGB_WHITE = "";
        public string PATH_IMAGEN_NIR_WHITE = "";
        double ScaleX1       = 0;
        double ScaleY1       = 0;

        public ManualselectionFrom()
        {
            InitializeComponent();
            Image1      = null;
            Window1     = hWindowControl1.HalconWindow;
            UL          = new HRegion();
            UR          = new HRegion();
            DL          = new HRegion();
            DR          = new HRegion();
            UL.GenEmptyRegion();
            UR.GenEmptyRegion();
            DL.GenEmptyRegion();
            DR.GenEmptyRegion();
            eposition   = null;
            HalconEngine.SetProcedurePath("Procedimientos//");
            HalconProcedureCalibracion  = new HDevProcedure("SegmentImageMacBethIndividual");
            ProcedureCallCalibracion    = new HDevProcedureCall(HalconProcedureCalibracion);
            HalconProcedureCorreccion   = new HDevProcedure("RC_optimar_preprocesar_imagenes");
            ProcedureCallCorreccion     = new HDevProcedureCall(HalconProcedureCorreccion);
            calibracion_ok              = 0;
            Regions1                    = new HRegion();
            imagesReult1                = new HImage();
            Regions1.GenEmptyRegion();
            imagesReult1.GenEmptyObj();
            imagesReult1.Dispose();
            camera_type                 = 0; // o sin ajuste po rplamo en la tabla macbeth
            checkBoxCorrection.Checked  = false;
            PATH_IMAGEN_RGB_WHITE       = Path.GetDirectoryName(Application.ExecutablePath) + "\\conf\\Patrones\\Cam1\\RGB_blanco.jpg";
            PATH_IMAGEN_NIR_WHITE       = Path.GetDirectoryName(Application.ExecutablePath) + "\\conf\\Patrones\\Cam1\\NIR_blanco.jpg";
        }

        private void btn_Act_Click(object sender, EventArgs e)
        {
            int x, y, button;
            this.Window1.GetMbutton(out x, out y, out button);
            this.Window1.DispCross((double) x, (double)y, 10.0, 0.0);
          
        }
        public double GetScaleX()
        {
            return (ScaleX1);
        }
        public double GetScaleY()
        {
            return (ScaleY1);

        }

        public void VisualizaImagen(HImage Image1, int camera_type, bool persistente)
        {          
            this.Window1.ClearWindow();
            this.camera_type    = camera_type;
            this.Image1         = Image1;
            if (Image1 != null)
            {
                hWindowControl1.SetFullImagePart(Image1);
                this.Window1.DispObj(Image1);
            }
            if (persistente)
            {      
                this.Imageorg   = Image1;
              
            }
            this.Window1.SetColor("red");
            this.Window1.DispCross(UL.Row, UL.Column, 20.0, 0.0);
            this.Window1.SetColor("green");
            this.Window1.DispCross(UR.Row, UR.Column, 20.0, 0.0);
            this.Window1.SetColor("blue");
            this.Window1.DispCross(DL.Row, DL.Column, 20.0, 0.0);
            this.Window1.SetColor("pink");
            this.Window1.DispCross(DR.Row, DR.Column, 20.0, 0.0);
        }
        /// <summary>
        /// 
        /// </summary>
        public void VisualizaImagen(HImage Image1)
        {
            if (Image1 != null)
            {
                hWindowControl1.SetFullImagePart(Image1);
                this.Window1.DispColor(Image1);
            }

            this.Window1.SetColor("red");
            this.Window1.DispCross(UL.Row, UL.Column, 20.0, 0.0);
            this.Window1.SetColor("green");
            this.Window1.DispCross(UR.Row, UR.Column, 20.0, 0.0);
            this.Window1.SetColor("blue");
            this.Window1.DispCross(DL.Row, DL.Column, 20.0, 0.0);
            this.Window1.SetColor("pink");
            this.Window1.DispCross(DR.Row, DR.Column, 20.0, 0.0);             
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hWindowControl1_HMouseDown(object sender, HMouseEventArgs e)
        {
            eposition = e;
            VisualizaImagen(Image1);
            this.Window1.SetColor("red");
            this.Window1.DispCross(e.Y, e.X, 10.0, 0.0);         
        }

        private void btn_UL_Click(object sender, EventArgs e)
        {
            if (eposition == null)
                return;
            UL.GenRegionPoints(eposition.Y, eposition.X);
            VisualizaImagen(Image1);
        }

        private void btn_UR_Click(object sender, EventArgs e)
        {
            if (eposition == null)
                return;
            UR.GenRegionPoints(eposition.Y, eposition.X);

            VisualizaImagen(Image1);


        }

        private void btn_DL_Click(object sender, EventArgs e)
        {
            if (eposition == null)
                return;
            DL.GenRegionPoints(eposition.Y, eposition.X);
            DL.DispRegion(Window1);
            VisualizaImagen(Image1);


        }

        private void btn_DR_Click(object sender, EventArgs e)
        {
            if (eposition == null)
                return;
            DR.GenRegionPoints(eposition.Y, eposition.X);
            VisualizaImagen(Image1);

        }

        private void button_Calibrar_Click(object sender, EventArgs e)
        {
            //comprobar que las 4 esquinas están
            ScaleX1 = 0.0;
            ScaleY1 = 0.0;
            // execute procedure
            ProcedureCallCalibracion.SetInputIconicParamObject("Image", Image1);
            ProcedureCallCalibracion.SetInputIconicParamObject("UL", UL);
            ProcedureCallCalibracion.SetInputIconicParamObject("UR", UR);
            ProcedureCallCalibracion.SetInputIconicParamObject("DL", DL);
            ProcedureCallCalibracion.SetInputIconicParamObject("DR", DR);
            ProcedureCallCalibracion.SetInputCtrlParamTuple("camera_type", camera_type);
            HTuple text = HalconEngine.GetLoadedProcedureNames();
            try
            {
                ProcedureCallCalibracion.Execute();
                HTuple output           = ProcedureCallCalibracion.GetOutputCtrlParamTuple("Texto_salida");            
                Regions1                = ProcedureCallCalibracion.GetOutputIconicParamRegion("Region");
                HTuple outputHandle1    = ProcedureCallCalibracion.GetOutputCtrlParamTuple("MatrixTransLCam");
                imagesReult1            = ProcedureCallCalibracion.GetOutputIconicParamImage("Imageresultado");
                ScaleX1                 = ProcedureCallCalibracion.GetOutputCtrlParamTuple("ScaleX2");
                ScaleY1                 = ProcedureCallCalibracion.GetOutputCtrlParamTuple("ScaleY2");
                ScaleX1                 = Math.Round(ScaleX1, 2);
                ScaleY1                 = Math.Round(ScaleY1, 2);
                this.Window1.ClearWindow();
                hWindowControl1.SetFullImagePart(imagesReult1);
                imagesReult1.DispColor(Window1);
                const string message =
                 "Es buena la calibración ?";
                const string caption = "Form Closing";
                var result = MessageBox.Show(message, caption,
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

                // If the no button was pressed ...
                calibracion_ok = 0;
                if (result == DialogResult.No)
                    return;

                HOperatorSet.GetFullMatrix(outputHandle1, out Matrix1);
                imagesReult1 = ProcedureCallCalibracion.GetOutputIconicParamImage("Imageresultado");
                calibracion_ok = 1;
                VisualizaImagen(imagesReult1,1, false);

            }
            catch(Exception ex)
            {
                MessageBox.Show("fallo de la calibración :" + ex.ToString());
                calibracion_ok = 0;
            }

           
        }

        private void button_salir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBoxCorrection_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxCorrection.Checked)
            {
                HTuple hv_CameraParameters      = null;
                HTuple hv_CameraPose            = null;
                HImage ImagenRGBWhite           = new HImage();
                HImage ImagenNIRWhite           = new HImage();
                double Angle_belt               = 0;
                try
                    {
                        HOperatorSet.ReadCamPar("conf\\Patrones\\Cam1\\CameraParametersRGB.cal", out hv_CameraParameters);
                        HOperatorSet.ReadPose("conf\\Patrones\\Cam1\\CameraPoseRGB.dat", out hv_CameraPose);
                    }
                    catch
                    {
                        const string message =
                        "Lens correction parameters not present";
                        const string caption = "Error: ";
                        var result = MessageBox.Show(message, caption,
                                                     MessageBoxButtons.OK,
                                                     MessageBoxIcon.Error);
                    }

                //CARGAR LA IMAGEN ImagenRgbWhite: imagen del blanco para eliminar el defecto porducido por la iluminación
                try
                {
                    ImagenRGBWhite.ReadImage(PATH_IMAGEN_RGB_WHITE);
                    ImagenNIRWhite.ReadImage(PATH_IMAGEN_NIR_WHITE);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message, "The white reference is not present, please capture one");
                    ImagenRGBWhite.GenEmptyObj();
                    ImagenNIRWhite.GenEmptyObj();
                    return;
                }
                Angle_belt = (double) numericUpDownAngle.Value;
                ProcedureCallCorreccion.SetInputIconicParamObject("ROI_0", this.Imageorg.GetDomain());
                ProcedureCallCorreccion.SetInputCtrlParamTuple("angle_belt",Angle_belt);
                ProcedureCallCorreccion.SetInputIconicParamObject("ImageOrg", this.Imageorg);
                ProcedureCallCorreccion.SetInputIconicParamObject("ImagenRgbWhite", ImagenRGBWhite);
                ProcedureCallCorreccion.SetInputCtrlParamTuple("CameraParameters", hv_CameraParameters);
                ProcedureCallCorreccion.SetInputCtrlParamTuple("CameraPose", hv_CameraPose);
                ProcedureCallCorreccion.Execute();
                // Parámetros de salida
                HImage ImagenRGBCorregida = ProcedureCallCorreccion.GetOutputIconicParamImage("Image1");
                //Limpiar porcedimiento
                VisualizaImagen(ImagenRGBCorregida, 1, false);

            }
            else
            {
                VisualizaImagen(this.Imageorg);
            }

        }
    }
}
