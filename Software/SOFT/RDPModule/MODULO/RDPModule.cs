using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Net;
using System.Threading;
using System.Xml.Serialization;
using System.Xml;

namespace RDPModule
{
    public class RDPModule : csr.modules.CSRFormModule
    {

       
        object                              m_lock              = new object();
        private static System.Threading.AutoResetEvent stopFlag = new System.Threading.AutoResetEvent(false);
        //////////////////////////////////////////////////////////////////////////////////
        public RDPModule(string _id)
            : base(_id)
        { }


        //////////////////////////////////////////////////////////////////////////////////
        public override bool Init()
        {
            base.Init();
            ModuleDelegates module_delegates    = new ModuleDelegates();
            WindowForm                          = new RDPForm(this, module_delegates);
            WriteConsole("Módulo cargado correctamente.",true);
            //inicio las comunicaciones
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////////
        public override void HandleMessages(csr.com.CSRMessage message)
        {
            if (message.ToString() == "SEND_IMAGES_TEXTURA")
            {
                

            }
        }

        
      
     
      

       

        
        ////////
    }

   

    //////////////////////////////////////////////////////////////
    public class ModuleDelegates
    {
       
        //////////////////////////////////////////////////////////////
        public ModuleDelegates()
        {
           
          
        }
        //////////////////////////////////////////////////////////////
    }

  }



 