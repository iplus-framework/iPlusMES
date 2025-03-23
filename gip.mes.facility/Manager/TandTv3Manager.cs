// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility.TandTv3;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTv3Manager'}de{'TandTv3Manager'}", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class TandTv3Manager : PARole, ITandTv3Manager, ITandTFetchCharge
    {
        #region constants
        public static string SearchModel_ParamValueKey = @"TrackingFilter";
        #endregion

        #region c´tors
        public TandTv3Manager(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
            _TandTBSOName = new ACPropertyConfigValue<string>(this, nameof(TandTBSOName), Const.BusinessobjectsACUrl + ACUrlHelper.Delimiter_Start + "BSOTandTv3");
            _FilterFaciltiyAtSearchInwardCharges = new ACPropertyConfigValue<bool>(this, nameof(FilterFaciltiyAtSearchInwardCharges), true);
            _MaxOrderCount = new ACPropertyConfigValue<int>(this, nameof(MaxOrderCount), 0);
            _OrderDepthSameRecipe = new ACPropertyConfigValue<int>(this, nameof(OrderDepthSameRecipe), -1);
            TandTv3Command = new TandTv3Command(FilterFaciltiyAtSearchInwardCharges);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseACInit = base.ACInit(startChildMode);

            _ = TandTBSOName;
            _ = FilterFaciltiyAtSearchInwardCharges;
            _ = _MaxOrderCount;
            _ = _OrderDepthSameRecipe;

            return baseACInit;
        }

        #endregion

        #region Configuration

        protected ACPropertyConfigValue<string> _TandTBSOName;
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

        protected ACPropertyConfigValue<bool> _FilterFaciltiyAtSearchInwardCharges;
        [ACPropertyConfig("en{'T&T-BSO-StartURL'}de{'T&T-BSO-StartURL'}")]
        public virtual bool FilterFaciltiyAtSearchInwardCharges
        {
            get
            {
                return _FilterFaciltiyAtSearchInwardCharges.ValueT;
            }
            set
            {
                _FilterFaciltiyAtSearchInwardCharges.ValueT = value;
            }
        }

        protected ACPropertyConfigValue<int> _MaxOrderCount;
        [ACPropertyConfig("en{'Max order count'}de{'Maximale Auftraganzahl'}")]
        public virtual int MaxOrderCount
        {
            get
            {
                return _MaxOrderCount.ValueT;
            }
            set
            {
                _MaxOrderCount.ValueT = value;
            }
        }

        protected ACPropertyConfigValue<int> _OrderDepthSameRecipe;
        [ACPropertyConfig("en{'Depth in same recipe'}de{'Tiefe im gleichen Rezept'}")]
        public virtual int OrderDepthSameRecipe
        {
            get
            {
                return _OrderDepthSameRecipe.ValueT;
            }
            set
            {
                _OrderDepthSameRecipe.ValueT = value;
            }
        }


        #endregion

        #region Manager instancing static methods
        public const string C_DefaultServiceACIdentifier = "TandTv3Manager";

        public static ITandTv3Manager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ITandTv3Manager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ITandTv3Manager> ACRefToServiceInstance(ACComponent requester)
        {
            ITandTv3Manager serviceInstance = GetServiceInstance(requester) as ITandTv3Manager;
            if (serviceInstance != null)
                return new ACRef<ITandTv3Manager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region Calling BSO
        public void StartTandTBSO(ACBSO bso, TandTv3FilterTracking filter)
        {
            string tandtBSOName = TandTBSOName;
            if (String.IsNullOrEmpty(tandtBSOName))
                return;
            gip.core.datamodel.ACClass acClassTT = null;
            using (ACMonitor.Lock(gip.core.datamodel.Database.GlobalDatabase.QueryLock_1X000))
            {
                acClassTT = gip.core.datamodel.Database.GlobalDatabase.ACClass.Where(c => c.ACIdentifier == TandTBSOName.Replace(Const.BusinessobjectsACUrl + ACUrlHelper.Delimiter_Start, "")).FirstOrDefault();
            }
            if (acClassTT == null)
                return;

            ACMethod acMethod = acClassTT.TypeACSignature();
            acMethod.ParameterValueList[SearchModel_ParamValueKey] = filter;
            bso.Root.RootPageWPF.StartBusinessobject(tandtBSOName, acMethod.ParameterValueList);
        }

        public void StartTandTBSO(ACBSO bso, MDTrackingDirectionEnum trackingDirection, MDTrackingStartItemTypeEnum trackingStartItemType, Guid itemSystemID, string itemSystemNo,
            bool recalcAgain = false, bool isDynamic = false, bool isReport = false)
        {
            TandTv3FilterTracking filter = TandTv3Command.FactoryFilter(trackingDirection, trackingStartItemType,
                itemSystemID, itemSystemNo, recalcAgain, isDynamic, isReport);
            StartTandTBSO(bso, filter);
        }

        #endregion

        #region Properties
        public TandTv3Command TandTv3Command { get; set; }
        #endregion

        #region ITv3Manager

        public virtual TandTv3Process<IACObjectEntity> GetProcessObject(DatabaseApp databaseApp, TandTv3FilterTracking filter, IACObjectEntity aCObjectEntity, string vbUserNo, bool useGroupResult)
        {
            if(MaxOrderCount > 0)
            {
                filter.MaxOrderCount = MaxOrderCount;
            }
            if(OrderDepthSameRecipe > -1)
            {
                filter.OrderDepthSameRecipe = OrderDepthSameRecipe;
            }
            return new TandTv3Process<IACObjectEntity>(databaseApp, TandTv3Command, filter, aCObjectEntity, vbUserNo, useGroupResult);
        }

        public virtual TandTv3.TandTResult DoTracking(DatabaseApp databaseApp, TandTv3FilterTracking filter, string vbUserNo, bool useGroupResult)
        {
            EntityKey entityKey = new EntityKey(databaseApp.DefaultContainerName + "." + filter.TandTv3MDTrackingStartItemTypeID.ToString(), filter.TandTv3MDTrackingStartItemTypeID.ToString() + "ID", filter.PrimaryKeyID);
            IACObjectEntity aCObjectEntity = TandTv3Command.FactoryObject(databaseApp, filter);
            TandTv3.TandTResult result = null;
            if (filter.CheckCancelWork())
                return null;
            TandTv3Process<IACObjectEntity> process = GetProcessObject(databaseApp, filter, aCObjectEntity, vbUserNo, useGroupResult);
            result = process.TandTResult;
            return result;
        }

        public virtual TandTv3.TandTResult DoSelect(DatabaseApp databaseApp, TandTv3FilterTracking filter, string vbUserNo, bool useGroupResult)
        {
            if (filter.RecalcAgain)
            {
                DoTracking(databaseApp, filter, vbUserNo, useGroupResult);
            }

            TandTv3.TandTResult result = TandTv3Command.DoSelect(databaseApp, filter);
            if (filter.CheckCancelWork())
                return null;
            if (filter.BackgroundWorker != null)
                filter.BackgroundWorker.ProgressInfo.TotalProgress.ProgressText = string.Format(@"Tracking: [{0}] | DoSelect() -> DoTrackingFinish() ...", filter.ItemSystemNo);
            TandTv3Command.DoTrackingFinish(databaseApp, result, filter);
            if (useGroupResult)
                result = TandTv3Command.BuildGroupResult(databaseApp, result);
            return result;
        }

        public MsgWithDetails DoDeleteTracking(DatabaseApp databaseApp, TandTv3FilterTracking filter)
        {
            MsgWithDetails msg = new MsgWithDetails();
            try
            {
                databaseApp.Database.ExecuteSql(FormattableStringFactory.Create("udpTandTv3FilterTrackingDelete @p0", filter.TandTv3FilterTrackingID));
                msg.MessageLevel = eMsgLevel.Info;
                msg.Message = "Successfully deleted T&T item!";
            }
            catch (Exception ec)
            {
                msg.MessageLevel = eMsgLevel.Error;
                msg.Message = ec.Message;
            }
            return msg;
        }

        #endregion

        #region ITandTFetchCharge
        public List<FacilityCharge> GetFacilityChargesBackward(DatabaseApp databaseApp, FacilityBooking facilityBooking, string userInitials, Func<object, bool> breakTrackingCondition)
        {
            List<FacilityCharge> facilityCharges = new List<FacilityCharge>();

            TandTv3FilterTracking filterTracking = new TandTv3FilterTracking();
            filterTracking.ItemSystemNo = facilityBooking.FacilityBookingNo;
            filterTracking.PrimaryKeyID = facilityBooking.FacilityBookingID;
            filterTracking.MDTrackingDirectionEnum = MDTrackingDirectionEnum.Backward;
            filterTracking.MDTrackingStartItemTypeEnum = MDTrackingStartItemTypeEnum.FacilityBooking;
            filterTracking.RecalcAgain = true;
            filterTracking.IsDynamic = true;
            filterTracking.BreakTrackingCondition = breakTrackingCondition;
            filterTracking.AggregateOrderData = false;


            TandTv3.TandTResult result = DoTracking(databaseApp, filterTracking, userInitials, false);
            if (result.Success && result.FacilityChargeIDs != null)
            {
                Guid[] facilityChargeIDs = result.FacilityChargeIDs.Select(c => c.FacilityChargeID).ToArray();
                facilityCharges = databaseApp
                    .FacilityCharge
                    .Where(c => facilityChargeIDs.Contains(c.FacilityChargeID))
                    .ToList();

            }

            return facilityCharges;

        }

        #endregion

    }
}
