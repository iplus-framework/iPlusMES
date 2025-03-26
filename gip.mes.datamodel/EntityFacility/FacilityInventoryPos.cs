using gip.core.datamodel;
using System;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioFacility, ConstApp.Inventoryposition, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, nameof(Sequence), "en{'Sequence'}de{'Sequenz'}", "", "", true)]
    [ACPropertyEntity(2, nameof(Comment), "en{'Comment'}de{'Bemerkung'}", "", "", true)]
    [ACPropertyEntity(3, nameof(FacilityCharge), "en{'Charge'}de{'Charge'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName, "", true)]
    [ACPropertyEntity(4, nameof(FacilityInventory), "en{'Inventory'}de{'Inventur'}", Const.ContextDatabase + "\\FacilityInventoryList", "", true)]
    [ACPropertyEntity(5, nameof(MDFacilityInventoryPosState), "en{'Inventorypositionstate'}de{'Inventurpositionsstatus'}", Const.ContextDatabase + "\\MDFacilityInventoryPosStateList", "", true)]
    [ACPropertyEntity(6, nameof(NewStockQuantity), "en{'New Stockquantity'}de{'Neue Lagermenge'}", "", "", true)]
    [ACPropertyEntity(7, nameof(NotAvailable), ConstApp.NotAvailable, "", "", true)]
    [ACPropertyEntity(8, nameof(StockQuantity), "en{'Old stock'}de{'Alter Bestand'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioFacility, Const.QueryPrefix + nameof(FacilityInventoryPos), ConstApp.Inventoryposition, typeof(FacilityInventoryPos), nameof(FacilityInventoryPos), "FacilityCharge\\Material\\MaterialNo", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<FacilityInventoryPos>) })]
    public partial class FacilityInventoryPos
    {

        #region New/Delete
        public static FacilityInventoryPos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            FacilityInventoryPos entity = new FacilityInventoryPos();
            entity.FacilityInventoryPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.MDFacilityInventoryPosState = MDFacilityInventoryPosState.DefaultMDFacilityInventoryPosState(dbApp);
            if (parentACObject is FacilityInventory)
            {
                FacilityInventory facilityInventory = parentACObject as FacilityInventory;
                entity.FacilityInventory = facilityInventory;
                facilityInventory.FacilityInventoryPos_FacilityInventory.Add(entity);

                if (facilityInventory.FacilityInventoryPos_FacilityInventory != null && facilityInventory.FacilityInventoryPos_FacilityInventory.Select(c => c.Sequence).Any())
                    entity.Sequence = facilityInventory.FacilityInventoryPos_FacilityInventory.Select(c => c.Sequence).Max() + 1;
                else
                    entity.Sequence = 1;
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
            FacilityInventory facilityInventory = this.FacilityInventory;
            database.DeleteObject(this);
            FacilityInventoryPos.RenumberSequence(facilityInventory, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// !!! Spezialsortierung, wegen alternativen Positionen
        /// </summary>
        public static void RenumberSequence(FacilityInventory facilityInventory, int sequence)
        {
            if (facilityInventory == null
                || !facilityInventory.FacilityInventoryPos_FacilityInventory.Any())
                return;
            var elements = facilityInventory.FacilityInventoryPos_FacilityInventory.Where(c => c.Sequence > sequence && c.Sequence != 0).OrderBy(c => c.Sequence);
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
                return FacilityCharge?.ACCaption;
            }
        }

        /// <summary>
        /// Returns FacilityInventory
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to FacilityInventory</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return FacilityInventory;
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

        private bool _IsSelected;
        [ACPropertyInfo(999, nameof(IsSelected), "en{'Selected'}de{'Ausgewählt'}")]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        /// <summary>
        /// Source Property: 
        /// </summary>
        [ACPropertyInfo(999, nameof(NewStock), "en{'New stock'}de{'Neuer Bestand'}")]
        public double? NewStock
        {
            get
            {
                if (NotAvailable)
                {
                    return null;
                }
                return NewStockQuantity != null ? (NewStockQuantity ?? 0) : StockQuantity;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [ACPropertyInfo(999, nameof(IsInfiniteStock), "en{'Infinite Stock'}de{'Unendlicher Bestand'}")]
        public bool IsInfiniteStock
        {
            get
            {
                return
                        FacilityCharge != null
                        && FacilityCharge.Material != null
                        && FacilityCharge.Material.MDInventoryManagementType != null
                        && FacilityCharge.Material.MDInventoryManagementType.InventoryManagementType == MDInventoryManagementType.InventoryManagementTypes.InfiniteStock;
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
