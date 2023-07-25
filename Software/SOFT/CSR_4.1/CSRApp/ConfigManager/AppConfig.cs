using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;

namespace CSRApp.config
{
    [DataContractAttribute()]
    public class AppConfig
    {
        ///////////////////////////////////////////////////////////
        [DataMember()]
        public string AppName { get; set; }
        [DataMember()]
        public Dictionary<string, ModuleInfo> Modules { get; set; }
        [DataMember()]
        public Size AppSize;
        [DataMember()]
        public bool isMinimized;
        
        ///////////////////////////////////////////////////////////
        public AppConfig()
        {
            AppName             = "undefined";
            isMinimized         = false;
            Modules             = null;
        }

        ///////////////////////////////////////////////////////////
    }
}
