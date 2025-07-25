using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Maintenance Order'}de{'Wartungsauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOMaintOrder")]

    [ACPropertyEntity(1, "MaintOrderNo", "en{'Maintorder-No.'}de{'Wartungauftrags-Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "StartDate", "en{'Start date'}de{'Startdatum'}", "", "", true)]
    [ACPropertyEntity(3, "EndDate", "en{'Completed at'}de{'Fertiggestellt am'}", "", "", true)]
    [ACPropertyEntity(4, "PlannedDuration", "en{'Planned duration'}de{'Geplante Dauer'}", "", "", true)]
    [ACPropertyEntity(5, "MaintACClass", "en{'Maintenance Objekt'}de{Wartungsobjekt'}", "", "", true)]
    [ACPropertyEntity(6, "MDMaintOrderState", "en{'Status'}de{'Status'}", Const.ContextDatabase + "\\" + MDMaintOrderState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(7, "IsActive", "en{'Active'}de{'Aktiv'}", "", "", true)]
    [ACPropertyEntity(8, "MaintInterval", "en{'Maintenance interval [days]'}de{'Wartungsintervall [Tage]'}", "", "", true)]
    [ACPropertyEntity(9, "LastMaintTerm", "en{'Last Maintenance on'}de{'Letzte Wartung am'}", "", "", true)]
    [ACPropertyEntity(10, "WarningDiff", "en{'Advance Notice Days'}de{'Vorank�ndigung Tage'}", "", "", true)]
    [ACPropertyEntity(11, "PlannedStartDate", "en{'Planned start date'}de{'Geplantes Startdatum'}", "", "", true)]
    [ACPropertyEntity(12, "MaintOrder1_BasedOnMaintOrder", "en{'BasedOnMaintOrder'}de{'BasedOnMaintOrder'}", Const.ContextDatabase + "\\" + MaintOrder.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(13, "Facility", "en{'Facility'}de{'Facility'}", Const.ContextDatabase + "\\" + Facility.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(14, "Picking", "en{'Picking'}de{'Picking'}", Const.ContextDatabase + "\\" + Picking.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
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
    [NotMapped]
    public partial class MaintOrder
    {
        [NotMapped]
        public const string ClassName = nameof(MaintOrder);
        [NotMapped]
        public const string NoColumnName = "MaintOrderNo";
        [NotMapped]
        public const string FormatNewNo = "MO{0}";
        [NotMapped]
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

        //[ACPropertyInfo(9999)]
        //public core.datamodel.ACClass TempACClass
        //{
        //    get;
        //    set;
        //}

        [NotMapped]
        [ACPropertyInfo(999, "", "en{'Next Maintenance on'}de{'N�chste Wartung am'}")]
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

        [NotMapped]
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

        [NotMapped]
        [ACPropertyInfo(999, "", "en{'Duration'}de{'Dauer'}")]
        public TimeSpan MaintOrderDuration
        {
            get
            {
                if (StartDate.HasValue && EndDate.HasValue && EndDate.Value > StartDate.Value)
                {
                    return EndDate.Value - StartDate.Value;
                }
                return TimeSpan.Zero;
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
