using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRTcpServerModule
{
    public class CSRTcpServerConfig
    {
        //////////////////////////////////////////////////////////////
        public int      Port        { get; set; }

        //////////////////////////////////////////////////////////////
        public CSRTcpServerConfig(int _port)
        {
            Port = _port;
        }

        //////////////////////////////////////////////////////////////
    }
}
