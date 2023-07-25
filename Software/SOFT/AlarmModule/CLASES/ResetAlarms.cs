using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AlarmModule
{
    public class ResetAlarms : BitArray
    {
        private csr.modules.CSRModule csrMod;
        ////////////////////////////////////////////////////////////////////////

        public ResetAlarms(csr.modules.CSRModule csr)
        {
            this.csrMod = csr;
            this.Nombre = "";
            this.Numero = 0;
            this.BitResumen = false;
            //BORRARthis.IO = new IO_Parameters();

            /*this.m_AlarmIds = new Dictionary<string, string>();
            this.m_AlarmDescriptions = new Dictionary<string, string>();

            LoadAlarmDescriptions();*/
        }

        ////////////////////////////////////////////////////////////////////////

        /*BORRARpublic ResetAlarms(int alarma, IO_Parameters io, string name)
        {
            this.Nombre = name;
            this.Numero = alarma;
            this.BitResumen = false;
            this.IO = io;

            /*this.m_AlarmIds = new Dictionary<string, string>();
            this.m_AlarmDescriptions = new Dictionary<string, string>();

            LoadAlarmDescriptions();*/
        //}

        ////////////////////////////////////////////////////////////////////////

        public bool activarReset(int ind)
        {
            return this.activarBit(ind);
        }

        ////////////////////////////////////////////////////////////////////////

        public bool desactivarReset(int ind)
        {
            return this.desactivarBit(ind);
        }

        /////////////////////////

        public bool ResetActivo(int ind)
        {
            return bitActivo(ind);
        }

        /////////////////////////
        /*private void LoadAlarmDescriptions()
        {
            try
            {
                //parent.WriteConsole("Cargando descripciones de alarma...");
           
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strAlarmDescFilename);
                XmlNodeList listModules = xmlDoc.GetElementsByTagName("Alarms");
                XmlNodeList listModule = ((XmlElement)listModules[0]).GetElementsByTagName("Alarm");
                foreach (XmlElement node in listModule)
                {
                    XmlNodeList aBit = node.GetElementsByTagName("Bit");
                    XmlNodeList aID = node.GetElementsByTagName("ID");
                    XmlNodeList aDesc = node.GetElementsByTagName("Description");
                    this.m_AlarmIds.Add(aBit[0].InnerText, aID[0].InnerText);
                    this.m_AlarmDescriptions.Add(aBit[0].InnerText, aDesc[0].InnerText);
                }
                //parent.WriteConsole("Descripciones de alarma cargadas.");
            }
            catch (Exception e)
            {
                //parent.WriteConsole("Descripciones de alarma no cargadas.");
                MessageBox.Show("Ocurrio un error leyendo el fichero de descripciones de alarmas. [" + strAlarmDescFilename + "]", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }*/

        ////////////////////////////////////////////////////////////////////////

        public uint ResetNum
        {
            get { return this.Numero; }
            set { this.Numero = value; }
        }

        ////////////////////////////////////////////////////////////////////////

        /*public bool Descripcion(int bit, out string  value)
        {
            if (this.m_AlarmDescriptions.ContainsKey(bit.ToString())){
                value = this.m_AlarmDescriptions[bit.ToString()];
                return true;
            }
            value = "";
            return false;
        }*/

        ////////////////////////////////////////////////////////////////////////

        /*public bool Id(int bit, out string  value)
        {
            if (this.m_AlarmIds.ContainsKey(bit.ToString())){
                value = this.m_AlarmIds[bit.ToString()];
                return true;
            }
            value = "";
            return false;
        }*/

        ////////////////////////////////////////////////////////////////////////

        /*public bool getBitByID(string id, out int bit)
        {
            if (this.m_AlarmIds.ContainsValue(id))
            {
                bit = Convert.ToInt32(this.m_AlarmIds.Where(x => x.Value == id).First().Key);
                return true;
            }
            bit = -1;
            return false;
        }*/

        ////////////////////////////////////////////////////////////////////////

        /*public bool getBitByDesc(string desc, out int bit)
        {
            if (this.m_AlarmIds.ContainsValue(desc))
            {
                bit = Convert.ToInt32(this.m_AlarmIds.Where(x => x.Value == desc).First().Key);
                return true;
            }
            bit = -1;
            return false;
        }*/
        ////////////////////////////////////////////////////////////////////////

        /*public int Count()
        {
            return m_AlarmDescriptions.Count;
        }*/
    }
}
