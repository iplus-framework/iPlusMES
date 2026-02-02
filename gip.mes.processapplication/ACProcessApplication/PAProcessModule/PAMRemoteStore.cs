// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.core.processapplication;
using gip.mes.datamodel;
using gip.mes.facility;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Shared store (remote)'}de{'Gemeinsam verwendetes Lager'}", Global.ACKinds.TPAProcessModule, Global.ACStorableTypes.Required, false, PWGroupVB.PWClassName, true)]
    public class PAMRemoteStore : PAMParkingspace, IRemoteFacilityForwarder
    {
        #region c'tors
        static PAMRemoteStore()
        {
            RegisterExecuteHandler(typeof(PAMRemoteStore), HandleExecuteACMethod_PAMRemoteStore);
        }

        public PAMRemoteStore(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _Broadcaster = new RemoteFacilityBroadcaster(this);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool initialized = base.ACInit(startChildMode);
            if (initialized && RemoteStorePostings == null)
                RemoteStorePostings = new SafeList<RemoteStorePostingData>();
            return initialized;
        }

        public override bool ACPostInit()
        {
            StartRedirection();
            return base.ACPostInit();
        }

        public override async Task<bool> ACDeInit(bool deleteACClassTask = false)
        {
            StopRedirection();
            return await base.ACDeInit(deleteACClassTask);
        }

        #endregion

        #region Points
        [ACPropertyPointConfig(9999, "", typeof(RemoteStoreConfig), "en{'Remote Facility Manager'}de{'Entfernte Facility Manager'}")]
        public List<core.datamodel.ACClassConfig> RemoteFacilityManager
        {
            get
            {
                return _Broadcaster.GetRemoteFacilityManager();
            }
        }
        #endregion

        #region Properties
        private RemoteFacilityBroadcaster _Broadcaster;

        [ACPropertyInfo(true, 400, DefaultValue = false)]
        public bool ConnectionDisabled
        {
            get;
            set;
        }

        [ACPropertyBindingSource]
        public IACContainerTNet<bool> IsActive { get; set; }

        SafeList<RemoteStorePostingData> _RemoteStorePostings;
        [ACPropertyInfo(true, 520, "", "en{'RemoteStorePostings'}de{'RemoteStorePostings'}", "", false)]
        public SafeList<RemoteStorePostingData> RemoteStorePostings
        {
            get
            {
                return _RemoteStorePostings;
            }
            set
            {
                _RemoteStorePostings = value;
                OnPropertyChanged();
            }
        }

        public string MethodNameOfRFM
        {
            get
            {
                return nameof(gip.mes.facility.RemoteFacilityManager.RefreshFacility);
            }
        }

        [ACPropertyInfo(true, 401, DefaultValue = false)]
        public bool LoggingOn { get; set; }

        #endregion


        #region Methods
        public override void RefreshFacility(bool preventBroadcast, Guid? fbID)
        {
            base.RefreshFacility(preventBroadcast, fbID);

            if (   Facility.ValueT == null 
                || Facility.ValueT.ValueT == null 
                || preventBroadcast
                || !_Broadcaster.RemoteInstances.Any()
                || !IsActive.ValueT)
                return;
            SendToRemoteStore(FacilityBooking.ClassName, fbID);
        }

        [ACMethodInfo("", "en{'Send Picking order'}de{'Sende Kommissionierauftrag'}", 401, true)]
        public virtual void SendPicking(bool preventBroadcast, Guid? pickingID)
        {
            if (Facility.ValueT == null
                || Facility.ValueT.ValueT == null
                || preventBroadcast
                || !_Broadcaster.RemoteInstances.Any()
                || !IsActive.ValueT)
                return;

            SendToRemoteStore(Picking.ClassName, pickingID);
        }

        protected virtual void SendToRemoteStore(string entityType, Guid? keyID)
        {
            _Broadcaster.SendToRemoteStore(entityType, keyID);
        }

        [ACMethodInteraction("Run", "en{'Activate redirection'}de{'Umleitung aktivieren'}", 200, true)]
        public void StartRedirection()
        {
            _Broadcaster.StartRedirection();
        }

        public virtual bool IsEnabledStartRedirection()
        {
            return _Broadcaster.IsEnabledStartRedirection();
        }

        [ACMethodInteraction("Run", "en{'Stop redirection'}de{'Umleitung beenden'}", 201, true)]
        public void StopRedirection()
        {
            _Broadcaster.StopRedirection();
        }

        public virtual bool IsEnabledStopRedirection()
        {
            return _Broadcaster.IsEnabledStopRedirection();
        }

        [ACMethodInteraction("", "en{'Add this store on remote Systems'}de{'Diesen Lagerplatz auf entfernten Systemen anlegen'}", 202, true)]
        public void SynchronizeThisFacility()
        {
            if (!IsEnabledSynchronizeThisFacility())
                return;
            SendToRemoteStore(gip.mes.datamodel.Facility.ClassName, this.Facility.ValueT.ValueT.FacilityID);
        }

        public bool IsEnabledSynchronizeThisFacility()
        {
            return IsEnabledStopRedirection() 
                && this.Facility.ValueT != null 
                && this.Facility.ValueT.ValueT != null;
        }

        #endregion


        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case nameof(StartRedirection):
                    StartRedirection();
                    return true;
                case nameof(StopRedirection):
                    StopRedirection();
                    return true;
                case nameof(SynchronizeThisFacility):
                    SynchronizeThisFacility();
                    return true;
                case Const.IsEnabledPrefix + nameof(StartRedirection):
                    result = IsEnabledStartRedirection();
                    return true;
                case Const.IsEnabledPrefix + nameof(StopRedirection):
                    result = IsEnabledStopRedirection();
                    return true;
                case Const.IsEnabledPrefix + nameof(SynchronizeThisFacility):
                    result = IsEnabledSynchronizeThisFacility();
                    return true;
                case nameof(SendPicking):
                    SendPicking((bool)acParameter[0], (Guid?)acParameter[1]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        public static bool HandleExecuteACMethod_PAMRemoteStore(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAMParkingspace(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }
        #endregion
    }

}
