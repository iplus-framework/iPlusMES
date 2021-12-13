using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Indeliverynotepos'}de{'Eingangslieferscheinposition'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Sequenz'}", "", "", true)]
    [ACPropertyEntity(2, OutOrderPos.ClassName, "en{'Outorderpos'}de{'Auftragsposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName, "", true)]
    [ACPropertyEntity(3, InOrderPos.ClassName, "en{'Inorderpos'}de{'Bestellposition'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName, "", true)]
    [ACPropertyEntity(4, "LineNumber", "en{'Item Number'}de{'Positionsnummer'}", "", "", true)]
    [ACPropertyEntity(5, DeliveryNote.ClassName, "en{'Deliverynote'}de{'Lieferschein'}", Const.ContextDatabase + "\\" + DeliveryNote.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + DeliveryNotePos.ClassName, "en{'Indeliverynotepos'}de{'Eingangslieferscheinposition'}", typeof(DeliveryNotePos), DeliveryNotePos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<DeliveryNotePos>) })]
    public partial class DeliveryNotePos
    {
        public const string ClassName = "DeliveryNotePos";

        #region New/Delete
        public static DeliveryNotePos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            DeliveryNotePos entity = new DeliveryNotePos();
            entity.DeliveryNotePosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is DeliveryNote)
            {
                DeliveryNote deliveryNote = parentACObject as DeliveryNote;
                if (deliveryNote.EntityState != System.Data.EntityState.Added)
                {
                    try
                    {
                        if (!deliveryNote.DeliveryNotePos_DeliveryNote.IsLoaded)
                            deliveryNote.DeliveryNotePos_DeliveryNote.Load();
                    }
                    catch (Exception ec)
                    {
                        string msg = ec.Message;
                        if (ec.InnerException != null && ec.InnerException.Message != null)
                            msg += " Inner:" + ec.InnerException.Message;

                        if (Database.Root != null && Database.Root.Messages != null)
                            Database.Root.Messages.LogException("DeliveryNotePos", Const.MN_NewACObject, msg);
                    }
                }
                if (deliveryNote.DeliveryNotePos_DeliveryNote != null && deliveryNote.DeliveryNotePos_DeliveryNote.Select(c => c.Sequence).Any())
                    entity.Sequence = deliveryNote.DeliveryNotePos_DeliveryNote.Select(c => c.Sequence).Max() + 1;
                else
                    entity.Sequence = 1;

                entity.DeliveryNote = deliveryNote;
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
            DeliveryNote inDeliveryNote = DeliveryNote;
            if (inDeliveryNote != null)
            {
                if (inDeliveryNote.DeliveryNotePos_DeliveryNote.IsLoaded)
                    inDeliveryNote.DeliveryNotePos_DeliveryNote.Remove(this);
            }
            database.DeleteObject(this);
            if (inDeliveryNote != null)
                DeliveryNotePos.RenumberSequence(inDeliveryNote, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(DeliveryNote inDeliveryNote, int sequence)
        {
            if (   inDeliveryNote == null
                || !inDeliveryNote.DeliveryNotePos_DeliveryNote.Any())
                return;

            var elements = from c in inDeliveryNote.DeliveryNotePos_DeliveryNote where c.Sequence > sequence orderby c.Sequence select c;
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
                if (Material == null)
                    return Sequence.ToString();
                return Sequence.ToString() + " DNP " + Material.ACCaption;
            }
        }

        /// <summary>
        /// Returns DeliveryNote
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to DeliveryNote</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return DeliveryNote;
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

        [ACPropertyInfo(3, "", "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName)]
        public Material Material
        {
            get
            {
                if (this.InOrderPos != null)
                    return InOrderPos.Material;
                else if (this.OutOrderPos != null)
                    return OutOrderPos.Material;
                return null;
            }
        }

    }
}
