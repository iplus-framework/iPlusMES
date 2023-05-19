using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gip.core.datamodel;
using gip.core.autocomponent;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Threading;
using System.Data.SqlClient;
using System.IO;
using gip.mes.datamodel;
using System.Data;
using System.Globalization;

namespace gip2006.variobatch.processapplication
{
    /// <summary>
    /// Reihenfolge Import
    /// 1. Import Material
    /// 2. Import Adressen
    /// 3. Import AdressenLief
    /// 4. Import AbladeStelle
    /// 5. Import ChipCardLog (Kombination aus LKKW und Fahrer)
    /// 6. Import Adressen, damit  Voucher 
    /// </summary>
    public class GIPConv2006TableImportAdressen : GIPConv2006TableImportClass
    {
        const string _FileName = "Adressen.txt";
        public override string FileName
        {
	        get 
            {
                return _FileName;
            }
        }

        public override object ReadLine(DatabaseApp dbApp, DataRow line, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            string addressNo = line["AdressID"].ToString();
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref addressNo))
                return null;
            string addressTyp = line["AdressTyp"].ToString();
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref addressTyp))
                return null;
            string name1 = line["Name1"].ToString();
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref name1))
                return null;

            return ReadAdressenFields(dbApp, addressNo, Convert.ToInt32(addressTyp), line.ItemArray, row, fields, bgWorker);
        }

        public override object ReadLine(DatabaseApp dbApp, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            int iNr = fields.IndexOf("AdressID");
            if (iNr < 0)
                return null;
            string addressNo = values[iNr] as string;
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref addressNo))
                return null;

            iNr = fields.IndexOf("AdressTyp");
            if (iNr < 0)
                return null;
            string addressTyp = values[iNr] as string;
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref addressTyp))
                return null;

            iNr = fields.IndexOf("Name1");
            if (iNr < 0)
                return null;
            string name1 = values[iNr] as string;
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref name1))
                return null;

            return ReadAdressenFields(dbApp, addressNo, Convert.ToInt32(addressTyp), values, row, fields, bgWorker);
        }

        protected virtual Company ReadAdressenFields(DatabaseApp dbApp, string addressNo, int addressTyp, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            // :Lieferant:Kunde:Spedition:Lieferant+Kunde:Lieferant+Spedition:Kunde+Spedition:Lieferant+Kunde+Spedition:Fahrer:Firma:BVD_Empfänger:
            bool isNewCompany = false;
            Company company = null;
            if (addressTyp != 8)
                company = dbApp.Company.Where(c => c.CompanyNo == addressNo).FirstOrDefault();
            else // Fahrer, Benutzer muss später den Fahrer der entsprechenden Firma im BSOCompany zuordnen
                company = dbApp.Company.Where(c => c.IsOwnCompany).FirstOrDefault();
            if (company == null)
            {
                string secondaryKey = addressTyp != 8 ? addressNo : "OwnCompany";
                company = Company.NewACObject(dbApp, null, secondaryKey);
                if (addressTyp == 8)
                {
                    company.IsOwnCompany = true;
                    company.CompanyName = "OwnCompany";
                    company.HouseCompanyAddress.Name1 = "OwnCompany";
                }
                isNewCompany = true;

                dbApp.Company.Add(company);
            }

            switch (addressTyp)
            {
                case 1: //Lieferant
                    company.IsCustomer = false;
                    company.IsDistributor = true;
                    company.IsDistributorLead = true;
                    company.IsSalesLead = false;
                    company.IsShipper = false;
                    break;
                case 2: // Kunde
                    company.IsCustomer = true;
                    company.IsDistributor = false;
                    company.IsDistributorLead = false;
                    company.IsSalesLead = false;
                    company.IsShipper = false;
                    break;
                case 3: // Spedition
                    company.IsCustomer = false;
                    company.IsDistributor = false;
                    company.IsDistributorLead = false;
                    company.IsSalesLead = false;
                    company.IsShipper = true;
                    break;
                case 4: // Lieferant+Kunde
                    company.IsCustomer = true;
                    company.IsDistributor = true;
                    company.IsDistributorLead = true;
                    company.IsSalesLead = false;
                    company.IsShipper = false;
                    break;
                case 5: // Lieferant+Spedition
                    company.IsCustomer = false;
                    company.IsDistributor = true;
                    company.IsDistributorLead = true;
                    company.IsSalesLead = false;
                    company.IsShipper = true;
                    break;
                case 6: // Kunde+Spedition
                    company.IsCustomer = true;
                    company.IsDistributor = false;
                    company.IsDistributorLead = false;
                    company.IsSalesLead = false;
                    company.IsShipper = true;
                    break;
                case 7: // Lieferant+Kunde+Spedition
                    company.IsCustomer = true;
                    company.IsDistributor = true;
                    company.IsDistributorLead = true;
                    company.IsSalesLead = false;
                    company.IsShipper = true;
                    break;
                case 8: // Fahrer
                    break;
                default: // 9: Firma, 10: BVD_Empfänger
                    company.IsCustomer = false;
                    company.IsDistributor = false;
                    company.IsDistributorLead = false;
                    company.IsSalesLead = false;
                    company.IsShipper = false;
                    break;
            }

            // Fahrer
            CompanyPerson companyPerson = null;
            if (addressTyp == 8)
            {
                if (!isNewCompany)
                    companyPerson = company.CompanyPerson_Company.Where(c => c.CompanyPersonNo == addressNo).FirstOrDefault();
                if (companyPerson == null)
                {
                    companyPerson = CompanyPerson.NewACObject(dbApp, company, addressNo);
                }
            }
            // TODO: @aagincic: Rewrite progress - ReadAdressenFields()
            //if (bgWorker != null)
            //    bgWorker.ProgressInfo.SubProgressText = "Nr: " + addressNo;

            int pos = 0;
            foreach (string v in values)
            {
                string value = v.ToString();
                string fieldName = fields[pos];
                pos++;
                if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref value))
                    continue;

                switch (fieldName)
                {
                    case "AdressID": //	integer (4)
                        break;
                    case "AdressTyp": //	integer (4)
                        break;
                    case "EU": //	integer (2)
                        break;
                    case "Fax": //	string (20)
                        if (addressTyp == 8)
                            companyPerson.Fax = value;
                        else
                            company.HouseCompanyAddress.Fax = value;
                        break;
                    case "HinweisExtern": //	Z-string (101)
                        if (addressTyp != 8)
                            company.NoteExternal = value;
                            break;
                    case "HinweisIntern": //	Z-string (101)
                        if (addressTyp != 8)
                            company.NoteInternal = value;
                        break;
                    case "Name1": //	string (40)
                        if (addressTyp == 8)
                            companyPerson.Name1 = value;
                        else
                        {
                            company.CompanyName = value;
                            company.HouseCompanyAddress.Name1 = value;
                        }
                        break;
                    case "Name2": //	string (40)
                        if (addressTyp == 8)
                            companyPerson.Name2 = value;
                        else
                            company.HouseCompanyAddress.Name2 = value;
                        break;
                    case "Name3": //	string (40)
                        if (addressTyp == 8)
                            companyPerson.Name3 = value;
                        else
                            company.HouseCompanyAddress.Name3 = value;
                        break;
                    case "Ort": //	string (40)
                        if (addressTyp == 8)
                            companyPerson.City = value;
                        else
                            company.HouseCompanyAddress.City = value;
                        break;
                    case "PLZ": //	string (10)
                        if (addressTyp == 8)
                            companyPerson.Postcode = value;
                        else
                            company.HouseCompanyAddress.Postcode = value;
                        break;
                    case "Postfach": //	string (10)
                        if (addressTyp == 8)
                            companyPerson.PostOfficeBox = value;
                        else
                            company.HouseCompanyAddress.PostOfficeBox = value;
                        break;
                    case "RAdressRefID": //	integer (4)
                        break;
                    case "RBuchhaltungsNr": //	string (20)
                        if (addressTyp != 8)
                            company.BillingAccountNo = value;
                        break;
                    case "RefID": //	integer (4)
                        break;
                    case "RZahlungsziel": //	string (10)
                        if (addressTyp != 8)
                        {
                            //company.BillingMDTermOfPayment = TODO
                        }
                        break;
                    case "Strasse": //	string (40)
                        if (addressTyp == 8)
                            companyPerson.Street = value;
                        else
                            company.HouseCompanyAddress.Street = value;
                        break;
                    case "Telefon": //	string (20)
                        if (addressTyp == 8)
                            companyPerson.Phone = value;
                        else
                            company.HouseCompanyAddress.Phone = value;
                        break;
                    case "VerwendeRNr": //	integer (2)
                        if (addressTyp != 8)
                            company.UseBillingAccountNo = value == "1" ? true : false;
                        break;
                    case "VerwendeWNr": //	integer (2)
                        if (addressTyp != 8)
                            company.UseShippingAccountNo = value == "1" ? true : false;
                        break;
                    case "WAdressRefID": //	integer (4)
                        break;
                    case "WBuchhaltungsNr": //	string (20)
                        if (addressTyp != 8)
                            company.ShippingAccountNo = value;
                        break;
                    case "WZahlungsziel": //	string (10)
                        if (addressTyp != 8)
                        {
                            //company.ShippingMDTermOfPayment = TODO
                        }
                        break;
                    case Const.EntityUpdateDate:
                        break;
                    case "UpdateTime":
                        break;
                    case Const.EntityUpdateName:
                        company.UpdateName = value.Length <= 5 ? value : value.Substring(0, 5);
                        break;
                    case Const.EntityInsertDate:
                        break;
                    case "InsertTime":
                        break;
                    case Const.EntityInsertName:
                        company.InsertName = value.Length <= 5 ? value : value.Substring(0, 5);
                        break;
                    default:
                        break;
                }
            }
            return company;
        }
    }


    public class GIPConv2006TableImportAdressenLief : GIPConv2006TableImportClass
    {
        const string _FileName = "AdressenLief.txt";
        public override string FileName
        {
	        get 
            {
                return _FileName;
            }
        }

        public override object ReadLine(DatabaseApp dbApp, DataRow line, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            return null;
            //string vehicleNo = line["Nr"].ToString();
            //if (IsImportStringNullOrEmpty(ref vehicleNo))
            //    return null;
            //string compartmentNo = line["SiloNr"].ToString();
            //if (IsImportStringNullOrEmpty(ref compartmentNo))
            //    return null;
            //return ReadAdressenLiefFields(dbApp, vehicleNo, compartmentNo, line.ItemArray, row, fields, bgWorker);
        }

        public override object ReadLine(DatabaseApp dbApp, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            return null;
            //int iNr = fields.IndexOf("Nr");
            //if (iNr < 0)
            //    return null;
            //string vehicleNo = values[iNr] as string;
            //if (IsImportStringNullOrEmpty(ref vehicleNo))
            //    return null;
            //vehicleNo = vehicleNo.Trim(' ', '"');
            //return ReadAdressenLiefFields(dbApp, vehicleNo, compartmentNo, values, row, fields, bgWorker);
        }

        protected virtual CompanyAddress ReadAdressenLiefFields(DatabaseApp dbApp, string vehicleNo, string compartmentNo, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            return null;
            //MDFacilityType facilityTypeVehicle = GetFacilityTypeVehicle(db);
            //if (facilityTypeVehicle == null)
            //    return null;
            //Facility facilityVehicle = db.Facility.Where(c => c.FacilityNo == vehicleNo && c.MDFacilityType == facilityTypeVehicle).FirstOrDefault();
            //if (facilityVehicle == null)
            //    return null;

            //MDFacilityType facilityTypeVehicleContainer = GetFacilityTypeVehicleContainer(db);
            //if (facilityTypeVehicleContainer == null)
            //    return null;
            //string vehicleContainerNo = vehicleNo + ": " + compartmentNo;
            //Facility facilityVehicleContainer = facilityVehicle.Facility_ParentFacility.Where(c => c.FacilityNo == vehicleContainerNo && c.MDFacilityType == facilityTypeVehicleContainer).FirstOrDefault();
            //if (facilityVehicleContainer == null)
            //{
            //    facilityVehicleContainer = Facility.NewACObject(db, facilityVehicle);
            //    facilityVehicleContainer.FacilityNo = vehicleContainerNo;
            //    facilityVehicleContainer.MDFacilityType = facilityTypeVehicleContainer;
            //}

            //if (bgWorker != null)
            //    bgWorker.ProgressInfo.SubProgressText = "Nr: " + vehicleContainerNo;

            //int pos = 0;
            //foreach (string v in values)
            //{
            //    string value = v.ToString();
            //    string fieldName = fields[pos];
            //    pos++;
            //    if (IsImportStringNullOrEmpty(ref value))
            //        continue;
            //    switch (fieldName)
            //    {
            //        case "Nr": //	Z-string (41)
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //return facilityVehicle;
        }
    }

    public class GIPConv2006TableImportAbladeStelle : GIPConv2006TableImportClass
    {
        const string _FileName = "AbladeStelle.txt";
        public override string FileName
        {
	        get 
            {
                return _FileName;
            }
        }

        public override object ReadLine(DatabaseApp dbApp, DataRow line, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            return null;
            //string vehicleNo = line["Nr"].ToString();
            //if (IsImportStringNullOrEmpty(ref vehicleNo))
            //    return null;
            //string compartmentNo = line["SiloNr"].ToString();
            //if (IsImportStringNullOrEmpty(ref compartmentNo))
            //    return null;
            //string lidNo = line["DomDeckelNr"].ToString();
            //if (IsImportStringNullOrEmpty(ref lidNo))
            //    return null;
            //return ReadAbladeStelleFields(db, vehicleNo, compartmentNo, lidNo, line.ItemArray, row, fields, bgWorker);
        }

        public override object ReadLine(DatabaseApp dbApp, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            return null;
            //int iNr = fields.IndexOf("Nr");
            //if (iNr < 0)
            //    return null;
            //string vehicleNo = values[iNr] as string;
            //if (IsImportStringNullOrEmpty(ref vehicleNo))
            //    return null;

            //int iSiloNr = fields.IndexOf("SiloNr");
            //if (iSiloNr < 0)
            //    return null;
            //string compartmentNo = values[iSiloNr] as string;
            //if (IsImportStringNullOrEmpty(ref compartmentNo))
            //    return null;

            //int iLidNr = fields.IndexOf("DomDeckelNr");
            //if (iLidNr < 0)
            //    return null;
            //string lidNo = values[iLidNr] as string;
            //if (IsImportStringNullOrEmpty(ref lidNo))
            //    return null;

            //return ReadAbladeStelleFields(db, vehicleNo, compartmentNo, lidNo, values, row, fields, bgWorker);
        }

        protected virtual CompanyAddressUnloadingpoint ReadAbladeStelleFields(DatabaseApp dbApp, string vehicleNo, string compartmentNo, string lidNo, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            return null;
            //MDFacilityType facilityTypeVehicle = GetFacilityTypeVehicle(db);
            //if (facilityTypeVehicle == null)
            //    return null;
            //Facility facilityVehicle = db.Facility.Where(c => c.FacilityNo == vehicleNo && c.MDFacilityType == facilityTypeVehicle).FirstOrDefault();
            //if (facilityVehicle == null)
            //    return null;

            //MDFacilityType facilityTypeVehicleContainer = GetFacilityTypeVehicleContainer(db);
            //if (facilityTypeVehicleContainer == null)
            //    return null;
            //string vehicleContainerNo = vehicleNo + ": " + compartmentNo;
            //Facility facilityVehicleContainer = facilityVehicle.Facility_ParentFacility.Where(c => c.FacilityNo == vehicleContainerNo && c.MDFacilityType == facilityTypeVehicleContainer).FirstOrDefault();
            //if (facilityVehicleContainer == null)
            //    return null;

            //string vehicleLidNo = vehicleContainerNo + ": " + lidNo;
            //Facility facilityVehicleLid = facilityVehicleContainer.Facility_ParentFacility.Where(c => c.FacilityNo == vehicleLidNo && c.MDFacilityType == facilityTypeVehicleContainer).FirstOrDefault();
            //if (facilityVehicleLid == null)
            //{
            //    facilityVehicleLid = Facility.NewACObject(db, facilityVehicleContainer);
            //    facilityVehicleLid.FacilityNo = vehicleLidNo;
            //    facilityVehicleLid.MDFacilityType = facilityTypeVehicleContainer;
            //}

            //if (bgWorker != null)
            //    bgWorker.ProgressInfo.SubProgressText = "Nr: " + vehicleLidNo;

            //int pos = 0;
            //foreach (string v in values)
            //{
            //    string value = v..ToString()
            //    string fieldName = fields[pos];
            //    pos++;
            //    if (IsImportStringNullOrEmpty(ref value))
            //        continue;
            //    switch (fieldName)
            //    {
            //        case "Nr": //	Z-string (41)
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //return facilityVehicle;
        }
    }
}
