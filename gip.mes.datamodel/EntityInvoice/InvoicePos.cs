using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESInvoicePos, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Folge'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(3, MDUnit.ClassName, ConstApp.MDUnit, Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(4, "TargetQuantityUOM", ConstApp.TargetQuantityUOM, "", "", true)]
    [ACPropertyEntity(5, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(6, MDCountrySalesTaxMDMaterialGroup.ClassName, ConstApp.ESCountrySalesTaxMDMaterialGroup, Const.ContextDatabase + "\\" + MDCountrySalesTaxMDMaterialGroup.ClassName, "", true)]
    [ACPropertyEntity(7, MDCountrySalesTaxMaterial.ClassName, ConstApp.ESCountrySalesTaxMaterial, Const.ContextDatabase + "\\" + MDCountrySalesTaxMaterial.ClassName, "", true)]
    [ACPropertyEntity(8, "PriceNet", ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(9, "PriceGross", ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(10, "SalesTax", ConstApp.ESCountrySalesTax, "", "", true)]
    [ACPropertyEntity(11, OutOrderPos.ClassName, "en{'Sales Order'}de{'Kundenauftrag'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName, "", true)]
    [ACPropertyEntity(12, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(9999, "XMLDesign", "en{'Design'}de{'Design'}")]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + InvoicePos.ClassName, ConstApp.ESInvoicePos, typeof(InvoicePos), InvoicePos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<InvoicePos>) })]
    public partial class InvoicePos
    {
        public const string ClassName = "InvoicePos";
        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static InvoicePos NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            InvoicePos entity = new InvoicePos();
            entity.InvoicePosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Invoice)
            {
                Invoice invoice = parentACObject as Invoice;
                entity.Sequence = 1;
                if (invoice.InvoicePos_Invoice.Any())
                    entity.Sequence = 1 + invoice.InvoicePos_Invoice.Max(x => x.Sequence);
                invoice.InvoicePos_Invoice.Add(entity);
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
            Invoice invoice = Invoice;
            if (invoice.InvoicePos_Invoice.IsLoaded)
                invoice.InvoicePos_Invoice.Remove(this);
            database.DeleteObject(this);
            InvoicePos.RenumberSequence(Invoice, sequence);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(Invoice invoice, int sequence)
        {
            var elements = from c in invoice.InvoicePos_Invoice where c.Sequence > sequence && c.EntityState != System.Data.EntityState.Deleted orderby c.Sequence select c;
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
                return Sequence.ToString() + " " + Material.ACCaption;
            }
        }

        /// <summary>
        /// Returns Invoice
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Invoice</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return Invoice;
            }
        }

        #region IACObjectEntity Members
        /// <summary>
        /// Method for validating values and references in this EF-Object.
        /// Is called from Change-Tracking before changes will be saved for new unsaved entity-objects.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="context">Entity-Framework databasecontext</param>
        /// <returns>NULL if sucessful otherwise a Message-List</returns>
        public override IList<Msg> EntityCheckAdded(string user, IACEntityObjectContext context)
        {
            if (this.Material == null)
            {
                List<Msg> messages = new List<Msg>();
                messages.Add(new Msg
                {
                    Source = GetACUrl(),
                    ACIdentifier = "Key",
                    Message = "Key",
                    //Message = Database.Root.Environment.TranslateMessage(this, "Error50000", "Key"), 
                    MessageLevel = eMsgLevel.Error
                });
                return messages;
            }
            base.EntityCheckAdded(user, context);
            return null;
        }

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

        [ACPropertyInfo(9999)]
        public string Position
        {
            get
            {
                return "Position " + Sequence.ToString();
            }
        }

        #endregion

        #region Additional
        private ReportData _TempReportData;
        [ACPropertyInfo(9999)]
        public ReportData TempReportData
        {
            get
            {
                return _TempReportData;
            }
            set
            {
                _TempReportData = value;
                OnPropertyChanged("TempReportData");
            }
        }
        #endregion

    }
}
