using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace gip.mes.datamodel
{
    // CompanyMaterialStock (Artikel-Bestand)
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'CompanyMaterialStock'}de{'Materialbestand'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, CompanyMaterial.ClassName, "en{'Material Contractor'}de{'Material Vertragspartner'}", Const.ContextDatabase + "\\" + CompanyMaterial.ClassName, "", true)]
    [ACPropertyEntity(2, "StockQuantity", ConstApp.StockQuantity, "", "", true)]
    [ACPropertyEntity(3, "StockWeight", ConstApp.StockWeight, "", "", true)]
    [ACPropertyEntity(4, MDReleaseState.ClassName, ConstApp.ESReleaseState, Const.ContextDatabase + "\\" + MDReleaseState.ClassName, "", true)]

    [ACPropertyEntity(5, "DayInward", "en{'Day Inward Qty'}de{'Tageszugang'}", "", "", true)]
    [ACPropertyEntity(6, "DayTargetInward", "en{'Day Inward Target Qty'}de{'Tageszugang Soll'}", "", "", true)]
    // 7 DayInwardDiff
    // 8 DayInwardDiffPercent
    [ACPropertyEntity(9, "DayOutward", "en{'Day Outward Qty'}de{'Tagesabgang'}", "", "", true)]
    [ACPropertyEntity(10, "DayTargetOutward", "en{'Day Outward Target Qty'}de{'Tagesabgang Soll'}", "", "", true)]
    // 11 DayOutwardDiff
    // 12 DayOutwardDiffPercent
    [ACPropertyEntity(13, "WeekInward", "en{'Week Inward Qty'}de{'Wochenzugang'}", "", "", true)]
    [ACPropertyEntity(14, "WeekTargetInward", "en{'Week Inward Target Qty'}de{'Wochenzugang Soll'}", "", "", true)]
    // 15 WeekInwardDiff
    // 16 WeekInwardDiffPercent
    [ACPropertyEntity(17, "WeekOutward", "en{'Week Outward Qty'}de{'Wochenabgang'}", "", "", true)]
    [ACPropertyEntity(18, "WeekTargetOutward", "en{'Week Outward Target Qty'}de{'Wochenabgang Soll'}", "", "", true)]
    // 19 WeekOutwardDiff
    // 20 WeekOutwardDiffPercent
    [ACPropertyEntity(21, "MonthInward", "en{'Month Inward Qty'}de{'Monatszugang'}", "", "", true)]
    [ACPropertyEntity(22, "MonthTargetInward", "en{'Month Inward Target Qty'}de{'Monatszugang Soll'}", "", "", true)]
    // 23 MonthInwardDiff
    // 24 MonthInwardDiffPercent
    [ACPropertyEntity(25, "MonthOutward", "en{'Month Outward Qty'}de{'Monatsabgang'}", "", "", true)]
    [ACPropertyEntity(26, "MonthTargetOutward", "en{'Month Outward Target Qty'}de{'Monatsabgang Soll'}", "", "", true)]
    // 27 MonthOutwardDiff
    // 28 MonthOutwardDiffPercent
    [ACPropertyEntity(29, "YearInward", "en{'Year Inward Qty'}de{'Jahreszugang'}", "", "", true)]
    [ACPropertyEntity(30, "YearTargetInward", "en{'Year Inward Target Qty'}de{'Jahreszugang Soll'}", "", "", true)]
    // 31 YearInwardDiff
    // 32 YearInwardDiffPercent
    [ACPropertyEntity(33, "YearOutward", "en{'Year Outward Qty'}de{'Jahresabgang'}", "", "", true)]
    [ACPropertyEntity(34, "YearTargetOutward", "en{'Year Outward Target Qty'}de{'Jahresabgang Soll'}", "", "", true)]
    // 35 YearOutwardDiff
    // 36 YearOutwardDiffPercent
    [ACPropertyEntity(37, "DayAdjustment", "en{'Day Adjustment'}de{'Tageskorrektur'}", "", "", true)]
    [ACPropertyEntity(38, "DayBalanceDate", "en{'Day Balance Date'}de{'Tagesbilanz Datum'}", "", "", true)]
    [ACPropertyEntity(40, "DayLastOutward", "en{'Last Day Outward'}de{'Letzter Tagesabgang'}", "", "", true)]
    [ACPropertyEntity(41, "DayLastStock", "en{'Last Daily Stock'}de{'Letzter Tagesbestand'}", "", "", true)]
    [ACPropertyEntity(42, "WeekAdjustment", "en{'Week Adjustment'}de{'Wochenkorrektur'}", "", "", true)]
    [ACPropertyEntity(43, "WeekBalanceDate", "en{'Week Balance Date'}de{'Wochenbilanz Datum'}", "", "", true)]
    [ACPropertyEntity(44, "MonthActStock", "en{'Current Monthly Stock'}de{'Aktueller Monatsbestand'}", "", "", true)]
    [ACPropertyEntity(45, "MonthAdjustment", "en{'Month Adjustment'}de{Monatskorrektur'}", "", "", true)]
    [ACPropertyEntity(46, "MonthAverageStock", "en{'Average Monthly Stock'}de{'Monatlicher Durchschnittsbestand'}", "", "", true)]
    [ACPropertyEntity(47, "MonthBalanceDate", "en{'Month Balance Date'}de{'Monatsbilanz Datum'}", "", "", true)]
    [ACPropertyEntity(49, "MonthLastOutward", "en{'Last Month Outward'}de{'Letzter Monatsabgang'}", "", "", true)]
    [ACPropertyEntity(50, "MonthLastStock", "en{'Last Monthly Stock'}de{'Letzter Monatsbestand'}", "", "", true)]
    [ACPropertyEntity(51, "YearAdjustment", "en{'Annual Adjustment'}de{'Jahreskorrektur'}", "", "", true)]
    [ACPropertyEntity(52, "YearBalanceDate", "en{'Annual Balance Sheet Date'}de{'Jahresbilanz Datum'}", "", "", true)]
    [ACPropertyEntity(53, "ReservedInwardQuantity", ConstApp.ReservedInwardQuantity, "", "", true)]
    [ACPropertyEntity(54, "ReservedOutwardQuantity", ConstApp.ReservedOutwardQuantity, "", "", true)]
    // 59 ReservedQuantity
    // 60 AvailableQuantity
    [ACPropertyEntity(61, "AverageProdQuantity", "en{'Average Prod. Quantity'}de{'Duchschnittliche Produktionsmenge'}", "", "", true)]

    [ACPropertyEntity(62, "StockQuantityAmb", "en{'Stock Quantity Ambient'}de{'Lagermenge ambient'}", "", "", true)]
    [ACPropertyEntity(63, "DayInwardAmb", "en{'Day Inward Qty Ambient'}de{'Tageszugang ambient'}", "", "", true)]
    [ACPropertyEntity(64, "DayTargetInwardAmb", "en{'Day Inward Target Qty Ambient'}de{'Tageszugang Soll ambient'}", "", "", true)]
    [ACPropertyEntity(65, "DayOutwardAmb", "en{'Day Outward Qty Ambient'}de{'Tagesabgang ambient'}", "", "", true)]
    [ACPropertyEntity(66, "DayTargetOutwardAmb", "en{'Day Outward Target Qty Ambient'}de{'Tagesabgang Soll ambient'}", "", "", true)]
    [ACPropertyEntity(67, "WeekInwardAmb", "en{'Week Inward Qty Ambient'}de{'Wochenzugang ambient'}", "", "", true)]
    [ACPropertyEntity(68, "WeekTargetInwardAmb", "en{'Week Inward Target Qty Ambient'}de{'Wochenzugang Soll ambient'}", "", "", true)]
    [ACPropertyEntity(69, "WeekOutwardAmb", "en{'Week Outward Qty Ambient'}de{'Wochenabgang ambient'}", "", "", true)]
    [ACPropertyEntity(70, "WeekTargetOutwardAmb", "en{'Week Outward Target Qty Ambient'}de{'Wochenabgang Soll ambient'}", "", "", true)]
    [ACPropertyEntity(71, "MonthInwardAmb", "en{'Month Inward Qty Ambient'}de{'Monatszugang ambient'}", "", "", true)]
    [ACPropertyEntity(72, "MonthTargetInwardAmb", "en{'Month Inward Target Qty Ambient'}de{'Monatszugang Soll ambient'}", "", "", true)]
    [ACPropertyEntity(73, "MonthOutwardAmb", "en{'Month Outward Qty Ambient'}de{'Monatsabgang ambient'}", "", "", true)]
    [ACPropertyEntity(74, "MonthTargetOutwardAmb", "en{'Month Outward Target Qty Ambient'}de{'Monatsabgang Soll ambient'}", "", "", true)]
    [ACPropertyEntity(75, "YearInwardAmb", "en{'Year Inward Qty Ambient'}de{'Jahreszugang ambient'}", "", "", true)]
    [ACPropertyEntity(76, "YearTargetInwardAmb", "en{'Year Inward Target Qty Ambient'}de{'Jahreszugang Soll ambient'}", "", "", true)]
    [ACPropertyEntity(77, "YearOutwardAmb", "en{'Year Outward Qty Ambient'}de{'Jahresabgang ambient'}", "", "", true)]
    [ACPropertyEntity(78, "YearTargetOutwardAmb", "en{'Year Outward Target Qty Ambient'}de{'Jahresabgang Soll ambient'}", "", "", true)]
    [ACPropertyEntity(79, "DayAdjustmentAmb", "en{'Day Adjustment Ambient'}de{'Tageskorrektur ambient'}", "", "", true)]
    [ACPropertyEntity(80, "DayLastOutwardAmb", "en{'Last Day Outward Qty Ambient'}de{'Letzter Tagesabgang ambient'}", "", "", true)]
    [ACPropertyEntity(81, "DayLastStockAmb", "en{'Last Daily Stock Ambient'}de{'Letzter Tagesbestand ambient'}", "", "", true)]
    [ACPropertyEntity(82, "WeekAdjustmentAmb", "en{'Week Adjustment Ambient'}de{'Wochenkorrektur ambient'}", "", "", true)]
    [ACPropertyEntity(83, "MonthActStockAmb", "en{'Current Monthly Stock Ambient'}de{'Aktueller Monatsbestand ambient'}", "", "", true)]
    [ACPropertyEntity(84, "MonthAdjustmentAmb", "en{'Month Adjustment Ambient'}de{'Monatskorrektur ambient'}", "", "", true)]
    [ACPropertyEntity(85, "MonthAverageStockAmb", "en{'Month Average Stock Ambient'}de{'Monatlicher Durchschnittsbestand ambient'}", "", "", true)]
    [ACPropertyEntity(86, "MonthLastOutwardAmb", "en{'Last Month Outward Qty Ambient'}de{'Letzter Monatsabgang ambient'}", "", "", true)]
    [ACPropertyEntity(87, "MonthLastStockAmb", "en{'Last Monthly Stock Ambient'}de{'Letzter Monatsbestand ambient'}", "", "", true)]
    [ACPropertyEntity(88, "YearAdjustmentAmb", "en{'Year Adjustment Ambient'}de{'Jahreskorrektur ambient'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]

    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + CompanyMaterialStock.ClassName, "en{'CompanyMaterialStock'}de{'Materialbestand'}", typeof(CompanyMaterialStock), CompanyMaterialStock.ClassName, CompanyMaterial.ClassName + "\\CompanyMaterialNo", CompanyMaterial.ClassName + "\\CompanyMaterialNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<CompanyMaterialStock>) })]
    public partial class CompanyMaterialStock
    {
        public const string ClassName = "CompanyMaterialStock";

        #region New/Delete
        public static CompanyMaterialStock NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            CompanyMaterialStock entity = new CompanyMaterialStock();
            entity.CompanyMaterialStockID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDReleaseState = MDReleaseState.DefaultMDReleaseState(dbApp);
            if (parentACObject is CompanyMaterial)
            {
                CompanyMaterial material = parentACObject as CompanyMaterial;
                entity.CompanyMaterial = material;
                entity.CompanyMaterial.CompanyMaterialStock_CompanyMaterial.Add(entity);
            }
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
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
                return this.CompanyMaterial.ACCaption;
            }
        }

        /// <summary>
        /// Returns CompanyMaterial
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to CompanyMaterial</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return this.CompanyMaterial;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "CompanyMaterial\\CompanyMaterialNo";
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
            if (this.CompanyMaterial == null)
                return;

            //this.ReservedInwardQuantity = 0;
            //// TODO: OR-Klausel einfügen für Produktionsaufträge
            //List<FacilityReservation> facilityReservationList = (from c in this.CompanyMaterial.FacilityReservation_CompanyMaterial where c.InOrderPos != null select c).ToList();
            //foreach (FacilityReservation facilityReservation in facilityReservationList)
            //{
            //    // Die ActualQuantity gibt an, wieviel bereits auf dem Material gebucht worden ist
            //    // Die TargetQuantity gibt an, wieviel Reserviert ist
            //    // Die Differenz gibt an, wieviel noch geliefert wird
            //    // TODO: Zugänge von Produktionsaufträge mit einrechnen
            //    this.ReservedInwardQuantity += facilityReservation.InOrderPos.TargetQuantity - facilityReservation.InOrderPos.ActualQuantity;
            //}
        }



        /// <summary>
        /// There are no comments for Property ReservedOutwardQuantity in the schema.
        /// </summary>
        public void RecalcReservedOutwardQuantity()
        {
            if (this.CompanyMaterial == null)
                return;

            //this.ReservedOutwardQuantity = 0;
            //// TODO: OR-Klausel einfügen für Produktionsaufträge
            //List<FacilityReservation> facilityReservationList = (from c in CompanyMaterial.FacilityReservation_Material where c.OutOrderPos != null select c).ToList();
            //foreach (FacilityReservation facilityReservation in facilityReservationList)
            //{
            //    // Die ActualQuantity gibt an, wieviel bereits auf dem Material gebucht worden ist
            //    // Die TargetQuantity gibt an, wieviel Reserviert ist
            //    // Die Differenz gibt an, wieviel noch abgebucht wird
            //    // TODO: Abgänge von Produktionsaufträgen mit einrechnen
            //    this.ReservedOutwardQuantity += facilityReservation.OutOrderPos.TargetQuantity - facilityReservation.OutOrderPos.ActualQuantity;

            //}
        }



        /// <summary>
        /// There are no comments for Property ReservedQuantity in the schema.
        /// </summary>
        [ACPropertyInfo(90, "", "en{'Reserved Quantity'}de{'Reservierte Menge'}")]
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
        [ACPropertyInfo(91, "", "en{'Available Quantity'}de{'Verfügbare Menge'}")]
        public Double AvailableQuantity
        {
            get
            {
                return StockQuantity - ReservedQuantity;
            }
        }

        partial void OnReservedOutwardQuantityChanged()
        {
            RefreshMinOptFields();
        }

        partial void OnReservedInwardQuantityChanged()
        {
            RefreshMinOptFields();
        }

        partial void OnStockQuantityChanged()
        {
            RefreshMinOptFields();
        }

        public void RefreshMinOptFields()
        {
            OnPropertyChanged("AvailableQuantity");
            OnPropertyChanged("ReservedQuantity");
            OnPropertyChanged("MinStockQuantityDiff");
            OnPropertyChanged("MinStockQuantityExceeded");
            OnPropertyChanged("OptStockQuantityDiff");
            OnPropertyChanged("OptStockQuantityExceeded");
            if (this.CompanyMaterial != null)
                this.CompanyMaterial.OnEntityPropertyChanged(null);
        }

        [ACPropertyInfo(92, "", "en{'Min. Stock diff.'}de{'Min. Bestand Diff.'}")]
        public Double? MinStockQuantityDiff
        {
            get
            {
                if (this.CompanyMaterial == null)
                    return null;
                if (!this.CompanyMaterial.MinStockQuantity.HasValue)
                    return null;
                return AvailableQuantity - CompanyMaterial.MinStockQuantity;
            }
        }

        [ACPropertyInfo(93, "", "en{'Min. Stock Exceeded'}de{'Min. Bestand überschritten'}")]
        public bool MinStockQuantityExceeded
        {
            get
            {
                if (!MinStockQuantityDiff.HasValue || MinStockQuantityDiff.Value > 0)
                    return false;
                return true;
            }
        }

        [ACPropertyInfo(94, "", "en{'Opt. Stock Diff.'}de{'Opt. Bestand Diff.'}")]
        public Double? OptStockQuantityDiff
        {
            get
            {
                if (this.CompanyMaterial == null)
                    return null;
                if (!this.CompanyMaterial.OptStockQuantity.HasValue)
                    return null;
                return AvailableQuantity - CompanyMaterial.OptStockQuantity;
            }
        }

        [ACPropertyInfo(95, "", "en{'Opt. Stock exceeded'}de{'Opt. Bestand überschritten'}")]
        public bool OptStockQuantityExceeded
        {
            get
            {
                if (!OptStockQuantityDiff.HasValue || OptStockQuantityDiff.Value > 0)
                    return false;
                return true;
            }
        }


        #endregion
    }
}
