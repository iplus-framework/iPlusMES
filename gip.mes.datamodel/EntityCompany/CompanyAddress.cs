// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
using gip.core.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Adress'}de{'Adresse'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true)]
    [ACPropertyEntity(1, "Name1", "en{'Name'}de{'Name'}", "", "", true, MinLength = 1)]
    [ACPropertyEntity(2, "Name2", "en{'Name 2'}de{'Name 2'}", "", "", true)]
    [ACPropertyEntity(3, "Name3", "en{'Name 3'}de{'Name 3'}", "", "", true)]
    [ACPropertyEntity(4, "Street", "en{'Street'}de{'Straße'}", "", "", true)]
    [ACPropertyEntity(5, "City", "en{'City'}de{'Stadt'}", "", "", true)]
    [ACPropertyEntity(6, MDCountry.ClassName, "en{'Country'}de{'Land'}", Const.ContextDatabase + "\\" + MDCountry.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(7, MDCountryLand.ClassName, "en{'Federal State'}de{'Bundesland'}", Const.ContextDatabase + "\\" + MDCountryLand.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(8, "Fax", "en{'Fax'}de{'Fax'}", "", "", true)]
    [ACPropertyEntity(9, "IsBillingCompanyAddress", "en{'Billing Address'}de{'Rechnungsadresse'}", "", "", true)]
    [ACPropertyEntity(10, "IsDeliveryCompanyAddress", "en{'Delivery Address'}de{'Lieferadresse'}", "", "", true)]
    [ACPropertyEntity(11, "IsFactory", "en{'Factory'}de{'Fabrik'}", "", "", true)]
    [ACPropertyEntity(12, "IsHouseCompanyAddress", "en{'House Address'}de{'Adresse Zentrale'}", "", "", true)]
    [ACPropertyEntity(13, "Mobile", "en{'Mobile'}de{'Handynummer'}", "", "", true)]
    [ACPropertyEntity(14, "Phone", "en{'Phone'}de{'Telefonnummer'}", "", "", true)]
    [ACPropertyEntity(15, "Postcode", "en{'Postcode'}de{'Postleitzahl'}", "", "", true)]
    [ACPropertyEntity(16, "PostOfficeBox", "en{'Post Office Box'}de{'Postfach'}", "", "", true)]
    [ACPropertyEntity(17, "RouteNo", "en{'RouteNo'}de{'Gebietsnummer'}", "", "", true)]
    [ACPropertyEntity(18, MDDelivType.ClassName, "en{'Delivery Type'}de{'Lieferart'}", Const.ContextDatabase + "\\" + MDDelivType.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(19, "EMail", "en{'E-Mail'}de{'E-Mail'}", "", "", true)]
    [ACPropertyEntity(20, "InvoiceIssuerNo", "en{'Invoice Issuer No.'}de{'Rechnungsaussteller-Nr.'}", "", "", true)]
    [ACPropertyEntity(21, nameof(WebUrl), "en{'Web address'}de{'Webadresse'}", "", "", true)]
    [ACPropertyEntity(9999, Company.ClassName, ConstApp.Company, Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACPropertyEntity(9999, "GEO_x", "en{'GEO_x'}de{'GEO_x'}", "", "", true)]
    [ACPropertyEntity(9999, "GEO_y", "en{'GEO_y'}de{'GEO_y'}", "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACQueryInfoPrimary(Const.PackName_VarioCompany, Const.QueryPrefix + CompanyAddress.ClassName, "en{'Adress'}de{'Adresse'}", typeof(CompanyAddress), CompanyAddress.ClassName, "Name1", "Name1")]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<CompanyAddress>) })]
    [NotMapped]
    public partial class CompanyAddress
    {
        [NotMapped]
        public const string ClassName = "CompanyAddress";

        #region New/Delete
        public static CompanyAddress NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            CompanyAddress entity = new CompanyAddress();
            entity.CompanyAddressID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (parentACObject is Company)
            {
                entity.Company = parentACObject as Company;
            }
            entity.MDCountry = MDCountry.DefaultMDCountry(dbApp);
            entity.MDCountryLand = MDCountryLand.DefaultMDCountryLand(dbApp);
            entity.MDDelivType = MDDelivType.DefaultMDDelivType(dbApp);
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
                return Name1 + " " + City;
            }
        }

        /// <summary>
        /// Returns Company
        /// NOT THREAD-SAFE USE QueryLock_1X000!
        /// </summary>
        /// <value>Reference to Company</value>
        [ACPropertyInfo(9999)]
        [NotMapped]
        public override IACObject ParentACObject
        {
            get
            {
                return Company;
            }
        }

        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {

            if (filterValues.Any())
            {
                switch (className)
                {
                    case CompanyAddressUnloadingpoint.ClassName:
                        Int32 sequence = 0;
                        if (Int32.TryParse(filterValues[0], out sequence))
                            return this.CompanyAddressUnloadingpoint_CompanyAddress.Where(c => c.Sequence == sequence).FirstOrDefault();
                        break;
                    case CompanyAddressDepartment.ClassName:
                        return this.CompanyAddressDepartment_CompanyAddress.Where(c => c.DepartmentName == filterValues[0]).FirstOrDefault();
                }
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
                return "Name1";
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

    }
}
