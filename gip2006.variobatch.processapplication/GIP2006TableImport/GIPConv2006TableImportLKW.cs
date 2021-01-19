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
    /// 2. Import von Adressen / Company muss zuvor durchgeführt werden
    /// 3. Import LKW
    /// 4. Import LKWKomp
    /// 5. Import LKWDeckel
    /// </summary>
    public class GIPConv2006TableImportLKW : GIPConv2006TableImportClass
    {
        const string _FileName = "LKW.txt";
        public override string FileName
        {
	        get 
            {
                return _FileName;
            }
        }

        public override object ReadLine(DatabaseApp dbApp, DataRow line, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            string vehicleNo = line["Nr"].ToString();
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref vehicleNo))
                return null;
            return ReadLKWFields(dbApp, vehicleNo, line.ItemArray, row, fields, bgWorker);
        }

        public override object ReadLine(DatabaseApp dbApp, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            int iNr = fields.IndexOf("Nr");
            if (iNr < 0)
                return null;
            string vehicleNo = values[iNr] as string;
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref vehicleNo))
                return null;

            return ReadLKWFields(dbApp, vehicleNo, values, row, fields, bgWorker);
        }

        protected virtual Facility ReadLKWFields(DatabaseApp dbApp, string vehicleNo, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            MDFacilityType facilityTypeVehicle = GetFacilityTypeVehicle(dbApp);
            if (facilityTypeVehicle == null)
                return null;
            Facility facilityVehicle = dbApp.Facility.Where(c => c.FacilityNo == vehicleNo && c.MDFacilityTypeID == facilityTypeVehicle.MDFacilityTypeID).FirstOrDefault();
            if (facilityVehicle == null)
            {
                facilityVehicle = Facility.NewACObject(dbApp, null);
                facilityVehicle.FacilityNo = vehicleNo.Length <= 20 ? vehicleNo : vehicleNo.Substring(0,20);
                facilityVehicle.FacilityName = vehicleNo.Length <= 40 ? vehicleNo : vehicleNo.Substring(0, 40);
                facilityVehicle.MDFacilityType = facilityTypeVehicle;
                dbApp.Facility.AddObject(facilityVehicle);
            }
            // TODO: @aagincic: Rewrite progress - ReadLKWFields()
            //if (bgWorker != null)
            //    bgWorker.ProgressInfo.SubProgressText = "Nr: " + vehicleNo;

            string driverName = "";
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
                    case "Nr": //	Z-string (41)
                        break;
                    case "Typ": //	integer (2)
                        short vehicleTypeNum = Convert.ToInt16(value);
                        MDFacilityVehicleType.FacilityVehicleTypes vehicleType = MDFacilityVehicleType.FacilityVehicleTypes.BallastTractor;
                        if (vehicleTypeNum == 1)
                            vehicleType = MDFacilityVehicleType.FacilityVehicleTypes.SemiTrailerTruck;
                        else if (vehicleTypeNum == 2)
                            vehicleType = MDFacilityVehicleType.FacilityVehicleTypes.SemiTrailer;
                        else if (vehicleTypeNum == 3)
                            vehicleType = MDFacilityVehicleType.FacilityVehicleTypes.BallastTractor;
                        else if (vehicleTypeNum == 4)
                            vehicleType = MDFacilityVehicleType.FacilityVehicleTypes.FullTrailer;
                        else if (vehicleTypeNum == 5)
                            vehicleType = MDFacilityVehicleType.FacilityVehicleTypes.CalibrationVehicle;
                        MDFacilityVehicleType mdVehicleType = dbApp.MDFacilityVehicleType.Where(c => c.MDFacilityVehicleTypeIndex == (short)vehicleType).FirstOrDefault();
                        if (mdVehicleType == null)
                            dbApp.MDFacilityVehicleType.FirstOrDefault();
                        facilityVehicle.MDFacilityVehicleType = mdVehicleType;
                        break;
                    case "AnhaengerNr": // Z-string (41) // TODO: Im zweiten Durchlauf als VisitorVoucher anlegen und Zugmaschine und Hänger zuordnen.
                        break;
                    case "MaxGewicht": //	IEEE floating point (8)
                        facilityVehicle.MaxWeightCapacity = Convert.ToDouble(value);
                        break;
                    case "MaxInhalt": //	IEEE floating point (8)
                        facilityVehicle.MaxVolumeCapacity = Convert.ToDouble(value);
                        break;
                    case "Spediteur": //	integer (4)
                        Company companyShipper = dbApp.Company.Where(c => c.IsShipper == true && c.CompanyNo == value).FirstOrDefault();
                        if (companyShipper != null)
                            facilityVehicle.Company = companyShipper;
                        break;
                    case "Bemerkung": //	Z-string (101)
                        facilityVehicle.Comment = value;
                        break;
                    case "SuchNr": //	Z-string (41)
                        //facilityVehicle.
                        break;
                    case "Tara": //	IEEE floating point (8)
                        facilityVehicle.Tara = Convert.ToDouble(value);
                        break;
                    case "TransponderNr1": //	integer (4)
                        break;
                    case "TransponderNr2": //	integer (4)
                        break;
                    case "Fahrer": //	Z-string (61)
                        driverName = value;
                        break;
                    case Const.EntityUpdateDate:
                        break;
                    case "UpdateTime":
                        break;
                    case Const.EntityUpdateName:
                        facilityVehicle.UpdateName = value.Length <= 5 ? value : value.Substring(0, 5);
                        break;
                    case Const.EntityInsertDate:
                        break;
                    case "InsertTime":
                        break;
                    case Const.EntityInsertName:
                        facilityVehicle.InsertName = value.Length <= 5 ? value : value.Substring(0, 5);
                        break;
                    default:
                        break;
                }
            }

            if (!String.IsNullOrEmpty(driverName) && facilityVehicle.Company != null)
            {
                CompanyPerson driver = facilityVehicle.Company.CompanyPerson_Company.Where(c => c.Name1 == driverName || c.CompanyPersonNo == driverName).FirstOrDefault();
                if (driver == null)
                {
                    //driver = CompanyPerson.NewACObject(db, facilityVehicle.Company);
                    //driver.Name1 = driverName;
                }
                facilityVehicle.CompanyPerson = driver;
            }
            return facilityVehicle;
        }

        public static MDFacilityType GetFacilityTypeVehicle(DatabaseApp dbApp)
        {
            return dbApp.MDFacilityType.Where(c => c.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.Vehicle).FirstOrDefault();
        }
    }

    public class GIPConv2006TableImportLKWKomp : GIPConv2006TableImportClass
    {
        const string _FileName = "LKWKomp.txt";
        public override string FileName
        {
	        get 
            {
                return _FileName;
            }
        }

        public override object ReadLine(DatabaseApp dbApp, DataRow line, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            string vehicleNo = line["Nr"].ToString();
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref vehicleNo))
                return null;
            string compartmentNo = line["SiloNr"].ToString();
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref compartmentNo))
                return null;
            return ReadLKWKompFields(dbApp, vehicleNo, compartmentNo, line.ItemArray, row, fields, bgWorker);
        }

        public override object ReadLine(DatabaseApp dbApp, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            int iNr = fields.IndexOf("Nr");
            if (iNr < 0)
                return null;
            string vehicleNo = values[iNr] as string;
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref vehicleNo))
                return null;

            int iSiloNr = fields.IndexOf("SiloNr");
            if (iSiloNr < 0)
                return null;
            string compartmentNo = values[iSiloNr] as string;
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref compartmentNo))
                return null;

            return ReadLKWKompFields(dbApp, vehicleNo, compartmentNo, values, row, fields, bgWorker);
        }

        protected virtual Facility ReadLKWKompFields(DatabaseApp dbApp, string vehicleNo, string compartmentNo, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            MDFacilityType facilityTypeVehicle = GIPConv2006TableImportLKW.GetFacilityTypeVehicle(dbApp);
            if (facilityTypeVehicle == null)
                return null;
            Facility facilityVehicle = dbApp.Facility.Where(c => c.FacilityNo == vehicleNo && c.MDFacilityTypeID == facilityTypeVehicle.MDFacilityTypeID).FirstOrDefault();
            if (facilityVehicle == null)
                return null;

            MDFacilityType facilityTypeVehicleContainer = GetFacilityTypeVehicleContainer(dbApp);
            if (facilityTypeVehicleContainer == null)
                return null;
            string vehicleContainerNo = vehicleNo + ": " + compartmentNo;
            if (vehicleContainerNo.Length > 40)
                return null;
            Facility facilityVehicleContainer = facilityVehicle.Facility_ParentFacility.Where(c => c.FacilityNo == compartmentNo && c.MDFacilityType == facilityTypeVehicleContainer).FirstOrDefault();
            if (facilityVehicleContainer == null)
            {
                facilityVehicleContainer = Facility.NewACObject(dbApp, facilityVehicle);
                facilityVehicleContainer.FacilityNo = compartmentNo.Length <= 20 ? compartmentNo : compartmentNo.Substring(0, 20);
                facilityVehicleContainer.FacilityName = vehicleContainerNo.Length <= 40 ? vehicleContainerNo : vehicleContainerNo.Substring(0, 40);
                facilityVehicleContainer.MDFacilityType = facilityTypeVehicleContainer;
            }

            // TODO: @aagincic: Rewrite progress ReadLKWKompFields()
            //if (bgWorker != null)
            //    bgWorker.ProgressInfo.SubProgressText = "Nr: " + vehicleContainerNo;

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
                    case "Nr": //	Z-string (41)
                        break;
                    case "SiloNr": //	integer (4)
                        break;
                    case "ArtikelNr": //	string (20)
                        facilityVehicleContainer.Material = dbApp.Material.Where(c => c.MaterialNo == value).FirstOrDefault();
                        break;
                    case "Folge": //	integer (4)
                        facilityVehicleContainer.LastFCSortNo = Convert.ToInt32(value);
                        break;
                    case "MaxGewicht": //	IEEE floating point (8)
                        facilityVehicleContainer.MaxWeightCapacity = Convert.ToDouble(value);
                        break;
                    case "MaxInhalt": //	IEEE floating point (8)
                        facilityVehicleContainer.MaxVolumeCapacity = Convert.ToDouble(value);
                        break;
                    case "Toleranz": //	IEEE floating point (8) [SQL-Nullable]
                        facilityVehicleContainer.Tolerance = Convert.ToSingle(value);
                        break;
                    case Const.EntityUpdateDate:
                        break;
                    case "UpdateTime":
                        break;
                    case Const.EntityUpdateName:
                        facilityVehicle.UpdateName = value.Length <= 5 ? value : value.Substring(0, 5);
                        break;
                    case Const.EntityInsertDate:
                        break;
                    case "InsertTime":
                        break;
                    case Const.EntityInsertName:
                        facilityVehicle.InsertName = value.Length <= 5 ? value : value.Substring(0, 5);
                        break;
                    default:
                        break;
                }
            }
            if (facilityVehicleContainer.LastFCSortNo <= 0)
            {
                try
                {
                    facilityVehicleContainer.LastFCSortNo = Convert.ToInt32(compartmentNo);
                }
                catch (Exception ec)
                {
                    string msg = ec.Message;
                    if (ec.InnerException != null && ec.InnerException.Message != null)
                        msg += " Inner:" + ec.InnerException.Message;

                    this.Root().Messages.LogException("GIPConv2006TableImportLKWKomp", "ReadLKWKompFields", msg);
                }
            }

            return facilityVehicle;
        }

        public static MDFacilityType GetFacilityTypeVehicleContainer(DatabaseApp dbApp)
        {
            return dbApp.MDFacilityType.Where(c => c.MDFacilityTypeIndex == (short)MDFacilityType.FacilityTypes.VehicleContainer).FirstOrDefault();
        }
    }

    public class GIPConv2006TableImportLKWDeckel : GIPConv2006TableImportClass
    {
        const string _FileName = "LKWDeckel.txt";
        public override string FileName
        {
	        get 
            {
                return _FileName;
            }
        }

        public override object ReadLine(DatabaseApp dbApp, DataRow line, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            string vehicleNo = line["Nr"].ToString();
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref vehicleNo))
                return null;
            string compartmentNo = line["SiloNr"].ToString();
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref compartmentNo))
                return null;
            string lidNo = line["DomDeckelNr"].ToString();
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref lidNo))
                return null;
            return ReadLKWDeckelFields(dbApp, vehicleNo, compartmentNo, lidNo, line.ItemArray, row, fields, bgWorker);
        }

        public override object ReadLine(DatabaseApp dbApp, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            int iNr = fields.IndexOf("Nr");
            if (iNr < 0)
                return null;
            string vehicleNo = values[iNr] as string;
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref vehicleNo))
                return null;

            int iSiloNr = fields.IndexOf("SiloNr");
            if (iSiloNr < 0)
                return null;
            string compartmentNo = values[iSiloNr] as string;
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref compartmentNo))
                return null;

            int iLidNr = fields.IndexOf("DomDeckelNr");
            if (iLidNr < 0)
                return null;
            string lidNo = values[iLidNr] as string;
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref lidNo))
                return null;

            return ReadLKWDeckelFields(dbApp, vehicleNo, compartmentNo, lidNo, values, row, fields, bgWorker);
        }

        protected virtual Facility ReadLKWDeckelFields(DatabaseApp dbApp, string vehicleNo, string compartmentNo, string lidNo, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            MDFacilityType facilityTypeVehicle = GIPConv2006TableImportLKW.GetFacilityTypeVehicle(dbApp);
            if (facilityTypeVehicle == null)
                return null;
            Facility facilityVehicle = dbApp.Facility.Where(c => c.FacilityNo == vehicleNo && c.MDFacilityTypeID == facilityTypeVehicle.MDFacilityTypeID).FirstOrDefault();
            if (facilityVehicle == null)
                return null;

            MDFacilityType facilityTypeVehicleContainer = GIPConv2006TableImportLKWKomp.GetFacilityTypeVehicleContainer(dbApp);
            if (facilityTypeVehicleContainer == null)
                return null;
            string vehicleContainerNo = vehicleNo + ": " + compartmentNo;
            Facility facilityVehicleContainer = facilityVehicle.Facility_ParentFacility.Where(c => c.FacilityNo == compartmentNo && c.MDFacilityTypeID == facilityTypeVehicleContainer.MDFacilityTypeID).FirstOrDefault();
            if (facilityVehicleContainer == null)
                return null;

            string vehicleLidNo = vehicleContainerNo + ": " + lidNo;
            if (vehicleLidNo.Length > 20)
                return null;
            Facility facilityVehicleLid = facilityVehicleContainer.Facility_ParentFacility.Where(c => c.FacilityNo == lidNo && c.MDFacilityTypeID == facilityTypeVehicleContainer.MDFacilityTypeID).FirstOrDefault();
            if (facilityVehicleLid == null)
            {
                facilityVehicleLid = Facility.NewACObject(dbApp, facilityVehicleContainer);
                facilityVehicleLid.FacilityNo = lidNo.Length <= 20 ? lidNo : lidNo.Substring(0, 20);
                facilityVehicleLid.FacilityName = vehicleLidNo.Length <= 40 ? vehicleLidNo : vehicleLidNo.Substring(0, 40);
                facilityVehicleLid.MDFacilityType = facilityTypeVehicleContainer;
            }

            // TODO: @aagincic: Rewrite progress ReadLKWDeckelFields()
            //if (bgWorker != null)
            //    bgWorker.ProgressInfo.SubProgressText = "Nr: " + vehicleLidNo;

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
                    case "Nr": //	Z-string (41)
                        break;
                    case "SiloNr": //	integer (4)
                        break;
                    case "DomDeckelNr": //	integer (4)
                        break;
                    case "Folge": //	integer (4)
                        facilityVehicleContainer.LastFCSortNo = Convert.ToInt32(value);
                        break;
                    case "MaxGewicht": //	IEEE floating point (8)
                        facilityVehicleContainer.MaxWeightCapacity = Convert.ToDouble(value);
                        break;
                    case "MaxInhalt": //	IEEE floating point (8)
                        facilityVehicleContainer.MaxVolumeCapacity = Convert.ToDouble(value);
                        break;
                    case "MassAbstandVor": //	IEEE floating point (8)
                        facilityVehicleContainer.FittingsDistanceFront = Convert.ToInt32(value);
                        break;
                    case "MassAbstandNach": //	IEEE floating point (8)
                        facilityVehicleContainer.FittingsDistanceBehind = Convert.ToInt32(value);
                        break;
                    case "GarniturAbstandVor": //	IEEE floating point (8)
                        facilityVehicleContainer.DistanceFront = Convert.ToDouble(value);
                        break;
                    case "GarniturAbstandNach": //	IEEE floating point (8)
                        facilityVehicleContainer.DistanceBehind = Convert.ToDouble(value);
                        break;
                    case Const.EntityUpdateDate:
                        break;
                    case "UpdateTime":
                        break;
                    case Const.EntityUpdateName:
                        facilityVehicle.UpdateName = value.Length <= 5 ? value : value.Substring(0, 5);
                        break;
                    case Const.EntityInsertDate:
                        break;
                    case "InsertTime":
                        break;
                    case Const.EntityInsertName:
                        facilityVehicle.InsertName = value.Length <= 5 ? value : value.Substring(0, 5);
                        break;
                    default:
                        break;
                }
            }
            return facilityVehicle;
        }
    }
}
