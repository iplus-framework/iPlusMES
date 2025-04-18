using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioLogistics, "en{'Delivery Note'}de{'Eingangslieferschein'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSODeliveryNote")]
    [ACPropertyEntity(1, "DeliveryNoteNo", "en{'Delivery Note No.'}de{'Lieferschein-Nr.'}", "", "", true)]
    [ACPropertyEntity(2, MDDelivNoteState.ClassName, "en{'Delivery Note Status'}de{'Lieferscheinstatus'}", Const.ContextDatabase + "\\" + MDDelivNoteState.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(28, "TourplanPos", "en{'Tour'}de{'Tour'}", Const.ContextDatabase + "\\TourplanPos" + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(32, VisitorVoucher.ClassName, "en{'Visitor Voucher'}de{'Besucher Beleg'}", Const.ContextDatabase + "\\" + VisitorVoucher.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(27, "DeliveryCompanyAddress", "en{'Delivery Address'}de{'Lieferadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(27, "ShipperCompanyAddress", "en{'Shipper Address'}de{'Speditionsadresse'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(28, "Delivery2CompanyAddress", "en{'Refiner'}de{'Veredler'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(3, "DeliveryDate", "en{'Delivery Date'}de{'Lieferdatum'}", "", "", true)]
    [ACPropertyEntity(5, "SupplierDeliveryNo", "en{'Extern Deliv.Note-No.'}de{'Externe Liefers.-Nr.'}", "", "", true)]
    [ACPropertyEntity(4, "Comment", ConstApp.Comment, "", "", true)]
    [ACPropertyEntity(4, "LossComment", "en{'Loss Remark'}de{'Schwund-Bem.'}", "", "", true)]
    [ACPropertyEntity(6, "TotalWeight", "en{'Total Weight'}de{'Gesamtgewicht'}", "", "", true)]
    [ACPropertyEntity(7, "EmptyWeight", "en{'Empty Weight'}de{'Leergewicht'}", "", "", true)]
    [ACPropertyEntity(8, "NetWeight", "en{'Net Weight'}de{'Nettogewicht'}", "", "", true)]
    [ACPropertyEntity(9, "LossWeight", "en{'Loss Weight'}de{'Schwund'}", "", "", true)]
    [ACPropertyEntity(10, "DeliveryWeightOrderIn", "en{'Target Weight In'}de{'Soll Liefergewicht Zugang'}", "", "", true)]
    [ACPropertyEntity(11, "DeliveryWeightDeliveryIn", "en{'Weight according to Deliv.Note'}de{'Gewicht lt. Lieferschein Zugang'}", "", "", true)]
    [ACPropertyEntity(12, "DeliveryWeightStockIn", "en{'Stock Weight In'}de{'Lagergewicht Zugang'}", "", "", true)]
    [ACPropertyEntity(13, "DeliveryWeightOrderOut", "en{'Target Weight Out'}de{'Soll Liefergewicht Abgang'}", "", "", true)]
    [ACPropertyEntity(14, "DeliveryWeightDeliveryOut", "en{'Weight according to Deliv.Note'}de{'Gewicht lt. Lieferschein Abgang'}", "", "", true)]
    [ACPropertyEntity(15, "DeliveryWeightStockOut", "en{'Stock Weight Out'}de{'Lagergewicht Abgang'}", "", "", true)]
    [ACPropertyEntity(9999, "DeliveryNoteTypeIndex", "en{'Delivery Note Type'}de{'Lieferscheintyp'}", typeof(GlobalApp.DeliveryNoteType), Const.ContextDatabase + "\\DeliveryNoteTypeList", "", false)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioLogistics, Const.QueryPrefix + DeliveryNote.ClassName, "en{'Deliverynote'}de{'Lieferschein'}", typeof(DeliveryNote), DeliveryNote.ClassName, "DeliveryNoteNo", "DeliveryNoteNo", new object[]
        {
                new object[] {Const.QueryPrefix + DeliveryNotePos.ClassName, "en{'Deliveryposition'}de{'Lieferscheinposition'}", typeof(DeliveryNotePos), DeliveryNotePos.ClassName + "_" + DeliveryNote.ClassName, "Sequence", "Sequence"}
        })
    ]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<DeliveryNote>) })]
    [NotMapped]
    public partial class DeliveryNote
    {
        [NotMapped]
        public const string ClassName = "DeliveryNote";
        [NotMapped]
        public const string NoColumnName = "DeliveryNoteNo";
        [NotMapped]
        public const string FormatNewNo = "DN{0}";

        #region New/Delete
        public static DeliveryNote NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey)
        {
            DeliveryNote entity = new DeliveryNote();
            entity.DeliveryNoteID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            entity.DeliveryNoteTypeIndex = (short)GlobalApp.DeliveryNoteType.Receipt;
            entity.MDDelivNoteState = MDDelivNoteState.DefaultMDDelivNoteState(dbApp);
            dbApp.Add(entity);
            entity.DeliveryNoteNo = secondaryKey;
            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            return entity;
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
                if (ShipperCompanyAddress == null)
                    return DeliveryNoteNo;
                return DeliveryNoteNo + " " + ShipperCompanyAddress.ACCaption;
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
            if (filterValues.Any() && className == DeliveryNotePos.ClassName)
            {
                Int32 sequence = 0;
                if (Int32.TryParse(filterValues[0], out sequence))
                    return this.DeliveryNotePos_DeliveryNote.Where(c => c.Sequence == sequence).FirstOrDefault();
            }
            return null;
        }

        #endregion

        #region IACObjectEntity Members


        [NotMapped]
        static public string KeyACIdentifier
        {
            get
            {
                return "DeliveryNoteNo";
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

        #region AdditionalProperties
        [NotMapped]
        public GlobalApp.DeliveryNoteType DeliveryNoteType
        {
            get
            {
                return (GlobalApp.DeliveryNoteType)DeliveryNoteTypeIndex;
            }
            set
            {
                DeliveryNoteTypeIndex = (Int16)value;
            }
        }
        #endregion
    }
}
