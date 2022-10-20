using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Role'}de{'Rolle'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Sequenz'}", "", "", true)]
    [ACPropertyEntity(2, "UnloadingPointName", "en{'Unloadingpoint'}de{'Verladestelle'}", "", "", true)]
    [ACPropertyEntity(3, "CompanyDepartment", "en{'Department'}de{'Abteilung'}", Const.ContextDatabase + "\\CompanyDepartment", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioCompany, Const.QueryPrefix + CompanyPersonRole.ClassName, "en{'Role'}de{'Rolle'}", typeof(CompanyPersonRole), CompanyPersonRole.ClassName, "RoleACClass\\ACIdentifier", "RoleACClass\\ACIdentifier")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<CompanyPersonRole>) })]
    public partial class CompanyPersonRole
    {
        public const string ClassName = "CompanyPersonRole";

        #region New/Delete
        public static CompanyPersonRole NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {

            CompanyPersonRole entity = new CompanyPersonRole();
            entity.CompanyPersonRoleID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject != null && parentACObject is CompanyPerson)
            {
                CompanyPerson companyPerson = parentACObject as CompanyPerson;

                entity.CompanyPerson = companyPerson;
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        /// <summary>
        /// Deletes this entity-object from the database
        /// </summary>
        /// <param name="database">Entity-Framework databasecontext</param>
        /// <param name="withCheck">If set to true, a validation happens before deleting this EF-object. If Validation fails message ís returned.</param>
        /// <param name="softDelete">If set to true a delete-Flag is set in the dabase-table instead of a physical deletion. If  a delete-Flag doesn't exit in the table the record will be deleted.</param>
        /// <returns>If a validation or deletion failed a message is returned. NULL if sucessful.</returns>
        public override MsgWithDetails DeleteACObject(IACEntityObjectContext database, bool withCheck, bool softDelete = false)
        {
            if (withCheck)
            {
                MsgWithDetails msg = IsEnabledDeleteACObject(database);
                if (msg != null)
                    return msg;
            }
            CompanyPerson companyPerson = CompanyPerson;
            database.DeleteObject(this);
            return null;
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
                return RoleACClass.ACCaption;
            }
        }

        /// <summary>
        /// Returns CompanyPerson
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to CompanyPerson</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return CompanyPerson;
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
            if (RoleACClass == null)
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "RoleACClass",
                    Message = "RoleACClass is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "RoleACClass"), 
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
                return "CompanyPerson\\Name1,RoleACClass\\ACIdentifier";
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

        #region VBIplus-Context
                private gip.core.datamodel.ACClass _RoleACClass;
                [ACPropertyInfo(9999, "", "en{'Role'}de{'Rolle'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACClass.ClassName)]
                public gip.core.datamodel.ACClass RoleACClass
                {
                    get
                    {
                        if (this.VBiRoleACClassID == null || this.VBiRoleACClassID == Guid.Empty)
                            return null;
                        if (_RoleACClass != null)
                            return _RoleACClass;
                        if (this.VBiRoleACClass == null)
                        {
                            DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                            _RoleACClass = dbApp.ContextIPlus.ACClass.Where(c => c.ACClassID == this.VBiRoleACClassID).FirstOrDefault();
                            return _RoleACClass;
                        }
                        else
                        {
                            _RoleACClass = this.VBiRoleACClass.FromIPlusContext<gip.core.datamodel.ACClass>();
                            return _RoleACClass;
                        }
                    }
                    set
                    {
                        if (value == null)
                        {
                            if (this.VBiRoleACClass == null)
                                return;
                            _RoleACClass = null;
                            this.VBiRoleACClass = null;
                        }
                        else
                        {
                            if (_RoleACClass != null && value == _RoleACClass)
                                return;
                            gip.mes.datamodel.ACClass value2 = value.FromAppContext<gip.mes.datamodel.ACClass>(this.GetObjectContext<DatabaseApp>());
                            // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                            if (value2 == null)
                            {
                                this.VBiRoleACClassID = value.ACClassID;
                                throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                                //return;
                            }
                            _RoleACClass = value;
                            if (value2 == this.VBiRoleACClass)
                                return;
                            this.VBiRoleACClass = value2;
                        }
                    }
                }

                partial void OnVBiRoleACClassIDChanged()
                {
                    OnPropertyChanged("RoleACClass");
                }
        #endregion

    }
}
