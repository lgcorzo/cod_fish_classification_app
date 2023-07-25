using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace AlarmModule
{
    public class Alarms
    {
        private csr.modules.CSRModule csrMod;
        private string strAlarmDescFilename = "Alarmas/alarmas.xml";    //Archivo donde se leen las alarmas y sus descripciones
        private string m_communication = "message";                     //Tipo de comunicacion utilizada en el CSR
        private Dictionary<string, AlarmBlock> m_alarmBlocks;           //La lista de los "Bloques de Alarmas" (1 Bloque = 31 Alarmas) definidos por un texto, por si hay variables que compartir
        //private Dictionary<int, string> communicationNames;             //Linkado de ids con su nombre de variable de comunicacion

        public event EventHandler CambioDeAlarma;

        ////////////////////////////////////////////////////////////////////////
        //Constructor
        public Alarms(csr.modules.CSRModule csr)
        {
            csrMod = csr;
            this.m_alarmBlocks = new Dictionary<string, AlarmBlock>();
            //communicationNames = new Dictionary<int, string>();
            LoadAlarms();
        }

        ////////////////////////////////////////////////////////////////////////
        //Activar la alarma usando el ID
        public bool activarAlarma(string id)
        {
            if (csrMod.Status == csr.modules.CSRModuleStatus.Closing || csrMod.Status == csr.modules.CSRModuleStatus.Closed)
                return false;
            bool solu = false;
            foreach (AlarmBlock ab in this.m_alarmBlocks.Values)
            {
                List<string> ids = new List<string>();
                if (!ab.getIds(out ids) || !ids.Contains(id))
                    continue;
                solu = ab.activarAlarma(id);
                if (CambioDeAlarma != null)
                    CambioDeAlarma(ab.Name, null);
                break;
            }
            return solu;
        }

        ////////////////////////////////////////////////////////////////////////
        //Desactivar la alarma usando el ID
        public bool desactivarAlarma(string id)
        {
            if (csrMod.Status == csr.modules.CSRModuleStatus.Closing || csrMod.Status == csr.modules.CSRModuleStatus.Closed)
                return false;
            bool solu = false;
            foreach (AlarmBlock ab in this.m_alarmBlocks.Values)
            {
                List<string> ids = new List<string>();
                if (!ab.getIds(out ids) || !ids.Contains(id))
                    continue;
                solu = ab.desactivarAlarma(id);
                if (CambioDeAlarma != null)
                    CambioDeAlarma(ab.Name, null);
                break;
            }
            return solu;
        }

        /////////////////////////
        //Ver si la alarma con cierto Id está activa o no
        public bool alarmaActiva(string id)
        {
            bool solu = false;

            foreach (AlarmBlock ab in this.m_alarmBlocks.Values)
            {
                List<string> ids = new List<string>();
                if (!ab.getIds(out ids) || !ids.Contains(id))
                    continue;
                solu = ab.alarmaActiva(id);
                break;
            }
            return solu;
        }

        /////////////////////////
        //Carga de los IDs, bits, descripciones y títulos de las alarmas.
        private void LoadAlarms()
        {
            try
            {
                //parent.WriteConsole("Cargando descripciones de alarma...");

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strAlarmDescFilename);
                XmlNodeList listModules = xmlDoc.GetElementsByTagName("Alarms");
                XmlNodeList listBlocks = ((XmlElement)listModules[0]).GetElementsByTagName("AlarmBlock");

                if (listBlocks.Count != 0)
                {
                    int BlockNum = 0;
                    foreach (XmlElement block in listBlocks)
                    {
                        XmlNodeList listModule = block.GetElementsByTagName("Alarm");
                        if (listModule.Count > 32)
                        {
                            throw new Exception("En el bloque de alarmas " + listBlocks.ToString() + " hay mas de 32 alarmas, modificar el fichero " + strAlarmDescFilename);
                            
                        }

                        AlarmBlock ab;
                        BlockNum++;
                        if (block.Attributes.Count > 0)
                            ab = new AlarmBlock(block.GetAttribute("Id"), false);
                        else
                            ab = new AlarmBlock(BlockNum.ToString(), false);

                        int AlarmNum = 0;
                        foreach (XmlElement node in listModule)
                        {
                            XmlNodeList aID = node.GetElementsByTagName("ID");
                            XmlNodeList aTitulo = node.GetElementsByTagName("Title");
                            XmlNodeList aDesc = node.GetElementsByTagName("Description");

                            ab.añadirAlarma(aID[0].InnerText, AlarmNum++, aTitulo[0].InnerText, aDesc[0].InnerText);
                        }

                        m_alarmBlocks.Add(ab.Name, ab);
                    }
                }



                /*int AlarmNum = 0;
                AlarmBlock ab = new AlarmBlock();
                foreach (XmlElement node in listModule)
                {
                    AlarmNum ++;
                    //XmlNodeList aBit = node.GetElementsByTagName("Bit");
                    
                    if (AlarmNum == 31)
                    {
                        this.m_alarmBlocks.Add(ab);
                        ab = new AlarmBlock();
                        AlarmNum = 0;
                    }
                }

                this.m_alarmBlocks.Add(ab);*/
                //parent.WriteConsole("Descripciones de alarma cargadas.");
            }
            catch (Exception e)
            {
                string text = e.ToString();
                //csrMod.Error("Ocurrio un error leyendo el fichero de descripciones de alarmas. [" + strAlarmDescFilename + "]. " + e.Message, true, false);
            }
        }

        ////////////////////////////////////////////////////////////////////////
        //Getter/setter de la cadena de bits de las alarmas
        /*public List<int> AlarmNum
        {
            get
            {
                List<int> get = new List<int>();
                foreach (AlarmBlock ab in m_alarmBlocks.Values)
                    get.Add(ab.AlarmNum);
                return get; }
            set
            {
                if (value.Count != this.m_alarmBlocks.Count)
                    throw new Exception("El número de bloques de alarmas no concuerda con los existentes.");
                for (int i = 0; i < this.m_alarmBlocks.Count; i++)
                    this.m_alarmBlocks[communicationNames[i]].AlarmNum = value[i];
            }
        }*/

        ////////////////////////////////////////////////////////////////////////
        //Get de la descripcion y titulo dependiendo del ID, si no existe devuelve null
        public bool Descripcion(string id, out DescripcionAlarma value)
        {
            value = null;
            foreach (AlarmBlock ab in this.m_alarmBlocks.Values)
                if (ab.Descripcion(id, out value))
                    break;
            return value != null;
        }

        ////////////////////////////////////////////////////////////////////////
        //Devuelve en el bool si hay algun valor, y en el out la lista de las claves de todos los bloques con todos los ID
        public bool getIds(out List<string> keys)
        {
            keys = new List<string>();
            foreach (AlarmBlock ab in this.m_alarmBlocks.Values)
            {
                List<string> tmp = new List<string>();
                if (ab.getIds(out tmp))
                    keys.AddRange(tmp);
            }

            return (keys.Count > 0);
        }

        ////////////////////////////////////////////////////////////////////////
        //Devuelve en el bool si hay algun valor, y en el out la lista de las claves del bloque con nombre "blockName" con todos sus ID
        public bool getIds(string blockName, out List<string> keys)
        {
            keys = new List<string>();
            return this.m_alarmBlocks[blockName].getIds(out keys);
        }

        ////////////////////////////////////////////////////////////////////////
        //Devuelve la cantidad de alarmas
        public int Count()
        {
            int solu = 0;
            foreach (AlarmBlock ab in this.m_alarmBlocks.Values)
                solu += ab.Count();
            return solu;
        }

        ////////////////////////////////////////////////////////////////////////
        //Define el numero del bloque "blockName"
        public bool setBlock(string blockName, uint num)
        {
            if (!m_alarmBlocks.ContainsKey(blockName))
                return false;
            m_alarmBlocks[blockName].AlarmNum = num;
            if (CambioDeAlarma != null)
                CambioDeAlarma(m_alarmBlocks[blockName].Name, null);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////
        //Getter/setter de la comunicación a usar
        public string Communication
        {
            get { return this.m_communication; }
            set { this.m_communication = value; }
        }
    }
}
