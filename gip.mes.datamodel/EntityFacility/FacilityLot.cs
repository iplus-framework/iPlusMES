using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace gip.mes.datamodel
{
    // FacilityLot (Chargenplatz)
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.Lot, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "LotNo", ConstApp.LotNo, "", "", true)]
    [ACPropertyEntity(2, MDReleaseState.ClassName, ConstApp.ESReleaseState, Const.ContextDatabase + "\\" + MDReleaseState.ClassName, "", true)]
    [ACPropertyEntity(3, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(6, "ExpirationDate", ConstApp.ExpirationDate, "", "", true)]
    [ACPropertyEntity(7, "FillingDate", ConstApp.FillingDate, "", "", true)]
    [ACPropertyEntity(8, "ProductionDate", ConstApp.ProductionDate, "", "", true)]
    [ACPropertyEntity(9, "StorageLife", ConstApp.StorageLife, "", "", true)]
    [ACPropertyEntity(20, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(21, Const.IsEnabled, Const.EntityIsEnabled, "", "", true)]
    [ACPropertyEntity(23, "Lock", ConstApp.Lock, "", "", true)]
    [ACPropertyEntity(27, "ExternLotNo", ConstApp.ExternLotNo, "", "", true)]
    [ACPropertyEntity(28, "ExternLotNo2", ConstApp.ExternLotNo2, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]

    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + FacilityLot.ClassName, ConstApp.Lot, typeof(FacilityLot), FacilityLot.ClassName, "LotNo", "LotNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<FacilityLot>) })]
    public partial class FacilityLot
    {
        public const string ClassName = "FacilityLot";
        public const string NoColumnName = "LotNo";
        public const string FormatNewNo = "FL{0}";

        #region New/Delete

        public static FacilityLot NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            FacilityLot entity = new FacilityLot();
            entity.FacilityLotID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.LotNo = secondaryKey;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        public override string ToString()
        {
            return LotNo;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                return LotNo;
            }
        }

        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && className == FacilityLotStock.ClassName)
                return this.FacilityLotStock_FacilityLot.Where(c => c.FacilityLot.LotNo == filterValues[0]).FirstOrDefault();
            return null;
        }

        #endregion

        #region IACObjectEntity Members

        public FacilityLotStock GetFacilityLotStock_InsertIfNotExists(DatabaseApp dbApp)
        {
            FacilityLotStock facilityStock = FacilityLotStock_FacilityLot.FirstOrDefault();
            if (facilityStock != null)
                return facilityStock;
            facilityStock = FacilityLotStock.NewACObject(dbApp, this);
            return facilityStock;
        }

        static public string KeyACIdentifier
        {
            get
            {
                return "LotNo";
            }
        }

        [ACPropertyInfo(9999, "", "en{'Storage Life Remaining'}de{'Verbleibende Haltbarkeit'}")]
        public int StorageLifeRemaining
        {
            get
            {
                if (ExpirationDate != null)
                {
                    if (((DateTime)ExpirationDate).Ticks > 0)
                        return (((DateTime)ExpirationDate) - DateTime.Now).Days;
                }
                return 0;
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

        /// <summary>
        /// There are no comments for Property CurrentFacilityStock in the schema.
        /// </summary>
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public FacilityLotStock CurrentFacilityLotStock
        {
            get
            {
                if (this.FacilityLotStock_FacilityLot.Count <= 0)
                    return null;
                return this.FacilityLotStock_FacilityLot.First();
            }
        }
        #endregion

        #region Cloning
        public object Clone(bool withReferences)
        {
            FacilityLot clonedObject = new FacilityLot();
            clonedObject.FacilityLotID = this.FacilityLotID;
            clonedObject.CopyFrom(this, withReferences);
            return clonedObject;
        }

        public void CopyFrom(FacilityLot from, bool withReferences)
        {
            if (withReferences)
            {
                MaterialID = from.MaterialID;
                MDReleaseStateID = from.MDReleaseStateID;
            }

            LotNo = from.LotNo;
            FillingDate = from.FillingDate;
            StorageLife = from.StorageLife;
            ProductionDate = from.ProductionDate;
            ExpirationDate = from.ExpirationDate;
            ExternLotNo = from.ExternLotNo;
            ExternLotNo2 = from.ExternLotNo2;
            Comment = from.Comment;
            Lock = from.Lock;
            IsEnabled = from.IsEnabled;
            XMLConfig = from.XMLConfig;
        }
        #endregion
    }
}