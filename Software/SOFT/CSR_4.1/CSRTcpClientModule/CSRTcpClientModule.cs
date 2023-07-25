using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace CSRTcpClientModule
{
    public class CSRTcpClientModule : csr.modules.CSRBackgroundModule
    {
        //////////////////////////////////////////////////////////////
        private CSRTcpClientConfig          config;
        private TcpClient                   tcpClient;
        private object                      _lock_ = new object();
        private System.Text.ASCIIEncoding   encoding = new System.Text.ASCIIEncoding();

        //////////////////////////////////////////////////////////////
        public CSRTcpClientModule(string _id) : base (_id)
        {
            tcpClient = new TcpClient();
        }

        //////////////////////////////////////////////////////////////
        public override bool Init()
        {
            try
            {
                CSRTcpClientConfigLoader loader = new CSRTcpClientConfigLoader("Config\\TcpClientsConfig.xml");
                config = loader.LoadConfig(Id);
                WriteConsole("Módulo de comunicación \'" + Id + "\' cargado correctamente.");
            }
            catch(Exception e)
            {
                Error("Ocurrió un error durante la carga de configuración de módulo de comunicación [" + Id + "]\n" + e.ToString());
                WriteConsole("Módulo de comunicación \'" + Id + "\' no ha sido cargado correctamente. Error de configuración");
                return false;
            }

            return Connect();
        }

        //////////////////////////////////////////////////////////////
        private bool Connect()
        {
            try{
                tcpClient.Connect(config.ServerAddr, config.Port);
            }
            catch
            {
                Error(String.Format("No se ha podido realizar la conexión mediante {0}, Server[{1}] Port[{2}] ", Id, config.ServerAddr, config.Port));
                WriteConsole("No se ha podido realizar la conexión mediante " + Id);
                return false;
                
            }
            Thread read_thread = new Thread(ReadDataThread);
            read_thread.Name = "CSR Client Read Data";
            read_thread.IsBackground = true;
            read_thread.Start();
            return true;
        }

        //////////////////////////////////////////////////////////////
        public override void HandleMessages(csr.com.CSRMessage message)
        {
            //lock (_lock_)
            {
                NetworkStream ns = tcpClient.GetStream();
                byte[] message_ba = encoding.GetBytes(String.Format("{0}#{1}#",message.DestinyId, message.ToString()));
                ns.Write(message_ba, 0, message_ba.Length);
                ns.Flush();
            }
        }

        //////////////////////////////////////////////////////////////
        public void ReadDataThread()
        {
            byte[] buffer = new byte[1024];
            while(true)
            {
                //lock (_lock_)
                {
                    NetworkStream ns = tcpClient.GetStream();
                    int read_count = ns.Read(buffer, 0, 1024);
                    string data = encoding.GetString(buffer, 0, read_count);
                    string[] split_data = data.Split('#');
                    if( split_data.Length >= 2 )
                        SendMessage(split_data[0], data.Substring(split_data[0].Length+1));
                }
                Thread.Sleep(1);
            }
        }

        //////////////////////////////////////////////////////////////
    }
}
