﻿using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Filter'}de{'Filter'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, TandTv3MDTrackingDirection.ClassName, "en{'TrackingStyle'}de{'TrackingStyle'}", Const.ContextDatabase + "\\" + TandTv3MDTrackingDirection.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, TandTv3MDTrackingStartItemType.ClassName, "en{'ItemType'}de{'ItemType'}", Const.ContextDatabase + "\\" + TandTv3MDTrackingStartItemType.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, "FilterNo", "en{'FilterNo'}de{'FilterNo'}", "", "", true)]
    [ACPropertyEntity(4, "StartTime", "en{'Tracking started at'}de{'Verfolgung gestartet am'}", "", "", true)]
    [ACPropertyEntity(5, "EndTime", "en{'Tracking completed at'}de{'Verfolgung beendet am'}", "", "", true)]
    [ACPropertyEntity(6, "FilterDateFrom", "en{'Filter from'}de{'Filter von'}", "", "", true)]
    [ACPropertyEntity(7, "FilterDateTo", "en{'Filter to'}de{'Filter bis'}", "", "", true)]
    [ACPropertyEntity(8, "ItemSystemNo", "en{'ItemSystemNo'}de{'ItemSystemNo'}", "", "", true)]
    [ACPropertyEntity(9, "PrimaryKeyID", "en{'PrimaryKeyID'}de{'PrimaryKeyID'}", "", "", true)]
    [ACPropertyEntity(10, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + TandTv3FilterTracking.ClassName, "en{'FilterTracking'}de{'FilterTracking'}", typeof(TandTv3FilterTracking), TandTv3FilterTracking.ClassName, "ItemSystemNo", "StartTime")]
    [NotMapped]
    public partial class TandTv3FilterTracking
    {
        [NotMapped]
        public const string ClassName = "TandTv3FilterTracking";

        #region Enum

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public MDTrackingDirectionEnum MDTrackingDirectionEnum
        {
            get
            {
                return (MDTrackingDirectionEnum)Enum.Parse(typeof(MDTrackingDirectionEnum), TandTv3MDTrackingDirectionID);
            }
            set
            {
                TandTv3MDTrackingDirectionID = value.ToString();
            }
        }

        [NotMapped]
        public MDTrackingStartItemTypeEnum MDTrackingStartItemTypeEnum
        {
            get
            {
                return (MDTrackingStartItemTypeEnum)Enum.Parse(typeof(MDTrackingStartItemTypeEnum), TandTv3MDTrackingStartItemTypeID);
            }
            set
            {
                TandTv3MDTrackingStartItemTypeID = value.ToString();
            }
        }

        private string _TrackingStyleACCaption;
        [ACPropertyInfo(999, "TrackingStyleACCaption", "en{'Direction'}de{'Richtung'}")]
        [NotMapped]
        public string TrackingStyleACCaption
        {
            get
            {
                if (string.IsNullOrEmpty(_TrackingStyleACCaption))
                {
                    string acCaptionTranslation = "";
                    switch (MDTrackingDirectionEnum)
                    {
                        case MDTrackingDirectionEnum.Backward:
                            acCaptionTranslation = "en{'Trace back'}de{'Rückwärtsverfolgung'}";
                            break;
                        case MDTrackingDirectionEnum.Forward:
                            acCaptionTranslation = "en{'Forward Track and Trace'}de{'Vorwärtsverfolgung'}";
                            break;
                    }
                    _TrackingStyleACCaption = Translator.GetTranslation(ACIdentifier, acCaptionTranslation);
                }
                return _TrackingStyleACCaption;
            }
        }

        private string _TrackingStartItemTypeACCaption;
        [ACPropertyInfo(999, "TrackingStartItemTypeACCaption", "en{'Item type'}de{'Typ'}")]
        [NotMapped]
        public string TrackingStartItemTypeACCaption
        {
            get
            {
                if (string.IsNullOrEmpty(_TrackingStartItemTypeACCaption))
                {
                    string acCaptionTranslation = "";
                    switch (MDTrackingStartItemTypeEnum)
                    {
                        case MDTrackingStartItemTypeEnum.ACClass:
                            acCaptionTranslation = "en{'Class'}de{'Klasse'}";
                            break;
                        case MDTrackingStartItemTypeEnum.DeliveryNote:
                            acCaptionTranslation = "en{'Delivery Note'}de{'Eingangslieferschein'}";
                            break;
                        case MDTrackingStartItemTypeEnum.DeliveryNotePos:
                            acCaptionTranslation = "en{'Indeliverynotepos'}de{'Eingangslieferscheinposition'}";
                            break;
                        case MDTrackingStartItemTypeEnum.Facility:
                            acCaptionTranslation = ConstApp.Facility;
                            break;
                        case MDTrackingStartItemTypeEnum.FacilityPreview:
                            acCaptionTranslation = ConstApp.Facility;
                            break;
                        case MDTrackingStartItemTypeEnum.FacilityBooking:
                            acCaptionTranslation = "en{'Stock Movement'}de{'Lagerbewegung'}";
                            break;
                        case MDTrackingStartItemTypeEnum.FacilityBookingCharge:
                            acCaptionTranslation = "en{'Stock Movement of Quant'}de{'Lagerbewegung Quant'}";
                            break;
                        case MDTrackingStartItemTypeEnum.FacilityCharge:
                            acCaptionTranslation = "en{'Batch Location'}de{'Chargenplatz'}";
                            break;
                        case MDTrackingStartItemTypeEnum.FacilityLot:
                            acCaptionTranslation = ConstApp.Lot;
                            break;
                        case MDTrackingStartItemTypeEnum.InOrder:
                            acCaptionTranslation = "en{'Purchase Order'}de{'Bestellung'}";
                            break;
                        case MDTrackingStartItemTypeEnum.InOrderPosPreview:
                            acCaptionTranslation = "en{'Purchase Order Pos.'}de{'Bestellposition'}";
                            break;
                        case MDTrackingStartItemTypeEnum.OutOrder:
                            acCaptionTranslation = "en{'Sales Order'}de{'Kundenauftrag'}";
                            break;
                        case MDTrackingStartItemTypeEnum.OutOrderPosPreview:
                            acCaptionTranslation = "en{'Sales Order Pos.'}de{'Auftragsposition'}";
                            break;
                        case MDTrackingStartItemTypeEnum.ProdOrder:
                            acCaptionTranslation = "en{'Production Order'}de{'Produktionsauftrag'}";
                            break;
                        case MDTrackingStartItemTypeEnum.ProdOrderPartslist:
                            acCaptionTranslation = "en{'ProductionorderBillOfMaterials'}de{'Produktionsauftragsstückliste'}";
                            break;
                        case MDTrackingStartItemTypeEnum.ProdOrderPartslistPos:
                            acCaptionTranslation = "en{'Production Component'}de{'Produktionskomponente'}";
                            break;
                        case MDTrackingStartItemTypeEnum.ProdOrderPartslistPosRelation:
                            acCaptionTranslation = "en{'Production Order Pos. Status'}de{'Produktionsauftrag Pos.-Status'}";
                            break;
                        case MDTrackingStartItemTypeEnum.FacilityPreBooking:
                            acCaptionTranslation = "en{'Planned posting'}de{'Geplante Buchung'}";
                            break;
                        case MDTrackingStartItemTypeEnum.TandTv3Point:
                            acCaptionTranslation = "en{'TandTv3Point'}de{'TandTv3Point'}";
                            break;
                        default:
                            break;
                    }
                    _TrackingStartItemTypeACCaption = Translator.GetTranslation(ACIdentifier, acCaptionTranslation);
                }
                return _TrackingStartItemTypeACCaption;
            }
        }

        #endregion

        #region Additional members

        [ACPropertyInfo(9999, "MaterialNOs", "en{'Query Materials'}de{'Abfragematerialen'}")]
        [NotMapped]
        public string MaterialNOs
        {
            get
            {
                string materialNOs = "";
                if (this.TandTv3FilterTrackingMaterial_TandTv3FilterTracking.Any())
                {
                    foreach (var mt in this.TandTv3FilterTrackingMaterial_TandTv3FilterTracking)
                        materialNOs = mt.Material.MaterialNo + ", ";
                    materialNOs = materialNOs.TrimEnd(", ".ToCharArray());
                }
                return materialNOs;
            }
        }

        [NotMapped]
        public List<Guid> MaterialIDs { get; set; }


        /// <summary>
        /// If job is found - old job is deleted, new created, calculated again
        /// option calling from production wehre order items changed countinous
        /// </summary>
        [NotMapped]
        public bool RecalcAgain { get; set; }

        /// <summary>
        /// When is parameter forwarded as report
        /// </summary>
        [NotMapped]
        public bool IsReport { get; set; }

        /// <summary>
        /// If no need result to be saved
        /// </summary>
        [NotMapped]
        public bool IsDynamic { get; set; }

        [NotMapped]
        public bool IsNew { get; set; }

        [NotMapped]
        public bool IsDisabledReworkTracking { get; set; }

        [NotMapped]
        public StreamWriter LogFileStream { get; set; }

        [NotMapped]
        public int? OrderDepth { get; set; }

		[NotMapped]
        public int? MaxOrderCount { get; set; }

		[NotMapped]
        public int? OrderDepthSameRecipe { get; set; }

        public bool IsForAddOrderConnection()
        {
            return
                OrderDepth != null
                || MaxOrderCount != null
                || OrderDepthSameRecipe != null;
        }

		[NotMapped]
        private List<string> _MaterialNOsForStopTracking;
        /// <summary>
        /// Materials where stop Inward booking search
        /// </summary>
        [NotMapped]
        public List<string> MaterialNOsForStopTracking
        {
            get
            {
                if (_MaterialNOsForStopTracking == null)
                    _MaterialNOsForStopTracking = new List<string>();
                return _MaterialNOsForStopTracking;
            }
        }

		[NotMapped]
        public string MaterialWFNoForFilterLotByTime { get; set; }

        #endregion

        #region Filter methods

        [NotMapped]
        public Func<object, bool> BreakTrackingCondition { get; set; }


        [NotMapped]
        public bool AggregateOrderData { get; set; }

        #endregion

        #region Overrides
        public override string ToString()
        {
            return string.Format(@"{0} (new:{1} recalc:{2})", ItemSystemNo, IsNew, RecalcAgain);
        }
        #endregion

        #region Background Worker Cancel

        [NotMapped]
        public ACBackgroundWorker BackgroundWorker { get; set; }
        [NotMapped]
        public DoWorkEventArgs DoWorkEventArgs { get; set; }

        public bool CheckCancelWork()
        {
            if (BackgroundWorker == null || DoWorkEventArgs == null) return false;
            bool isCancelWork = BackgroundWorker.CancellationPending;
            if (isCancelWork)
                BackgroundWorker.CancelAsync();
            return isCancelWork;
        }

       
        #endregion

    }
}
