using System;
using System.Linq;
using System.Xml;

namespace CSRTcpServerModule
{
    public class CSRTcpServerConfigLoader
    {
        //////////////////////////////////////////////////////////////
        private string filename;

        //////////////////////////////////////////////////////////////
        public CSRTcpServerConfigLoader(string _filename)
        {
            filename = _filename;
        }

        //////////////////////////////////////////////////////////////
        public CSRTcpServerConfig LoadConfig(string id)
        {
            string file_content = System.IO.File.ReadAllText(filename);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(file_content);
            XmlNode nodePort    = doc.SelectSingleNode("/TcpServers/Channel[@module_id='" + id + "']/Port");
            int port;
            int.TryParse(nodePort.InnerText, out port);
            return new CSRTcpServerConfig(port);
        }

        //////////////////////////////////////////////////////////////
    }
}
