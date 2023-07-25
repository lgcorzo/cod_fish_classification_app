using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TwincatModule.VISTA;


namespace TwincatModule
{
   

    public class TwincatModule : csr.modules.CSRFormModule
    {
        //////////////////////////////////////////////////////////////////////////////////
        private TwinCatCommunication m_twincat_com;
        private Thread          thrRefreshTC;
        private bool            needUpdate;
        private UInt64          CommsystemtimeStamp;
        private DateTime        lastConection;
        private int             connectionTimeout = 50;
        public ParamConf        ConfiguracionParams;
     

        //////////////////////////////////////////////////////////////////////////////////
        public TwincatModule(string _id)
            : base(_id)
        { }

        //////////////////////////////////////////////////////////////////////////////////
        public override bool Init()
        {
            base.Init();
            
            //carga los parametros de configuracion
            LoadIOConf();
            WindowForm = new TwincatForm(this);
            
            m_twincat_com = new TwinCatCommunication(this);
            m_twincat_com.OnReadError += OnTwincatReadError;
            m_twincat_com.OnReadError += OnTwincatWriteError;
          

            ((TwincatForm)this.WindowForm).SelectedInChanged += new EventHandler(DisplayInput);
            ((TwincatForm)this.WindowForm).SelectedOutChanged += new EventHandler(DisplayOutput);


            if (!m_twincat_com.Connect(ConfiguracionParams.TWINCAT_ip, 851)) //La Union        
            {
                //OnError(this, "No se ha podido conectar al servidor Twincat!");
                throw (new Exception("Error al conectar con twincat."));
            }

            try
            {
                m_twincat_com.InitConfig();
                this.SetGlobalParameter("IO", m_twincat_com.IOParameters);
            }
            catch (Exception e)
            {
                //OnError(this, "Error al iniciar la configuración Twincat.\n" + e.ToString());
                throw (new Exception("Error al iniciar la configuración Twincat.\n" + e.ToString()));
            }

            SuscribeRedirections();

            ((TwincatForm)this.WindowForm).SetSourceIn(this.m_twincat_com.IOParameters.ToList(true));
            ((TwincatForm)this.WindowForm).SetSourceOut(this.m_twincat_com.IOParameters.ToList(false));
        

            thrRefreshTC = new Thread(TCUpdater);
            thrRefreshTC.Name = "TCUpdater";
            thrRefreshTC.IsBackground = true;
            thrRefreshTC.Start();
            needUpdate = false;       
            SetGlobalParameter("Communication", "twincat");
            lastConection = DateTime.Now;
            WriteConsole("Modulo cargado correctamente.");

        
            return true;
        }

        public bool LoadIOConf()
        {
            //carga el filtro
            string filenamePath = "conf\\Twincat\\";
            string filenamefullPath = "conf\\Twincat\\config.xml";
            ParamConf ComParam = new ParamConf();
            XmlSerializer serializerConf = new XmlSerializer(typeof(ParamConf));
            FileStream filestreamFilter;
            if (!Directory.Exists(filenamePath))
                Directory.CreateDirectory(filenamePath);
            try
            {
                filestreamFilter = new FileStream(filenamefullPath, FileMode.Open);
                ComParam = (ParamConf)serializerConf.Deserialize(filestreamFilter);
                filestreamFilter.Close();
            }
            catch
            {
                //no existe el fichero genera unos con todas las variables
                filestreamFilter = new FileStream(filenamefullPath, FileMode.Create);
                ComParam = new ParamConf();
                serializerConf.Serialize(filestreamFilter, ComParam);
                filestreamFilter.Close();
            }

            ConfiguracionParams = ComParam;
   
            //parametros de la camara        
            if (Directory.GetFiles("conf\\Twincat\\", "config.xml").Select(path => Path.GetFileName(path)).ToArray().Length > 0)
                return true;



            return false;
        }

        //////////////////////////////////////////////////////////////////////////////////
        public  bool ReInit()
        {
           
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////
        public override bool Destroy()
        {
            m_twincat_com.Disconnect();
            thrRefreshTC.Join();
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////
        public void ResetMaquina()
        {
            try
            {
              
                //Conectar los diferentes bloques al ADS  
                m_twincat_com.DisconnectBlocks();             
                m_twincat_com.ConnectBlocks(m_twincat_com.tcAds.ServerNetID, m_twincat_com.tcAds.ServerPort);
                //Ver que variable se almacena en cada bloque y añadir las redirecciones
           
                foreach (TCBlock tcb in m_twincat_com.m_TCbloques.Values)
                {                 
                  //Redirecciones
                    tcb.ReAñadirRedireccion();              
                }

            }
            catch (Exception ex)
            {
                string textout = ex.ToString();
            }
            

        }

        //////////////////////////////////////////////////////////////////////////////////
        public override void HandleMessages(csr.com.CSRMessage message)
        {
            
            string strMessage = message.ToString();
            string [] strMessagesplited = strMessage.Split('_');

            if (strMessagesplited[0] == "CAMERASTATUS" && ConfiguracionParams != null) //SendMessage("Communication", "CAMERA_STATUS");
            {
                ConfiguracionParams.Cam_life_count =ulong.Parse(strMessagesplited[1]);
                return;
            }
            if (strMessagesplited[0] == "SYSTEMID" && ConfiguracionParams != null)
            {
                ConfiguracionParams.Line_ID = int.Parse(strMessagesplited[1]);
                return;
            }
            if (strMessagesplited[0] == "SYSTEMERROR" && ConfiguracionParams != null) //SendMessage("Communication", "SYSTEMERROR");
            {
                ConfiguracionParams.Error = uint.Parse(strMessagesplited[1]);
                return;
            }

        }

        //////////////////////////////////////////////////////////////////////////////////

        private void TCUpdater()
        {

            lastConection = DateTime.Now;
            IO_Parameters       io                  = m_twincat_com.IOParameters;
            string              Variable_IO         = " ";
            string              Variable_IO_Fisrt   = " ";
            string              Variable_IO_second  = " ";
            UInt64              actualsystemtime    = 0;
            UInt64              systemtime          = 0;
            UInt32              counter             = 0;
            UInt32              counter5000         = 0;
            bool first_time_comm = true;


            while (Status != csr.modules.CSRModuleStatus.Closing && Status != csr.modules.CSRModuleStatus.Closed)
            {

               

                counter++;
                if (counter > 500)
                    counter = 0;
                counter5000++;
                if (counter5000 > 5000)
                    counter5000 = 5000;

                Thread.Sleep(200);
                try
                {
                    //lectura de la variable de timestamp
                    Variable_IO = "GVL.CSR";
                    Variable_IO += "_TIME_STAMP";
                    try
                    {
                        actualsystemtime = Convert.ToUInt64(io.Out[Variable_IO].Value);
                        
                    }
                    catch (Exception ex)
                    {
                        string textoout = ex.ToString();
                    }
                    //Timeout Watcher
                  
                    if (CommsystemtimeStamp == actualsystemtime && CommsystemtimeStamp != 0)
                    {
                        SendMessage("alarm", "1003");
                        //Conectar los diferentes bloques al ADS
                        //sends messages to the twincat control statur                               
                        ((TwincatForm)WindowForm).Reconnect();
                       
                    }
                    else
                    {
                        
                        SendMessage("alarm", "-1003");
                    }
                    CommsystemtimeStamp = actualsystemtime;

                    //Si el modulo esta activo actualizar, sino no gastar recursos del PC
                    if (needUpdate)
                    {
                        needUpdate = false;
                        if (WindowForm.IsDisposed)
                            return;
                        ((TwincatForm)this.WindowForm).IOUpdate();
                    }

                    if (ConfiguracionParams.Line_ID > 0)
                    {
                        Variable_IO_Fisrt = "GVL.CSR_LINE_" + ConfiguracionParams.Line_ID.ToString();
                        Variable_IO_second = "_TIME_STAMP";
                    }
                    else
                    {
                        Variable_IO_Fisrt = "GVL.CSR_SYSTEM";
                        Variable_IO_second = "_TIME_STAMP";
                        ConfiguracionParams.Cam_life_count = counter;
                       
                    }

                    Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                    io.Out[Variable_IO].Write((UInt32)ConfiguracionParams.Cam_life_count);

                    if (ConfiguracionParams.Line_ID > 0)
                    {
                        Variable_IO_Fisrt = "GVL.CSR_SYSTEM";
                        Variable_IO_second = "_TIME_STAMP";
                        Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                        systemtime = Convert.ToUInt64(io.Out[Variable_IO].Value);

                        Variable_IO_Fisrt = "GVL.CSR_LINE_" + ConfiguracionParams.Line_ID.ToString();
                        Variable_IO_second = "_ID";
                        Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                        io.Out[Variable_IO].Write((UInt16)systemtime);     
                        
                        if(ConfiguracionParams.Error != 0)
                        {
                            Variable_IO_Fisrt = "GVL.CSR_LINE_" + ConfiguracionParams.Line_ID.ToString();
                            Variable_IO_second = "_SPECIES_TYPE";
                            Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                            io.Out[Variable_IO].Write("CLEAN THE CAMERA AREA");

                            Variable_IO_Fisrt = "GVL.CSR_SYSTEM";
                            Variable_IO_second = "_ALARMS";
                            Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                            io.Out[Variable_IO].Write((UInt16)ConfiguracionParams.Error);
                        }              
                    }
                    else
                    {

                        Variable_IO_Fisrt = "GVL.CSR_LINE_" + "1";
                        Variable_IO_second = "_SPECIES_TYPE";
                        Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                        string text1 = Convert.ToString(io.Out[Variable_IO].Value);

                        Variable_IO_Fisrt = "GVL.CSR_LINE_" + "2";
                        Variable_IO_second = "_SPECIES_TYPE";
                        Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                        string text2 = Convert.ToString(io.Out[Variable_IO].Value);
                        ConfiguracionParams.Error = 0;
                        if (text1.Contains("CLEAN"))
                            ConfiguracionParams.Error = 1;

                        if(text2.Contains("CLEAN"))
                            ConfiguracionParams.Error = 1;

                        Variable_IO_Fisrt = "GVL.CSR_SYSTEM";
                        Variable_IO_second = "_ALARMS";
                        Variable_IO = Variable_IO_Fisrt + Variable_IO_second;
                        io.Out[Variable_IO].Write((UInt16)ConfiguracionParams.Error);
                    }
                    
                  
                   if (Status == csr.modules.CSRModuleStatus.Running && first_time_comm == true)
                    {
                        first_time_comm = false;
                        this.GoToModule("Communication");

                    }
                       


                }
                catch { }
         
            }
        }

        ///////////////////////////////////////////////////////////////////////
        public void OnTwincatReadError(object sender, string var_name, string function_name)
        {
            WriteConsole(String.Format("Error leyendo variable Twincat. Nombre Variable:\"{0}\" Función:\"{1}\"", var_name, function_name), true);
        }

        ///////////////////////////////////////////////////////////////////////
        public void OnTwincatWriteError(object sender, string var_name, string function_name)
        {
            WriteConsole(String.Format("Error escribiendo variable Twincat. Nombre Variable:\"{0}\" Función:\"{1}\"", var_name, function_name), true);
        }

        private void Leer(object sender, EventArgs e)
        {
            this.m_twincat_com.ForceReadTwincatValues();
        }


        private void DisplayInput(object sender, EventArgs e)
        {
            try
            {
                string comment = "No se ha podido leer la variable.";
                string valueStr = "-";
                if (m_twincat_com.Config.Variables.ContainsKey(sender.ToString()))
                {
                    comment     = m_twincat_com.Config.Variables[sender.ToString()].Symbol.Comment;
                    valueStr    = m_twincat_com.IOParameters.In[sender.ToString()].Value.ToString();
                }

                ((TwincatForm)this.WindowForm).DisplayIO(true, comment, valueStr);

            }
            catch (Exception er)
            {
                string textoout = er.ToString();
                ((TwincatForm)this.WindowForm).ErrorDisplaying(true);
            }
        }

        private void DisplayOutput(object sender, EventArgs e)
        {
            try
            {
                string comment = "No se ha podido leer la variable.";
                string valueStr = "-";
                if (m_twincat_com.Config.Variables.ContainsKey(sender.ToString()))
                {
                    comment = m_twincat_com.Config.Variables[sender.ToString()].Symbol.Comment;
                    valueStr = m_twincat_com.IOParameters.Out[sender.ToString()].Value.ToString();
                }
                ((TwincatForm)this.WindowForm).DisplayIO(false, comment, valueStr);
            }
            catch (Exception er)
            {
                string textoout = er.ToString();
                ((TwincatForm)this.WindowForm).ErrorDisplaying(false);
            }
        }


        public void IODisplayUpdate()
        {
            needUpdate = true;
        }

        private void VariableRedirection(object sender, EventArgs e)
        {
            IO_Parameter IO = (IO_Parameter)sender;
            foreach (string dest in IO.Redirections)
                SendMessage(dest, IO.Name + "#" + IO.Value.ToString());
        }

        private void SuscribeRedirections()
        {
            foreach (IO_Parameter IO in this.m_twincat_com.IOParameters.In.Values)
            {
                if (IO.Redirections.Count > 0)
                    IO.OnChange += VariableRedirection;
            }

            foreach (IO_Parameter IO in this.m_twincat_com.IOParameters.Out.Values)
            {
                if (IO.Redirections.Count > 0)
                    IO.OnChange += VariableRedirection;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////
    }

    public class ParamConf
    {
        [XmlElement("TWINCAT_ip")]
        public string   TWINCAT_ip      = "169.254.181.242.1.1";
        public ulong   Cam_life_count  = 0;
        public int     Line_ID         = 0;
        public uint    Error           = 0;

    }
}
