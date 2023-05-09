using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    // FacilityHistory (LagerHistorie)
    [ACClassInfo(Const.PackName_VarioFacility, "en{'Facility History'}de{'Lagerhistorie'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, Facility.ClassName, ConstApp.Facility, Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(2, "StockQuantity", ConstApp.StockQuantity, "", "", true)]
    [ACPropertyEntity(3, "Inward", "en{'Inward Quantity'}de{'Zugangsmenge'}", "", "", true)]
    [ACPropertyEntity(4, "Outward", "en{'Outward Quantity'}de{'Abgangsmenge'}", "", "", true)]
    [ACPropertyEntity(5, "TargetInward", "en{'Target Inward Quantity'}de{'Ziel Zugangsmenge'}", "", "", true)]
    [ACPropertyEntity(6, "TargetOutward", "en{'Target Outward Quantity'}de{'Ziel Abgangsmenge'}", "", "", true)]
    [ACPropertyEntity(7, "Adjustment", "en{'Adjustment'}de{'Korrektur'}", "", "", true)]
    [ACPropertyEntity(8, "LastStockQuantity", "en{'Last Stock'}de{'Letzte Lagermenge'}", "", "", true)]
    [ACPropertyEntity(10, "StockQuantityAmb", "en{'Stock Quantity Ambient'}de{'Lagermenge ambient'}", "", "", true)]
    [ACPropertyEntity(11, "InwardAmb", "en{'Inward Quantity Ambient'}de{'Zugangsmenge ambient'}", "", "", true)]
    [ACPropertyEntity(12, "OutwardAmb", "en{'Outward Quantity Ambient'}de{'Abgangsmenge ambient'}", "", "", true)]
    [ACPropertyEntity(13, "TargetInwardAmb", "en{'Target Inward Qty Ambient'}de{'Ziel Zugangsmenge ambient'}", "", "", true)]
    [ACPropertyEntity(14, "TargetOutwardAmb", "en{'Target Outward Qty Ambient'}de{'Ziel Abgangsmenge ambient'}", "", "", true)]
    [ACPropertyEntity(15, "AdjustmentAmb", "en{'Adjustment Ambient'}de{'Korrektur ambient'}", "", "", true)]
    [ACPropertyEntity(16, "LastStockQuantityAmb", "en{'Last Stock Ambient'}de{'Letzte Lagermenge ambient'}", "", "", true)]
    [ACPropertyEntity(17, "MinStockQuantity", ConstApp.MinStockQuantity, "", "", true)]
    [ACPropertyEntity(18, "OptStockQuantity", ConstApp.OptStockQuantity, "", "", true)]
    [ACPropertyEntity(19, "ReservedInwardQuantity", ConstApp.ReservedInwardQuantity, "", "", true)]
    [ACPropertyEntity(20, "ReservedOutwardQuantity", ConstApp.ReservedOutwardQuantity, "", "", true)]
    [ACPropertyEntity(9999, History.ClassName, "en{'Balance Sheet History'}de{'Bilanzhistorie'}", Const.ContextDatabase + "\\" + History.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityHistory.ClassName, "en{'Facility History'}de{'Lagerhistorie'}", typeof(FacilityHistory), FacilityHistory.ClassName, Facility.ClassName + "\\FacilityNo,History\\BalanceDate", Facility.ClassName + "\\FacilityNo,History\\BalanceDate")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<FacilityHistory>) })]
    public partial class FacilityHistory
    {
        public const string ClassName = "FacilityHistory";

        #region New/Delete
        public static FacilityHistory NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            FacilityHistory entity = new FacilityHistory();
            entity.FacilityHistoryID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is History)
            {
                History history = parentACObject as History;
                entity.History = history;
                history.FacilityHistory_History.Add(entity);
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
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return Facility.ACCaption;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "Facility\\FacilityNo,History\\BalanceDate,History\\TimePeriodIndex";
            }
        }

        #endregion

        [NotMapped]
        bool bRefreshConfig = false;
        protected override void OnPropertyChanging<T>(T newValue, string propertyName, bool afterChange)
        {
            if (propertyName == nameof(XMLConfig))
            {
                string xmlConfig = newValue as string;
                if (afterChange)
                {
                    if (bRefreshConfig)
                        ACProperties.Refresh();
                }
                else
                {
                    bRefreshConfig = false;
                    if (this.EntityState != EntityState.Detached && (!(String.IsNullOrEmpty(xmlConfig) && String.IsNullOrEmpty(XMLConfig)) && xmlConfig != XMLConfig))
                        bRefreshConfig = true;
                }
            }
            base.OnPropertyChanging(newValue, propertyName, afterChange);
        }

        #region additional properties

        /// <summary>
        /// There are no comments for Property ReservedQuantity in the schema.
        /// </summary>
        [ACPropertyInfo(90)]
        [NotMapped]
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
        [ACPropertyInfo(91)]
        [NotMapped]
        public Double AvailableQuantity
        {
            get
            {
                return StockQuantity - ReservedQuantity;
            }
        }

        [ACPropertyInfo(94, "", "en{'Min. Stock Diff.'}de{'Min. Bestand Diff.'}")]
        [NotMapped]
        public Double? MinStockQuantityDiff
        {
            get
            {
                if (!MinStockQuantity.HasValue)
                    return null;
                return AvailableQuantity - MinStockQuantity;
            }
        }

        [ACPropertyInfo(95, "", "en{'Min. Stock Exceeded'}de{'Min. Bestand überschritten'}")]
        [NotMapped]
        public bool MinStockQuantityExceeded
        {
            get
            {
                if (!MinStockQuantityDiff.HasValue || MinStockQuantityDiff.Value > 0)
                    return false;
                return true;
            }
        }

        [ACPropertyInfo(96, "", "en{'Opt. Stock Diff.'}de{'Opt. Bestand Diff.'}")]
        [NotMapped]
        public Double? OptStockQuantityDiff
        {
            get
            {
                if (!OptStockQuantity.HasValue)
                    return null;
                return AvailableQuantity - OptStockQuantity;
            }
        }

        [ACPropertyInfo(7, "", "en{'Opt. Stock Exceeded'}de{'Opt. Bestand überschritten'}")]
        [NotMapped]
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



