using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions; using gip.core.datamodel;
using Microsoft.EntityFrameworkCore;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Inorderpossplit'}de{'Bestellpositionsplittung'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Folge'}","", "", true)]
    [ACPropertyEntity(2, "TargetQuantity", ConstApp.TargetQuantity,"", "", true)]
    [ACPropertyEntity(3, "TargetWeight", "en{'Target Weight'}de{'Zielgewicht'}","", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioPurchase, Const.QueryPrefix + InOrderPosSplit.ClassName, "en{'Inorderpossplit'}de{'Bestellpositionsplittung'}", typeof(InOrderPosSplit), InOrderPosSplit.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<InOrderPosSplit>) })]
    public partial class InOrderPosSplit
    {
        public const string ClassName = "InOrderPosSplit";

        #region New/Delete
        public static InOrderPosSplit NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            InOrderPosSplit entity = new InOrderPosSplit();
            entity.InOrderPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is InOrderPos)
            {
                InOrderPos inOrderPos = parentACObject as InOrderPos;
                try
                {
                    if (!inOrderPos.InOrderPosSplit_InOrderPos_IsLoaded)
                        inOrderPos.InOrderPosSplit_InOrderPos.AutoLoad(inOrderPos.InOrderPosSplit_InOrderPosReference, inOrderPos);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (Database.Root != null && Database.Root.Messages != null)
                        Database.Root.Messages.LogException("InOrderPosSplit", Const.MN_NewACObject, msg);
                }

                if (inOrderPos.InOrderPosSplit_InOrderPos != null && inOrderPos.InOrderPosSplit_InOrderPos.Select(c => c.Sequence).Any())
                    entity.Sequence = inOrderPos.InOrderPosSplit_InOrderPos.Select(c => c.Sequence).Max() + 1;
                else
                    entity.Sequence = 1;
                entity.InOrderPos = inOrderPos;
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
            InOrderPos inOrderPos = InOrderPos;
            database.Remove(this);
            InOrderPosSplit.RenumberSequence(inOrderPos, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(InOrderPos inOrderPos, int sequence)
        {
            if (inOrderPos == null
                || !inOrderPos.InOrderPosSplit_InOrderPos.Any())
                return;

            var elements = inOrderPos.InOrderPosSplit_InOrderPos.Where(c => c.Sequence > sequence).OrderBy(c => c.Sequence);
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
        /// Returns InOrderPos
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to InOrderPos</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return InOrderPos;
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
