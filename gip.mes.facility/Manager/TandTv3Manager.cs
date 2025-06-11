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
            _IsFixDataPickingSourceActive = new ACPropertyConfigValue<bool>(this, nameof(IsFixDataPickingSourceActive), false);
            _IsCalculateDosedQuantityActive = new ACPropertyConfigValue<bool>(this, nameof(IsCalculateDosedQuantityActive), true);
            _MDUnitForRounding = new ACPropertyConfigValue<string>(this, nameof(MDUnitForRounding), "");
            TandTv3Command = new TandTv3Command(FilterFaciltiyAtSearchInwardCharges);
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            bool baseACInit = base.ACInit(startChildMode);

            _ = TandTBSOName;
            _ = FilterFaciltiyAtSearchInwardCharges;
            _ = MaxOrderCount;
            _ = OrderDepthSameRecipe;
            _ = IsFixDataPickingSourceActive;
            _ = IsCalculateDosedQuantityActive;
            _ = MDUnitForRounding;

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

        protected ACPropertyConfigValue<bool> _IsFixDataPickingSourceActive;
        [ACPropertyConfig("en{'Fix picking source'}de{'Kommissionierquelle korrigieren'}")]
        public virtual bool IsFixDataPickingSourceActive
        {
            get
            {
                return _IsFixDataPickingSourceActive.ValueT;
            }
            set
            {
                _IsFixDataPickingSourceActive.ValueT = value;
            }
        }

        protected ACPropertyConfigValue<bool> _IsCalculateDosedQuantityActive;
        [ACPropertyConfig("en{'Calculate dosed quantity active'}de{'Dosierte Wirkstoffmenge berechnen'}")]
        public virtual bool IsCalculateDosedQuantityActive
        {
            get
            {
                return _IsCalculateDosedQuantityActive.ValueT;
            }
            set
            {
                _IsCalculateDosedQuantityActive.ValueT = value;
            }
        }

        private ACPropertyConfigValue<string> _MDUnitForRounding;
        [ACPropertyConfig("en{'Show images'}de{'Bilder anzeigen'}")]
        public string MDUnitForRounding
        {
            get
            {
                return _MDUnitForRounding.ValueT;
            }
            set
            {
                _MDUnitForRounding.ValueT = value;
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
            if (MaxOrderCount > 0)
            {
                filter.MaxOrderCount = MaxOrderCount;
            }
            if (OrderDepthSameRecipe > -1)
            {
                filter.OrderDepthSameRecipe = OrderDepthSameRecipe;
            }
            return new TandTv3Process<IACObjectEntity>(databaseApp, TandTv3Command, filter, aCObjectEntity, vbUserNo, useGroupResult);
        }

        public virtual TandTv3.TandTResult DoTracking(DatabaseApp databaseApp, TandTv3FilterTracking filter, string vbUserNo, bool useGroupResult)
        {
            IACObjectEntity aCObjectEntity = TandTv3Command.FactoryObject(databaseApp, filter);
            TandTv3.TandTResult result = null;
            if (filter.CheckCancelWork())
                return null;
            TandTv3Process<IACObjectEntity> process = GetProcessObject(databaseApp, filter, aCObjectEntity, vbUserNo, useGroupResult);
            result = process.TandTResult;

            if (IsFixDataPickingSourceActive)
            {
                FixDataPickingSource(databaseApp, result.DeliveryNotes);
            }

            if (IsCalculateDosedQuantityActive)
            {
                string programNo = result.ProdOrders.Select(c => c.ProgramNo).FirstOrDefault();
                List<FacilityChargeModel> outwardFacilityCharges = GetOutwardFacilityChargeModels(databaseApp, result);
                result.OutwardFacilityCharges = outwardFacilityCharges;
                CalculateDosedQuantity(databaseApp, programNo, outwardFacilityCharges, MDUnitForRounding);
                foreach (FacilityChargeModel outwardCharge in result.OutwardFacilityCharges)
                {
                    FacilityChargeModel charge = result.FacilityCharges.Where(c => c.FacilityNo == outwardCharge.FacilityNo && c.MaterialNo == outwardCharge.MaterialNo && c.LotNo == outwardCharge.LotNo).FirstOrDefault();
                    if (charge != null)
                    {
                        charge.DosedQuantity = outwardCharge.DosedQuantity;
                    }
                }
            }

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

            if (IsCalculateDosedQuantityActive)
            {
                string programNo = result.ProdOrders.Select(c => c.ProgramNo).FirstOrDefault();
                List<FacilityChargeModel> outwardFacilityCharges = GetOutwardFacilityChargeModels(databaseApp, result);
                result.OutwardFacilityCharges = outwardFacilityCharges;
                CalculateDosedQuantity(databaseApp, programNo, outwardFacilityCharges, MDUnitForRounding);
                foreach (FacilityChargeModel outwardCharge in result.OutwardFacilityCharges)
                {
                    FacilityChargeModel charge = result.FacilityCharges.Where(c => c.FacilityNo == outwardCharge.FacilityNo && c.MaterialNo == outwardCharge.MaterialNo && c.LotNo == outwardCharge.LotNo).FirstOrDefault();
                    if (charge != null)
                    {
                        charge.DosedQuantity = outwardCharge.DosedQuantity;
                    }
                }
            }

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

        #region Data correction

        public void FixDataPickingSource(DatabaseApp databaseApp, List<DeliveryNotePosPreview> deliveryNotes)
        {
            foreach (DeliveryNotePosPreview dnp in deliveryNotes)
            {
                if (string.IsNullOrEmpty(dnp.FacilityNo))
                {
                    DeliveryNotePos dns = databaseApp.DeliveryNotePos.Where(c => c.DeliveryNotePosID == dnp.DeliveryNotePosID).FirstOrDefault();
                    if (dns != null && dns.InOrderPos != null)
                    {
                        CompanyAddress ca =
                            dns
                            .InOrderPos
                            .InOrder
                            .DistributorCompany?
                            .CompanyAddress_Company
                            // for every change of 
                            .OrderByDescending(c => c.UpdateDate)
                            .FirstOrDefault();

                        if (ca != null)
                        {
                            dnp.DeliveryAddress = ca.Name1;
                            dnp.ShipperAddress = ca.Name1;
                        }
                        dnp.DeliveryDate = dns.InOrderPos.InOrder.InOrderDate;
                        dnp.DeliveryNoteNo = dns.InOrderPos.InOrder.InOrderNo;

                        InOrderPos topParentInOrderPos = dns.InOrderPos.TopParentInOrderPos;
                        dnp.MDUnitName = topParentInOrderPos.MDUnit?.TechnicalSymbol;
                        List<FacilityBookingCharge> facilityBookingCharges = new List<FacilityBookingCharge>();
                        ExtractFBCFromInOrderPos(facilityBookingCharges, topParentInOrderPos);

                        if (!facilityBookingCharges.Any())
                        {
                            facilityBookingCharges =
                            dns
                            .InOrderPos
                            .InOrderPos_ParentInOrderPos
                            .SelectMany(c => c.PickingPos_InOrderPos)
                            .SelectMany(c => c.FacilityBookingCharge_PickingPos)
                            .ToList();
                        }

                        if (facilityBookingCharges != null && facilityBookingCharges.Any())
                        {
                            if (dnp.LotList == null)
                                dnp.LotList = new List<string>();
                            if (dnp.ExternLotList == null)
                                dnp.ExternLotList = new List<string>();
                            dnp.FacilityNo = facilityBookingCharges.Where(c => c.InwardFacilityID != null).Select(c => c.InwardFacility.FacilityNo).FirstOrDefault();
                            foreach (FacilityBookingCharge fbc in facilityBookingCharges)
                            {
                                if (!dnp.LotList.Contains(fbc.InwardFacilityLot.LotNo))
                                {
                                    dnp.LotList.Add(fbc.InwardFacilityLot.LotNo);
                                }
                                if (!string.IsNullOrEmpty(fbc.InwardFacilityLot.ExternLotNo) && !dnp.ExternLotList.Contains(fbc.InwardFacilityLot.ExternLotNo))
                                {
                                    dnp.ExternLotList.Add(fbc.InwardFacilityLot.ExternLotNo);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ExtractFBCFromInOrderPos(List<FacilityBookingCharge> facilityBookingCharges, InOrderPos inOrderPos)
        {
            facilityBookingCharges.AddRange(inOrderPos.FacilityBookingCharge_InOrderPos);
            foreach (InOrderPos tmp in inOrderPos.InOrderPos_ParentInOrderPos.ToArray())
                ExtractFBCFromInOrderPos(facilityBookingCharges, tmp);
        }

        public List<FacilityChargeModel> GetOutwardFacilityChargeModels(DatabaseApp databaseApp, TandTv3.TandTResult result)
        {
            List<FacilityChargeModel> facilityChargeModels = new List<FacilityChargeModel>();
            var outwardBookingNos =
               result
               .MixPoints
               .SelectMany(c => c.OutwardBookings)
               .Select(c => c.FacilityBookingNo);

            FacilityBookingCharge[] outwardBookings =
                databaseApp
                .FacilityBooking
                .Where(c => outwardBookingNos.Contains(c.FacilityBookingNo))
                .SelectMany(c => c.FacilityBookingCharge_FacilityBooking)
                .ToArray();

            facilityChargeModels =
                outwardBookings
                .Where(c => c.ProdOrderPartslistPosRelationID != null && c.OutwardFacilityCharge != null)
                .GroupBy(c => new
                {
                    c.OutwardFacilityCharge?.FacilityLot?.LotNo,
                    c.OutwardFacilityCharge?.FacilityLot?.ExternLotNo,
                    c.OutwardMaterial.MaterialNo,
                    c.OutwardMaterial.MaterialName1,
                    c.OutwardFacility.FacilityNo,
                    c.MDUnit.TechnicalSymbol,
                    c.ProdOrderPartslistPosRelation.TargetProdOrderPartslistPos?.ProdOrderPartslist.ProdOrder.ProgramNo
                })
                .Select(c => new FacilityChargeModel()
                {
                    LotNo = c.Key.LotNo,
                    ExternLotNo = c.Key.ExternLotNo,
                    FacilityNo = c.Key.FacilityNo,
                    MaterialNo = c.Key.MaterialNo,
                    MaterialName1 = c.Key.MaterialName1,
                    MDUnitName = c.Key.TechnicalSymbol,
                    ProdOrderProgramNo = c.Key.ProgramNo,

                    ActualQuantity = c.Sum(x => x.OutwardQuantityUOM),
                    StockQuantity =
                                    c
                                    .Where(x => x.OutwardFacilityCharge != null && x.OutwardFacilityCharge.FacilityLot != null)
                                    .Select(x => x.OutwardFacilityCharge.FacilityLot)
                                    .SelectMany(x => x.FacilityCharge_FacilityLot)
                                    .Where(x => !x.NotAvailable)
                                    .Select(x => new { x.Facility.FacilityNo, x.StockQuantityUOM })
                                    .Distinct()
                                    .Select(x => x.StockQuantityUOM).Sum()
                })
                .ToList();


            facilityChargeModels =
                facilityChargeModels
                .Where(c => c.ActualQuantity > 0)
                .OrderBy(c => c.ProdOrderProgramNo)
                .ThenBy(c => c.MaterialNo)
                .ToList();

            return facilityChargeModels;
        }

        /// <summary>
        /// Solution for calc usage in final order
        /// </summary>
        /// <param name="databaseApp"></param>
        /// <param name="header"></param>
        /// <param name="facilityChargeModels"></param>
        public void CalculateDosedQuantity(DatabaseApp databaseApp, string programNo, List<FacilityChargeModel> facilityChargeModels, string mdUnitForRounding)
        {
            try
            {
                string[] programNos = facilityChargeModels.Select(x => x.ProdOrderProgramNo).Distinct().ToArray();
                ProdOrderPlOverview[] orderInformations =
                    databaseApp
                    .ProdOrder
                    .Where(c => programNos.Contains(c.ProgramNo))
                    .SelectMany(c => c.ProdOrderPartslist_ProdOrder)
                    .AsEnumerable()
                    .Select(c => new ProdOrderPlOverview(c))
                    .ToArray();

                // calculate dosed quantity 
                foreach (FacilityChargeModel facilityChargeModel in facilityChargeModels)
                {
                    if (
                            facilityChargeModel.ProdOrderProgramNo == programNo
                            || orderInformations.Any(x => x.ProgramNo == facilityChargeModel.ProdOrderProgramNo && x.MaterialNo == facilityChargeModel.MaterialNo)
                       )
                    {
                        facilityChargeModel.DosedQuantity = facilityChargeModel.ActualQuantity;
                    }
                    else
                    {

                        ProdOrderPlOverview fcmOrder =
                            orderInformations
                            .Where(c => c.Inputs.Any(x => x.MaterialNo == facilityChargeModel.MaterialNo && x.DosingRatio > 0))
                            .FirstOrDefault();

                        double ratioInOwnOrder =
                            fcmOrder
                            .Inputs
                            .Where(c => c.MaterialNo == facilityChargeModel.MaterialNo)
                            .Select(c => c.DosingRatio)
                            .DefaultIfEmpty()
                            .FirstOrDefault();

                        double usageInFinal = facilityChargeModels.Where(c => c.ProdOrderProgramNo == programNo && c.MaterialNo == fcmOrder.MaterialNo).Select(c => c.ActualQuantity).DefaultIfEmpty().Sum();

                        double factor = 1;
                        double allAvailableChargesSum =
                            facilityChargeModels
                            .Where(c => c.ProdOrderProgramNo == facilityChargeModel.ProdOrderProgramNo && c.MaterialNo == facilityChargeModel.MaterialNo)
                            .Select(c => c.ActualQuantity)
                            .DefaultIfEmpty()
                            .Sum();
                        if (allAvailableChargesSum > 0)
                        {
                            factor = facilityChargeModel.ActualQuantity / allAvailableChargesSum;
                        }

                        facilityChargeModel.DosedQuantity = usageInFinal * ratioInOwnOrder * factor;
                    }

                    facilityChargeModel.ActualQuantity = facilityChargeModel.ActualQuantity.RoundQuantity(0, facilityChargeModel.MDUnitName, mdUnitForRounding);
                    facilityChargeModel.DosedQuantity = facilityChargeModel.DosedQuantity.RoundQuantity(0, facilityChargeModel.MDUnitName, mdUnitForRounding);
                }
            }
            catch (Exception ec)
            {
                Messages.LogException(GetACUrl(), nameof(CalculateDosedQuantity), ec);
            }
        }

        #endregion

    }
}
