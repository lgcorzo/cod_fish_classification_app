using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Xml.Serialization;
using System.Xml;
using System.ServiceModel;
using System.ServiceModel.Description;
using ServiceLibrary;
using System.IO;

namespace WCFModule
{
    
   
    public partial class WCFForm : Form
    {
        public class WCFComConf
        {
            [XmlElement("Comunication_Params_server")]
            public List<ServerInterfaceString> ServerInterfaceList = new List<ServerInterfaceString>();
            [XmlElement("Comunication_Params_client")]
            public ServerInterfaceString ClientInterface            = null;     
        }

        public class ServerInterfaceString
        {
           
            public string   _ipAddress  ="127.0.0.1";          
            public string   _port       = "8010";         
        }

        private csr.modules.CSRFormModule   parent;
        ModuleDelegates                     moduleDelegates;
        WCFComConf                          CommConf;
        bool                                loaded                  = false;
        public List<ServerInterface>        ServerInterfaceList     = null;
        public ServerInterface              ClientInterface         = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="module_delegates"></param>
        public WCFForm(csr.modules.CSRFormModule _parent, ModuleDelegates module_delegates)
        {
            this.parent             = _parent;
            this.moduleDelegates    = module_delegates;
            InitializeComponent();
            Init();
        }
          
        /// <summary>
        /// 
        /// </summary>
        private void Init()
        {
            LoadIOConf();        
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            
            moduleDelegates.initServer(ServerInterfaceList);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        public void Append(string str)
        {
            if (loaded)
            textBox1.AppendText("\r\n" + str);
        }
        /// <summary>
        /// inicializa el cliente conectandose a la url especificada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            moduleDelegates.initClient(ClientInterface);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            moduleDelegates.stopServer();
        }

        public delegate void ActualizaPantallaDelegate(string Texto);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="caracteristicas"></param>
        public void ActualizaPantalla(string Text)
        {
            //actualiza el grid
            if (this.InvokeRequired)
            {
                ActualizaPantallaDelegate refrescoDelegate = new ActualizaPantallaDelegate(ActualizaPantalla);
                this.Invoke(refrescoDelegate, new Object[] { Text });
            }
            else //1
            {
                Append(Text);
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool LoadIOConf()
        {
            //carga el filtro
            string          filenamePath        = "conf\\WCF";
            string          filenamefullPath    = "conf\\WCF\\config.xml";
            WCFComConf      ComParam            = new WCFComConf();
            XmlSerializer   serializerConf      = new XmlSerializer(typeof(WCFComConf));
            FileStream      filestreamFilter    = null;

            if (!Directory.Exists(filenamePath))
                Directory.CreateDirectory(filenamePath);
            try
            {
                filestreamFilter = new FileStream(filenamefullPath, FileMode.Open);
                ComParam = (WCFComConf)serializerConf.Deserialize(filestreamFilter);
                filestreamFilter.Close();
            }
            catch 
            {
                //no existe el fichero genera unos con todas las variables
                filestreamFilter        = new FileStream(filenamefullPath, FileMode.Create);
                ComParam                = new WCFComConf();
                ServerInterfaceString Item1   = new ServerInterfaceString();       
                Item1._ipAddress        = "127.0.0.1";           
                Item1._port             = "8010";
                ServerInterfaceString Item2   = new ServerInterfaceString();
                Item2._ipAddress        = "192.168.1.10";
                Item2._port             = "8010";
                ComParam.ServerInterfaceList.Add(Item1);
                ComParam.ServerInterfaceList.Add(Item2);
                ServerInterfaceString clientItem = new ServerInterfaceString();
                clientItem._ipAddress   = "127.0.0.1";
                clientItem._port        = "8010";
                ComParam.ClientInterface = clientItem;

                serializerConf.Serialize(filestreamFilter, ComParam);
                filestreamFilter.Close();         
            }

            CommConf                    = ComParam;
            ServerInterfaceList         = new List<ServerInterface>();
            for ( int index = 0; index < ComParam.ServerInterfaceList.Count(); index++ )
            {
                ServerInterface item = new ServerInterface();
                item._ipAddress     = IPAddress.Parse(ComParam.ServerInterfaceList[index]._ipAddress);
                item._port          = ComParam.ServerInterfaceList[index]._port;
                ServerInterfaceList.Add(item);            
            }

            if(ComParam.ClientInterface != null)
            {
                ClientInterface = new ServerInterface();
                ClientInterface._ipAddress  = IPAddress.Parse(ComParam.ClientInterface._ipAddress);
                ClientInterface._port       = ComParam.ClientInterface._port;
                //

            }
           

            if (ServerInterfaceList.Count() > 0)
                button1_Click(null, null);
            if (ComParam.ClientInterface != null)
                button3_Click(null, null);

            if (Directory.GetFiles("conf\\WCF\\", "config.xml").Select(path => Path.GetFileName(path)).ToArray().Length > 0)
                return true;

            return false;
        }

        private void WCFForm_Load(object sender, EventArgs e)
        {
            loaded = true;
        }
         

    }
}
