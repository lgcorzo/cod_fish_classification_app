using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using System.Windows.Forms;

namespace TwincatModule
{
    
    public class TwincatConfig
    {
        ///////////////////////////////////////////////////////////////////////////////
        

        private Dictionary<string, TwincatVariable> m_dctVariables;
        private TcAdsClient             m_tcAds;
        private TcAdsSymbolInfoLoader   m_symbolLoader;
        private int                     tries_connection = 0;

        private Dictionary<string, Type> m_tipos = new Dictionary<string, Type> 
        {
            {"BOOL", typeof(Boolean)},
            {"BYTE", typeof(Byte)},
            {"WORD", typeof(ushort)},
            {"DWORD", typeof(uint)},
            {"SINT", typeof(sbyte)},
            {"INT16", typeof(short)},
            {"DINT", typeof(int)},
            {"LINT", typeof(long)},
            {"USINT", typeof(Byte)},
            {"UINT", typeof(ushort)},
            {"UDINT", typeof(uint)},
            {"ULINT", typeof(ulong)},
            {"REAL", typeof(float)},
            {"LREAL", typeof(Double)},
            {"STRING(80)", typeof(String)},
        };
       

        ///////////////////////////////////////////////////////////////////////////////
        public TwincatConfig(TcAdsClient _tcAds)
        {
            string type = "";
            try
            {
                this.m_tcAds = _tcAds;
                this.m_dctVariables = new Dictionary<string, TwincatVariable>();

                this.m_symbolLoader = this.m_tcAds.CreateSymbolInfoLoader();               
                foreach (TcAdsSymbolInfo symbol in this.m_symbolLoader)
                {
                    if (symbol.Name.Contains("CSR_"))
                    {
                        type = symbol.Type.ToString();

                        if (this.m_tipos.ContainsKey(type))
                            this.m_dctVariables.Add(symbol.Name, new TwincatVariable(symbol, this.m_tipos[type]));
                    }

                }

            }catch(Exception e)
            {
                string textoout = e.ToString();
                tries_connection++;
                TwincatReConfig(this.m_tcAds);
            }
        }
        /// <summary>
        /// reconfiguracion en caso de fallo
        /// </summary>
        /// <param name="_tcAds"></param>
        public void TwincatReConfig(TcAdsClient _tcAds)
        {
            string type = "";
            try
            {

                this.m_symbolLoader = this.m_tcAds.CreateSymbolInfoLoader();
                foreach (TcAdsSymbolInfo symbol in this.m_symbolLoader)
                {
                    if (symbol.Name.Contains("CSR_"))
                    {
                        type = symbol.Type.ToString();

                        if (this.m_tipos.ContainsKey(type))
                            this.m_dctVariables.Add(symbol.Name, new TwincatVariable(symbol, this.m_tipos[type]));
                    }

                }

            }
            catch (Exception e)
            {

                if (tries_connection > 5) //despiues de 5 minutos saca el error
                    MessageBox.Show(new Form() { TopMost = true }, type + e.ToString(), "Error", MessageBoxButtons.OK);
                else
                {
                    tries_connection++;
                    System.Threading.Thread.Sleep(5000);
                    TwincatReConfig(this.m_tcAds);

                }
            }
        }

        public Dictionary<string, TwincatVariable> Variables
        {
            get { return this.m_dctVariables; }
            set { this.m_dctVariables = value; }
        }

    }
}
