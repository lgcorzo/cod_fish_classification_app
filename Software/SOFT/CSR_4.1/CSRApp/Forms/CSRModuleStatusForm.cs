using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace CSRApp
{
    public partial class CSRModuleStatusForm : Form
    {
        public delegate void WriteConsoleDelegate(string id, string message);
        ///////////////////////////////////////////////////////////

        private Dictionary<string, CSRModuleStatusControl> statusControls;

        ///////////////////////////////////////////////////////////
        public CSRModuleStatusForm()
        {
            InitializeComponent();
            statusControls = new Dictionary<string, CSRModuleStatusControl>();
        }

        ///////////////////////////////////////////////////////////
        public void AddModule(string _id, string _name)
        {
            CSRModuleStatusControl status_control = new CSRModuleStatusControl(_name);
            status_control.Parent = groupModules;
            status_control.Location = new Point(5, 25 + ((status_control.Height + 10) * statusControls.Count));
            groupModules.Controls.Add(status_control);
            statusControls.Add(_id, status_control);
        }

        ///////////////////////////////////////////////////////////
        public void SetModuleError(string id, bool hasError)
        {
            CSRModuleStatusControl statuscontrol;
            if(statusControls.TryGetValue(id, out statuscontrol))
                statuscontrol.SetHasErrorsStatus(true);
        }

        ///////////////////////////////////////////////////////////
        public void SetModuleConnection(string id, bool isConnected)
        {
            CSRModuleStatusControl statuscontrol;
            if (statusControls.TryGetValue(id, out statuscontrol))
                statuscontrol.SetIsConnectedStatus(true);
        }

        ///////////////////////////////////////////////////////////
        public void WriteConsole(string id, string message)
        {
            try
            {
                if (!IsDisposed)
                {
                    if (txtConsole.InvokeRequired)
                        txtConsole.Invoke(new WriteConsoleDelegate(WriteConsole), new object[] { id, message });  // invoking itself
                    else
                    {
                        string built_message = String.Format("[{0}]-[{1}]::{2}", DateTime.Now.ToString("HH:mm:ss.fff"), id, message);
                        if (IsDisposed)
                            return;
                        Size ConsoleSize = txtConsole.Size;
                        Font ConsoleFont = txtConsole.Font;
                        string completeText = "";
                        if( txtConsole.Text == "")
                            completeText = built_message;
                        else
                            completeText = String.Format("{0}" + System.Environment.NewLine + "{1}", built_message, txtConsole.Text);

                        int visibleChars = 0;
                        int visibleLines = 0;

                        string textoValido = completeText;

                        if (IsDisposed)
                            return;
                        CreateGraphics().MeasureString(completeText, ConsoleFont, new SizeF(ConsoleSize.Width, ConsoleSize.Height), new StringFormat(StringFormatFlags.FitBlackBox), out visibleChars, out visibleLines);
                        int maxLineas = (int)(ConsoleSize.Height / (2.6 + ConsoleFont.Size + 2.6));

                        while (visibleLines >= maxLineas && completeText.Contains("\r\n"))
                        {
                            textoValido = completeText;

                            completeText = completeText.Substring(0, completeText.LastIndexOf("\r\n"));
                            if (IsDisposed) 
                                return;
                            CreateGraphics().MeasureString(completeText, ConsoleFont, new SizeF(ConsoleSize.Width, ConsoleSize.Height), new StringFormat(StringFormatFlags.FitBlackBox), out visibleChars, out visibleLines);  
                        }

                        if (!completeText.Contains("\r\n"))
                            textoValido = completeText;

                        if (IsDisposed)
                            return;
                        txtConsole.Text = textoValido;
                    }
                }
            }
            catch (Exception e)
            {
                StackTrace sTrace = new StackTrace(e, true);
                StackFrame sFrame = sTrace.GetFrame(sTrace.FrameCount - 1);
                int line = sFrame.GetFileLineNumber();
                MessageBox.Show("Error interno del CSR. WriteConsole bloqueado. " + e.Message + " -> linea " + line);
            }
        }
        ///////////////////////////////////////////////////////////
    }
}
