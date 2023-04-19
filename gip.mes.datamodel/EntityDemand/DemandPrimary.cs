using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'DemandPrimary'}de{'DemandPrimary'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSODemandPrimary")]
    [ACPropertyEntity(1, "DemandPrimaryNo", "en{'DemandPrimaryNo'}de{'Tourenplan-Nr.'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + DemandPrimary.ClassName, "en{'DemandPrimary'}de{'DemandPrimary'}", typeof(DemandPrimary), DemandPrimary.ClassName, "DemandPrimaryNo", "DemandPrimaryNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<DemandPrimary>) })]
    public partial class DemandPrimary
    {
        public const string ClassName = "DemandPrimary";
        public const string NoColumnName = "DemandPrimaryNo";
        public const string FormatNewNo = "DP{0}";

        #region New/Delete
        public static DemandPrimary NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            DemandPrimary entity = new DemandPrimary();
            entity.DemandPrimaryID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.DemandPrimaryNo = secondaryKey;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        public override string ACCaption
        {
            get
            {
                return DemandPrimaryNo;
            }
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
            if (Material == null)
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = Material.ClassName,
                    Message = "Material is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", Material.ClassName), 
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
                return "DemandPrimaryNo";
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
