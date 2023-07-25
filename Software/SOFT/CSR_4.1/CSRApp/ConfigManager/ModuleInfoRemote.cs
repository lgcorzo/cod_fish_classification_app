using System;
using System.Runtime.Serialization;

namespace CSRApp.config
{

    [DataContractAttribute()]
    public class ModuleInfoRemote : ModuleInfo
    {
        ///////////////////////////////////////////////////////////
        [DataMember()]
        public string Filename { get; set; }
        [DataMember()]
        public string ModuleType { get; set; }

        [DataMember()]
        public string Ip { get; set; }

        ///////////////////////////////////////////////////////////
        public ModuleInfoRemote()
            : base("undefined", "undefined")
        {
            Filename = "undefined";
            ModuleType = typeof(csr.modules.CSRModule).FullName;
            Ip = "undefined";
        }
    }
}
