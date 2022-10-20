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
using gip.core.processapplication;
using System.Data;

namespace gip2006.variobatch.processapplication
{
    public abstract class GIPConv2006TableImportClass
    {
        public abstract object ReadLine(DatabaseApp dbApp, DataRow line, int row, List<string> fields, ACBackgroundWorker bgWorker);
        public abstract object ReadLine(DatabaseApp dbApp, object[] values, int row, List<string> fields, ACBackgroundWorker bgWorker);
        public abstract string FileName { get; }
    }


    /// <summary>
    /// Baseclass for query old Variobatch-Database
    /// </summary>
    [ACClassInfo(ConstGIP2006.PackName_VarioGIP2006, "en{'GIPConv2006TableImport'}de{'GIPConv2006TableImport'}", Global.ACKinds.TPABGModule, Global.ACStorableTypes.NotStorable, false, true)]
    public partial class GIPConv2006TableImport : ACComponent
    {
        #region enums
        public enum AutoSaveMode
        {
            CallerSaves,
            SaveAfterEveryRow,
            SaveAfterImport
        }
        #endregion

        #region c'tors
        public GIPConv2006TableImport(gip.core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier="")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            if (!base.ACInit(startChildMode))
                return false;

            return true;
        }
        #endregion

        #region Properties
        protected List<GIPConv2006TableImportClass> _Importer = null;
        public virtual List<GIPConv2006TableImportClass> Importer
        {
            get
            {
                if (_Importer == null)
                {
                    _Importer = new List<GIPConv2006TableImportClass>();
                    _Importer.Add(new GIPConv2006TableImportArtikel());
                    _Importer.Add(new GIPConv2006TableImportRezept());
                    _Importer.Add(new GIPConv2006TableImportLKW());
                    _Importer.Add(new GIPConv2006TableImportLKWKomp());
                    _Importer.Add(new GIPConv2006TableImportLKWDeckel());
                    _Importer.Add(new GIPConv2006TableImportAdressen());
                    _Importer.Add(new GIPConv2006TableImportAdressenLief());
                    _Importer.Add(new GIPConv2006TableImportAbladeStelle());
                    _Importer.Add(new GIPConv2006TableImportChipCard());
                }
                return _Importer;
            }
        }

        #endregion

        #region Methods

        #region Private

        protected virtual GIPConv2006TableImportClass FindImporter(string fileName)
        {
            return Importer.Where(c => c.FileName == fileName).FirstOrDefault();
        }

        private void ReportProgressSub(ACBackgroundWorker bgWorker)
        {
            // TODO: @aagincic: Rewrite progress - ReportProgressSub()
            if (bgWorker == null)
                return;
            // Falls Abbruch von Benutzer
            if (bgWorker.CancellationPending == true)
            {
                bgWorker.EventArgs.Cancel = true;
                throw new Exception(Root.Environment.TranslateMessage(this, "Error00043"));
            }

            //bgWorker.ProgressInfo.SubProgressCurrent++;
            //bgWorker.ReportProgress();
        }

        public static bool IsImportStringNullOrEmpty(ref String value)
        {
            if (String.IsNullOrEmpty(value))
                return true;
            value = value.Trim(' ', '"');
            if (value == "NULL" || value == "Null" || value == "null")
                return true;
            return false;
        }

        #endregion

        #region Public
        public Msg ImportString(String importString, String fileName, DatabaseApp dbApp, AutoSaveMode autoSave, ACBackgroundWorker bgWorker)
        {
            if (String.IsNullOrEmpty(importString))
            {
                return new Msg() { Source = this.GetACUrl(), Message = "Empty importString", MessageLevel = eMsgLevel.Exception };
            }
            string txt = GIPConv2006TableImport.CorrectCVSString(importString);
            using (CSVReader reader = new CSVReader(txt, Encoding.GetEncoding(1252)))
            {
                DataTable table = reader.CreateDataTable(true);
                return ImportDataTable(table, fileName, dbApp, autoSave, bgWorker);
            }

            //string[] lines = importString.Split(new char[] { '\n'});
            //return ImportFile(lines, fileName, db, autoSave, bgWorker);
        }

        public Msg ImportFile(String path, DatabaseApp dbApp, AutoSaveMode autoSave, ACBackgroundWorker bgWorker)
        {
            if (dbApp == null)
                return null;
            string fileName = Path.GetFileName(path);
            GIPConv2006TableImportClass importer = FindImporter(fileName);
            if (importer == null)
                return null;

            string[] lines;
            try
            {
                lines = File.ReadAllLines(path, Encoding.GetEncoding(1252));
            }
            catch (Exception e)
            {
                return new Msg() { Source = this.GetACUrl(), Message = e.Message, MessageLevel = eMsgLevel.Exception };
            }

            return ImportFile(lines, fileName, dbApp, autoSave, bgWorker);
        }

        public Msg ImportFileWithCSVReader(String path, DatabaseApp dbApp, AutoSaveMode autoSave, ACBackgroundWorker bgWorker)
        {
            if (dbApp == null)
                return null;
            string fileName = Path.GetFileName(path);
            GIPConv2006TableImportClass importer = FindImporter(fileName);
            if (importer == null)
                return null;

            DataTable table = null;
            try
            {
                table = CSVReader.ReadCSVFile(path, true, Encoding.GetEncoding(1252));
            }
            catch (Exception e)
            {
                return new Msg() { Source = this.GetACUrl(), Message = e.Message, MessageLevel = eMsgLevel.Exception };
            }

            return ImportDataTable(table, fileName, dbApp, autoSave, bgWorker);
        }

        private Msg ImportFile(string[] lines, String fileName, DatabaseApp dbApp, AutoSaveMode autoSave, ACBackgroundWorker bgWorker)
        {
            if (dbApp == null)
                return null;
            GIPConv2006TableImportClass importer = FindImporter(fileName);
            if (importer == null)
                return null;


            // TODO: @aagincic: Rewrite progress - ImportFile()
            //if (bgWorker != null)
            //{
            //    bgWorker.ProgressInfo.SubProgressRangeFrom = 0;
            //    bgWorker.ProgressInfo.SubProgressRangeTo = lines.Count() - 1;
            //    bgWorker.ProgressInfo.SubProgressCurrent = 0;
            //}

            List<string> fields = new List<string>();
            int row = 0;
            foreach (string line in lines)
            {
                if (row == 0)
                {
                    string[] fieldArr = line.Split(new char[] { ',' }, StringSplitOptions.None);
                    foreach (string value in fieldArr)
                    {
                        fields.Add(value.Trim(' ', '"'));
                    }
                    row++;
                    continue;
                }

                object entity = null;
                string[] values = line.Replace("NULL,", "\"NULL\",").Split(new string[] { "\"," }, StringSplitOptions.None);
                try
                {
                    entity = importer.ReadLine(dbApp, values, row, fields, bgWorker);
                }
                catch (Exception e)
                {
                    if (autoSave != AutoSaveMode.CallerSaves)
                    {
                        dbApp.ACUndoChanges();
                    }
                    return new Msg() { Source = this.GetACUrl(), Message = e.Message, MessageLevel = eMsgLevel.Exception };
                }

                row++;

                if (entity != null && autoSave == AutoSaveMode.SaveAfterEveryRow)
                {
                    Msg result = dbApp.ACSaveChanges();
                    if (result != null)
                    {
                        dbApp.ACUndoChanges();
                        return result;
                    }
                }
                row++;
                ReportProgressSub(bgWorker);
            }

            if (autoSave == AutoSaveMode.SaveAfterImport)
            {
                Msg result = dbApp.ACSaveChanges();
                if (result != null)
                {
                    dbApp.ACUndoChanges();
                    return result;
                }
            }

            return null;
        }

        public Msg ImportDataTable(DataTable table, String fileName, DatabaseApp dbApp, AutoSaveMode autoSave, ACBackgroundWorker bgWorker)
        {
            if (dbApp == null)
                return null;
            GIPConv2006TableImportClass importer = FindImporter(fileName);
            if (importer == null)
                return null;

            if (bgWorker != null)
            {
                // TODO: @aagincic: Rewrite progress - ImportDataTable()
                //bgWorker.ProgressInfo.SubProgressRangeFrom = 0;
                //bgWorker.ProgressInfo.SubProgressRangeTo = table.Rows.Count - 1;
                //bgWorker.ProgressInfo.SubProgressCurrent = 0;
            }

            List<string> fields = new List<string>(table.Columns.Count);
            int col = 0;
            foreach (DataColumn column in table.Columns)
            {
                fields.Add(column.ColumnName);
                col++;
            }

            int row = 0;
            foreach (DataRow line in table.Rows)
            {
                //if (row == 0)
                //{
                //    row++;
                //    continue;
                //}

                object entity = null;
                try
                {
                    entity = importer.ReadLine(dbApp, line, row, fields, bgWorker);
                }
                catch (Exception e)
                {
                    if (autoSave != AutoSaveMode.CallerSaves)
                    {
                        dbApp.ACUndoChanges();
                    }
                    return new Msg() { Source = this.GetACUrl(), Message = e.Message, MessageLevel = eMsgLevel.Exception };
                }

                row++;

                if (entity != null && autoSave == AutoSaveMode.SaveAfterEveryRow)
                {
                    Msg result = dbApp.ACSaveChanges();
                    if (result != null)
                    {
                        dbApp.ACUndoChanges();
                        return result;
                    }
                }
                row++;
                ReportProgressSub(bgWorker);
            }

            if (autoSave == AutoSaveMode.SaveAfterImport)
            {
                Msg result = dbApp.ACSaveChanges();
                if (result != null)
                {
                    dbApp.ACUndoChanges();
                    return result;
                }
            }

            return null;
        }

        public static string CorrectCVSString(string txt)
        {
            if (String.IsNullOrEmpty(txt))
                return txt;
            txt = txt.Replace("NULL,", "\"NULL\",");
            txt = txt.Replace(", NULL", ", \"NULL\"");
            txt = txt.Replace("\", \"", "\",\"");
            return txt;
        }

        #endregion

        #region Protected
        #endregion

        #endregion
    }
}
