using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace gip.mes.datamodel
{
    // MaterialStock (Artikel-Bestand)
    [ACClassInfo(Const.PackName_VarioMaterial, ConstApp.MaterialStock, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(2, "StockQuantity", ConstApp.StockQuantity, "", "", true)]
    [ACPropertyEntity(3, "StockWeight", ConstApp.StockWeight, "", "", true)]
    [ACPropertyEntity(4, MDReleaseState.ClassName, ConstApp.ESReleaseState, Const.ContextDatabase + "\\" + MDReleaseState.ClassName, "", true)]

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
    [ACPropertyEntity(44, "MonthActStock", ConstApp.MonthActStock, "", "", true)]
    [ACPropertyEntity(45, "MonthAdjustment", ConstApp.MonthAdjustment, "", "", true)]
    [ACPropertyEntity(46, "MonthAverageStock", ConstApp.MonthAverageStock, "", "", true)]
    [ACPropertyEntity(47, "MonthBalanceDate", ConstApp.MonthBalanceDate, "", "", true)]
    [ACPropertyEntity(49, "MonthLastOutward", ConstApp.MonthLastOutward, "", "", true)]
    [ACPropertyEntity(50, "MonthLastStock", ConstApp.MonthLastStock, "", "", true)]
    [ACPropertyEntity(51, "YearAdjustment", ConstApp.YearAdjustment, "", "", true)]
    [ACPropertyEntity(52, "YearBalanceDate", ConstApp.YearBalanceDate, "", "", true)]
    [ACPropertyEntity(53, "ReservedInwardQuantity", ConstApp.ReservedInwardQuantity, "", "", true)]
    [ACPropertyEntity(54, "ReservedOutwardQuantity", ConstApp.ReservedOutwardQuantity, "", "", true)]
    [ACPropertyEntity(61, "AverageProdQuantity", "en{'Average Production Qty'}de{'Duchschnittliche Produktionsmenge'}", "", "", true)]

    [ACPropertyEntity(62, "StockQuantityAmb", ConstApp.StockQuantityAmb, "", "", true)]
    [ACPropertyEntity(63, "DayInwardAmb", ConstApp.DayInwardAmb, "", "", true)]
    [ACPropertyEntity(64, "DayTargetInwardAmb", ConstApp.DayTargetInwardAmb, "", "", true)]
    [ACPropertyEntity(65, "DayOutwardAmb", ConstApp.DayOutwardAmb, "", "", true)]
    [ACPropertyEntity(66, "DayTargetOutwardAmb", ConstApp.DayTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(67, "WeekInwardAmb", ConstApp.WeekInwardAmb, "", "", true)]
    [ACPropertyEntity(68, "WeekTargetInwardAmb", ConstApp.WeekTargetInwardAmb, "", "", true)]
    [ACPropertyEntity(69, "WeekOutwardAmb", ConstApp.WeekOutwardAmb, "", "", true)]
    [ACPropertyEntity(70, "WeekTargetOutwardAmb", ConstApp.WeekTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(71, "MonthInwardAmb", ConstApp.MonthInwardAmb, "", "", true)]
    [ACPropertyEntity(72, "MonthTargetInwardAmb", ConstApp.MonthTargetInwardAmb, "", "", true)]
    [ACPropertyEntity(73, "MonthOutwardAmb", ConstApp.MonthOutwardAmb, "", "", true)]
    [ACPropertyEntity(74, "MonthTargetOutwardAmb", ConstApp.MonthTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(75, "YearInwardAmb", ConstApp.YearInwardAmb, "", "", true)]
    [ACPropertyEntity(76, "YearTargetInwardAmb", ConstApp.YearTargetInwardAmb, "", "", true)]
    [ACPropertyEntity(77, "YearOutwardAmb", ConstApp.YearOutwardAmb, "", "", true)]
    [ACPropertyEntity(78, "YearTargetOutwardAmb", ConstApp.YearTargetOutwardAmb, "", "", true)]
    [ACPropertyEntity(79, "DayAdjustmentAmb", ConstApp.DayAdjustmentAmb, "", "", true)]
    [ACPropertyEntity(80, "DayLastOutwardAmb", ConstApp.DayLastOutwardAmb, "", "", true)]
    [ACPropertyEntity(81, "DayLastStockAmb", ConstApp.DayLastStockAmb, "", "", true)]
    [ACPropertyEntity(82, "WeekAdjustmentAmb", ConstApp.WeekAdjustmentAmb, "", "", true)]
    [ACPropertyEntity(83, "MonthActStockAmb", ConstApp.MonthActStockAmb, "", "", true)]
    [ACPropertyEntity(84, "MonthAdjustmentAmb", ConstApp.MonthAdjustmentAmb, "", "", true)]
    [ACPropertyEntity(85, "MonthAverageStockAmb", ConstApp.MonthAverageStockAmb, "", "", true)]
    [ACPropertyEntity(86, "MonthLastOutwardAmb", ConstApp.MonthLastOutwardAmb, "", "", true)]
    [ACPropertyEntity(87, "MonthLastStockAmb", ConstApp.MonthLastStockAmb, "", "", true)]
    [ACPropertyEntity(88, "YearAdjustmentAmb", ConstApp.YearAdjustmentAmb, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]

    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + MaterialStock.ClassName, ConstApp.MaterialStock, typeof(MaterialStock), MaterialStock.ClassName, Material.ClassName + "\\MaterialNo," + Material.ClassName + "\\MaterialName1", Material.ClassName + "\\MaterialNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaterialStock>) })]
    public partial class MaterialStock
    {
        public const string ClassName = "MaterialStock";

        #region New/Delete
        public static MaterialStock NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MaterialStock entity = new MaterialStock();
            entity.MaterialStockID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDReleaseState = MDReleaseState.DefaultMDReleaseState(dbApp);
            if (parentACObject is Material)
            {
                Material material = parentACObject as Material;
                entity.Material = material;
                entity.Material.MaterialStock_Material.Add(entity);
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
                return Material.ACCaption;
            }
        }

        /// <summary>
        /// Returns Material
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Material</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return Material;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "Material\\MaterialNo";
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
        /// Get weight. It's calculated over Density and StockQuantity - only if Dimension of BaseMDUnit is Volume
        /// </summary>
        [ACPropertyInfo(36, "", "en{'Weight 15°C'}de{'Gewicht 15°C'}")]
        public Double Weight15FromVolume
        {
            get
            {
                if (this.Material.BaseMDUnit == null || this.Material.BaseMDUnit.SIDimension != GlobalApp.SIDimensions.Volume || this.Material.BaseMDUnit.SIUnit == null)
                    return 0;
                if (Math.Abs(this.Material.Density - 0) <= double.Epsilon)
                    return 0;
                double quantity = this.StockQuantity;
                if (this.Material.BaseMDUnit != this.Material.BaseMDUnit.SIUnit)
                    quantity = this.Material.BaseMDUnit.ConvertToUnit(this.StockQuantity, this.Material.BaseMDUnit.SIUnit);
                return quantity * this.Material.Density;
            }
        }

        /// <summary>
        /// Get weight. It's calculated over Density and ambien5t StockQuantity - only if Dimension of BaseMDUnit is Volume
        /// </summary>
        [ACPropertyInfo(37, "", "en{'Weight Ambient'}de{'Gewicht Ambient'}")]
        public Double WeightAmbFromVolume
        {
            get
            {
                if (this.Material.BaseMDUnit == null || this.Material.BaseMDUnit.SIDimension != GlobalApp.SIDimensions.Volume || this.Material.BaseMDUnit.SIUnit == null)
                    return 0;
                if (Math.Abs(this.Material.DensityAmb - 0) <= double.Epsilon)
                    return 0;
                double quantity = this.StockQuantityAmb;
                if (this.Material.BaseMDUnit != this.Material.BaseMDUnit.SIUnit)
                    quantity = this.Material.BaseMDUnit.ConvertToUnit(this.StockQuantityAmb, this.Material.BaseMDUnit.SIUnit);
                return quantity * this.Material.DensityAmb;
            }
        }

        /// <summary>
        /// There are no comments for Property ReservedInwardQuantity in the schema.
        /// </summary>
        public void RecalcReservedInwardQuantity()
        {
            if (this.Material == null)
                return;

            this.ReservedInwardQuantity = 0;
            // TODO: OR-Klausel einfügen für Produktionsaufträge
            List<FacilityReservation> facilityReservationList = (from c in Material.FacilityReservation_Material where c.InOrderPos != null select c).ToList();
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
            if (this.Material == null)
                return;

            this.ReservedOutwardQuantity = 0;
            // TODO: OR-Klausel einfügen für Produktionsaufträge
            List<FacilityReservation> facilityReservationList = (from c in Material.FacilityReservation_Material where c.OutOrderPos != null select c).ToList();
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
            if (this.Material != null)
                this.Material.OnEntityPropertyChanged(null);
        }

        [ACPropertyInfo(92, "", "en{'Min. Stock Diff.'}de{'Min. Bestand Diff.'}")]
        public Double? MinStockQuantityDiff
        {
            get
            {
                if (this.Material == null)
                    return null;
                if (!this.Material.MinStockQuantity.HasValue)
                    return null;
                return AvailableQuantity - Material.MinStockQuantity;
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
                if (this.Material == null)
                    return null;
                if (!this.Material.OptStockQuantity.HasValue)
                    return null;
                return AvailableQuantity - Material.OptStockQuantity;
            }
        }

        [ACPropertyInfo(95, "", "en{'Opt. Stock Exceeded'}de{'Opt. Bestand überschritten'}")]
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
