using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Linq;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'ACTrackingAndTracingManager'}de{'ACTrackingAndTracingManager'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class ACTrackingAndTracingManager : PARole, ITandTDriver
    {
        #region c´tors
        public ACTrackingAndTracingManager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _TandTBSOName = new ACPropertyConfigValue<string>(this, "TandTBSOName", Const.BusinessobjectsACUrl + ACUrlHelper.Delimiter_Start + "BSOTrackingAndTracing");
        }
        #endregion

        #region static Methods
        public const string C_DefaultServiceACIdentifier = "ACTrackingAndTracingManager";

        public static ACTrackingAndTracingManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACTrackingAndTracingManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ACTrackingAndTracingManager> ACRefToServiceInstance(ACComponent requester)
        {
            ACTrackingAndTracingManager serviceInstance = GetServiceInstance(requester) as ACTrackingAndTracingManager;
            if (serviceInstance != null)
                return new ACRef<ACTrackingAndTracingManager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region ITandTDriver

        public TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, FacilityBooking facilityBooking, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter)
        {
            TrackingAndTracingResult result = null;
            switch (searchModel)
            {
                case GlobalApp.TrackingAndTracingSearchModel.Backward:
                    result = TransformToPresentation(dbApp, TrackingBackward(dbApp, facilityBooking, filter));
                    break;
                case GlobalApp.TrackingAndTracingSearchModel.Forward:
                    result = TransformToPresentation(dbApp, TrackingForward(dbApp, facilityBooking, filter));
                    break;
            }
            return result;
        }

        public TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, FacilityPreBooking facilitiyPreBooking, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter)
        {
            TrackingAndTracingResult result = null;
            switch (searchModel)
            {
                case GlobalApp.TrackingAndTracingSearchModel.Backward:
                    result = TransformToPresentation(dbApp, TrackingBackward(dbApp, facilitiyPreBooking, filter));
                    break;
                case GlobalApp.TrackingAndTracingSearchModel.Forward:
                    result = TransformToPresentation(dbApp, TrackingForward(dbApp, facilitiyPreBooking, filter));
                    break;
            }
            return result;
        }

        public TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, InOrderPos inOrderPos, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter)
        {
            TrackingAndTracingResult result = null;
            switch (searchModel)
            {
                case GlobalApp.TrackingAndTracingSearchModel.Backward:
                    result = TransformToPresentation(dbApp, TrackingBackward(dbApp, inOrderPos, filter));
                    break;
                case GlobalApp.TrackingAndTracingSearchModel.Forward:
                    result = TransformToPresentation(dbApp, TrackingForward(dbApp, inOrderPos, filter));
                    break;
            }
            return result;
        }

        public TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, OutOrderPos outOrderPos, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter)
        {
            TrackingAndTracingResult result = null;
            switch (searchModel)
            {
                case GlobalApp.TrackingAndTracingSearchModel.Backward:
                    result = TransformToPresentation(dbApp, TrackingBackward(dbApp, outOrderPos, filter));
                    break;
                case GlobalApp.TrackingAndTracingSearchModel.Forward:
                    result = TransformToPresentation(dbApp, TrackingForward(dbApp, outOrderPos, filter));
                    break;
            }
            return result;
        }

        public TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, DeliveryNotePos deliveryNotePos, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter)
        {
            TrackingAndTracingResult result = null;
            switch (searchModel)
            {
                case GlobalApp.TrackingAndTracingSearchModel.Backward:
                    result = TransformToPresentation(dbApp, TrackingBackward(dbApp, deliveryNotePos, filter));
                    break;
                case GlobalApp.TrackingAndTracingSearchModel.Forward:
                    result = TransformToPresentation(dbApp, TrackingForward(dbApp, deliveryNotePos, filter));
                    break;
            }
            return result;
        }

        public void SearchLot(ACBSO bso, core.datamodel.IACObjectEntity fb, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter, bool searchIntermediately = true)
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
            acMethod.ParameterValueList["CallerObject"] = fb;
            acMethod.ParameterValueList["SearchModel"] = searchModel;
            acMethod.ParameterValueList["IsSearchIntermediately"] = searchIntermediately;
            acMethod.ParameterValueList["TandTFilter"] = filter;
            bso.Root.RootPageWPF.StartBusinessobject(tandtBSOName, acMethod.ParameterValueList);
        }

        public TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, FacilityCharge fc, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter)
        {
            TrackingAndTracingResult result = null;
            switch (searchModel)
            {
                case GlobalApp.TrackingAndTracingSearchModel.Backward:
                    result = TransformToPresentation(dbApp, TrackingBackward(dbApp, fc, filter));
                    break;
                case GlobalApp.TrackingAndTracingSearchModel.Forward:
                    result = TransformToPresentation(dbApp, TrackingForward(dbApp, fc, filter));
                    break;
            }
            return result;
        }

        public TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, FacilityLot fl, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter)
        {
            TrackingAndTracingResult result = null;
            switch (searchModel)
            {
                case GlobalApp.TrackingAndTracingSearchModel.Backward:
                    result = TransformToPresentation(dbApp, TrackingBackward(dbApp, fl, filter));
                    break;
                case GlobalApp.TrackingAndTracingSearchModel.Forward:
                    result = TransformToPresentation(dbApp, TrackingForward(dbApp, fl, filter));
                    break;
            }
            return result;
        }

        public TrackingAndTracingResult TrackingAndTracing(DatabaseApp dbApp, ProdOrderPartslistPos pos, GlobalApp.TrackingAndTracingSearchModel searchModel, TandTFilter filter)
        {
            TrackingAndTracingResult result = null;
            switch (searchModel)
            {
                case GlobalApp.TrackingAndTracingSearchModel.Backward:
                    result = TransformToPresentation(dbApp, TrackingBackward(dbApp, pos, filter));
                    break;
                case GlobalApp.TrackingAndTracingSearchModel.Forward:
                    result = TransformToPresentation(dbApp, TrackingForward(dbApp, pos, filter));
                    break;
            }
            return result;
        }

        private const string _BSOName = "BSOTrackingAndTracing";
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

        #region Presentation Transformation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public TrackingAndTracingResult TransformToPresentation(DatabaseApp dbApp, TandTResult logicResult)
        {
            TrackingAndTracingResult presentResult = new TrackingAndTracingResult();
            TandTPoint logicRootPoint = logicResult.Results.Where(x => x.Parent == null).FirstOrDefault();
            presentResult.RootPoint = new TrackingAndTracingPoint(presentResult, logicRootPoint);
            presentResult.ProcessStatistics(dbApp);
            return presentResult;
        }

        #endregion

        #region Backward

        private TandTResult TrackingBackward(DatabaseApp dbApp, FacilityBooking facilityBooking, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new BackwardPointFacilityBooking(dbApp, rs, null, facilityBooking, filter);
            return rs;
        }

        private TandTResult TrackingBackward(DatabaseApp dbApp, FacilityPreBooking facilitiyPreBooking, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new BackwardPointFacilityPreBooking(dbApp, rs, null, facilitiyPreBooking, filter);
            return rs;
        }

        private TandTResult TrackingBackward(DatabaseApp dbApp, InOrderPos inOrderPos, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new BackwardPointInOrderPos(dbApp, rs, null, inOrderPos, filter);
            return rs;
        }

        private TandTResult TrackingBackward(DatabaseApp dbApp, OutOrderPos outOrderPos, TandTFilter filter)
        {
            throw new NotImplementedException();
        }

        private TandTResult TrackingBackward(DatabaseApp dbApp, DeliveryNotePos deliveryNotePos, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new BackwardPointDeliveryNotePos(dbApp, rs, null, deliveryNotePos, filter);
            return rs;
        }

        private TandTResult TrackingBackward(DatabaseApp dbApp, FacilityCharge fc, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new BackwardPointFacilityCharge(dbApp, rs, null, fc, filter);
            return rs;
        }

        private TandTResult TrackingBackward(DatabaseApp dbApp, FacilityLot fl, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new BackwardPointFacilityLot(dbApp, rs, null, fl, filter);
            return rs;
        }

        private TandTResult TrackingBackward(DatabaseApp dbApp, ProdOrderPartslistPos pos, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new BackwardPointProdOrderPartslistPos(dbApp, rs, null, pos, filter);
            return rs;
        }
        #endregion

        #region Forward

        private TandTResult TrackingForward(DatabaseApp dbApp, FacilityBooking facilityBooking, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new ForwardPointFacilityBooking(dbApp, rs, null, facilityBooking, filter);
            return rs;
        }

        private TandTResult TrackingForward(DatabaseApp dbApp, DeliveryNotePos deliveryNotePos, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new ForwardPointDeliveryNotePos(dbApp, rs, null, deliveryNotePos, filter);
            return rs;
        }

        private TandTResult TrackingForward(DatabaseApp dbApp, OutOrderPos outOrderPos, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new ForwardPointOutOrderPos(dbApp, rs, null, outOrderPos, filter);
            return rs;
        }

        private TandTResult TrackingForward(DatabaseApp dbApp, InOrderPos inOrderPos, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new ForwardPointInOrderPos(dbApp, rs, null, inOrderPos, filter);
            return rs;
        }

        private TandTResult TrackingForward(DatabaseApp dbApp, FacilityPreBooking facilitiyPreBooking, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new ForwardPointFacilityPreBooking(dbApp, rs, null, facilitiyPreBooking, filter);
            return rs;
        }

        private TandTResult TrackingForward(DatabaseApp dbApp, FacilityCharge fc, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new ForwardPointFacilityCharge(dbApp, rs, null, fc, filter);
            return rs;
        }

        private TandTResult TrackingForward(DatabaseApp dbApp, FacilityLot fl, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new ForwardPointFacilityLot(dbApp, rs, null, fl, filter);
            return rs;
        }

        private TandTResult TrackingForward(DatabaseApp dbApp, ProdOrderPartslistPos pos, TandTFilter filter)
        {
            TandTResult rs = new TandTResult();
            new ForwardPointProdOrderPartslistPos(dbApp, rs, null, pos, filter);
            return rs;
        }

        #endregion

    }
}
