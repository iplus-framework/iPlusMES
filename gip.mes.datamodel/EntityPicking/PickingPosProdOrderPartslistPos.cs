using gip.core.datamodel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{

    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Picking Order'}de{'Kommissionierauftrag'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOPicking")]
    [ACPropertyEntity(1, "PickingPos", "en{'PickingPos'}de{'PickingPos'}", Const.ContextDatabase + "\\" + PickingPos.ClassName, "", true)]
    [ACPropertyEntity(2, "ProdOrderPartslistPos", "en{'ProdOrderPartslistPos'}de{'ProdOrderPartslistPos'}", Const.ContextDatabase + "\\" + ProdOrderPartslistPos.ClassName, "", true)]
    public partial class PickingPosProdOrderPartslistPos
    {
        [NotMapped]
        public const string ClassName = "PickingPosProdOrderPartslistPos";

        #region New/Delete
        public static PickingPosProdOrderPartslistPos NewACObject(DatabaseApp dbApp, PickingPos pickingPos, ProdOrderPartslistPos pos)
        {
            PickingPosProdOrderPartslistPos entity = new PickingPosProdOrderPartslistPos();
            entity.PickingPosProdOrderPartslistPosID = Guid.NewGuid();
            entity.PickingPos = pickingPos;
            entity.ProdorderPartslistPos = pos;
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
            database.Remove(this);
            return null;
        }

       

        #endregion
    }
}
