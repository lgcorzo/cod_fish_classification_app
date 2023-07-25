using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics;
using TwincatModule;
using System.Drawing;
using System.Net.NetworkInformation;

namespace AlarmModule
{
    public class AlarmModule : csr.modules.CSRFormModule
    {
        private Alarms      m_alarmas;
        AlarmasConf         CommConf;
        string              filenamePath;

        public class AlarmasConf
        {
            [XmlElement("WOL_Params")]
            public List<WOLParams> ListWolParams = new List<WOLParams>();           
            [XmlElement("BAT_Path")]
            public string YourApplicationPath = "";
            [XmlElement("BAT_Path_stop")]
            public string YourApplicationPath_stop = "";

        }

        public class WOLParams
        {
            [XmlElement("ipAddress1")]
            public string ipAddress1 = "192.168.11.250";
            [XmlElement("MAC1")]
            public string MAC1 = "F8:32:E4:6E:CC:9C";
            [XmlElement("puerto1")]
            public int puerto1 = 8;
            [XmlElement("ActivarRemotamente1")]
            public bool activar1 = false;
        }

        //////////////////////////////////////////////////////////////////////////////////
        public AlarmModule(string _id)
            : base(_id)
        {
            //LAUNION: Para cuando inicie el primer modulo (este) encienda los pcs de vision       
            //carga los parametros de encendido de los PC 
            LoadIOConf();
            for(int index = 0; index < CommConf.ListWolParams.Count(); index++)
            {
                if (CommConf.ListWolParams[index].activar1 == true)
                    WakeFunction(CommConf.ListWolParams[index].MAC1, CommConf.ListWolParams[index].ipAddress1, CommConf.ListWolParams[index].puerto1);
            }
           
           

           
        }
        public override bool Destroy()
        {

            return true;
        }


     
        public void AlarmModuleBatexecStart()
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.WindowStyle       = ProcessWindowStyle.Hidden;
                processInfo.FileName = Path.GetFileName(CommConf.YourApplicationPath);
                processInfo.WorkingDirectory = Path.GetDirectoryName(CommConf.YourApplicationPath);
                //processInfo.UseShellExecute = false;
                //processInfo.Arguments = "/c START ";
                Process.Start(processInfo);
            }
            catch
            {
                WriteConsole("Script start no encontrado", true);
            }
            
        }

        public void AlarmModuleBatexecStop()
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo();
                processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                processInfo.FileName = Path.GetFileName(CommConf.YourApplicationPath_stop);
                processInfo.WorkingDirectory = Path.GetDirectoryName(CommConf.YourApplicationPath_stop);
                //processInfo.UseShellExecute = false;
                //processInfo.Arguments = "/c START ";
                Process.Start(processInfo);
            }
            catch
            {
                WriteConsole("Script stop no encontrado", true);
            }

        }

        private bool LoadIOConf()
        {
            //carga el filtro
            filenamePath = "conf\\Alarmas";
            string filenamefullPath = "conf\\Alarmas\\config.xml";
            AlarmasConf ComParam = new AlarmasConf();
            XmlSerializer serializerConf = new XmlSerializer(typeof(AlarmasConf));
            FileStream filestreamFilter;

            if (!Directory.Exists(filenamePath))
                Directory.CreateDirectory(filenamePath);

            try
            {
                filestreamFilter = new FileStream(filenamefullPath, FileMode.Open);
                ComParam = (AlarmasConf)serializerConf.Deserialize(filestreamFilter);
                filestreamFilter.Close();
            }
            catch (Exception e1)
            {
                string textoout = e1.ToString();
                //no existe el fichero genera unos con todas las variables
                filestreamFilter = new FileStream(filenamefullPath, FileMode.Create);
                ComParam = new AlarmasConf();
                WOLParams WOLParamsitem = new WOLParams();
                ComParam.ListWolParams.Add(WOLParamsitem);
                serializerConf.Serialize(filestreamFilter, ComParam);
                filestreamFilter.Close();
            }

            CommConf = ComParam;
           

            if (Directory.GetFiles("conf\\Alarmas\\", "config.xml").Select(path => Path.GetFileName(path)).ToArray().Length > 0)
                return true;

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="MAC_ADDRESS"></param>
        /// <param name="stIpAddress"></param>
        /// <param name="port"></param>
        private void WakeFunction(string MAC_ADDRESS, string stIpAddress, int port)
        {

          
                        WOL client = new WOL();
                        // IPAddress Adress = IPAddress.Parse(stIpAddress);
                        IPAddress Adress = IPAddress.Parse(stIpAddress);
                        IPAddress AdressBC = IPAddress.Broadcast;

                        client.Bind(new IPEndPoint(Adress, 0)); // Bind to local address using automatic port
                        client.Connect(AdressBC, port); // port=9 let's use this one 
                        client.SetClientToBrodcastMode();
                        //set sending bites
                        int counter = 0;
                        //buffer to be send
                        byte[] bytes = new byte[1024];   // more than enough :-)
                                                         //first 6 bytes should be 0xFF
                        for (int y = 0; y < 6; y++)
                            bytes[counter++] = 0xFF;
                        //now repeate MAC 16 times
                        for (int y = 0; y < 16; y++)
                        {
                            int i = 0;
                            for (int z = 0; z < 6; z++)
                            {
                                string submac = MAC_ADDRESS.Substring(i, 2);
                                bytes[counter++] = byte.Parse(submac, NumberStyles.HexNumber);
                                i += 3;
                            }
                        }

                        //now send wake up packet
                        int reterned_value = client.Send(bytes, 1024);
                        client.Close();          
        }
        /// <summary>
        /// function to make a wol over all the ethernet interfaces
        /// </summary>
        /// <param name="MAC_ADDRESS"></param>
        /// <param name="stIpAddress"></param>
        /// <param name="port"></param>
        private void WakeFunctionALL(string MAC_ADDRESS, string stIpAddress, int port)
        {

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                string output = "";
               
                    if (adapter.NetworkInterfaceType == NetworkInterfaceType.Ethernet) //&& adapter.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in adapter.GetIPProperties().UnicastAddresses)
                        {
                           
                            output = ip.Address.ToString();
                            WOL client = new WOL();
                            // IPAddress Adress = IPAddress.Parse(stIpAddress);
                            IPAddress Adress = IPAddress.Parse(output);
                            IPAddress AdressBC = IPAddress.Broadcast;
                            client.Bind(new IPEndPoint(Adress, 0)); // Bind to local address using automatic port
                            client.Connect(AdressBC, port); // port=9 let's use this one 
                            client.SetClientToBrodcastMode();
                            //set sending bites
                            int counter = 0;
                            //buffer to be send
                            byte[] bytes = new byte[1024];   // more than enough :-)
                                                             //first 6 bytes should be 0xFF
                            for (int y = 0; y < 6; y++)
                                bytes[counter++] = 0xFF;
                            //now repeate MAC 16 times
                            for (int y = 0; y < 16; y++)
                            {
                                int i = 0;
                                for (int z = 0; z < 6; z++)
                                {
                                    string submac = MAC_ADDRESS.Substring(i, 2);
                                    bytes[counter++] = byte.Parse(submac, NumberStyles.HexNumber);
                                    i += 3;
                                }
                            }

                            //now send wake up packet
                            int reterned_value = client.Send(bytes, 1024);
                            client.Close();
                        }
                    }
               
            }           
            }

        ////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////

        //////////////////////////////////////////////////////////////////////////////////
        public override bool Init()
        {
            base.Init();

            ModuleDelegates module_delegates = new ModuleDelegates(AlarmModuleBatexecStop);
            WindowForm = new AlarmForm(this, module_delegates);
           

            this.m_alarmas = new Alarms(this);
            this.m_alarmas.CambioDeAlarma += ActualizarAlarma;
            this.m_alarmas.Communication = this.GetGlobalParameter("Communication").ToString();

            if (this.m_alarmas.Communication == "message")
                this.SetGlobalParameter("Alarm", this.m_alarmas);
          
            WriteConsole("Módulo cargado correctamente.");
            AlarmModuleBatexecStart();
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////
        public override void HandleMessages(csr.com.CSRMessage message)
        {
            if(Status != csr.modules.CSRModuleStatus.Closing)
            {
                //TODO: Esto debe de escribirse correctamente en el modulo twincat, de momento el CSR lo machaca
                SetGlobalParameter("Communication", "twincat");
                this.m_alarmas.Communication = this.GetGlobalParameter("Communication").ToString();
                //

                string strMessage = message.ToString();

                //Parametros del mensaje
                string[] strParams = strMessage.Split('#');
                //

                IO_Parameters io = (IO_Parameters)GetGlobalParameter("IO");

                //Si solo se envia un codigo, significa que es la alarma a activar
                if (strParams.Length == 1)
                {
                    if (double.Parse(strParams[0]) > 0)
                        m_alarmas.activarAlarma(strParams[0]);
                    else
                        m_alarmas.desactivarAlarma((double.Parse(strParams[0]) * -1).ToString());
                }
                else if (this.m_alarmas.Communication == "twincat")
                {
                    m_alarmas.setBlock(strParams[0], Convert.ToUInt32(io.Out[strParams[0]].Value));
                }
            }     
        }

        //////////////////////////////////////////////////////////////////////////////////

        private void ActualizarAlarma(object sender, EventArgs e)
        {
            try
            {
                if(Status != csr.modules.CSRModuleStatus.Closing && Status != csr.modules.CSRModuleStatus.Closed)
                {
                    List<string> keyList = new List<string>();
                    if (this.m_alarmas.getIds(sender.ToString(), out keyList))
                    {
                        foreach (string val in keyList)
                        {
                            DescripcionAlarma da = new DescripcionAlarma();
                            if (this.m_alarmas.Descripcion(val, out da))
                            {
                                if (this.m_alarmas.alarmaActiva(val))
                                {
                                    //La Union: En caso de que sea necesario rearmar que no aparezcan las alarmas de los variadores
                                    if ((Convert.ToInt32(val) > 200) || (!m_alarmas.alarmaActiva("217")))
                                    {
                                        if (((AlarmForm)WindowForm).AñadirALista(da))
                                        {
                                            SendMessage("bd", "insert#PFAlarmas#Fecha,IdAlarma#\"" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.FFF") + "\"," + da.ID);
                                            WriteConsole("Alarma " + da.ID + " activada: " + da.Titulo, true);
                                            if (!ButtonColor("alarm", Color.Yellow, false))
                                                Error("Error al colorear la alarma " + da.ID, true, false);
                                        }

                                    }
                                    //Solo la condicion es parcheada.
                                }
                                else
                                {
                                    if (((AlarmForm)WindowForm).BorrarDeLista(da))
                                    {
                                        WriteConsole("Alarma " + da.ID + " desactivada: " + da.Titulo, true);
                                        if (((AlarmForm)WindowForm).ListaVacia())
                                            if (!ButtonColor("alarm", null, false))
                                                Error("Error al decolorear la alarma " + da.ID, true, false);
                                    }
                                }
                            }
                            else
                                WriteConsole("No se encuentra la descripción de la alarma " + val + " entre las descripciones cargadas.", true);
                        }
                    }
                    else
                    {
                        WriteConsole("No se pueden actualizar las alarmas ya que no hay ninguna cargada.", true);
                    }
                }
            }
            catch (Exception ex)
            {
                if (Status != csr.modules.CSRModuleStatus.Closing && Status != csr.modules.CSRModuleStatus.Closed)
                {
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame.GetFileLineNumber();
                    Error("Error al activar una alarma. Linea de código " + line + ": " + ex.Message);
                }
            }
        }
    }

    public delegate void ExecuteScriptDelegate();

    //////////////////////////////////////////////////////////////
    public class ModuleDelegates
    {

        public ExecuteScriptDelegate ExecuteScript;
        
        //////////////////////////////////////////////////////////////
        public ModuleDelegates(ExecuteScriptDelegate pExecuteScript)
        {
            ExecuteScript = pExecuteScript;         
        }

        //////////////////////////////////////////////////////////////

    }
}
