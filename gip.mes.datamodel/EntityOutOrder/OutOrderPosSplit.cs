using gip.core.datamodel;
using System;
using System.Linq;

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
    public partial class OutOrderPosSplit
    {
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
                    if (!outOrderPos.OutOrderPosSplit_OutOrderPos.IsLoaded)
                        outOrderPos.OutOrderPosSplit_OutOrderPos.Load();
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
            entity.SetInsertAndUpdateInfo(Database.Initials, dbApp);
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
            database.DeleteObject(this);
            OutOrderPosSplit.RenumberSequence(outOrderPos, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(OutOrderPos outOrderPos, int sequence)
        {
            var elements = from c in outOrderPos.OutOrderPosSplit_OutOrderPos where c.Sequence > sequence orderby c.Sequence select c;
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
        public override IACObject ParentACObject
        {
            get
            {
                return OutOrderPos;
            }
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "Sequence";
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
