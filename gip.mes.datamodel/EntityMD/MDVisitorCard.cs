using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.ESVisitorCard, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "MDBSOVisitorCard")]
    [ACPropertyEntity(1, "MDVisitorCardNo", "en{'Card Number'}de{'Ausweisnummer'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, "MDVisitorCardKey", "en{'Card Key'}de{'Ausweisschlüssel'}", "", "", true)]
    [ACPropertyEntity(3, Const.IsDefault, Const.EntityIsDefault, "", "", true)]
    [ACPropertyEntity(4, "MDVisitorCardState", "en{'Card State'}de{'Ausweisstatus'}", Const.ContextDatabase + "\\MDVisitorCardState", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + MDVisitorCard.ClassName, ConstApp.ESVisitorCard, typeof(MDVisitorCard), MDVisitorCard.ClassName, "MDVisitorCardNo", "MDVisitorCardNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MDVisitorCard>) })]
    public partial class MDVisitorCard
    {
        public const string ClassName = "MDVisitorCard";

        #region New/Delete
        public static MDVisitorCard NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            MDVisitorCard entity = new MDVisitorCard();
            entity.MDVisitorCardID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDVisitorCardState = MDVisitorCardState.DefaultMDVisitorCardState(dbApp);
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
                return MDVisitorCardNo;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "MDVisitorCardNo";
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

        #region AdditionalProperties

        [ACPropertyInfo(9999, "", "en{'Assigned Visitor'}de{'Zugeordneter Besucher'}")]
        public Visitor AssignedVisitor
        {
            get
            {
                if (!this.Visitor_MDVisitorCard.Any())
                    return null;
                return this.Visitor_MDVisitorCard.First();
            }
        }
#endregion
    }
}




