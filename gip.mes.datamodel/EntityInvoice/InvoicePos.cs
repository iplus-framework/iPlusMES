﻿using gip.core.autocomponent;
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, ConstApp.ESInvoicePos, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Folge'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, MDUnit.ClassName, ConstApp.MDUnit, Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(4, "TargetQuantityUOM", ConstApp.TargetQuantityUOM, "", "", true)]
    [ACPropertyEntity(5, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(6, MDCountrySalesTaxMDMaterialGroup.ClassName, ConstApp.ESCountrySalesTaxMDMaterialGroup, Const.ContextDatabase + "\\" + MDCountrySalesTaxMDMaterialGroup.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(7, MDCountrySalesTaxMaterial.ClassName, ConstApp.ESCountrySalesTaxMaterial, Const.ContextDatabase + "\\" + MDCountrySalesTaxMaterial.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(8, "PriceNet", ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(9, "PriceGross", ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(10, "SalesTax", ConstApp.ESCountrySalesTax, "", "", true)]
    [ACPropertyEntity(11, OutOrderPos.ClassName, "en{'Sales Order'}de{'Kundenauftrag'}", Const.ContextDatabase + "\\" + OutOrderPos.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(12, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(9999, "XMLDesign", "en{'Design'}de{'Design'}")]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + InvoicePos.ClassName, ConstApp.ESInvoicePos, typeof(InvoicePos), InvoicePos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<InvoicePos>) })]
    [NotMapped]
    public partial class InvoicePos : IOutOrderPos
    {
        [NotMapped]
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
            entity.XMLDesign = Invoice.Const_XMLDesign;
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
            Invoice invoice = Invoice;
            if (invoice.InvoicePos_Invoice_IsLoaded)
                invoice.InvoicePos_Invoice.Remove(this);
            base.DeleteACObject(database, withCheck, softDelete);
            return null;
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
                return Sequence.ToString() + " " + Material.ACCaption;
            }
        }

        /// <summary>
        /// Returns Invoice
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Invoice</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
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
                return Sequence.ToString("00");
            }
        }

        #endregion

        #region IOutOrder

        [ACPropertyInfo(31, "", ConstApp.VATPerUnit)]
        [NotMapped]
        public decimal SalesTaxAmount
        {
            get
            {
                return PriceNet * (SalesTax / 100);
            }
        }

        [ACPropertyInfo(32, "", ConstApp.PriceNetTotal)]
        [NotMapped]
        public decimal TotalPrice
        {
            get
            {
                return Convert.ToDecimal(TargetQuantity) * PriceNet;
            }
        }

        [ACPropertyInfo(32, "", ConstApp.VATTotal)]
        [NotMapped]
        public decimal TotalSalesTax
        {
            get
            {
                return Convert.ToDecimal(TargetQuantity) * SalesTaxAmount;
            }
        }

        [ACPropertyInfo(33, "", ConstApp.PriceGrossTotal)]
        [NotMapped]
        public decimal TotalPriceWithTax
        {
            get
            {
                return TotalPrice + TotalSalesTax;
            }
        }

        [ACPropertyInfo(34)]
        [NotMapped]
        public string QuantityWithUnitPrinted
        {
            get
            {
                //if (TargetQuantity > 0)
                    return TargetQuantity + " " + DerivedMDUnit?.Symbol;
                //return "";
            }
        }

        [ACPropertyInfo(37)]
        [NotMapped]
        public MDUnit DerivedMDUnit
        {
            get
            {
                if (MDUnit != null)
                    return MDUnit;
                if (this.Material != null)
                    return Material.BaseMDUnit;
                return null;
            }
        }

        [NotMapped]
        public bool InRecalculation { get; set; }
        //private bool _InLocalRecalc = false;

        [ACPropertyInfo(38, "", ConstApp.ForeignPriceNet)]
        [NotMapped]
        public decimal? ForeignPriceNet
        {
            get
            {
                if (Invoice == null || Invoice.MDCurrencyExchange == null)
                    return null;
                return Invoice.MDCurrencyExchange.ConvertToForeignCurrency(this.PriceNet);
            }
            set
            {
                if (Invoice == null || Invoice.MDCurrencyExchange == null || value == null)
                    return;
                PriceNet = Invoice.MDCurrencyExchange.ConvertBackToLocalCurrency(value.Value);
                //OnPropertyChanged("ForeignPriceNet");
            }
        }

        [ACPropertyInfo(38, "", ConstApp.ForeignPriceGross)]
        [NotMapped]
        public decimal? ForeignPriceGross
        {
            get
            {
                if (Invoice == null || Invoice.MDCurrencyExchange == null)
                    return null;
                return Invoice.MDCurrencyExchange.ConvertToForeignCurrency(this.PriceGross);
            }
            set
            {
                if (Invoice == null || Invoice.MDCurrencyExchange == null || value == null)
                    return;
                PriceGross = Invoice.MDCurrencyExchange.ConvertBackToLocalCurrency(value.Value);
                //OnPropertyChanged("ForeignPriceGross");
            }
        }

        [ACPropertyInfo(39, "", ConstApp.ForeignTotalPrice)]
        [NotMapped]
        public decimal? ForeignTotalPrice
        {
            get
            {
                if (Invoice == null || Invoice.MDCurrencyExchange == null)
                    return null;
                return Invoice.MDCurrencyExchange.ConvertToForeignCurrency(this.TotalPrice);
            }
        }

        [ACPropertyInfo(40, "", ConstApp.ForeignPriceGrossTotal)]
        [NotMapped]
        public decimal? ForeignTotalPriceWithTax
        {
            get
            {
                if (Invoice == null || Invoice.MDCurrencyExchange == null)
                    return null;
                return Invoice.MDCurrencyExchange.ConvertToForeignCurrency(this.TotalPriceWithTax);
            }
        }

        [NotMapped]
        private string _MaterialNo;
        [ACPropertyInfo(41)]
        [NotMapped]
        public string MaterialNo
        {
            get
            {
                if (_MaterialNo != null)
                    return _MaterialNo;

                return Material?.MaterialNo;
            }
            set
            {
                _MaterialNo = value;
            }
        }

        [ACPropertyInfo(42)]
        [NotMapped]
        public string SalesTaxPrinted
        {
            get
            {
                //if (!GroupSum)
                    return SalesTax.ToString("N");
                //return "";
            }
        }

        [ACPropertyInfo(43)]
        [NotMapped]
        public string PriceNetPrinted
        {
            get
            {
                //if (!GroupSum)
                    return FormatWithCurrency(PriceNet.ToString("N"));
                //return "";
            }
        }

        [NotMapped]
        public string _TotalPricePrinted;
        [ACPropertyInfo(44)]
        [NotMapped]
        public string TotalPricePrinted
        {
            get
            {
                if (_TotalPricePrinted != null)
                    return FormatWithCurrency(_TotalPricePrinted);
                //if (!GroupSum)
                    return FormatWithCurrency(TotalPrice.ToString("N"));
                //return "";
            }
            set
            {
                _TotalPricePrinted = value;
            }
        }

        [ACPropertyInfo(45)]
        [NotMapped]
        public string ForeignPriceNetPrinted
        {
            get
            {
                //if (!GroupSum)
                if (!ForeignPriceNet.HasValue)
                    return "";
                return FormatWithForeignCurrency(ForeignPriceNet.Value.ToString("N"));
                //return "";
            }
        }

        [NotMapped]
        public string _ForeignTotalPricePrinted;
        [ACPropertyInfo(46)]
        [NotMapped]
        public string ForeignTotalPricePrinted
        {
            get
            {
                if (_ForeignTotalPricePrinted != null)
                    return FormatWithForeignCurrency(_ForeignTotalPricePrinted);
                //if (!GroupSum)
                if (!ForeignTotalPrice.HasValue)
                    return "";
                return FormatWithForeignCurrency(ForeignTotalPrice.Value.ToString("N"));
                //return "";
            }
            set
            {
                _ForeignTotalPricePrinted = value;
            }
        }

        public string FormatWithCurrency(string value)
        {
            if (Invoice == null)
                return null;
            return String.Format("{0} {1}", value, Invoice.MDCurrency.MDCurrencyShortname);
        }

        public string FormatWithForeignCurrency(string value)
        {
            if (Invoice == null || Invoice.MDCurrencyExchange == null)
                return null;
            return String.Format("({0} {1})", value, Invoice.MDCurrencyExchange.ToMDCurrency.MDCurrencyShortname);
        }

        #endregion

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            switch (propertyName)
            {
                case nameof(PriceGross):
                    base.OnPropertyChanged("ForeignPriceGross");
                    break;
                case nameof(PriceNet):
                    base.OnPropertyChanged("ForeignPriceNet");
                    break;
            }
            base.OnPropertyChanged(propertyName);
        }

    }
}
