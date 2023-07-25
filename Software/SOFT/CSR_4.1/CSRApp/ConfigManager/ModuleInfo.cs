using System.Runtime.Serialization;

namespace CSRApp.config
{

    [DataContractAttribute()]
    [KnownType(typeof(ModuleInfoLocal))]
    [KnownType(typeof(ModuleInfoRemote))]
    public class ModuleInfo
    {
        ///////////////////////////////////////////////////////////
        [DataMember()]
        public string Id { get; set; }
        [DataMember()]
        public string Name { get; set; }
        
        ///////////////////////////////////////////////////////////
        public ModuleInfo()
        {
            Id      = "undefined";
            Name    = "undefined";
        }

        ///////////////////////////////////////////////////////////
        protected ModuleInfo(string _id, string _name)
        {
            Id      = _id;
            Name    = _name;
        }

        ///////////////////////////////////////////////////////////
    }
}
