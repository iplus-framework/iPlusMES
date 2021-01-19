using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions; using gip.core.datamodel;

namespace gip.mes.datamodel
{
    //    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Productionorder'}de{'Produktionsauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSODemandProdOrder")]
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Productionorder'}de{'Produktionsauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, "ProgramNo", "en{'ProgramNo'}de{'Programmnummer'}","", "", true)]
    [ACPropertyEntity(2, "TenantCompany", "en{'Tenant'}de{'Besitzer'}", Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + DemandProdOrder.ClassName, "en{'Productionorder'}de{'Produktionsauftrag'}", typeof(DemandProdOrder), DemandProdOrder.ClassName, "ProgramNo", "ProgramNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<DemandProdOrder>) })]
    public partial class DemandProdOrder : IACObject
    {
        public const string ClassName = "DemandProdOrder";

        #region new/Delete
        public static DemandProdOrder NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {

            DemandProdOrder entity = new DemandProdOrder();
            entity.DemandProdOrderID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is DemandOrder)
            {
                entity.DemandOrder = parentACObject as DemandOrder;
            }
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl

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
                return ProgramNo;
            }
        }

        #endregion

        #region IACObjectEntity Members
        static public string KeyACIdentifier
        {
            get
            {
                return "ProgramNo";
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
