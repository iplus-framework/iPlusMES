using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.Windows.Media;
using gip.core.layoutengine;
using System.Threading;
using System.Windows.Threading;
using System.Security.Principal;
using System.Text;
using System.Diagnostics;
using gip.mes.datamodel;
using gip.core.wpfservices;

namespace gip.mes.client
{

    /// <summary>
    /// Anwendungsweiter Delegat zum aktualisieren der UI-Elemente auf dem UI-Thread.
    /// </summary>
    /// <remarks>Wird benötigt um Cross-Threading Situationen zu vermeiden.</remarks>
    internal delegate void Invoker();

    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        static ACStartUpRoot _StartUpManager = null;
        public static App _GlobalApp = null;

        #region internal Delegates

        /// <summary>
        /// Delegate zum initialisieren der VarioBatchLogin-Klasse.
        /// </summary>
        /// <param name="VarioBatchLogin">Eine Instanz der VarioBatchLogin-Klasse.</param>
        /// <remarks>n/a</remarks>
        internal delegate void ApplicationInitializeDelegate(Login VarioBatchLogin);

        /// <summary>
        /// Hält eine Instanz des ApplicationInitializeDelegate.
        /// </summary>
        /// <remarks>Die Methode wird im Konstruktor der Klasse an den Delegaten übergeben.</remarks>
        internal ApplicationInitializeDelegate ApplicationInitialize;

        /// <summary>
        /// Gibt die aktuelle AppDomain als Instanz der App-Klasse zurück.
        /// </summary>
        /// <value>Eine Instanz der App-Klasse.</value>
        /// <remarks>Wird benötigt um auf den ApplicationInitialize Delegaten zugreifen zu können.</remarks>
        public static new App Current
        {
            get { return Application.Current as App; }
        }

        #endregion


        #region c'tors
        public App()
        {
            // Die Initialisierungs-Methode an den Delegaten übergeben.
            ApplicationInitialize = applicationInitialize;

            _GlobalApp = this;
            _StartUpManager = new ACStartUpRoot(new WPFServices());
            //this.Startup += new StartupEventHandler(App_Startup);

            // Add the event handler for handling non-UI thread exceptions to the event. 
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }
        #endregion


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
                        ACRoot.SRoot.Messages.LogException("App.CurrentDomain_UnhandledException", "Stacktrace", stackDesc);
                    }
                }
            }
            catch (Exception exc)
            {
                try
                {
                    Msg userMsg = new Msg() { Message = "Fatal Non-UI Error. Could not write the error to the event log. Reason: " + exc.Message, MessageLevel = eMsgLevel.Info };
                    VBWindowDialogMsg vbMessagebox = new VBWindowDialogMsg(userMsg, eMsgButton.OK, null);
                    vbMessagebox.ShowMessageBox();
                }
                finally
                {
                    if (ACRoot.SRoot != null)
                        ACRoot.SRoot.ACDeInit();
                    // Ist notwendig, damit die Anwendung auch wirklich als Prozess beendet wird
                    App._GlobalApp.Shutdown();
                }
            }
        }
        #endregion

        #region Startup
        public void applicationInitialize(Login loginWindow)
        {
            string[] cmLineArg = System.Environment.GetCommandLineArgs();

            bool RegisterACObjects = false;
            bool PropPersistenceOff = false;
            bool WCFOff = cmLineArg.Contains("/" + Const.StartupParamWCFOff);
            bool simulation = cmLineArg.Contains("/" + Const.StartupParamSimulation);
            bool fullscreen = cmLineArg.Contains("/" + Const.StartupParamFullscreen);
            string UserName = "";
            string PassWord = "";
            eWpfTheme wpfTheme = gip.mes.client.Properties.Settings.Default.WpfTheme;
            ControlManager.RestoreWindowsOnSameScreen = gip.mes.client.Properties.Settings.Default.RestoreWindowsOnSameScreen;
            ControlManager.TouchScreenMode = gip.mes.client.Properties.Settings.Default.TouchScreenMode;

            if (!cmLineArg.Contains("/autologin"))
            {
                UserName = gip.mes.client.Properties.Settings.Default.User;
#if DEBUG
                PassWord = gip.mes.client.Properties.Settings.Default.Password;
#endif
            }

            if (cmLineArg.Contains("-controlLoad=True"))
                loginWindow.IsLoginWithControlLoad = true;

            String errorMsg = "";
            for (int i = 0; i < 3; i++)
            {
                if (!cmLineArg.Contains("/autologin") || i > 0)
                {
                    loginWindow.DisplayLogin(true, UserName, PassWord, wpfTheme, errorMsg);
                    loginWindow.GetLoginResult(ref UserName, ref PassWord, ref RegisterACObjects, ref PropPersistenceOff);
                    wpfTheme = loginWindow.WpfTheme;
                    errorMsg = "";
                    loginWindow.DisplayLogin(false, "", "", wpfTheme, errorMsg);
                }
                else
                {
                    if (i > 0)
                        break;
                    UserName = WindowsIdentity.GetCurrent().Name.Split('\\')[1];
                    PassWord = "autologin";
                }

                ControlManager.WpfTheme = wpfTheme;
                short result = _StartUpManager.LoginUser(UserName, PassWord, RegisterACObjects, PropPersistenceOff, ref errorMsg, WCFOff, simulation, fullscreen);
                if (result == 1)
                {
                    if (!cmLineArg.Contains("/autologin"))
                    {
                        gip.mes.client.Properties.Settings.Default.User = UserName;
#if DEBUG
                        gip.mes.client.Properties.Settings.Default.Password = PassWord;
#endif
                        gip.mes.client.Properties.Settings.Default.WpfTheme = wpfTheme;
                        gip.mes.client.Properties.Settings.Default.RestoreWindowsOnSameScreen = ControlManager.RestoreWindowsOnSameScreen;
                        gip.mes.client.Properties.Settings.Default.TouchScreenMode = ControlManager.TouchScreenMode;
                        gip.mes.client.Properties.Settings.Default.Save();
                    }

                    // Do First query on DatabaseApp to initiate parsing of edmx-Model because it takes long.
                    DatabaseApp dbApp = RootDbOpQueue.AppContext as DatabaseApp;
                    if (dbApp != null)
                        dbApp.MDMaterialType.Any();

                    break;
                }
                // Keine Lizenz
                if (result == -1)
                    break;
            }

            if (ACRoot.SRoot != null)
                ACRoot.SRoot.Environment.License.PropertyChanged += License_PropertyChanged;

            // Initialisierung abgeschlossen, Hauptfenster laden
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Invoker)delegate
            {
                ControlManager.RegisterImplicitStyles(this);
                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                if (ACRoot.SRoot == null)
                {
                    Shutdown();
                    return;
                }

                Application.Current.MainWindow = new Masterpage();
                UpdateLicenseTitle();
                Application.Current.MainWindow.Show();
            });
        }


        private void License_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e != null && e.PropertyName == "IsTrial")
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Invoker)delegate
                {
                    UpdateLicenseTitle();
                });
            }
        }

        private void UpdateLicenseTitle()
        {
            string info = "";
#if DEBUG
            info = "DEBUG";
#else
            if (ACRoot.SRoot.Environment.License.IsTrial)
                info = "Trial";
            else if (ACRoot.SRoot.Environment.License.IsDeveloper)
                info = "Development";
            else if (ACRoot.SRoot.Environment.License.IsRemoteDeveloper)
                info = "Remote development";
#endif

            Application.Current.MainWindow.Title = String.Format("iPlus.MES{3}({0}, {1}) {2}", ACRoot.SRoot.Environment.User.VBUserName,
                                                                      ACRoot.SRoot.Environment.DatabaseName, info, ACRoot.SRoot.Environment.License.LicensedToTitle);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            return;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (ACRoot.SRoot != null
                && ACRoot.SRoot.Environment != null
                && ACRoot.SRoot.Environment.License != null)
                ACRoot.SRoot.Environment.License.PropertyChanged -= License_PropertyChanged;
            base.OnExit(e);
        }

        #endregion

    }
}
