using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Person'}de{'Person'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "CompanyPersonNo", "en{'Person No.'}de{'Personen-Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "Name1", "en{'Name 1'}de{'Name 1'}", "", "", true)]
    [ACPropertyEntity(3, "Name2", "en{'Name 2'}de{'Name 2'}", "", "", true)]
    [ACPropertyEntity(4, "Name3", "en{'Name 3'}de{'Name 3'}", "", "", true)]
    [ACPropertyEntity(5, "Street", "en{'Street'}de{'Straße'}", "", "", true)]
    [ACPropertyEntity(6, "City", "en{'City'}de{'Stadt'}", "", "", true)]
    [ACPropertyEntity(7, MDCountry.ClassName, "en{'Country'}de{'Land'}", Const.ContextDatabase + "\\" + MDCountry.ClassName, "", true)]
    [ACPropertyEntity(8, "Fax", "en{'Fax'}de{'Fax'}", "", "", true)]
    [ACPropertyEntity(9, "Mobile", "en{'Mobile'}de{'Handynummer'}", "", "", true)]
    [ACPropertyEntity(10, "Phone", "en{'Phone'}de{'Telefon'}", "", "", true)]
    [ACPropertyEntity(11, "Postcode", "en{'ZIP-Code'}de{'PLZ'}", "", "", true)]
    [ACPropertyEntity(12, "PostOfficeBox", "en{'Post Office Box'}de{'Postfach'}", "", "", true)]
    [ACPropertyEntity(13, MDTimeRange.ClassName, "en{'Shiftmodel'}de{'Schichtmodell'}", Const.ContextDatabase + "\\" + MDTimeRange.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioCompany, Const.QueryPrefix + CompanyPerson.ClassName, "en{'Person'}de{'Person'}", typeof(CompanyPerson), CompanyPerson.ClassName, "Name1", "Name1")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<CompanyPerson>) })]
    public partial class CompanyPerson
    {
        public const string ClassName = "CompanyPerson";
        public const string NoColumnName = "CompanyPersonNo";
        public const string FormatNewNo = "CP{0}";
        #region New/Delete
        public static CompanyPerson NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            CompanyPerson entity = new CompanyPerson();
            entity.CompanyPersonID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Company)
            {
                entity.Company = parentACObject as Company;
            }
            entity.CompanyPersonNo = secondaryKey;
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
                return Name1;
            }
        }

        /// <summary>
        /// Returns Company
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Company</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return Company;
            }
        }

        /// <summary>
        /// Returns a related EF-Object which is in a Child-Relationship to this.
        /// </summary>
        /// <param name="className">Classname of the Table/EF-Object</param>
        /// <param name="filterValues">Search-Parameters</param>
        /// <returns>A Entity-Object as IACObject</returns>
        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && className == CompanyPersonRole.ClassName)
                return this.CompanyPersonRole_CompanyPerson.Where(c => c.RoleACClass.ACIdentifier == filterValues[0]).FirstOrDefault();
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
            if (string.IsNullOrEmpty(Name1))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "Name1",
                    Message = "Name1 is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", MDUnit.ClassName), 
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
                return "Name1";
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
