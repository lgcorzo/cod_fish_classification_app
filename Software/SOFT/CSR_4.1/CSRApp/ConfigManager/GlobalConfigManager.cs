using System;
using System.Collections.Concurrent;

namespace CSRApp.config.global
{
    public static class GlobalConfigManager
    {
        ///////////////////////////////////////////////////////////
        private static ConcurrentDictionary<string, object> aValues;

        ///////////////////////////////////////////////////////////
        public static void Init()
        {
            aValues = new ConcurrentDictionary<string, object>();
        }

        ///////////////////////////////////////////////////////////
        public static object GetParameter(string strParam)
        {
            try
            {
                object oValue;
                if (aValues.TryGetValue(strParam, out oValue))
                    return oValue;
                else
                    return (int)-1;
            }
            catch (Exception e)
            {
                throw new Exception("Error al conseguir el valor del parametro global. " + e.Message);
            }
        }

        ///////////////////////////////////////////////////////////
        public static void SetParameter(string strParam, object value)
        {
            //Concurrente
            aValues.AddOrUpdate(strParam, value, (key, oldValue) => value);
            //Normal
            //aValues[strParam] = value;
        }

        ///////////////////////////////////////////////////////////
    }
}
