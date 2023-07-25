using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ProcesadoEspecies
{
    public class Features
    {
        private Dictionary<string, Feature> data;
        private static DataContractSerializer data_contract_serializer = new DataContractSerializer(typeof(List<Feature>));
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, Feature> Data
        {
            get { return this.data; }
            set { this.data = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Features()
        {
            this.data = new Dictionary<string, Feature>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="feature_list"></param>
        public Features(List<Feature> feature_list)
        {
            this.data = new Dictionary<string, Feature>();
            foreach (Feature feature in feature_list)
            {
                this.data.Add(feature.Name, feature);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Feature> ToList()
        {
            return this.data.Values.ToList();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Feature> GetSelected()
        {
            List<Feature> selected = new List<Feature>();
            foreach (Feature feature in this.data.Values)
            {
                if (feature.IsSelected())
                    selected.Add(feature);
            }

            return selected;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="namesToSelect"></param>
        public void Select(List<string> namesToSelect)
        {
            foreach (Feature feature in this.data.Values)
            {
                feature.Select(false);
            }
            foreach (string name in namesToSelect)
            {
                this.data[name].Select(true);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="featuresTemplate"></param>
        public void Select(List<Feature> featuresTemplate)
        {
            foreach (Feature feature in featuresTemplate)
            {
                if (this.data.ContainsKey(feature.Name))
                {
                    if (feature.IsSelected())
                        this.data[feature.Name].Select(true);
                    else
                        this.data[feature.Name].Select(false);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="features"></param>
        /// <returns></returns>
        public static bool LoadFromFile(string filename, out Features features)
        {
            try
            {
                FileStream filestream = new FileStream(filename, FileMode.Open);
                List<Feature> feature_list = (List<Feature>)data_contract_serializer.ReadObject(filestream);
                features = new Features(feature_list);
                filestream.Close();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("There was an error parsing " + filename + " file. [" + e.Message + "]");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="features"></param>
        /// <returns></returns>
        public static bool StoreToFile(string filename, Features features)
        {
            try
            {
                FileStream filestream = new FileStream(filename, FileMode.Create);
                XmlTextWriter xml_text_writer = new XmlTextWriter(filestream, System.Text.Encoding.ASCII);
                xml_text_writer.Formatting = Formatting.Indented;
                data_contract_serializer.WriteObject(xml_text_writer, features.ToList());
                xml_text_writer.Flush();
                filestream.Flush();
                xml_text_writer.Close();
                filestream.Close();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("There was an error storing " + filename + " file. [" + e.Message + "]");
            }
        }
        /// <summary>
        /// Funcion para generar la instancia para el clasificador weka 
        /// https://weka.wikispaces.com/Creating+an+ARFF+file
        /// </summary>
        /// <param name="features"></param>
        /// <returns></returns>
        public static weka.core.Instance GenerarInstancia(Features features, List<string> labelsCategoriaForzada)
        {
           
            int contador                        = 0;
            int class_index                     = 0;
            List<Feature> SelectedFeatures      = features.GetSelected();
            int numero_elementos                = SelectedFeatures.Count();

            //Add attributes         
            java.util.ArrayList attributes = new java.util.ArrayList();


            foreach (Feature feature in SelectedFeatures)
            {
                if (feature.Name != "Categoria_Forzada")
                {
                    weka.core.Attribute attr = new weka.core.Attribute(feature.Name);
                    attributes.add(attr);
                }
                else
                {
                    //weka.core.FastVector labels = new weka.core.FastVector();
                    java.util.ArrayList labels = new java.util.ArrayList();
                    for(int index = 0; index < labelsCategoriaForzada.Count(); index++)
                        labels.add(labelsCategoriaForzada[index]);                 
                    weka.core.Attribute attr = new weka.core.Attribute(feature.Name, labels);
                    attributes.add(attr);
                    class_index = contador;
                }
                
                // el ultimo elementos es la clase de decision
                contador++;
            }

            weka.core.Instances dataset = new weka.core.Instances("TestInstances", attributes, 0);
            //LO NUEVO (2)
            // Assign the prediction attribute to the dataset. This attribute will
            // be used to make a prediction.
            dataset.setClassIndex(class_index );

            weka.core.Instance currentInst = new weka.core.DenseInstance(numero_elementos);
            contador = 0;
            foreach (Feature feature in SelectedFeatures)
            {
                currentInst.setValue(contador, feature.Value);
                if (contador == numero_elementos - 1)
                    break;
                contador++;
            }
            //LO NUEVO (1)
            currentInst.setDataset(dataset);
            //filter de instance to eliminate attributes
          


            return currentInst;
        }
        /// <summary>
        /// adaptarlo para escribir arff
        /// https://weka.wikispaces.com/ARFF
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="features"></param>
        /// <returns></returns>
        public static bool StoreToFileArff(string filename, Features features, List<string> labelsCategoriaForzada)
        {
            if (features.Data.Count > 10) //solo se escribe si hay datos
            {
                try
                {
                //escribo la cabecera
                if (!File.Exists(filename))
                {
                    string textocategorias = "{";
                    foreach (string text in labelsCategoriaForzada)
                    {
                        textocategorias += text + ",";
                    }
                    textocategorias += "}";

                    string cabecera = "@RELATION Categoria_Forzada \n";
                    foreach (Feature feature in features.Data.Values)
                    {
                        if (feature.Name != "Categoria_Forzada")
                            cabecera += "@ATTRIBUTE " + feature.Name + " NUMERIC" + " \n";
                        else
                            cabecera += "@ATTRIBUTE " + feature.Name + textocategorias + " \n";


                    }
                    cabecera += "@DATA" + "\n\r";

                    System.IO.File.WriteAllText(filename, cabecera);

                }
                //escribe los datos
                string line = "";
                long longvalor = 0;
                double dvalor = 0.0;
                string textprint = "";
                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(filename, true))
                {

                    foreach (Feature feature in features.Data.Values)
                    {
                        if (features.Data.Count > 10) //solo se escribe si hay datos
                        {
                            if(feature.Name == "ID")
                                {
                                  longvalor = (long) feature.lValue;
                                    textprint = longvalor.ToString("G").Replace(",", ".");
                                }
                                   
                                else                   
                                {
                                     dvalor = feature.Value;
                                    textprint = dvalor.ToString("G").Replace(",", ".");
                                }

                                   
                                


                            if (feature.Name == "Categoria_Forzada")
                            {
                                int indice = (int)dvalor;
                                    textprint = labelsCategoriaForzada[indice];

                            }

                            line += textprint + ",";
                        }


                    }
                    line += "\n";


                    file.WriteLine(line);
                }


                return true;
            }
            catch (Exception e)
            {
                throw new Exception("There was an error storing " + filename + " file. [" + e.Message + "]");
            }
         }
            return true;
        }

        /// <summary>
        /// adaptarlo para escribir arff
        /// https://weka.wikispaces.com/ARFF
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="features"></param>
        /// <returns></returns>
        public static bool LoadArff(string filename, out List<Features> Instancias)
        {
           
            Instancias = new List<Features>();
            java.io.File file = new java.io.File(filename);
            weka.core.Instance currentInstance; 
            try
            {
                //escribo la cabecera
                if (File.Exists(filename))
                {
                    weka.core.converters.ArffLoader arffloader = new weka.core.converters.ArffLoader();
                    arffloader.setFile(file);
                    weka.core.Instances wekainstancesstructure = arffloader.getStructure();
                    weka.core.Instances intanciasWeka = arffloader.getDataSet();
                    //currentInstance = arffloader.getNextInstance(wekainstancesstructure);
                    int numintances = intanciasWeka.numInstances();
                    for (int j = 0; j < numintances; j++)
                    {
                        Features Instancia = new Features();
                        currentInstance = intanciasWeka.instance(j);
                        int count = currentInstance.numAttributes();
                        for ( int i = 0; i < count; i++)
                        {
                            weka.core.Attribute atributo = currentInstance.attribute(i);
                            string name = atributo.name();
                            int index = atributo.index();                          
                            int type = atributo.type();
                           
                            if(type == 0)
                            {
                                double value = currentInstance.value(index);
                                long lvalue = 0;
                                if(name == "ID")
                                    lvalue = (long)currentInstance.value(index);

                                Instancia.addFeature(name, value, lvalue);
                            }                           
                            else if(type == 1)
                            {
                                string valuename = currentInstance.stringValue(index);
                                string[] namesplit = valuename.Split('_');
                                double valor = Int32.Parse(namesplit[0]);
                                Instancia.addFeature(name, valor);
                            }
                            
                        }
                        Instancias.Add(Instancia);
                    }
                }

                return true;
            }
            catch
            {
                return false;
                //throw new Exception("There was an error loading " + filename + " file. [" + e.Message + "]");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void addFeature(string name, double value, long lvalue = 0)
        {
            addFeature(name, value, lvalue, true);
        }

        public void addFeature(string name, double value,long lvalue, bool selected)
        {
            if (this.data.ContainsKey(name))
            {
                this.Data[name].Name = name;
                this.Data[name].Value = value;
                this.Data[name].lValue = lvalue;
                this.Data[name].Select(selected);
            }
            else
            {
                this.data.Add(name, new Feature(name, value, lvalue, true));
            }
        }

       

        public List<Feature> filterFeatures(List<Feature> filterList)
        {
            List<Feature> filteredFeatureList = new List<Feature>();
            List<string> keysFilter = new List<string>();
            foreach (Feature f in filterList)
            {
                keysFilter.Add(f.Name);
            }

            Dictionary<string, Feature> newData = new Dictionary<string, Feature>(filteredFeatureList.Count);

            foreach (string keyInFilter in keysFilter)
            {
                newData.Add(keyInFilter, this.data[keyInFilter]);
            }

            filteredFeatureList = new List<Feature>(newData.Values);

            return filteredFeatureList;
        }
    }

    public class Feature
    {
        private string name;
        private double value;
        private long lvalue;
        private bool selected;

        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        public long lValue
        {
            get { return this.lvalue; }
            set { this.lvalue = value; }
        }

        public Feature(string _name, double _value, long _lvalue,  bool _selected)
        {
            this.name = _name;
            this.value = _value;
            this.lvalue = _lvalue;
            this.selected = _selected;
        }

    

        public Feature()
        { }

        public bool IsSelected()
        {
            return this.selected;
        }
        public void Select(bool select)
        {
            this.selected = select;
        }
    }
}
