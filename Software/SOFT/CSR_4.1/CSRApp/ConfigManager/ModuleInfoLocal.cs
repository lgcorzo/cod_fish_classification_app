using System;
using System.Runtime.Serialization;

namespace CSRApp.config
{

    [DataContractAttribute()]
    public class ModuleInfoLocal : ModuleInfo
    {
        ///////////////////////////////////////////////////////////
        [DataMember()]
        public string Filename { get; set; }
        [DataMember()]
        public string ModuleType { get; set; }

        ///////////////////////////////////////////////////////////
        public ModuleInfoLocal()
            : base("undefined", "undefined")
        {
            Filename = "undefined";
            ModuleType = typeof(csr.modules.CSRModule).FullName;
        }
    }
}
