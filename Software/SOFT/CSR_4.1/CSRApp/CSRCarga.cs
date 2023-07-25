using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSRApp.forms;
using System.Threading;
using System.Windows.Forms;

namespace CSRApp
{
    public class CSRCarga
    {
        static Carga MySplashForm = null;
        static Thread MySplashThread = null;

        //	internally used as a thread function - showing the form and
        //	starting the messageloop for it
        static void ShowThread()
        {
            MySplashForm = new Carga();
            Application.Run(MySplashForm);
            
        }

        //	public Method to show the SplashForm
        static public void Show()
        {
            if (MySplashThread != null)
                return;

            MySplashThread = new Thread(new ThreadStart(CSRCarga.ShowThread));
            MySplashThread.Name = "CSR Carga";
            MySplashThread.IsBackground = true;
            MySplashThread.SetApartmentState(ApartmentState.STA);
            MySplashThread.Start();

            while (MySplashForm == null || !MySplashForm.Loaded)
                Thread.Sleep(5);
        }

        //	public Method to hide the SplashForm
        static public void Close()
        {
            if (MySplashThread == null) return;
            if (MySplashForm == null) return;

            try
            {
                MySplashForm.Invoke(new MethodInvoker(MySplashForm.Close));
            }
            catch (Exception)
            {
            }
            MySplashThread = null;
            MySplashForm = null;
        }

        //	public Method to set or get the loading Status
        static public string Status
        {
            set
            {
                if (MySplashForm == null)
                {
                    return;
                }

                MySplashForm.StatusInfo = value;
            }
            get
            {
                if (MySplashForm == null)
                {
                    throw new InvalidOperationException("El logo de carga no está en pantalla.");
                }
                return MySplashForm.StatusInfo;
            }
        }

        static public int Progress
        {
            set
            {
                if (MySplashForm == null)
                {
                    return;
                }

                MySplashForm.Progress = value;
            }
            get
            {
                if (MySplashForm == null)
                {
                    throw new InvalidOperationException("El logo de carga no está en pantalla.");
                }
                return MySplashForm.Progress;
            }
        }

    }
}
