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
    public class GIPConv2006TableImportRezept : GIPConv2006TableImportClass
    {
        const string _FileName = "Rezepte.txt";
        public override string FileName
        {
	        get 
            {
                return _FileName;
            }
        }

        public override object ReadLine(DatabaseApp dbApp, DataRow line, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            string rezeptNo = line["Nr"].ToString();
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref rezeptNo))
                return null;

            return ReadRezeptFields(dbApp, rezeptNo, line.ItemArray, row, fields, bgWorker);
        }

        public override object ReadLine(DatabaseApp dbApp, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {
            int iNr = fields.IndexOf("Nr");
            if (iNr < 0)
                return null;
            string rezeptNo = values[iNr] as string;
            if (GIPConv2006TableImport.IsImportStringNullOrEmpty(ref rezeptNo))
                return null;

            return ReadRezeptFields(dbApp, rezeptNo, values, row, fields, bgWorker);
        }

        protected virtual Partslist ReadRezeptFields(DatabaseApp dbApp, string rezeptNo, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker)
        {         
            Partslist partsList = dbApp.Partslist.Where(c => c.PartslistNo == rezeptNo && c.DeleteDate == null).FirstOrDefault();
            if (partsList == null)
            {
                partsList = Partslist.NewACObject(dbApp, null, rezeptNo);
                partsList.PartslistNo = rezeptNo;
                dbApp.Partslist.Add(partsList);
            }
            // TODO: @aagincic: Rewrite progress - ReadRezeptFields()
            //if (bgWorker != null)
            //    bgWorker.ProgressInfo.SubProgressText = "Nr: " + rezeptNo;

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
                    case "Nr": 
                        break; 
                    default:
                        break;
                }
            }
            return partsList;
        }
    }
}
