using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Transactions; 
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    //    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Productionorder'}de{'Produktionsauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSODemandProdOrder")]
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Productionorder'}de{'Produktionsauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "")]
    [ACPropertyEntity(1, "ProgramNo", "en{'ProgramNo'}de{'Programmnummer'}","", "", true)]
    [ACPropertyEntity(2, "TenantCompany", "en{'Tenant'}de{'Besitzer'}", Const.ContextDatabase + "\\" + Company.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + DemandProdOrder.ClassName, "en{'Productionorder'}de{'Produktionsauftrag'}", typeof(DemandProdOrder), DemandProdOrder.ClassName, "ProgramNo", "ProgramNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<DemandProdOrder>) })]
    public partial class DemandProdOrder : IACObject
    {
        [NotMapped]
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
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
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
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return ProgramNo;
            }
        }

        #endregion

        #region IACObjectEntity Members
        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "ProgramNo";
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
    }
}
