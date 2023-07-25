using System;
using System.Text;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using CSRApp.Auxiliars;

namespace CSRApp.Logger
{
    //////////////////////////////////////////////////////////////
    public sealed class CSRLogger:FileWriter
    {

        //////////////////////////////////////////////////////////////
        private static volatile CSRLogger instance;
        private static object sync = new Object();

        //////////////////////////////////////////////////////////////
        private CSRLogger()
        {
            
        }

        //////////////////////////////////////////////////////////////
        public static CSRLogger Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (sync) 
                    {
                        if (instance == null) 
                            instance = new CSRLogger();
                    }
                }
                return instance;
            }
        }

        //////////////////////////////////////////////////////////////
        public bool Init(string _app_name = "", string _filename = "", bool _debug = false)
        {
            base.Init("Logger", _app_name, _filename, _debug);
            return true;
        }

        //////////////////////////////////////////////////////////////
        public bool Log(string owner, string input)
        {
            Write(owner, input);
            return true;
        }
    }

    //////////////////////////////////////////////////////////////
}
