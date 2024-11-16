using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.console
{
    public class Program
    {

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
//#if DEBUG
//            if (!System.Diagnostics.Debugger.IsAttached)
//            {
//                Console.WriteLine("Wait for attaching debugger");
//                Console.ReadLine();
//            }
//#endif
            CommandLineHelper cmdHelper = new CommandLineHelper(args);
            bool WCFOff = args.Contains("/WCFOff");
            bool simulation = args.Contains("/Simulation");

            // Install sdk for linux first to be able to debug
            // https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu-install?pivots=os-linux-ubuntu-2004&tabs=dotnet8
            // https://learn.microsoft.com/en-us/sql/linux/sql-server-linux-setup-tools?view=sql-server-ver16&tabs=ubuntu-install#ubuntu
            // ./sqlcmd -S 192.168.178.123\\SQLEXP16 -U gip -P netspirit -C

            ACStartUpRoot startUpManager = new ACStartUpRoot(null);
            String errorMsg = "";

            if (startUpManager.LoginUser(cmdHelper.LoginUser, cmdHelper.LoginPassword, false, false, ref errorMsg, WCFOff, simulation) != 1)
            {
                Messages.ConsoleMsg("System", "Login Error.");
                Messages.ConsoleMsg("System", errorMsg);
                Console.ReadLine();
                return;
            }

            Messages.ConsoleMsg("System", "Service is ready for requests.  Press any key to close service.");
            Console.WriteLine();

            Console.ReadLine();
            Messages.ConsoleMsg("System", "Closing service...");
            ACRoot.SRoot.ACDeInit();
            Messages.ConsoleMsg("System", "Service shutdown...");
            Console.ReadLine();
        }

        private static void MainThread(object data)
        {
        }

        #region Global Exception-Handler
        // Handle the UI exceptions by showing a dialog box, and asking the user whether
        // or not they wish to abort execution.
        // NOTE: This exception cannot be kept from terminating the application - it can only 
        // log the event, and inform the user about it. 
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                if (ACRoot.SRoot != null)
                {
                    if (ACRoot.SRoot.Messages != null)
                    {
                        StringBuilder desc = new StringBuilder();
                        StackTrace stackTrace = new StackTrace(ex, true);
                        for (int i = 0; i < stackTrace.FrameCount; i++)
                        {
                            StackFrame sf = stackTrace.GetFrame(i);
                            desc.AppendFormat(" Method: {0}", sf.GetMethod());
                            desc.AppendFormat(" File: {0}", sf.GetFileName());
                            desc.AppendFormat(" Line Number: {0}", sf.GetFileLineNumber());
                            desc.AppendLine();
                        }

                        ACRoot.SRoot.Messages.LogException("App.CurrentDomain_UnhandledException", "0", ex.Message);
                        if (ex.InnerException != null && !String.IsNullOrEmpty(ex.InnerException.Message))
                            ACRoot.SRoot.Messages.LogException("App.CurrentDomain_UnhandledException", "0", ex.InnerException.Message);

                        string stackDesc = desc.ToString();
                        if (Database.Root != null)
                            Database.Root.Messages.LogException("App.CurrentDomain_UnhandledException", "Stacktrace", stackDesc);
                    }
                }
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                if (Database.Root != null && Database.Root.Messages != null && Database.Root.InitState == ACInitState.Initialized)
                    Database.Root.Messages.LogException("gip.iplus.console.Program", "CurrentDomain_UnhandledException", msg);
            }
        }
        #endregion
    }
}
