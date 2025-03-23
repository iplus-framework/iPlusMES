// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using gip.core.layoutengine;
using gip.core.datamodel;
using gip.core.autocomponent;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Reflection;
using gip.core.layoutengine.Helperclasses;
using System.Runtime.CompilerServices;

namespace gip.mes.client
{
    public partial class Masterpage : Window, IRootPageWPF, INotifyPropertyChanged, IACInteractiveObject
    {

        public Masterpage()
        {
            InitializeComponent();
            Loaded += Masterpage_Loaded;
            LocationChanged += Masterpage_LocationChanged;
            ControlManager.RegisterImplicitStyles(this);

            // Refresh restore bounds from previous window opening
            WindowStateHandleSettings windowStateHandleSettings = WindowStateHandleSettings.Factory();
            this.sldZoom.Value = windowStateHandleSettings.Zoom;
            // Fractal
            _FractalImagePresenter = new FractalImagePresenter(this);
            _FractalImagePresenter.Generator.Completed += new EventHandler<FractalImageCompletedEventArgs>(Generator_Completed);

            InitConnectionInfo();
        }

        void Masterpage_Loaded(object sender, RoutedEventArgs e)
        {
            WindowStateHandle.RestoreState(this);
        }

        void Masterpage_LocationChanged(object sender, EventArgs e)
        {
            WindowStateHandle.Save(this, 0, true);
        }

        public void Styles_PropertyChanged(object sender, EventArgs e)
        { }

        #region eventhandling
        ACMenuItem mainMenu;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string[] cmLineArg = System.Environment.GetCommandLineArgs();
            ACRoot.SRoot.RootPageWPF = this;

            if (ACRoot.SRoot.Environment.User.MenuACClassDesign == null)
            {

                ACClassDesign acClassDesign = ACRoot.SRoot.GetDesign(Global.ACKinds.DSDesignMenu);

                mainMenu = acClassDesign.GetMenuEntryWithCheck(ACRoot.SRoot);
            }
            else
            {

                using (ACMonitor.Lock(ACRoot.SRoot.Database.QueryLock_1X000))
                {
                    mainMenu = ACRoot.SRoot.Environment.User.MenuACClassDesign.GetMenuEntryWithCheck(ACRoot.SRoot);
                }
            }

            if (mainMenu != null)
                CreateMenu(mainMenu.Items, MainMenu.Items);

            InitAppCommands();
            InitMainDockManager();
        }



        private void InitAppCommands()
        {
            //CommandBindings.Add(new CommandBinding(AppCommands.CmdNew, menuItem_Click, menuItem_IsEnabled));
            //CommandBindings.Add(new CommandBinding(AppCommands.CmdLoad, menuItem_Click, menuItem_IsEnabled));
            //CommandBindings.Add(new CommandBinding(AppCommands.CmdDelete, menuItem_Click, menuItem_IsEnabled));
            //CommandBindings.Add(new CommandBinding(AppCommands.CmdSave, menuItem_Click, menuItem_IsEnabled));
            //CommandBindings.Add(new CommandBinding(AppCommands.CmdSearch, menuItem_Click, menuItem_IsEnabled));
            //CommandBindings.Add(new CommandBinding(AppCommands.CmdRequery, menuItem_Click, menuItem_IsEnabled));
            //CommandBindings.Add(new CommandBinding(AppCommands.CmdNavigateFirst, menuItem_Click, menuItem_IsEnabled));
            //CommandBindings.Add(new CommandBinding(AppCommands.CmdNavigatePrev, menuItem_Click, menuItem_IsEnabled));
            //CommandBindings.Add(new CommandBinding(AppCommands.CmdNavigateNext, menuItem_Click, menuItem_IsEnabled));
            //CommandBindings.Add(new CommandBinding(AppCommands.CmdNavigateLast, menuItem_Click, menuItem_IsEnabled));
            //CommandBindings.Add(new CommandBinding(AppCommands.CmdReportPrintDlg, menuItem_Click, menuItem_IsEnabled));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (ACComponent childComp in ACRoot.SRoot.ACComponentChilds)
            {
                if (childComp is ApplicationManagerProxy || childComp is ACComponentManager)
                {
                    IACPropertyNetBase alarmProperty = childComp.GetPropertyNet("HasAlarms") as IACPropertyNetBase;
                    if (alarmProperty != null)
                        alarmProperty.PropertyChanged -= alarmProperty_PropertyChanged;
                    alarmProperty = childComp.GetPropertyNet("AlarmsAsText") as IACPropertyNetBase;
                    if (alarmProperty != null)
                        alarmProperty.PropertyChanged -= alarmProperty_PropertyChanged;
                }
            }


            Mouse.OverrideCursor = Cursors.Wait;
            Hide();
            
            // Save restore bounds for the next time this window is opened
            WindowStateHandle.Save(this, (int)this.sldZoom.Value);
            Properties.Settings.Default.Save();

            ACRoot.SRoot.ACDeInit();
            // Ist notwendig, damit die Anwendung auch wirklich als Prozess beendet wird

            App._GlobalApp.Shutdown();
        }

        public void CloseWindowFromThread()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Invoker)delegate { Close(); });
        }

        private bool _InFullscreen = false;
        object _FullscreenContent = null;
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.Key == Key.F11)
            {
                if (!_InFullscreen)
                {
                    if (_RootVBDesign != null
                        && DockingManager != null
                        && DockingManager.vbDockingPanelTabbedDoc_TabControl != null
                        && DockingManager.vbDockingPanelTabbedDoc_TabControl.SelectedItem != null)
                    {
                        TabItem tabItem = DockingManager.vbDockingPanelTabbedDoc_TabControl.SelectedItem as TabItem;
                        _FullscreenContent = tabItem.Content;
                        tabItem.Content = null;
                        this.Content = _FullscreenContent;
                        (this.Content as FrameworkElement).LayoutTransform = SubMainDockPanel.LayoutTransform;
                        this.WindowStyle = System.Windows.WindowStyle.None;
                        this.WindowState = System.Windows.WindowState.Maximized;
                        _InFullscreen = true;
                        e.Handled = true;
                    }
                }
                else
                {
                    if (_RootVBDesign != null
                        && _FullscreenContent != null
                        && DockingManager != null
                        && DockingManager.vbDockingPanelTabbedDoc_TabControl != null
                        && DockingManager.vbDockingPanelTabbedDoc_TabControl.SelectedItem != null)
                    {
                        this.Content = MainDockPanel;
                        TabItem tabItem = DockingManager.vbDockingPanelTabbedDoc_TabControl.SelectedItem as TabItem;
                        (_FullscreenContent as FrameworkElement).LayoutTransform = null;
                        tabItem.Content = _FullscreenContent;
                        _FullscreenContent = null;
                        this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                        this.WindowState = System.Windows.WindowState.Normal;
                        _InFullscreen = false;
                        e.Handled = true;
                    }
                }
            }
            base.OnKeyUp(e);
        }

#endregion

#region Menü
        private void CreateMenu(ACMenuItemList mainMenu, ItemCollection items)
        {
            foreach (ACMenuItem acMenuItem in mainMenu)
            {
                if (acMenuItem.ACCaption == "-")
                {
                    VBMenuSeparator _Seperator = new VBMenuSeparator();
                    items.Add(_Seperator);
                    continue;
                }

                VBMenuItem menuItem = new VBMenuItem(ContextACObject, acMenuItem);
                items.Add(menuItem);
                if (acMenuItem.Items != null && acMenuItem.Items.Count > 0)
                {
                    CreateMenu(acMenuItem.Items, menuItem.Items);
                }
            }
        }
#endregion


#region Layout
        VBDesign _RootVBDesign = null;

        VBDockingManager DockingManager
        {
            get
            {
                if (_RootVBDesign == null)
                    return null;
                return _RootVBDesign.Content as VBDockingManager;
            }
        }

        private void InitMainDockManager()
        {
            if (_RootVBDesign != null)
                return;
            if (ACRoot.SRoot.Businessobjects != null)
            {
                ACClassDesign acClassDesign = ACRoot.SRoot.Businessobjects.GetDesign(Global.ACKinds.DSDesignLayout, Global.ACUsages.DUMain);
                if (acClassDesign != null)
                {
                    _RootVBDesign = new VBDesign();
                    _RootVBDesign.VBContent = "*" + acClassDesign.ACIdentifier;
                    _RootVBDesign.DataContext = ACRoot.SRoot.Businessobjects;
                }
                acClassDesign = ACRoot.SRoot.Businessobjects.GetDesign("AppResourceDict");
                if (acClassDesign != null)
                {
                    ResourceDictionary resDict = Layoutgenerator.LoadResource(acClassDesign.XMLDesign, ACRoot.SRoot.Businessobjects, null);
                    if (resDict != null)
                    {
                        resDict.Add("TouchScreenMode", ControlManager.TouchScreenMode);
                        App._GlobalApp.Resources.MergedDictionaries.Add(resDict);
                    }
                }
            }
            if (_RootVBDesign == null)
            {
                _RootVBDesign = new VBDesign();
                _RootVBDesign.DataContext = ACRoot.SRoot.Businessobjects;
            }
            _RootVBDesign.Margin = new Thickness(0, 0, -5, 0);
            _RootVBDesign.Loaded += new RoutedEventHandler(RootVBDesign_Loaded);
            SubMainDockPanel.Children.Add(_RootVBDesign);
            foreach (ACComponent childComp in ACRoot.SRoot.ACComponentChilds)
            {
                if (childComp is ApplicationManagerProxy || childComp is ACComponentManager)
                {
                    IACPropertyNetBase alarmProperty = childComp.GetPropertyNet("HasAlarms") as IACPropertyNetBase;
                    if (alarmProperty != null)
                        alarmProperty.PropertyChanged += alarmProperty_PropertyChanged;
                    alarmProperty = childComp.GetPropertyNet("AlarmsAsText") as IACPropertyNetBase;
                    if (alarmProperty != null)
                        alarmProperty.PropertyChanged += alarmProperty_PropertyChanged;

                }
            }
            RefreshWarningIcon();
        }

        void alarmProperty_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RefreshWarningIcon();
        }

        private bool _TooltipShowDurationSet = false;
        void RefreshWarningIcon()
        {
            if (!this.WarningIcon.CheckAccess())
            {
                this.WarningIcon.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(RefreshWarningIcon));
                return;
            }

            bool hasAlarms = false;
            StringBuilder alarmInfo = new StringBuilder();
            foreach (ACComponent childComp in ACRoot.SRoot.ACComponentChilds)
            {
                if (childComp is ApplicationManagerProxy || childComp is ACComponentManager)
                {
                    IACPropertyNetBase alarmProperty = childComp.GetPropertyNet("HasAlarms") as IACPropertyNetBase;
                    IACPropertyNetBase alarmText = childComp.GetPropertyNet("AlarmsAsText") as IACPropertyNetBase;
                    if (alarmProperty != null && alarmText != null)
                    {
                        if ((bool)alarmProperty.Value)
                        {
                            hasAlarms = true;
                            alarmInfo.AppendLine(String.Format("{0}: {1}", childComp.ACCaption, alarmText.Value as string));
                        }
                    }
                }
            }

            if (!_TooltipShowDurationSet)
            {
                ToolTipService.SetShowDuration(WarningIcon, 20000);
                _TooltipShowDurationSet = true;
            }
            if (hasAlarms)
            {
                WarningIcon.Visibility = System.Windows.Visibility.Visible;
                WarningIcon.ToolTip = alarmInfo.ToString();
            }
            else
            {
                WarningIcon.Visibility = System.Windows.Visibility.Collapsed;
                WarningIcon.ToolTip = null;
            }
        }

        void RootVBDesign_Loaded(object sender, RoutedEventArgs e)
        {
            if ((_RootVBDesign.Content == null) || !(_RootVBDesign.Content is VBDockingManager))
            {
                _RootVBDesign.Content = new VBDockingManager();
                DockingManager.Name = "mainDockingManager";
            }
            DockingManager.IsBSOManager = true;
            DockingManager.InitBusinessobjectsAtStartup();
            if (!_startingFullScreen && ACRoot.SRoot.Fullscreen)
            {
                StartInFullScreen();
                _startingFullScreen = true;
            }
        }

        IACComponent _CurrentACComponent = null;
        public IACComponent CurrentACComponent
        {
            get
            {
                if (_CurrentACComponent == null)
                    return ACRoot.SRoot;
                return _CurrentACComponent;
            }
            set
            {
                _CurrentACComponent = value;
            }

        }

        public object WPFApplication
        {
            get
            {
                return App._GlobalApp;
            }
        }

        public void StartBusinessobjectByACCommand(ACCommand acCommand)
        {
            if (DockingManager == null)
                return;

            bool ribbonVisibilityOff = false;
            string caption = "";
            ACMenuItem menuItem = acCommand as ACMenuItem;
            if (menuItem != null)
            {
                ribbonVisibilityOff = menuItem.RibbonOff;
                caption = menuItem.UseACCaption ? menuItem.ACCaption : "";
            }

            DockingManager.StartBusinessobject(acCommand.GetACUrl(), acCommand.ParameterList, caption, ribbonVisibilityOff);
        }

        public void StartBusinessobject(string acUrl, ACValueList parameterList, string acCaption = "")
        {
            if (DockingManager == null)
                return;
            DockingManager.StartBusinessobject(acUrl, parameterList, acCaption);
        }

        public FocusBSOResult FocusBSO(IACBSO bso)
        {
            if (DockingManager == null
                || DockingManager.vbDockingPanelTabbedDoc_TabControl == null
                || DockingManager.vbDockingPanelTabbedDoc_TabControl.Items.Count <= 0)
                return FocusBSOResult.NotFocusable;
            VBTabItem vbTabItem = DockingManager.vbDockingPanelTabbedDoc_TabControl.SelectedItem as VBTabItem;
            VBDesign vbDesign = VBLogicalTreeHelper.FindChildObject(vbTabItem, typeof(VBDesign)) as VBDesign;
            if (vbDesign != null && vbDesign.BSOACComponent == bso)
                return FocusBSOResult.AlreadyFocused;
            VBTabItem tabitem2Select = null;
            foreach (var tabitem in DockingManager.vbDockingPanelTabbedDoc_TabControl.Items)
            {
                vbTabItem = tabitem as VBTabItem;
                if (vbTabItem != null)
                {
                    vbDesign = VBLogicalTreeHelper.FindChildObject(vbTabItem, typeof(VBDesign)) as VBDesign;
                    if (vbDesign != null && vbDesign.BSOACComponent == bso)
                    {
                        tabitem2Select = vbTabItem;
                        break;
                    }
                }
            }
            if (tabitem2Select != null)
            {
                DockingManager.vbDockingPanelTabbedDoc_TabControl.SelectedItem = tabitem2Select;
                return FocusBSOResult.SelectionSwitched;
            }
            return FocusBSOResult.NotFocusable;
        }

        #endregion

        #region Fractal
        private FractalImagePresenter _FractalImagePresenter;
        private void Generator_Completed(object sender, FractalImageCompletedEventArgs e) { this.image.ImageSource = e.Image; }
#endregion

#region INotifyPropertyChanged Member

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
#endregion

        private void InitConnectionInfo()
        {
            Communications wcfManager = ACRoot.SRoot.GetChildComponent("Communications") as Communications;
            if (wcfManager == null)
                return;
            if (wcfManager.WCFClientManager != null)
            {
                Binding bindingClientIcon = new Binding();
                bindingClientIcon.Source = wcfManager.WCFClientManager;
                bindingClientIcon.Path = new PropertyPath("ConnectionQuality");
                ClientConnIcon.SetBinding(VBConnectionState.ConnectionQualityProperty, bindingClientIcon);

                Binding bindingClientText = new Binding();
                bindingClientText.Source = wcfManager.WCFClientManager;
                bindingClientText.Path = new PropertyPath("ConnectionShortInfo");
                ClientConnText.SetBinding(VBTextBlock.TextProperty, bindingClientText);
            }

            if (wcfManager.WCFServiceManager != null)
            {
                Binding bindingServerIcon = new Binding();
                bindingServerIcon.Source = wcfManager.WCFServiceManager;
                bindingServerIcon.Path = new PropertyPath("ConnectionQuality");
                ServerConnIcon.SetBinding(VBConnectionState.ConnectionQualityProperty, bindingServerIcon);

                Binding bindingServerText = new Binding();
                bindingServerText.Source = wcfManager.WCFServiceManager;
                bindingServerText.Path = new PropertyPath("ConnectionShortInfo");
                ServerConnText.SetBinding(VBTextBlock.TextProperty, bindingServerText);
            }
            else
            {
                ServerConnIcon.Visibility = System.Windows.Visibility.Collapsed;
                ServerConnText.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void ClientConnectionInfoClicked(object sender, RoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (!ACRoot.SRoot.Environment.User.IsSuperuser)
                    return;
                WCFClientManager channelManager = ACRoot.SRoot.ACUrlCommand("?\\Communications\\WCFClientManager") as WCFClientManager;
                if (channelManager != null)
                {
                    channelManager.BroadcastShutdownAllClients();
                }
            }
            else
            {
                if (DockingManager == null)
                    return;
                ACComponent channelManager = (ACComponent)ACRoot.SRoot.ACUrlCommand("?\\Communications\\WCFClientManager");
                if (channelManager != null)
                    DockingManager.ShowDialog(channelManager, "ConnectionInfo", "", false);
            }
        }

        void ServerConnectionInfoClicked(object sender, RoutedEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                WCFServiceManager serviceHost = ACRoot.SRoot.ACUrlCommand("?\\Communications\\WCFServiceManager") as WCFServiceManager;
                if (serviceHost != null)
                    serviceHost.ShutdownClients();
            }
            else
            {
                if (DockingManager == null)
                    return;
                ACComponent serviceHost = (ACComponent)ACRoot.SRoot.ACUrlCommand("?\\Communications\\WCFServiceManager");
                if (serviceHost != null)
                    DockingManager.ShowDialog(serviceHost, "ConnectionInfo", "", false);
            }
        }


#region IRootPageWPF
        delegate Global.MsgResult ShowMsgBoxDelegate(Msg msg, eMsgButton msgButton);
        public Global.MsgResult ShowMsgBox(Msg msg, eMsgButton msgButton)
        {
            // Workaround: Wenn MessageBox in OnApplyTemplate aufgerufen wird, dann findet eine Exception statt weil die Nachrichtenverarbeitungsschleife des Dispatchers noch deaktiviert ist
            // Das findet man über den Zugriff auf eine interne Member heraus:
            //System.Reflection.MemberInfo[] infos = typeof(Dispatcher).GetMember("_disableProcessingCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            Type typeDispatcher = typeof(Dispatcher);
            FieldInfo fieldInfo = typeDispatcher.GetField("_disableProcessingCount", BindingFlags.NonPublic | BindingFlags.Instance);
            int _disableProcessingCount = 0;
            if (fieldInfo != null)
            {
                _disableProcessingCount = (int)fieldInfo.GetValue(this.Dispatcher);
            }
            if (_disableProcessingCount <= 0)
            {
                try
                {
                    return ShowMsgBoxIntern(msg, msgButton);
                }
                catch (InvalidOperationException)
                {
                    ShowMsgBoxDelegate showDel = ShowMsgBoxIntern;
                    DispatcherOperation op = Dispatcher.BeginInvoke(showDel, DispatcherPriority.Loaded, msg, msgButton);
                    //op.Wait();
                    //return (Global.MsgResult) op.Result;
                    return Global.MsgResult.None;
                }
            }
            else
            {
                ShowMsgBoxDelegate showDel = ShowMsgBoxIntern;
                DispatcherOperation op = Dispatcher.BeginInvoke(showDel, DispatcherPriority.Loaded, msg, msgButton);
                //op.Wait();
                //return (Global.MsgResult) op.Result;
                return Global.MsgResult.None;
            }
        }

        private Global.MsgResult ShowMsgBoxIntern(Msg msg, eMsgButton msgButton)
        {
            VBWindowDialogMsg vbMessagebox = new VBWindowDialogMsg(msg, msgButton, this);
            return vbMessagebox.ShowMessageBox();
        }

        public void StoreSettingsWndPos(object settingsVBDesignWndPos)
        {
            if ((settingsVBDesignWndPos == null) || !(settingsVBDesignWndPos is SettingsVBDesignWndPos))
                return;
            SettingsVBDesignWndPos wndPos = settingsVBDesignWndPos as SettingsVBDesignWndPos;
            if (Properties.Settings.Default.DockWndPositions == null)
                Properties.Settings.Default.DockWndPositions = new SettingsVBDesignWndPosList();
            var query = Properties.Settings.Default.DockWndPositions.Where(c => c.ACIdentifier == wndPos.ACIdentifier);
            if (query.Any())
            {
                query.First().WndRect = wndPos.WndRect;
            }
            else
            {
                Properties.Settings.Default.DockWndPositions.Add(wndPos);
            }
        }

        public object ReStoreSettingsWndPos(string acName)
        {
            if (String.IsNullOrEmpty(acName))
                return null;
            if (Properties.Settings.Default.DockWndPositions == null)
            {
                Properties.Settings.Default.DockWndPositions = new SettingsVBDesignWndPosList();
                return null;
            }
            return Properties.Settings.Default.DockWndPositions.Where(c => c.ACIdentifier == acName).FirstOrDefault();
        }


        public string OpenFileDialog(string filter, string initialDirectory = null, bool restoreDirectory = true)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (!String.IsNullOrEmpty(filter))
                dlg.Filter = filter;
            if (!String.IsNullOrEmpty(initialDirectory))
                dlg.InitialDirectory = initialDirectory;
            dlg.RestoreDirectory = restoreDirectory;
            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }

            return null;
        }

        public string SaveFileDialog(string filter, string initialDirectory = null, bool restoreDirectory = true)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            if (!String.IsNullOrEmpty(filter))
                dlg.Filter = filter;
            if (!String.IsNullOrEmpty(initialDirectory))
                dlg.InitialDirectory = initialDirectory;
            dlg.RestoreDirectory = restoreDirectory;
            if (dlg.ShowDialog() == true)
            {
                return dlg.FileName;
            }

            return null;
        }
        #endregion

        #region IACUrl Member
        /// <summary>
        /// The ACUrlCommand is a universal method that can be used to query the existence of an instance via a string (ACUrl) to:
        /// 1. get references to components,
        /// 2. query property values,
        /// 3. execute method calls,
        /// 4. start and stop Components,
        /// 5. and send messages to other components.
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>Result if a property was accessed or a method was invoked. Void-Methods returns null.</returns>
        public object ACUrlCommand(string acUrl, params Object[] acParameter)
        {
            switch (acUrl)
            {
                case Const.CmdShowMsgBox:
                    return ShowMsgBox(acParameter[0] as Msg, (eMsgButton)acParameter[1]);
                case Const.CmdStartBusinessobject:
                    if (acParameter.Count() > 1)
                    {
                        StartBusinessobject(acParameter[0] as string, acParameter[1] as ACValueList);
                    }
                    else
                    {
                        StartBusinessobject(acParameter[0] as string, null);
                    }
                    return null;
                default:
                    return null;
            }
        }

        /// <summary>
        /// This method is called before ACUrlCommand if a method-command was encoded in the ACUrl
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>true if ACUrlCommand can be invoked</returns>
        public bool IsEnabledACUrlCommand(string acUrl, params Object[] acParameter)
        {
            return true;
        }

        /// <summary>
        /// Returns the parent object
        /// </summary>
        /// <value>Reference to the parent object</value>
        public IACObject ParentACObject
        {
            get
            {
                return Parent as IACObject;
            }
        }

        /// <summary>
        /// Method that returns a source and path for WPF-Bindings by passing a ACUrl.
        /// </summary>
        /// <param name="acUrl">ACUrl of the Component, Property or Method</param>
        /// <param name="acTypeInfo">Reference to the iPlus-Type (ACClass)</param>
        /// <param name="source">The Source for WPF-Databinding</param>
        /// <param name="path">Relative path from the returned source for WPF-Databinding</param>
        /// <param name="rightControlMode">Information about access rights for the requested object</param>
        /// <returns><c>true</c> if binding could resolved for the passed ACUrl<c>false</c> otherwise</returns>
        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return false;
        }

        public string GetACUrlComponent(IACObject rootACObject = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a ACUrl relatively to the passed object.
        /// If the passed object is null then the absolute path is returned
        /// </summary>
        /// <param name="rootACObject">Object for creating a realtive path to it</param>
        /// <returns>ACUrl as string</returns>
        [ACMethodInfo("", "", 9999)]
        public string GetACUrl(IACObject rootACObject = null)
        {
            return ACIdentifier;
        }

        /// <summary>
        /// A "content list" contains references to the most important data that this instance primarily works with. It is primarily used to control the interaction between users, visual objects, and the data model in a generic way. For example, drag-and-drop or context menu operations. A "content list" can also be null.
        /// </summary>
        /// <value> A nullable list ob IACObjects.</value>
        public IEnumerable<IACObject> ACContentList
        {
            get
            {
                return null;
            }
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999, "", "")]
        public string ACCaption
        {
            get
            {
                return null;
            }
        }

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        [ACPropertyInfo(9999)]
        public string ACIdentifier
        {
            get
            {
                return this.Name;
            }
        }
        #endregion

        #region IRootPageWPF Member

        /*private List<VBDockingManager> _DockingManagerList = new List<VBDockingManager>();
        public void DockingManagerLoaded(object dockingManager)
        {
            if (dockingManager == null)
                return;
            if (!(dockingManager is VBDockingManager))
                return;
            (dockingManager as VBDockingManager).FreezeActive = FreezeScreenIcon.ControlSelectionActive;
            //(dockingManager as VBDockingManager).ActivateDesignModeSelectionFrame(EditVBDesignIcon.ControlSelectionActive);
            _DockingManagerList.Add(dockingManager as VBDockingManager);
        }

        public void DockingManagerUnLoaded(object dockingManager)
        {
            if (dockingManager == null)
                return;
            if (!(dockingManager is VBDockingManager))
                return;
            (dockingManager as VBDockingManager).FreezeActive = FreezeScreenIcon.ControlSelectionActive;
            //(dockingManager as VBDockingManager).ActivateDesignModeSelectionFrame(EditVBDesignIcon.ControlSelectionActive);
            _DockingManagerList.Remove(dockingManager as VBDockingManager);
        }*/

        /*private void SwitchFreezeStateOfDockingManager()
        {
            foreach (VBDockingManager dockingManager in _DockingManagerList)
            {
                dockingManager.FreezeActive = FreezeScreenIcon.ControlSelectionActive;
            }
        }*/


        #region Blidschirm für nächsten Start einfrieren
        public static readonly DependencyProperty VBDockingManagerFreezingProperty
            = DependencyProperty.Register("VBDockingManagerFreezing", typeof(WPFControlSelectionEventArgs), typeof(Masterpage));

        public WPFControlSelectionEventArgs VBDockingManagerFreezing
        {
            get { return (WPFControlSelectionEventArgs)GetValue(VBDockingManagerFreezingProperty); }
            set { SetValue(VBDockingManagerFreezingProperty, value); }
        }

        // 1. Click auf StatusBar-Icon von Benutzer
        private void FreezeScreenIcon_Click(object sender, RoutedEventArgs e)
        {
            FreezeScreenIcon.SwitchControlSelectionState();
            if (FreezeScreenIcon.ControlSelectionActive)
                VBDockingManagerFreezing = new WPFControlSelectionEventArgs(ControlSelectionState.FrameSearch);
            else
                VBDockingManagerFreezing = new WPFControlSelectionEventArgs(ControlSelectionState.Off);
        }

        // 2. Aufruf vom Dockingmanager, dass Rahmen geklickt worden ist => Schalte Modus aus
        public void DockingManagerFreezed(object dockingManager)
        {
            FreezeScreenIcon.SwitchControlSelectionState();
            if (FreezeScreenIcon.ControlSelectionActive)
                VBDockingManagerFreezing = new WPFControlSelectionEventArgs(ControlSelectionState.FrameSearch);
            else
                VBDockingManagerFreezing = new WPFControlSelectionEventArgs(ControlSelectionState.Off);
        }
#endregion

#region Editierung des VB-Designs

        public static readonly DependencyProperty VBDesignEditingProperty
            = DependencyProperty.Register("VBDesignEditing", typeof(WPFControlSelectionEventArgs), typeof(Masterpage));

        public WPFControlSelectionEventArgs VBDesignEditing
        {
            get { return (WPFControlSelectionEventArgs)GetValue(VBDesignEditingProperty); }
            set { SetValue(VBDesignEditingProperty, value); }
        }

        // 1. Click auf StatusBar-Icon von Benutzer
        // 3. Click wenn Editierung zu Ende ist
        private void EditVBDesignIcon_Click(object sender, RoutedEventArgs e)
        {
            EditVBDesignIcon.SwitchControlSelectionState();
            if (EditVBDesignIcon.ControlSelectionActive)
                VBDesignEditing = new WPFControlSelectionEventArgs(ControlSelectionState.FrameSearch);
            else
                VBDesignEditing = new WPFControlSelectionEventArgs(ControlSelectionState.Off);
        }

        // 2. Aufruf von VBDesign, dass Rahmen geklickt worden ist
        public void VBDesignEditingActivated(object vbDesign)
        {
            if (!EditVBDesignIcon.ControlSelectionActive)
                EditVBDesignIcon.SwitchControlSelectionState();
            VBDesignEditing = new WPFControlSelectionEventArgs(ControlSelectionState.FrameSelected);
        }
#endregion

#endregion

        private void CustomVBTextBlock_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (RenderOptions.ProcessRenderMode == System.Windows.Interop.RenderMode.Default)
                RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
            else
                RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.Default;
        }


        #region IACInteractiveObject Member
        /// <summary>
        /// Metadata (iPlus-Type) of this instance. ATTENTION: IACType are EF-Objects. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>  iPlus-Type (EF-Object from ACClass*-Tables)</value>
        public IACType ACType
        {
            get;
        }


        /// <summary>By setting a ACUrl in XAML, the Control resolves it by calling the IACObject.ACUrlBinding()-Method. 
        /// The ACUrlBinding()-Method returns a Source and a Path which the Control use to create a WPF-Binding to bind the right value and set the WPF-DataContext.
        /// ACUrl's can be either absolute or relative to the DataContext of the parent WPFControl (or the ContextACObject of the parent IACInteractiveObject)</summary>
        /// <value>Relative or absolute ACUrl</value>
        [Category("VBControl")]
        public string VBContent
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// ContextACObject is used by WPF-Controls and mostly it equals to the FrameworkElement.DataContext-Property.
        /// IACInteractiveObject-Childs in the logical WPF-tree resolves relative ACUrl's to this ContextACObject-Property.
        /// </summary>
        /// <value>The Data-Context as IACObject</value>
        public IACObject ContextACObject
        {
            get
            {
                return ACRoot.SRoot;
            }
        }

        /// <summary>
        /// ACAction is called when one IACInteractiveObject (Source) wants to inform another IACInteractiveObject (Target) about an relevant interaction-event.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        public void ACAction(ACActionArgs actionArgs)
        {
        }

        /// <summary>
        /// It's called at the Target-IACInteractiveObject to inform the Source-IACInteractiveObject that ACAction ist allowed to be invoked.
        /// </summary>
        /// <param name="actionArgs">Information about the type of interaction and the source</param>
        /// <returns><c>true</c> if ACAction can be invoked otherwise, <c>false</c>.</returns>
        public bool IsEnabledACAction(ACActionArgs actionArgs)
        {
            return false;
        }
#endregion


        public double Zoom
        {
            get { return this.sldZoom.Value; }
        }

        private bool _startingFullScreen = false;
        private void StartInFullScreen()
        {
            if (mainMenu == null || !mainMenu.Items.Any() || DockingManager == null)
                return;
            if (_RootVBDesign != null
                && DockingManager != null
                && DockingManager.vbDockingPanelTabbedDoc_TabControl != null)
            {
                if (DockingManager.vbDockingPanelTabbedDoc_TabControl.SelectedItem == null)
                {
                    string startUrl = mainMenu.Items.FirstOrDefault().ACUrl;
                    if (String.IsNullOrEmpty(startUrl))
                        return;
                    DockingManager.StartBusinessobject(startUrl, null, null, true);
                }
                _InFullscreen = false;
                SwitchFullScreen();
            }
        }


        public bool InFullscreen
        {
            get { return _InFullscreen; }
        }

        public void SwitchFullScreen()
        {
            if (!_InFullscreen)
            {
                if (_RootVBDesign != null
                    && DockingManager != null
                    && DockingManager.vbDockingPanelTabbedDoc_TabControl != null
                    && DockingManager.vbDockingPanelTabbedDoc_TabControl.SelectedItem != null)
                {
                    TabItem tabItem = DockingManager.vbDockingPanelTabbedDoc_TabControl.SelectedItem as TabItem;
                    _FullscreenContent = tabItem.Content;
                    tabItem.Content = null;
                    this.Content = _FullscreenContent;
                    (this.Content as FrameworkElement).LayoutTransform = SubMainDockPanel.LayoutTransform;
                    (this.Content as FrameworkElement).PreviewTouchDown += new EventHandler<TouchEventArgs>(FullscreenContent_PreviewTouchDown);
                    (this.Content as FrameworkElement).PreviewTouchMove += new EventHandler<TouchEventArgs>(FullscreenContent_PreviewTouchMove);
                    (this.Content as FrameworkElement).PreviewTouchUp += new EventHandler<TouchEventArgs>(FullscreenContent_PreviewTouchUp);
                    this.WindowStyle = System.Windows.WindowStyle.None;
                    this.WindowState = System.Windows.WindowState.Maximized;
                    _InFullscreen = true;
                }
            }
            else
            {
                if (_RootVBDesign != null
                    && _FullscreenContent != null
                    && DockingManager != null
                    && DockingManager.vbDockingPanelTabbedDoc_TabControl != null
                    && DockingManager.vbDockingPanelTabbedDoc_TabControl.SelectedItem != null)
                {
                    this.Content = MainDockPanel;
                    TabItem tabItem = DockingManager.vbDockingPanelTabbedDoc_TabControl.SelectedItem as TabItem;
                    (this.Content as FrameworkElement).PreviewTouchDown -= new EventHandler<TouchEventArgs>(FullscreenContent_PreviewTouchDown);
                    (this.Content as FrameworkElement).PreviewTouchMove -= new EventHandler<TouchEventArgs>(FullscreenContent_PreviewTouchMove);
                    (this.Content as FrameworkElement).PreviewTouchUp -= new EventHandler<TouchEventArgs>(FullscreenContent_PreviewTouchUp);
                    (_FullscreenContent as FrameworkElement).LayoutTransform = null;
                    tabItem.Content = _FullscreenContent;
                    _FullscreenContent = null;
                    this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                    this.WindowState = System.Windows.WindowState.Normal;
                    _InFullscreen = false;
                }
            }
        }

        #region Touchscreen
        private Dictionary<int, Point> _LastPositionDict = new Dictionary<int, Point>();
        private void SubMainDockPanel_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                TouchPoint tp = e.GetTouchPoint(null);
                _LastPositionDict.Add(e.TouchDevice.Id, tp.Position);
            }
            catch (Exception ec)
            {
                string msg = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msg += " Inner:" + ec.InnerException.Message;

                this.Root().Messages.LogException("Masterpage", "SubMainDockPanel_PreviewTouchDown", msg);
            }
        }

        private void SubMainDockPanel_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            TouchPoint tp = e.GetTouchPoint(null);

            Point lastPoint = _LastPositionDict[e.TouchDevice.Id];
            if (_LastPositionDict.Count == 2) // Zwei Finger Eingabe
            {
                // hole rechten oberen Finger
                if (_LastPositionDict.First().Value.X > _LastPositionDict.Last().Value.X
                    && _LastPositionDict.First().Value.Y < _LastPositionDict.Last().Value.Y
                    && _LastPositionDict.First().Value == lastPoint)
                {
                    // Vergrößerung
                    if (Convert.ToInt32(lastPoint.X) < Convert.ToInt32(tp.Position.X) && Convert.ToInt32(lastPoint.Y) > Convert.ToInt32(tp.Position.Y))
                    {
                        sldZoom.Value += Convert.ToInt32(tp.Position.X) - Convert.ToInt32(lastPoint.X);
                    }
                    // Verkleinerung
                    if (Convert.ToInt32(lastPoint.X) > Convert.ToInt32(tp.Position.X) && Convert.ToInt32(lastPoint.Y) < Convert.ToInt32(tp.Position.Y))
                    {
                        sldZoom.Value -= Convert.ToInt32(lastPoint.X) - Convert.ToInt32(tp.Position.X);
                    }
                }
                else if (_LastPositionDict.Last().Value.X > _LastPositionDict.First().Value.X
                    && _LastPositionDict.Last().Value.Y < _LastPositionDict.First().Value.Y
                    && _LastPositionDict.Last().Value == lastPoint)
                {
                    // Vergrößerung
                    if (Convert.ToInt32(lastPoint.X) < Convert.ToInt32(tp.Position.X) && Convert.ToInt32(lastPoint.Y) > Convert.ToInt32(tp.Position.Y))
                    {
                        sldZoom.Value += Convert.ToInt32(tp.Position.X) - Convert.ToInt32(lastPoint.X);
                    }
                    // Verkleinerung
                    if (Convert.ToInt32(lastPoint.X) > Convert.ToInt32(tp.Position.X) && Convert.ToInt32(lastPoint.Y) < Convert.ToInt32(tp.Position.Y))
                    {
                        sldZoom.Value -= Convert.ToInt32(lastPoint.X) - Convert.ToInt32(tp.Position.X);
                    }
                }
            }
            _LastPositionDict[e.TouchDevice.Id] = tp.Position;
        }

        private void SubMainDockPanel_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            TouchPoint tp = e.GetTouchPoint(null);
            _LastPositionDict.Remove(e.TouchDevice.Id);
        }

        private Dictionary<int, Point> _LastPositionFullscreenDict = new Dictionary<int, Point>();
        void FullscreenContent_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            // No Zoom for special user, wenn in Fullscreen
            //if (ACRoot.SRoot.Environment.User.VBUserName.StartsWith("VBTERM"))
            //    return;
        }

        void FullscreenContent_PreviewTouchMove(object sender, TouchEventArgs e)
        {
            // No Zoom for special user, wenn in Fullscreen
            //if (ACRoot.SRoot.Environment.User.VBUserName.StartsWith("VBTERM"))
            //    return;

            TouchPoint tp = e.GetTouchPoint(null);

            Point lastPoint = _LastPositionFullscreenDict[e.TouchDevice.Id];
            if (_LastPositionFullscreenDict.Count == 2) // Zwei Finger Eingabe
            {
                // hole rechten oberen Finger
                if (_LastPositionFullscreenDict.First().Value.X > _LastPositionFullscreenDict.Last().Value.X
                    && _LastPositionFullscreenDict.First().Value.Y < _LastPositionFullscreenDict.Last().Value.Y
                    && _LastPositionFullscreenDict.First().Value == lastPoint)
                {
                    // Vergrößerung
                    if (Convert.ToInt32(lastPoint.X) < Convert.ToInt32(tp.Position.X) && Convert.ToInt32(lastPoint.Y) > Convert.ToInt32(tp.Position.Y))
                    {
                        sldZoom.Value += Convert.ToInt32(tp.Position.X) - Convert.ToInt32(lastPoint.X);
                    }
                    // Verkleinerung
                    if (Convert.ToInt32(lastPoint.X) > Convert.ToInt32(tp.Position.X) && Convert.ToInt32(lastPoint.Y) < Convert.ToInt32(tp.Position.Y))
                    {
                        sldZoom.Value -= Convert.ToInt32(lastPoint.X) - Convert.ToInt32(tp.Position.X);
                    }
                }
                else if (_LastPositionFullscreenDict.Last().Value.X > _LastPositionFullscreenDict.First().Value.X
                    && _LastPositionFullscreenDict.Last().Value.Y < _LastPositionFullscreenDict.First().Value.Y
                    && _LastPositionFullscreenDict.Last().Value == lastPoint)
                {
                    // Vergrößerung
                    if (Convert.ToInt32(lastPoint.X) < Convert.ToInt32(tp.Position.X) && Convert.ToInt32(lastPoint.Y) > Convert.ToInt32(tp.Position.Y))
                    {
                        sldZoom.Value += Convert.ToInt32(tp.Position.X) - Convert.ToInt32(lastPoint.X);
                    }
                    // Verkleinerung
                    if (Convert.ToInt32(lastPoint.X) > Convert.ToInt32(tp.Position.X) && Convert.ToInt32(lastPoint.Y) < Convert.ToInt32(tp.Position.Y))
                    {
                        sldZoom.Value -= Convert.ToInt32(lastPoint.X) - Convert.ToInt32(tp.Position.X);
                    }
                }
            }
            _LastPositionFullscreenDict[e.TouchDevice.Id] = tp.Position;
        }

        void FullscreenContent_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            // No Zoom for special user, wenn in Fullscreen
            //if (Root.SRoot.Environment.User.VBUserName.StartsWith("VBTERM"))
            //    return;

            TouchPoint tp = e.GetTouchPoint(null);
            _LastPositionFullscreenDict.Remove(e.TouchDevice.Id);
        }
#endregion

        private void WarningIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            IACComponent bsoAlarmExplorer = ACRoot.SRoot.Businessobjects.StartComponent("BSOAlarmExplorer", this, null);
            if (bsoAlarmExplorer != null)
            {
                bsoAlarmExplorer.ACUrlCommand("!ShowAlarmExplorer");
                bsoAlarmExplorer.Stop();
            }
        }
    }
}
