using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRApp.Auxiliars;

namespace CSRApp.Logger
{
    public sealed class CSRExecutionTimeWriter:FileWriter
    {

        //////////////////////////////////////////////////////////////
        private static volatile CSRExecutionTimeWriter instance;
        private static object sync = new Object();

        private DateTime startTime = DateTime.MinValue;
        private DateTime prevTime = DateTime.MinValue;

        //////////////////////////////////////////////////////////////
        private CSRExecutionTimeWriter()
        {
            
        }

        //////////////////////////////////////////////////////////////
        public static CSRExecutionTimeWriter Instance
        {
            get 
            {
                if (instance == null) 
                {
                    lock (sync) 
                    {
                        if (instance == null) 
                            instance = new CSRExecutionTimeWriter();
                    }
                }
                return instance;
            }
        }

        //////////////////////////////////////////////////////////////
        public bool Init(DateTime startTime, string _app_name = "", string _filename = "", bool _debug = false)
        {
            base.Init("ExecutionTimeWriter", _app_name, _filename, _debug);
            this.startTime = startTime;
            prevTime = startTime;
            return true;
        }

        //////////////////////////////////////////////////////////////
        public bool WriteExecutionTime(string owner, string input, DateTime time)
        {
            string str = "(+" + (time - prevTime) + " / " + (time - startTime).ToString() +") " + input;
            prevTime = time;
   
            Write(owner, str);
            return true;
        }
    }

    //////////////////////////////////////////////////////////////
}
