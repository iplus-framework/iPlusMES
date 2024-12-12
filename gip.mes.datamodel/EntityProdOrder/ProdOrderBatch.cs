using System;
using System.ComponentModel.DataAnnotations.Schema;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Batch'}de{'Charge'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOProdOrder")]
    [ACPropertyEntity(1, "ProdOrderBatchNo", "en{'Batch-No.'}de{'Batch-Nr.'}", "", "", true)]
    [ACPropertyEntity(2, "BatchSeqNo", "en{'Sequence'}de{'Reihenfolge'}", "", "", true)]
    [ACPropertyEntity(5, MDProdOrderState.ClassName, "en{'Production Status'}de{'Produktionsstatus'}", Const.ContextDatabase + "\\" + MDProdOrderState.ClassName, "", true)]
    [ACPropertyEntity(5, MDProdOrderState.ClassName, "en{'Production Status'}de{'Produktionsstatus'}", Const.ContextDatabase + "\\" + MDProdOrderState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(6, ProdOrderBatchPlan.ClassName, "en{'Batch plan'}de{'Batchplan'}", Const.ContextDatabase + "\\" + ProdOrderBatchPlan.ClassName, "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]

    [ACQueryInfoPrimary(Const.PackName_VarioManufacturing, Const.QueryPrefix + ProdOrderBatch.ClassName, "en{'Batch'}de{'Batch'}", typeof(ProdOrderBatch), ProdOrderBatch.ClassName, "ProdOrderBatchNo", "ProdOrderBatchNo")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<ProdOrderBatch>) })]
    [NotMapped]
    public partial class ProdOrderBatch : IACObjectEntity
    {
        [NotMapped]
        public const string ClassName = "ProdOrderBatch";
        [NotMapped]
        public const string NoColumnName = "ProdOrderBatchNo";
        [NotMapped]
        public const string FormatNewNo = "PB{0}";

        #region New/Delete
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbApp"></param>
        /// <param name="parentACObject"></param>
        /// <param name="formatNewNo"></param>
        /// <returns></returns>
        public static ProdOrderBatch NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            ProdOrderBatch entity = new ProdOrderBatch();
            entity.ProdOrderBatchID = Guid.NewGuid();
            entity.MDProdOrderState = MDProdOrderState.DefaultMDProdOrderState(dbApp);
            entity.DefaultValuesACObject();
            entity.ProdOrderBatchNo = secondaryKey;
            if (parentACObject != null)
            {
                if (parentACObject is ProdOrderPartslist)
                {
                    ProdOrderPartslist partslistPos = (ProdOrderPartslist)parentACObject;
                    entity.ProdOrderPartslist = partslistPos;
                }
            }
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
        }

        #endregion

        #region IACUrl Member

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override string ACCaption
        {
            get
            {
                return this.BatchSeqNo.ToString();
            }
        }

        /// <summary>
        /// Returns ProdOrderPartslist
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to ProdOrderPartslist</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return this.ProdOrderPartslist;
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
            return null;
        }

        #endregion

        #region IACObjectEntity Members

        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "ProdOrderBatchNo";
            }
        }

        #endregion
    }
}
