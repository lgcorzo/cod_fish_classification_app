using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProcesadoEspecies
{
  

    class wekaClassifier
    {
        public weka.classifiers.Classifier m_clasificador1          = null;
        public weka.classifiers.Classifier m_clasificador2          = null;
        public weka.classifiers.Classifier m_clasificador3          = null;
        public weka.classifiers.Classifier m_clasificador4          = null;
        public weka.classifiers.Classifier m_clasificador5          = null;
        public List<string>                m_classNames             = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trainingDatasetFilePath"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public  bool Entrenarclasificador(string trainingDatasetFilePath, string savePath)
        {
            ////leer archivo ARFF
            //weka.core.Instances trainDataset = new weka.core.Instances(new java.io.FileReader(trainingDatasetFilePath));
            //trainDataset.setClassIndex(trainDataset.numAttributes() - 1);

            ////Nuevo clasificador
            //m_clasificador1 = (weka.classifiers.Classifier)(weka.core.SerializationHelper.read(savePath));

            ////Entrenar clasificador
            //m_clasificador1.buildClassifier(trainDataset);

            ////Guardar el modelo generado
            //weka.core.SerializationHelper.write(savePath, m_clasificador1);

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="openPath"></param>
        /// <returns></returns>
        public  bool CargarClasificador(string openPath)
        {
            bool estado_clasificador = true;
            try
            {
                m_clasificador1 = (weka.classifiers.Classifier)(weka.core.SerializationHelper.read(openPath));
            }
            catch
            {
                m_clasificador1 = null;
                estado_clasificador = false;
            }
            return (estado_clasificador);
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="openPath"></param>
        /// <returns></returns>
        public bool CargarClasificador2(string openPath)
        {
            bool estado_clasificador = true;
            try
            {
                m_clasificador2 = (weka.classifiers.Classifier)(weka.core.SerializationHelper.read(openPath));
            }
            catch
            {
                m_clasificador2 = null;
                estado_clasificador = false;
            }
            return (estado_clasificador);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="openPath"></param>
        /// <returns></returns>
        public bool CargarClasificador3(string openPath)
        {
            bool estado_clasificador = true;
            try
            {
                m_clasificador3 = (weka.classifiers.Classifier)(weka.core.SerializationHelper.read(openPath));
            }
            catch 
            {
                m_clasificador3 = null;
                estado_clasificador = false;
            }
            return (estado_clasificador);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="openPath"></param>
        /// <returns></returns>
        public bool CargarClasificador4(string openPath)
        {
            bool estado_clasificador = true;
            try
            {
                m_clasificador4 = (weka.classifiers.Classifier)(weka.core.SerializationHelper.read(openPath));
            }
            catch 
            {
                m_clasificador4 = null;
                estado_clasificador = false;
            }
            return (estado_clasificador);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="openPath"></param>
        /// <returns></returns>
        public bool CargarClasificador5(string openPath)
        {
            bool estado_clasificador = true;
            try
            {
                m_clasificador5 = (weka.classifiers.Classifier)(weka.core.SerializationHelper.read(openPath));
            }
            catch
            {
                m_clasificador5 = null;
                estado_clasificador = false;
            }
            return (estado_clasificador);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="openPath"></param>
        /// <returns></returns>
        public bool CargarNombreClass(string openPath)
        {
          
            //carga el filtro
            string filenamePath = openPath;
            string filenamefullPath = openPath + "\\classnames.xml";
            List<string> classNames = new List<string>();
            XmlSerializer serializerConf = new XmlSerializer(typeof(List<string>));
            FileStream filestreamFilter;

            if (!Directory.Exists(filenamePath))
                Directory.CreateDirectory(filenamePath);

            try
            {
                filestreamFilter = new FileStream(filenamefullPath, FileMode.Open);
                classNames = (List<string>)serializerConf.Deserialize(filestreamFilter);
                filestreamFilter.Close();
            }
            catch
            {
                //no existe el fichero genera unos con todas las variables
                filestreamFilter = new FileStream(filenamefullPath, FileMode.Create);
                classNames = new List<string>();
                classNames.Add("0-Unknown");
                classNames.Add("1-One");
                classNames.Add("2-Two");
                serializerConf.Serialize(filestreamFilter, classNames);
                filestreamFilter.Close();
            }

            m_classNames = classNames;

            if (Directory.GetFiles(filenamePath, "classnames.xml").Select(path => Path.GetFileName(path)).ToArray().Length > 0)
                return true;

            return false;

        }
        /// <summary>
        /// https://weka.wikispaces.com/Use+Weka+in+your+Java+code
        /// </summary>
        /// <param name="intancia"></param>
        /// <returns></returns>
        public double Clasifica1Instacia(weka.core.Instance currentInst,double limite_pertenencia, out double pertenencia)
        {
            double predictedClass = 0;
            double[] predictedClassprob;
            pertenencia = 0.0;


            try
            {
                if (m_clasificador1 != null)
                {
                    predictedClass = m_clasificador1.classifyInstance(currentInst);
                    predictedClassprob = m_clasificador1.distributionForInstance(currentInst);
                    if ((predictedClassprob[(int)predictedClass]) * 100.0 < limite_pertenencia)
                        predictedClass++;
                 
                    if (predictedClass > predictedClassprob.Length)
                        predictedClass = predictedClassprob.Length;

                    pertenencia = predictedClassprob[(int)predictedClass];

                }        
     
            }
            catch(Exception ex)
            {
                string textoOuterror = ex.ToString();
                predictedClass = -2;
               // textout = e.ToString();
            }

        

            return (predictedClass);
        }

        /// <summary>
        /// https://weka.wikispaces.com/Use+Weka+in+your+Java+code
        /// </summary>
        /// <param name="intancia"></param>
        /// <returns></returns>
        public double Clasifica2Instacia(weka.core.Instance currentInst, double limite_pertenencia, out double pertenencia)
        {
            double predictedClass = 0;
            double[] predictedClassprob;
            pertenencia = 0.0;
            try
            {
                if (m_clasificador2 != null)
                {
                    predictedClass = m_clasificador2.classifyInstance(currentInst);
                    predictedClassprob = m_clasificador2.distributionForInstance(currentInst);
                   
                    if (predictedClass > predictedClassprob.Length)
                        predictedClass = predictedClassprob.Length;

                    pertenencia = predictedClassprob[(int)predictedClass];
                }

                //textout = currentInst.classAttribute().value((int)predictedClass);

            }
            catch 
            {
                predictedClass = -2;
               // textout = e.ToString();
            }
            return (predictedClass);
        }

        /// <summary>
        /// https://weka.wikispaces.com/Use+Weka+in+your+Java+code
        /// </summary>
        /// <param name="intancia"></param>
        /// <returns></returns>
        public double Clasifica3Instacia(weka.core.Instance currentInst, double limite_pertenencia, out double pertenencia)
        {
            double predictedClass = 0;
            double[] predictedClassprob;
            pertenencia = 0.0;
            try
            {
                if (m_clasificador3 != null)
                {
                    predictedClass = m_clasificador3.classifyInstance(currentInst);
                    predictedClassprob = m_clasificador3.distributionForInstance(currentInst);

                    if (predictedClass > predictedClassprob.Length)
                        predictedClass = predictedClassprob.Length;

                    pertenencia = predictedClassprob[(int)predictedClass];
                }

                //textout = currentInst.classAttribute().value((int)predictedClass);

            }
            catch 
            {
                predictedClass = -2;
                // textout = e.ToString();
            }

            return (predictedClass);
        }

        /// <summary>
        /// https://weka.wikispaces.com/Use+Weka+in+your+Java+code
        /// </summary>
        /// <param name="intancia"></param>
        /// <returns></returns>
        public double Clasifica4Instacia(weka.core.Instance currentInst, double limite_pertenencia, out double pertenencia)
        {
            double predictedClass = 0;
            double[] predictedClassprob;
            pertenencia = 0.0;
            try
            {
                if (m_clasificador4 != null)
                {
                    predictedClass = m_clasificador4.classifyInstance(currentInst);
                    predictedClassprob = m_clasificador4.distributionForInstance(currentInst);

                    if (predictedClass > predictedClassprob.Length)
                        predictedClass = predictedClassprob.Length;

                    pertenencia = predictedClassprob[(int)predictedClass];
                }

                //textout = currentInst.classAttribute().value((int)predictedClass);

            }
            catch
            {
                predictedClass = -2;
                // textout = e.ToString();
            }

            return (predictedClass);
        }

        /// <summary>
        /// https://weka.wikispaces.com/Use+Weka+in+your+Java+code
        /// </summary>
        /// <param name="intancia"></param>
        /// <returns></returns>
        public double Clasifica5Instacia(weka.core.Instance currentInst, double limite_pertenencia, out double pertenencia)
        {
            double predictedClass = 0;
            double[] predictedClassprob;
            pertenencia = 0.0;

            try
            {
                if (m_clasificador5 != null)
                {
                    predictedClass = m_clasificador5.classifyInstance(currentInst);
                    predictedClassprob = m_clasificador5.distributionForInstance(currentInst);
                    if ((predictedClassprob[(int)predictedClass]) * 100.0 < limite_pertenencia)
                        predictedClass++;

                    if (predictedClass > predictedClassprob.Length)
                        predictedClass = predictedClassprob.Length;

                    pertenencia = predictedClassprob[(int)predictedClass];

                }

            }
            catch
            {
                predictedClass = -2;
                // textout = e.ToString();
            }



            return (predictedClass);
        }

        ///////////////////////////////////////////////////

    }
}
