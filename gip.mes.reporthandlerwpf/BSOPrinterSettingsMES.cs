// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.reporthandler;
using gip.core.reporthandlerwpf;
using gip.mes.datamodel;
using gip.mes.facility;
using System.ComponentModel;

namespace gip.mes.reporthandlerwpf
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Printer settings MES'}de{'Drucker-Einstellungen MES'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
    public class BSOPrinterSettingsMES : BSOPrinterSettings
    {
        #region const
        #endregion

        #region ctor's
        /// <summary>
        /// Creates a new instance of the BSOPropertyLogRules.
        /// </summary>
        /// <param name="acType">The acType parameter.</param>
        /// <param name="content">The content parameter.</param>
        /// <param name="parentACObject">The parentACObject parameter.</param>
        /// <param name="parameter">The parameters in the ACValueList.</param>
        /// <param name="acIdentifier">The acIdentifier parameter.</param>
        public BSOPrinterSettingsMES(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
            // 
        }

        /// <summary>
        /// Initializes this component.
        /// </summary>
        /// <param name="startChildMode">The start child mode parameter.</param>
        /// <returns>True if is initialization success, otherwise returns false.</returns>
        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseInit = base.ACInit(startChildMode);
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                CurrentFacilityRoot = FacilityTree.LoadFacilityTree(databaseApp);
                CurrentFacility = CurrentFacilityRoot;
            }
            return baseInit;
        }

        /// <summary>
        ///  Deinitializes this component.
        /// </summary>
        /// <param name="deleteACClassTask">The deleteACClassTask parameter.</param>
        /// <returns>True if is deinitialization success, otherwise returns false.</returns>
        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            bool done = base.ACDeInit(deleteACClassTask);
            if (_PrintManager != null)
                ACPrintManager.DetachACRefFromServiceInstance(this, _PrintManager);
            _PrintManager = null;
            return done;
        }
        #endregion

        #region Properties
        #region Properties -> FacilityTree

        ACFSItem _CurrentFacilityRoot;
        ACFSItem _CurrentFacility;


        /// <summary>
        /// Gets or sets the current import project item root.
        /// </summary>
        /// <value>The current import project item root.</value>
        [ACPropertyCurrent(9999, "FacilityRoot")]
        public ACFSItem CurrentFacilityRoot
        {
            get
            {
                return _CurrentFacilityRoot;
            }
            set
            {
                _CurrentFacilityRoot = value;
                OnPropertyChanged("CurrentFacilityRoot");
            }

        }

        /// <summary>
        /// Gets or sets the current import project item.
        /// </summary>
        /// <value>The current import project item.</value>
        [ACPropertyCurrent(9999, "Facility")]
        public ACFSItem CurrentFacility
        {
            get
            {
                return _CurrentFacility;
            }
            set
            {
                if (_CurrentFacility != value)
                {
                    if (_CurrentFacility != null && _CurrentFacility.ACObject != null)
                        (_CurrentFacility.ACObject as INotifyPropertyChanged).PropertyChanged -= _CurrentFacility_PropertyChanged;
                    _CurrentFacility = value;
                    if (_CurrentFacility != null && _CurrentFacility.ACObject != null)
                        (_CurrentFacility.ACObject as INotifyPropertyChanged).PropertyChanged += _CurrentFacility_PropertyChanged;
                    if (value != null && value.Value != null)
                    {
                        SelectedMachine = null;
                        LocationName = (value.Value as Facility).FacilityNo;
                    }
                    OnPropertyChanged("CurrentFacility");
                    OnPropertyChanged("SelectedFacility");
                }
            }
        }

        private void _CurrentFacility_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FacilityNo" || e.PropertyName == "FacilityName")
            {
                ACFSItem current = CurrentFacility;
                current.ACCaption = FacilityTree.FacilityACCaption(CurrentFacility.ACObject as Facility);
                CurrentFacilityRoot = FacilityTree.GetNewRootFacilityACFSItem(Database as gip.core.datamodel.Database, CurrentFacilityRoot.Items);
                CurrentFacility = current;
            }
        }

        [ACPropertyInfo(9999, "SelectedFacility")]
        public Facility SelectedFacility
        {
            get
            {
                if (CurrentFacility != null && CurrentFacility.ACObject != null)
                    return CurrentFacility.ACObject as Facility;
                return null;
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Methods -> Overrides
        public override void LoadConfiguredPrinters()
        {
            if (PrintManager == null)
                return;
            ConfiguredPrinterList = ACPrintManager.GetConfiguredPrinters(Database as gip.core.datamodel.Database, PrintManager.ComponentClass.ACClassID, false);
            foreach (PrinterInfo pInfo in ConfiguredPrinterList)
            {
                pInfo.Attach(Db);
            }
        }


        public override void OnSelectedMachineChanged(ACItem machine)
        {
            if (machine != null)
                CurrentFacility = null;
        }

        public override void SetPrinterTarget(PrinterInfo printerInfo)
        {
            if (!String.IsNullOrEmpty(LocationName))
                printerInfo.MachineACUrl = LocationName;
            else if (SelectedFacility != null)
            {
                printerInfo.FacilityID = SelectedFacility.FacilityID;
                printerInfo.FacilityNo = SelectedFacility.FacilityNo;
            }
        }

        public override bool IsEnabledAddPrinter()
        {
            if (ConfiguredPrinterList == null)
                return false;
            return
                    (!String.IsNullOrEmpty(LocationName) || SelectedFacility != null || SelectedVBUser != null)
                && (SelectedWindowsPrinter != null || SelectedPrintServer != null)
                && !ConfiguredPrinterList.Any(c =>
                    (
                            (SelectedWindowsPrinter != null && c.PrinterName == SelectedWindowsPrinter.PrinterName)
                        || (SelectedPrintServer != null && c.PrinterACUrl == SelectedPrintServer.PrinterACUrl)
                    )
                    && ((!String.IsNullOrEmpty(LocationName) && c.MachineACUrl == LocationName)
                        || (SelectedFacility != null && c.FacilityID == SelectedFacility.FacilityID))
                );
        }

        #endregion

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(IsEnabledAddPrinter):
                    result = IsEnabledAddPrinter();
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }
}
