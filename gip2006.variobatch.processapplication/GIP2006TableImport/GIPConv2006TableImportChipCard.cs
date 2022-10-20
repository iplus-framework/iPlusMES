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
    /// 1. Import LKW
    /// 2. Import ChipCard
    /// </summary>
    public class GIPConv2006TableImportChipCard : GIPConv2006TableImportClass
    {
        const string _FileName = "ChipCard.txt";
        public override string FileName
        {
	        get 
            {
                return _FileName;
            }
        }

        public override object ReadLine(DatabaseApp dbApp, DataRow line, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            string chipNo = line["Nr"].ToString();
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref chipNo))
                return null;
            return ReadChipCardFields(dbApp, chipNo, line.ItemArray, row, fields, bgWorker);
        }

        public override object ReadLine(DatabaseApp dbApp, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            int iNr = fields.IndexOf("Nr");
            if (iNr < 0)
                return null;
            string chipNo = values[iNr] as string;
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref chipNo))
                return null;
            return ReadChipCardFields(dbApp, chipNo, values, row, fields, bgWorker);
        }

        protected virtual MDVisitorCard ReadChipCardFields(DatabaseApp dbApp, string chipNo, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            MDVisitorCard mdVisitorCard = dbApp.MDVisitorCard.Where(c => c.MDVisitorCardNo == chipNo).FirstOrDefault();
            if (mdVisitorCard == null)
            {
                mdVisitorCard = MDVisitorCard.NewACObject(dbApp, null);
                mdVisitorCard.MDVisitorCardNo = chipNo;
                dbApp.MDVisitorCard.AddObject(mdVisitorCard);
            }

            Visitor currentVisitor = null;
            string driverName = "";

            // TODO: @aagincic: Rewrite progress - ReadChipCardFields()
            //if (bgWorker != null)
            //    bgWorker.ProgressInfo.SubProgressText = "Nr: " + chipNo;

            int pos = 0;
            foreach (string v in values)
            {
                string value = v.ToString();
                string fieldName = fields[pos];
                pos++;
                if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref value))
                    continue;
                if (value == "NULL" || value == "Null" || value == "null")
                    continue;

                switch (fieldName)
                {
                    case "Nr": //	string (20)
                        break;
                    case "IsFixDriverCard": //	integer (2)
                        break;
                    case "LKW": //	Z-string (41)
                        MDFacilityType facilityTypeVehicle = GIPConv2006TableImportLKW.GetFacilityTypeVehicle(dbApp);
                        if (facilityTypeVehicle != null)
                        {
                            Facility facilityVehicle = dbApp.Facility.Where(c => c.MDFacilityTypeID == facilityTypeVehicle.MDFacilityTypeID && c.FacilityNo == value).FirstOrDefault();
                            if (facilityVehicle != null)
                            {
                                var query = dbApp.Visitor.Where(c => c.VehicleFacilityID == facilityVehicle.FacilityID && !c.IsFinished);
                                foreach (Visitor visitor in query)
                                {
                                    currentVisitor = visitor;
                                    visitor.MDVisitorCard = mdVisitorCard;
                                }
                                if (currentVisitor == null)
                                {
                                    currentVisitor = Visitor.NewACObject(dbApp, null, "C" + mdVisitorCard.MDVisitorCardNo);
                                    currentVisitor.VehicleFacility = facilityVehicle;
                                    currentVisitor.MDVisitorCard = mdVisitorCard;
                                }
                            }
                        }
                        break;
                    case "ReadAtDeviceID1": //	Z-string (21)
                        break;
                    case "ReadAtDeviceID2": //	Z-string (21)
                        break;
                    case "TableID": //	integer (2)
                        break;
                    case "TableKey1": //	string (20)
                        break;
                    case "TableKey2": //	string (20)
                        break;
                    case "TableKey3": //	string (20)
                        break;
                    case "Status": //	integer (2)
                        break;
                    case Const.EntityUpdateDate:
                        break;
                    case "UpdateTime":
                        break;
                    case Const.EntityUpdateName:
                        mdVisitorCard.UpdateName = value.Length <= 5 ? value : value.Substring(0, 5);
                        break;
                    case Const.EntityInsertDate:
                        break;
                    case "InsertTime":
                        break;
                    case Const.EntityInsertName:
                        mdVisitorCard.InsertName = value.Length <= 5 ? value : value.Substring(0, 5);
                        break;
                    default:
                        break;
                }
            }

            if (!String.IsNullOrEmpty(driverName) && currentVisitor != null)
            {
                CompanyPerson driver = dbApp.CompanyPerson.Where(c => c.CompanyPersonNo == driverName).FirstOrDefault();
                if (driver != null)
                {
                    currentVisitor.VisitorCompanyPerson = driver;
                    currentVisitor.VisitorCompany = currentVisitor.VisitorCompanyPerson.Company;
                }
            }
            return mdVisitorCard;
        }
    }
}
