using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Transactions; 
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;
using static Microsoft.Isam.Esent.Interop.EnumeratedColumn;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioScheduling, "en{'Calendar'}de{'Kalender'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOCalendar")]
    [ACPropertyEntity(1, "CalendarDate", "en{'Date'}de{'Datum'}","", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioScheduling, Const.QueryPrefix + Calendar.ClassName, "en{'Calendar'}de{'Kalender'}", typeof(Calendar), Calendar.ClassName, "CalendarDate", "CalendarDate", new object[]
        {
            new object[] {Const.QueryPrefix + CalendarShift.ClassName, "en{'Shift'}de{'Schicht'}", typeof(CalendarShift), CalendarShift.ClassName, "MDTimeRange\\MDTimeRangeName", "MDTimeRange\\MDTimeRangeName", new object[]
                {
                    new object[] { Const.QueryPrefix + CalendarShiftPerson.ClassName, "en{'Person'}de{'Person'}", typeof(CalendarShiftPerson), CalendarShiftPerson.ClassName, CompanyPerson.ClassName + "\\Name1", CompanyPerson.ClassName + "\\Name1"}
                
                }
            },
            new object[] { Const.QueryPrefix + CalendarHoliday.ClassName, "en{'Holiday'}de{'Feiertag'}", typeof(CalendarHoliday), "CalendarHoliday", "HolidayName", "HolidayName"}
        })
    ]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<Calendar>) })]
    [NotMapped]
    public partial class Calendar
    {
        [NotMapped]
        public const string ClassName = "Calendar";
        #region New/Delete
        public static Calendar NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            Calendar entity = new Calendar();
            entity.CalendarID = Guid.NewGuid();
            entity.DefaultValuesACObject();
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
                return CalendarDate.ToString();
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
            if (filterValues.Any())
            {
                switch (className)
                {
                    case CalendarShift.ClassName:
                        return this.CalendarShift_Calendar.Where(c => c.MDTimeRange.MDTimeRangeName == filterValues[0]).FirstOrDefault();
                    case CalendarHoliday.ClassName:
                        return this.CalendarHoliday_Calendar.Where(c => c.HolidayName == filterValues[0]).FirstOrDefault();
                }
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
            if (CalendarDate == DateOnly.MinValue)
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "CalendarDate",
                    Message = "CalendarDate is null",
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
                return "MDTimeRange\\MDTimeRangeName";
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
