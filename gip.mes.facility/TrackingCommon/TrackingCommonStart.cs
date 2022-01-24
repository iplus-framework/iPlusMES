using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using System;
using System.Linq;
namespace gip.mes.facility
{
    public class TrackingCommonStart : ITrackingCommand
    {
        #region configuration

        private const string V1_Manager = @"ACTrackingAndTracingManager";
        private const string V2_Manager = @"TandT2Manager";
        private const string V3_Manager = @"TandTv3Manager";
        private const string BSOName_LocalConfigACUrl = @"TandTBSOName";


        private const string V1_BSOName = "BSOTrackingAndTracing";
        private const string V2_BSOName = "BSOTandTv2";
        private const string V3_BSOName = "BSOTandTv3";

        #endregion

        public void DoTracking(ACBSO bso, GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter)
        {
            TrackingConfiguration trackingConfiguration = (TrackingConfiguration)CommandLineHelper.ConfigCurrentDir.GetSection("trackingConfiguration");
            DoTracking(bso, direction, itemForTrack, additionalFilter, trackingConfiguration.DefaultTrackingEngine);
        }

        public void DoTracking(ACBSO bso, GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter, TrackingEnginesEnum engine)
        {
            switch (engine)
            {
                case TrackingEnginesEnum.v2:
                    DoTracking_v2(bso, direction, itemForTrack, additionalFilter);
                    break;
                case TrackingEnginesEnum.v3:
                    DoTracking_v3(bso, direction, itemForTrack, additionalFilter);
                    break;
                default:
                    DoTracking_v1(bso, direction, itemForTrack, additionalFilter);
                    break;
            }
        }

        public ACRef<ITandTFetchCharge> GetFetchChargeManagerInstance(ACComponent acComponent)
        {
            TrackingConfiguration trackingConfiguration = (TrackingConfiguration)CommandLineHelper.ConfigCurrentDir.GetSection("trackingConfiguration");
            ACRef<ITandTFetchCharge> manager = null;
            switch (trackingConfiguration.DefaultTrackingEngine)
            {
                case TrackingEnginesEnum.v2:
                    ACRef<TandT2Manager> v2Manager = TandT2Manager.ACRefToServiceInstance(acComponent);
                    manager = new ACRef<ITandTFetchCharge>(v2Manager.ValueT, acComponent);
                    break;
                case TrackingEnginesEnum.v3:
                    ACRef<ITandTv3Manager> v3Manager = TandTv3Manager.ACRefToServiceInstance(acComponent);
                    manager = new ACRef<ITandTFetchCharge>(v3Manager.ValueT, acComponent);
                    break;
                default:
                    break;
            }
            return manager;
        }

        public void DetachChargeManagerInstance(ACComponent acComponent, ACRef<ITandTFetchCharge> acRef)
        {
            ACComponent manager = acRef.ValueT as ACComponent;
            acRef.Detach();
            if (manager != null)
            {
                if (manager.ParentACComponent == (acComponent.Root as ACRoot).LocalServiceObjects)
                {
                    if (!manager.ReferencePoint.HasStrongReferences)
                    {
                        manager.Stop();
                    }
                }
            }
        }

        public ACMenuItemList GetTrackingAndTrackingMenuItems(IACInteractiveObject handlerACElement, IACObject item)
        {
            ACMenuItemList aCMenuItems = new ACMenuItemList();

            ACValueList backwardParamBase = new ACValueList();
            backwardParamBase.Add(new ACValue("direction", GlobalApp.TrackingAndTracingSearchModel.Backward));
            backwardParamBase.Add(new ACValue("itemForTrack", item));
            backwardParamBase.Add(new ACValue("additionalFilter", ""));

            ACValueList forwardParamBase = new ACValueList();
            forwardParamBase.Add(new ACValue("direction", GlobalApp.TrackingAndTracingSearchModel.Forward));
            forwardParamBase.Add(new ACValue("itemForTrack", item));
            forwardParamBase.Add(new ACValue("additionalFilter", ""));

            ACValueList backwardParams = null;
            ACValueList forwardParams = null;

            string backwardTranslation = "en{'Backward Track and Trace'}de{'Rückverfolgung'}";
            string forwardTranslation = "en{'Forward Track and Trace'}de{'Vorwärtsverfolgung'}";

            TrackingConfiguration trackingConfiguration = (TrackingConfiguration)CommandLineHelper.ConfigCurrentDir.GetSection("trackingConfiguration");
            if (trackingConfiguration == null)
                trackingConfiguration = TrackingConfiguration.FactoryDefaultConfiguration();

            if (trackingConfiguration.WorkingModel == TrackingWorkingModelEnum.Single)
            {
                backwardParams = new ACValueList(backwardParamBase);
                backwardParams.Add(new ACValue("engine", trackingConfiguration.DefaultTrackingEngine));

                forwardParams = new ACValueList(forwardParamBase);
                forwardParams.Add(new ACValue("engine", trackingConfiguration.DefaultTrackingEngine));

                backwardTranslation = string.Format("en{{'Backward Track and Trace {0}'}}de{{'Rückverfolgung {0}'}}", trackingConfiguration.DefaultTrackingEngine.ToString());
                forwardTranslation = string.Format("en{{'Forward Track and Trace {0}'}}de{{'Vorwärtsverfolgung {0}'}}", trackingConfiguration.DefaultTrackingEngine.ToString());
            }

            ACMenuItem backwardMenuItem = new ACMenuItem(null, backwardTranslation, "!OnTrackingCall", 0, backwardParams, true, handlerACElement);
            ACMenuItem forwardMenuItem = new ACMenuItem(null, forwardTranslation, "!OnTrackingCall", 0, forwardParams, true, handlerACElement);

            if (trackingConfiguration.WorkingModel == TrackingWorkingModelEnum.Multiple)
            {
                FactorySumMenuItem(handlerACElement, backwardMenuItem, backwardParamBase);
                FactorySumMenuItem(handlerACElement, forwardMenuItem, forwardParamBase);
            }

            aCMenuItems.Add(backwardMenuItem);
            aCMenuItems.Add(forwardMenuItem);

            return aCMenuItems;
        }

        #region Private handlers methods

        #region Private -> DoTracking versions
        private void DoTracking_v1(ACBSO bso, GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter)
        {
            TrackingCommonStart_Config config = GetManagerConfigParam(TrackingEnginesEnum.v1);
            if (config == null)
                return;

            TandTFilter filter = additionalFilter as TandTFilter;

            ACMethod acMethod = config.ACClassTT.TypeACSignature();
            acMethod.ParameterValueList["CallerObject"] = itemForTrack;
            acMethod.ParameterValueList["SearchModel"] = direction;
            acMethod.ParameterValueList["IsSearchIntermediately"] = true;
            acMethod.ParameterValueList["TandTFilter"] = filter;
            bso.Root.RootPageWPF.StartBusinessobject(config.TandtBSOName, acMethod.ParameterValueList);
        }

        private void DoTracking_v2(ACBSO bso, GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter)
        {
            TrackingCommonStart_Config config = GetManagerConfigParam(TrackingEnginesEnum.v2);
            if (config == null)
                return;
            TandTv2Job job = additionalFilter as TandTv2Job;
            if (job == null
                && (additionalFilter == null
                    || (additionalFilter is string && string.IsNullOrEmpty(additionalFilter as string))))
            {
                #region Populate by elements
                job = new TandTv2Job();

                // DeliveryNotePos
                if (itemForTrack is DeliveryNotePos)
                {
                    DeliveryNotePos deliveryNotePos = itemForTrack as DeliveryNotePos;
                    job.ItemSystemNo = deliveryNotePos.Sequence.ToString();
                    job.PrimaryKeyID = deliveryNotePos.DeliveryNotePosID;
                    job.ItemTypeEnum = TandTv2ItemTypeEnum.DeliveryNotePos;
                }

                // FacilityBooking
                if (itemForTrack is FacilityBooking)
                {
                    FacilityBooking facilityBooking = itemForTrack as FacilityBooking;
                    job.ItemSystemNo = facilityBooking.FacilityBookingNo;
                    job.PrimaryKeyID = facilityBooking.FacilityBookingID;
                    job.ItemTypeEnum = TandTv2ItemTypeEnum.FacilityBooking;
                }

                // FacilityBookingCharge
                if (itemForTrack is FacilityBookingCharge)
                {
                    FacilityBookingCharge facilityBookingCharge = itemForTrack as FacilityBookingCharge;
                    job.ItemSystemNo = facilityBookingCharge.FacilityBookingChargeNo;
                    job.PrimaryKeyID = facilityBookingCharge.FacilityBookingChargeID;
                    job.ItemTypeEnum = TandTv2ItemTypeEnum.FacilityBooking;
                }

                // InOrderPos
                if (itemForTrack is InOrderPos)
                {
                    InOrderPos inOrderPos = itemForTrack as InOrderPos;
                    job.ItemSystemNo = inOrderPos.Sequence.ToString();
                    job.PrimaryKeyID = inOrderPos.InOrderPosID;
                    job.ItemTypeEnum = TandTv2ItemTypeEnum.InOrderPos;
                }

                // OutOrderPos
                if (itemForTrack is OutOrderPos)
                {
                    OutOrderPos outOrderPos = itemForTrack as OutOrderPos;
                    job.ItemSystemNo = outOrderPos.Sequence.ToString();
                    job.PrimaryKeyID = outOrderPos.OutOrderPosID;
                    job.ItemTypeEnum = TandTv2ItemTypeEnum.OutOrderPos;
                }


                // FacilityPreBooking
                if (itemForTrack is FacilityPreBooking)
                {
                    FacilityPreBooking facilityPreBooking = itemForTrack as FacilityPreBooking;
                    job.ItemSystemNo = facilityPreBooking.FacilityPreBookingNo;
                    job.PrimaryKeyID = facilityPreBooking.FacilityPreBookingID;
                    job.ItemTypeEnum = TandTv2ItemTypeEnum.FacilityPreBooking;
                }
                #endregion
            }

            if (job == null)
                return;

            if (direction == GlobalApp.TrackingAndTracingSearchModel.Backward)
                job.TrackingStyleEnum = TandTv2TrackingStyleEnum.Backward;
            else if (direction == GlobalApp.TrackingAndTracingSearchModel.Forward)
                job.TrackingStyleEnum = TandTv2TrackingStyleEnum.Forward;

            ACMethod acMethod = config.ACClassTT.TypeACSignature();
            acMethod.ParameterValueList[TandT2Manager.SearchModel_ParamValueKey] = job;
            bso.Root.RootPageWPF.StartBusinessobject(config.TandtBSOName, acMethod.ParameterValueList);
        }

        private void DoTracking_v3(ACBSO bso, GlobalApp.TrackingAndTracingSearchModel direction, IACObject itemForTrack, object additionalFilter)
        {
            TrackingCommonStart_Config config = GetManagerConfigParam(TrackingEnginesEnum.v3);
            if (config == null)
                return;
            TandTv3FilterTracking filter = additionalFilter as TandTv3FilterTracking;
            if (filter == null
                && (additionalFilter == null
                    || (additionalFilter is string && string.IsNullOrEmpty(additionalFilter as string))))
            {
                filter = new TandTv3FilterTracking();
                #region Populate by elements

                filter.RecalcAgain = false;
                filter.IsDynamic = false;
                filter.IsReport = false;
                filter.MDTrackingDirectionEnum = direction == GlobalApp.TrackingAndTracingSearchModel.Backward ? MDTrackingDirectionEnum.Backward : MDTrackingDirectionEnum.Forward;
                // DeliveryNotePos
                if (itemForTrack is DeliveryNotePos)
                {
                    DeliveryNotePos deliveryNotePos = itemForTrack as DeliveryNotePos;
                    filter.ItemSystemNo = deliveryNotePos.DeliveryNotePosID.ToString();
                    filter.PrimaryKeyID = deliveryNotePos.DeliveryNotePosID;
                    filter.MDTrackingStartItemTypeEnum = MDTrackingStartItemTypeEnum.DeliveryNotePos;
                }

                // FacilityBooking
                if (itemForTrack is FacilityBooking)
                {
                    FacilityBooking facilityBooking = itemForTrack as FacilityBooking;
                    filter.ItemSystemNo = facilityBooking.FacilityBookingNo;
                    filter.PrimaryKeyID = facilityBooking.FacilityBookingID;
                    filter.MDTrackingStartItemTypeEnum = MDTrackingStartItemTypeEnum.FacilityBooking;
                }

                // FacilityBookingCharge
                if (itemForTrack is FacilityBookingCharge)
                {
                    FacilityBookingCharge facilityBookingCharge = itemForTrack as FacilityBookingCharge;
                    filter.ItemSystemNo = facilityBookingCharge.FacilityBookingChargeNo;
                    filter.PrimaryKeyID = facilityBookingCharge.FacilityBookingChargeID;
                    filter.MDTrackingStartItemTypeEnum = MDTrackingStartItemTypeEnum.FacilityBookingCharge;
                }

                // InOrderPos
                if (itemForTrack is InOrderPos)
                {
                    InOrderPos inOrderPos = itemForTrack as InOrderPos;
                    filter.ItemSystemNo = inOrderPos.InOrderPosID.ToString();
                    filter.PrimaryKeyID = inOrderPos.InOrderPosID;
                    filter.MDTrackingStartItemTypeEnum = MDTrackingStartItemTypeEnum.InOrderPos;
                }

                // OutOrderPos
                if (itemForTrack is OutOrderPos)
                {
                    OutOrderPos outOrderPos = itemForTrack as OutOrderPos;
                    filter.ItemSystemNo = outOrderPos.OutOrderPosID.ToString();
                    filter.PrimaryKeyID = outOrderPos.OutOrderPosID;
                    filter.MDTrackingStartItemTypeEnum = MDTrackingStartItemTypeEnum.OutOrderPosPreview;
                }


                // FacilityPreBooking
                if (itemForTrack is FacilityPreBooking)
                {
                    FacilityPreBooking facilityPreBooking = itemForTrack as FacilityPreBooking;
                    filter.ItemSystemNo = facilityPreBooking.FacilityPreBookingNo;
                    filter.PrimaryKeyID = facilityPreBooking.FacilityPreBookingID;
                    filter.MDTrackingStartItemTypeEnum = MDTrackingStartItemTypeEnum.FacilityPreBooking;
                }
                #endregion
            }

            if (filter == null)
                return;

            ACMethod acMethod = config.ACClassTT.TypeACSignature();
            acMethod.ParameterValueList[TandTv3Manager.SearchModel_ParamValueKey] = filter;
            bso.Root.RootPageWPF.StartBusinessobject(config.TandtBSOName, acMethod.ParameterValueList);
        }
        #endregion

        #region Private metods -> Factory ACMenuItems

        private void FactorySumMenuItem(IACInteractiveObject handlerACElement, ACMenuItem parentItem, ACValueList baseParams)
        {
            foreach (TrackingEnginesEnum engine in (TrackingEnginesEnum[])Enum.GetValues(typeof(TrackingEnginesEnum)))
            {
                FactorySumMenuItem(handlerACElement, parentItem, baseParams, engine);
            }
        }

        private void FactorySumMenuItem(IACInteractiveObject handlerACElement, ACMenuItem parentMenuItem, ACValueList baseParams, TrackingEnginesEnum engine)
        {
            var subMenuParam = new ACValueList(baseParams);
            subMenuParam.Add(new ACValue("engine", engine));
            var subMenuItem = new ACMenuItem(parentMenuItem, TrackingEngineList.TrackingEngines[engine], "!OnTrackingCall", 0, subMenuParam, true, handlerACElement);
            parentMenuItem.Items.Add(subMenuItem);
        }

        #endregion

        #region Distribute config

        public TrackingCommonStart_Config GetManagerConfigParam(TrackingEnginesEnum engine)
        {
            TrackingCommonStart_Config config = null;
            switch (engine)
            {
                case TrackingEnginesEnum.v2:
                    config = GetManagerConfigParam(V2_Manager, V2_BSOName);
                    break;
                case TrackingEnginesEnum.v3:
                    config = GetManagerConfigParam(V3_Manager, V3_BSOName);
                    break;
                default:
                    config = GetManagerConfigParam(V1_Manager, V1_BSOName);
                    break;
            }
            return config;
        }

        private TrackingCommonStart_Config GetManagerConfigParam(string managerName, string bsoName)
        {
            TrackingCommonStart_Config config = null;
            string tandtBSOName = "";
            gip.core.datamodel.ACClass acClassTT = null;
            using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
            {
                gip.core.datamodel.ACClassConfig aCClassConfig =
                    gip.core.datamodel.Database.GlobalDatabase
                    .ACClassConfig
                    .Where(c => c.LocalConfigACUrl == BSOName_LocalConfigACUrl && c.ACClass.ACIdentifier == managerName)
                    .ToList()
                    .Where(c => c.Value != null)
                    .FirstOrDefault();
                if (aCClassConfig != null)
                    tandtBSOName = aCClassConfig.Value.ToString();

                acClassTT = gip.core.datamodel.Database.GlobalDatabase.ACClass.Where(c => c.ACIdentifier == bsoName).FirstOrDefault();
            }
            if (!string.IsNullOrEmpty(tandtBSOName) && acClassTT != null)
                config = new TrackingCommonStart_Config() { TandtBSOName = tandtBSOName, ACClassTT = acClassTT };
            return config;
        }

        #endregion

        #endregion

    }

    public class TrackingCommonStart_Config
    {
        public string TandtBSOName { get; set; }
        public gip.core.datamodel.ACClass ACClassTT { get; set; }
    }
}
