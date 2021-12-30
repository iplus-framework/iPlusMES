using System;
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
    public class RemoteFacilityManager : PARole
    {
        #region ctor's
        public RemoteFacilityManager(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool result = base.ACInit(startChildMode);
            _DelegateQueue = new ACDelegateQueue(ACIdentifier);
            _DelegateQueue.StartWorkerThread();
            return result;
        }

        public override bool ACPostInit()
        {
            return base.ACPostInit();
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_DelegateQueue != null)
            {
                _DelegateQueue.StopWorkerThread();
                _DelegateQueue = null;
            }
            bool result = base.ACDeInit(deleteACClassTask);
            return result;
        }


        #endregion

        #region Properties
        private ACDelegateQueue _DelegateQueue = null;
        public ACDelegateQueue DelegateQueue
        {
            get
            {
                return _DelegateQueue;
            }
        }
        #endregion

        #region static Methods
        #endregion

        #region private 
        #endregion

        #region Methods
        [ACMethodInfo("", "en{'Synchronize remote facility'}de{'Synchronize remote facility'}", 1000)]
        public void RefreshFacility(string acUrlOfCaller, RemoteStorePostingData remoteStorePosting)
        {
            if (String.IsNullOrEmpty(acUrlOfCaller) || remoteStorePosting == null)
                return;
            List<string> hierarchy = ACUrlHelper.ResolveParents(acUrlOfCaller);
            string remoteProxyACurl = hierarchy.FirstOrDefault();
            if (String.IsNullOrEmpty(remoteProxyACurl))
                return;
            ACComponent remoteAppManager = ACUrlCommand(remoteProxyACurl) as ACComponent;
            if (remoteAppManager == null)
                return;
            string remoteConnString = remoteAppManager["RemoteConnString"] as string;
            if (String.IsNullOrEmpty(remoteConnString))
                return;
            DelegateQueue.Add(() => { SynchronizeFacility(acUrlOfCaller, remoteConnString, remoteStorePosting); });
        }

        protected void SynchronizeFacility(string acUrlOfCaller, string remoteConnString, RemoteStorePostingData remoteStorePosting)
        {
            try
            {
                using (DatabaseApp dbRemote = new DatabaseApp(remoteConnString))
                using (DatabaseApp dbLocal = new DatabaseApp())
                {
                    Material m1 = dbRemote.Material.FirstOrDefault();
                    Material m2 = dbLocal.Material.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Messages.LogException(this.GetACUrl(), "SynchronizeFacility", e);
            }
        }
        #endregion

        #region Execute-Helper-Handlers

        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "RefreshFacility":
                    RefreshFacility((string)acParameter[0], (RemoteStorePostingData)acParameter[1]);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }

        #endregion

    }

}
