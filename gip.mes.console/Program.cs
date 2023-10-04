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
            CommandLineHelper cmdHelper = new CommandLineHelper(args);
            bool WCFOff = args.Contains("/WCFOff");
            bool simulation = args.Contains("/Simulation");

            // TODO: Two different Implementaions for Linux or Windows-Platform
            //ACStartUpRoot startUpManager = new ACStartUpRoot(new WPFServices());
            // If Linux, then pass null
            ACStartUpRoot startUpManager = new ACStartUpRoot(null);
            String errorMsg = "";
            // 1. Datenbankverbindung herstellen
            Thread.Sleep(5000);
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
            Messages.ConsoleMsg("System", "Service beendet...");
            Console.ReadLine();
        }

        /// <summary>
        /// Methode liest die Aktuelle Ip des Servers aus und vergleicht diese mit der Ip in der Datenbank.
        /// Falls die Aktuelle Ip von der In der Datenbank abweicht, wird die Ip der Datenbank überschrieben.
        /// Nun werden die Ports Ausgelesen.
        /// Falls in der Datenbank keine Ports vorhanden sind werden die Defaultports, welche Hardcoded 
        /// in dieser Klasse als property vorhanden sind genommen und in die DB geschrieben.
        /// Ansonsten werden die Ports aus der Datenbank genommen.
        /// </summary>

        private static void MainThread(object data)
        {
            //int interval = Program._Random.Next(2000, 7000);
            //_Timer.Change(interval, Timeout.Infinite);
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
