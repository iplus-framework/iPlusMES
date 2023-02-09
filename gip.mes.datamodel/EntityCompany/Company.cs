using System;
using System.Collections.Generic;
using System.Linq;
using gip.core.datamodel;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioCompany, "en{'Company'}de{'Unternehmen'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOCompany")]
    [ACPropertyEntity(1, "CompanyName", "en{'Company Name'}de{'Name des Unternehmens'}", "", "Name of Company", MinLength = 1)]
    [ACPropertyEntity(2, "CompanyNo", "en{'Company No.'}de{'Unternehmensnr.'}", "", "Number of Company", MinLength = 1)]
    [ACPropertyEntity(3, "IsCustomer", "en{'Customer'}de{'Kunde'}", "", "", true)]
    [ACPropertyEntity(4, "IsDistributor", "en{'Distributor'}de{'Lieferant'}", "", "", true)]
    [ACPropertyEntity(5, "IsDistributorLead", "en{'Lead Distributor'}de{'Hauptlieferant'}", "", "", true)]
    [ACPropertyEntity(6, "IsOwnCompany", "en{'Own Company'}de{'Eigene Firma'}", "", "", true)]
    [ACPropertyEntity(7, "IsSalesLead", "en{'Lead Sales'}de{'Hauptverkäufer'}", "", "", true)]
    [ACPropertyEntity(8, "IsShipper", "en{'Shipper'}de{'Spedition'}", "", "", true)]
    [ACPropertyEntity(9, "NoteExternal", "en{'Note External'}de{'Hinweis extern'}", "", "", true)]
    [ACPropertyEntity(10, "NoteInternal", "en{'Note Internal'}de{'Hinweis intern'}", "", "", true)]
    [ACPropertyEntity(11, "BillingAccountNo", "en{'Billing Account No.'}de{'Rechnungs-Kontonummer'}", "", "", true)]
    [ACPropertyEntity(12, "UseBillingAccountNo", "en{'Use Billing Account No.'}de{'Verwende Rechnungs-KtNr.'}", "", "", true)]
    [ACPropertyEntity(13, "BillingMDTermOfPayment", "en{'Billing Term of Payment'}de{'Zahlungsziel der Fakturierung'}", Const.ContextDatabase + "\\" + MDTermOfPayment.ClassName, "", true)]
    [ACPropertyEntity(14, "ShippingAccountNo", "en{'Shipping Account No.'}de{'Speditions-Kontonummer'}", "", "", true)]
    [ACPropertyEntity(15, "UseShippingAccountNo", "en{'Use Shipping Account No.'}de{'Verwende Speditions-KtNr.'}", "", "", true)]
    [ACPropertyEntity(16, "ShippingMDTermOfPayment", "en{'Shipping Term of Payment'}de{'Zahlungsbedingungen für den Versand'}", Const.ContextDatabase + "\\" + MDTermOfPayment.ClassName, "", true)]
    [ACPropertyEntity(17, MDCurrency.ClassName, "en{'Currency'}de{'Währung'}", Const.ContextDatabase + "\\" + MDCurrency.ClassName, "", true)]
    //[ACPropertyEntity(18, CompanyAddress.ClassName, "en{'CompanyAddress'}de{'Firmenanschrift'}", Const.ContextDatabase + "\\" + CompanyAddress.ClassName, "", true)]
    [ACPropertyEntity(19, "IsActive", "en{'Active'}de{'Aktiv'}", "", "", true)]
    [ACPropertyEntity(20, "VATNumber", "en{'VAT-Number'}de{'USt.-Identifikationsnummer'}", "", "", true)]
    [ACPropertyEntity(21, "IsTenant", "en{'Client/Contract Partner'}de{'Mandant/Vertragspartner'}", "", "", true)]
    [ACPropertyEntity(22, ConstApp.KeyOfExtSys, ConstApp.EntityTranslateKeyOfExtSys, "", "", true)]
    [ACPropertyEntity(496, Const.EntityInsertDate, Const.EntityTransInsertDate)]
    [ACPropertyEntity(497, Const.EntityInsertName, Const.EntityTransInsertName)]
    [ACPropertyEntity(498, Const.EntityUpdateDate, Const.EntityTransUpdateDate)]
    [ACPropertyEntity(499, Const.EntityUpdateName, Const.EntityTransUpdateName)]
    [ACPropertyEntity(9999, "Company1_ParentCompany", "en{'Parent Company'}de{'Muttergesellschaft'}", Const.ContextDatabase + "\\" + Company.ClassName, "", true)]
    [ACDeleteAction("Company_ParentCompany", Global.DeleteAction.CascadeManual)]
    [ACQueryInfoPrimary(Const.PackName_VarioCompany, Const.QueryPrefix + Company.ClassName, "en{'Company'}de{'Unternehmen'}", typeof(Company), Company.ClassName, "CompanyNo,CompanyName", "CompanyName", new object[]
        {
                new object[] { Const.QueryPrefix + CompanyAddress.ClassName, "en{'Adress'}de{'Adresse'}", typeof(CompanyAddress), CompanyAddress.ClassName + "_" + Company.ClassName, "Name1", "Name1",  new object[]
                    {
                        new object[] {Const.QueryPrefix + CompanyAddressUnloadingpoint.ClassName, "en{'Unloadingpoint'}de{'Abladestelle'}", typeof(CompanyAddressUnloadingpoint), CompanyAddressUnloadingpoint.ClassName + "_" + CompanyAddress.ClassName, "UnloadingPointName", "Sequence"}
                    }
                },
                new object[] {Const.QueryPrefix + CompanyMaterial.ClassName, "en{'Company Material'}de{'Unternehmensmaterial'}", typeof(CompanyMaterial), CompanyMaterial.ClassName + "_" + Company.ClassName, "CompanyMaterialNo", "CompanyMaterialNo"},
                new object[] {Const.QueryPrefix + CompanyPerson.ClassName, "en{'Person'}de{'Person'}", typeof(CompanyPerson), CompanyPerson.ClassName + "_" + Company.ClassName, "Name1", "Name1"}
        })
    ]
    [ACSerializeableInfo(new Type[] { typeof(ACRef<Company>) })]
    public partial class Company
    {
        public const string ClassName = "Company";
        public const string NoColumnName = "CompanyNo";
        public const string FormatNewNo = null;

        #region New/Delete
        public static Company NewACObject(DatabaseApp dbApp, IACObject parentACObject, string secondaryKey, bool autoGetNewNo = true)
        {
            Company entity = new Company();
            entity.CompanyID = Guid.NewGuid();
            entity.DefaultValuesACObject();
            if (autoGetNewNo)
                entity.CompanyNo = secondaryKey;
            entity.MDCurrency = MDCurrency.DefaultMDCurrency(dbApp);

            if (parentACObject is Company)
            {
                Company parentCompany = parentACObject as Company;
                entity.Company1_ParentCompany = parentCompany;
                parentCompany.Company_ParentCompany.Add(entity);
            }

            // Automatisch eine Hausaddresse anlegen
            CompanyAddress address = CompanyAddress.NewACObject(dbApp, entity);
            address.IsHouseCompanyAddress = true;
            address.IsBillingCompanyAddress = true;
            address.IsDeliveryCompanyAddress = true;
            entity.CompanyAddress_Company.Add(address);
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

            // Löschen
            try
            {
                //database.ExecuteStoreCommand("DELETE FROM FacilityBookingCharge WHERE InwardFacilityID={0}", this.FacilityID);
                foreach (CompanyAddress cAdr in this.CompanyAddress_Company.ToArray())
                {
                    MsgWithDetails msg = cAdr.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.CompanyAddress_Company.Clear();

                //database.ExecuteStoreCommand("DELETE FROM FacilityBookingCharge WHERE InwardFacilityID={0}", this.FacilityID);
                foreach (CompanyMaterial cMat in this.CompanyMaterial_Company.ToArray())
                {
                    MsgWithDetails msg = cMat.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.CompanyMaterial_Company.Clear();

                //database.ExecuteStoreCommand("DELETE FROM FacilityBookingCharge WHERE InwardFacilityID={0}", this.FacilityID);
                foreach (CompanyPerson cPers in this.CompanyPerson_Company.ToArray())
                {
                    MsgWithDetails msg = cPers.DeleteACObject(database, withCheck);
                    if (msg != null)
                        return msg;
                }
                this.CompanyPerson_Company.Clear();

            }
            catch (Exception e)
            {
                MsgWithDetails msg = new MsgWithDetails { Source = "Company", MessageLevel = eMsgLevel.Error, ACIdentifier = "DeleteACObject", Message = Database.Root.Environment.TranslateMessage(Database.GlobalDatabase, "Error00035") };
                ACObjectContextHelper.ParseExceptionStatic(msg, e);
                return msg;
            }

            // Referenzen auflösen:

            //database.ExecuteStoreCommand("UPDATE Facility SET IncomingFacilityID = NULL WHERE IncomingFacilityID={0}", this.FacilityID);
            foreach (FacilityBooking fb in this.FacilityBooking_CPartnerCompany.ToArray())
            {
                fb.CPartnerCompany = null;
            }
            this.FacilityBooking_CPartnerCompany.Clear();

            //database.ExecuteStoreCommand("UPDATE Facility SET IncomingFacilityID = NULL WHERE IncomingFacilityID={0}", this.FacilityID);
            foreach (OutOffer outOff in this.OutOffer_CustomerCompany.ToArray())
            {
                outOff.CustomerCompany = null;
            }
            this.OutOffer_CustomerCompany.Clear();

            //database.ExecuteStoreCommand("UPDATE Facility SET IncomingFacilityID = NULL WHERE IncomingFacilityID={0}", this.FacilityID);
            foreach (OutOrder outOrder in this.OutOrder_CPartnerCompany.ToArray())
            {
                outOrder.CPartnerCompany = null;
            }
            this.OutOrder_CPartnerCompany.Clear();

            //database.ExecuteStoreCommand("UPDATE Facility SET IncomingFacilityID = NULL WHERE IncomingFacilityID={0}", this.FacilityID);
            foreach (OutOrder outOrder in this.OutOrder_CustomerCompany.ToArray())
            {
                outOrder.CustomerCompany = null;
            }
            this.OutOrder_CustomerCompany.Clear();


            //database.ExecuteStoreCommand("UPDATE Facility SET IncomingFacilityID = NULL WHERE IncomingFacilityID={0}", this.FacilityID);
            foreach (Visitor visitor in this.Visitor_VisitedCompany.ToArray())
            {
                visitor.VisitedCompany = null;
            }
            this.Visitor_VisitedCompany.Clear();

            //database.ExecuteStoreCommand("UPDATE Facility SET IncomingFacilityID = NULL WHERE IncomingFacilityID={0}", this.FacilityID);
            foreach (Visitor visitor in this.Visitor_VisitorCompany.ToArray())
            {
                visitor.VisitorCompany = null;
            }
            this.Visitor_VisitorCompany.Clear();

            //database.ExecuteStoreCommand("UPDATE Facility SET IncomingFacilityID = NULL WHERE IncomingFacilityID={0}", this.FacilityID);
            foreach (VisitorVoucher visitor in this.VisitorVoucher_VisitorCompany.ToArray())
            {
                visitor.VisitorCompany = null;
            }
            this.VisitorVoucher_VisitorCompany.Clear();

            database.DeleteObject(this);
            return null;
        }

        /// <summary>
        /// Thread-critical: If Entity is from GlobalDatabase-context, then lock operation together with SaveChanges!!!
        /// </summary>
        /// <param name="database">The database.</param>
        /// <param name="withCheck">if set to <c>true</c> [with check].</param>
        /// <returns>Msg.</returns>
        public Msg DeleteCompanyRecursive(Database database, bool withCheck)
        {
            if (withCheck)
            {
                Msg msg = IsEnabledDeleteRecursive(database);
                if (msg != null)
                    return msg;
            }

            DeleteCompanyRecursiveInternal(database);
            return null;
        }

        /// <summary>
        /// Deletes the AC class Recursive internal.
        /// </summary>
        /// <param name="database">The database.</param>
        void DeleteCompanyRecursiveInternal(Database database)
        {
            if (Company_ParentCompany.Count > 0)
            {
                foreach (var companyChild in Company_ParentCompany.ToList())
                {
                    companyChild.DeleteCompanyRecursiveInternal(database);
                }
            }

            DeleteACObject(database, false);
        }


        /// <summary>
        /// Determines whether [is enabled delete Recursive] [the specified database].
        /// </summary>
        /// <param name="database">The database.</param>
        public Msg IsEnabledDeleteRecursive(Database database)
        {
            // Diese Klasse prüfen
            MsgWithDetails msg = IsEnabledDeleteACObject(database);
            if (msg != null)
                return msg;

            // Unter-ACClass überprüfen
            foreach (var companyChild in Company_ParentCompany)
            {
                Msg msg2 = companyChild.IsEnabledDeleteRecursive(database);
                if (msg2 != null)
                    return msg2;
            }

            return null;
        }

        #endregion

        #region Hilfsfunktionen
        [ACPropertyInfo(9999)]
        public CompanyAddress HouseCompanyAddress
        {
            get
            {
                CompanyAddress address = this.CompanyAddress_Company.Where(c => c.IsHouseCompanyAddress).FirstOrDefault();
                if (address != null)
                    return address;
                return CompanyAddress_Company.Where(c => c.EntityState == System.Data.EntityState.Added && c.IsHouseCompanyAddress == true).FirstOrDefault();
            }
        }

        [ACPropertyInfo(9999)]
        public CompanyAddress BillingCompanyAddress
        {
            get
            {
                CompanyAddress address = this.CompanyAddress_Company.Where(c => c.IsBillingCompanyAddress).FirstOrDefault();
                if (address != null)
                    return address;
                return CompanyAddress_Company.Where(c => c.EntityState == System.Data.EntityState.Added && c.IsBillingCompanyAddress == true).FirstOrDefault();
            }
        }

        [ACPropertyInfo(9999)]
        public CompanyAddress DeliveryCompanyAddress
        {
            get
            {
                CompanyAddress address = this.CompanyAddress_Company.Where(c => c.IsDeliveryCompanyAddress).FirstOrDefault();
                if (address != null)
                    return address;
                return CompanyAddress_Company.Where(c => c.EntityState == System.Data.EntityState.Added && c.IsDeliveryCompanyAddress == true).FirstOrDefault();
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
                return CompanyNo + " " + CompanyName;
            }
        }

        public override IACObject GetChildEntityObject(string className, params string[] filterValues)
        {
            if (filterValues.Any())
            {
                switch (className)
                {
                    case CompanyAddress.ClassName:
                        return this.CompanyAddress_Company.Where(c => c.Name1 == filterValues[0]).FirstOrDefault();
                    case CompanyMaterial.ClassName:
                        return this.CompanyMaterial_Company.Where(c => c.CompanyMaterialNo == filterValues[0]).FirstOrDefault();
                    case CompanyPerson.ClassName:
                        return this.CompanyPerson_Company.Where(c => c.Name1 == filterValues[0]).FirstOrDefault();
                }
            }
            return null;
        }

        #endregion

        #region IACObjectEntity Members

        static public string KeyACIdentifier
        {
            get
            {
                return "CompanyNo";
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
