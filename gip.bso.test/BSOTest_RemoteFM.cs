using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [ACMethodInfo("RecieveRemotePicking", "en{'Recive remote Picking'}de{'Recive remote Picking'}", 999)]
        public void RecieveRemotePicking()
        {
            if (!IsEnabledRecieveRemotePicking())
                return;
            CallRemoteFacilityManagerForPicking(SelectedRemoteFacilityManagerInfo.RemoteFacilityManager, RemotePickingNo, SelectedRemoteFacilityManagerInfo.RemoteConnString);
        }

        public bool IsEnabledRecieveRemotePicking()
        {
            return !string.IsNullOrEmpty(RemotePickingNo) && SelectedRemoteFacilityManagerInfo != null;
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
            RemoteStorePostingData remoteStorePostingData = GetRemoteStorePostingData(RemotePickingNo, SelectedRemoteFacilityManagerInfo.RemoteConnString);
            fm.SynchronizeFacility(this, Messages, PickingManager, SelectedRemoteFacilityManagerInfo.RemoteConnString, remoteStorePostingData);

        }

        public bool IsEnabledReciveRemotePickingLocal()
        {
            return !string.IsNullOrEmpty(RemotePickingNo) && SelectedRemoteFacilityManagerInfo != null;
        }

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

        private string GetRemoteFacilityManagerConnectionString(RemoteFacilityManager remoteFacilityManager1)
        {
            List<string> hierarchy = ACUrlHelper.ResolveParents(remoteFacilityManager1.ACUrl);
            string remoteProxyACurl = hierarchy.FirstOrDefault();
            if (String.IsNullOrEmpty(remoteProxyACurl))
                return null;
            ACComponent remoteAppManager = ACUrlCommand(remoteProxyACurl) as ACComponent;
            if (remoteAppManager == null)
                return null;
            return remoteAppManager[nameof(RemoteAppManager.RemoteConnString)] as string;
        }

        private void CallRemoteFacilityManagerForPicking(ACComponent remoteFacilityManager, string pickingNo, string remoteConnString)
        {
            RemoteStorePostingData remoteStorePostingData = GetRemoteStorePostingData(pickingNo, remoteConnString);

            if (remoteStorePostingData != null)
            {
                remoteFacilityManager.ACUrlCommand("!SynchronizeFacility", remoteFacilityManager.ACUrl, remoteConnString, remoteStorePostingData, false);
            }
        }


        private RemoteStorePostingData GetRemoteStorePostingData(string pickingNo, string remoteConnString)
        {
            RemoteStorePostingData remoteStorePostingData = null;

            Picking picking = null;

            using (DatabaseApp remoteDbApp = new DatabaseApp(remoteConnString))
            {
                picking = remoteDbApp.Picking.Where(c => c.PickingNo == pickingNo).FirstOrDefault();
                if (picking != null)
                {
                    remoteStorePostingData = new RemoteStorePostingData();
                    Guid[] faciltiyBookingIDs = picking.PickingPos_Picking.SelectMany(c => c.FacilityBooking_PickingPos).Select(c => c.FacilityBookingID).ToArray();
                    Guid facilityID = Guid.Empty;
                    foreach (PickingPos pickingPos in picking.PickingPos_Picking.ToArray())
                    {
                        if (pickingPos.FromFacility != null)
                        {
                            facilityID = pickingPos.FromFacility.FacilityID;
                            break;
                        }
                        if (pickingPos.ToFacility != null)
                        {
                            facilityID = pickingPos.ToFacility.FacilityID;
                            break;
                        }
                    }

                    remoteStorePostingData.FBIds.Add(
                        new RSPDEntry()
                        {
                            EntityType = nameof(Picking),
                            KeyId = picking.PickingID
                        });

                    foreach (Guid id in faciltiyBookingIDs)
                    {
                        remoteStorePostingData.FBIds.Add(
                        new RSPDEntry()
                        {
                            EntityType = nameof(FacilityBooking),
                            KeyId = id
                        });
                    }
                }
            }
            return remoteStorePostingData;
        }

        #endregion

    }
}
