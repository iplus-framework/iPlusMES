using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Weighing'}de{'Wägung'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, false, "", "BSOWeighing")]
    [ACPropertyEntity(1, "WeighingNo", "en{'Weighing-No.'}de{'Wägungs-Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "VBiACClassID", "en{'Scale'}de{'Waage'}", "", "", true)]
    [ACPropertyEntity(3, "Weight", "en{'Weight'}de{'Gewicht'}", "", "", true)]
    [ACPropertyEntity(4, "IdentNr", "en{'Weighing-ID'}de{'Wägeidentnr.'}", "", "", true)]
    [ACPropertyEntity(5, "StartDate", "en{'Start weighing'}de{'Start Wägung'}", "", "", true)]
    [ACPropertyEntity(6, "EndDate", "en{'End weighing'}de{'Ende Wägung'}", "", "", true)]
    [ACPropertyEntity(11, OutOrderPos.ClassName, "en{'Orderline (Delivery)'}de{'Warenausgangsposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName, "", true)]
    [ACPropertyEntity(12, InOrderPos.ClassName, "en{'Orderline (Goods issue)'}de{'Wareneingangsposition'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName, "", true)]
    [ACPropertyEntity(13, PickingPos.ClassName, "en{'Commissioningline'}de{'Bestellposition'}", Const.ContextDatabase + "\\" + PickingPos.ClassName, "", true)]
    [ACPropertyEntity(14, LabOrderPos.ClassName, "en{'Lab order line'}de{'Laborauftrag Position'}", Const.ContextDatabase + "\\" + LabOrderPos.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(9999, "WeighingTotalXML", "en{'Serialized Weighingdata'}de{'Serialisierte Wägedaten'}", "", "", false)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + Weighing.ClassName, "en{'Weighing'}de{'Wägung'}", typeof(Weighing), Weighing.ClassName, "WeighingNo", "WeighingNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<Weighing>) })]
    public partial class Weighing
    {
        public const string ClassName = "Weighing";
        public const string NoColumnName = "WeighingNo";
        public const string FormatNewNo = "WG{0}";

        #region New/Delete
        public static Weighing NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            Weighing entity = new Weighing();
            entity.WeighingID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.WeighingNo = secondaryKey;

            LabOrderPos lOPos = parentACObject as LabOrderPos;
            if (lOPos != null)
                entity.LabOrderPosID = lOPos.LabOrderPosID;

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
                return WeighingNo;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "WeighingNo";
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

    }
}




