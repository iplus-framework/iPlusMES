using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioMaterial, "en{'Lab Order Position'}de{'Laborauftrag Position'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Reihenfolge'}", "", "", true)]
    [ACPropertyEntity(2, "ReferenceValue", "en{'Reference Value'}de{'Referenzwert'}", "", "", true)]
    [ACPropertyEntity(3, "ActualValue", "en{'Actual Value'}de{'Aktueller Wert'}", "", "", true)]
    [ACPropertyEntity(11, "MDLabTag", "en{'Laboratory Tag'}de{'Laborkennzeichen'}", Const.ContextDatabase + "\\MDLabTag" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, "MDLabOrderPosState", "en{'Lab Order Pos. Status'}de{'Laborauftrag Position Status'}", Const.ContextDatabase + "\\MDLabOrderPosState" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(5, "Comment", "en{'Comment'}de{'Bemerkung'}", "", "", true)]
    [ACPropertyEntity(6, "LineNumber", "en{'Item Number'}de{'Positionsnummer'}", "", "", true)]
    [ACPropertyEntity(7, "ValueMinMin", "en{'Lowest value for alarm'}de{'Unterer Alarmgrenzwert'}", "", "", true)]
    [ACPropertyEntity(8, "ValueMin", "en{'Lowest value'}de{'Unterer Grenzwert'}", "", "", true)]
    [ACPropertyEntity(9, "ValueMax", "en{'Maximum value'}de{'Oberer Grenzwert'}", "", "", true)]
    [ACPropertyEntity(10, "ValueMaxMax", "en{'Maximum value for alarm'}de{'Oberer Alarmgrenzwert'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioMaterial, Const.QueryPrefix + LabOrderPos.ClassName, "en{'Lab Order Position'}de{'Laborauftrag Position'}", typeof(LabOrderPos), LabOrderPos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<LabOrderPos>) })]
    [NotMapped]
    public partial class LabOrderPos
    {
        [NotMapped]
        public const string ClassName = "LabOrderPos";

        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static LabOrderPos NewACObject(DatabaseApp dbApp, IACObject parentACObject, IACObject parentPosition = null)
        {
            LabOrderPos entity = new LabOrderPos();
            entity.LabOrderPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is LabOrder)
            {
                LabOrder labReport = parentACObject as LabOrder;
                if (labReport != null)
                {
                    if (labReport.LabOrderPos_LabOrder != null && labReport.LabOrderPos_LabOrder.Select(c => c.Sequence).Any())
                        entity.Sequence = labReport.LabOrderPos_LabOrder.Select(c => c.Sequence).Max() + 1;
                    else
                        entity.Sequence = 1;
                }
                if (parentPosition is LabOrderPos)
                {
                    LabOrderPos template = parentPosition as LabOrderPos;
                    entity.MDLabTag = template.MDLabTag;
                    entity.ValueMin = template.ValueMin;
                    entity.ValueMax = template.ValueMax;
                    entity.ValueMinMin = template.ValueMinMin;
                    entity.ValueMaxMax = template.ValueMaxMax;
                    entity.Sequence = template.Sequence;
                    entity.LineNumber = template.LineNumber;
                    entity.XMLConfig = template.XMLConfig;
                }
                entity.LabOrder = labReport;
            }
            entity.MDLabOrderPosState = MDLabOrderPosState.DefaultMDLabOrderPosState(dbApp);
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        public MsgWithDetails DeleteACObject(IACEntityObjectContext database, bool withCheck)
        {
            if (withCheck)
            {
                MsgWithDetails msg = IsEnabledDeleteACObject(database);
                if (msg != null)
                    return msg;
            }
            int sequence = Sequence;
            LabOrder labReport = LabOrder;
            database.Remove(this);
            LabOrderPos.RenumberSequence(labReport, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(LabOrder labReport, int sequence)
        {
            if (labReport == null
                || !labReport.LabOrderPos_LabOrder.Any())
                return;

            var elements = labReport.LabOrderPos_LabOrder.Where(c => c.Sequence > sequence).OrderBy(c => c.Sequence);
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
                return Sequence.ToString();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Returns LabOrder
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to LabOrder</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return LabOrder;
            }
        }

        #endregion

        #region IACObjectEntity Members
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

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(ValueMax):
                    base.OnPropertyChanged("ExceedState");
                    break;
                case nameof(ValueMaxMax):
                    base.OnPropertyChanged("ExceedState");
                    break;
                case nameof(ValueMin):
                    base.OnPropertyChanged("ExceedState");
                    break;
                case nameof(ValueMinMin):
                    base.OnPropertyChanged("ExceedState");
                    break;
                case nameof(ActualValue):
                    base.OnPropertyChanged("ExceedState");
                    break;
            }
            base.OnPropertyChanged(propertyName);
        }

        #region Exceed
        public enum ExceedStates : short
        {
            Ok = 0,
            WarningMin = 1,
            WarningMax = 2,
            ErrorMin = 3,
            ErrorMax = 4
        }

        [ACPropertyInfo(1000, "", "en{'Exceedstate'}de{'Überschreitungsstatus'}")]
        [NotMapped]
        public ExceedStates ExceedState
        {
            get
            {
                if (!ActualValue.HasValue)
                    return ExceedStates.Ok;
                if (ValueMinMin.HasValue && ActualValue < ValueMinMin)
                    return ExceedStates.ErrorMin;
                else if (ValueMin.HasValue && ActualValue < ValueMin)
                    return ExceedStates.WarningMin;
                else if (ValueMaxMax.HasValue && ActualValue > ValueMaxMax)
                    return ExceedStates.ErrorMax;
                if (ValueMaxMax.HasValue && ActualValue > ValueMax)
                    return ExceedStates.WarningMax;
                return ExceedStates.Ok;
            }
        }
        #endregion
    }
}
