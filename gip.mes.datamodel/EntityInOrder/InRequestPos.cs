using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioPurchase, "en{'Quotation Pos.'}de{'Anfrageposition'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Folge'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, "TargetQuantityUOM", ConstApp.TargetQuantityUOM, "", "", true)]
    [ACPropertyEntity(4, MDUnit.ClassName, ConstApp.MDUnit, Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(6, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(7, "TargetWeight", "en{'Target Weight'}de{'Zielgewicht'}", "", "", true)]
    [ACPropertyEntity(8, "TargetDeliveryDate", ConstApp.TargetDeliveryDate, "", "", true)]
    [ACPropertyEntity(9, "TargetDeliveryMaxDate", ConstApp.TargetDeliveryMaxDate, "", "", true)]
    [ACPropertyEntity(10, "TargetDeliveryPriority", "en{'Delivery Priority'}de{'Lieferpriorität'}", "", "", true)]
    [ACPropertyEntity(11, MDTimeRange.ClassName, ConstApp.ESTimeRange, Const.ContextDatabase + "\\" + MDTimeRange.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(12, "PriceNet", ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(13, "PriceGross", ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(14, MDCountrySalesTax.ClassName, "en{'Sales tax'}de{'Steuersatz'}", Const.ContextDatabase + "\\" + MDCountrySalesTax.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(15, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(16, "Comment2", "en{'Comment 2'}de{'Bemerkung 2'}", "", "", true)]
    [ACPropertyEntity(17, "LineNumber", "en{'Item Number'}de{'Positionsnummer'}", "", "", true)]
    [ACPropertyEntity(9999, "InRequest", "en{'Quotation'}de{'Anfrage'}", Const.ContextDatabase + "\\InRequest" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(9999, "MaterialPosTypeIndex", "en{'Positiontype'}de{'Posistionstyp'}", typeof(GlobalApp.MaterialPosTypes), "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioPurchase, Const.QueryPrefix + InRequestPos.ClassName, "en{'Quotation Pos.'}de{'Anfrageposition'}", typeof(InRequestPos), InRequestPos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<InRequestPos>) })]
    [NotMapped]
    public partial class InRequestPos
    {
        [NotMapped]
        public const string ClassName = "InRequestPos";

        #region New/Delete
        public static InRequestPos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            InRequestPos entity = new InRequestPos();
            entity.InRequestPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is InRequest)
            {
                InRequest inRequest = parentACObject as InRequest;
                try
                {
                    if (!inRequest.InRequestPos_InRequest_IsLoaded)
                        inRequest.InRequestPos_InRequest.AutoLoad(inRequest.InRequestPos_InRequestReference, inRequest);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (Database.Root != null && Database.Root.Messages != null)
                        Database.Root.Messages.LogException("InRequestPost", Const.MN_NewACObject, msg);
                }

                if (inRequest.InRequestPos_InRequest != null && inRequest.InRequestPos_InRequest.Select(c => c.Sequence).Any())
                    entity.Sequence = inRequest.InRequestPos_InRequest.Select(c => c.Sequence).Max() + 1;
                else
                    entity.Sequence = 1;
                entity.InRequest = inRequest;
            }
            entity.TargetQuantityUOM = 1;
            entity.MaterialPosTypeIndex = (Int16)GlobalApp.MaterialPosTypes.InwardRoot;
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
            InRequest inRequest = InRequest;
            base.DeleteACObject(database, withCheck, softDelete);
            InRequestPos.RenumberSequence(inRequest, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(InRequest inRequest, int sequence)
        {
            if (inRequest == null
                || !inRequest.InRequestPos_InRequest.Any())
                return;

            var elements = inRequest.InRequestPos_InRequest.Where(c => c.Sequence > sequence).OrderBy(c => c.Sequence);
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
                return Sequence.ToString() + " " + Material.ACCaption;
            }
        }

        /// <summary>
        /// Returns InRequest
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to InRequest</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return InRequest;
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

        [ACPropertyInfo(9999)]
        [NotMapped]
        public string Position
        {
            get
            {
                return "Position " + Sequence.ToString();
            }
        }
    }
}
