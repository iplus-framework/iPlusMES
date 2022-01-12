using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace gip.mes.datamodel
{
    // FacilityLotStock (Artikel-Bestand)
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Facilitylotstock'}de{'Artikel-Bestand'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, FacilityLot.ClassName, ConstApp.Lot, Const.ContextDatabase + "\\" + FacilityLot.ClassName, "", true)]
    [ACPropertyEntity(2, MDReleaseState.ClassName, ConstApp.ESReleaseState, Const.ContextDatabase + "\\" + MDReleaseState.ClassName, "", true)]
    [ACPropertyEntity(3, "StockQuantity", ConstApp.StockQuantity, "", "", true)]
    [ACPropertyEntity(4, "StockWeight", ConstApp.StockWeight, "", "", true)]

    [ACPropertyEntity(5, "DayInward", ConstApp.DayInward, "", "", true)]
    [ACPropertyEntity(6, "DayTargetInward", ConstApp.DayTargetInward, "", "", true)]
    // 7 DayInwardDiff
    // 8 DayInwardDiffPercent
    [ACPropertyEntity(9, "DayOutward", ConstApp.DayOutward, "", "", true)]
    [ACPropertyEntity(10, "DayTargetOutward", ConstApp.DayTargetOutward, "", "", true)]
    // 11 DayOutwardDiff
    // 12 DayOutwardDiffPercent
    [ACPropertyEntity(13, "WeekInward", ConstApp.WeekInward, "", "", true)]
    [ACPropertyEntity(14, "WeekTargetInward", ConstApp.WeekTargetInward, "", "", true)]
    // 15 WeekInwardDiff
    // 16 WeekInwardDiffPercent
    [ACPropertyEntity(17, "WeekOutward", ConstApp.WeekOutward, "", "", true)]
    [ACPropertyEntity(18, "WeekTargetOutward", ConstApp.WeekTargetOutward, "", "", true)]
    // 19 WeekOutwardDiff
    // 20 WeekOutwardDiffPercent
    [ACPropertyEntity(21, "MonthInward", ConstApp.MonthInward, "", "", true)]
    [ACPropertyEntity(22, "MonthTargetInward", ConstApp.MonthTargetInward, "", "", true)]
    // 23 MonthInwardDiff
    // 24 MonthInwardDiffPercent
    [ACPropertyEntity(25, "MonthOutward", ConstApp.MonthOutward, "", "", true)]
    [ACPropertyEntity(26, "MonthTargetOutward", ConstApp.MonthTargetOutward, "", "", true)]
    // 27 MonthOutwardDiff
    // 28 MonthOutwardDiffPercent
    [ACPropertyEntity(29, "YearInward", ConstApp.YearInward, "", "", true)]
    [ACPropertyEntity(30, "YearTargetInward", ConstApp.YearTargetInward, "", "", true)]
    // 31 YearInwardDiff
    // 32 YearInwardDiffPercent
    [ACPropertyEntity(33, "YearOutward", ConstApp.YearOutward, "", "", true)]
    [ACPropertyEntity(34, "YearTargetOutward", ConstApp.YearTargetOutward, "", "", true)]
    // 35 YearOutwardDiff
    // 36 YearOutwardDiffPercent
    [ACPropertyEntity(37, "DayAdjustment", ConstApp.DayAdjustment, "", "", true)]
    [ACPropertyEntity(38, "DayBalanceDate", ConstApp.DayBalanceDate, "", "", true)]
    [ACPropertyEntity(40, "DayLastOutward", ConstApp.DayLastOutward, "", "", true)]
    [ACPropertyEntity(41, "DayLastStock", ConstApp.DayLastStock, "", "", true)]
    [ACPropertyEntity(42, "WeekAdjustment", ConstApp.WeekAdjustment, "", "", true)]
    [ACPropertyEntity(43, "WeekBalanceDate", ConstApp.WeekBalanceDate, "", "", true)]
    [ACPropertyEntity(44, "MonthActStock", ConstApp.MonthAverageStock, "", "", true)]
    [ACPropertyEntity(45, "MonthAdjustment", ConstApp.MonthAdjustment, "", "", true)]
    [ACPropertyEntity(46, "MonthAverageStock", ConstApp.MonthAverageStock, "", "", true)]
    [ACPropertyEntity(47, "MonthBalanceDate", ConstApp.MonthBalanceDate, "", "", true)]
    [ACPropertyEntity(49, "MonthLastOutward", ConstApp.MonthLastOutward, "", "", true)]
    [ACPropertyEntity(50, "MonthLastStock", ConstApp.MonthLastStock, "", "", true)]
    [ACPropertyEntity(51, "YearAdjustment", ConstApp.YearAdjustment, "", "", true)]
    [ACPropertyEntity(52, "YearBalanceDate", ConstApp.YearBalanceDate, "", "", true)]
    [ACPropertyEntity(53, "ReservedInwardQuantity", ConstApp.ReservedInwardQuantity, "", "", true)]
    [ACPropertyEntity(54, "ReservedOutwardQuantity", ConstApp.ReservedOutwardQuantity, "", "", true)]

    [ACPropertyEntity(55, "StockQuantityAmb", ConstApp.StockQuantity, "", "", true)]
    [ACPropertyEntity(56, "DayInwardAmb", ConstApp.DayInwardAmb, "", "", true)]
    [ACPropertyEntity(57, "DayTargetInwardAmb", ConstApp.DayTargetInwardAmb, "", "", true)]
    [ACPropertyEntity(58, "DayOutwardAmb", ConstApp.DayOutwardAmb, "", "", true)]
    [ACPropertyEntity(59, "DayTargetOutwardAmb", ConstApp.DayTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(60, "WeekInwardAmb", ConstApp.WeekInwardAmb, "", "", true)]
    [ACPropertyEntity(61, "WeekTargetInwardAmb", ConstApp.WeekTargetInwardAmb, "", "", true)]
    [ACPropertyEntity(62, "WeekOutwardAmb", ConstApp.WeekOutwardAmb, "", "", true)]
    [ACPropertyEntity(63, "WeekTargetOutwardAmb", ConstApp.WeekTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(64, "MonthInwardAmb", ConstApp.MonthInwardAmb, "", "", true)]
    [ACPropertyEntity(65, "MonthTargetInwardAmb", ConstApp.MonthTargetInwardAmb, "", "", true)]
    [ACPropertyEntity(66, "MonthOutwardAmb", ConstApp.MonthOutwardAmb, "", "", true)]
    [ACPropertyEntity(67, "MonthTargetOutwardAmb", ConstApp.MonthTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(68, "YearInwardAmb", ConstApp.YearInwardAmb, "", "", true)]
    [ACPropertyEntity(69, "YearTargetInwardAmb", ConstApp.YearTargetInwardAmb, "", "", true)]
    [ACPropertyEntity(70, "YearOutwardAmb", ConstApp.YearOutwardAmb, "", "", true)]
    [ACPropertyEntity(71, "YearTargetOutwardAmb", ConstApp.YearTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(72, "DayAdjustmentAmb", ConstApp.DayAdjustmentAmb, "", "", true)]
    [ACPropertyEntity(73, "DayLastOutwardAmb", ConstApp.DayLastOutwardAmb, "", "", true)]
    [ACPropertyEntity(74, "DayLastStockAmb", ConstApp.DayLastStockAmb, "", "", true)]
    [ACPropertyEntity(75, "WeekAdjustmentAmb", ConstApp.WeekAdjustmentAmb, "", "", true)]
    [ACPropertyEntity(76, "MonthActStockAmb", ConstApp.MonthActStock, "", "", true)]
    [ACPropertyEntity(77, "MonthAdjustmentAmb", ConstApp.MonthAdjustmentAmb, "", "", true)]
    [ACPropertyEntity(78, "MonthAverageStockAmb", ConstApp.MonthAverageStockAmb, "", "", true)]
    [ACPropertyEntity(79, "MonthLastOutwardAmb", ConstApp.MonthLastOutwardAmb, "", "", true)]
    [ACPropertyEntity(80, "MonthLastStockAmb", ConstApp.MonthLastStockAmb, "", "", true)]
    [ACPropertyEntity(81, "YearAdjustmentAmb", ConstApp.YearAdjustmentAmb, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]

    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityLotStock.ClassName, "en{'Facilitylotstock'}de{'Artikel-Bestand'}", typeof(FacilityLotStock), FacilityLotStock.ClassName, FacilityLot.ClassName + "\\LotNo", FacilityLot.ClassName + "\\LotNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<FacilityLotStock>) })]
    public partial class FacilityLotStock
    {
        public const string ClassName = "FacilityLotStock";

        #region New/Delete
        public static FacilityLotStock NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            FacilityLotStock entity = new FacilityLotStock();
            entity.FacilityLotStockID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDReleaseState = MDReleaseState.DefaultMDReleaseState(dbApp);
            if (parentACObject is FacilityLot)
            {
                FacilityLot facility = parentACObject as FacilityLot;
                entity.FacilityLot = facility;
                entity.FacilityLot.FacilityLotStock_FacilityLot.Add(entity);
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            return ACCaption;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                return FacilityLot.ACCaption;
            }
        }

        /// <summary>
        /// Returns FacilityLot
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to FacilityLot</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return FacilityLot;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "FacilityLot\\LotNo";
            }
        }

        #endregion

        #region IEntityProperty Members

        bool bRefreshConfig = false;
        partial void OnXMLConfigChanging(global::System.String value)
        {
            bRefreshConfig = false;
            if (this.EntityState != System.Data.EntityState.Detached && (!(String.IsNullOrEmpty(value) && String.IsNullOrEmpty(XMLConfig)) && value != XMLConfig))
                bRefreshConfig = true;
        }

        partial void OnXMLConfigChanged()
        {
            if (bRefreshConfig)
                ACProperties.Refresh();
        }

        partial void OnStockQuantityChanged()
        {
            RefreshMinOptFields();
        }

        public void RefreshMinOptFields()
        {
            if (this.FacilityLot != null)
                this.FacilityLot.OnEntityPropertyChanged(null);
        }

        #endregion


        /*
         * Onlinehelp: Definition von zusätzlichen Properties bei Entitäten im Datenbankmodell
         * In der Region "additional properties" können zusätzliche Properties bereit 
         * gestellt werden, die nicht in der Datenbank gespeichert werden.
         * 
         * Wenn die Events vom Set-Property (z.B. OnDayInward) abgefangen werden, 
         * kann dadurch eine automatische Berechnung der zusätzlichen Properties erfolgen.
         * 
         * TODO: Wie erfolgt die erste Berechnung nach dem Laden aus der Datenbank
         * TODO: Automatische Berechnung implementieren
         */
        #region additional properties
        /// <summary>
        /// There are no comments for Property DayInwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(7, "", "en{'Day Inward Qty Difference'}de{'Tageszugang Differenz'}")]
        public Double DayInwardDiff
        {
            get
            {
                return DayInward - DayTargetInward;
            }
        }


        /// <summary>
        /// There are no comments for Property DayInwardDiffPercent in the schema.
        /// </summary>
        [ACPropertyInfo(8, "", "en{'Day Inward Qty Difference in %'}de{'Tageszugang Differenz in %'}")]
        public Double DayInwardDiffPercent
        {
            get
            {
                if ((Math.Abs(DayInwardDiff - 0) > double.Epsilon) && (Math.Abs(DayInward - 0) > double.Epsilon))
                    return (DayInwardDiff * 100) / DayInward;
                return 0;
            }
        }

        /// <summary>
        /// There are no comments for Property DayOutwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(11, "", "en{'Day Outward Qty Difference'}de{'Tagesabgang Differenz'}")]
        public Double DayOutwardDiff
        {
            get
            {
                return DayOutward - DayTargetOutward;
            }
        }

        /// <summary>
        /// There are no comments for Property DayOutwardDiffPercent in the schema.
        /// </summary>
        [ACPropertyInfo(12, "", "en{'Day Outward Qty Difference in %'}de{'Tagesabgang Differenz in %'}")]
        public Double DayOutwardDiffPercent
        {
            get
            {
                if ((Math.Abs(DayOutwardDiff - 0) > double.Epsilon) && (Math.Abs(DayOutward - 0) > double.Epsilon))
                    return (DayOutwardDiff * 100) / DayInward;
                return 0;
            }
        }

        /// <summary>
        /// There are no comments for Property WeekInwardDiff in the schema.
        /// </summary>       
        [ACPropertyInfo(15, "", "en{'Week Inward Qty Difference'}de{'Wochenzugang Differenz'}")]
        public Double WeekInwardDiff
        {
            get
            {
                return WeekInward - WeekTargetInward;
            }
        }

        /// <summary>
        /// There are no comments for Property WeekInwardDiffPercent in the schema.
        /// </summary>
        [ACPropertyInfo(16, "", "en{'Week Inward Qty Difference in %'}de{'Wochenzugang Differenz in %'}")]
        public Double WeekInwardDiffPercent
        {
            get
            {
                if ((Math.Abs((float)WeekInwardDiff - 0) > double.Epsilon) && (Math.Abs(WeekInward - 0) > double.Epsilon))
                    return (WeekInwardDiff * 100) / WeekInward;
                return 0;
            }
        }


        /// <summary>
        /// There are no comments for Property WeekOutwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(19, "", "en{'Week Outward Qty Difference'}de{'Wochenabgang Differenz'}")]
        public Double WeekOutwardDiff
        {
            get
            {
                return WeekOutward - WeekTargetOutward;
            }
        }

        /// <summary>
        /// There are no comments for Property WeekOutwardDiffPercent in the schema.
        /// </summary>
        [ACPropertyInfo(20, "", "en{'Week Outward Qty Difference in %'}de{'Wochenabgang Differenz in %'}")]
        public Double WeekOutwardDiffPercent
        {
            get
            {
                if ((Math.Abs((float)WeekOutwardDiff - 0) > double.Epsilon) && (Math.Abs(WeekOutward - 0) > double.Epsilon))
                    return (WeekOutwardDiff * 100) / WeekInward;
                return 0;
            }
        }

        /// <summary>
        /// There are no comments for Property MonthInwardDiff in the schema.
        /// </summary>

        [ACPropertyInfo(23, "", "en{'Month Inward Qty Difference'}de{'Monatszugang Differenz'}")]
        public Double MonthInwardDiff
        {
            get
            {
                return MonthInward - MonthTargetInward;
            }
        }

        /// <summary>
        /// There are no comments for Property MonthInwardDiffPercent in the schema.
        /// </summary>
        [ACPropertyInfo(24, "", "en{'Month Inward Qty Difference in %'}de{'Monatszugang Differenz in %'}")]
        public Double MonthInwardDiffPercent
        {
            get
            {
                if ((Math.Abs((float)MonthInwardDiff - 0) > double.Epsilon) && (Math.Abs(MonthInward - 0) > double.Epsilon))
                    return (MonthInwardDiff * 100) / MonthInward;
                return 0;
            }
        }


        /// <summary>
        /// There are no comments for Property MonthOutwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(27, "", "en{'Month Outward Qty Difference'}de{'Monatsabgang Differenz in %'}")]
        public Double MonthOutwardDiff
        {
            get
            {
                return MonthOutward - MonthTargetOutward;
            }
        }

        /// <summary>
        /// There are no comments for Property MonthOutwardDiffPercent in the schema.
        /// </summary>
        [ACPropertyInfo(28, "", "en{'Month Outward Qty Difference in %'}de{'Monatsabgang Differenz in %'}")]
        public Double MonthOutwardDiffPercent
        {
            get
            {
                if ((Math.Abs((float)MonthOutwardDiff - 0) > double.Epsilon) && (Math.Abs(MonthOutward - 0) > double.Epsilon))
                    return (MonthOutwardDiff * 100) / MonthInward;
                return 0;
            }
        }

        /// <summary>
        /// There are no comments for Property YearInwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(31, "", "en{'Year Inward Qty Difference'}de{'Jahreseingang Differenz'}")]
        public Double YearInwardDiff
        {
            get
            {
                return YearInward - YearTargetInward;
            }
        }

        /// <summary>
        /// There are no comments for Property YearInwardDiffPercent in the schema.
        /// </summary>
        [ACPropertyInfo(32, "", "en{'Year Inward Qty Difference in %'}de{'Jahreseingang Differenz in %'}")]
        public Double YearInwardDiffPercent
        {
            get
            {
                if ((Math.Abs((float)YearInwardDiff - 0) > double.Epsilon) && (Math.Abs(YearInward - 0) > double.Epsilon))
                    return (YearInwardDiff * 100) / YearInward;
                return 0;
            }
        }

        /// <summary>
        /// There are no comments for Property YearOutwardDiff in the schema.
        /// </summary>
        [ACPropertyInfo(35, "", "en{'Year Outward Qty Difference'}de{'Jahresabgang Differenz'}")]
        public Double YearOutwardDiff
        {
            get
            {
                return YearOutward - YearTargetOutward;
            }
        }


        /// <summary>
        /// There are no comments for Property YearOutwardDiffPercent in the schema.
        /// </summary>
        [ACPropertyInfo(36, "", "en{'Year Outward Qty Difference in %'}de{'Jahresabgang Differenz in %'}")]
        public Double YearOutwardDiffPercent
        {
            get
            {
                if ((Math.Abs((float)YearOutwardDiff - 0) > double.Epsilon) && (Math.Abs(YearOutward - 0) > double.Epsilon))
                    return (YearOutwardDiff * 100) / YearInward;
                return 0;
            }
        }


        /// <summary>
        /// There are no comments for Property ReservedInwardQuantity in the schema.
        /// </summary>
        public void RecalcReservedInwardQuantity()
        {
            if ((this.FacilityLot == null) || (this.FacilityLot.Material == null))
                return;

            this.ReservedInwardQuantity = 0;
            // TODO: OR-Klausel einfügen für Produktionsaufträge
            List<FacilityReservation> facilityReservationList = (from c in FacilityLot.FacilityReservation_FacilityLot where c.InOrderPos != null select c).ToList();
            foreach (FacilityReservation facilityReservation in facilityReservationList)
            {
                // Die ActualQuantity gibt an, wieviel bereits auf dem Material gebucht worden ist
                // Die TargetQuantity gibt an, wieviel Reserviert ist
                // Die Differenz gibt an, wieviel noch geliefert wird
                // TODO: Zugänge von Produktionsaufträge mit einrechnen
                this.ReservedInwardQuantity += facilityReservation.InOrderPos.TargetQuantity - facilityReservation.InOrderPos.ActualQuantity;
            }
        }


        /// <summary>
        /// There are no comments for Property ReservedOutwardQuantity in the schema.
        /// </summary>
        public void RecalcReservedOutwardQuantity()
        {
            if ((this.FacilityLot == null) || (this.FacilityLot.Material == null))
                return;

            this.ReservedOutwardQuantity = 0;
            // TODO: OR-Klausel einfügen für Produktionsaufträge
            List<FacilityReservation> facilityReservationList = (from c in FacilityLot.FacilityReservation_FacilityLot where c.OutOrderPos != null select c).ToList();
            foreach (FacilityReservation facilityReservation in facilityReservationList)
            {
                // Die ActualQuantity gibt an, wieviel bereits auf dem Material gebucht worden ist
                // Die TargetQuantity gibt an, wieviel Reserviert ist
                // Die Differenz gibt an, wieviel noch abgebucht wird
                // TODO: Abgänge von Produktionsaufträgen mit einrechnen
                this.ReservedOutwardQuantity += facilityReservation.OutOrderPos.TargetQuantity - facilityReservation.OutOrderPos.ActualQuantity;
            }
        }


        /// <summary>
        /// There are no comments for Property ReservedQuantity in the schema.
        /// </summary>
        [ACPropertyInfo(59, "", "en{'Reserved Quantity'}de{'Reservierte Menge'}")]
        public Double ReservedQuantity
        {
            get
            {
                return ReservedOutwardQuantity - ReservedInwardQuantity;
            }
        }


        /// <summary>
        /// There are no comments for Property AvailableQuantity in the schema.
        /// </summary>
        [ACPropertyInfo(60, "", "en{'Available Quantity'}de{'Verfügbare Menge'}")]
        public Double AvailableQuantity
        {
            get
            {
                return StockQuantity - ReservedQuantity;
            }
        }

        #endregion
    }
}
