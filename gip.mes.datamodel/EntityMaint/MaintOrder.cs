using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance Order'}de{'Wartungsauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOMaintOrder")]

    [ACPropertyEntity(1, "MaintOrderNo", "en{'Maintorder-No.'}de{'Wartungauftrags-Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "BasedOnMaintOrder", "en{'BasedOnMaintOrder'}de{'BasedOnMaintOrder'}", Const.ContextDatabase + "\\" + MaintOrder.ClassName, "", true)]
    [ACPropertyEntity(3, "Facility", "en{'Facility'}de{'Facility'}", Const.ContextDatabase + "\\" + Facility.ClassName, "", true)]
    [ACPropertyEntity(4, "Picking", "en{'Picking'}de{'Picking'}", Const.ContextDatabase + "\\" + Picking.ClassName, "", true)]
    [ACPropertyEntity(5, "IsActive", "en{'Active'}de{'Aktiv'}", "", "", true)]
    [ACPropertyEntity(6, "MaintInterval", "en{'Maintenance interval'}de{'Wartungsintervall'}", "", "", true)]
    [ACPropertyEntity(7, "LastMaintTerm", "en{'Last Maintenance on'}de{'Letzte Wartung am'}", "", "", true)]
    [ACPropertyEntity(8, "WarningDiff", "en{'Advance Notice Days'}de{'Vorankündigung Tage'}", "", "", true)]
    [ACPropertyEntity(9, "PlannedStartDate", "en{'Planned start date'}de{'Geplantes Startdatum'}", "", "", true)]
    [ACPropertyEntity(10, "StartDate", "en{'Start date'}de{'Startdatum'}", "", "", true)]
    [ACPropertyEntity(11, "EndDate", "en{'Completed at'}de{'Fertiggestellt am'}", "", "", true)]
    [ACPropertyEntity(8, "MDMaintOrderState", "en{'Status'}de{'Status'}", Const.ContextDatabase + "\\MDMaintOrderState", "", true)]
    [ACPropertyEntity(13, "MaintACClass", "en{'Maintenance Objekt'}de{Wartungsobjekt'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioAutomation, Const.QueryPrefix + MaintOrder.ClassName, "en{'Maintenance Order'}de{'Wartungsauftrag'}", typeof(MaintOrder), MaintOrder.ClassName, "MaintOrderNo", "MaintOrderNo", new object[]
        {
            new object[] {Const.QueryPrefix + mes.datamodel.MaintOrderProperty.ClassName, "en{'Maint Order Properties'}de{'Wartungsauftrag Eigenschaften'}", typeof(MaintOrderProperty), mes.datamodel.MaintOrderProperty.ClassName + "_" + MaintOrder.ClassName, mes.datamodel.MaintOrderProperty.ClassName + "\\ACIdentifier", mes.datamodel.MaintOrderProperty.ClassName + "\\ACIdentifier"}
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<MaintOrder>) })]
    public partial class MaintOrder
    {
        public const string ClassName = nameof(MaintOrder);
        public const string NoColumnName = "MaintOrderNo";
        public const string FormatNewNo = "MO{0}";
        public const string FormatNewNoTemplate = "MOT{0}";

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
            //if (filterValues.Any() && className == MaintOrderProperty.ClassName)
            //    return this.MaintOrderProperty_MaintOrder.Where(c => c.MaintACClassProperty.ACIdentifier == filterValues[0]).FirstOrDefault();
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

        static public string KeyACIdentifier
        {
            get
            {
                return "MaintOrderNo";
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

        [ACPropertyInfo(9999)]
        public core.datamodel.ACClass TempACClass
        {
            get;
            set;
        }

        [ACPropertyInfo(999, "", "en{'Next Maintenance on'}de{'Nächste Wartung am'}")]
        public DateTime? NextMaintTerm
        {
            get
            {
                if (LastMaintTerm.HasValue && MaintInterval.HasValue)
                    return LastMaintTerm + TimeSpan.FromDays(MaintInterval.Value);
                return null;
            }
            set
            {
                //_NextMaintTerm = value;
                OnPropertyChanged(nameof(NextMaintTerm));
            }
        }

        [ACPropertyInfo(9999)]
        public string ComponentACCaption
        {
            get
            {
                if (Facility != null)
                {
                    return Facility.FacilityNo + Environment.NewLine + Facility.FacilityName;
                }
                else if (MaintACClass != null)
                {
                    return MaintACClass.ACClass.ACCaption + Environment.NewLine + MaintACClass.ACClassACUrl;
                }

                return null;
            }
        }

        //private TimeSpan _MaintActDurationTS;
        //[ACPropertyInfo(999, "", "en{'Duration'}de{'Dauer'}")]
        //public TimeSpan MaintActDurationTS
        //{
        //    get
        //    {
        //        if (MaintActDuration > 0)
        //            _MaintActDurationTS = TimeSpan.FromMinutes(MaintActDuration);
        //        else
        //            _MaintActDurationTS = new TimeSpan();
        //        return _MaintActDurationTS;
        //    }
        //    set
        //    {
        //        _MaintActDurationTS = value;
        //        MaintActDuration = (int)_MaintActDurationTS.TotalMinutes;
        //        OnPropertyChanged("MaintActDurationTS");
        //    }
        //}

        //[ACPropertyInfo(999, "", "en{'Url of the Objekt'}de{'Url des Objekts'}")]
        //public string ComponentACUrl
        //{
        //    get
        //    {
        //        return VBiPAACClass.FromIPlusContext<core.datamodel.ACClass>().GetACUrlComponent();
        //    }
        //}

        #endregion
    }
}
