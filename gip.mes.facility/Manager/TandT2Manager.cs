using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;


namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandT2Manager'}de{'TandT2Manager'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class TandT2Manager : PARole, ITandTFetchCharge
    {

        #region constants
        public static string SearchModel_ParamValueKey = @"JobFilter";
        #endregion

        #region Configuration

        private const string _BSOName = "BSOTandTv2";
        private ACPropertyConfigValue<string> _TandTBSOName;
        [ACPropertyConfig("en{'T&T-BSO-StartURL'}de{'T&T-BSO-StartURL'}")]
        public virtual string TandTBSOName
        {
            get
            {
                return _TandTBSOName.ValueT;
            }
            set
            {
                _TandTBSOName.ValueT = value;
            }
        }

        #endregion

        #region c´tors
        public TandT2Manager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _TandTBSOName = new ACPropertyConfigValue<string>(this, "TandTBSOName", Const.BusinessobjectsACUrl + ACUrlHelper.Delimiter_Start + "BSOTandTv2");
        }
        #endregion

        #region Manager instancing static methods
        public const string C_DefaultServiceACIdentifier = "TandT2Manager";

        public static TandT2Manager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<TandT2Manager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<TandT2Manager> ACRefToServiceInstance(ACComponent requester)
        {
            TandT2Manager serviceInstance = GetServiceInstance(requester) as TandT2Manager;
            if (serviceInstance != null)
                return new ACRef<TandT2Manager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region Calling BSO
        public void StartTandTBSO(ACBSO bso, TandTv2Job job)
        {
            string tandtBSOName = TandTBSOName;
            if (String.IsNullOrEmpty(tandtBSOName))
                return;
            gip.core.datamodel.ACClass acClassTT = null;
            using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
            {
                acClassTT = gip.core.datamodel.Database.GlobalDatabase.ACClass.Where(c => c.ACIdentifier == _BSOName).FirstOrDefault();
            }
            if (acClassTT == null)
                return;

            ACMethod acMethod = acClassTT.TypeACSignature();
            acMethod.ParameterValueList[SearchModel_ParamValueKey] = job;
            bso.Root.RootPageWPF.StartBusinessobject(tandtBSOName, acMethod.ParameterValueList);
        }

        #endregion

        #region Methods

        #region Methods -> Public

        public TandTv2Result DoTracking(TandTv2Job jobFilter, string vBUserNo)
        {
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                return DoTracking(databaseApp, jobFilter, vBUserNo);
            }
        }

        public TandTv2Result DoSelect(TandTv2Job jobFilter)
        {
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                return DoSelect(databaseApp, jobFilter);
            }
        }

        public void ApplyFilter(TandTv2Result result, List<TandTv2ItemTypeEnum> displayTypesForPrevew, bool buildGraphic = false)
        {
            // build a filtered list
            result.FilteredStepItems = result
                       .StepItems
                       .Where(c =>
                           displayTypesForPrevew.Contains(c.TandT_ItemTypeEnum)/* &&
                            (FilterDateFrom == null || FilterDateFrom <= c.InsertDate) &&
                            (FilterDateTo == null || FilterDateTo > c.InsertDate)*/
                           )
                       .OrderBy(c => c.TandTv2Step.StepNo)
                       .ToList();

            if (buildGraphic)
                BuildGraphic(result);
        }

        public KeyValuePair<Msg, TandTv2Job> DoDeleteTracking(TandTv2Job selectedJob)
        {
            using (DatabaseApp databaseApp = new DatabaseApp())
            {
                return DoDeleteTracking(databaseApp, selectedJob);
            }
        }

        #endregion

        #region Methods -> Public (context forwarded)
        public TandTv2Result DoTracking(DatabaseApp databaseApp, TandTv2Job jobFilter, string vBUserNo)
        {
            return TandTv2Command.DoTracking(databaseApp, jobFilter, vBUserNo);
        }

        protected TandTv2Result DoSelect(DatabaseApp databaseApp, TandTv2Job jobFilter)
        {
            return TandTv2Command.DoSelect(databaseApp, jobFilter);
        }

        protected KeyValuePair<Msg, TandTv2Job> DoDeleteTracking(DatabaseApp databaseApp, TandTv2Job selectedJob)
        {
            MsgWithDetails msg = new MsgWithDetails();
            try
            {
                databaseApp.udpTandTv2JobDelete(selectedJob.TandTv2JobID);
                msg.MessageLevel = eMsgLevel.Info;
                msg.Message = "Successfully deleted T&T item!";

            }
            catch (Exception ec)
            {
                msg.MessageLevel = eMsgLevel.Error;
                msg.Message = ec.Message;
            }
            return new KeyValuePair<Msg, TandTv2Job>(msg, selectedJob);
        }

        #endregion

        #endregion

        #region private methods

        private static void BuildGraphic(TandTv2Result result)
        {
            // Clean up helper lists
            foreach (var item in result.StepItems)
            {
                //item.HelperSourceItems = null;
                //item.HelperTargetItems = null;
            }
            // Add items from relations to helper list
            TandTv2Command.BuildStepItemHelperSourceTargetList(result);
            // Build helper list on filtered list result
            TandTv2Command.BuildFilteredStepItemHelperSourceTargetList(result);
            // Build relations on Helper list
            TandTv2Command.BuildFilteredStepItemRelations(result);
            // Generate edges from relations
            TandTv2Command.BuildEdges(result);
        }

        #endregion

        #region ITandTFetchCharge

        public List<FacilityCharge> GetFacilityChargesBackward(DatabaseApp databaseApp, FacilityBooking facilityBooking, string userInitials, Func<object, bool> breakTrackingCondition)
        {
            List<FacilityCharge> facilityCharges = new List<FacilityCharge>();
            TandTv2Job job = new TandTv2Job();
            job.ItemSystemNo = facilityBooking.FacilityBookingNo;
            job.PrimaryKeyID = facilityBooking.FacilityBookingID;
            job.ItemTypeEnum = TandTv2ItemTypeEnum.FacilityBooking;
            job.TrackingStyleEnum = TandTv2TrackingStyleEnum.Backward;
            job.RecalcAgain = true;
            job.IsDynamic = true;
            TandTv2Result tandTResult = DoTracking(databaseApp, job, Root.CurrentInvokingUser.Initials);
            if (tandTResult.Success)
                facilityCharges = tandTResult
                                   .StepItems
                                   .Where(c => c.FacilityCharge != null)
                                   .Select(c => c.FacilityCharge)
                                   //.Where(x => x.IsNoConnectionWithProduction)
                                   .ToList();
            return facilityCharges;
        }


        #endregion

    }
}
