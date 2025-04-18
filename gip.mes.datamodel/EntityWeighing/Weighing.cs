using System;
using gip.core.datamodel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.Weighing, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, false, "", "BSOWeighing")]
    [ACPropertyEntity(1, "WeighingNo", "en{'Weighing-No.'}de{'Wägungs-Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "VBiACClassID", "en{'Scale'}de{'Waage'}", "", "", true)]
    [ACPropertyEntity(3, "Weight", "en{'Weight'}de{'Gewicht'}", "", "", true)]
    [ACPropertyEntity(4, "IdentNr", "en{'Weighing-ID'}de{'Wägeidentnr.'}", "", "", true)]
    [ACPropertyEntity(5, "StartDate", "en{'Start weighing'}de{'Start Wägung'}", "", "", true)]
    [ACPropertyEntity(6, "EndDate", "en{'End weighing'}de{'Ende Wägung'}", "", "", true)]
    [ACPropertyEntity(11, OutOrderPos.ClassName, "en{'Orderline (Delivery)'}de{'Warenausgangsposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(12, InOrderPos.ClassName, "en{'Orderline (Goods issue)'}de{'Wareneingangsposition'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(13, PickingPos.ClassName, "en{'Commissioningline'}de{'Bestellposition'}", Const.ContextDatabase + "\\" + PickingPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(14, LabOrderPos.ClassName, "en{'Lab order line'}de{'Laborauftrag Position'}", Const.ContextDatabase + "\\" + LabOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(15, VisitorVoucher.ClassName, "en{'Visitor voucher'}de{'Besucherbeleg'}", Const.ContextDatabase + "\\" + VisitorVoucher.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACPropertyEntity(9999, "WeighingTotalXML", "en{'Serialized Weighingdata'}de{'Serialisierte Wägedaten'}", "", "", false)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + Weighing.ClassName, "en{'Weighing'}de{'Wägung'}", typeof(Weighing), Weighing.ClassName, "WeighingNo", "WeighingNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<Weighing>) })]
    [NotMapped]
    public partial class Weighing
    {
        [NotMapped]
        public const string ClassName = "Weighing";
        [NotMapped]
        public const string NoColumnName = "WeighingNo";
        [NotMapped]
        public const string FormatNewNo = "WG{0}";

        #region New/Delete
        public static Weighing NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            Weighing entity = new Weighing();
            entity.WeighingID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.WeighingNo = secondaryKey;
            entity.WeighingState = WeighingStateEnum.New;

            LabOrderPos lOPos = parentACObject as LabOrderPos;
            if (lOPos != null)
                entity.LabOrderPosID = lOPos.LabOrderPosID;

            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        [ACPropertyInfo(999)]
        public WeighingStateEnum WeighingState
        {
            get
            {
                return (WeighingStateEnum)StateIndex;
            }
            set
            {
                StateIndex = (Int16)value;
            }
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
                return WeighingNo;
            }
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "WeighingNo";
            }
        }
        #endregion

        #region IEntityProperty Members

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

        #endregion

        #region VBIplus-Context
        [NotMapped]
        private gip.core.datamodel.ACClass _ACClass;
        [NotMapped]
        [ACPropertyInfo(9999, "", "en{'Scale'}de{'Waage'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClass.ClassName)]
        public gip.core.datamodel.ACClass ACClass
        {
            get
            {
                if (!this.VBiACClassID.HasValue|| this.VBiACClassID.Value == Guid.Empty)
                    return null;
                if (_ACClass != null)
                    return _ACClass;
                DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                if (dbApp != null)
                    _ACClass = dbApp.ContextIPlus.GetACType(this.VBiACClassID.Value);
                return _ACClass;
            }
            set
            {
                if (value == null)
                    _ACClass = null;
                else
                {
                    if (_ACClass != null && value == _ACClass)
                        return;
                    _ACClass = value;
                    this.VBiACClassID = _ACClass != null ? _ACClass.ACClassID : (Guid?)null;
                }
            }
        }

        #endregion


    }
}




