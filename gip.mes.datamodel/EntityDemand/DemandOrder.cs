using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Transactions;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Demandorder'}de{'Anforderungsauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSODemandOrder")]
    [ACPropertyEntity(1, "DemandOrderNo", "en{'Demandorderno'}de{'Anforderungsauftragnr'}", "", "", true)]
    [ACPropertyEntity(2, "DemandOrderName", "en{'Demandordername'}de{'Anforderungsauftragname'}", "", "", true)]
    [ACPropertyEntity(3, "DemandOrderDate", "en{'Date'}de{'Datum'}", "", "", true)]
    [ACPropertyEntity(4, "MDDemandOrderState", "en{'Demandorderstate'}de{'Anforderungsauftragsstatus'}", Const.ContextDatabase + "\\MDDemandOrderState", "", true)]
    [ACPropertyEntity(5, "TenantCompany", "en{'Tenant'}de{'Besitzer'}", Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + DemandOrder.ClassName, "en{'Demandorder'}de{'Anforderungsauftrag'}", typeof(DemandOrder), DemandOrder.ClassName, "DemandOrderNo", "DemandOrderNo", new object[]
        {
                new object[] {Const.QueryPrefix + DemandOrderPos.ClassName, "en{'Demandorderposistion'}de{'Anforderungsauftragsposition'}", typeof(DemandOrderPos), DemandOrderPos.ClassName + "_" + DemandOrder.ClassName, "Sequence", "Sequence"}
        })
    ]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<DemandOrder>) })]
    public partial class DemandOrder
    {
        public const string ClassName = "DemandOrder";
        public const string NoColumnName = "DemandOrderNo";
        public const string FormatNewNo = "DO{0}";

        #region New/Delete
        public static DemandOrder NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            DemandOrder entity = new DemandOrder();
            entity.DemandOrderID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDDemandOrderState = MDDemandOrderState.DefaultMDDemandOrderState(dbApp);
            entity.DemandOrderNo = secondaryKey;
            entity.DemandOrderName = "<TODO>";
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
                return DemandOrderNo + " " + DemandOrderName;
            }
        }

        /// <summary>
        /// Returns a related EF-Object which is in a Child-Relationship to this.
        /// THREAD-SAFE (QueryLock_1X000)
        /// </summary>
        /// <param name="className">Classname of the Table/EF-Object</param>
        /// <param name="filterValues">Search-Parameters</param>
        /// <returns>A Entity-Object as IACObject</returns>
        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && className == DemandOrderPos.ClassName)
            {
                Int32 sequence = 0;
                if (Int32.TryParse(filterValues[0], out sequence))
                    return this.DemandOrderPos_DemandOrder.Where(c => c.Sequence == sequence).FirstOrDefault();
            }
            return null;
        }

        #endregion

        #region IACObjectEntity Members
        /// <summary>
        /// Method for validating values and references in this EF-Object.
        /// Is called from Change-Tracking before changes will be saved for new unsaved entity-objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">Entity-Framework databasecontext</param>
        /// <returns>NULL if sucessful otherwise a Message-List</returns>
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            if (string.IsNullOrEmpty(DemandOrderNo))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "DemandOrderNo",
                    Message = "DemandOrderNo is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "DemandOrderNo"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }


        static public string KeyACIdentifier
        {
            get
            {
                return "DemandOrderNo";
            }
        }
        #endregion

        #region IEntityProperty Members

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
