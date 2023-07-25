using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TwinCAT.Ads;
using System.IO;
using System.Timers;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml;

namespace TwincatModule
{
    public class TwinCatCommunication
    {
        ///////////////////////////////////////////////////////////////////////////////
        private csr.modules.CSRModule csrMod;
        public TcAdsClient tcAds;
        private TwincatConfig tcConfig;
        private string      m_filename = "conf\\IO\\config.xml";
        private string      m_filenamefilter = "conf\\IO\\confiFilter.xml";
        private string      net_id;
        private int         port;
        public Dictionary<int, TCBlock> m_TCbloques;
        public Dictionary<string, int> m_BlockMap;

        private int varMaxPorBloque = 1000;
        
        //TODO: Sin uso, se podria evitar mensajes de inicio con esto? 
        private Dictionary<string, bool> initialized;

        public delegate void TwincatComErrorHandler(object sender, string var_name, string function_name);
        public event TwincatComErrorHandler OnReadError;
        public event TwincatComErrorHandler OnWriteError;

        private List<IO_Parameter_Filter> listIO_filter;

        //public event EventHandler IOValuesUpdated;
        ///////////////////////////////////////////////////////////////////////////////
        public TwinCatCommunication(csr.modules.CSRModule parent)
        {
            tcAds = new TcAdsClient();
            initialized = new Dictionary<string, bool>();
            m_TCbloques = new Dictionary<int,TCBlock>();
            m_BlockMap = new Dictionary<string, int>();
            csrMod = parent;
        }

        public void reconnect()
        {
            tcAds.Disconnect();
            if (tcAds != null)
                tcAds.Dispose();
            tcAds = new TcAdsClient();
           
        }
        ///////////////////////////////////////////////////////////////////////////////

        public IO_Parameters IOParameters
        {
            get {
                IO_Parameters result = new IO_Parameters();
                foreach (TCBlock bl in m_TCbloques.Values)
                {
                    result.AddIOParameters(bl.IOParameters);
                }
                return result;
                }
        }

        ///////////////////////////////////////////////////////////////////////////////
        public bool Connect(string net_id, int port)
        {
            try
            {
                
                tcAds.Connect(net_id, port);
                this.net_id     = net_id;
                this.port       = port;
                tcAds.AdsNotificationError += new AdsNotificationErrorEventHandler(adsClient_AdsNotificationError);
           
                return true;
            }
            catch (Exception ex)
            {
                string textoout = ex.ToString();
                return false;
            }
        }

       
        ///////////////////////////////////////////////////////////////////////////////
        public bool ConnectBlocks(string net_id, int port)
        {
           
            foreach (TCBlock tcb in m_TCbloques.Values){
                try{
                   
                    tcb.Connect(net_id, port);
                }
                catch{
                    return false;
                }
            } 
            return true;
        }


      
        ///////////////////////////////////////////////////////////////////////////////
        public bool DisconnectBlocks()
        {

            foreach (TCBlock tcb in m_TCbloques.Values)
            {
                try
                {
                    tcb.Disconnect();
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        ///////////////////////////////////////////////////////////////////////////////
        public void InitConfig()
        {
            tcConfig = new TwincatConfig(tcAds);

            //Cargar datos desde fichero o desde el PLC
            if (!LoadIOConf())
            {
                int cantBloques = (tcConfig.Variables.Keys.Count + varMaxPorBloque - 1) / varMaxPorBloque;

                for (int i = 0; i < cantBloques; i++)
                {
                    Dictionary<string, TwincatVariable> aux = subDict(tcConfig.Variables, i * varMaxPorBloque, varMaxPorBloque);
                    m_TCbloques.Add(i, new TCBlock(i, aux, m_filename, this));
                    m_TCbloques[i].SaveIOConf();
                    m_TCbloques[i].SetWriteDelegate(WriteTwincatValue);
                }

                if (!CreateIOConf())
                    csrMod.Error("Error al crear el archivo de configuración de las variables compartidas. Avisad a Roboconcept.", true, false);
            }
            else
            {

                AsociarRedirecciones();
                if (!LeerBloques())
                    csrMod.Error("Error al cargar los bloques de variables compartidas", true, false);
                foreach (TCBlock tcb in m_TCbloques.Values)
                {
                    tcb.SetWriteDelegate(WriteTwincatValue);
                }
            }

            //Conectar los diferentes bloques al ADS
            ConnectBlocks(tcAds.ServerNetID, tcAds.ServerPort);
            //Ver que variable se almacena en cada bloque y añadir las redirecciones
            int j = 0;
            foreach (TCBlock tcb in m_TCbloques.Values)
            {
                //Mapeo e variables
                foreach (IO_Parameter io in tcb.ListaCompleta)
                {
                    m_BlockMap.Add(io.Name, j);
                }

                //Redirecciones
                tcb.AñadirRedireccion();
                j++;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        public void ReConfig()
        {
          

        }

        ///////////////////////////////////////////////////////////////////////////

        private bool LeerBloques()
        {
            try
            {
                string path = Path.GetDirectoryName(m_filename) + "\\";
                string filenameWithoutExt = Path.GetFileNameWithoutExtension(m_filename);
                string filenameExt = Path.GetExtension(m_filename);
                string fich = filenameWithoutExt + "*" + filenameExt;

                string[] filenameBloques = Directory.GetFiles(path, fich).Select(path1 => Path.GetFileName(path1)).Where(val => val != Path.GetFileName(m_filename)).ToArray();

                foreach (string filenameBloque in filenameBloques)
                {
                    TCBlock block = new TCBlock(Convert.ToInt32(filenameBloque.Replace(filenameWithoutExt, "").Replace(filenameExt, "")), m_filename, this);
                    m_TCbloques.Add(block.Id, block);
                }

                return true;
            }catch (Exception e)
            {
                string textoout = e.ToString();
                return false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////

        private bool AsociarRedirecciones()
        {
            try
            {
                XmlDocument xmlRedirecciones = new XmlDocument();

                //La ruta del documento XML permite rutas relativas 
                //respecto del ejecutable!
                xmlRedirecciones.Load(m_filename);

                string[] filenameBloques = Directory.GetFiles("conf\\IO\\", "config*.xml").Select(path => Path.GetFileName(path)).Where(val => val != Path.GetFileName(m_filename)).ToArray();

                //Leer Las listas In y Out
                XmlNodeList listParam = xmlRedirecciones.GetElementsByTagName("ArrayOfIO_Parameter");
                foreach (XmlElement param in listParam)
                {
                    //Leer la lista de Parametros de cada lista
                    XmlNodeList lista = param.GetElementsByTagName("IO_Parameter");
                    foreach (XmlElement nodo in lista)
                    {
                        //Si no tiene redirecciones, pasar al siguiente
                        if (!nodo.LastChild.HasChildNodes)
                            continue;
                        
                        //Si tiene redirecciones hay que buscar en todos los bloques dicho Parametro
                        foreach (string filenameBloque in filenameBloques)
                        {
                            XmlDocument xmlBloque = new XmlDocument();
                            xmlBloque.Load("conf\\IO\\" + filenameBloque);

                            //Leer todos los nombres de variables
                            XmlNodeList listParamBloque = xmlBloque.GetElementsByTagName("Name");
                            foreach (XmlElement nombre in listParamBloque)
                            {
                                //Si el nombre de variable coincide con el Parametro
                                if (nombre.FirstChild.Value == nodo.FirstChild.FirstChild.Value)
                                {
                                    //Recorre todas las redirecciones del Parametro a la variable
                                    foreach(XmlNode redireParam in nodo.LastChild.ChildNodes)
                                    {
                                        //Pero primero hay que verificar que no exista en la variable
                                        bool found = false;
                                        foreach (XmlNode redireVar in nombre.ParentNode.LastChild.ChildNodes)
                                        {
                                            if (redireParam.FirstChild.Value == redireVar.FirstChild.Value)
                                            {
                                                found = true;
                                                break;
                                            }  
                                        }
                                        if (!found)
                                        {
                                            XmlNode importNode = nombre.ParentNode.LastChild.OwnerDocument.ImportNode(redireParam, true);
                                            nombre.ParentNode.LastChild.AppendChild(importNode);
                                        }
                                    }
                                }
                            }
                            xmlBloque.Save("conf\\IO\\" + filenameBloque);
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                string textoout = e.ToString();
                csrMod.Error("Archivo de variables compartidas corrupto. Avisad a Roboconcept.", true, false);
                return false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////

        private bool CreateIOConf()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<List<IO_Parameter>>));

                List<List<IO_Parameter>> listIO = new List<List<IO_Parameter>>();
                List<IO_Parameter> listIn = new List<IO_Parameter>();
                List<IO_Parameter> listOut = new List<IO_Parameter>();

                foreach (IO_Parameter IO in IOParameters.In.Values)
                    listIn.Add(IO);

                foreach (IO_Parameter IO in IOParameters.Out.Values)
                    listOut.Add(IO);

                listIO.Add(listIn);
                listIO.Add(listOut);

                FileStream filestream = new FileStream(this.m_filename, FileMode.Create);

                try
                {
                    serializer.Serialize(filestream, listIO);
                }
                catch (Exception e1)
                {
                    filestream.Close();
                    throw (e1);
                }

                filestream.Close();

                return true;
            }
            catch (Exception e)
            {
                string textoout = e.ToString();
                return false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////

        private List<IO_Parameter> getAllIOParametes()
        {
            List<IO_Parameter> result = new List<IO_Parameter>();
            foreach (TCBlock tcb in m_TCbloques.Values)
                result = result.Concat(tcb.ListaCompleta).ToList();
            return result;
        }

        ///////////////////////////////////////////////////////////////////////////

        private Dictionary<string, TwincatVariable> subDict(Dictionary<string, TwincatVariable> dict, int indIni, int length)
        {
            Dictionary<string, TwincatVariable> result = new Dictionary<string, TwincatVariable>();

            List<string> keys = dict.Keys.Skip(indIni).Take(length).ToList();
            List<TwincatVariable> values = dict.Values.Skip(indIni).Take(length).ToList();

            result = keys.Zip(values, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);

            return result;
        }

        ////////////////////////////////////////////////////////////////////////////////
        private bool LoadIOConf()
        {
            //carga el filtro
            IO_Parameter_Filter IO_flters   = new IO_Parameter_Filter();
            XmlSerializer serializerFilter  = new XmlSerializer(typeof(List<IO_Parameter_Filter>));
            FileStream filestreamFilter;
                
            try
            {
                filestreamFilter = new FileStream(this.m_filenamefilter, FileMode.Open);
                listIO_filter = (List<IO_Parameter_Filter>)serializerFilter.Deserialize(filestreamFilter);
                filestreamFilter.Close();
            }
            catch (Exception e1)
            {
                string textoOuterror = e1.ToString();
                //no existe el fichero genera unos con todas las variables
                filestreamFilter = new FileStream(this.m_filenamefilter, FileMode.Create);
                listIO_filter = new List<IO_Parameter_Filter>();
                IO_Parameter_Filter IO_filter = new IO_Parameter_Filter();
                IO_filter.Name.Add("CSR");
                //IO_filter.Redirections.Add("modulo");
                listIO_filter.Add(IO_filter);
                serializerFilter.Serialize(filestreamFilter, listIO_filter);
                filestreamFilter.Close();
                // listIO_filter.Clear();
            }

            if (Directory.GetFiles("conf\\IO\\", "config.xml").Select(path => Path.GetFileName(path)).ToArray().Length > 0)
                return true;

 
            return false;
        }

        public bool cumpleFiltrado(ref IO_Parameter ioParam)
        {
            bool cumple = false;
            foreach (IO_Parameter_Filter IOFilter in listIO_filter)
            {
                bool Existe_condicion = false;
                string expresion = ioParam.Name;
                for (int i = 0; i < IOFilter.Name.Count; i++)
                {
                    MatchCollection resultado;

                    string expresion_regular = IOFilter.Name[i];
                    resultado = Regex.Matches(expresion, expresion_regular);
                    if (resultado.Count > 0)
                        Existe_condicion = true;
                    else
                    {
                        Existe_condicion = false;
                        break;
                    }
                }

                if (Existe_condicion)
                {
                    int count = IOFilter.Redirections.Count;
                    for (int i = 0; i < count; i++)
                        ioParam.Redirections.Add(IOFilter.Redirections[i]);
                    cumple = true;
                    break;
                }
            }
            return cumple;
        }


        ///////////////////////////////////////////////////////////////////////////
        //public void Save()
        //{
        //    this.SaveIOConf(this.IOparameters);
        //}
        
        ///////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////

        private void WriteTwincatValue(string name, object value)
        {
            try
            {
                if (this.Config.Variables.ContainsKey(name))
                {
                    //Ugaitz: Añadido para filtrar arrays
                    if (!this.Config.Variables[name].Symbol.IsArray)
                        //
                        this.tcAds.WriteSymbol(Config.Variables[name].Symbol, value);
                }
            }
            catch (Exception err)
            {
                if (err.Message == "Cannot convert object to symbol type.")
                {
                    //TODO: Lanzar alarma
                    throw (new Exception("La variable compartida" + name + " no tiene el mismo tipo que el enviado. (Guardad el texto del error y avisad a Roboconcept)"));
                }
                else
                    throw (err);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////////////

        public void ForceReadTwincatValues()
        {
            try
            {
                Dictionary<string, object> dict = ReadValues();

                foreach (KeyValuePair<string, object> kvpair in dict)
                    m_TCbloques[m_BlockMap[kvpair.Key]].IOParameters.Set(kvpair.Key, kvpair.Value);
            }
            catch
            {
                CSRMod.Error("Error al forzar la lectura de variables compartidas. Avisad a Roboconcept", true, false);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        public TwincatConfig Config { get { return tcConfig; } }

        ///////////////////////////////////////////////////////////////////////////////
        public bool Disconnect()
        {
            try
            {
                DisconnectBlocks();
                tcAds.Dispose();
                return true;
            }
            catch
            {
                return false;
            }
        }



        ///////////////////////////////////////////////////////////////////////////////
        private void adsClient_AdsNotificationError(object sender, AdsNotificationErrorEventArgs e)
        {

        }

        ///////////////////////////////////////////////////////////////////////////////
        private void adsClient_AdsStateChanged(object sender, AdsStateChangedEventArgs e)
        {

        }
        ///////////////////////////////////////////////////////////////////////////////
        private void adsClient_NotificaitonEx(object sender, AdsNotificationExEventArgs e)
        {

        }
        ///////////////////////////////////////////////////////////////////////////////
        private void adsClient_Notificaiton(object sender, AdsNotificationEventArgs e)
        {

        }
        /// <summary>
        /// detection of the notification when the router is off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        ///////////////////////////////////////////////////////////////////////////////
        private void amsRouter_Notificaiton(object sender, AmsRouterNotificationEventArgs e)
        {
          
        }

        

        private Dictionary<string, object> ReadValues()
        {
            try
            {
                DateTime t1 = DateTime.Now;
                Dictionary<string, object> values = new Dictionary<string, object>();
                foreach (TwincatVariable variable in this.Config.Variables.Values)
                    values.Add(variable.Symbol.Name, this.tcAds.ReadSymbol(variable.Symbol));
                    
                DateTime t2 = DateTime.Now;
                values.Add("tElapsed: ", (t2 - t1).TotalMilliseconds.ToString());
                return values;
            }
            catch (Exception e)
            {
                string textoOuterror = e.ToString();
                return null;
            }
        }

        public csr.modules.CSRModule CSRMod
        {
            get { return csrMod; }
            set { csrMod = value; }
        }
    }
}



