using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TwincatModule
{
    public class IO_Parameters
    {
        Dictionary<string, IO_Parameter> m_parametersIn;
        Dictionary<string, IO_Parameter> m_parametersOut;

        public IO_Parameters()
        {
            this.m_parametersIn = new Dictionary<string, IO_Parameter>();
            this.m_parametersOut = new Dictionary<string, IO_Parameter>();
        }

        public IO_Parameters(Dictionary<string, TwincatVariable> variables)
        {
            this.m_parametersIn = new Dictionary<string, IO_Parameter>();
            this.m_parametersOut = new Dictionary<string, IO_Parameter>();
            foreach (TwincatVariable tv in variables.Values)
            {
                if ((tv.Symbol.Name.Contains("_in_")) || (tv.Symbol.Name.Contains("_IN_")) || (tv.Symbol.Name.Contains("_In_")))
                    this.m_parametersIn.Add(tv.Symbol.Name, new IO_Parameter(tv.Symbol.Name, tv.Type));
                else
                    this.m_parametersOut.Add(tv.Symbol.Name, new IO_Parameter(tv.Symbol.Name, tv.Type));
            }
        }

        public IO_Parameters(Dictionary<string, TwincatVariable> variables, IO_Parameter.TwincatWriteHandler WriteDelegate)
        {
            this.m_parametersIn = new Dictionary<string, IO_Parameter>();
            this.m_parametersOut = new Dictionary<string, IO_Parameter>();
            foreach (TwincatVariable tv in variables.Values)
            {
                if ((tv.Symbol.Name.Contains("_in_")) || (tv.Symbol.Name.Contains("_IN_")) || (tv.Symbol.Name.Contains("_In_")))
                    this.m_parametersIn.Add(tv.Symbol.Name, new IO_Parameter(tv.Symbol.Name, tv.Type, WriteDelegate));
                else
                    this.m_parametersOut.Add(tv.Symbol.Name, new IO_Parameter(tv.Symbol.Name, tv.Type, WriteDelegate));
            }
        }

        public Dictionary<string, IO_Parameter> In
        {
            get { return this.m_parametersIn; }
        }

        public Dictionary<string, IO_Parameter> Out
        {
            get { return this.m_parametersOut; }
        }

        public void SetAll(Dictionary<string, object> variables)
        {
            foreach (string vName in variables.Keys)
            {
                if (this.m_parametersIn.ContainsKey(vName))
                    this.m_parametersIn[vName].Set(variables[vName]);
                if (this.m_parametersOut.ContainsKey(vName))
                    this.m_parametersOut[vName].Set(variables[vName]);
            }
        }

        public void Set(string variable, object value)
        {
            if (this.m_parametersIn.ContainsKey(variable))
                this.m_parametersIn[variable].Set(value);
            if (this.m_parametersOut.ContainsKey(variable))
                this.m_parametersOut[variable].Set(value);
        }

        public void AddIOParameters(IO_Parameters io)
        {
            foreach (KeyValuePair<string, IO_Parameter> kvpair in io.In)
            {
                if (!this.m_parametersIn.ContainsKey(kvpair.Key))
                    this.m_parametersIn[kvpair.Key] = kvpair.Value;
            }
            foreach (KeyValuePair<string, IO_Parameter> kvpair in io.Out)
            {
                if (!this.m_parametersOut.ContainsKey(kvpair.Key))
                    this.m_parametersOut[kvpair.Key] = kvpair.Value;
            }
            
        }

        public List<IO_Parameter> ToList(bool input)
        {
            if (input)
                return this.m_parametersIn.Values.ToList();
            else
                return this.m_parametersOut.Values.ToList();
        }

        public void SetWriteDelegate(IO_Parameter.TwincatWriteHandler WriteDelegate)
        {
            foreach (IO_Parameter param in this.In.Values)
            {
                param.OnWrite += WriteDelegate;
            }

            foreach (IO_Parameter param in this.Out.Values)
            {
                param.OnWrite += WriteDelegate;
            }
        }

        public bool setRedirections(string paramName, List<string> moduleNames)
        {

            if (m_parametersIn.ContainsKey(paramName))
            {
                m_parametersIn[paramName].Redirections = moduleNames;
                return true;
            }
            if (m_parametersOut.ContainsKey(paramName))
            {
                m_parametersOut[paramName].Redirections = moduleNames;
                return true;
            }
            return false;
        }
    }
}
