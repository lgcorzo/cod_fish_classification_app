﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AlarmModule
{
    public class WOL : UdpClient
    {
        public WOL() : base()
        {
        }
        //this is needed to send broadcast packet
        public void SetClientToBrodcastMode()
        {
           
            if (this.Active)
                this.Client.SetSocketOption(SocketOptionLevel.Socket,
                                          SocketOptionName.Broadcast, 0);
        }

        //this is needed to send broadcast packet
        public void Bind(EndPoint localEP )
        {
            try
            {
                this.Client.Bind(localEP);
            }
            catch(Exception ex)
            {
                string textoout = ex.ToString();
            }
            
            

        }
    }
}
