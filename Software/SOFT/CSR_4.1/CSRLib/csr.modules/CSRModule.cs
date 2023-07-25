using System;
using System.Collections.Generic;
using System.Drawing;

namespace csr.modules
{
    public class CSRModule
    {
        //////////////////////////////////////////////////////////////
        private SendMessageDelegate             SendMessageFunction;
        private GetAvailableModuleNamesDelegate GetAvailableModulesFunction;
        private GetModule                       GetModuleFunction;
        private LogDelegate                     LogFunction;
        private WriteConsoleDelegate            WriteConsoleFunction;
        private ErrorDelegate                   ErrorFunction;
        private NotifyDelegate                  NotifyFunction;
        private ExecutionTimeDelegate           ExecutionTimeFunction;  
        private SetGlobalParameterDelegate      SetGlobalParameterFunction;
        private GetGlobalParameterDelegate      GetGlobalParameterFunction;
        private GoToModuleDelegate              GoToModuleFunction;
        private ButtonColorDelegate             ButtonColorFunction;

        public event ModuleStatusChangedDelegate StatusChanged;

        //////////////////////////////////////////////////////////////
        public string           Id      { get; set; }
        public string           Name    { get; set; }
        public CSRModuleStatus  Status  {
            //
            get
            {
                return m_status;
            }
            //
            set
            {
                m_status = value;
                if( StatusChanged != null )
                    StatusChanged(Id, m_status);
            }
            //
        }

        private CSRModuleStatus m_status;

        //////////////////////////////////////////////////////////////
        public CSRModule(string _id)
        {
            Id      = _id;
            Status  = CSRModuleStatus.Paused;
        }
        
        //////////////////////////////////////////////////////////////
        public bool SendMessage(string destiny, string message)
        {
            return SendMessageFunction(Id, destiny, message);
        }

        //////////////////////////////////////////////////////////////
        public void Log(string message)
        {
            LogFunction(Id, message);
        }

        //////////////////////////////////////////////////////////////
        public void WriteConsole(string message, bool write_log = false)
        {
            WriteConsoleFunction(Id, message, write_log);
        }

        //////////////////////////////////////////////////////////////
        public void WriteExecutionTime(string message, DateTime time = new DateTime())
        {
            if (time == DateTime.MinValue)
                time = DateTime.Now;
            ExecutionTimeFunction(Id, message, time);
        }

        //////////////////////////////////////////////////////////////
        public void Notify(string message, bool writeInLog = false, bool isModal = true)
        {
            NotifyFunction(Id, message, writeInLog, isModal);
        }

        //////////////////////////////////////////////////////////////
        public void Error(string message, bool writeInLog = false, bool isModal = true)
        {
            ErrorFunction(Id, message, writeInLog, isModal);
            m_status = CSRModuleStatus.Error;
        }

        //////////////////////////////////////////////////////////////
        public Dictionary<string,string> GetAvailableModules()
        {
            return GetAvailableModulesFunction();
        }

        //////////////////////////////////////////////////////////////
        public CSRModule GetModule(string ModuleId)
        {
            if(!GetModuleFunction().ContainsKey(ModuleId))
                return null;

            return GetModuleFunction()[ModuleId];
        }

        //////////////////////////////////////////////////////////////
        public void SetGlobalParameter(string parameter_name, object value)
        {
            SetGlobalParameterFunction(parameter_name, value);
        }

        //////////////////////////////////////////////////////////////
        public object GetGlobalParameter(string parameter_name)
        {
            return GetGlobalParameterFunction(parameter_name);
        }

        //////////////////////////////////////////////////////////////
        public bool GoToModule(string moduleId)
        {
            return GoToModuleFunction(moduleId);
        }

        //////////////////////////////////////////////////////////////
        public bool ButtonColor(string moduleId, Color? color, bool isPermanent)
        {
            return ButtonColorFunction(moduleId, color, isPermanent);
        }

        //////////////////////////////////////////////////////////////
        public virtual bool Init()
        {
            // override in modules
            Status = CSRModuleStatus.Running;
            return true;
        }

        //////////////////////////////////////////////////////////////
        public virtual bool Destroy()
        {
            // override in modules
            Status = CSRModuleStatus.Closed;
            return true;
        }
        //////////////////////////////////////////////////////////////
        public virtual void HandleMessages(com.CSRMessage message)
        {
            // override in modules
        }

        //////////////////////////////////////////////////////////////
        public void AppendDelegates(csr.modules.CSRModuleDelegates module_delegates)
        {
            this.SendMessageFunction            = module_delegates.SendMessageFunction;
            this.GetAvailableModulesFunction    = module_delegates.GetAvailableModulesFunction;
            this.GetModuleFunction              = module_delegates.GetModule;
            this.LogFunction                    = module_delegates.LogFunction;
            this.WriteConsoleFunction           = module_delegates.WriteConsoleFunction;
            this.NotifyFunction                 = module_delegates.NotifyFunction;
            this.ErrorFunction                  = module_delegates.ErrorFunction;
            this.ExecutionTimeFunction          = module_delegates.ExecutionTimeFunction;
            this.SetGlobalParameterFunction     = module_delegates.SetGlobalParameterFunction;
            this.GetGlobalParameterFunction     = module_delegates.GetGlobalParameterFunction;
            this.GoToModuleFunction             = module_delegates.GoToModuleFunction;
            this.ButtonColorFunction            = module_delegates.ButtonColorFunction;
        }
        //////////////////////////////////////////////////////////////
    }
}
