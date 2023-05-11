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
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Unloadingpoint'}de{'Abladestelle'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Sequenz'}","", "", true)]
    [ACPropertyEntity(2, "UnloadingPointName", "en{'Unloadingpoint'}de{'Abladestelle'}","", "", true)]
    [ACPropertyEntity(3, CompanyAddress.ClassName, "en{'Company Address'}de{'Unternehmensadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, "Comment", "en{'Comment'}de{'Bemerkung'}","", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioCompany, Const.QueryPrefix + CompanyAddressUnloadingpoint.ClassName, "en{'Unloadingpoint'}de{'Abladestelle'}", typeof(CompanyAddressUnloadingpoint), CompanyAddressUnloadingpoint.ClassName, "UnloadingPointName", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<CompanyAddressUnloadingpoint>) })]
    public partial class CompanyAddressUnloadingpoint
    {
        [NotMapped]
        public const string ClassName = "CompanyAddressUnloadingpoint";

        #region New/Delete
        public static CompanyAddressUnloadingpoint NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            CompanyAddressUnloadingpoint entity = new CompanyAddressUnloadingpoint();
            entity.CompanyAddressUnloadingpointID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject != null && parentACObject is CompanyAddress)
            {
                CompanyAddress address = parentACObject as CompanyAddress;
                try
                {
                    if (!address.CompanyAddressUnloadingpoint_CompanyAddress_IsLoaded)
                        address.CompanyAddressUnloadingpoint_CompanyAddress.AutoLoad(address.CompanyAddressUnloadingpoint_CompanyAddressReference, address);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        if(Database.Root != null && Database.Root.Messages != null && Database.Root.InitState == ACInitState.Initialized)
                            Database.Root.Messages.LogException("CompanyAddressUnloadingpoint", Const.MN_NewACObject, msg);
                }

                if (address.CompanyAddressUnloadingpoint_CompanyAddress != null && address.CompanyAddressUnloadingpoint_CompanyAddress.Select(c => c.Sequence).Any())
                    entity.Sequence = address.CompanyAddressUnloadingpoint_CompanyAddress.Select(c => c.Sequence).Max() + 1;
                else
                    entity.Sequence = 1;

                entity.CompanyAddress = address;
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
            int sequence = Sequence;
            CompanyAddress address = CompanyAddress;
            database.Remove(this);
            CompanyAddressUnloadingpoint.RenumberSequence(address, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(CompanyAddress address, int sequence) 
        {
            if (   address == null
                || !address.CompanyAddressUnloadingpoint_CompanyAddress.Any())
                return;

            var elements = address.CompanyAddressUnloadingpoint_CompanyAddress.Where(c => c.Sequence > sequence).OrderBy(c => c.Sequence);
            int sequenceCount = sequence;
            foreach (var element in elements)
            {
                element.Sequence = sequence;
                sequence++;
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
                return CompanyAddress.ACCaption;
            }
        }

        /// <summary>
        /// Returns CompanyAddress
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to CompanyAddress</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return CompanyAddress;
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
            if (string.IsNullOrEmpty(UnloadingPointName))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "UnloadingPointName",
                    Message = "UnloadingPointName is null",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "UnloadingPointName"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "Sequence";
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
