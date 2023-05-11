using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Transactions; 
using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioScheduling, "en{'Holiday'}de{'Feiertag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(2, MDTimeRange.ClassName, "en{'Shift'}de{'Schicht'}", Const.ContextDatabase + "\\" + MDTimeRange.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioScheduling, Const.QueryPrefix + CalendarHoliday.ClassName, "en{'Holiday'}de{'Feiertag'}", typeof(CalendarHoliday), CalendarHoliday.ClassName, "HolidayName", "HolidayName")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<CalendarHoliday>) })]
    public partial class CalendarHoliday
    {
        [NotMapped]
        public const string ClassName = "CalendarHoliday";
        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static CalendarHoliday NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            CalendarHoliday entity = new CalendarHoliday();
            entity.CalendarHolidayID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Calendar)
            {
                Calendar calendar = parentACObject as Calendar;
                entity.Calendar = calendar;
            }
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
                return HolidayName;
            }
        }

        /// <summary>
        /// Returns Calendar
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Calendar</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return Calendar;
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
            if (string.IsNullOrEmpty(HolidayName))
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "HolidayName",
                    Message = "HolidayName is null",
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
                return "HolidayName";
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

        #region VBIplus-Context
        //private gip.core.datamodel.ACProject _ACProject;
        //[ACPropertyInfo(9999, "", "en{'Project'}de{'Projekt'}", Const.ContextDatabaseIPlus + "\\" + ACProject.ClassName)]
        //public gip.core.datamodel.ACProject ACProject
        //{
        //    get
        //    {
        //        if (this.VBiACProjectID == Guid.Empty)
        //            return null;
        //        if (_ACProject != null)
        //            return _ACProject;
        //        if (this.VBiACProject == null)
        //        {
        //            DatabaseApp database = this.GetObjectContext<DatabaseApp>();
        //            _ACProject = database.ContextIPlus.ACProject.Where(c => c.ACProjectID == this.VBiACProjectID).FirstOrDefault();
        //            return _ACProject;
        //        }
        //        else
        //        {
        //            _ACProject = this.VBiACProject.FromIPlusContext<gip.core.datamodel.ACProject>();
        //            return _ACProject;
        //        }
        //    }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            if (this.VBiACProject == null)
        //                return;
        //            _ACProject = null;
        //            this.VBiACProject = null;
        //        }
        //        else
        //        {
        //            if (_ACProject != null && value == _ACProject)
        //                return;
        //            gip.mes.datamodel.ACProject value2 = value.FromAppContext<gip.mes.datamodel.ACProject>(this.GetObjectContext<DatabaseApp>());
        //            // Neu angelegtes Objekt, das im AppContext noch nicht existiert
        //            if (value2 == null)
        //            {
        //                this.VBiACProjectID = value.ACProjectID;
        //                throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
        //                //return;
        //            }
        //            _ACProject = value;
        //            if (value2 == this.VBiACProject)
        //                return;
        //            this.VBiACProject = value2;
        //        }
        //    }
        //}

        //partial void OnACVBiProjectIDChanged()
        //{
        //    OnPropertyChanged("ACProject");
        //}
#endregion
    }
}
