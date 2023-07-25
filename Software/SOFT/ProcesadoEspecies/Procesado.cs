using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using System.IO;



namespace ProcesadoEspecies
{
    public class Procesado
    {

     
        //clasificador de weka para arboles de decision
        wekaClassifier      Clasificador;
        HTuple hv_ClassLUTHandle = null;
        public int NumProcesamientos { get; set; }
        public int NumProcesamientosTextura { get; set; }
        public int NumProcesamientosSegmentacion { get; set; }
       

        //Para leer imagenes desde archivo
        int             counter         = 0;
        string          BaseFolder      ="";
        HTuple          RutasImagenes;
        HTuple          RutasImagenesBase;
        public int      numero_imagenes_folder = 0;

        /// <summary>
        /// 
        /// </summary>
        public  Procesado()
        {
            Init();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {    
            Clasificador                        = new wekaClassifier();
            NumProcesamientos                   = 0;
            NumProcesamientosTextura            = 0;
            NumProcesamientosSegmentacion       = 0;
            Load_Knn_color_segmentation("Procedimientos//");          
        }

        public List<string> Get_Class_names()
        {
            return Clasificador.m_classNames;

        }
        /// <summary>
        /// carga el modelo de clasificacion necesario
        /// </summary>
        /// <param name="ModelPath"></param>
        public string CargarModeloClasificacion(string ModelPath)
        {
            string Output = "classificadores cargados:";
            Clasificador.CargarNombreClass(ModelPath);
            //inicializa el modelo de clasificacion
            string path = ModelPath + "\\classifier1_1_1_FilteredClassifier.model";
            bool classifier1ok = Clasificador.CargarClasificador(path);
            if (classifier1ok)
                Output = Output + "classifier1 ";

            path = ModelPath + "\\classifier2_1_1_FilteredClassifier.model";
            bool classifier2ok = Clasificador.CargarClasificador2(path);
            if (classifier2ok)
                Output = Output + "classifier2 ";

            path = ModelPath + "\\classifier3_1_1_FilteredClassifier.model";
            bool classifier3ok = Clasificador.CargarClasificador3(path);
            if (classifier3ok)
                Output = Output + "classifier3 ";

            path = ModelPath + "\\classifier4_1_1_FilteredClassifier.model";
            bool classifier4ok = Clasificador.CargarClasificador4(path);
            if (classifier4ok)
                Output = Output + "classifier4 ";

            path = ModelPath + "\\classifier5_1_1_FilteredClassifier.model";
            bool classifier5ok = Clasificador.CargarClasificador5(path);
            if (classifier5ok)
                Output = Output + "classifier5 ";

            if (classifier1ok == false && classifier2ok == false && classifier3ok == false
                && classifier4ok == false && classifier5ok == false)
                Output = "Error en la carga del clasificador";

            return (Output);

        }
        /// <summary>
        /// reentrena el modelo de clasificacion necesario
        /// </summary>
        /// <param name="ModelPath"></param>
        public void Entrenarclasificador(string trainingDatasetFilePath, string savePath)
        {
            //inicializa el modelo de clasificacion
            Clasificador.Entrenarclasificador( trainingDatasetFilePath,  savePath);
        }
        /// <summary>
        /// lectura del knn para la segmentacion del color
        /// bitdepth 4, threshold 1000
        /// </summary>
        private void Load_Knn_color_segmentation(string Path)
        {

            HTuple hv_KNNHandle = null;
            // Initialize local and output iconic variables 
            HTuple hv_FileName = Path + "KNNClassifier.gnc";
            HOperatorSet.ReadClassKnn(hv_FileName, out hv_KNNHandle);
            HOperatorSet.CreateClassLutKnn(hv_KNNHandle, (new HTuple("bit_depth")).TupleConcat(
                "rejection_threshold"), (new HTuple(4)).TupleConcat(1000), out hv_ClassLUTHandle);
            HOperatorSet.ClearClassKnn(hv_KNNHandle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="img1rgb"></param> 
        /// <param name="img1nir"></param>
        public string ExecuteSegmentacion(HImage img1rgb, HImage img1nir, double ScaleX, double ScaleY,
            List<Sample> Samples, out HRegion Region1, out HRegion Region2)
        {
            DateTime Tinicial1 = DateTime.Now;
            Samples.Clear();
            string output_text   = "error en procesamiento";
             Region1 = null;
             Region2 = null;             
            NumProcesamientosSegmentacion++;
            try
            {
                //calcula los tiempos de porcesado de cada cosa
               
                this.SegmentationAndConnection( img1rgb, img1nir, ScaleX,  ScaleY ,out Region1, out Region2);

                DateTime Tfinal1 = DateTime.Now;
                TimeSpan time1;
                time1 = Tfinal1 - Tinicial1;
                //envía la imgen recortada para procesar la textura             
                output_text = "segmentacion: " + time1.TotalMilliseconds.ToString();              
                return (output_text);
            }
            catch (Exception e)
            {
                if(Samples.Count == 0)
                {
                    Sample sample = new Sample();
                    sample.Features.addFeature("error_process", 1);
                    Samples.Add(sample);
                }
                else
                    Samples[0].Features.addFeature("error_process", 1);
                return (e.ToString());
            }

          
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="img1rgb"></param>
        /// <param name="img1nir"></param>    
        /// <param name="ScaleX"></param>
        /// <param name="ScaleY"></param>
        /// <param name="Samples"></param>
        /// <param name="Region1"></param>
        /// <param name="Region2"></param>
        /// <returns></returns>
        public string ExecuteProcess(HImage img1rgb, HImage img1nir, double ScaleX, double ScaleY,
            List<Sample> Samples,  HRegion Region1,  HRegion Region2)
        {
            Samples.Clear();
            string output_text = "error en procesamiento";
          
        
            NumProcesamientos++;
            NumProcesamientosTextura++;
            try
            {          
                //envía la imgen recortada para procesar la textura
                DateTime Tinicial2 = DateTime.Now;
                this.Process(img1rgb, img1nir, ScaleX, ScaleY, Region1, Region2, Samples);
                DateTime Tfinal2 = DateTime.Now;
                TimeSpan time2;
                time2   = Tfinal2 - Tinicial2;

                DateTime Tinicial3 = DateTime.Now;
                this.Decision(Samples);
                DateTime Tfinal3 = DateTime.Now;
                TimeSpan time3;
                time3   = Tfinal3 - Tinicial3;

                output_text = " procesamiento : " + time2.TotalMilliseconds.ToString() + " decision: " + time3.TotalMilliseconds.ToString();
                return (output_text);


            }
            catch (Exception e)
            {
                if (Samples.Count == 0)
                {
                    Sample sample = new Sample();
                    sample.Features.addFeature("error_process", 1);
                    Samples.Add(sample);
                }
                else
                    Samples[0].Features.addFeature("error_process", 1);
                return (e.ToString());
            }

       
           
        }   
        /// <summary>
        /// 
        /// </summary>
        private void SegmentationAndConnection(HImage img1rgb, HImage img1nir, double ScaleX, double ScaleY, out HRegion Regions1, out HRegion Regions2)
        {
                 
            HImage images = new HImage();
            images = img1rgb.ConcatObj(img1nir);
            HTuple width, height;
            img1rgb.GetImageSize(out width, out height);
            img1nir.GetImageSize(out width, out height);   
            // Ejecutar procedimiento
            int SinFondo = 1;
            HTuple hv_tipo_producto = SinFondo;
            HObject hRegions1, hRegions2;
            Segmentation(images, out hRegions1, out hRegions2, hv_tipo_producto);
            Regions1 = new HRegion(hRegions1);
            Regions2 = new HRegion(hRegions2);

        }
      /// <summary>
      /// 
      /// </summary>
        private void Process(HImage img1rgb,HImage img1nir, double ScaleX, double ScaleY,
             HRegion Regions1,  HRegion Regions2, List<Sample> Samples)
        {
                  
            int samplesNum  = Regions1.CountObj();
            Sample sample   = new Sample();
            //procedimeinto de color y forma         
            HRegion Region1     = Regions1;
            HRegion Region2     = Regions2;
            HTuple outputHandle = new HTuple();
            HImage imgsSample   = new HImage();
            HTuple keys;

            img1rgb = img1rgb.ReduceDomain(Region1);
            img1nir = img1nir.ReduceDomain(Region2);
            imgsSample = img1rgb.ConcatObj(img1nir);
            HOperatorSet.CreateMessage(out outputHandle);         

           ProcessHdev(imgsSample,  outputHandle, hv_ClassLUTHandle);

           /* using (HDevProcedureCall HalconProcedureProcessCall = HalconProcedureProcess1Call.GetProcedure().CreateCall())
            {              
                HalconProcedureProcessCall.SetInputIconicParamObject("ImagesSample", imgsSample);
                HalconProcedureProcessCall.SetInputCtrlParamTuple("ClassLUTHandle", hv_ClassLUTHandle);
                HalconProcedureProcessCall.SetInputCtrlParamTuple("Features", outputHandle);
                HalconProcedureProcessCall.Execute();
                HalconProcedureProcessCall.Dispose();
            }*/
            HOperatorSet.GetMessageParam(outputHandle, "message_keys", new HTuple(), out keys);                      
            foreach (HTuple key in keys.SArr)
            {
                HTuple value;
                HOperatorSet.GetMessageTuple(outputHandle, key, out value);
                try
                {
                    sample.Features.addFeature(key, value);
                }
                catch (Exception ex)
                {
                    string exceptiotext = ex.ToString();
                    value = 0.0;
                    sample.Features.addFeature(key, value);
                }
            }
            //free message queue
            HOperatorSet.ClearMessage(outputHandle);
            keys = null;
            outputHandle = null;
            imgsSample.Dispose();    
            Samples.Add(sample);        
        }
      
        /// <summary>
        /// funcion de toma de decision con weka
        /// </summary>
        private void Decision(List<Sample> Samples)
        {
            double categoria                            = 0;
            double categoria1                           = 0;
            double categoria2                           = 0;
            double categoria3                           = 0;
            double categoria4                           = 0;
            double categoria5                           = 0;
            double categoria1predicted                  = 0;
            double categoria2predicted                  = 0;
            double categoria3predicted                  = 0;
            double categoria4predicted                  = 0;
            double categoria5predicted                  = 0;
            double categoriaponderada                   = 0;
            wekaClassifier ClasificadorThread           = new wekaClassifier();
            ClasificadorThread.m_clasificador1          = Clasificador.m_clasificador1;
            ClasificadorThread.m_clasificador2          = Clasificador.m_clasificador2;
            ClasificadorThread.m_clasificador3          = Clasificador.m_clasificador3;
            ClasificadorThread.m_clasificador4          = Clasificador.m_clasificador4;
            ClasificadorThread.m_clasificador5          = Clasificador.m_clasificador5;
            ClasificadorThread.m_classNames             = Clasificador.m_classNames;
            Features features                           = Samples[0].Features;
            int numclasesscount                         = ClasificadorThread.m_classNames.Count();
            int[] CategoriaVect                         = new int[numclasesscount];
            for(int index = 0; index < numclasesscount; index++)
            {
                CategoriaVect[index] = 0;
            }
              
            //instancia a partir de las features obtenidas del procesado
            weka.core.Instance Instancia                = Features.GenerarInstancia(features, ClasificadorThread.m_classNames);
            categoria1                                  = ClasificadorThread.Clasifica1Instacia(Instancia, 0.0, out categoria1predicted);
            categoria2                                  = ClasificadorThread.Clasifica2Instacia(Instancia, 0.0, out categoria2predicted);
            categoria3                                  = ClasificadorThread.Clasifica3Instacia(Instancia, 0.0, out categoria3predicted);
            categoria4                                  = ClasificadorThread.Clasifica4Instacia(Instancia, 0.0, out categoria4predicted);
            categoria5                                  = ClasificadorThread.Clasifica5Instacia(Instancia, 0.0, out categoria5predicted);
            //contador de categorias
            CategoriaVect[(int)categoria1]++;
            CategoriaVect[(int)categoria2]++;
            CategoriaVect[(int)categoria3]++;
            CategoriaVect[(int)categoria4]++;
            CategoriaVect[(int)categoria5]++;
            //liberar la memoria de la instancia
            Instancia.dataset().clear();

            int maximun             = 0;
            int posmax              = 0;
            int countdecisiones     = 0;
            for (int index = 1; index < numclasesscount; index++)
            {
                countdecisiones += CategoriaVect[index];
                if (CategoriaVect[index] > maximun)
                {
                    maximun         = CategoriaVect[index];
                    posmax          = index;
                    
                }                  

            }
            if (countdecisiones > 0)
                categoriaponderada = 100.0*((double)CategoriaVect[posmax] / (double)countdecisiones);
            else
                categoriaponderada = 0.0;
            categoria           = posmax;
            features.addFeature("Categoria_pertenencia_1", categoria1predicted);
            features.addFeature("Categoria_pertenencia_2", categoria2predicted);
            features.addFeature("Categoria_pertenencia_3", categoria3predicted);
            features.addFeature("Categoria_pertenencia_4", categoria4predicted);
            features.addFeature("Categoria_pertenencia_5", categoria5predicted);
            features.addFeature("Categoria_asignada_1", categoria1);
            features.addFeature("Categoria_asignada_2", categoria2);
            features.addFeature("Categoria_asignada_3", categoria3);
            features.addFeature("Categoria_asignada_4", categoria4);
            features.addFeature("Categoria_asignada_5", categoria5);     
            features.addFeature("Categoria_asignada_ponderada", categoriaponderada);
      
           
            //establezco la pertenencia al grupo mayoritario
            if (categoriaponderada <  40)
                categoria = 0;
           

            features.addFeature("Categoria_asignada", categoria);
            //libera la memoria 
            Instancia           = null;
            ClasificadorThread = null;
          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseFolder"></param>
        public HTuple  SetPathImagenen_ID(long ID)
        {
            string nombreFichero    = @"RGB_" + ID.ToString() + @".jpg";         
            RutasImagenes           = RutasImagenesBase.TupleRegexpSelect(new HTuple(nombreFichero, "ignore_case"));
            counter                 = 0;
            numero_imagenes_folder  = RutasImagenesBase.Length;
            return RutasImagenes;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseFolder"></param>
        public void SetPathImagenesBase()
        {
            RutasImagenes           = RutasImagenesBase;
            counter                 = 0;
            numero_imagenes_folder  = RutasImagenesBase.Length;
      
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseFolder"></param>
        public void CargarImagenes(string baseFolder)
        {
            try
            {
                HOperatorSet.ListFiles(new HTuple(baseFolder), new HTuple("files", "recursive", "max_depth 5"), out RutasImagenes);
                //selecciono solo las imagenes
                RutasImagenesBase = RutasImagenes.TupleRegexpSelect(new HTuple("\\.(tif|tiff|gif|bmp|jpg|jpeg|jp2|png|pcx|pgm|ppm|pbm|xwd|ima|hobj)$", "ignore_case"));
                //selecciono solo las imagenes que estan en Anterior
                RutasImagenesBase = RutasImagenes.TupleRegexpSelect(new HTuple("RGB_", "ignore_case"));
                BaseFolder = baseFolder;
                SetPathImagenesBase();

            }
            catch
            {
                //mostrar dialogo de que el fichero no es bueno
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Image1RGB"></param>
        /// <param name="Image2RGB"></param>
        /// <param name="Image3RGB"></param>
        /// <param name="ImageAnterior"></param>
        /// <param name="ImagePosterior"></param>
        /// <param name="Image1NIR"></param>
        /// <param name="Image2NIR"></param>
        /// <param name="Image3NIR"></param>
        /// <returns></returns>
        public bool LeerImagenes(out HImage Image1RGB, out HImage Image1NIR, List<Features> Instancias, out double ScaleX, out double ScaleY, out int linea, out long ID, out bool forzada,
            out int categoria_forzada)
        {
            bool existe_instancia = false;
            bool Image_ok        = false;
            forzada             = false;
            Image1RGB           = null;      
            Image1NIR           = null;    
            ID                  = 0;
            ScaleX              = 1;
            ScaleY              = 1;  // tamaño de la imagen para 1:1 502
            //lectura de la resolucion en el texto del path
            linea               = 0;
            categoria_forzada   = 0;
            string busqueda1    = "\\Trained\\";
            if (BaseFolder!="")
            {
                
                //BaseFolder
                if (counter < RutasImagenes.Length)
                {
                    try
                    {
                    string[]    stringSeparators   = new string[] { "\\RGB" };
                    string      ImagePath          = RutasImagenes[counter];
                    string[]    strParams          = ImagePath.Split(stringSeparators, StringSplitOptions.None);
                    int         lastcam1           = strParams.Count();
                    if (lastcam1 > 2)
                            return false;                     
                    string      filename           = Path.GetFileName(ImagePath);
                    string[]    strParamsname      = filename.Split('.');
                    string[]    strParamsnumber    = strParamsname[0].Split('_');
                    ID = long.Parse(strParamsnumber[1]);
                   
                    if (Instancias != null && Instancias.Count > 0)
                    {
                       existe_instancia = false;
                       for (int j = 0; j < Instancias.Count; j++)
                        {
                            Features Instancia = Instancias[j];
                            List<string> namesToSelect = new List<string>();
                            namesToSelect.Clear();
                            namesToSelect.Add("ID");
                            Instancia.Select(namesToSelect);
                            List<Feature> Attributos = Instancia.GetSelected();
                            long ID_local = (long)Attributos[0].lValue;
                                if (ID == ID_local)
                                    existe_instancia = true;

                        }
                    }

                    string ImagePathBase = strParams[0];                                  
                    if (ImagePath.Contains(busqueda1))
                    {
                            int indice1             = ImagePath.IndexOf(busqueda1);
                            indice1                 = indice1 + busqueda1.Length;
                            if (ImagePath.Contains("\\RGB"))
                            {
                                int indice2 = ImagePath.IndexOf("\\RGB", indice1);
                                string categoria_leida  = ImagePath.Substring(indice1, indice2 - indice1);
                                string[] strParamsnumbercat = categoria_leida.Split('_');
                                forzada                 = true;
                                categoria_forzada       = Int32.Parse(strParamsnumbercat[0]);
                            }
                               
                     }

                        HObject ho_Image1RGB;
                        HObject ho_Image1NIR;
                        HTuple hv_pathAnterior = RutasImagenes[counter];
                        HTuple hv_pathBase = ImagePathBase;
                        //comprueba que no existe un ID ya existe
                        
                        if(existe_instancia == false)
                        {
                            LeerImagenesCode(out ho_Image1RGB, out ho_Image1NIR, hv_pathAnterior, hv_pathBase);
                            Image1RGB = new HImage(ho_Image1RGB);
                            Image1NIR = new HImage(ho_Image1NIR);
                            Image_ok = true;
                        }               
                         counter++;
                    }
                    catch 
                    { return Image_ok; }
                    

                   
                    return Image_ok;
                }
            }
            return Image_ok;
        }
        //
        //
        //
        public void LeerImagenesCode(out HObject ho_Image1RGB, out HObject ho_Image1NIR, HTuple hv_pathAnterior,
        HTuple hv_pathBase)
        {
            // Local control variables 

            HTuple hv_pos = null, hv_namePost = null, hv_pathImage1RGB = null;
            HTuple hv_pathImage1NIR = null, hv_Exception = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image1RGB);
            HOperatorSet.GenEmptyObj(out ho_Image1NIR);
            HOperatorSet.TupleStrstr(hv_pathAnterior, "RGB", out hv_pos);
            HOperatorSet.TupleStrLastN(hv_pathAnterior, hv_pos + 3, out hv_namePost);
            hv_pathImage1RGB = (hv_pathBase + "/RGB") + hv_namePost;
            hv_pathImage1NIR = (hv_pathBase + "/NIR") + hv_namePost;

            try
            {
                ho_Image1RGB.Dispose();
                HOperatorSet.ReadImage(out ho_Image1RGB, hv_pathImage1RGB);
            }
            // catch (Exception) 
            catch (HalconException HDevExpDefaultException1)
            {
                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                ho_Image1RGB.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Image1RGB);
            }


            try
            {
                ho_Image1NIR.Dispose();
                HOperatorSet.ReadImage(out ho_Image1NIR, hv_pathImage1NIR);
            }
            // catch (Exception) 
            catch (HalconException HDevExpDefaultException1)
            {
                HDevExpDefaultException1.ToHTuple(out hv_Exception);
                ho_Image1NIR.Dispose();
                HOperatorSet.GenEmptyObj(out ho_Image1NIR);
            }

            return;
        }


        //*******************************************************************************************************//

        // Procedures 
        public void Calcula_PCA_lab_params(HObject ho_ImageWorkingRGB, HObject ho_Mascaraworking,
            HObject ho_ImageLabIn, out HTuple hv_Trans, out HTuple hv_TransInv, out HTuple hv_MeanLab,
            out HTuple hv_CovLab, out HTuple hv_InfoPerComp)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Imagenworking = null, ho_ImR = null;
            HObject ho_ImG = null, ho_ImB = null, ho_L = null, ho_a = null;
            HObject ho_b = null, ho_ImageLAB = null;

            // Local control variables 

            HTuple hv_num = null, hv_Exception = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Imagenworking);
            HOperatorSet.GenEmptyObj(out ho_ImR);
            HOperatorSet.GenEmptyObj(out ho_ImG);
            HOperatorSet.GenEmptyObj(out ho_ImB);
            HOperatorSet.GenEmptyObj(out ho_L);
            HOperatorSet.GenEmptyObj(out ho_a);
            HOperatorSet.GenEmptyObj(out ho_b);
            HOperatorSet.GenEmptyObj(out ho_ImageLAB);
            hv_InfoPerComp = new HTuple();
            try
            {
                HOperatorSet.CountObj(ho_ImageLabIn, out hv_num);
                if ((int)(new HTuple(hv_num.TupleEqual(0))) != 0)
                {
                    ho_Imagenworking.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageWorkingRGB, ho_Mascaraworking, out ho_Imagenworking
                        );
                    ho_ImR.Dispose(); ho_ImG.Dispose(); ho_ImB.Dispose();
                    HOperatorSet.Decompose3(ho_Imagenworking, out ho_ImR, out ho_ImG, out ho_ImB
                        );
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_ImR, out ExpTmpOutVar_0, "real");
                        ho_ImR.Dispose();
                        ho_ImR = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_ImG, out ExpTmpOutVar_0, "real");
                        ho_ImG.Dispose();
                        ho_ImG = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_ImB, out ExpTmpOutVar_0, "real");
                        ho_ImB.Dispose();
                        ho_ImB = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ScaleImage(ho_ImR, out ExpTmpOutVar_0, 1 / 255.0, 0.0);
                        ho_ImR.Dispose();
                        ho_ImR = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ScaleImage(ho_ImG, out ExpTmpOutVar_0, 1 / 255.0, 0.0);
                        ho_ImG.Dispose();
                        ho_ImG = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ScaleImage(ho_ImB, out ExpTmpOutVar_0, 1 / 255.0, 0.0);
                        ho_ImB.Dispose();
                        ho_ImB = ExpTmpOutVar_0;
                    }
                    ho_L.Dispose(); ho_a.Dispose(); ho_b.Dispose();
                    HOperatorSet.TransFromRgb(ho_ImR, ho_ImG, ho_ImB, out ho_L, out ho_a, out ho_b,
                        "cielab");
                    ho_ImageLAB.Dispose();
                    HOperatorSet.Compose3(ho_L, ho_a, ho_b, out ho_ImageLAB);

                }
                else
                {
                    ho_ImageLAB.Dispose();
                    ho_ImageLAB = ho_ImageLabIn.CopyObj(1, -1);
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_ImageLAB, ho_Mascaraworking, out ExpTmpOutVar_0
                        );
                    ho_ImageLAB.Dispose();
                    ho_ImageLAB = ExpTmpOutVar_0;
                }
                hv_TransInv = new HTuple();
                hv_Trans = new HTuple();
                hv_MeanLab = 0;
                hv_CovLab = 0;
                try
                {
                    //para la comparacion de los espacios de color  Lab
                    HOperatorSet.GenPrincipalCompTrans(ho_ImageLAB, out hv_Trans, out hv_TransInv,
                        out hv_MeanLab, out hv_CovLab, out hv_InfoPerComp);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_TransInv = new HTuple();
                    hv_TransInv[0] = 0;
                    hv_TransInv[1] = 0;
                    hv_TransInv[2] = 0;
                    hv_TransInv[3] = 0;
                    hv_TransInv[4] = 0;
                    hv_TransInv[5] = 0;
                    hv_TransInv[6] = 0;
                    hv_TransInv[7] = 0;
                    hv_TransInv[8] = 0;
                    hv_TransInv[9] = 0;
                    hv_TransInv[10] = 0;
                    hv_TransInv[11] = 0;
                    if (hv_MeanLab == null)
                        hv_MeanLab = new HTuple();
                    hv_MeanLab[0] = 0;
                    if (hv_MeanLab == null)
                        hv_MeanLab = new HTuple();
                    hv_MeanLab[1] = 0;
                    if (hv_MeanLab == null)
                        hv_MeanLab = new HTuple();
                    hv_MeanLab[2] = 0;
                    if (hv_CovLab == null)
                        hv_CovLab = new HTuple();
                    hv_CovLab[0] = 0;
                    if (hv_CovLab == null)
                        hv_CovLab = new HTuple();
                    hv_CovLab[1] = 0;
                    if (hv_CovLab == null)
                        hv_CovLab = new HTuple();
                    hv_CovLab[2] = 0;
                    if (hv_CovLab == null)
                        hv_CovLab = new HTuple();
                    hv_CovLab[3] = 0;
                    if (hv_CovLab == null)
                        hv_CovLab = new HTuple();
                    hv_CovLab[4] = 0;
                    if (hv_CovLab == null)
                        hv_CovLab = new HTuple();
                    hv_CovLab[5] = 0;
                    if (hv_CovLab == null)
                        hv_CovLab = new HTuple();
                    hv_CovLab[6] = 0;
                    if (hv_CovLab == null)
                        hv_CovLab = new HTuple();
                    hv_CovLab[7] = 0;
                    if (hv_CovLab == null)
                        hv_CovLab = new HTuple();
                    hv_CovLab[8] = 0;
                }

                ho_Imagenworking.Dispose();
                ho_ImR.Dispose();
                ho_ImG.Dispose();
                ho_ImB.Dispose();
                ho_L.Dispose();
                ho_a.Dispose();
                ho_b.Dispose();
                ho_ImageLAB.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Imagenworking.Dispose();
                ho_ImR.Dispose();
                ho_ImG.Dispose();
                ho_ImB.Dispose();
                ho_L.Dispose();
                ho_a.Dispose();
                ho_b.Dispose();
                ho_ImageLAB.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void Difuminado_rapido_gauss(HObject ho_Rgb_blanco, HObject ho_region,
            out HObject ho_Rgb_blancoOut, HTuple hv_resize_factor, HTuple hv_gauss_size)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_total_reg, ho_region_fondo, ho_R = null;
            HObject ho_G = null, ho_B = null;

            // Local control variables 

            HTuple hv_numchanels = null, hv_Width = null;
            HTuple hv_Height = null, hv_Mean = new HTuple(), hv_Deviation = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Rgb_blancoOut);
            HOperatorSet.GenEmptyObj(out ho_total_reg);
            HOperatorSet.GenEmptyObj(out ho_region_fondo);
            HOperatorSet.GenEmptyObj(out ho_R);
            HOperatorSet.GenEmptyObj(out ho_G);
            HOperatorSet.GenEmptyObj(out ho_B);
            try
            {
                ho_Rgb_blancoOut.Dispose();
                ho_Rgb_blancoOut = ho_Rgb_blanco.CopyObj(1, -1);
                HOperatorSet.CountChannels(ho_Rgb_blancoOut, out hv_numchanels);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.FullDomain(ho_Rgb_blancoOut, out ExpTmpOutVar_0);
                    ho_Rgb_blancoOut.Dispose();
                    ho_Rgb_blancoOut = ExpTmpOutVar_0;
                }
                HOperatorSet.GetImageSize(ho_Rgb_blancoOut, out hv_Width, out hv_Height);
                ho_total_reg.Dispose();
                HOperatorSet.GenRectangle1(out ho_total_reg, 0, 0, hv_Height, hv_Width);
                ho_region_fondo.Dispose();
                HOperatorSet.Difference(ho_total_reg, ho_region, out ho_region_fondo);
                if ((int)(new HTuple(hv_numchanels.TupleEqual(1))) != 0)
                {
                    HOperatorSet.Intensity(ho_region, ho_Rgb_blancoOut, out hv_Mean, out hv_Deviation);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.PaintRegion(ho_region_fondo, ho_Rgb_blancoOut, out ExpTmpOutVar_0,
                            hv_Mean, "fill");
                        ho_Rgb_blancoOut.Dispose();
                        ho_Rgb_blancoOut = ExpTmpOutVar_0;
                    }
                }

                if ((int)(new HTuple(hv_numchanels.TupleEqual(3))) != 0)
                {
                    ho_R.Dispose(); ho_G.Dispose(); ho_B.Dispose();
                    HOperatorSet.Decompose3(ho_Rgb_blancoOut, out ho_R, out ho_G, out ho_B);
                    HOperatorSet.Intensity(ho_region, ho_R, out hv_Mean, out hv_Deviation);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.PaintRegion(ho_region_fondo, ho_R, out ExpTmpOutVar_0, hv_Mean,
                            "fill");
                        ho_R.Dispose();
                        ho_R = ExpTmpOutVar_0;
                    }
                    HOperatorSet.Intensity(ho_region, ho_G, out hv_Mean, out hv_Deviation);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.PaintRegion(ho_region_fondo, ho_G, out ExpTmpOutVar_0, hv_Mean,
                            "fill");
                        ho_G.Dispose();
                        ho_G = ExpTmpOutVar_0;
                    }
                    HOperatorSet.Intensity(ho_region, ho_B, out hv_Mean, out hv_Deviation);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.PaintRegion(ho_region_fondo, ho_B, out ExpTmpOutVar_0, hv_Mean,
                            "fill");
                        ho_B.Dispose();
                        ho_B = ExpTmpOutVar_0;
                    }
                    ho_Rgb_blancoOut.Dispose();
                    HOperatorSet.Compose3(ho_R, ho_G, ho_B, out ho_Rgb_blancoOut);
                }

                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ZoomImageSize(ho_Rgb_blancoOut, out ExpTmpOutVar_0, hv_Width / hv_resize_factor,
                        hv_Height / hv_resize_factor, "constant");
                    ho_Rgb_blancoOut.Dispose();
                    ho_Rgb_blancoOut = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.SmoothImage(ho_Rgb_blancoOut, out ExpTmpOutVar_0, "gauss", hv_gauss_size);
                    ho_Rgb_blancoOut.Dispose();
                    ho_Rgb_blancoOut = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ZoomImageSize(ho_Rgb_blancoOut, out ExpTmpOutVar_0, hv_Width,
                        hv_Height, "constant");
                    ho_Rgb_blancoOut.Dispose();
                    ho_Rgb_blancoOut = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_Rgb_blancoOut, ho_region, out ExpTmpOutVar_0);
                    ho_Rgb_blancoOut.Dispose();
                    ho_Rgb_blancoOut = ExpTmpOutVar_0;
                }
                ho_total_reg.Dispose();
                ho_region_fondo.Dispose();
                ho_R.Dispose();
                ho_G.Dispose();
                ho_B.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_total_reg.Dispose();
                ho_region_fondo.Dispose();
                ho_R.Dispose();
                ho_G.Dispose();
                ho_B.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void Histo_ab_generator(HObject ho_ImageWorkingRGB, HObject ho_Mascara,
            HObject ho_ImageLabIn, out HObject ho_HistogramOut, HTuple hv_gaussSize, HTuple hv_MAX_Val)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Imagenworking = null, ho_ImR = null;
            HObject ho_ImG = null, ho_ImB = null, ho_L = null, ho_a = null;
            HObject ho_b = null, ho_ImageLAB = null, ho_Histo2Dim, ho_Histo2Dimreal;
            HObject ho_Histo2Dimrealgauss;

            // Local control variables 

            HTuple hv_num = null, hv_Max = null, hv_Min = null;
            HTuple hv_mult = null, hv_add = null, hv_Area = null, hv_Row4 = null;
            HTuple hv_Column4 = null;
            HTuple hv_MAX_Val_COPY_INP_TMP = hv_MAX_Val.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_HistogramOut);
            HOperatorSet.GenEmptyObj(out ho_Imagenworking);
            HOperatorSet.GenEmptyObj(out ho_ImR);
            HOperatorSet.GenEmptyObj(out ho_ImG);
            HOperatorSet.GenEmptyObj(out ho_ImB);
            HOperatorSet.GenEmptyObj(out ho_L);
            HOperatorSet.GenEmptyObj(out ho_a);
            HOperatorSet.GenEmptyObj(out ho_b);
            HOperatorSet.GenEmptyObj(out ho_ImageLAB);
            HOperatorSet.GenEmptyObj(out ho_Histo2Dim);
            HOperatorSet.GenEmptyObj(out ho_Histo2Dimreal);
            HOperatorSet.GenEmptyObj(out ho_Histo2Dimrealgauss);
            try
            {
                HOperatorSet.CountObj(ho_ImageLabIn, out hv_num);
                if ((int)(new HTuple(hv_num.TupleEqual(0))) != 0)
                {
                    ho_Imagenworking.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageWorkingRGB, ho_Mascara, out ho_Imagenworking
                        );
                    ho_ImR.Dispose(); ho_ImG.Dispose(); ho_ImB.Dispose();
                    HOperatorSet.Decompose3(ho_Imagenworking, out ho_ImR, out ho_ImG, out ho_ImB
                        );
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_ImR, out ExpTmpOutVar_0, "real");
                        ho_ImR.Dispose();
                        ho_ImR = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_ImG, out ExpTmpOutVar_0, "real");
                        ho_ImG.Dispose();
                        ho_ImG = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_ImB, out ExpTmpOutVar_0, "real");
                        ho_ImB.Dispose();
                        ho_ImB = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ScaleImage(ho_ImR, out ExpTmpOutVar_0, 1 / 255.0, 0.0);
                        ho_ImR.Dispose();
                        ho_ImR = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ScaleImage(ho_ImG, out ExpTmpOutVar_0, 1 / 255.0, 0.0);
                        ho_ImG.Dispose();
                        ho_ImG = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ScaleImage(ho_ImB, out ExpTmpOutVar_0, 1 / 255.0, 0.0);
                        ho_ImB.Dispose();
                        ho_ImB = ExpTmpOutVar_0;
                    }
                    ho_L.Dispose(); ho_a.Dispose(); ho_b.Dispose();
                    HOperatorSet.TransFromRgb(ho_ImR, ho_ImG, ho_ImB, out ho_L, out ho_a, out ho_b,
                        "cielab");
                    ho_ImageLAB.Dispose();
                    HOperatorSet.Compose3(ho_L, ho_a, ho_b, out ho_ImageLAB);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ReduceDomain(ho_ImageLAB, ho_Mascara, out ExpTmpOutVar_0);
                        ho_ImageLAB.Dispose();
                        ho_ImageLAB = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ReduceDomain(ho_L, ho_Mascara, out ExpTmpOutVar_0);
                        ho_L.Dispose();
                        ho_L = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ReduceDomain(ho_a, ho_Mascara, out ExpTmpOutVar_0);
                        ho_a.Dispose();
                        ho_a = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ReduceDomain(ho_b, ho_Mascara, out ExpTmpOutVar_0);
                        ho_b.Dispose();
                        ho_b = ExpTmpOutVar_0;
                    }
                }
                else
                {
                    ho_ImageLAB.Dispose();
                    ho_ImageLAB = ho_ImageLabIn.CopyObj(1, -1);
                    ho_L.Dispose(); ho_a.Dispose(); ho_b.Dispose();
                    HOperatorSet.Decompose3(ho_ImageLAB, out ho_L, out ho_a, out ho_b);
                }
                //espacio de color Lab
                hv_Max = 98.0;
                hv_Min = -86.0;
                HOperatorSet.TupleReal(hv_MAX_Val_COPY_INP_TMP, out hv_MAX_Val_COPY_INP_TMP);
                HOperatorSet.TupleReal(hv_Max, out hv_Max);
                HOperatorSet.TupleReal(hv_Min, out hv_Min);
                hv_mult = hv_MAX_Val_COPY_INP_TMP / (hv_Max - hv_Min);
                hv_add = (-hv_mult) * hv_Min;
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ScaleImage(ho_a, out ExpTmpOutVar_0, hv_mult, hv_add);
                    ho_a.Dispose();
                    ho_a = ExpTmpOutVar_0;
                }

                hv_Max = 95.0;
                hv_Min = -108.0;
                HOperatorSet.TupleReal(hv_MAX_Val_COPY_INP_TMP, out hv_MAX_Val_COPY_INP_TMP);
                HOperatorSet.TupleReal(hv_Max, out hv_Max);
                HOperatorSet.TupleReal(hv_Min, out hv_Min);
                hv_mult = hv_MAX_Val_COPY_INP_TMP / (hv_Max - hv_Min);
                hv_add = (-hv_mult) * hv_Min;
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ScaleImage(ho_b, out ExpTmpOutVar_0, hv_mult, hv_add);
                    ho_b.Dispose();
                    ho_b = ExpTmpOutVar_0;
                }

                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConvertImageType(ho_a, out ExpTmpOutVar_0, "byte");
                    ho_a.Dispose();
                    ho_a = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConvertImageType(ho_b, out ExpTmpOutVar_0, "byte");
                    ho_b.Dispose();
                    ho_b = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_a, ho_Mascara, out ExpTmpOutVar_0);
                    ho_a.Dispose();
                    ho_a = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_b, ho_Mascara, out ExpTmpOutVar_0);
                    ho_b.Dispose();
                    ho_b = ExpTmpOutVar_0;
                }
                ho_Histo2Dim.Dispose();
                HOperatorSet.Histo2dim(ho_Mascara, ho_a, ho_b, out ho_Histo2Dim);
                //visualiza la imagen
                //compose2 (a, b, ab)
                //scale_image (ab, ab, 10, 0)

                ho_Histo2Dimreal.Dispose();
                HOperatorSet.ConvertImageType(ho_Histo2Dim, out ho_Histo2Dimreal, "real");
                //escalodo para tener referenia igual a todos las imagenes
                HOperatorSet.AreaCenter(ho_Mascara, out hv_Area, out hv_Row4, out hv_Column4);
                HOperatorSet.TupleReal(hv_Area, out hv_Area);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ScaleImage(ho_Histo2Dimreal, out ExpTmpOutVar_0, 100.0 / hv_Area,
                        0);
                    ho_Histo2Dimreal.Dispose();
                    ho_Histo2Dimreal = ExpTmpOutVar_0;
                }
                ho_Histo2Dimrealgauss.Dispose();
                HOperatorSet.SmoothImage(ho_Histo2Dimreal, out ho_Histo2Dimrealgauss, "gauss",
                    hv_gaussSize);
                ho_HistogramOut.Dispose();
                ho_HistogramOut = ho_Histo2Dimrealgauss.CopyObj(1, -1);
                ho_Imagenworking.Dispose();
                ho_ImR.Dispose();
                ho_ImG.Dispose();
                ho_ImB.Dispose();
                ho_L.Dispose();
                ho_a.Dispose();
                ho_b.Dispose();
                ho_ImageLAB.Dispose();
                ho_Histo2Dim.Dispose();
                ho_Histo2Dimreal.Dispose();
                ho_Histo2Dimrealgauss.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Imagenworking.Dispose();
                ho_ImR.Dispose();
                ho_ImG.Dispose();
                ho_ImB.Dispose();
                ho_L.Dispose();
                ho_a.Dispose();
                ho_b.Dispose();
                ho_ImageLAB.Dispose();
                ho_Histo2Dim.Dispose();
                ho_Histo2Dimreal.Dispose();
                ho_Histo2Dimrealgauss.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void Histo_gray_generator(HObject ho_ImageWorking, HObject ho_Mascara,
            HTuple hv_MAX_Val, out HTuple hv_HistogramOut)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_regionInteres, ho_Imagenworkingtemp;
            HObject ho_a;

            // Local control variables 

            HTuple hv_Max = null, hv_Min = null, hv_Area = null;
            HTuple hv_Row = null, hv_Column = null, hv_mult = null;
            HTuple hv_add = null, hv_AbsoluteHisto1 = null, hv_RelativeHisto1 = null;
            HTuple hv_MAX_Val_COPY_INP_TMP = hv_MAX_Val.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_regionInteres);
            HOperatorSet.GenEmptyObj(out ho_Imagenworkingtemp);
            HOperatorSet.GenEmptyObj(out ho_a);
            try
            {
                //elimino los brillos
                //elimino los brillos

                hv_Max = 255;
                hv_Min = 0;
                ho_regionInteres.Dispose();
                HOperatorSet.Threshold(ho_ImageWorking, out ho_regionInteres, hv_Min, hv_Max);
                HOperatorSet.AreaCenter(ho_Mascara, out hv_Area, out hv_Row, out hv_Column);
                HOperatorSet.TupleReal(hv_Area, out hv_Area);
                hv_Area = hv_Area / 100;
                ho_Imagenworkingtemp.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageWorking, ho_regionInteres, out ho_Imagenworkingtemp
                    );
                //espacio de color Lab

                HOperatorSet.TupleReal(hv_MAX_Val_COPY_INP_TMP, out hv_MAX_Val_COPY_INP_TMP);
                HOperatorSet.TupleReal(hv_Max, out hv_Max);
                HOperatorSet.TupleReal(hv_Min, out hv_Min);
                hv_mult = hv_MAX_Val_COPY_INP_TMP / (hv_Max - hv_Min);
                hv_add = (-hv_mult) * hv_Min;
                ho_a.Dispose();
                HOperatorSet.ScaleImage(ho_Imagenworkingtemp, out ho_a, hv_mult, hv_add);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConvertImageType(ho_a, out ExpTmpOutVar_0, "byte");
                    ho_a.Dispose();
                    ho_a = ExpTmpOutVar_0;
                }
                HOperatorSet.GrayHisto(ho_regionInteres, ho_a, out hv_AbsoluteHisto1, out hv_RelativeHisto1);
                hv_HistogramOut = hv_AbsoluteHisto1 / hv_Area;
                ho_regionInteres.Dispose();
                ho_Imagenworkingtemp.Dispose();
                ho_a.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_regionInteres.Dispose();
                ho_Imagenworkingtemp.Dispose();
                ho_a.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Short Description: Detect a fin 
        public void optimar_extraer_caracteristicas_Peces(HObject ho_ImagenRGB, HObject ho_ImagenNIR,
            HTuple hv_ClassLUTHandle, HTuple hv_ParametrosCola)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_MascaraRGB, ho_MascaraNIR, ho_Gray;
            HObject ho_Matrix0Gray, ho_Matrix45Gray, ho_Matrix90Gray;
            HObject ho_Matrix135Gray, ho_R, ho_G, ho_B, ho_H, ho_S;
            HObject ho_V, ho_l, ho_a, ho_b, ho_ImageLab, ho_ImageWorkingRGB = null;
            HObject ho_MascaraWorking = null, ho_Imagenworking, ho_ImR;
            HObject ho_ImG, ho_ImB, ho_L, ho_ImageLAB, ho_ImageLabOut;
            HObject ho_HistogramOut, ho_Hist_domain, ho_HistogramOutlineal;
            HObject ho_Maxpos, ho_TiledImageRGB, ho_Imagencolorconjuntoreduced;
            HObject ho_ClassRegionsLUT, ho_totalRegionLUT, ho_ColorCuerpo;
            HObject ho_slectedobj = null, ho_Hue, ho_Saturation, ho_Intensity;
            HObject ho_Image1Median5, ho_ImagenFiltrada1, ho_Image1NIR;

            // Local control variables 

            HTuple hv_MeanGray = null, hv_DeviationGray = null;
            HTuple hv_GrayPlaneDeviation = null, hv_EntropyGray = null;
            HTuple hv_AnisotropyGray = null, hv_EntropyFuzzyGray = null;
            HTuple hv_PerimeterFuzzyGray = null, hv_Energy0gray = null;
            HTuple hv_Correlation0gray = null, hv_Homogeneity0gray = null;
            HTuple hv_Contrast0gray = null, hv_Energy45gray = null;
            HTuple hv_Correlation45gray = null, hv_Homogeneity45gray = null;
            HTuple hv_Contrast45gray = null, hv_Energy90gray = null;
            HTuple hv_Correlation90gray = null, hv_Homogeneity90gray = null;
            HTuple hv_Contrast90gray = null, hv_Energy135gray = null;
            HTuple hv_Correlation135gray = null, hv_Homogeneity135gray = null;
            HTuple hv_Contrast135gray = null, hv_MeanR = null, hv_DeviationR = null;
            HTuple hv_RPlaneDeviation = null, hv_EntropyR = null, hv_AnisotropyR = null;
            HTuple hv_EntropyFuzzyR = null, hv_PerimeterFuzzyR = null;
            HTuple hv_MeanG = null, hv_DeviationG = null, hv_GPlaneDeviation = null;
            HTuple hv_EntropyG = null, hv_AnisotropyG = null, hv_EntropyFuzzyG = null;
            HTuple hv_PerimeterFuzzyG = null, hv_MeanB = null, hv_DeviationB = null;
            HTuple hv_BPlaneDeviation = null, hv_EntropyB = null, hv_AnisotropyB = null;
            HTuple hv_EntropyFuzzyB = null, hv_PerimeterFuzzyB = null;
            HTuple hv_MeanH = null, hv_DeviationH = null, hv_HPlaneDeviation = null;
            HTuple hv_EntropyH = null, hv_AnisotropyH = null, hv_EntropyFuzzyH = null;
            HTuple hv_PerimeterFuzzyH = null, hv_MeanS = null, hv_DeviationS = null;
            HTuple hv_SPlaneDeviation = null, hv_EntropyS = null, hv_AnisotropyS = null;
            HTuple hv_EntropyFuzzyS = null, hv_PerimeterFuzzyS = null;
            HTuple hv_MeanV = null, hv_DeviationV = null, hv_VPlaneDeviation = null;
            HTuple hv_EntropyV = null, hv_AnisotropyV = null, hv_EntropyFuzzyV = null;
            HTuple hv_PerimeterFuzzyV = null, hv_Meanl = null, hv_Deviationl = null;
            HTuple hv_lPlaneDeviation = null, hv_Entropyl = null, hv_Anisotropyl = null;
            HTuple hv_EntropyFuzzyl = null, hv_PerimeterFuzzyl = null;
            HTuple hv_Meana = null, hv_Deviationa = null, hv_aPlaneDeviation = null;
            HTuple hv_Entropya = null, hv_Anisotropya = null, hv_EntropyFuzzya = null;
            HTuple hv_PerimeterFuzzya = null, hv_Meanb = null, hv_Deviationb = null;
            HTuple hv_bPlaneDeviation = null, hv_Entropyb = null, hv_Anisotropyb = null;
            HTuple hv_EntropyFuzzyb = null, hv_PerimeterFuzzyb = null;
            HTuple hv_Row = null, hv_Column = null, hv_Phi = null;
            HTuple hv_Largo = null, hv_Ancho = null, hv_Area = null;
            HTuple hv_Row1 = null, hv_Column3 = null, hv_Ra = null;
            HTuple hv_Rb = null, hv_Ra_Rb = null, hv_Circularity = null;
            HTuple hv_Compactness = null, hv_Distance = null, hv_Sigma = null;
            HTuple hv_Roundness = null, hv_Sides = null, hv_Convexity = null;
            HTuple hv_Rectangularity = null, hv_Anisometry = null;
            HTuple hv_Bulkiness = null, hv_StructureFactor = null;
            HTuple hv_PSI1 = null, hv_PSI2 = null, hv_PSI3 = null;
            HTuple hv_PSI4 = null, hv_M11 = null, hv_M20 = null, hv_M02 = null;
            HTuple hv_M21 = null, hv_M12 = null, hv_M03 = null, hv_M30 = null;
            HTuple hv_gaussSize = null, hv_MAX_Val = null, hv_Min = null;
            HTuple hv_Max = null, hv_Range = null, hv_area = null;
            HTuple hv_Max_x = null, hv_Max_y = null, hv_MRow = null;
            HTuple hv_MCol = null, hv_Alpha = null, hv_Beta = null;
            HTuple hv_Mean = null, hv_texto = null, hv_Index1 = null;
            HTuple hv_Index2 = new HTuple(), hv_textof = new HTuple();
            HTuple hv_Grayval = new HTuple(), hv_Trans = null, hv_TransInv = null;
            HTuple hv_MeanLab = null, hv_CovLab = null, hv_InfoPerComp = null;
            HTuple hv_num = null, hv_MeanL = null, hv_DevL = null;
            HTuple hv_Deva = null, hv_Devb = null, hv_AreaTotalLUT = null;
            HTuple hv_RowTotalLUT = null, hv_ColumnTotalLUT = null;
            HTuple hv_AreaLUT = null, hv_RowLUT = null, hv_ColumnLUT = null;
            HTuple hv_pattern_elements = null, hv_AreaCuerpo = null;
            HTuple hv_Columm = null, hv_IndicesCuerpo = null, hv_dimColores = null;
            HTuple hv_pos = new HTuple(), hv_Dev = new HTuple(), hv_HistogramGrayOut = null;
            HTuple hv_Function = null, hv_SmoothedFunction = null;
            HTuple hv_XValues = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_MascaraRGB);
            HOperatorSet.GenEmptyObj(out ho_MascaraNIR);
            HOperatorSet.GenEmptyObj(out ho_Gray);
            HOperatorSet.GenEmptyObj(out ho_Matrix0Gray);
            HOperatorSet.GenEmptyObj(out ho_Matrix45Gray);
            HOperatorSet.GenEmptyObj(out ho_Matrix90Gray);
            HOperatorSet.GenEmptyObj(out ho_Matrix135Gray);
            HOperatorSet.GenEmptyObj(out ho_R);
            HOperatorSet.GenEmptyObj(out ho_G);
            HOperatorSet.GenEmptyObj(out ho_B);
            HOperatorSet.GenEmptyObj(out ho_H);
            HOperatorSet.GenEmptyObj(out ho_S);
            HOperatorSet.GenEmptyObj(out ho_V);
            HOperatorSet.GenEmptyObj(out ho_l);
            HOperatorSet.GenEmptyObj(out ho_a);
            HOperatorSet.GenEmptyObj(out ho_b);
            HOperatorSet.GenEmptyObj(out ho_ImageLab);
            HOperatorSet.GenEmptyObj(out ho_ImageWorkingRGB);
            HOperatorSet.GenEmptyObj(out ho_MascaraWorking);
            HOperatorSet.GenEmptyObj(out ho_Imagenworking);
            HOperatorSet.GenEmptyObj(out ho_ImR);
            HOperatorSet.GenEmptyObj(out ho_ImG);
            HOperatorSet.GenEmptyObj(out ho_ImB);
            HOperatorSet.GenEmptyObj(out ho_L);
            HOperatorSet.GenEmptyObj(out ho_ImageLAB);
            HOperatorSet.GenEmptyObj(out ho_ImageLabOut);
            HOperatorSet.GenEmptyObj(out ho_HistogramOut);
            HOperatorSet.GenEmptyObj(out ho_Hist_domain);
            HOperatorSet.GenEmptyObj(out ho_HistogramOutlineal);
            HOperatorSet.GenEmptyObj(out ho_Maxpos);
            HOperatorSet.GenEmptyObj(out ho_TiledImageRGB);
            HOperatorSet.GenEmptyObj(out ho_Imagencolorconjuntoreduced);
            HOperatorSet.GenEmptyObj(out ho_ClassRegionsLUT);
            HOperatorSet.GenEmptyObj(out ho_totalRegionLUT);
            HOperatorSet.GenEmptyObj(out ho_ColorCuerpo);
            HOperatorSet.GenEmptyObj(out ho_slectedobj);
            HOperatorSet.GenEmptyObj(out ho_Hue);
            HOperatorSet.GenEmptyObj(out ho_Saturation);
            HOperatorSet.GenEmptyObj(out ho_Intensity);
            HOperatorSet.GenEmptyObj(out ho_Image1Median5);
            HOperatorSet.GenEmptyObj(out ho_ImagenFiltrada1);
            HOperatorSet.GenEmptyObj(out ho_Image1NIR);
            try
            {
                //  Parámetros de entrada:
                //    - ImagenRGB
                //    - ImagenNIR
                //
                //  Parámetros de salida:
                //    - message_queue
                //
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "ID", 0);

                //************************************************* MÁSCARAS **************************************************
                //*************************************************************************************************************
                ho_MascaraRGB.Dispose();
                HOperatorSet.Threshold(ho_ImagenRGB, out ho_MascaraRGB, 10, 255);
                ho_MascaraNIR.Dispose();
                HOperatorSet.Threshold(ho_ImagenNIR, out ho_MascaraNIR, 10, 255);

                //********************************************** NIVEL DE GRIS ************************************************
                //*************************************************************************************************************
                //Imagen escala de grises
                ho_Gray.Dispose();
                HOperatorSet.Rgb1ToGray(ho_ImagenRGB, out ho_Gray);
                //MeanGray,
                HOperatorSet.Intensity(ho_MascaraRGB, ho_Gray, out hv_MeanGray, out hv_DeviationGray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanGray", hv_MeanGray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationGray", hv_DeviationGray);
                //GrayPlaneDeviation
                HOperatorSet.PlaneDeviation(ho_MascaraRGB, ho_Gray, out hv_GrayPlaneDeviation);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "GrayPlaneDeviation", hv_GrayPlaneDeviation);
                //EntropyGray, AnisotropyGray
                HOperatorSet.EntropyGray(ho_MascaraRGB, ho_Gray, out hv_EntropyGray, out hv_AnisotropyGray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyGray", hv_EntropyGray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyGray", hv_AnisotropyGray);
                //EntropyFuzzyGray
                HOperatorSet.FuzzyEntropy(ho_MascaraRGB, ho_Gray, 0, 255, out hv_EntropyFuzzyGray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyGray", hv_EntropyFuzzyGray);
                //PerimeterFuzzyGray
                HOperatorSet.FuzzyPerimeter(ho_MascaraRGB, ho_Gray, 0, 255, out hv_PerimeterFuzzyGray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyGray", hv_PerimeterFuzzyGray);
                //Matriz de co-ocurrencia 0
                ho_Matrix0Gray.Dispose();
                HOperatorSet.GenCoocMatrix(ho_MascaraRGB, ho_Gray, out ho_Matrix0Gray, 6, 0);
                //Energy0gray, Correlation0gray, Homogeneity0gray, Contrast0gray
                HOperatorSet.CoocFeatureMatrix(ho_Matrix0Gray, out hv_Energy0gray, out hv_Correlation0gray,
                    out hv_Homogeneity0gray, out hv_Contrast0gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Energy0gray", hv_Energy0gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Correlation0gray", hv_Correlation0gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Homogeneity0gray", hv_Homogeneity0gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Contrast0gray", hv_Contrast0gray);
                //Matriz de co-ocurrencia 45
                ho_Matrix45Gray.Dispose();
                HOperatorSet.GenCoocMatrix(ho_MascaraRGB, ho_Gray, out ho_Matrix45Gray, 6,
                    45);
                //Energy45gray, Correlation45gray, Homogeneity45gray, Contrast45gray
                HOperatorSet.CoocFeatureMatrix(ho_Matrix45Gray, out hv_Energy45gray, out hv_Correlation45gray,
                    out hv_Homogeneity45gray, out hv_Contrast45gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Energy45gray", hv_Energy45gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Correlation45gray", hv_Correlation45gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Homogeneity45gray", hv_Homogeneity45gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Contrast45gray", hv_Contrast45gray);
                //Matriz de co-ocurrencia 90
                ho_Matrix90Gray.Dispose();
                HOperatorSet.GenCoocMatrix(ho_MascaraRGB, ho_Gray, out ho_Matrix90Gray, 6,
                    90);
                //Energy90gray, Correlation90gray, Homogeneity90gray, Contrast90gray
                HOperatorSet.CoocFeatureMatrix(ho_Matrix90Gray, out hv_Energy90gray, out hv_Correlation90gray,
                    out hv_Homogeneity90gray, out hv_Contrast90gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Energy90gray", hv_Energy90gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Correlation90gray", hv_Correlation90gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Homogeneity90gray", hv_Homogeneity90gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Contrast90gray", hv_Contrast90gray);
                //Matriz de co-ocurrencia 135
                ho_Matrix135Gray.Dispose();
                HOperatorSet.GenCoocMatrix(ho_MascaraRGB, ho_Gray, out ho_Matrix135Gray, 6,
                    135);
                //Energy135gray, Correlation135gray, Homogeneity135gray, Contrast135gray
                HOperatorSet.CoocFeatureMatrix(ho_Matrix135Gray, out hv_Energy135gray, out hv_Correlation135gray,
                    out hv_Homogeneity135gray, out hv_Contrast135gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Energy135gray", hv_Energy135gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Correlation135gray", hv_Correlation135gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Homogeneity135gray", hv_Homogeneity135gray);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Contrast135gray", hv_Contrast135gray);
                //*************************************************** RGB *****************************************************
                //*************************************************************************************************************
                //Canal R de la imagen RGB
                ho_R.Dispose();
                HOperatorSet.AccessChannel(ho_ImagenRGB, out ho_R, 1);
                //MeanR, DeviationR
                HOperatorSet.Intensity(ho_MascaraRGB, ho_R, out hv_MeanR, out hv_DeviationR);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanR", hv_MeanR);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationR", hv_DeviationR);
                //RPlaneDeviation
                HOperatorSet.PlaneDeviation(ho_MascaraRGB, ho_R, out hv_RPlaneDeviation);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "RPlaneDeviation", hv_RPlaneDeviation);
                //EntropyR, AnisotropyR
                HOperatorSet.EntropyGray(ho_MascaraRGB, ho_R, out hv_EntropyR, out hv_AnisotropyR);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyR", hv_EntropyR);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyR", hv_AnisotropyR);
                //EntropyFuzzyR
                HOperatorSet.FuzzyEntropy(ho_MascaraRGB, ho_R, 0, 255, out hv_EntropyFuzzyR);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyR", hv_EntropyFuzzyR);
                //PerimeterFuzzyR
                HOperatorSet.FuzzyPerimeter(ho_MascaraRGB, ho_R, 0, 255, out hv_PerimeterFuzzyR);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyR", hv_PerimeterFuzzyR);
                //Canal G de la imagen RGB
                ho_G.Dispose();
                HOperatorSet.AccessChannel(ho_ImagenRGB, out ho_G, 1);
                //MeanG, DeviationG
                HOperatorSet.Intensity(ho_MascaraRGB, ho_G, out hv_MeanG, out hv_DeviationG);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanG", hv_MeanG);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationG", hv_DeviationG);
                //GPlaneDeviation
                HOperatorSet.PlaneDeviation(ho_MascaraRGB, ho_G, out hv_GPlaneDeviation);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "GPlaneDeviation", hv_GPlaneDeviation);
                //EntropyG, AnisotropyG
                HOperatorSet.EntropyGray(ho_MascaraRGB, ho_G, out hv_EntropyG, out hv_AnisotropyG);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyG", hv_EntropyG);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyG", hv_AnisotropyG);
                //EntropyFuzzyG
                HOperatorSet.FuzzyEntropy(ho_MascaraRGB, ho_G, 0, 255, out hv_EntropyFuzzyG);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyG", hv_EntropyFuzzyG);
                //PerimeterFuzzyG
                HOperatorSet.FuzzyPerimeter(ho_MascaraRGB, ho_G, 0, 255, out hv_PerimeterFuzzyG);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyG", hv_PerimeterFuzzyG);
                //Canal B de la imagen RGB
                ho_B.Dispose();
                HOperatorSet.AccessChannel(ho_ImagenRGB, out ho_B, 3);
                //MeanB, DeviationB
                HOperatorSet.Intensity(ho_MascaraRGB, ho_B, out hv_MeanB, out hv_DeviationB);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanB", hv_MeanB);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationB", hv_DeviationB);
                //BPlaneDeviation
                HOperatorSet.PlaneDeviation(ho_MascaraRGB, ho_B, out hv_BPlaneDeviation);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "BPlaneDeviation", hv_BPlaneDeviation);
                //EntropyB, AnisotropyB
                HOperatorSet.EntropyGray(ho_MascaraRGB, ho_B, out hv_EntropyB, out hv_AnisotropyB);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyB", hv_EntropyB);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyB", hv_AnisotropyB);
                //EntropyFuzzyB
                HOperatorSet.FuzzyEntropy(ho_MascaraRGB, ho_B, 0, 255, out hv_EntropyFuzzyB);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyB", hv_EntropyFuzzyB);
                //PerimeterFuzzyB
                HOperatorSet.FuzzyPerimeter(ho_MascaraRGB, ho_B, 0, 255, out hv_PerimeterFuzzyB);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyB", hv_PerimeterFuzzyB);
                //*************************************************** HSV *****************************************************
                //*************************************************************************************************************
                //Imagen HSV
                ho_H.Dispose(); ho_S.Dispose(); ho_V.Dispose();
                HOperatorSet.TransFromRgb(ho_R, ho_G, ho_B, out ho_H, out ho_S, out ho_V, "hsv");
                //Canal H de la imagen HSV
                //MeanH, DeviationH
                HOperatorSet.Intensity(ho_MascaraRGB, ho_H, out hv_MeanH, out hv_DeviationH);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanH", hv_MeanH);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationH", hv_DeviationH);
                //HPlaneDeviation
                HOperatorSet.PlaneDeviation(ho_MascaraRGB, ho_H, out hv_HPlaneDeviation);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "HPlaneDeviation", hv_HPlaneDeviation);
                //EntropyH, AnisotropyH
                HOperatorSet.EntropyGray(ho_MascaraRGB, ho_H, out hv_EntropyH, out hv_AnisotropyH);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyH", hv_EntropyH);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyH", hv_AnisotropyH);
                //EntropyFuzzyH
                HOperatorSet.FuzzyEntropy(ho_MascaraRGB, ho_H, 0, 255, out hv_EntropyFuzzyH);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyH", hv_EntropyFuzzyH);
                //PerimeterFuzzyH
                HOperatorSet.FuzzyPerimeter(ho_MascaraRGB, ho_H, 0, 255, out hv_PerimeterFuzzyH);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyH", hv_PerimeterFuzzyH);
                //Canal S de la imagen HSV
                //MeanS, DeviationS
                HOperatorSet.Intensity(ho_MascaraRGB, ho_S, out hv_MeanS, out hv_DeviationS);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanS", hv_MeanS);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationS", hv_DeviationS);
                //SPlaneDeviation
                HOperatorSet.PlaneDeviation(ho_MascaraRGB, ho_S, out hv_SPlaneDeviation);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "SPlaneDeviation", hv_SPlaneDeviation);
                //EntropyS, AnisotropyS
                HOperatorSet.EntropyGray(ho_MascaraRGB, ho_S, out hv_EntropyS, out hv_AnisotropyS);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyS", hv_EntropyS);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyS", hv_AnisotropyS);
                //EntropyFuzzyS
                HOperatorSet.FuzzyEntropy(ho_MascaraRGB, ho_S, 0, 255, out hv_EntropyFuzzyS);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyS", hv_EntropyFuzzyS);
                //PerimeterFuzzyS
                HOperatorSet.FuzzyPerimeter(ho_MascaraRGB, ho_S, 0, 255, out hv_PerimeterFuzzyS);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyS", hv_PerimeterFuzzyS);
                //Canal V de la imagen HSV
                //MeanV, DeviationV
                HOperatorSet.Intensity(ho_MascaraRGB, ho_V, out hv_MeanV, out hv_DeviationV);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanV", hv_MeanV);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationV", hv_DeviationV);
                //VPlaneDeviation
                HOperatorSet.PlaneDeviation(ho_MascaraRGB, ho_V, out hv_VPlaneDeviation);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "VPlaneDeviation", hv_VPlaneDeviation);
                //EntropyV, AnisotropyV
                HOperatorSet.EntropyGray(ho_MascaraRGB, ho_V, out hv_EntropyV, out hv_AnisotropyV);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyV", hv_EntropyV);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyV", hv_AnisotropyV);
                //EntropyFuzzyV
                HOperatorSet.FuzzyEntropy(ho_MascaraRGB, ho_V, 0, 255, out hv_EntropyFuzzyV);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyV", hv_EntropyFuzzyV);
                //PerimeterFuzzyV
                HOperatorSet.FuzzyPerimeter(ho_MascaraRGB, ho_B, 0, 255, out hv_PerimeterFuzzyV);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyV", hv_PerimeterFuzzyV);
                //************************************************* CIElab ****************************************************
                //*************************************************************************************************************
                //Imagen CIElab
                ho_l.Dispose(); ho_a.Dispose(); ho_b.Dispose();
                HOperatorSet.TransFromRgb(ho_R, ho_G, ho_B, out ho_l, out ho_a, out ho_b, "cielab");
                //Canal l de la imagen CIElab
                //Meanl, Deviationl
                HOperatorSet.Intensity(ho_MascaraRGB, ho_l, out hv_Meanl, out hv_Deviationl);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Meanl", hv_Meanl);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Deviationl", hv_Deviationl);
                //lPlaneDeviation
                HOperatorSet.PlaneDeviation(ho_MascaraRGB, ho_l, out hv_lPlaneDeviation);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "lPlaneDeviation", hv_lPlaneDeviation);
                //Entropyl, Anisotropyl
                HOperatorSet.EntropyGray(ho_MascaraRGB, ho_l, out hv_Entropyl, out hv_Anisotropyl);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Entropyl", hv_Entropyl);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Anisotropyl", hv_Anisotropyl);
                //EntropyFuzzyl
                HOperatorSet.FuzzyEntropy(ho_MascaraRGB, ho_l, 0, 255, out hv_EntropyFuzzyl);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyl", hv_EntropyFuzzyl);
                //PerimeterFuzzyl
                HOperatorSet.FuzzyPerimeter(ho_MascaraRGB, ho_l, 0, 255, out hv_PerimeterFuzzyl);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyl", hv_PerimeterFuzzyl);

                //Canal a de la imagen CIElab
                //Meana, Deviationa
                HOperatorSet.Intensity(ho_MascaraRGB, ho_a, out hv_Meana, out hv_Deviationa);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Meana", hv_Meana);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Deviationa", hv_Deviationa);
                //aPlaneDeviation
                HOperatorSet.PlaneDeviation(ho_MascaraRGB, ho_a, out hv_aPlaneDeviation);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "aPlaneDeviation", hv_aPlaneDeviation);
                //Entropya, Anisotropya
                HOperatorSet.EntropyGray(ho_MascaraRGB, ho_a, out hv_Entropya, out hv_Anisotropya);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Entropya", hv_Entropya);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Anisotropya", hv_Anisotropya);
                //EntropyFuzzya
                HOperatorSet.FuzzyEntropy(ho_MascaraRGB, ho_a, 0, 255, out hv_EntropyFuzzya);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzya", hv_EntropyFuzzya);
                //PerimeterFuzzya
                HOperatorSet.FuzzyPerimeter(ho_MascaraRGB, ho_a, 0, 255, out hv_PerimeterFuzzya);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzya", hv_PerimeterFuzzya);

                //Canal b de la imagen CIElab
                //Meanb, Deviationb
                HOperatorSet.Intensity(ho_MascaraRGB, ho_b, out hv_Meanb, out hv_Deviationb);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Meanb", hv_Meanb);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Deviationb", hv_Deviationb);
                //bPlaneDeviation
                HOperatorSet.PlaneDeviation(ho_MascaraRGB, ho_b, out hv_bPlaneDeviation);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "bPlaneDeviation", hv_bPlaneDeviation);
                //Entropyb, Anisotropyb
                HOperatorSet.EntropyGray(ho_MascaraRGB, ho_b, out hv_Entropyb, out hv_Anisotropyb);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Entropyb", hv_Entropyb);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Anisotropyb", hv_Anisotropyb);
                //EntropyFuzzyb
                HOperatorSet.FuzzyEntropy(ho_MascaraRGB, ho_b, 0, 255, out hv_EntropyFuzzyb);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyb", hv_EntropyFuzzyb);
                //PerimeterFuzzyb
                HOperatorSet.FuzzyPerimeter(ho_MascaraRGB, ho_b, 0, 255, out hv_PerimeterFuzzyb);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyb", hv_PerimeterFuzzyb);

                ho_ImageLab.Dispose();
                HOperatorSet.Compose3(ho_l, ho_a, ho_b, out ho_ImageLab);

                //************************************************ MORFOLOGÍA *************************************************
                //*************************************************************************************************************
                //Sacara la máscara del controno de la ImagenRBG (imagen sin fondo)
                //Largo, ancho
                HOperatorSet.SmallestRectangle2(ho_MascaraRGB, out hv_Row, out hv_Column, out hv_Phi,
                    out hv_Largo, out hv_Ancho);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Largo", hv_Largo);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Ancho", hv_Ancho);
                //Área
                HOperatorSet.AreaCenter(ho_MascaraRGB, out hv_Area, out hv_Row1, out hv_Column3);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Area", hv_Area);
                //Ra / Rb
                HOperatorSet.EllipticAxis(ho_MascaraRGB, out hv_Ra, out hv_Rb, out hv_Phi);
                HOperatorSet.TupleDiv(hv_Ra, hv_Rb, out hv_Ra_Rb);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Ra_Rb", hv_Ra_Rb);
                //Circularity
                HOperatorSet.Circularity(ho_MascaraRGB, out hv_Circularity);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Circularity", hv_Circularity);
                //Compactness
                HOperatorSet.Compactness(ho_MascaraRGB, out hv_Compactness);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Compactness", hv_Compactness);
                //Roundness
                HOperatorSet.Roundness(ho_MascaraRGB, out hv_Distance, out hv_Sigma, out hv_Roundness,
                    out hv_Sides);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Roundness", hv_Roundness);
                //Convexity
                HOperatorSet.Convexity(ho_MascaraRGB, out hv_Convexity);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Convexity", hv_Convexity);
                //Rectangularity
                HOperatorSet.Rectangularity(ho_MascaraRGB, out hv_Rectangularity);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Rectangularity", hv_Rectangularity);
                //Anisometry, Bulkiness, StructureFactor
                HOperatorSet.Eccentricity(ho_MascaraRGB, out hv_Anisometry, out hv_Bulkiness,
                    out hv_StructureFactor);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Anisometry", hv_Anisometry);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Bulkiness", hv_Bulkiness);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "StructureFactor", hv_StructureFactor);
                //PSI1, PSI2, PSI3, PSI4
                HOperatorSet.MomentsRegionCentralInvar(ho_MascaraRGB, out hv_PSI1, out hv_PSI2,
                    out hv_PSI3, out hv_PSI4);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PSI1", hv_PSI1);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PSI2", hv_PSI2);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PSI3", hv_PSI3);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PSI4", hv_PSI4);
                //M11, M20, M02
                HOperatorSet.MomentsRegion2ndInvar(ho_MascaraRGB, out hv_M11, out hv_M20, out hv_M02);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M11", hv_M11);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M20", hv_M20);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M02", hv_M02);
                //M21, M12, M03, M30
                HOperatorSet.MomentsRegion3rdInvar(ho_MascaraRGB, out hv_M21, out hv_M12, out hv_M03,
                    out hv_M30);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M21", hv_M21);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M12", hv_M12);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M03", hv_M03);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M30", hv_M30);
                //************************************************ Histograma de color ab*************************************************
                //*************************************************************************************************************
                ho_ImageWorkingRGB.Dispose();
                ho_ImageWorkingRGB = ho_ImagenRGB.CopyObj(1, -1);
                ho_MascaraWorking.Dispose();
                ho_MascaraWorking = ho_MascaraRGB.CopyObj(1, -1);
                ho_Imagenworking.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageWorkingRGB, ho_MascaraWorking, out ho_Imagenworking
                    );
                ho_ImR.Dispose(); ho_ImG.Dispose(); ho_ImB.Dispose();
                HOperatorSet.Decompose3(ho_Imagenworking, out ho_ImR, out ho_ImG, out ho_ImB
                    );
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConvertImageType(ho_ImR, out ExpTmpOutVar_0, "real");
                    ho_ImR.Dispose();
                    ho_ImR = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConvertImageType(ho_ImG, out ExpTmpOutVar_0, "real");
                    ho_ImG.Dispose();
                    ho_ImG = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConvertImageType(ho_ImB, out ExpTmpOutVar_0, "real");
                    ho_ImB.Dispose();
                    ho_ImB = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ScaleImage(ho_ImR, out ExpTmpOutVar_0, 1 / 255.0, 0.0);
                    ho_ImR.Dispose();
                    ho_ImR = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ScaleImage(ho_ImG, out ExpTmpOutVar_0, 1 / 255.0, 0.0);
                    ho_ImG.Dispose();
                    ho_ImG = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ScaleImage(ho_ImB, out ExpTmpOutVar_0, 1 / 255.0, 0.0);
                    ho_ImB.Dispose();
                    ho_ImB = ExpTmpOutVar_0;
                }
                ho_L.Dispose(); ho_a.Dispose(); ho_b.Dispose();
                HOperatorSet.TransFromRgb(ho_ImR, ho_ImG, ho_ImB, out ho_L, out ho_a, out ho_b,
                    "cielab");
                ho_ImageLAB.Dispose();
                HOperatorSet.Compose3(ho_L, ho_a, ho_b, out ho_ImageLAB);
                //funcion para el calculo del histograma de color ab
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_ImageWorkingRGB, ho_MascaraWorking, out ExpTmpOutVar_0
                        );
                    ho_ImageWorkingRGB.Dispose();
                    ho_ImageWorkingRGB = ExpTmpOutVar_0;
                }
                hv_gaussSize = 0.2;
                hv_MAX_Val = 20.0;
                ho_ImageLabOut.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ImageLabOut);

                ho_HistogramOut.Dispose();
                Histo_ab_generator(ho_ImageWorkingRGB, ho_MascaraWorking, ho_ImageLAB, out ho_HistogramOut,
                    hv_gaussSize, hv_MAX_Val);
                ho_Hist_domain.Dispose();
                HOperatorSet.GenRectangle1(out ho_Hist_domain, 0, 0, hv_MAX_Val, hv_MAX_Val);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_HistogramOut, ho_Hist_domain, out ExpTmpOutVar_0
                        );
                    ho_HistogramOut.Dispose();
                    ho_HistogramOut = ExpTmpOutVar_0;
                }
                ho_HistogramOutlineal.Dispose();
                HOperatorSet.CropDomain(ho_HistogramOut, out ho_HistogramOutlineal);

                HOperatorSet.MinMaxGray(ho_HistogramOut, ho_HistogramOut, 0, out hv_Min, out hv_Max,
                    out hv_Range);
                ho_Maxpos.Dispose();
                HOperatorSet.Threshold(ho_HistogramOut, out ho_Maxpos, hv_Max, hv_Max);
                HOperatorSet.AreaCenter(ho_Maxpos, out hv_area, out hv_Max_x, out hv_Max_y);
                HOperatorSet.MomentsGrayPlane(ho_HistogramOut, ho_HistogramOut, out hv_MRow,
                    out hv_MCol, out hv_Alpha, out hv_Beta, out hv_Mean);


                hv_texto = "COLOR_Histo_ab_";
                HTuple end_val330 = hv_MAX_Val - 1;
                HTuple step_val330 = 1;
                for (hv_Index1 = 0; hv_Index1.Continue(end_val330, step_val330); hv_Index1 = hv_Index1.TupleAdd(step_val330))
                {
                    HTuple end_val331 = hv_MAX_Val - 1;
                    HTuple step_val331 = 1;
                    for (hv_Index2 = 0; hv_Index2.Continue(end_val331, step_val331); hv_Index2 = hv_Index2.TupleAdd(step_val331))
                    {
                        hv_textof = ((hv_texto + hv_Index1) + "_") + hv_Index2;
                        HOperatorSet.GetGrayval(ho_HistogramOut, hv_Index1, hv_Index2, out hv_Grayval);
                        HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_textof, hv_Grayval);
                    }
                }
                hv_texto = "COLOR_Histo_ab_max_X";
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_texto, hv_Max_x);
                hv_texto = "COLOR_Histo_ab_max_Y";
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_texto, hv_Max_y);
                hv_texto = "COLOR_Histo_ab_Dev_X";
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_texto, hv_MCol);
                hv_texto = "COLOR_Histo_ab_Dev_Y";
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_texto, hv_MRow);
                //************************************  Color LAB **************************************
                //*************************************************************************************************************
                //Parametros color RGB Cuerpo
                ho_ImageWorkingRGB.Dispose();
                ho_ImageWorkingRGB = ho_ImagenRGB.CopyObj(1, -1);
                ho_MascaraWorking.Dispose();
                ho_MascaraWorking = ho_MascaraRGB.CopyObj(1, -1);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_ImageWorkingRGB, ho_MascaraRGB, out ExpTmpOutVar_0
                        );
                    ho_ImageWorkingRGB.Dispose();
                    ho_ImageWorkingRGB = ExpTmpOutVar_0;
                }
                //calcula los parametros PCA de la imagen y mascara asignados
                Calcula_PCA_lab_params(ho_ImageWorkingRGB, ho_MascaraWorking, ho_ImageLAB,
                    out hv_Trans, out hv_TransInv, out hv_MeanLab, out hv_CovLab, out hv_InfoPerComp);
                hv_num = new HTuple(hv_TransInv.TupleLength());
                if ((int)(new HTuple(hv_num.TupleEqual(0))) != 0)
                {
                    hv_TransInv = new HTuple();
                    hv_TransInv[0] = 0;
                    hv_TransInv[1] = 0;
                    hv_TransInv[2] = 0;
                    hv_TransInv[3] = 0;
                    hv_TransInv[4] = 0;
                    hv_TransInv[5] = 0;
                    hv_TransInv[6] = 0;
                    hv_TransInv[7] = 0;
                    hv_TransInv[8] = 0;
                    hv_TransInv[9] = 0;
                    hv_TransInv[10] = 0;
                    hv_TransInv[11] = 0;
                }
                hv_MeanL = hv_MeanLab.TupleSelect(0);
                hv_Meana = hv_MeanLab.TupleSelect(1);
                hv_Meanb = hv_MeanLab.TupleSelect(2);
                hv_DevL = ((hv_CovLab.TupleSelect(0))).TupleSqrt();
                hv_Deva = ((hv_CovLab.TupleSelect(4))).TupleSqrt();
                hv_Devb = ((hv_CovLab.TupleSelect(8))).TupleSqrt();

                //parametros de color medias y desviaciones
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_Mean_cuerpo_lineal_L",
                    hv_MeanL);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_Mean_cuerpo_lineal_a",
                    hv_Meana);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_Mean_cuerpo_lineal_b",
                    hv_Meanb);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_Dev_cuerpo_lineal_L",
                    hv_DevL);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_Dev_cuerpo_lineal_a",
                    hv_Deva);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_Dev_cuerpo_lineal_b",
                    hv_Devb);
                //vector Y
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_X_1",
                    hv_TransInv.TupleSelect(0));
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_X_2",
                    hv_TransInv.TupleSelect(4));
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_X_3",
                    hv_TransInv.TupleSelect(8));
                //vector Y
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_Y_1",
                    hv_TransInv.TupleSelect(1));
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_Y_2",
                    hv_TransInv.TupleSelect(5));
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_Y_3",
                    hv_TransInv.TupleSelect(9));
                //vector Z
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_Z_1",
                    hv_TransInv.TupleSelect(2));
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_Z_2",
                    hv_TransInv.TupleSelect(6));
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_Z_3",
                    hv_TransInv.TupleSelect(10));
                //origen de coordenadas
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_O_1",
                    hv_TransInv.TupleSelect(3));
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_O_2",
                    hv_TransInv.TupleSelect(7));
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_O_3",
                    hv_TransInv.TupleSelect(11));


                //***********************************************SEGMENTACION COLOR********************************************
                //**************************************************************************************************
                ho_ImageWorkingRGB.Dispose();
                ho_ImageWorkingRGB = ho_ImagenRGB.CopyObj(1, -1);
                ho_MascaraWorking.Dispose();
                ho_MascaraWorking = ho_MascaraRGB.CopyObj(1, -1);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_ImageWorkingRGB, ho_MascaraRGB, out ExpTmpOutVar_0
                        );
                    ho_ImageWorkingRGB.Dispose();
                    ho_ImageWorkingRGB = ExpTmpOutVar_0;
                }
                ho_TiledImageRGB.Dispose();
                HOperatorSet.MedianImage(ho_ImageWorkingRGB, out ho_TiledImageRGB, "circle",
                    2, "mirrored");
                ho_Imagencolorconjuntoreduced.Dispose();
                decimar_imagen(ho_TiledImageRGB, out ho_Imagencolorconjuntoreduced);
                ho_ClassRegionsLUT.Dispose();
                HOperatorSet.ClassifyImageClassLut(ho_Imagencolorconjuntoreduced, out ho_ClassRegionsLUT,
                    hv_ClassLUTHandle);

                ho_totalRegionLUT.Dispose();
                HOperatorSet.GetDomain(ho_TiledImageRGB, out ho_totalRegionLUT);
                HOperatorSet.AreaCenter(ho_totalRegionLUT, out hv_AreaTotalLUT, out hv_RowTotalLUT,
                    out hv_ColumnTotalLUT);
                HOperatorSet.AreaCenter(ho_ClassRegionsLUT, out hv_AreaLUT, out hv_RowLUT,
                    out hv_ColumnLUT);
                HOperatorSet.TupleReal(hv_AreaLUT, out hv_AreaLUT);
                HOperatorSet.TupleReal(hv_AreaTotalLUT, out hv_AreaTotalLUT);

                hv_AreaLUT = (100 * hv_AreaLUT) / hv_AreaTotalLUT;
                hv_texto = "Patt_Dist";
                HOperatorSet.CountObj(ho_ClassRegionsLUT, out hv_pattern_elements);
                HTuple end_val407 = hv_pattern_elements;
                HTuple step_val407 = 1;
                for (hv_Index1 = 1; hv_Index1.Continue(end_val407, step_val407); hv_Index1 = hv_Index1.TupleAdd(step_val407))
                {
                    hv_textof = (hv_texto + "_") + hv_Index1;
                    HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_textof, hv_AreaLUT.TupleSelect(
                        hv_Index1 - 1));
                }

                //color mayoritario cuerpo area
                HOperatorSet.AreaCenter(ho_ClassRegionsLUT, out hv_AreaCuerpo, out hv_Row,
                    out hv_Columm);
                //genero una region con los tres colores mayoritarios
                ho_ColorCuerpo.Dispose();
                HOperatorSet.GenEmptyObj(out ho_ColorCuerpo);
                HOperatorSet.TupleSortIndex(hv_AreaCuerpo, out hv_IndicesCuerpo);
                hv_dimColores = new HTuple(hv_AreaCuerpo.TupleLength());
                HTuple end_val418 = hv_dimColores;
                HTuple step_val418 = 1;
                for (hv_Index1 = 1; hv_Index1.Continue(end_val418, step_val418); hv_Index1 = hv_Index1.TupleAdd(step_val418))
                {
                    hv_pos = hv_IndicesCuerpo.TupleSelect(hv_dimColores - hv_Index1);
                    ho_slectedobj.Dispose();
                    HOperatorSet.SelectObj(ho_ClassRegionsLUT, out ho_slectedobj, hv_pos + 1);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_ColorCuerpo, ho_slectedobj, out ExpTmpOutVar_0);
                        ho_ColorCuerpo.Dispose();
                        ho_ColorCuerpo = ExpTmpOutVar_0;
                    }
                }
                //region_to_mean (ClassRegionsLUT, Imagencolorconjuntoreduced, ImageMean)
                ho_Hue.Dispose(); ho_Saturation.Dispose(); ho_Intensity.Dispose();
                HOperatorSet.Decompose3(ho_ImageLAB, out ho_Hue, out ho_Saturation, out ho_Intensity
                    );
                //escribe los colores en orden de la tabla
                hv_texto = "Color";
                hv_dimColores = new HTuple(hv_AreaCuerpo.TupleLength());
                HTuple end_val428 = hv_dimColores;
                HTuple step_val428 = 1;
                for (hv_Index1 = 1; hv_Index1.Continue(end_val428, step_val428); hv_Index1 = hv_Index1.TupleAdd(step_val428))
                {
                    //si se quiere por orden descendente ColorCuerpo
                    ho_slectedobj.Dispose();
                    HOperatorSet.SelectObj(ho_ColorCuerpo, out ho_slectedobj, hv_Index1);
                    HOperatorSet.Intensity(ho_slectedobj, ho_Hue, out hv_MeanH, out hv_Dev);
                    HOperatorSet.Intensity(ho_slectedobj, ho_Saturation, out hv_MeanS, out hv_Dev);
                    HOperatorSet.Intensity(ho_slectedobj, ho_Intensity, out hv_MeanV, out hv_Dev);
                    HOperatorSet.TupleReal(hv_MeanH, out hv_MeanH);
                    HOperatorSet.TupleReal(hv_MeanS, out hv_MeanS);
                    HOperatorSet.TupleReal(hv_MeanV, out hv_MeanV);
                    hv_textof = (hv_texto + "_Media_L_") + hv_Index1;
                    HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_textof, hv_MeanH);
                    hv_textof = (hv_texto + "_Media_a_") + hv_Index1;
                    HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_textof, hv_MeanS);
                    hv_textof = (hv_texto + "_Media_b_") + hv_Index1;
                    HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_textof, hv_MeanV);


                }


                //*****************************************TEXTURA*************************************************
                //*********************************************************************************************
                ho_Image1Median5.Dispose();
                HOperatorSet.MedianImage(ho_ImagenNIR, out ho_Image1Median5, "circle", 2, "mirrored");
                ho_ImagenFiltrada1.Dispose();
                Difuminado_rapido_gauss(ho_Image1Median5, ho_MascaraNIR, out ho_ImagenFiltrada1,
                    10, 6);
                ho_Image1NIR.Dispose();
                HOperatorSet.DivImage(ho_Image1Median5, ho_ImagenFiltrada1, out ho_Image1NIR,
                    124, 0);
                Histo_gray_generator(ho_Image1NIR, ho_MascaraNIR, 100, out hv_HistogramGrayOut);
                HOperatorSet.CreateFunct1dArray(hv_HistogramGrayOut, out hv_Function);
                HOperatorSet.SmoothFunct1dGauss(hv_Function, 1, out hv_SmoothedFunction);
                HOperatorSet.Funct1dToPairs(hv_SmoothedFunction, out hv_XValues, out hv_HistogramGrayOut);

                //mensajes Histogramas color
                hv_texto = "TEXTURA_Histo";
                for (hv_Index1 = 0; (int)hv_Index1 <= (int)(100 - 1); hv_Index1 = (int)hv_Index1 + 1)
                {
                    hv_textof = (hv_texto + "_") + hv_Index1;
                    HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_textof, hv_HistogramGrayOut.TupleSelect(
                        hv_Index1));
                }


                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_pertenencia_1",
                    0);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_pertenencia_2",
                    0);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_pertenencia_3",
                    0);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_pertenencia_4",
                    0);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_pertenencia_5",
                    0);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada_1", 0);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada_2", 0);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada_3", 0);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada_4", 0);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada_5", 0);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada_ponderada",
                    0);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada", 0);
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_Forzada", 0);

                ho_MascaraRGB.Dispose();
                ho_MascaraNIR.Dispose();
                ho_Gray.Dispose();
                ho_Matrix0Gray.Dispose();
                ho_Matrix45Gray.Dispose();
                ho_Matrix90Gray.Dispose();
                ho_Matrix135Gray.Dispose();
                ho_R.Dispose();
                ho_G.Dispose();
                ho_B.Dispose();
                ho_H.Dispose();
                ho_S.Dispose();
                ho_V.Dispose();
                ho_l.Dispose();
                ho_a.Dispose();
                ho_b.Dispose();
                ho_ImageLab.Dispose();
                ho_ImageWorkingRGB.Dispose();
                ho_MascaraWorking.Dispose();
                ho_Imagenworking.Dispose();
                ho_ImR.Dispose();
                ho_ImG.Dispose();
                ho_ImB.Dispose();
                ho_L.Dispose();
                ho_ImageLAB.Dispose();
                ho_ImageLabOut.Dispose();
                ho_HistogramOut.Dispose();
                ho_Hist_domain.Dispose();
                ho_HistogramOutlineal.Dispose();
                ho_Maxpos.Dispose();
                ho_TiledImageRGB.Dispose();
                ho_Imagencolorconjuntoreduced.Dispose();
                ho_ClassRegionsLUT.Dispose();
                ho_totalRegionLUT.Dispose();
                ho_ColorCuerpo.Dispose();
                ho_slectedobj.Dispose();
                ho_Hue.Dispose();
                ho_Saturation.Dispose();
                ho_Intensity.Dispose();
                ho_Image1Median5.Dispose();
                ho_ImagenFiltrada1.Dispose();
                ho_Image1NIR.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_MascaraRGB.Dispose();
                ho_MascaraNIR.Dispose();
                ho_Gray.Dispose();
                ho_Matrix0Gray.Dispose();
                ho_Matrix45Gray.Dispose();
                ho_Matrix90Gray.Dispose();
                ho_Matrix135Gray.Dispose();
                ho_R.Dispose();
                ho_G.Dispose();
                ho_B.Dispose();
                ho_H.Dispose();
                ho_S.Dispose();
                ho_V.Dispose();
                ho_l.Dispose();
                ho_a.Dispose();
                ho_b.Dispose();
                ho_ImageLab.Dispose();
                ho_ImageWorkingRGB.Dispose();
                ho_MascaraWorking.Dispose();
                ho_Imagenworking.Dispose();
                ho_ImR.Dispose();
                ho_ImG.Dispose();
                ho_ImB.Dispose();
                ho_L.Dispose();
                ho_ImageLAB.Dispose();
                ho_ImageLabOut.Dispose();
                ho_HistogramOut.Dispose();
                ho_Hist_domain.Dispose();
                ho_HistogramOutlineal.Dispose();
                ho_Maxpos.Dispose();
                ho_TiledImageRGB.Dispose();
                ho_Imagencolorconjuntoreduced.Dispose();
                ho_ClassRegionsLUT.Dispose();
                ho_totalRegionLUT.Dispose();
                ho_ColorCuerpo.Dispose();
                ho_slectedobj.Dispose();
                ho_Hue.Dispose();
                ho_Saturation.Dispose();
                ho_Intensity.Dispose();
                ho_Image1Median5.Dispose();
                ho_ImagenFiltrada1.Dispose();
                ho_Image1NIR.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void decimar_imagen(HObject ho_ImagencolorconjuntoMala, out HObject ho_ImagencolorconjuntoMalareduced)
        {



            // Local iconic variables 

            HObject ho_ImageConverted, ho_ImageConvertedOut;

            // Local control variables 

            HTuple hv_Max = null, hv_Min = null, hv_MAX_Val = null;
            HTuple hv_mult = null, hv_add = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImagencolorconjuntoMalareduced);
            HOperatorSet.GenEmptyObj(out ho_ImageConverted);
            HOperatorSet.GenEmptyObj(out ho_ImageConvertedOut);
            try
            {
                ho_ImageConverted.Dispose();
                HOperatorSet.ConvertImageType(ho_ImagencolorconjuntoMala, out ho_ImageConverted,
                    "byte");
                //ESCALA LA IMAGEN PARA REDUCIR LA DIMENSIONALIDAD
                hv_Max = 255.0;
                hv_Min = 0.0;
                hv_MAX_Val = 16.0;
                HOperatorSet.TupleReal(hv_MAX_Val, out hv_MAX_Val);
                HOperatorSet.TupleReal(hv_Max, out hv_Max);
                HOperatorSet.TupleReal(hv_Min, out hv_Min);
                hv_mult = hv_MAX_Val / (hv_Max - hv_Min);
                hv_add = (-hv_mult) * hv_Min;
                ho_ImageConvertedOut.Dispose();
                HOperatorSet.ScaleImage(ho_ImageConverted, out ho_ImageConvertedOut, hv_mult,
                    hv_add);
                hv_mult = 1 / hv_mult;
                ho_ImagencolorconjuntoMalareduced.Dispose();
                HOperatorSet.ScaleImage(ho_ImageConvertedOut, out ho_ImagencolorconjuntoMalareduced,
                    hv_mult, hv_add);
                //ImagencolorconjuntoMalareduced imagen con el numero de colores reducido MAX_Val^3
                ho_ImageConverted.Dispose();
                ho_ImageConvertedOut.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageConverted.Dispose();
                ho_ImageConvertedOut.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Short Description: Detect a fin 
        public void optimar_extraer_caracteristicas_Peces_null(HTuple hv_ParametrosCola)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_MAX_Val = null, hv_texto = null;
            HTuple hv_Index1 = null, hv_Index2 = new HTuple(), hv_textof = new HTuple();
            HTuple hv_num = null, hv_TransInv = new HTuple(), hv_MeanL = null;
            HTuple hv_Meana = null, hv_Meanb = null, hv_DevL = null;
            HTuple hv_Deva = null, hv_Devb = null, hv_dimColores = null;
            // Initialize local and output iconic variables 
            //  Parámetros de entrada:
            //    - ImagenRGB
            //    - ImagenNIR
            //
            //  Parámetros de salida:
            //    - message_queue
            //

            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "ID", 1);
            //********************************************** NIVEL DE GRIS ************************************************
            //*************************************************************************************************************
            //MeanGray,
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanGray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationGray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "GrayPlaneDeviation", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyGray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyGray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyGray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyGray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Energy0gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Correlation0gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Homogeneity0gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Contrast0gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Energy45gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Correlation45gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Homogeneity45gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Contrast45gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Energy90gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Correlation90gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Homogeneity90gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Contrast90gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Energy135gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Correlation135gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Homogeneity135gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Contrast135gray", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanR", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationR", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "RPlaneDeviation", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyR", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyR", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyR", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyR", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanG", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationG", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "GPlaneDeviation", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyG", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyG", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyG", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyG", 0);


            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanB", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationB", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "BPlaneDeviation", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyB", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyB", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyB", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyB", 0);



            //*************************************************** HSV *****************************************************
            //*************************************************************************************************************
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanH", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationH", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "HPlaneDeviation", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyH", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyH", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyH", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyH", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanS", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationS", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "SPlaneDeviation", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyS", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyS", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyS", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyS", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "MeanV", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "DeviationV", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "VPlaneDeviation", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyV", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "AnisotropyV", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyV", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyV", 0);



            //************************************************* CIElab ****************************************************
            //*************************************************************************************************************
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Meanl", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Deviationl", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "lPlaneDeviation", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Entropyl", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Anisotropyl", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyl", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyl", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Meana", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Deviationa", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "aPlaneDeviation", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Entropya", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Anisotropya", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzya", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzya", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Meanb", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Deviationb", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "bPlaneDeviation", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Entropyb", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Anisotropyb", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "EntropyFuzzyb", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PerimeterFuzzyb", 0);
            //************************************************ MORFOLOGÍA *************************************************
            //*************************************************************************************************************
            //Sacara la máscara del controno de la ImagenRBG (imagen sin fondo)
            //Largo, ancho
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Largo", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Ancho", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Area", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Ra_Rb", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Circularity", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Compactness", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Roundness", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Convexity", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Rectangularity", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Anisometry", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Bulkiness", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "StructureFactor", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PSI1", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PSI2", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PSI3", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "PSI4", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M11", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M20", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M02", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M21", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M12", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M03", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "M30", 0);
            //************************************************ Histograma de color ab*************************************************
            //*************************************************************************************************************
            hv_MAX_Val = 20.0;
            hv_texto = "COLOR_Histo_ab_";
            HTuple end_val141 = hv_MAX_Val - 1;
            HTuple step_val141 = 1;
            for (hv_Index1 = 0; hv_Index1.Continue(end_val141, step_val141); hv_Index1 = hv_Index1.TupleAdd(step_val141))
            {
                HTuple end_val142 = hv_MAX_Val - 1;
                HTuple step_val142 = 1;
                for (hv_Index2 = 0; hv_Index2.Continue(end_val142, step_val142); hv_Index2 = hv_Index2.TupleAdd(step_val142))
                {
                    hv_textof = ((hv_texto + hv_Index1) + "_") + hv_Index2;

                    HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_textof, 0);
                }
            }



            hv_texto = "COLOR_Histo_ab_max_X";
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_texto, 0);
            hv_texto = "COLOR_Histo_ab_max_Y";
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_texto, 0);
            hv_texto = "COLOR_Histo_ab_Dev_X";
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_texto, 0);
            hv_texto = "COLOR_Histo_ab_Dev_Y";
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_texto, 0);

            //************************************  Color LAB **************************************
            //*************************************************************************************************************

            hv_num = 0;
            if ((int)(new HTuple(hv_num.TupleEqual(0))) != 0)
            {
                hv_TransInv = new HTuple();
                hv_TransInv[0] = 0;
                hv_TransInv[1] = 0;
                hv_TransInv[2] = 0;
                hv_TransInv[3] = 0;
                hv_TransInv[4] = 0;
                hv_TransInv[5] = 0;
                hv_TransInv[6] = 0;
                hv_TransInv[7] = 0;
                hv_TransInv[8] = 0;
                hv_TransInv[9] = 0;
                hv_TransInv[10] = 0;
                hv_TransInv[11] = 0;
            }
            hv_MeanL = 0;
            hv_Meana = 0;
            hv_Meanb = 0;
            hv_DevL = 0;
            hv_Deva = 0;
            hv_Devb = 0;

            //parametros de color medias y desviaciones
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_Mean_cuerpo_lineal_L",
                hv_MeanL);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_Mean_cuerpo_lineal_a",
                hv_Meana);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_Mean_cuerpo_lineal_b",
                hv_Meanb);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_Dev_cuerpo_lineal_L",
                hv_DevL);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_Dev_cuerpo_lineal_a",
                hv_Deva);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_Dev_cuerpo_lineal_b",
                hv_Devb);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_X_1",
                hv_TransInv.TupleSelect(0));
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_X_2",
                hv_TransInv.TupleSelect(4));
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_X_3",
                hv_TransInv.TupleSelect(8));
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_Y_1",
                hv_TransInv.TupleSelect(1));
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_Y_2",
                hv_TransInv.TupleSelect(5));
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_Y_3",
                hv_TransInv.TupleSelect(9));
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_Z_1",
                hv_TransInv.TupleSelect(2));
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_Z_2",
                hv_TransInv.TupleSelect(6));
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_Z_3",
                hv_TransInv.TupleSelect(10));
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_O_1",
                hv_TransInv.TupleSelect(3));
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_O_2",
                hv_TransInv.TupleSelect(7));
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "COLOR_PCA_cuerpo_lineal_lab_O_3",
                hv_TransInv.TupleSelect(11));

            //***********************************************SEGMENTACION COLOR********************************************
            //**************************************************************************************************

            hv_texto = "Patt_Dist";

            for (hv_Index1 = 1; (int)hv_Index1 <= 88; hv_Index1 = (int)hv_Index1 + 1)
            {
                hv_textof = (hv_texto + "_") + hv_Index1;
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_textof, 0);
            }

            //c
            hv_texto = "Color";
            hv_dimColores = 88;
            HTuple end_val207 = hv_dimColores;
            HTuple step_val207 = 1;
            for (hv_Index1 = 1; hv_Index1.Continue(end_val207, step_val207); hv_Index1 = hv_Index1.TupleAdd(step_val207))
            {

                hv_textof = (hv_texto + "_Media_L_") + hv_Index1;
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_textof, 0);
                hv_textof = (hv_texto + "_Media_a_") + hv_Index1;
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_textof, 0);
                hv_textof = (hv_texto + "_Media_b_") + hv_Index1;
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_textof, 0);

            }
            //*****************************************TEXTURA*************************************************
            //*********************************************************************************************


            //mensajes Histogramas color
            hv_texto = "TEXTURA_Histo";
            for (hv_Index1 = 0; (int)hv_Index1 <= (int)(100 - 1); hv_Index1 = (int)hv_Index1 + 1)
            {
                hv_textof = (hv_texto + "_") + hv_Index1;
                HOperatorSet.SetMessageTuple(hv_ParametrosCola, hv_textof, 0);
            }

            //**************************************************************************************************
            //**************************************************************************************************


            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_pertenencia_1", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_pertenencia_2", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_pertenencia_3", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_pertenencia_4", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_pertenencia_5", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada_1", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada_2", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada_3", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada_4", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada_5", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada_ponderada",
                0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_asignada", 0);
            HOperatorSet.SetMessageTuple(hv_ParametrosCola, "Categoria_Forzada", 0);




            return;
        }

        public void ProcessHdev(HObject ho_ImagesSample, HTuple hv_Features, HTuple hv_ClassLUTHandle)
        {
          // Local iconic variables 

            HObject ho_Image1RGB, ho_Image1NIR;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image1RGB);
            HOperatorSet.GenEmptyObj(out ho_Image1NIR);
            try
            {
                //Extraer características
                //- ImagesSample:
                // Image1RGB
                //
                ho_Image1RGB.Dispose();
                HOperatorSet.SelectObj(ho_ImagesSample, out ho_Image1RGB, 1);
                ho_Image1NIR.Dispose();
                HOperatorSet.SelectObj(ho_ImagesSample, out ho_Image1NIR, 2);
                optimar_extraer_caracteristicas_Peces_null(hv_Features);
                optimar_extraer_caracteristicas_Peces(ho_Image1RGB, ho_Image1NIR, hv_ClassLUTHandle,hv_Features);

                ho_Image1RGB.Dispose();
                ho_Image1NIR.Dispose();

                return;

            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image1RGB.Dispose();
                ho_Image1NIR.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void RC_optimar_contorno_imagen_sinfondo(HObject ho_ImagenEntradaRGB, HObject ho_ImagenEntradaNIR,
      out HObject ho_Mascara, out HObject ho_Contorno, out HObject ho_ImagenSinFondoRGB,
      out HObject ho_ImagenSinFondoNIR, HTuple hv_SinFondo, out HTuple hv_iniPez,
      out HTuple hv_finPez, out HTuple hv_Largo, out HTuple hv_Ancho, out HTuple hv_Area,
      out HTuple hv_Fila1, out HTuple hv_Fila2)
        {


            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ImagenNIR = null, ho_ImagenRGB = null;
            HObject ho_pezregion = null, ho_ContornoSuavizado = null, ho_MascaraSuavizada = null;
            HObject ho_Mascarabin = null, ho_Region1 = null;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_numContornos = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Row3 = new HTuple();
            HTuple hv_Column3 = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_margen = new HTuple(), hv_Exception = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Mascara);
            HOperatorSet.GenEmptyObj(out ho_Contorno);
            HOperatorSet.GenEmptyObj(out ho_ImagenSinFondoRGB);
            HOperatorSet.GenEmptyObj(out ho_ImagenSinFondoNIR);
            HOperatorSet.GenEmptyObj(out ho_ImagenNIR);
            HOperatorSet.GenEmptyObj(out ho_ImagenRGB);
            HOperatorSet.GenEmptyObj(out ho_pezregion);
            HOperatorSet.GenEmptyObj(out ho_ContornoSuavizado);
            HOperatorSet.GenEmptyObj(out ho_MascaraSuavizada);
            HOperatorSet.GenEmptyObj(out ho_Mascarabin);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            hv_iniPez = new HTuple();
            hv_finPez = new HTuple();
            hv_Largo = new HTuple();
            hv_Ancho = new HTuple();
            hv_Area = new HTuple();
            hv_Fila1 = new HTuple();
            hv_Fila2 = new HTuple();
            try
            {
                //  Parámetros de entrada:
                //    - ImagenEntrada
                //
                //  Parámetros de salida:
                //    - Mascara
                //    - Contorno
                //    - ContornoSuavizado
                //    - MascaraFilaMin
                //    - MascaraFilaMax
                //    - ImagenSinFondo

                try
                {

                    ho_ImagenNIR.Dispose();
                    ho_ImagenNIR = ho_ImagenEntradaNIR.CopyObj(1, -1);
                    ho_ImagenRGB.Dispose();
                    ho_ImagenRGB = ho_ImagenEntradaRGB.CopyObj(1, -1);

                    hv_Largo = 0;
                    hv_Ancho = 0;
                    hv_iniPez = 0;
                    hv_finPez = 0;
                    ho_ImagenSinFondoRGB.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImagenSinFondoRGB);
                    ho_ImagenSinFondoNIR.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImagenSinFondoNIR);

                    //Ancho y largo de la imagen
                    HOperatorSet.GetImageSize(ho_ImagenEntradaRGB, out hv_Width, out hv_Height);

                    ho_pezregion.Dispose();
                    HOperatorSet.Threshold(ho_ImagenNIR, out ho_pezregion, 1, 255);



                    //sacar contorno
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.Connection(ho_pezregion, out ExpTmpOutVar_0);
                        ho_pezregion.Dispose();
                        ho_pezregion = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.SelectShapeStd(ho_pezregion, out ExpTmpOutVar_0, "max_area",
                            100);
                        ho_pezregion.Dispose();
                        ho_pezregion = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ErosionCircle(ho_pezregion, out ExpTmpOutVar_0, 1.5);
                        ho_pezregion.Dispose();
                        ho_pezregion = ExpTmpOutVar_0;
                    }
                    ho_Contorno.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_pezregion, out ho_Contorno, "border");
                    ho_ContornoSuavizado.Dispose();
                    HOperatorSet.SmoothContoursXld(ho_Contorno, out ho_ContornoSuavizado, 69);
                    ho_MascaraSuavizada.Dispose();
                    HOperatorSet.GenRegionContourXld(ho_ContornoSuavizado, out ho_MascaraSuavizada,
                        "filled");

                    //selecciono el de mayor area
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.SelectShapeStd(ho_MascaraSuavizada, out ExpTmpOutVar_0, "max_area",
                            100);
                        ho_MascaraSuavizada.Dispose();
                        ho_MascaraSuavizada = ExpTmpOutVar_0;
                    }

                    ho_Contorno.Dispose();
                    ho_Contorno = ho_ContornoSuavizado.CopyObj(1, -1);
                    ho_Mascara.Dispose();
                    ho_Mascara = ho_MascaraSuavizada.CopyObj(1, -1);

                    //mirar si hay pez
                    HOperatorSet.GetImageSize(ho_ImagenEntradaRGB, out hv_Width, out hv_Height);

                    //ver si sólo hay un contorno
                    HOperatorSet.CountObj(ho_Contorno, out hv_numContornos);
                    HOperatorSet.SmallestRectangle1(ho_Mascara, out hv_Fila1, out hv_Column1,
                        out hv_Fila2, out hv_Column2);
                    hv_iniPez = 0;
                    if ((int)(new HTuple(hv_Fila2.TupleLess(hv_Height - 20))) != 0)
                    {
                        hv_iniPez = 1;
                    }

                    hv_finPez = 0;
                    if ((int)(new HTuple(hv_Fila1.TupleGreater(20))) != 0)
                    {
                        hv_finPez = 1;
                    }

                    HOperatorSet.AreaCenter(ho_Mascara, out hv_Area, out hv_Row, out hv_Column);
                    HOperatorSet.SmallestRectangle2(ho_Mascara, out hv_Row3, out hv_Column3,
                        out hv_Phi, out hv_Length1, out hv_Length2);
                    hv_Largo = hv_Length1 * 2;
                    hv_Ancho = hv_Length2 * 2;
                    ho_Mascarabin.Dispose();
                    HOperatorSet.RegionToBin(ho_Mascara, out ho_Mascarabin, 255, 0, hv_Width,
                        hv_Height);
                    //Imagen Sin Fondo

                    HOperatorSet.SmallestRectangle1(ho_Mascara, out hv_Row1, out hv_Column1,
                        out hv_Row2, out hv_Column2);
                    ho_ImagenSinFondoRGB.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImagenRGB, ho_Mascara, out ho_ImagenSinFondoRGB
                        );
                    ho_ImagenSinFondoNIR.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImagenNIR, ho_Mascara, out ho_ImagenSinFondoNIR
                        );
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ReduceDomain(ho_Mascarabin, ho_Mascara, out ExpTmpOutVar_0);
                        ho_Mascarabin.Dispose();
                        ho_Mascarabin = ExpTmpOutVar_0;
                    }
                    hv_margen = 0;
                    if ((int)(new HTuple(hv_SinFondo.TupleEqual(1))) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.CropRectangle1(ho_ImagenSinFondoRGB, out ExpTmpOutVar_0, hv_Row1 - hv_margen,
                                hv_Column1 - hv_margen, hv_Row2 + hv_margen, hv_Column2 + hv_margen);
                            ho_ImagenSinFondoRGB.Dispose();
                            ho_ImagenSinFondoRGB = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.CropRectangle1(ho_ImagenSinFondoNIR, out ExpTmpOutVar_0, hv_Row1 - hv_margen,
                                hv_Column1 - hv_margen, hv_Row2 + hv_margen, hv_Column2 + hv_margen);
                            ho_ImagenSinFondoNIR.Dispose();
                            ho_ImagenSinFondoNIR = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.CropRectangle1(ho_Mascarabin, out ExpTmpOutVar_0, hv_Row1 - hv_margen,
                                hv_Column1 - hv_margen, hv_Row2 + hv_margen, hv_Column2 + hv_margen);
                            ho_Mascarabin.Dispose();
                            ho_Mascarabin = ExpTmpOutVar_0;
                        }

                        ho_Region1.Dispose();
                        HOperatorSet.Threshold(ho_Mascarabin, out ho_Region1, 128, 255);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ReduceDomain(ho_ImagenSinFondoRGB, ho_Region1, out ExpTmpOutVar_0
                                );
                            ho_ImagenSinFondoRGB.Dispose();
                            ho_ImagenSinFondoRGB = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ReduceDomain(ho_ImagenSinFondoNIR, ho_Region1, out ExpTmpOutVar_0
                                );
                            ho_ImagenSinFondoNIR.Dispose();
                            ho_ImagenSinFondoNIR = ExpTmpOutVar_0;
                        }
                        ho_Mascara.Dispose();
                        ho_Mascara = ho_Region1.CopyObj(1, -1);

                    }



                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    //No hay imagen
                }


                ho_ImagenNIR.Dispose();
                ho_ImagenRGB.Dispose();
                ho_pezregion.Dispose();
                ho_ContornoSuavizado.Dispose();
                ho_MascaraSuavizada.Dispose();
                ho_Mascarabin.Dispose();
                ho_Region1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImagenNIR.Dispose();
                ho_ImagenRGB.Dispose();
                ho_pezregion.Dispose();
                ho_ContornoSuavizado.Dispose();
                ho_MascaraSuavizada.Dispose();
                ho_Mascarabin.Dispose();
                ho_Region1.Dispose();

                throw HDevExpDefaultException;
            }
        }
        public void Segmentation(HObject ho_Images, out HObject ho_Regions1, out HObject ho_Regions2,
      HTuple hv_tipo_producto)
        {




            // Local iconic variables 

            HObject ho_Image1RGB, ho_Image1NIR, ho_Mascara;
            HObject ho_Contorno, ho_ImagenSinFondoRGB, ho_ImagenSinFondoNIR;

            // Local control variables 

            HTuple hv_iniPez = null, hv_finPez = null;
            HTuple hv_Largo = null, hv_Ancho = null, hv_Area = null;
            HTuple hv_Fila1 = null, hv_Fila2 = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regions1);
            HOperatorSet.GenEmptyObj(out ho_Regions2);
            HOperatorSet.GenEmptyObj(out ho_Image1RGB);
            HOperatorSet.GenEmptyObj(out ho_Image1NIR);
            HOperatorSet.GenEmptyObj(out ho_Mascara);
            HOperatorSet.GenEmptyObj(out ho_Contorno);
            HOperatorSet.GenEmptyObj(out ho_ImagenSinFondoRGB);
            HOperatorSet.GenEmptyObj(out ho_ImagenSinFondoNIR);
            try
            {



                ho_Image1RGB.Dispose();
                HOperatorSet.SelectObj(ho_Images, out ho_Image1RGB, 1);
                ho_Image1NIR.Dispose();
                HOperatorSet.SelectObj(ho_Images, out ho_Image1NIR, 2);

                ho_Mascara.Dispose(); ho_Contorno.Dispose(); ho_ImagenSinFondoRGB.Dispose(); ho_ImagenSinFondoNIR.Dispose();
                RC_optimar_contorno_imagen_sinfondo(ho_Image1RGB, ho_Image1NIR, out ho_Mascara,
                    out ho_Contorno, out ho_ImagenSinFondoRGB, out ho_ImagenSinFondoNIR, hv_tipo_producto,
                    out hv_iniPez, out hv_finPez, out hv_Largo, out hv_Ancho, out hv_Area,
                    out hv_Fila1, out hv_Fila2);


                ho_Regions1.Dispose();
                ho_Regions1 = ho_Mascara.CopyObj(1, -1);
                ho_Regions2.Dispose();
                ho_Regions2 = ho_Mascara.CopyObj(1, -1);

                ho_Image1RGB.Dispose();
                ho_Image1NIR.Dispose();
                ho_Mascara.Dispose();
                ho_Contorno.Dispose();
                ho_ImagenSinFondoRGB.Dispose();
                ho_ImagenSinFondoNIR.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image1RGB.Dispose();
                ho_Image1NIR.Dispose();
                ho_Mascara.Dispose();
                ho_Contorno.Dispose();
                ho_ImagenSinFondoRGB.Dispose();
                ho_ImagenSinFondoNIR.Dispose();

                throw HDevExpDefaultException;
            }
        }

        //************************************************************************************************************//

    }

}
