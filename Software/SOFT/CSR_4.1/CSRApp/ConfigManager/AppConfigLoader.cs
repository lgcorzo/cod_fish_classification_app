using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace CSRApp.config
{
    public class AppConfigLoader
    {
        ///////////////////////////////////////////////////////////
        private string m_filename;
        private DataContractSerializer xml_serializer;

        ///////////////////////////////////////////////////////////
        public AppConfigLoader(string _filename)
        {
            m_filename = _filename;
            xml_serializer = new DataContractSerializer(typeof(AppConfig));
        }

        ///////////////////////////////////////////////////////////
        public bool Load(out AppConfig _config)
        {
            try
            {
                FileStream filestream = new FileStream(m_filename, FileMode.Open);
                _config = (AppConfig)xml_serializer.ReadObject(filestream);
                filestream.Close();
              
                return true;
            }
            catch (Exception e)
            {
                string textoOuterror = e.ToString();
                _config = null;
                return false;
            }
        }

        ///////////////////////////////////////////////////////////
        public bool Save(AppConfig _config)
        {
            try
            {
                FileStream filestream = new FileStream(m_filename, FileMode.Create);
                xml_serializer.WriteObject(filestream, _config);
                filestream.Close();
                return true;
            }
            catch (Exception e)
            {
                string textoOuterror = e.ToString();
                return false;
            }
        }

        ///////////////////////////////////////////////////////////
    }
}
