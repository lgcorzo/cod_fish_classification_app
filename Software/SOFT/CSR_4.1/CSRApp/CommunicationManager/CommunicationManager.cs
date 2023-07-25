using System;
using System.Collections.Generic;
using System.Linq;

namespace CSRApp.communication
{
    public class CommunicationManager : CSRAppComponent
    {
        ///////////////////////////////////////////////////////////
        private config.AppConfig current_config;
        private Dictionary<string, csr.modules.CSRModule> local_modules;

        ///////////////////////////////////////////////////////////
        public CommunicationManager(config.AppConfig _config) : base("CommunicationManager")
        {
            local_modules = new Dictionary<string, csr.modules.CSRModule>();
            current_config = _config;
        }

        ///////////////////////////////////////////////////////////
        public bool Init()
        {
            return true;
        }

        ///////////////////////////////////////////////////////////
        public bool SendMessage(string origin, string destiny, string message)
        {
            if (!current_config.Modules.ContainsKey(destiny))
                return false;

            config.ModuleInfo module_info = current_config.Modules[destiny];
            
            if (module_info == null)
                return false;

            if (module_info is config.ModuleInfoLocal)
            {
                // local message
                csr.modules.CSRModule module = local_modules[module_info.Id];
                csr.com.CSRMessage message_obj = new csr.com.CSRMessage(destiny, message);
                message_obj.OriginId = origin;
                module.HandleMessages(message_obj);
            }

            else if (module_info is config.ModuleInfoRemote)
            {
                // do remote call (not implemented)
                // can be done through two bridge tcp/ip client/server modules
                new NotImplementedException("Servicio aun no disponible");
            }

            return true;
        }

        ///////////////////////////////////////////////////////////
        public void EnableLocalModules(Dictionary<string, csr.modules.CSRModule> _local_modules)
        {
            local_modules = _local_modules;
        }

        ///////////////////////////////////////////////////////////
    }
}
