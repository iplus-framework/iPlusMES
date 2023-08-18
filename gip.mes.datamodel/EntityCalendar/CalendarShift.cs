using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Transactions; using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioScheduling, "en{'Shift'}de{'Schicht'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(2, MDTimeRange.ClassName, "en{'Shift'}de{'Schicht'}", Const.ContextDatabase + "\\" + MDTimeRange.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, "TimeFrom", "en{'Begin of Shift'}de{'Schichtanfang'}","", "", true)]
    [ACPropertyEntity(4, "TimeTo", "en{'End of Shift'}de{'Schichtende'}","", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioScheduling, Const.QueryPrefix + CalendarShift.ClassName, "en{'Shift'}de{'Shift'}", typeof(CalendarShift), CalendarShift.ClassName, "MDTimeRange\\MDTimeRangeName", "MDTimeRange\\MDTimeRangeName", new object[]
        {
            new object[] {Const.QueryPrefix + CalendarShift.ClassName, "en{'Person'}de{'Person'}", typeof(CalendarShiftPerson), CalendarShiftPerson.ClassName, CompanyPerson.ClassName + "\\Name1", CompanyPerson.ClassName + "\\Name1"}
        }
    )]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<CalendarShift>) })]
    [NotMapped]
    public partial class CalendarShift
    {
        [NotMapped]
        public const string ClassName = "CalendarShift";

        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static CalendarShift NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            CalendarShift entity = new CalendarShift();
            entity.CalendarShiftID = Guid.NewGuid();
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
                return MDTimeRange.MDTimeRangeName;
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

        /// <summary>
        /// Returns a related EF-Object which is in a Child-Relationship to this.
        /// </summary>
        /// <param name="className">Classname of the Table/EF-Object</param>
        /// <param name="filterValues">Search-Parameters</param>
        /// <returns>A Entity-Object as IACObject</returns>
        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any() && className == CalendarShiftPerson.ClassName)
                return this.CalendarShiftPerson_CalendarShift.Where(c => c.CompanyPerson.Name1 == filterValues[0]).FirstOrDefault();
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
            if (this.MDTimeRange == null)
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = MDTimeRange.ClassName,
                    Message = "MDTimeRange is null",
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

        #region VBIplus-Context
        [NotMapped]
        private gip.core.datamodel.ACProject _ACProject;
        [ACPropertyInfo(9999, "", "en{'Project'}de{'Projekt'}", Const.ContextDatabaseIPlus + "\\" + gip.core.datamodel.ACProject.ClassName + Const.DBSetAsEnumerablePostfix)]
        [NotMapped]
        public gip.core.datamodel.ACProject ACProject
        {
            get
            {
                if (this.VBiACProjectID == Guid.Empty)
                    return null;
                if (_ACProject != null)
                    return _ACProject;
                if (this.VBiACProject == null)
                {
                    DatabaseApp dbApp = this.GetObjectContext<DatabaseApp>();
                    _ACProject = dbApp.ContextIPlus.ACProject.Where(c => c.ACProjectID == this.VBiACProjectID).FirstOrDefault();
                    return _ACProject;
                }
                else
                {
                    _ACProject = this.VBiACProject.FromIPlusContext<gip.core.datamodel.ACProject>();
                    return _ACProject;
                }
            }
            set
            {
                if (value == null)
                {
                    if (this.VBiACProject == null)
                        return;
                    _ACProject = null;
                    this.VBiACProject = null;
                }
                else
                {
                    if (_ACProject != null && value == _ACProject)
                        return;
                    gip.mes.datamodel.ACProject value2 = value.FromAppContext<gip.mes.datamodel.ACProject>(this.GetObjectContext<DatabaseApp>());
                    // Neu angelegtes Objekt, das im AppContext noch nicht existiert
                    if (value2 == null)
                    {
                        this.VBiACProjectID = value.ACProjectID;
                        throw new NullReferenceException("Value doesn't exist in Application-Context. Please save new value in iPlusContext before setting this property!");
                        //return;
                    }
                    _ACProject = value;
                    if (value2 == this.VBiACProject)
                        return;
                    this.VBiACProject = value2;
                }
            }
        }

        #endregion

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(VBiACProjectID))
            {
                base.OnPropertyChanged("ACProject");
            }
            base.OnPropertyChanged(propertyName);
        }

    }
}
