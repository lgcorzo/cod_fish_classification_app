using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using TwinCAT.Ads;

namespace TwincatModule
{

    public delegate bool TBlockDisposeDelegate();
    public class TCBlock
    {
        private TwinCatCommunication        parent;
        private string                      m_filename = "";
        public TcAdsClient                  tcAds;
        private IO_Parameters               IOparameters;
        private int                         String_size = 80;
        public Dictionary<int, string>      notifications;
        private AdsStream                   stream;
        private BinaryReader                reader;
      

        private int m_id;

        public TCBlock(int id, Dictionary<string, TwincatVariable> dict, string filename, TwinCatCommunication comm)
        {
            m_id = id;
            m_filename = filename.Insert(filename.LastIndexOf('.'), id.ToString());
            parent = comm;
            tcAds                   = new TcAdsClient();
            notifications           = new Dictionary<int, string>();
            IOparameters            = new IO_Parameters(dict);
          
        }

        public TCBlock(int id, string fullFilename, TwinCatCommunication comm)
        {
            string path                 = Path.GetDirectoryName(fullFilename) + "\\";
            string filenameWithoutExt   = Path.GetFileNameWithoutExtension(fullFilename);
            string filenameExt          = Path.GetExtension(fullFilename);      
            m_id                        = id;
            m_filename                  = path + filenameWithoutExt + id + filenameExt;
            parent                      = comm;    
            notifications               = new Dictionary<int, string>();
            IOparameters                = new IO_Parameters();
            tcAds                       = new TcAdsClient();
            LoadIOConf();
        }

        public bool Init()
        {
            
            return true;
        }

        public bool Connect(string net_id, int port)
        {
            try
            {                  
                tcAds.Connect(net_id, port);
                tcAds.AdsNotificationError += new AdsNotificationErrorEventHandler(adsClient_AdsNotificationError);          
      

                return true;
            }
            catch(Exception ex)
            {
                string textoout = ex.ToString();
                return false;
            }
        }

        public bool Disconnect()
        {
            try
            {                     
                tcAds.Disconnect();              
                return true;
            }
            catch(Exception ex)
            {
                string textout = ex.ToString();
                return false;
            }
        }

        public bool SaveIOConf()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<List<IO_Parameter>>));

                List<List<IO_Parameter>> listIO     = new List<List<IO_Parameter>>();
                List<IO_Parameter> listIn           = new List<IO_Parameter>();
                List<IO_Parameter> listOut  = new List<IO_Parameter>();

                foreach (IO_Parameter IO in IOparameters.In.Values)
                    listIn.Add(IO);

                foreach (IO_Parameter IO in IOparameters.Out.Values)
                    listOut.Add(IO);

                listIO.Add(listIn);
                listIO.Add(listOut);

                FileStream filestream = new FileStream(this.m_filename, FileMode.Create);

                try
                {
                    serializer.Serialize(filestream, listIO);
                }
                catch (Exception e1)
                {
                    filestream.Close();
                    throw (e1);
                }

                filestream.Close();

                return true;
            }
            catch (Exception e)
            {
                string textoout = e.ToString();
                return false;
            }
        }
        ///////////////////////////////////////////////////////////////////////////

        public bool LoadIOConf(){
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<List<IO_Parameter>>));
                FileStream filestream = new FileStream(this.m_filename, FileMode.Open);
                List<List<IO_Parameter>> listIO;
                try
                {
                    listIO = (List<List<IO_Parameter>>)serializer.Deserialize(filestream);
                }
                catch (Exception e1)
                {
                    filestream.Close();
                    throw (e1);
                }

                filestream.Close();

                for (int i = 0; i < listIO[0].Count; i++){
                    IO_Parameter IO = listIO[0][i];
                    if(parent.cumpleFiltrado(ref IO))
                        IOparameters.In.Add(IO.Name, IO);
                }


                for (int i = 0; i < listIO[1].Count; i++)
                {
                    IO_Parameter IO = listIO[1][i];
                    if (parent.cumpleFiltrado(ref IO))
                        IOparameters.Out.Add(IO.Name, IO);
                }
                    

                return true;
            }
            catch(Exception e) 
            {
                string textoout = e.ToString();
                //parent.CSRMod.Error("Error al cargar el bloque de comunicacion número " + m_id + ". Avisad a Roboconcept", true);
                return false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        private void adsClient_AdsNotificationError(object sender, AdsNotificationErrorEventArgs e)
        {
            //Error que aparece al cerrar el programa, por que se esta cerrando el programa.
            if (parent.CSRMod.Status != csr.modules.CSRModuleStatus.Closing && parent.CSRMod.Status != csr.modules.CSRModuleStatus.Closed)
            {
                //parent.CSRMod.Error("Error en las notificaciones del ADS. " + e.Exception.Message, true, false);
            }
          
        }


        ///////////////////////////////////////////////////////////////////////////
        public void SetWriteDelegate(IO_Parameter.TwincatWriteHandler WriteTwincatValue)
        {
            IOparameters.SetWriteDelegate(WriteTwincatValue);
        }
        
        ///////////////////////////////////////////////////////////////////////////

        public void AñadirRedireccion(){
            int tamaño = 0;

            foreach (IO_Parameter IO in IOparameters.Out.Values)
            {
                Type myType1 = Type.GetType(IO.TypeStr);
                int size = 0;
                if (IO.TypeStr == "System.String")
                    size = String_size;
                else
                    size = Marshal.SizeOf(myType1);
                tamaño += size;
            }

            stream = new AdsStream(tamaño);
            reader = new BinaryReader(stream, Encoding.ASCII);
                
            tamaño = 0;
            try
            {
                foreach (IO_Parameter IO in IOparameters.Out.Values)
                {

                    int size = 0;
                    if (IO.TypeStr == "System.String")
                        size = String_size;
                    else
                        size = Marshal.SizeOf(Type.GetType(IO.TypeStr));
                    int Handelnotif = tcAds.AddDeviceNotification(IO.Name, stream, tamaño, size, AdsTransMode.OnChange, 1, 1, IO.Value);
                    notifications.Add(Handelnotif, IO.Name);

                }
            }
            catch
            {
                //tcAds.AdsNotification += new AdsNotificationEventHandler(OnNotification);
            }

            tcAds.AdsNotification += new AdsNotificationEventHandler(OnNotification);
        }

        public void ReAñadirRedireccion()
        {
            int tamaño = 0;

            foreach (IO_Parameter IO in IOparameters.Out.Values)
            {
                Type myType1 = Type.GetType(IO.TypeStr);
                int size = 0;
                if (IO.TypeStr == "System.String")
                    size = String_size;
                else
                    size = Marshal.SizeOf(myType1);
                tamaño += size;
            }

            stream = new AdsStream(tamaño);
            reader = new BinaryReader(stream, Encoding.ASCII);
            tcAds.AdsNotification -= new AdsNotificationEventHandler(OnNotification);
            tamaño = 0;
            try        
            {
              
                int contador_notoficacion = 1;
                foreach (IO_Parameter IO in IOparameters.Out.Values)
                {

                    int size = 0;
                    if (IO.TypeStr == "System.String")
                        size = String_size;
                    else
                        size = Marshal.SizeOf(Type.GetType(IO.TypeStr));
                    string var = notifications[contador_notoficacion];
                   // tcAds.DeleteDeviceNotification(contador_notoficacion);
                    tcAds.AddDeviceNotification(IO.Name, stream, tamaño, size, AdsTransMode.OnChange, 1, 1, IO.Value);
                    contador_notoficacion++;
                }
            }
            catch (Exception ex)
            {
                string textoout = ex.ToString();

            }

            tcAds.AdsNotification += new AdsNotificationEventHandler(OnNotification);
        }


        ///////////////////////////////////////////////////////////////////////////


        private void OnNotification(object sender, AdsNotificationEventArgs e)
        {   
            
            string var = notifications[e.NotificationHandle];

            Type tipo;
            if (IOparameters.In.ContainsKey(var))
                tipo = Type.GetType(IOparameters.In[var].TypeStr);
            else if (IOparameters.Out.ContainsKey(var))
                tipo = Type.GetType(IOparameters.Out[var].TypeStr);
            else
                throw new Exception("No se encuentra la variable " + var);

            object obj = leerValor(tipo);
            IOparameters.Set(var, obj);

     
            ((TwincatModule)parent.CSRMod).IODisplayUpdate();
        }

        ///////////////////////////////////////////////////////////////////////////


        private object leerValor(Type tipo)
        {
            object result = null;
            if (tipo == typeof(UInt32))
                result = reader.ReadUInt32();
            else if (tipo == typeof(Boolean))
                result = reader.ReadBoolean();
            else if (tipo == typeof(Int64))
                result = reader.ReadInt64();
            else if (tipo == typeof(Byte))
                result = reader.ReadByte();
            else if (tipo == typeof(UInt16))
                result = reader.ReadUInt16();
            else if (tipo == typeof(float))
                result = reader.ReadSingle();
            else if (tipo == typeof(String))
            {
                char[] caharr = reader.ReadChars(String_size);
                string Texto = "";

                for (int i = 0; i < String_size; i++)
                {
                    if (caharr[i].ToString() == "/0")
                        break;
                    Texto = Texto + caharr[i].ToString();
                }
                result = Texto;

            }

            else
                parent.CSRMod.Error("Tipo inesperado: " + tipo.ToString(), true, false);
            return result;
        }

        ///////////////////////////////////////////////////////////////////////////

        public IO_Parameters IOParameters
        {
            get { return IOparameters; }
        }

        public bool setRedirections(string paramName, List<string> moduleNames)
        {
            return IOparameters.setRedirections(paramName, moduleNames);
        }

        public List<IO_Parameter> ListaCompleta
        {
            get { return IOparameters.ToList(true).Concat(IOparameters.ToList(false)).ToList(); }
        }

        public int Id
        {
            get { return m_id; }
        }
    }
}
