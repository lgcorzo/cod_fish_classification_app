using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRTcpClientModule
{
    public class CSRTcpClientConfig
    {
        //////////////////////////////////////////////////////////////
        public string   ServerAddr  { get; set; }
        public int      Port        { get; set; }

        //////////////////////////////////////////////////////////////
        public CSRTcpClientConfig(string _server, int _port)
        {
            ServerAddr = _server;
            Port = _port;
        }

        //////////////////////////////////////////////////////////////
    }
}
