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

namespace TrainingModule
{
    public partial class ManualselectionFrom : Form
    {
        public int calibracion_ok;
        HWindow Window1;
        HImage Image1;
        private static HDevEngine HalconEngine = new HDevEngine();
        HDevProcedureCall ProcedureCallCorreccion;
        HDevProcedure HalconProcedureCorreccion;

        public ManualselectionFrom()
        {
            InitializeComponent();
            Image1 = null;
            Window1 = hWindowControl1.HalconWindow;
            HalconEngine.SetProcedurePath("Procedimientos//");
            try
            {
                HalconProcedureCorreccion = new HDevProcedure("Correccion_individual");
                ProcedureCallCorreccion = new HDevProcedureCall(HalconProcedureCorreccion);
            }
            catch 
            {

            }
          
        }

      
        public void VisualizaImagen(HImage Image1, bool persistente)
        {
            
            this.Window1.ClearWindow();

            try
            {
                ProcedureCallCorreccion.SetInputIconicParamObject("Image", Image1);
                ProcedureCallCorreccion.Execute();
                Image1 = ProcedureCallCorreccion.GetOutputIconicParamImage("Imageresultado");

            }
            catch
            { }
           

            if (Image1 != null)
            {
                hWindowControl1.SetFullImagePart(Image1);
                int channels = Image1.CountChannels();
                if (channels == 3)
                    this.Window1.DispColor(Image1);
                else if (channels == 1)
                    this.Window1.DispImage(Image1);

            }

            if (persistente)
            {

                this.Image1 = Image1;
               
            }
       
         

        }
        /// <summary>
        /// 
        /// </summary>
        public void VisualizaImagen()
        {

            if (this.Image1 != null)
            {
                hWindowControl1.SetFullImagePart(this.Image1);
                this.Window1.DispColor(this.Image1);
            }
        
        }

   

        private void button_salir_Click(object sender, EventArgs e)
        {
            //calibracion_ok = 0;
            this.Close();
        }
    }
}
