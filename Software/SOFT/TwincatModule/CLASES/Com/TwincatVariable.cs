using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;

namespace TwincatModule
{
    public class TwincatVariable
    {
        TcAdsSymbolInfo m_symbol;
        int m_handle = -1;
        Type m_type;

        public TwincatVariable(TcAdsSymbolInfo symbol, Type type)
        {
            this.m_symbol = symbol;
            this.m_type=type;
        }

        public TcAdsSymbolInfo Symbol
        {
            get { return this.m_symbol; }
            set { this.m_symbol = value; }
        }

        public int Handle
        {
            get { return this.m_handle; }
            set { this.m_handle = value; }
        }

        public Type Type
        {
            get { return this.m_type; }
            set { this.m_type = value; }
        }

    }
}
