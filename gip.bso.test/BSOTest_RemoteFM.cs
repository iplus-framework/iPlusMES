// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.bso.test
{
    public partial class BSOTest
    {

        #region Properties

        private string _InputRemoteFM;
        [ACPropertyInfo(999, "InputRemoteFM", "en{'RemoteFM ACUrl'}de{'RemoteFM ACUrl'}")]
        public string InputRemoteFM
        {
            get
            {
                return _InputRemoteFM;
            }
            set
            {
                if (_InputRemoteFM != value)
                {
                    _InputRemoteFM = value;
                    OnPropertyChanged();
                }
            }
        }


        /// <summary>
        /// Source Property: 
        /// </summary>
        private string _RemotePickingNo;
        [ACPropertyInfo(999, "RemotePickingNo", "en{'Remote PickingNo'}de{'Remote PickngNo'}")]
        public string RemotePickingNo
        {
            get
            {
                return _RemotePickingNo;
            }
            set
            {
                if (_RemotePickingNo != value)
                {
                    _RemotePickingNo = value;
                    OnPropertyChanged(nameof(RemotePickingNo));
                }
            }
        }

        private RemoteFacilityModel _SelectedRemoteFacilityManagerInfo;
        /// <summary>
        /// Selected property for RemoteFacilityManagerInfo
        /// </summary>
        /// <value>The selected RemoteFacilityManagerInfo</value>
        [ACPropertySelected(9999, "PropertyGroupName", "en{'TODO: RemoteFacilityManagerInfo'}de{'TODO: RemoteFacilityManagerInfo'}")]
        public RemoteFacilityModel SelectedRemoteFacilityManagerInfo
        {
            get
            {
                return _SelectedRemoteFacilityManagerInfo;
            }
            set
            {
                if (_SelectedRemoteFacilityManagerInfo != value)
                {
                    _SelectedRemoteFacilityManagerInfo = value;
                    OnPropertyChanged(nameof(SelectedRemoteFacilityManagerInfo));
                }
            }
        }


        private List<RemoteFacilityModel> _RemoteFacilityManagerInfoList;
        /// <summary>
        /// List property for RemoteFacilityManagerInfo
        /// </summary>
        /// <value>The RemoteFacilityManagerInfo list</value>
        [ACPropertyList(9999, "PropertyGroupName")]
        public List<RemoteFacilityModel> RemoteFacilityManagerInfoList
        {
            get
            {
                if (_RemoteFacilityManagerInfoList == null)
                    _RemoteFacilityManagerInfoList = LoadRemoteFacilityManagerInfoList();
                return _RemoteFacilityManagerInfoList;
            }
        }

        #endregion

        #region Methods

        #region Methods -> ACMethods

        [ACMethodInfo("AddRemoteFM", "en{'Add RemoteFM'}de{'Add RemoteFM'}", 9999, false, false, true, Global.ACKinds.MSMethodPrePost)]
        public virtual void AddRemoteFM()
        {
            if (!IsEnabledAddRemoteFM())
                return;

            IACComponent component = Root.ACUrlCommand(InputRemoteFM) as IACComponent;
            if (component != null && component is ACComponent)
            {
                RemoteFacilityModel info = new RemoteFacilityModel(component as ACComponent);
                RemoteFacilityManagerInfoList.Add(info);
                OnPropertyChanged(nameof(RemoteFacilityManagerInfoList));
                SelectedRemoteFacilityManagerInfo = info;
            }
        }

        public bool IsEnabledAddRemoteFM()
        {
            return !string.IsNullOrEmpty(InputRemoteFM);
        }

        /// <summary>
        /// Source Property: ReciveRemotePicking
        /// </summary>
        [ACMethodInfo("ReciveRemotePickingLocal", "en{'Recive remote Picking local'}de{'Recive remote Picking local'}", 999)]
        public void ReciveRemotePickingLocal()
        {
            if (!IsEnabledReciveRemotePickingLocal())
                return;
            RemoteFMHelper fm = new RemoteFMHelper();
            RemoteStorePostingData remoteStorePostingData = fm.GetRemoteStorePostingData(RemotePickingNo, SelectedRemoteFacilityManagerInfo.RemoteConnString);
            fm.SynchronizeFacility(this, Messages,ACFacilityManager, PickingManager, SelectedRemoteFacilityManagerInfo.RemoteConnString, remoteStorePostingData);
        }

        public bool IsEnabledReciveRemotePickingLocal()
        {
            return !string.IsNullOrEmpty(RemotePickingNo) && SelectedRemoteFacilityManagerInfo != null;
        }

        #endregion

        #region Methods -> Helper methods

        private List<RemoteFacilityModel> LoadRemoteFacilityManagerInfoList()
        {
            List<RemoteFacilityModel> rmList = new List<RemoteFacilityModel>();

            gip.core.datamodel.ACClass rmClass = DatabaseApp.ContextIPlus.ACClass.FirstOrDefault(c => c.ACIdentifier == "RemoteFacilityManager");

            List<ACComponent> remoteFacilityManagers = new List<ACComponent>();
            IACComponent[] appManagers = Root.ACComponentChilds.Where(c => c is ApplicationManager || c is ApplicationManagerProxy).ToArray();
            foreach (IACComponent appManager in appManagers)
            {
                IACComponent remoteFacilityManager = appManager.FindChildComponents(rmClass, 0).FirstOrDefault();
                if (remoteFacilityManager != null)
                {
                    remoteFacilityManagers.Add(remoteFacilityManager as ACComponent);
                }
            }

            foreach (ACComponent remoteFacilityManager in remoteFacilityManagers)
            {

                RemoteFacilityModel rm = new RemoteFacilityModel(remoteFacilityManager);
                rmList.Add(rm);
            }

            return rmList;
        }

        private void CallRemoteFacilityManagerForPicking(ACComponent remoteFacilityManager, string pickingNo, string remoteConnString)
        {
            RemoteFMHelper fm = new RemoteFMHelper();

            RemoteStorePostingData remoteStorePostingData = fm.GetRemoteStorePostingData(pickingNo, remoteConnString);

            if (remoteStorePostingData != null)
            {
                remoteFacilityManager.ACUrlCommand("!SynchronizeFacility", remoteFacilityManager.ACUrl, remoteConnString, remoteStorePostingData, false);
            }
        }

        

        #endregion

        #endregion

    }
}
