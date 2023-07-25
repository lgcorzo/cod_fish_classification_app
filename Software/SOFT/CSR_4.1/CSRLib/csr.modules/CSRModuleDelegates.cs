using System;
using System.Collections.Generic;
using System.Drawing;

namespace csr.modules
{
    //////////////////////////////////////////////////////////////
    public delegate bool SendMessageDelegate(string origin, string destiny, string message);
    public delegate Dictionary<string,string> GetAvailableModuleNamesDelegate();
    public delegate Dictionary<string, CSRModule> GetModule();
    public delegate void LogDelegate(string module_id, string message);
    public delegate void WriteConsoleDelegate(string module_id, string message, bool write_to_log = false);
    public delegate void NotifyDelegate(string module_id, string message, bool writeInLog = false, bool isModal = true);
    public delegate void ErrorDelegate(string module_id, string message, bool writeInLog = false, bool isModal = true);
    public delegate void ExecutionTimeDelegate(string id, string message, DateTime time);
    public delegate void SetGlobalParameterDelegate(string parameter, object value);
    public delegate object GetGlobalParameterDelegate(string parameter);
    public delegate bool GoToModuleDelegate(string moduleId);
    public delegate bool ButtonColorDelegate(string moduleId, Color? color, bool isPermanent);

    public delegate void ModuleStatusChangedDelegate(string module_id, CSRModuleStatus status);

    //////////////////////////////////////////////////////////////
    public class CSRModuleDelegates
    {
        public SendMessageDelegate              SendMessageFunction;
        public GetAvailableModuleNamesDelegate  GetAvailableModulesFunction;
        public GetModule                        GetModule;
        public LogDelegate                      LogFunction;
        public WriteConsoleDelegate             WriteConsoleFunction;
        public NotifyDelegate                   NotifyFunction;
        public ErrorDelegate                    ErrorFunction;
        public ExecutionTimeDelegate            ExecutionTimeFunction;
        public SetGlobalParameterDelegate       SetGlobalParameterFunction;
        public GetGlobalParameterDelegate       GetGlobalParameterFunction;
        public GoToModuleDelegate               GoToModuleFunction;
        public ButtonColorDelegate              ButtonColorFunction;

        //////////////////////////////////////////////////////////////
        public CSRModuleDelegates(  SendMessageDelegate pSendMessageFunction,
                                    GetAvailableModuleNamesDelegate pGetAvailableModulesFunction,
                                    GetModule pGetModule,
                                    LogDelegate pLogFunction,
                                    WriteConsoleDelegate pWriteConsoleFunction,
                                    NotifyDelegate pNotifyFunction,
                                    ErrorDelegate pErrorFunction,
                                    ExecutionTimeDelegate pExecutionTimeFunction,
                                    SetGlobalParameterDelegate pSetGlobalParameterFunction,
                                    GetGlobalParameterDelegate pGetGlobalParameterFunction,
                                    GoToModuleDelegate pGoToModuleFunction,
                                    ButtonColorDelegate pButtonColoFunction)
        {
            SendMessageFunction         = pSendMessageFunction;
            GetAvailableModulesFunction = pGetAvailableModulesFunction;
            GetModule                   = pGetModule;
            LogFunction                 = pLogFunction;
            WriteConsoleFunction        = pWriteConsoleFunction;
            NotifyFunction              = pNotifyFunction;
            ErrorFunction               = pErrorFunction;
            ExecutionTimeFunction       = pExecutionTimeFunction;
            SetGlobalParameterFunction  = pSetGlobalParameterFunction;
            GetGlobalParameterFunction  = pGetGlobalParameterFunction;
            GoToModuleFunction          = pGoToModuleFunction;
            ButtonColorFunction         = pButtonColoFunction;
        }

        //////////////////////////////////////////////////////////////
    }
}
