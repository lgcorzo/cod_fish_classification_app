using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using ServiceLibrary;
using System.Net;
using AdquisitionModule;
using System.Threading;
using System.Xml.Serialization;
using System.Xml;

namespace WCFModule
{
    public class WCFModule : csr.modules.CSRFormModule
    {

        public PilaImagenesRemote           listimagenestosend  = null;
        System.Threading.Thread             thServer;
        Thread                              HiloenvioComm       = null;
        List<ServerInterface>               ServerInterfaceList = null;
        private String                      ipAddressCliente;
        private ChannelFactory<ServiceLibraryClient.IService1> clientChannel = null;
        public ServiceLibrary.service1      service             = null;
        ServiceLibraryClient.IService1      proxy               = null;
        NetTcpBinding                       tcpBinding          = null;
        EndpointAddress                     endpointAddress     = null;
        string                              endPointAddr        = "";
        private List<ServiceHost>           hostlist            = null;      
        private string                      urlMeta             = "";
        List<string>                        urlService          = null;
        string                              m_IP_direccion, m_puerto;
        object                              m_lock              = new object();
        private static System.Threading.AutoResetEvent stopFlag = new System.Threading.AutoResetEvent(false);
        //////////////////////////////////////////////////////////////////////////////////
        public WCFModule(string _id)
            : base(_id)
        { }


        //////////////////////////////////////////////////////////////////////////////////
        public override bool Init()
        {
            base.Init();
            ModuleDelegates module_delegates    = new ModuleDelegates(IniciarServidorWCF, TerminarServidorWCF, IniciarClienteWCF);
            WindowForm                          = new WCFForm(this, module_delegates);
            WriteConsole("Módulo cargado correctamente.",true);
            //inicio las comunicaciones
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////
        public override void HandleMessages(csr.com.CSRMessage message)
        {
            if (message.ToString() == "SEND_IMAGES_TEXTURA")
            {
                

            }
        }

        /// <summary>
        /// lanza el hilo de envio de parametros
        /// </summary>
        /// <param name="imagenes"></param>
        public  void EnviarImagenesTexturaCliente(ImagesToProcess imagenes)
        {

            if (imagenes == null)
            return;

            ImagesToProcessRemote ImagenesRemote = new ImagesToProcessRemote();

            if (imagenes.img1RGB != null)
                ImagenesRemote.img1RGB = imagenes.img1RGB.CopyImage();
            if (imagenes.img2RGB != null)
                ImagenesRemote.img2RGB = imagenes.img2RGB.CopyImage();
            if (imagenes.img3RGB != null)
                ImagenesRemote.img3RGB = imagenes.img3RGB.CopyImage();
            if (imagenes.img1NIR != null)
                ImagenesRemote.img1NIR = imagenes.img1NIR.CopyImage();
            if (imagenes.img2NIR != null)
                ImagenesRemote.img2NIR = imagenes.img2NIR.CopyImage();
            if (imagenes.img3NIR != null)
                ImagenesRemote.img3NIR = imagenes.img3NIR.CopyImage();
            if (imagenes.imgAnteriorRGB != null)
                ImagenesRemote.imgAnteriorRGB = imagenes.imgAnteriorRGB.CopyImage();
            if (imagenes.imgPosteriorRGB != null)
                ImagenesRemote.imgPosteriorRGB = imagenes.imgPosteriorRGB.CopyImage();
          
            ImagenesRemote.taxi             = imagenes.taxi;
            ImagenesRemote.TimeStamp        = imagenes.TimeStamp;
            ImagenesRemote.Nombre_variable  = imagenes.Nombre_variable;
            ImagenesRemote.IDproducto         = imagenes.IDproducto;
            ImagenesRemote.parametros.sParam1 = imagenes.parametros.sParam1;
            ImagenesRemote.parametros.fParam1 = imagenes.parametros.fParam1;
            ImagenesRemote.parametros.fParam2 = imagenes.parametros.fParam2;
            ImagenesRemote.parametros.fParam3 = imagenes.parametros.fParam3;
            ImagenesRemote.parametros.fParam4 = imagenes.parametros.fParam4;
            ImagenesRemote.parametros.iParam1 = imagenes.parametros.iParam1;
            ImagenesRemote.parametros.iParam2 = imagenes.parametros.iParam2;
            ImagenesRemote.parametros.iParam3 = imagenes.parametros.iParam3;
            ImagenesRemote.parametros.iParam4 = imagenes.parametros.iParam4;



            //guardo en una pila las imagenes a enviar
            if (listimagenestosend == null)
                listimagenestosend = new PilaImagenesRemote();


            listimagenestosend.Apilar(ImagenesRemote);

            //si el Hilo no está en marcha lo arranca
            if(HiloenvioComm == null ||  HiloenvioComm.IsAlive == false)
            {
                HiloenvioComm = new Thread(() => EnviarImagenesTexturaClienteThraed());
                HiloenvioComm.Priority = ThreadPriority.Highest;
                HiloenvioComm.IsBackground = true;
                HiloenvioComm.Start();
                
            }
           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imagenes"></param>
        public void EnviarImagenesTexturaClienteThraed(ImagesToProcessRemote imagenes)
        {
            if (imagenes == null || proxy == null)
                return;
   
                ServiceLibraryClient.DataContract1 parametros = new ServiceLibraryClient.DataContract1();
                parametros.ImagenesRemote                   = new ImagesToProcessRemote();
                parametros.FirstName                        = "pepito1";
                parametros.ImagenesRemote.img1RGB           = imagenes.img1RGB;
                parametros.ImagenesRemote.img2RGB           = imagenes.img2RGB;
                parametros.ImagenesRemote.img3RGB           = imagenes.img3RGB;
                parametros.ImagenesRemote.img1NIR           = imagenes.img1NIR;
                parametros.ImagenesRemote.img2NIR           = imagenes.img2NIR;
                parametros.ImagenesRemote.img3NIR           = imagenes.img3NIR;
                parametros.ImagenesRemote.imgAnteriorRGB    = imagenes.imgAnteriorRGB;
                parametros.ImagenesRemote.imgPosteriorRGB   = imagenes.imgPosteriorRGB;
                parametros.ImagenesRemote.taxi              = imagenes.taxi;
                parametros.ImagenesRemote.TimeStamp         = imagenes.TimeStamp;
                parametros.ImagenesRemote.Nombre_variable   = imagenes.Nombre_variable;
                parametros.ImagenesRemote.IDproducto        = imagenes.IDproducto;

                parametros.ImagenesRemote.parametros.sParam1 = imagenes.parametros.sParam1;
                parametros.ImagenesRemote.parametros.fParam1 = imagenes.parametros.fParam1;
                parametros.ImagenesRemote.parametros.fParam2 = imagenes.parametros.fParam2;
                parametros.ImagenesRemote.parametros.fParam3 = imagenes.parametros.fParam3;
                parametros.ImagenesRemote.parametros.fParam4 = imagenes.parametros.fParam4;
                parametros.ImagenesRemote.parametros.iParam1 = imagenes.parametros.iParam1;
                parametros.ImagenesRemote.parametros.iParam2 = imagenes.parametros.iParam2;
                parametros.ImagenesRemote.parametros.iParam3 = imagenes.parametros.iParam3;
                parametros.ImagenesRemote.parametros.iParam4 = imagenes.parametros.iParam4;
             


            try
            {
                    string outputtextRemote;
                    DateTime Tinicialremote     = DateTime.Now;
                    proxy.MyOperation2(parametros);
                    DateTime Tfinalremote       = DateTime.Now;
                    TimeSpan timeremote;
                    timeremote                  = Tfinalremote - Tinicialremote;
                    outputtextRemote = "Tiempo de envio de paramatros: " + timeremote.TotalMilliseconds.ToString();
                    WriteConsole(outputtextRemote, true);
                   
                }
                catch (Exception ex1)
                {
                string textoout = ex1.ToString();
                Append("Message from server: " + "Reiniciando conexion");                             
                    try
                    {
                        clientChannel.Abort();
                        OpenClienteWCF();
                        DateTime Tinicialremote = DateTime.Now;
                        proxy.MyOperation2(parametros);
                        DateTime Tfinalremote   = DateTime.Now;
                        TimeSpan timeremote;
                        timeremote              = Tfinalremote - Tinicialremote;
                        string outputtextRemote;
                        outputtextRemote = "Tiempo de envio de paramatros: " + timeremote.TotalMilliseconds.ToString();
                            WriteConsole(outputtextRemote, true);
                    }
                    catch (Exception ex)
                    {
                        string txt = ex.Message;
                        Append("Message from server: " + "No hay servidor conectado");
                    }

            }
            
            parametros.ImagenesRemote = null;
            //llamada a la eluminacion de estructuras
            //GC.Collect();
        }
       /// <summary>
       /// 
       /// </summary>
        public void EnviarImagenesTexturaClienteThraed()
        {

            while (Status != csr.modules.CSRModuleStatus.Closing && Status != csr.modules.CSRModuleStatus.Closed)
            {
              
                if (stopFlag.WaitOne(5, true))
                {
                    break;
                }

                ImagesToProcessRemote ImagenesRemote = new ImagesToProcessRemote();
                ImagenesRemote = listimagenestosend.Desapilar();
                if(ImagenesRemote != null && proxy != null)
                {
                    //si el Hilo no está en marcha lo arranca
                    Thread HiloenvioCommint = new Thread(() => EnviarImagenesTexturaClienteThraed(ImagenesRemote));
                    HiloenvioCommint.Priority = ThreadPriority.Highest;
                    HiloenvioCommint.IsBackground = true;
                    HiloenvioCommint.Start();
                }
        
            }     
        }
        /// <summary>
        /// initalizes the server threat and the client to start the communication between modules
        /// </summary>
        public void IniciarServidorWCF(List<ServerInterface> InServerInterfaceList )
        {

            ServerInterfaceList = InServerInterfaceList;       
            thServer = new System.Threading.Thread(IniciarThreadServidorWCF);
            thServer.IsBackground = true;
            thServer.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ImagesToProcess DesapilarImagentextura()
        {                
            if (service == null || service.listimagenestoprocess == null)
                return (null);

            if (service.listimagenestoprocess.m_pila.Count == 0)
                return (null);
            
            ImagesToProcess imagenes    = new ImagesToProcess();
           
            lock(m_lock)
            { 
            ImagesToProcessRemote imagenesRemote = new ImagesToProcessRemote();
            imagenesRemote          = service.listimagenestoprocess.Desapilar();
            imagenes.img1NIR        = imagenesRemote.img1NIR;
            imagenes.img2NIR        = imagenesRemote.img2NIR;
            imagenes.img3NIR        = imagenesRemote.img3NIR;
            imagenes.img1RGB        = imagenesRemote.img1RGB;
            imagenes.img2RGB        = imagenesRemote.img2RGB;
            imagenes.img3RGB        = imagenesRemote.img3RGB;
            imagenes.imgAnteriorRGB = imagenesRemote.imgAnteriorRGB;
            imagenes.imgPosteriorRGB = imagenesRemote.imgPosteriorRGB;
            imagenes.Nombre_variable = imagenesRemote.Nombre_variable;
            imagenes.taxi           = imagenesRemote.taxi;
            imagenes.Linea          = imagenesRemote.Linea;
            imagenes.IDproducto     = imagenesRemote.IDproducto;

            imagenes.parametros.sParam1 = imagenesRemote.parametros.sParam1;
            imagenes.parametros.fParam1 = imagenesRemote.parametros.fParam1;
            imagenes.parametros.fParam2 = imagenesRemote.parametros.fParam2;
            imagenes.parametros.fParam3 = imagenesRemote.parametros.fParam3;
            imagenes.parametros.fParam4 = imagenesRemote.parametros.fParam4;
            imagenes.parametros.iParam1 = imagenesRemote.parametros.iParam1;
            imagenes.parametros.iParam2 = imagenesRemote.parametros.iParam2;
            imagenes.parametros.iParam3 = imagenesRemote.parametros.iParam3;
            imagenes.parametros.iParam4 = imagenesRemote.parametros.iParam4;
          
                imagenesRemote = null;
            }                  
            return (imagenes);                    
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool clientActive()
        {
            bool active = false;
            if (this.proxy != null)
                active = true;
            return active;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void  ApilarImagentextura(ImagesToProcess imagenes)
        {
            if (service == null)
                return;

            if (service.listimagenestoprocess == null)
                service.listimagenestoprocess = new PilaImagenesRemote();

            lock (m_lock)
            {
                ImagesToProcessRemote imagenesRemote = new ImagesToProcessRemote();
                imagenesRemote.img1NIR          = imagenes.img1NIR;
                imagenesRemote.img2NIR          = imagenes.img2NIR;
                imagenesRemote.img3NIR          = imagenes.img3NIR;
                imagenesRemote.img1RGB          = imagenes.img1RGB;
                imagenesRemote.img2RGB          = imagenes.img2RGB;
                imagenesRemote.img3RGB          = imagenes.img3RGB;
                imagenesRemote.imgAnteriorRGB   = imagenes.imgAnteriorRGB;
                imagenesRemote.imgPosteriorRGB  = imagenes.imgPosteriorRGB;
                imagenesRemote.Nombre_variable  = imagenes.Nombre_variable;
                imagenesRemote.taxi             = imagenes.taxi;
                imagenesRemote.Linea            = imagenes.Linea;
                imagenesRemote.IDproducto       = imagenes.IDproducto;
                imagenesRemote.parametros.sParam1 = imagenes.parametros.sParam1;
                imagenesRemote.parametros.fParam1 = imagenes.parametros.fParam1;
                imagenesRemote.parametros.fParam2 = imagenes.parametros.fParam2;
                imagenesRemote.parametros.fParam3 = imagenes.parametros.fParam3;
                imagenesRemote.parametros.fParam4 = imagenes.parametros.fParam4;
                imagenesRemote.parametros.iParam1 = imagenes.parametros.iParam1;
                imagenesRemote.parametros.iParam2 = imagenes.parametros.iParam2;
                imagenesRemote.parametros.iParam3 = imagenes.parametros.iParam3;
                imagenesRemote.parametros.iParam4 = imagenes.parametros.iParam4;
              

                service.listimagenestoprocess.Apilar(imagenesRemote);
            }        
        }
        /// <summary>
        /// 
        /// </summary>
        public void IniciarThreadServidorWCF()
        {
            try
            {
              
                string urlServiceItem   = "";
                int num_servers         = ServerInterfaceList.Count();             
                service                 = new ServiceLibrary.service1();   // Instruct the ServiceHost that the type that is used is a ServiceLibrary.service1
                if (num_servers > 0)
                {
                    urlService                  = new List<string>();
                    hostlist                    = new List<ServiceHost>();
                    for (int index = 0; index < num_servers; index++)
                    {
                        urlServiceItem =  "net.tcp://" + ServerInterfaceList[index]._ipAddress.ToString() + ":" + ServerInterfaceList[index]._port + "/WCFService";
                        urlService.Add(urlServiceItem);
                        //inicializa toda la clase para poder recivir los parametros
                        ServiceHost hostItem = new ServiceHost(service);
                        hostItem.Opening    += new EventHandler(host_Opening);
                        hostItem.Opened     += new EventHandler(host_Opened);
                        hostItem.Closing    += new EventHandler(host_Closing);
                        hostItem.Closed     += new EventHandler(host_Closed);
                        hostlist.Add(hostItem);
                        // The binding is where we can choose what transport layer we want to use. HTTP, TCP ect.
                        NetTcpBinding tcpBindingserv                            = new NetTcpBinding();
                        tcpBindingserv.TransactionFlow                          = false;
                        tcpBindingserv.Security.Transport.ProtectionLevel       = System.Net.Security.ProtectionLevel.EncryptAndSign;
                        tcpBindingserv.Security.Transport.ClientCredentialType  = TcpClientCredentialType.Windows;
                        tcpBindingserv.Security.Mode                            = SecurityMode.None; // <- Very crucial
                        tcpBindingserv.MaxReceivedMessageSize                   = 2147483647;
                        tcpBindingserv.MaxBufferSize                            = 2147483647;
                        tcpBindingserv.MaxBufferPoolSize                        = 524288;
                        tcpBindingserv.TransferMode                             = TransferMode.Streamed;
                        tcpBindingserv.ReaderQuotas.MaxArrayLength              = 2147483647;
                        tcpBindingserv.PortSharingEnabled                       = true;
                        hostlist[index].AddServiceEndpoint(typeof(ServiceLibrary.IService1), tcpBindingserv, urlService[index]);

                    }
                    // A channel to describe the service. Used with the proxy scvutil.exe tool
                    ServiceMetadataBehavior metadataBehaviorserv;
                    metadataBehaviorserv = hostlist[0].Description.Behaviors.Find<ServiceMetadataBehavior>();
                    if (metadataBehaviorserv == null)
                    {
                        // This is how I create the proxy object that is generated via the svcutil.exe tool
                        metadataBehaviorserv                = new ServiceMetadataBehavior();
                        metadataBehaviorserv.HttpGetUrl     = new Uri("http://" + ServerInterfaceList[0]._ipAddress.ToString() + ":8002/WCFService");
                        metadataBehaviorserv.HttpGetEnabled = true;
                        metadataBehaviorserv.ToString();
                        hostlist[0].Description.Behaviors.Add(metadataBehaviorserv);
                        urlMeta                             = metadataBehaviorserv.HttpGetUrl.ToString();
                    }
                    for (int index = 0; index < num_servers; index++)
                    {
                        try
                        {
                            hostlist[index].Open();
                        }
                        catch
                        {
                            WriteConsole("Error can not open the server", true);
                        }
                       
                    }

                }

                //espera a que lo pare para salir del hilo
                while (Status != csr.modules.CSRModuleStatus.Closing && Status != csr.modules.CSRModuleStatus.Closed)
                {     
                    if (stopFlag.WaitOne(5, true))
                    {
                        service = null;
                        break;
                    }
                     //to do want it is needed                  
                }
              
            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.StackTrace);
                WriteConsole(ex1.StackTrace, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        private void Append(string str)
        {
            if(WindowForm != null)
            ((WCFForm)WindowForm).ActualizaPantalla(str);
        }

        private void host_Opening(object sender, EventArgs e)
        {
            ServiceHost Servicehostitem = (ServiceHost)sender;
            string text = "Service opening ... Stand by: " + Servicehostitem.Description.Endpoints[0].Address.ToString();
            Append(text);
            WriteConsole(text, true);
            
        }

        private void host_Closed(object sender, EventArgs e)
        {

            ServiceHost Servicehostitem = (ServiceHost)sender;
            string text = "Service closed: " + Servicehostitem.Description.Endpoints[0].Address.ToString();
            Append(text);
            WriteConsole(text, true);
         
        }

        private void host_Closing(object sender, EventArgs e)
        {
            ServiceHost Servicehostitem = (ServiceHost)sender;
            string text = "Service closing ... stand by: " + Servicehostitem.Description.Endpoints[0].Address.ToString();
            Append(text);
            WriteConsole(text, true);
                 
        }

        private void host_Opened(object sender, EventArgs e)
        {

            ServiceHost Servicehostitem = (ServiceHost) sender;
            string text = "Service opened: " +  Servicehostitem.Description.Endpoints[0].Address.ToString();
            Append(text);
            WriteConsole(text, true);
        }
        /// <summary>
        /// 
        /// </summary>
        private void TerminarServidorWCF()
        {
            stopFlag.Set();
            if(thServer != null)
                thServer.Join();
            thServer = null;
            try
            {
                int num_servers = hostlist.Count();
                for (int index = 0; index < num_servers; index++)
                {
                    try
                    {
                        hostlist[index].Close();
                    }
                    catch
                    {
                        WriteConsole("Error can not close the server", true);
                    }
                  
                }
            }
            catch
            { }         
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="IP_direccion"></param>
        public void IniciarClienteWCF(ServerInterface InterfaceClient)
        {
            string IP_direccion         = InterfaceClient._ipAddress.ToString();
            string puerto               = InterfaceClient._port.ToString();
            if (IP_direccion != "" && puerto != ""  )
            {
                m_IP_direccion = IP_direccion;
                m_puerto = puerto;
                ipAddressCliente = IP_direccion + ":" + puerto;
                //inicia las comunicaciones
                endPointAddr                                        = "net.tcp://" + ipAddressCliente + "/WCFService";
                tcpBinding                                          = new NetTcpBinding();
                tcpBinding.TransactionFlow                          = false;
                tcpBinding.Security.Transport.ProtectionLevel       = System.Net.Security.ProtectionLevel.EncryptAndSign;
                tcpBinding.Security.Transport.ClientCredentialType  = TcpClientCredentialType.Windows;
                tcpBinding.Security.Mode                            = SecurityMode.None;
                tcpBinding.MaxReceivedMessageSize                   = 2147483647;
                tcpBinding.MaxBufferSize                            = 2147483647;
                tcpBinding.MaxBufferPoolSize                        = 524288;
                tcpBinding.TransferMode                             = TransferMode.Streamed;
                tcpBinding.ReaderQuotas.MaxArrayLength              = 2147483647;
                tcpBinding.PortSharingEnabled                       = true; 
                endpointAddress                                     = new EndpointAddress(endPointAddr);         
                clientChannel                                       = new ChannelFactory<ServiceLibraryClient.IService1>(tcpBinding, endpointAddress);
                proxy                                               = clientChannel.CreateChannel();
                string text                                         = "Client conected to: " + endPointAddr;
                Append(text);
                WriteConsole(text, true);
                //inicia el proceso de evio de las imagenes capturadas              
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IP_direccion"></param>
        public void OpenClienteWCF()
        {
            try
            {
                if (clientChannel.State != CommunicationState.Opened)
                {

                    proxy = null;
                    clientChannel = null;
                    GC.Collect();
                    clientChannel = new ChannelFactory<ServiceLibraryClient.IService1>(tcpBinding, endpointAddress);
                    proxy = clientChannel.CreateChannel();
                }
            }
            catch 
            { }
           
               
        }
        ////////
    }

    public class ServerInterface
    {      
        public IPAddress _ipAddress = IPAddress.None;   
        public string    _port      = "";
    }
    //////////////////////////////////////////////////////////////
    public delegate void InitServer(List<ServerInterface> ServerInterfaceList);
    public delegate void StopServer();
    public delegate void InitClient(ServerInterface InterfaceClient);

    //////////////////////////////////////////////////////////////
    public class ModuleDelegates
    {
        public InitServer initServer;
        public StopServer stopServer;
        public InitClient initClient;
        //////////////////////////////////////////////////////////////
        public ModuleDelegates(InitServer pinitServer, StopServer pstopServer, InitClient pinitClient)
        {
            initServer = pinitServer;
            stopServer = pstopServer;
            initClient = pinitClient;
        }
        //////////////////////////////////////////////////////////////
    }

  }



 