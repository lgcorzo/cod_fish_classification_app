using System;

namespace CSRApp.Logger
{
    public delegate void ConsoleWriteDelegate(string id, string message);
    public delegate void ModuleErrorDelegate(string id, bool hasError);
    public delegate void ModuleConnectionDelegate(string id, bool isConnected);

    public sealed class CSRStatusFunctions
    {
       //////////////////////////////////////////////////////////////
        private static volatile CSRStatusFunctions instance;
        private static object sync = new Object();
        private static ConsoleWriteDelegate console_write_function;
        private static ModuleErrorDelegate module_error_function;
        private static ModuleConnectionDelegate module_connection_function;

        //////////////////////////////////////////////////////////////
        private CSRStatusFunctions()
        {

        }

        //////////////////////////////////////////////////////////////
        public static CSRStatusFunctions Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (sync) 
                    {
                        if (instance == null)
                            instance = new CSRStatusFunctions();
                    }
                }
                return instance;
            }
        }

        //////////////////////////////////////////////////////////////
        public void SetConsoleWriteDelegate(ConsoleWriteDelegate _console_write_function)
        {
            console_write_function = _console_write_function;
        }

        //////////////////////////////////////////////////////////////
        public void SetModuleErrorDelegate(ModuleErrorDelegate _module_error_function)
        {
            module_error_function = _module_error_function;
        }

        //////////////////////////////////////////////////////////////
        public void SetModuleConnectionDelegate(ModuleConnectionDelegate _module_connection_function)
        {
            module_connection_function = _module_connection_function;
        }

        //////////////////////////////////////////////////////////////
        public void ConsoleWrite(string id, string message)
        {
            if(console_write_function != null)
                console_write_function(id, message);
        }

        //////////////////////////////////////////////////////////////

        public void SetModuleError(string id)
        {
            if(module_error_function != null)
                module_error_function(id, true);
        }

        //////////////////////////////////////////////////////////////

        public void SetModuleConnection(string id, bool isConnected)
        {
            if (module_connection_function != null)
                module_connection_function(id, isConnected);
        }

        //////////////////////////////////////////////////////////////
    }
}
