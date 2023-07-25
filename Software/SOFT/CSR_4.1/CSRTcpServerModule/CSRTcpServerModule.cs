using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace CSRTcpServerModule
{
    public class CSRTcpServerModule : csr.modules.CSRBackgroundModule
    {
        //////////////////////////////////////////////////////////////
        private TcpListener                 tcp_listener;
        private CSRTcpServerConfig          config;
        private List<TcpClient>             client_list;
        private object                      _lock_;
        private System.Text.ASCIIEncoding   encoding = new System.Text.ASCIIEncoding();

        //////////////////////////////////////////////////////////////
        public CSRTcpServerModule(string id) : base(id)
        {
            _lock_          = new object();
            client_list     = new List<TcpClient>();
        }

        //////////////////////////////////////////////////////////////
        public override bool Init()
        {
            try
            {
                CSRTcpServerConfigLoader loader = new CSRTcpServerConfigLoader("Config\\TcpServersConfig.xml");
                config = loader.LoadConfig(Id);

                tcp_listener = new TcpListener(IPAddress.Any, config.Port);
                tcp_listener.Start();

                Thread listen_thread        = new Thread(ListenForClients);
                listen_thread.Name          = "CSR Server";
                listen_thread.IsBackground  = true;
                listen_thread.Start();

                WriteConsole("Módulo de comunicación \'" + Id + "\' cargado correctamente.");
                return true;
            }
            catch (Exception e)
            {
                Error("Ocurrió un error durante la carga de configuración de módulo de comunicación [" + Id + "]\n" + e.ToString());
                WriteConsole("Módulo de comunicación \'" + Id + "\' no ha sido cargado correctamente. Error de configuración");
                return false;
            }
        }

        //////////////////////////////////////////////////////////////
        public override void HandleMessages(csr.com.CSRMessage message)
        {
            // Broadcast a todos los clientes conectados (no se hace login, por lo que no sabemos para quien es)
            foreach (TcpClient client in client_list)
            {
                //lock (_lock_)
                {
                    NetworkStream ns = client.GetStream();
                    byte[] message_ba = encoding.GetBytes(String.Format("{0}#{1}#", message.DestinyId, message.ToString()));
                    ns.Write(message_ba, 0, message_ba.Length);
                    ns.Flush();
                }
            }
        }

        //////////////////////////////////////////////////////////////
        private void ListenForClients()
        {
            while (true)
            {
                TcpClient newClient = tcp_listener.AcceptTcpClient();
                Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                t.Name = "CSR Listener";
                t.IsBackground = true;
                t.Start(newClient);
                Thread.Sleep(1);
            }
        }

        //////////////////////////////////////////////////////////////
        private void HandleClient(object _client)
        {
            TcpClient client                    = (TcpClient)_client;
            NetworkStream server_stream         = client.GetStream();
            byte[] buffer                       = new byte[1024];

            client_list.Add(client);

            HandleMessages(new csr.com.CSRMessage("csr_slave", "super mensaje!"));

            while(true)
            {
                //lock (_lock_)
                {
                    int read_count = server_stream.Read(buffer, 0, 1024);
                    string data = encoding.GetString(buffer, 0, read_count);
                    string[] split_data = data.Split('#');
                    if (split_data.Length >= 2)
                    {
                        SendMessage(split_data[0], data.Substring(split_data[0].Length + 1));
                    }
                }

            }
        }

        //////////////////////////////////////////////////////////////
    }
}
