using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TwinCAT.Ads;

namespace TwincatModule
{
    public class IO_Parameter
    {
        private string m_name;
        private string m_Type;
        private object m_value;
        private List<string> m_redirections = new List<string>();

        public delegate void TwincatWriteHandler(string variableName, object value);

        public event EventHandler OnChange;
        public event TwincatWriteHandler OnWrite;

        public IO_Parameter() { }
        public IO_Parameter(string name, Type type)
        {
            this.m_name = name;
            this.m_Type = type.ToString();

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    this.m_value = false;
                    break;
                case TypeCode.String:
                    this.m_value = "";
                    break;
                default:
                    this.m_value = 0;
                    break;
            }
        }

        public IO_Parameter(string name, Type type, TwincatWriteHandler WriteDelegate)
        {
            this.m_name = name;
            this.m_Type = type.ToString();

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    this.m_value = false;
                    break;
                case TypeCode.String:
                    this.m_value = "";
                    break;
                default:
                    this.m_value = 0;
                    break;
            }
            this.OnWrite += WriteDelegate;
        }

        public void Set(object value)
        {
            bool onChangeBool = false;
            if (!this.m_value.Equals(value))
                onChangeBool = true;
            this.m_value = value;
            if ((onChangeBool) && (OnChange != null))
                OnChange(this, null);
        }

        public void enviarMensajes()
        {
            if (OnChange != null)
                OnChange(this, null);
        }

        public void Write(object value)
        {
            this.m_value = value;
            if (OnWrite != null)
                OnWrite(this.Name, value);
        }

        public string Name
        {
            get { return this.m_name; }
            set { this.m_name = value; }
        }

        public string TypeStr
        {
            get { return this.m_Type; }
            set { this.m_Type = value; }
        }

        [XmlIgnoreAttribute]
        protected Type Type
        {
            get { return Type.GetType(this.m_Type); }
            set { this.m_Type = value.ToString(); }
        }

        public object Value
        {
            get { return this.m_value; }
            set { this.m_value = value; }
        }

        public List<string> Redirections
        {
            get { return this.m_redirections; }
            set { this.m_redirections = value; }
        }
    }

    public class IO_Parameter_Filter
    {
        private List<string> m_name = new List<string>();
        private List<string> m_redirections = new List<string>();

        public IO_Parameter_Filter() { }
      
        public List<string> Name
        {
            get { return this.m_name; }
            set { this.m_name = value; }
        }

        public List<string> Redirections
        {
            get { return this.m_redirections; }
            set { this.m_redirections = value; }
        }
    }
}
