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

using System.IO;
using MSTSCLib;

namespace RDPModule
{
    
   
    public partial class RDPForm : Form
    {
        public class RDPComConf
        {
            [XmlElement("Comunication_Params_server")]
            public List<ServerRDPInterfaceString> ServerInterfaceList = new List<ServerRDPInterfaceString>();
           
        }

        public class ServerRDPInterfaceString
        {
           
            public string   _ipAddress  ="192.168.11.50";          
            public string   _port       = "8010";
            public string _username = "Roboconcept";
            public string _password = "R7D-t=KM";
        }

        private csr.modules.CSRFormModule   parent;
        ModuleDelegates                     moduleDelegates;
        RDPComConf                          CommConf;
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="module_delegates"></param>
        public RDPForm(csr.modules.CSRFormModule _parent, ModuleDelegates module_delegates)
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
        /// <param name="str"></param>
        public void Append(string str)
        {
            
        }
        /// <summary>
        /// inicializa el cliente conectandose a la url especificada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RDPForm));
            this.rdp2 = new AxMSTSCLib.AxMsTscAxNotSafeForScripting();
            this.rdp = new AxMSTSCLib.AxMsTscAxNotSafeForScripting();
            ((System.ComponentModel.ISupportInitialize)(this.rdp2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdp)).BeginInit();

            this.rdp2.Enabled = true;
            this.rdp2.Location = new System.Drawing.Point(this.panel2.Location.X, this.panel2.Location.Y);
            this.rdp2.Name = "rdp2";
            this.rdp2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("rdp2.OcxState")));
            this.rdp2.Size = new System.Drawing.Size(this.panel2.Size.Width, this.panel2.Size.Height);
            this.rdp2.TabIndex = 13;
            // 
            // rdp
            // 
            this.rdp.Enabled = true;
            this.rdp.Location = new System.Drawing.Point(this.panel1.Location.X, this.panel1.Location.Y);
            this.rdp.Name = "rdp";
            this.rdp.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("rdp.OcxState")));
            this.rdp.Size = new System.Drawing.Size(this.panel1.Size.Width, this.panel1.Size.Height);
            this.rdp.TabIndex = 14;

            this.Controls.Add(this.rdp);
            this.Controls.Add(this.rdp2);

            ((System.ComponentModel.ISupportInitialize)(this.rdp2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rdp)).EndInit();


            try
            {
                
                               
                if (rdp.Connected.ToString() != "1")
                {
                    rdp.Server = CommConf.ServerInterfaceList[0]._ipAddress;
                    rdp.UserName = CommConf.ServerInterfaceList[0]._username;

                    IMsTscNonScriptable secured = (IMsTscNonScriptable)rdp.GetOcx();
                    secured.ClearTextPassword = CommConf.ServerInterfaceList[0]._password;
                    rdp.Connect();
                }

            }
            catch (Exception Ex)
            {
                string textout = Ex.ToString();
            }

            try
            {


                if (rdp2.Connected.ToString() != "1")
                {
                    rdp2.Server = CommConf.ServerInterfaceList[1]._ipAddress;
                    rdp2.UserName = CommConf.ServerInterfaceList[1]._username;

                    IMsTscNonScriptable secured = (IMsTscNonScriptable)rdp2.GetOcx();
                    secured.ClearTextPassword = CommConf.ServerInterfaceList[1]._password;
                    rdp2.Connect();
                }

            }
            catch (Exception Ex)
            {
                string textout = Ex.ToString();
            }
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
            string          filenamePath        = "conf\\RDP";
            string          filenamefullPath    = "conf\\RDP\\config.xml";
            RDPComConf      ComParam            = new RDPComConf();
            XmlSerializer   serializerConf      = new XmlSerializer(typeof(RDPComConf));
            FileStream      filestreamFilter    = null;

            if (!Directory.Exists(filenamePath))
                Directory.CreateDirectory(filenamePath);
            try
            {
                filestreamFilter = new FileStream(filenamefullPath, FileMode.Open);
                ComParam = (RDPComConf)serializerConf.Deserialize(filestreamFilter);
                filestreamFilter.Close();
            }
            catch 
            {
                //no existe el fichero genera unos con todas las variables
                filestreamFilter        = new FileStream(filenamefullPath, FileMode.Create);
                ComParam                = new RDPComConf();
                ServerRDPInterfaceString Item1   = new ServerRDPInterfaceString();       
                Item1._ipAddress        = "192.168.11.80";           
                Item1._port             = "8010";
                Item1._username = "Roboconcept";
                Item1._password = "R7D-t=KM";
                ServerRDPInterfaceString Item2   = new ServerRDPInterfaceString();
                Item2._ipAddress        = "192.168.12.80";
                Item2._port             = "8010";
                Item2._username = "Roboconcept";
                Item2._password = "R7D-t=KM";
                ComParam.ServerInterfaceList.Add(Item1);
                ComParam.ServerInterfaceList.Add(Item2);
               
                serializerConf.Serialize(filestreamFilter, ComParam);
                filestreamFilter.Close();         
            }

            CommConf                    = ComParam;         

         
            if (Directory.GetFiles("conf\\RDP\\", "config.xml").Select(path => Path.GetFileName(path)).ToArray().Length > 0)
                return true;

            return false;
        }

      

        private void RDPForm_Leave(object sender, EventArgs e)
        {


         
            try
            {
                // Check if connected before disconnecting
                if (rdp.Connected.ToString() == "1")
                    rdp.Disconnect();
            }
            catch (Exception Ex)
            {
                string textout = Ex.ToString();
            }
            try
            {
                if (rdp2.Connected.ToString() == "1")
                    rdp2.Disconnect();
            }
            catch (Exception Ex)
            {
                string textout = Ex.ToString();
            }

            this.Controls.Remove(this.rdp);
            this.Controls.Remove(this.rdp2);


        }

     
        private void RDPForm_Enter(object sender, EventArgs e)
        {
            //button3_Click(sender, e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
