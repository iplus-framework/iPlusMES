using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance Order'}de{'Wartungsauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOMaintOrder")]

    [ACPropertyEntity(1, "MaintOrderNo", "en{'Maintorder-No.'}de{'Wartungauftrags-Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "MaintSetDate", "en{'Order Date'}de{'Auftragsdatum'}", "", "", true)]
    [ACPropertyEntity(3, "MaintSetDuration", "en{'Maintenance Interval'}de{'Wartungsintervall'}", "", "", true)]
    [ACPropertyEntity(4, "Comment", "en{'Comment'}de{'Bemerkung'}", "", "", true)]
    [ACPropertyEntity(5, "MaintActDuration", "en{'Duration'}de{'Dauer'}", "", "", true)]
    [ACPropertyEntity(6, "MaintActStartDate", "en{'Start of Maintenance'}de{'Wartungsbeginn'}", "", "", true)]
    [ACPropertyEntity(7, "MaintActEndDate", "en{'Completed at'}de{'Fertiggestellt am'}", "", "", true)]
    [ACPropertyEntity(8, "MDMaintOrderState", "en{'Status'}de{'Status'}", Const.ContextDatabase + "\\MDMaintOrderState" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9, "MDMaintMode", "en{'Maintenance Mode'}de{'Wartungsmodus'}", Const.ContextDatabase + "\\MDMaintMode" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(10, "VBiPAACClass", "en{'Maintenance Objekt'}de{'Wartungsobjekt'}")]
    [ACPropertyEntity(13, "MaintACClass", "en{'Maintenance Objekt'}de{Wartungsobjekt'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MaintOrder.ClassName, "en{'Maintenance Order'}de{'Wartungsauftrag'}", typeof(MaintOrder), MaintOrder.ClassName, "MaintOrderNo", "MaintOrderNo", new object[]
        {
            new object[] {Const.QueryPrefix + MaintOrderProperty.ClassName, "en{'Maint Order Properties'}de{'Wartungsauftrag Eigenschaften'}", typeof(MaintOrderProperty), MaintOrderProperty.ClassName + "_" + MaintOrder.ClassName, MaintOrderProperty.ClassName + "\\ACIdentifier", MaintOrderProperty.ClassName + "\\ACIdentifier"}
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaintOrder>) })]
    public partial class MaintOrder
    {
        public const string ClassName = "MaintOrder";
        public const string NoColumnName = "MaintOrderNo";
        public const string FormatNewNo = "MO{0}";

        #region New/Delete
        public static MaintOrder NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            MaintOrder entity = new MaintOrder();
            entity.MaintOrderID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MaintOrderNo = secondaryKey;
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
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return MaintOrderNo;
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
            if (filterValues.Any() && className == MaintOrderProperty.ClassName)
                return this.MaintOrderProperty_MaintOrder.Where(c => c.MaintACClassProperty.ACIdentifier == filterValues[0]).FirstOrDefault();
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
            if (string.IsNullOrEmpty(MaintOrderNo))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "MaintOrderNo",
                    Message = "MaintOrderNo",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "MaintOrderNo"), 
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
                return "MaintOrderNo";
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

        #region AdditionalProperties

        [NotMapped]
        private TimeSpan _MaintActDurationTS;
        [ACPropertyInfo(999, "", "en{'Duration'}de{'Dauer'}")]
        [NotMapped]
        public TimeSpan MaintActDurationTS
        {
            get
            {
                if (MaintActDuration > 0)
                    _MaintActDurationTS = TimeSpan.FromMinutes(MaintActDuration);
                else
                    _MaintActDurationTS = new TimeSpan();
                return _MaintActDurationTS;
            }
            set
            {
                _MaintActDurationTS = value;
                MaintActDuration = (int)_MaintActDurationTS.TotalMinutes;
                OnPropertyChanged("MaintActDurationTS");
            }
        }

        [ACPropertyInfo(999, "", "en{'Url of the Objekt'}de{'Url des Objekts'}")]
        [NotMapped]
        public string ComponentACUrl
        {
            get
            {
                return VBiPAACClass.FromIPlusContext<core.datamodel.ACClass>().GetACUrlComponent();
            }
        }

        #endregion
    }
}
