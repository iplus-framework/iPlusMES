using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data.Objects.DataClasses;
using System.Collections;
using System.Transactions; using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioScheduling, "en{'Shift'}de{'Schicht'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, CompanyPerson.ClassName, "en{'Person'}de{'Person'}", Const.ContextDatabase + "\\" + CompanyPerson.ClassName, "", true)]
    [ACPropertyEntity(2, "Percantage", "en{'Percent'}de{'Prozent'}","", "", true)]
    [ACPropertyEntity(3, "TimeFrom", "en{'Begin of Shift'}de{'Schichtanfang'}","", "", true)]
    [ACPropertyEntity(4, "TimeTo", "en{'End of Shift'}de{'Schichtende'}","", "", true)]
    [ACPropertyEntity(5, "ShiftStateIndex", "en{'Shiftstate'}de{'Schichtstatus'}", typeof(GlobalApp.ShiftStates), "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioScheduling, Const.QueryPrefix + CalendarShiftPerson.ClassName, "en{'Person'}de{'Person'}", typeof(CalendarShiftPerson), CalendarShiftPerson.ClassName, CompanyPerson.ClassName + "\\Name1", CompanyPerson.ClassName + "\\Name1")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<CalendarShiftPerson>) })]
    public partial class CalendarShiftPerson
    {
        public const string ClassName = "CalendarShiftPerson";

        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static CalendarShiftPerson NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            CalendarShiftPerson entity = new CalendarShiftPerson();
            entity.CalendarShiftPersonID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is CalendarShift)
            {
                CalendarShift calendarShift = parentACObject as CalendarShift;
                entity.CalendarShift = calendarShift;
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
        public override string ACCaption
        {
            get
            {
                return CompanyPerson.Name1;
            }
        }

        /// <summary>
        /// Returns CalendarShift
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to CalendarShift</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return CalendarShift;
            }
        }

        #endregion

        #region IACObjectEntity Members
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            if (this.CompanyPerson == null)
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = CompanyPerson.ClassName,
                    Message = "CompanyPerson is null",
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
                return "CompanyPerson\\Name1";
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

    }
}