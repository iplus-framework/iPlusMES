using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, ConstApp.DeliveryNotePos, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Sequenz'}", "", "", true)]
    [ACPropertyEntity(2, OutOrderPos.ClassName, "en{'Outorderpos'}de{'Auftragsposition'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, InOrderPos.ClassName, "en{'Inorderpos'}de{'Bestellposition'}", Const.ContextDatabase + "\\" + InOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(4, "LineNumber", "en{'Item Number'}de{'Positionsnummer'}", "", "", true)]
    [ACPropertyEntity(5, DeliveryNote.ClassName, "en{'Deliverynote'}de{'Lieferschein'}", Const.ContextDatabase + "\\" + DeliveryNote.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + DeliveryNotePos.ClassName, "en{'Indeliverynotepos'}de{'Eingangslieferscheinposition'}", typeof(DeliveryNotePos), DeliveryNotePos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<DeliveryNotePos>) })]
    [NotMapped]
    public partial class DeliveryNotePos
    {
        [NotMapped]
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
                if (deliveryNote.EntityState != EntityState.Added)
                {
                    try
                    {
                        if (!deliveryNote.DeliveryNotePos_DeliveryNote_IsLoaded)
                            deliveryNote.DeliveryNotePos_DeliveryNote.AutoLoad(deliveryNote.DeliveryNotePos_DeliveryNoteReference, deliveryNote);
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
            DeliveryNote inDeliveryNote = DeliveryNote;
            if (inDeliveryNote != null)
            {
                if (inDeliveryNote.DeliveryNotePos_DeliveryNote_IsLoaded)
                    inDeliveryNote.DeliveryNotePos_DeliveryNote.Remove(this);
            }
            base.DeleteACObject(database, withCheck, softDelete);
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

            var elements = inDeliveryNote.DeliveryNotePos_DeliveryNote.Where(c => c.Sequence > sequence).OrderBy(c => c.Sequence);
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
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return DeliveryNote;
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

        [ACPropertyInfo(3, "", "en{'Material'}de{'Material'}", Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix)]
        [NotMapped]
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

        public double? TargetQuantityUOM
        {
            get
            {
                if (this.InOrderPos != null)
                    return InOrderPos.TargetQuantityUOM;
                else if (this.OutOrderPos != null)
                    return OutOrderPos.TargetQuantityUOM;
                return null;
            }
        }

        public double? TargetQuantity
        {
            get
            {
                if (this.InOrderPos != null)
                    return InOrderPos.TargetQuantity;
                else if (this.OutOrderPos != null)
                    return OutOrderPos.TargetQuantity;
                return null;
            }
        }
    }
}
