using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Net;
using HalconDotNet;

/*

    HOW TO HOST THE WCF SERVICE IN THIS LIBRARY IN ANOTHER PROJECT
    You will need to do the following things: 
    1)    Add a Host project to your solution
        a.    Right click on your solution
        b.    Select Add
        c.    Select New Project
        d.    Choose an appropriate Host project type (e.g. Console Application)
    2)    Add a new source file to your Host project
        a.    Right click on your Host project
        b.    Select Add
        c.    Select New Item
        d.    Select "Code File"
    3)    Paste the contents of the "MyServiceHost" class below into the new Code File
    4)    Add an "Application Configuration File" to your Host project
        a.    Right click on your Host project
        b.    Select Add
        c.    Select New Item
        d.    Select "Application Configuration File"
    5)    Paste the contents of the App.Config below that defines your service endoints into the new Config File
    6)    Add the code that will host, start and stop the service
        a.    Call MyServiceHost.StartService() to start the service and MyServiceHost.EndService() to end the service
    7)    Add a Reference to System.ServiceModel.dll
        a.    Right click on your Host Project
        b.    Select "Add Reference"
        c.    Select "System.ServiceModel.dll"
    8)    Add a Reference from your Host project to your Service Library project
        a.    Right click on your Host Project
        b.    Select "Add Reference"
        c.    Select the "Projects" tab
    9)    Set the Host project as the "StartUp" project for the solution
        a.    Right click on your Host Project
        b.    Select "Set as StartUp Project"

    ################# START MyServiceHost.cs #################

    using System;
    using System.ServiceModel;

    // A WCF service consists of a contract (defined below), 
    // a class which implements that interface, and configuration 
    // entries that specify behaviors and endpoints associated with 
    // that implementation (see <system.serviceModel> in your application
    // configuration file).

    internal class MyServiceHost
    {
        internal static ServiceHost myServiceHost = null;

        internal static void StartService()
        {
            //Consider putting the baseAddress in the configuration system
            //and getting it here with AppSettings
            Uri baseAddress = new Uri("http://localhost:8080/service1");

            //Instantiate new ServiceHost 
            myServiceHost = new ServiceHost(typeof(ServiceLibrary.service1), baseAddress);

            //Open myServiceHost
            myServiceHost.Open();
        }

        internal static void StopService()
        {
            //Call StopService from your shutdown logic (i.e. dispose method)
            if (myServiceHost.State != CommunicationState.Closed)
                myServiceHost.Close();
        }
    }

    ################# END MyServiceHost.cs #################
    ################# START App.config or Web.config #################

    <system.serviceModel>
    <services>
         <service name="ServiceLibrary.service1">
           <endpoint contract="ServiceLibrary.IService1" binding="wsHttpBinding"/>
         </service>
       </services>
    </system.serviceModel>

    ################# END App.config or Web.config #################

*/
namespace ServiceLibrary
{
    // You have created a class library to define and implement your WCF service.
    // You will need to add a reference to this library from another project and add 
    // the code to that project to host the service as described below.  Another way
    // to create and host a WCF service is by using the Add New Item, WCF Service 
    // template within an existing project such as a Console Application or a Windows 
    // Application.
    
    [ServiceContract()]
    public interface IService1
    {
        [OperationContract]
        string MyOperation1(string myValue);
        [OperationContract(IsOneWay = true,IsTerminating = false)]
        void MyOperation2(DataContract1 dataContractValue);
        [OperationContract]
        string HelloWorld(string str);

    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class service1 : IService1
    {
        public PilaImagenesRemote listimagenestoprocess = null;

        public string MyOperation1(string myValue)
        {
            return "Hello: " + myValue;
        }
        public void MyOperation2(DataContract1 dataContractValue)
        {
            if (listimagenestoprocess == null)
                listimagenestoprocess = new PilaImagenesRemote();
            listimagenestoprocess.Apilar(dataContractValue.ImagenesRemote);          
            return;
        }
        public string HelloWorld(string str)
        {
            IPHostEntry ips = Dns.GetHostEntry(Dns.GetHostName());
            // Select the first entry. I hope it's this maschines IP
            IPAddress _ipAddress = ips.AddressList[2];
            return _ipAddress.ToString() + "   " + str ;
        }
    }

    [DataContract]
    public class DataContract1
    {
        string                  firstName;
        string                  lastName;
        ImagesToProcessRemote   imagenesRemote = new ImagesToProcessRemote();

        [DataMember]
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
        [DataMember]
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
        [DataMember]
        public ImagesToProcessRemote ImagenesRemote
        {
            get { return imagenesRemote; }
            set {
                imagenesRemote                  = new ImagesToProcessRemote();           
                imagenesRemote.img1RGB          = value.img1RGB;                       
                imagenesRemote.img2RGB          = value.img2RGB;             
                imagenesRemote.img3RGB          = value.img3RGB;               
                imagenesRemote.img1NIR          = value.img1NIR;              
                imagenesRemote.img2NIR          = value.img2NIR;               
                imagenesRemote.img3NIR          = value.img3NIR;               
                imagenesRemote.imgAnteriorRGB   = value.imgAnteriorRGB;
                imagenesRemote.imgPosteriorRGB  = value.imgPosteriorRGB;
                imagenesRemote.Linea            = value.Linea;
                imagenesRemote.TimeStamp        = value.TimeStamp;
                imagenesRemote.taxi             = value.taxi;
                imagenesRemote.Nombre_variable  = value.Nombre_variable;
                imagenesRemote.IDproducto       = value.IDproducto;
                imagenesRemote.IDproductoCSR    = value.IDproductoCSR;
                imagenesRemote.parametros.sParam1 = value.parametros.sParam1;
                imagenesRemote.parametros.fParam1 = value.parametros.fParam1;
                imagenesRemote.parametros.fParam2 = value.parametros.fParam2;
                imagenesRemote.parametros.fParam3 = value.parametros.fParam3;
                imagenesRemote.parametros.fParam4 = value.parametros.fParam4;
                imagenesRemote.parametros.iParam1 = value.parametros.iParam1;
                imagenesRemote.parametros.iParam2 = value.parametros.iParam2;
                imagenesRemote.parametros.iParam3 = value.parametros.iParam3;
                imagenesRemote.parametros.iParam4 = value.parametros.iParam4;
               

            }
        }
    }

    public class ImagesToProcessRemote
    {
        public HImage   img1RGB = null;
        public HImage   img2RGB = null;
        public HImage   img3RGB = null;
        public HImage   img1NIR = null;
        public HImage   img2NIR = null;
        public HImage   img3NIR = null;
        public HImage   imgAnteriorRGB = null;
        public HImage   imgPosteriorRGB = null;
        public int      Linea = 0;
        public DateTime TimeStamp;
        public int      taxi = 0;
        public string   Nombre_variable = "";
        public int      IDproducto = 0;
        public int      IDproductoCSR = 0;
        public Parametros_Process parametros = new Parametros_Process();
    }

    public class Parametros_Process
    {
        public String sParam1 = "";
        public float fParam1 = 0;
        public float fParam2 = 0;
        public float fParam3 = 0;
        public float fParam4 = 0;
        public int iParam1 = 0;
        public int iParam2 = 0;
        public int iParam3 = 0;
        public int iParam4 = 0;
    }

    public class PilaImagenesRemote
    {
        object m_lock = new object();
        public Queue<ImagesToProcessRemote> m_pila = new Queue<ImagesToProcessRemote>();

        public PilaImagenesRemote()
        {
            m_pila = new Queue<ImagesToProcessRemote>();
        }

        public void Apilar(ImagesToProcessRemote imgsToProcess)
        {
            lock (m_lock)
            {
                m_pila.Enqueue(imgsToProcess);
            }
        }

        public ImagesToProcessRemote Desapilar()
        {
            ImagesToProcessRemote imgs = null;

            lock (m_lock)
            {
                if (m_pila.Count > 0)
                    imgs = m_pila.Dequeue();
            }

            return imgs;
        }

        public void Clear()
        {
            lock (m_lock)
            {
                m_pila.Clear();
            }
        }
    }

}
