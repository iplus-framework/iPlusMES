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

namespace gip2006.variobatch.processapplication
{
    public class GIPConv2006TableImportArtikel : GIPConv2006TableImportClass
    {
        const string _FileName = "Artikel.txt";
        public override string FileName
        {
	        get 
            {
                return _FileName;
            }
        }

        public override object ReadLine(DatabaseApp dbApp, DataRow line, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            string materialNo = line["Nr"].ToString();
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref materialNo))
                return null;
            return ReadArtikelFields(dbApp, materialNo, line.ItemArray, row, fields, bgWorker);
        }

        public override object ReadLine(DatabaseApp dbApp, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            int iNr = fields.IndexOf("Nr");
            if (iNr < 0)
                return null;
            string materialNo = values[iNr] as string;
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref materialNo))
                return null;

            return ReadArtikelFields(dbApp, materialNo, values, row, fields, bgWorker);
        }

        protected virtual Material ReadArtikelFields(DatabaseApp dbApp, string materialNo, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            Material material = dbApp.Material.Where(c => c.MaterialNo == materialNo).FirstOrDefault();
            if (material == null)
            {
                material = Material.NewACObject(dbApp, null);
                material.MaterialNo = materialNo;
                dbApp.Material.AddObject(material);
            }

            // TODO: @aagincic: Rewrite progress -  ReadArtikelFields()
            //if (bgWorker != null)
            //    bgWorker.ProgressInfo.SubProgressText = "Nr: " + materialNo;

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
                    case "Nr": 
                        break; 
                    case "ArtikelRefID": 
                        break; 
                    case "Bez1": 
                        material.MaterialName1 = value;
                        break; 
                    case "Bez2": 
                        material.MaterialName2 = value;
                        break; 
                    case "Bez3": 
                        material.MaterialName3 = value;
                        break; 
                    case "ArtikelTyp": 
                        break; 
                    case "Wareneingangsart": 
                        break; 
                    case "ArtikelUeber": 
                        break; 
                    case "ArtikelUeberAnz": 
                        break; 
                    case "Mengengewicht": 
                        break; 
                    case "Gewichtseinheit": 
                        break;
                    case "BasisMengeneinheit":
                        MDUnit unit = null;
                        if (value == "2")
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "TNE").FirstOrDefault();
                        else if (value == "7")
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "CT").FirstOrDefault();
                        else if (value == "8")
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "SA(25)").FirstOrDefault();
                        else if (value == "9")
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "43").FirstOrDefault();
                        else if (value == "10")
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "CA").FirstOrDefault();
                        else if (value == "17")
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "MTR").FirstOrDefault();
                        else if (value == "18")
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "LTR").FirstOrDefault();
                        else if (value == "20")
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "MIN").FirstOrDefault();
                        else if (value == "22")
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "HUR").FirstOrDefault();
                        else if (value == "23" || value == "24")
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "BX").FirstOrDefault();
                        else if (value == "25")
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "CL").FirstOrDefault();
                        else if (value == "26")
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "D97").FirstOrDefault();
                        else
                            unit = dbApp.MDUnit.Where(c => c.ISOCode == "KGM").FirstOrDefault();
                        material.BaseMDUnit = unit;
                        break; 
                    case "MengeBasis2": 
                        break; 
                    case "Mengeneinheit2": 
                        break; 
                    case "MengeBasis3": 
                        break; 
                    case "Mengeneinheit3": 
                        break; 
                    case "Haltbarkeit": 
                        material.StorageLife = System.Convert.ToInt32(value);
                        break; 
                    case "Bereich": 
                        break; 
                    case "Chargenverfolgung": 
                        material.IsLotManaged = value == "0" ? false : true;
                        break; 
                    case "ChargenVerfDummyNr": 
                        break; 
                    case "EinwegArtikel": 
                        break; 
                    case "ZustaendigkeitKen": 
                        break; 
                    case "Bem": 
                        material.Comment = value;
                        break; 
                    case "AusKartei": 
                        break; 
                    case "Bestellsperre": 
                        material.IsActive = value == "0" ? true : false;
                        break; 
                    case "Lagerfaehig": 
                        break; 
                    case "Palettierer": 
                        break; 
                    case "RezeptNr": 
                        break; 
                    case "SteuerRezNr": 
                        break; 
                    case "Dosierart": 
                        break; 
                    case "Stufe": 
                        break; 
                    case "EigenschaftProd": 
                        break; 
                    case "VerteilFuellgrWaage": 
                        break; 
                    case "RedlerV": 
                        break; 
                    case "Dichte": 
                        material.Density = System.Convert.ToDouble(value);
                        break; 
                    case "Toleranz": 
                        break; 
                    case "ToleranzAutoPlus": 
                        break; 
                    case "ToleranzAutoMinus": 
                        break; 
                    case "ProzOrKg": 
                        break; 
                    case "GMPArtGruppe": 
                        break; 
                    case "Laborkennz": 
                        break; 
                    case "MusterZMenge": 
                        break; 
                    case "KostenBezMenge": 
                        break; 
                    case "KostenMat": 
                        break; 
                    case "KostenVar": 
                        break; 
                    case "KostenFix":
                        break;
                    case "KostenVerp": 
                        break; 
                    case "KostenGemein": 
                        break; 
                    case "KostenVerlust": 
                        break; 
                    case "KostenHandlingVar": 
                        break; 
                    case "KostenHandlingFix": 
                        break; 
                    case "DSchnittProdMenge": 
                        break; 
                    case "Einlagerort": 
                        break; 
                    case "Auslagerort": 
                        break; 
                    case "MinLBestand": 
                        material.MinStockQuantity = System.Convert.ToDouble(value);
                        break; 
                    case "OptLBestand": 
                        material.OptStockQuantity = System.Convert.ToDouble(value);
                        break; 
                    case "Bestand": 
                        break; 
                    case "BestandFrei": 
                        break; 
                    case "BestandGesperrt": 
                        break; 
                    case "AbgangReserviert": 
                        break; 
                    case "ZugangReserviert": 
                        break; 
                    case "Gewichtung1": 
                        break; 
                    case "Gewichtung2": 
                        break; 
                    case "Gewichtung3": 
                        break; 
                    case "Gewichtung4": 
                        break; 
                    case "IntVal1": 
                        break; 
                    case "IntVal2": 
                        break; 
                    case "FloatVal1": 
                        break; 
                    case "FloatVal2": 
                        break; 
                    case "Tag_Verbrauch": 
                        break; 
                    case "Tag_LVerbrauch": 
                        break; 
                    case "Tag_LBestand": 
                        break; 
                    case "Tag_Zugang": 
                        break; 
                    case "Tag_Korrektur": 
                        break; 
                    case "Tag_BilanzDate": 
                        break; 
                    case "Tag_BilanzTime": 
                        break; 
                    case "Woche_Verbrauch": 
                        break; 
                    case "Woche_Zugang": 
                        break; 
                    case "Woche_Korrektur": 
                        break;
                    case "Woche_BilanzDate": 
                        break; 
                    case "Woche_BilanzTime": 
                        break; 
                    case "Monat_Verbrauch": 
                        break; 
                    case "Monat_AktBestand": 
                        break; 
                    case "Monat_LBestand": 
                        break; 
                    case "Monat_LDSBestand": 
                        break; 
                    case "Monat_LVerbrauch": 
                        break; 
                    case "Monat_Zugang": 
                        break; 
                    case "Monat_Korrektur": 
                        break; 
                    case "Monat_BilanzDate": 
                        break; 
                    case "Monat_BilanzTime": 
                        break; 
                    case "Jahr_Verbrauch": 
                        break; 
                    case "Jahr_Zugang": 
                        break; 
                    case "Jahr_Korrektur": 
                        break; 
                    case "Jahr_BilanzDate": 
                        break; 
                    case "Jahr_BilanzTime": 
                        break; 
                    case "Tag_SZugang": 
                        break; 
                    case "Tag_SVerbrauch": 
                        break; 
                    case "Woche_SZugang": 
                        break; 
                    case "Woche_SVerbrauch": 
                        break; 
                    case "Monat_SZugang": 
                        break; 
                    case "Monat_SVerbrauch": 
                        break; 
                    case "Jahr_SZugang": 
                        break; 
                    case "Jahr_SVerbrauch": 
                        break;
                    case Const.EntityUpdateDate: 
                        break; 
                    case "UpdateTime": 
                        break;
                    case Const.EntityUpdateName:
                        material.UpdateName = value.Length <= 5 ? value : value.Substring(0, 5);
                        break;
                    case Const.EntityInsertDate: 
                        break; 
                    case "InsertTime": 
                        break;
                    case Const.EntityInsertName:
                        material.InsertName = value.Length <= 5 ? value : value.Substring(0, 5);
                        break;
                    case "SpezWaerme":
                        material.SpecHeatCapacity = System.Convert.ToDouble(value);
                        break;
                    default:
                        break;
                }
            }
            if (material.BaseMDUnit == null)
            {
                material.BaseMDUnit = dbApp.MDUnit.Where(c => c.ISOCode == "KGM").FirstOrDefault();
            }
            return material;
        }
    }
}
