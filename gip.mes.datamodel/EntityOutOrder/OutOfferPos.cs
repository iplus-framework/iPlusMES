using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSales, "en{'Offering Pos'}de{'Angebotsbotsposition'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Sequence", "en{'Sequence'}de{'Folge'}", "", "", true)]
    [ACPropertyEntity(2, Material.ClassName, ConstApp.Material, Const.ContextDatabase + "\\" + Material.ClassName, "", true)]
    [ACPropertyEntity(3, "TargetQuantityUOM", ConstApp.TargetQuantityUOM, "", "", true)]
    [ACPropertyEntity(4, MDUnit.ClassName, ConstApp.MDUnit, Const.ContextDatabase + "\\MDUnitList", "", true)]
    [ACPropertyEntity(6, "TargetQuantity", ConstApp.TargetQuantity, "", "", true)]
    [ACPropertyEntity(7, "TargetWeight", "en{'Target Weight'}de{'Zielgewicht'}", "", "", true)]
    [ACPropertyEntity(8, "TargetDeliveryDate", "en{'Target Delivery Date'}de{'Gewünschter Liefertermin'}", "", "", true)]
    [ACPropertyEntity(9, "TargetDeliveryMaxDate", "en{'Latest Delivery Date'}de{'Spätester Liefertermin'}", "", "", true)]
    [ACPropertyEntity(10, "TargetDeliveryPriority", "en{'Delivery Priority'}de{'Lieferpriorität'}", "", "", true)]
    [ACPropertyEntity(11, MDTimeRange.ClassName, "en{'Time Range'}de{'Zeitraum'}", Const.ContextDatabase + "\\" + MDTimeRange.ClassName, "", true)]
    [ACPropertyEntity(12, "PriceNet", ConstApp.PriceNet, "", "", true)]
    [ACPropertyEntity(13, "PriceGross", ConstApp.PriceGross, "", "", true)]
    [ACPropertyEntity(14, MDCountrySalesTax.ClassName, "en{'Sales Tax'}de{'Umsatzsteuer'}", Const.ContextDatabase + "\\" + MDCountrySalesTax.ClassName, "", true)]
    [ACPropertyEntity(15, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(16, "Comment2", "en{'Comment 2'}de{'Bemerkung 2'}", "", "", true)]
    [ACPropertyEntity(17, "GroupSum", "en{'Group subtotal'}de{'Zwischensummengruppe '}", "", "", true)]
    [ACPropertyEntity(18, "SalesTax", ConstApp.ESCountrySalesTax, "", "", true)]
    [ACPropertyEntity(9999, OutOffer.ClassName, "en{'Offer'}de{'Angebot'}", "", "", true)]
    [ACPropertyEntity(9999, "MaterialPosTypeIndex", "en{'Position Type'}de{'Posistionstyp'}", typeof(GlobalApp.MaterialPosTypes), "", "", true)]
    [ACPropertyEntity(9999, "XMLDesign", "en{'Design'}de{'Design'}")]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioSales, Const.QueryPrefix + OutOfferPos.ClassName, "en{'Offer Position'}de{'Angebotsposition'}", typeof(OutOfferPos), OutOfferPos.ClassName, "Sequence", "Sequence")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<OutOfferPos>) })]
    public partial class OutOfferPos : IOutOrderPos, ICloneable
    {
        public const string ClassName = "OutOfferPos";

        #region New/Delete
        /// <summary>
        /// Handling von Sequencenummer wird automatisch bei der Anlage durchgeführt
        /// </summary>
        public static OutOfferPos NewACObject(DatabaseApp dbApp, IACObject parentACObject, OutOfferPos parentGroupPos)
        {
            OutOfferPos entity = new OutOfferPos();
            entity.OutOfferPosID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is OutOffer)
            {
                OutOffer OutOffer = parentACObject as OutOffer;
                try
                {
                    if (OutOffer.EntityState != EntityState.Added)
                    {
                        if (!OutOffer.OutOfferPos_OutOffer_IsLoaded)
                            OutOffer.OutOfferPos_OutOffer.AutoLoad(OutOffer.OutOfferPos_OutOfferReference, OutOffer);
                    }
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    if (Database.Root != null && Database.Root.Messages != null)
                        Database.Root.Messages.LogException(ClassName, Const.MN_NewACObject, msg);
                }

                if (parentGroupPos == null)
                {
                    if (OutOffer.OutOfferPos_OutOffer != null && OutOffer.OutOfferPos_OutOffer.Where(c => !c.GroupOutOfferPosID.HasValue).Select(c => c.Sequence).Any())
                        entity.Sequence = OutOffer.OutOfferPos_OutOffer.Where(c => !c.GroupOutOfferPosID.HasValue).Select(c => c.Sequence).Max() + 1;
                    else
                        entity.Sequence = 1;
                }
                else
                {
                    if (OutOffer.OutOfferPos_OutOffer != null && OutOffer.OutOfferPos_OutOffer.Where(c => c.GroupOutOfferPosID == parentGroupPos.OutOfferPosID).Select(c => c.Sequence).Any())
                        entity.Sequence = OutOffer.OutOfferPos_OutOffer.Where(c => c.GroupOutOfferPosID == parentGroupPos.OutOfferPosID).Select(c => c.Sequence).Max() + 1;
                    else
                        entity.Sequence = 1;
                }
                entity.OutOffer = OutOffer;
            }
            entity.MaterialPosTypeIndex = (Int16)GlobalApp.MaterialPosTypes.OutwardRoot;
            //entity.TargetQuantityUOM = 1;
            entity.XMLDesign = "<?xml version=\"1.0\" encoding=\"utf-8\"?><FlowDocument PageWidth=\"816\" PageHeight=\"1056\" PagePadding=\"96,96,96,96\" AllowDrop=\"True\" NumberSubstitution.CultureSource=\"User\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"></FlowDocument>";
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
            OutOffer outOffer = OutOffer;
            OutOfferPos groupPos = OutOfferPos1_GroupOutOfferPos;
            database.Remove(this);
            OutOfferPos.RenumberSequence(outOffer, sequence, groupPos);
            return null;
        }

        /// <summary>
        /// Handling von Sequencenummer ist nach dem Löschen aufzurufen
        /// </summary>
        public static void RenumberSequence(OutOffer outOffer, int sequence, OutOfferPos groupPos = null)
        {
            if (outOffer == null
                || !outOffer.OutOfferPos_OutOffer.Any())
                return;

            IEnumerable<OutOfferPos> elements = null;

            if (groupPos != null)
            {
                if (!groupPos.OutOfferPos_GroupOutOfferPos.Any())
                    return;
                elements = groupPos.OutOfferPos_GroupOutOfferPos.Where(c => c.Sequence > sequence).ToList();
            }
            else
            {
                elements = outOffer.OutOfferPos_OutOffer.Where(c => c.Sequence > sequence).ToList();
            }

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
                return Sequence.ToString() + " " + Material?.ACCaption;
            }
        }

        /// <summary>
        /// Returns OutOffer
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to OutOffer</value>
        [ACPropertyInfo(9999)]
        public override IACObject ParentACObject
        {
            get
            {
                return OutOffer;
            }
        }

        #endregion

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

        #region Additional properties

        [ACPropertyInfo(30)]
        public string Position
        {
            get
            {
                if (OutOfferPos1_GroupOutOfferPos == null)
                    return Sequence.ToString("00");
                else
                {
                    return OutOfferPos1_GroupOutOfferPos.Position + "." + Sequence.ToString("00");
                }
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (propertyName == nameof(Sequence))
            {
                base.OnPropertyChanged("Position");
            }
            base.OnPropertyChanged(propertyName);
        }


        #region Report helper

        [ACPropertyInfo(31)]
        public List<OutOfferPos> Items
        {
            get
            {
                return OutOfferPos_GroupOutOfferPos?.ToList();
            }
        }

        [ACPropertyInfo(32, "", ConstApp.PriceNetTotal)]
        public decimal TotalPrice
        {
            get
            {
                return Convert.ToDecimal(TargetQuantity) * PriceNet;
            }
        }

        [ACPropertyInfo(33)]
        public string QuantityWithUnitPrinted
        {
            get
            {
                if (!GroupSum)
                    return TargetQuantity + " " + DerivedMDUnit?.Symbol;
                return "";
            }
        }

        [ACPropertyInfo(34)]
        public string PriceNetPrinted
        {
            get
            {
                if (!GroupSum)
                    return PriceNet.ToString("N");
                return "";
            }
        }

        public string _TotalPricePrinted;
        [ACPropertyInfo(35)]
        public string TotalPricePrinted
        {
            get
            {
                if (_TotalPricePrinted != null)
                    return _TotalPricePrinted;
                if (!GroupSum)
                    return TotalPrice.ToString("N");
                return "";
            }
            set
            {
                _TotalPricePrinted = value;
            }
        }

        private string _MaterialNo;
        [ACPropertyInfo(36)]
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

        [ACPropertyInfo(38)]
        public string SalesTaxPrinted
        {
            get
            {
                if (!GroupSum)
                    return SalesTax.ToString("N");
                return "";
            }
        }

        [ACPropertyInfo(31, "", ConstApp.VATPerUnit)]
        public decimal SalesTaxAmount
        {
            get
            {
                return PriceNet * (SalesTax / 100);            
            }
        }

        [ACPropertyInfo(32, "", ConstApp.VATTotal)]
        public decimal TotalSalesTax
        {
            get
            {
                return SalesTaxAmount * Convert.ToDecimal(this.TargetQuantity);
            }
        }

        [ACPropertyInfo(33, "", ConstApp.PriceGrossTotal)]
        public decimal TotalPriceWithTax
        {
            get
            {
                return TotalPrice + TotalSalesTax;
            }
        }

        [ACPropertyInfo(37)]
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


        public bool InRecalculation { get; set; }
        #endregion


        #endregion

        public object Clone()
        {
            OutOfferPos clonedObject = new OutOfferPos();
            clonedObject.OutOfferPosID = this.OutOfferPosID;
            clonedObject.OutOfferID = this.OutOfferID;
            clonedObject.ParentOutOfferPosID = this.ParentOutOfferPosID;
            clonedObject.GroupOutOfferPosID = this.GroupOutOfferPosID;
            clonedObject.MDTimeRangeID = this.MDTimeRangeID;
            clonedObject.MaterialPosTypeIndex = this.MaterialPosTypeIndex;
            clonedObject.MDCountrySalesTaxID = this.MDCountrySalesTaxID;
            clonedObject.MDCountrySalesTaxMDMaterialGroupID = this.MDCountrySalesTaxMDMaterialGroupID;
            clonedObject.MDCountrySalesTaxMaterialID = this.MDCountrySalesTaxMaterialID;
            clonedObject.Sequence = this.Sequence;
            clonedObject.MaterialID = this.MaterialID;
            clonedObject.MDUnitID = this.MDUnitID;
            clonedObject.TargetQuantityUOM = this.TargetQuantityUOM;
            clonedObject.TargetQuantity = this.TargetQuantity;
            clonedObject.PriceNet = this.PriceNet;
            clonedObject.PriceGross = this.PriceGross;
            clonedObject.SalesTax = this.SalesTax;
            clonedObject.TargetWeight = this.TargetWeight;
            clonedObject.TargetDeliveryDate = this.TargetDeliveryDate;
            clonedObject.TargetDeliveryMaxDate = this.TargetDeliveryMaxDate;
            clonedObject.TargetDeliveryPriority = this.TargetDeliveryPriority;
            clonedObject.GroupSum = this.GroupSum;
            clonedObject.Comment = this.Comment;
            clonedObject.Comment2 = this.Comment2;
            clonedObject.XMLDesign = this.XMLDesign;
            clonedObject.XMLConfig = this.XMLConfig;

            return clonedObject;
        }
    }
}
