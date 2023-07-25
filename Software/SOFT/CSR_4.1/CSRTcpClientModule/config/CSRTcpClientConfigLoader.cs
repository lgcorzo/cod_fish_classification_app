using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Threading.Tasks;

namespace CSRTcpClientModule
{

    public class CSRTcpClientConfigLoader
    {
        //////////////////////////////////////////////////////////////
        private string filename;

        //////////////////////////////////////////////////////////////
        public CSRTcpClientConfigLoader(string _filename)
        {
            filename = _filename;
        }

        //////////////////////////////////////////////////////////////
        public CSRTcpClientConfig LoadConfig(string id)
        {
            string file_content = System.IO.File.ReadAllText(filename);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(file_content);
            XmlNode nodeServer  = doc.SelectSingleNode("/TcpClients/Channel[@module_id='" + id + "']/Server");
            XmlNode nodePort    = doc.SelectSingleNode("/TcpClients/Channel[@module_id='" + id + "']/Port");
            int port;
            int.TryParse(nodePort.InnerText, out port);
            return new CSRTcpClientConfig(nodeServer.InnerText, port);
        }

        //////////////////////////////////////////////////////////////
    }
}
