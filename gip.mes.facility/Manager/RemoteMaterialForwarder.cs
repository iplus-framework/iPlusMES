// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Globalization;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "", Global.ACKinds.TPARole, Global.ACStorableTypes.Required, false, false)]
    public class RemoteMaterialForwarder : PAClassAlarmingBase, IRemoteFacilityForwarder
    {
        #region c'tors
        public RemoteMaterialForwarder(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
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

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            StopRedirection();
            return base.ACDeInit(deleteACClassTask);
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
                return nameof(gip.mes.facility.RemoteFacilityManager.RefreshMaterial);
            }
        }

        [ACPropertyInfo(true, 401, DefaultValue = false)]
        public bool LoggingOn { get; set; }

        #endregion


        #region Methods
        public static bool ForwardMaterial(ACComponent requester, string acUrlOfForwarder, Guid materialID)
        {
            ACComponent aCComponent = requester.ACUrlCommand(acUrlOfForwarder) as ACComponent;
            if (aCComponent == null)
                return false;
            if (aCComponent.ConnectionState == ACObjectConnectionState.DisConnected)
                return false;
            aCComponent.ExecuteMethod(nameof(SendMaterial), materialID);
            return true;
        }

        [ACMethodInfo("", "en{'Synchronize remote material'}de{'Synchronize remote material'}", 1001)]
        public void SendMaterial(Guid materialID)
        {
            SendToRemoteStore(Material.ClassName, materialID);
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

        [ACMethodInteraction("", "en{'Synchronize all materials'}de{'Alle Materialien synchronisieren'}", 202, true)]
        public void SynchronizeAllMaterials()
        {
            if (!IsEnabledSynchronizeAllMaterials())
                return;
            SendToRemoteStore(Material.ClassName, Guid.Empty);
        }

        public bool IsEnabledSynchronizeAllMaterials()
        {
            return IsEnabledStopRedirection();
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
                case nameof(SynchronizeAllMaterials):
                    SynchronizeAllMaterials();
                    return true;
                case Const.IsEnabledPrefix + nameof(StartRedirection):
                    result = IsEnabledStartRedirection();
                    return true;
                case Const.IsEnabledPrefix + nameof(StopRedirection):
                    result = IsEnabledStopRedirection();
                    return true;
                case Const.IsEnabledPrefix + nameof(SynchronizeAllMaterials):
                    result = IsEnabledSynchronizeAllMaterials();
                    return true;
                case nameof(SendMaterial):
                    SendMaterial((Guid)acParameter[0]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion
    }

}
