using gip.core.datamodel;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Inorderpossplit'}de{'Bestellpositionsplittung'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Folge'}", "", "", true)]
    [ACPropertyEntity(2, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(3, "TargetWeight", "en{'Target Weight'}de{'Zielgewicht'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioPurchase, Const.QueryPrefix + OutOrderPosSplit.ClassName, "en{'Inorderpossplit'}de{'Bestellpositionsplittung'}", typeof(OutOrderPosSplit), OutOrderPosSplit.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<OutOrderPosSplit>) })]
    [NotMapped]
    public partial class OutOrderPosSplit
    {
        [NotMapped]
        public const string ClassName = "OutOrderPosSplit";

        #region New/Delete
        public static OutOrderPosSplit NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            OutOrderPosSplit entity = new OutOrderPosSplit();
            entity.OutOrderPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is OutOrderPos)
            {
                OutOrderPos outOrderPos = parentACObject as OutOrderPos;
                try
                {
                    if (!outOrderPos.OutOrderPosSplit_OutOrderPos_IsLoaded)
                        outOrderPos.OutOrderPosSplit_OutOrderPos.AutoLoad(outOrderPos.OutOrderPosSplit_OutOrderPosReference, outOrderPos);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (Database.Root != null && Database.Root.Messages != null)
                        Database.Root.Messages.LogException(ClassName, Const.MN_NewACObject, msg);
                }

                if (outOrderPos.OutOrderPosSplit_OutOrderPos != null && outOrderPos.OutOrderPosSplit_OutOrderPos.Select(c => c.Sequence).Any())
                    entity.Sequence = outOrderPos.OutOrderPosSplit_OutOrderPos.Select(c => c.Sequence).Max() + 1;
                else
                    entity.Sequence = 1;
                entity.OutOrderPos = outOrderPos;
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
            OutOrderPos outOrderPos = OutOrderPos;
            database.Remove(this);
            OutOrderPosSplit.RenumberSequence(outOrderPos, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(OutOrderPos outOrderPos, int sequence)
        {
            if (outOrderPos == null
                || !outOrderPos.OutOrderPosSplit_OutOrderPos.Any())
                return;

            var elements = outOrderPos.OutOrderPosSplit_OutOrderPos.Where(c => c.Sequence > sequence).OrderBy(c => c.Sequence);
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
        }

        /// <summary>
        /// Returns OutOrderPos
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to OutOrderPos</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return OutOrderPos;
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
    }
}
