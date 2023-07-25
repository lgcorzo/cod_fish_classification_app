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
using ProcesadoEspecies;

namespace CalibracionModule
{
    public partial class CalibracionForm : Form
    {
        private csr.modules.CSRFormModule parent;
     
        HWindow Window1;
        HWindow Window4;
        HWindow Window5;
     
        ManualselectionFrom SeleccionMacbeth = null;
        int linea;
        bool activadacaptura = false;
        HImage Image4, Image5;
        HXLD contorno4 = null, contorno5 = null;   
        ModuleDelegates moduleDelegates;

        ///////////////////////////////////
        ///////////////////////////////////

        public CalibracionForm(csr.modules.CSRFormModule _parent, ModuleDelegates module_delegates)
        { 
            this.parent = _parent;
            InitializeComponent();
            this.moduleDelegates = module_delegates;
            Init();
        }

        ///////////////////////////////////
        private void Init()
        {

        
            Image4 = null;
            Image5 = null;     
            linea = 0;

            Window1 = hWindowControl1.HalconWindow;
            Window4 = hWindowControl4.HalconWindow;
            Window5 = hWindowControl5.HalconWindow;
        



        }

        public void CleanDispImages()
        {
          
            this.Window1.ClearWindow();         
            this.Window4.ClearWindow();
            this.Window5.ClearWindow();
           

        }

        public void DispImagesResultados(HImage img1, HImage img2, int  linea, double ScaleX, double ScaleY)

        {
            //limpia las imagenes anteriores

            this.linea = linea;

            textBoxScaleY.Text = ScaleY.ToString();
            textBoxScaleX.Text = ScaleX.ToString();

            if (img1 != null)
            {
                hWindowControl1.SetFullImagePart(img1);
                this.Window1.DispColor(img1);
             
            }

            if (Image4 != null)
            {
               
                hWindowControl4.SetFullImagePart(Image4);
                if (Image4.CountChannels() == 3)
                    this.Window4.DispColor(Image4);
                else
                    this.Window4.DispObj(Image4);
                this.Window4.DispXld(contorno4);
            }
            if (Image5 != null)
            {
               
                hWindowControl5.SetFullImagePart(Image5);
                if (Image5.CountChannels() == 3)
                    this.Window5.DispColor(Image5);
                else
                    this.Window5.DispObj(Image5);
                this.Window5.DispXld(contorno5);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img4"></param>
        /// <param name="img5"></param>
        /// <param name="linea"></param>
        public void DispImagesArea(HImage img4, HImage img5, int linea)
        {
            //ÁREA
            HRegion RegionContorno4 = new HRegion();          
            HRegion RegionContorno5 = new HRegion();
            RegionContorno5.GenEmptyRegion();
            RegionContorno4.GenEmptyRegion();
            Image4 = img4;
            Image5 = img5;          
            if (Image4 != null)
            {
                RegionContorno4 = img4.GetDomain();
                Image4 = img4.FullDomain();
                contorno4 = RegionContorno4.GenContourRegionXld("border");
                hWindowControl4.SetFullImagePart(Image4);
                if(Image4.CountChannels() == 3)
                    this.Window4.DispColor(Image4);
                else
                    this.Window4.DispObj(Image4);
                this.Window4.DispXld(contorno4);
            }
            if (Image5 != null)
            {
                RegionContorno5 = img5.GetDomain();
                Image5 = img5.FullDomain();
                contorno5 = RegionContorno5.GenContourRegionXld("border");
                hWindowControl5.SetFullImagePart(Image5);
                if (Image5.CountChannels() == 3)
                    this.Window5.DispColor(Image5);
                else
                    this.Window5.DispObj(Image5);
                this.Window5.DispXld(contorno5);
            }                     
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="activada"></param>
        public void CapturaActivada(bool activada)
        {
            activadacaptura = activada;
            if (activada)
            {               
                this.btn_Act.Text = "Desactivar";
            }else
            {
                this.btn_Act.Text = "Activar";           
            }
        }   
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool GetSaveImage()
        {
            bool valorretorno = checkBoxsaveimages.Checked;
            return (valorretorno);
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Forzar_Click(object sender, EventArgs e)
        {
            if (moduleDelegates.ForzarCaptura != null)
                moduleDelegates.ForzarCaptura();
        }

        private void btn_Blancos_Click(object sender, EventArgs e)
        {

            if (moduleDelegates.Blancos != null)
              moduleDelegates.Blancos(1);
        }

        private void btn_Fondo_Click(object sender, EventArgs e)
        {

            if (moduleDelegates.Fondo != null)
                moduleDelegates.Fondo(1);
        }

        private void Btn_Correccion_Click(object sender, EventArgs e)
        {

            if (moduleDelegates.Correccion != null)
                moduleDelegates.Correccion(1);
        }

        private void CalibracionForm_Paint(object sender, PaintEventArgs e)
        {
            if (moduleDelegates.ActualizaTexto != null && activadacaptura == false)
                moduleDelegates.ActualizaTexto();

            try
            {
                if (Image4 != null)
                {
                    hWindowControl4.SetFullImagePart(Image4);
                    this.Window4.DispColor(Image4);
                    this.Window4.SetColor("red");

                }

                if (Image5 != null)
                {
                    hWindowControl5.SetFullImagePart(Image5);
                    this.Window5.DispObj(Image5);
                    this.Window5.SetColor("red");

                }
            }
            catch
            { }
           
      
            //muestra los valores RGB de la matri

        }

        public int WritetxtoResultados(string text, int ID, bool borrado_lineas = false)
        {
             
            if(borrado_lineas)
                Window1.ClearWindow();
            Window1.WriteString(text);
            Window1.NewLine();
                       
            return (0);
        }

      

        private void Reset_button_Click(object sender, EventArgs e)
        {
            if (moduleDelegates.Resetear != null)
                moduleDelegates.Resetear();
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            if (moduleDelegates.CorrecionRGB != null)
                moduleDelegates.CorrecionRGB(1);

        }

        private void buttonActualizar_Click(object sender, EventArgs e)
        {
            if (moduleDelegates.Actualizar != null)
                moduleDelegates.Actualizar();
        }

        private void hWindowControl1_HMouseDown(object sender, HMouseEventArgs e)
        {
            int matrixID = 1;
           
           SeleccionMacbeth = new ManualselectionFrom();         
           SeleccionMacbeth.VisualizaImagen(Image4, 1, true);        
            SeleccionMacbeth.ShowDialog();
            if (SeleccionMacbeth.calibracion_ok == 1)
            {
                hWindowControl1.SetFullImagePart(SeleccionMacbeth.imagesReult1);       
                Window1.DispColor(SeleccionMacbeth.imagesReult1);
                if (moduleDelegates.ActualizarMatriz != null)
                    moduleDelegates.ActualizarMatriz(matrixID,SeleccionMacbeth.Matrix1);
                textBoxScaleX.Text = SeleccionMacbeth.GetScaleX().ToString();
                textBoxScaleY.Text = SeleccionMacbeth.GetScaleY().ToString();
            }

        }
       
                
        private void btn_Macbech_Click(object sender, EventArgs e)
        {
            int linea = 1;
          
            if (moduleDelegates.Macbech != null)
                moduleDelegates.Macbech(linea);
        }

        private void btn_Act_Click(object sender, EventArgs e)
        {
            if (moduleDelegates.ActivarCaptura!= null)
                moduleDelegates.ActivarCaptura();
        }

       
        private void btn_Calibrar_Click(object sender, EventArgs e)
        {
            Window1.ClearWindow();    
            if (moduleDelegates.Calibrar != null)
                moduleDelegates.Calibrar();
        }

        private void button1_Click(object sender, EventArgs e)
        {
             int cam = 1;         
            if (moduleDelegates.Blancos != null)
                moduleDelegates.Blancos(cam);
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            int cam = 1;
            if (moduleDelegates.Blancos_patron != null)
                moduleDelegates.Blancos_patron(cam);
        }

        private void btn_MacbechArea_Click(object sender, EventArgs e)
        {
            int cam = 1;         
            if (moduleDelegates.Macbech != null)
                moduleDelegates.Macbech(cam);
        }

        private void btn_FondoArea_Click(object sender, EventArgs e)
        {
            int cam = 1;          
            if (moduleDelegates.Fondo != null)
                moduleDelegates.Fondo(cam);
        }
    }
}
