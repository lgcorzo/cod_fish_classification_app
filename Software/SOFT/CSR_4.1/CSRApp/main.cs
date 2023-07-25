using System;
using System.Windows.Forms;


namespace CSRApp
{
    static class main
    {

        ///////////////////////////////////////////////////////////
        [STAThread]
        static void Main()
        {
            CSRApp app = new CSRApp(DateTime.Now);

            CSRCarga.Show();
            if (app.Init()){
                CSRCarga.Close();
                app.RunApp();
            }
            else
                System.Windows.Forms.MessageBox.Show("Ha ocurrido un error durante la ejecución de la función Init de la aplicación.\nLa aplicación no puede continuar la ejecución.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            app.Destroy();
        }

        ///////////////////////////////////////////////////////////
    }
}
