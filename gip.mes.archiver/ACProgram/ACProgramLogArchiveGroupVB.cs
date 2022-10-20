using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using vd = gip.mes.datamodel;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using gip.core.autocomponent;
using gip.core.archiver;
using gip.core.communication;
using System.Threading;

namespace gip.mes.archiver
{
    [ACClassInfo(Const.PackName_VarioSystem, "en{'ACProgramLog archive'}de{'ACProgramLog archive'}", Global.ACKinds.TACDAClass, Global.ACStorableTypes.Required, false, false)]
    public class ACProgramLogArchiveGroupVB : ACProgramLogArchiveGroup
    {
        #region c'tors
        public ACProgramLogArchiveGroupVB(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACInit(core.datamodel.Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            return base.ACDeInit(deleteACClassTask);
        }

        public new const string ClassName = "ACProgramLogArchiveGroupVB";
        #endregion


        #region Execute-Helper-Handlers
        protected override bool HandleExecuteACMethod(out object result, AsyncMethodInvocationMode invocationMode, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            result = null;
            switch (acMethodName)
            {
                case "RestoreArchivedProgramLogVB":
                    result = RestoreArchivedProgramLogVB(acParameter[0] as String, (DateTime) acParameter[1]);
                    return true;
                case "ArchiveProgramLogVBManual":
                    result = ArchiveProgramLogVBManual(acParameter[0] as String, (DateTime)acParameter[1], acParameter[2] as String);
                    return true;
            }
            return base.HandleExecuteACMethod(out result, invocationMode, acMethodName, acClassMethod, acParameter);
        }
        #endregion


        #region Methods
        
        #region overrides

        protected override string OnProgramLogArchive(ACProgram acProgram, string exportPath)
        {
            IEnumerable<vd.OrderLog> orderLogList;
            using (vd.DatabaseApp dbApp = new vd.DatabaseApp())
            {
                vd.ACProgram acProgramVB = dbApp.ACProgram.FirstOrDefault(c => c.ACProgramID == acProgram.ACProgramID);
                orderLogList = dbApp.OrderLog.Where(c => c.VBiACProgramLog.ACProgramID == acProgram.ACProgramID).ToArray();

                if (orderLogList.Any())
                {
                    string acProgramFolderName = CreateACProgramDirectory(acProgramVB, exportPath);
                    ArchiveOrderLog(orderLogList, acProgram, acProgramFolderName, dbApp);
                    MsgWithDetails msg = dbApp.ACSaveChanges();
                    if (msg != null)
                    {
                        AddAlarm(PropNameExportAlarm, msg);
                        Messages.LogError(GetACUrl(), "OnProgramLogArchive(10)", msg.Message);
                        Messages.LogError(GetACUrl(), "OnProgramLogArchive(11)", msg.InnerMessage);
                    }

                    return acProgramFolderName;
                }
                else
                    return base.OnProgramLogArchive(acProgram, exportPath);
            }
        }

        protected override string CreateACProgramDirectory(IACObject acProgram, string exportPath)
        {
            string exportPathWithDate = exportPath;

            vd.ACProgram acProgramVB = acProgram as vd.ACProgram;
            if (acProgramVB == null && acProgram is ACProgram)
                return base.CreateACProgramDirectory(acProgram, exportPath);

            if (acProgramVB == null)
                return null;

            Tuple<string, DateTime> prodOrder = GetProdOrderProgramNoAndInsertDate(acProgramVB);

            if (prodOrder == null)
                return null;

            DateTime prodOrderInsertDate = prodOrder.Item2;
            exportPathWithDate = exportPath + "\\" + prodOrderInsertDate.Year;

            if (!Directory.Exists(exportPathWithDate))
                Directory.CreateDirectory(exportPathWithDate);

            exportPathWithDate = string.Format("{0}\\{1:00}", exportPathWithDate, prodOrderInsertDate.Month);
            if (!Directory.Exists(exportPathWithDate))
                Directory.CreateDirectory(exportPathWithDate);

            string acProgramDirPath = string.Format("{0}\\ProdOrderProgramNo({1})ACProgram({2})", exportPathWithDate, prodOrder.Item1, acProgramVB.ProgramNo);  
            if (File.Exists(acProgramDirPath + ".zip"))
            {
                File.Delete(acProgramDirPath + ".zip");
                //AddAlarm("ArchivePrograms(1)", string.Format("File {0}.zip already exist!!!", acProgramFolderName));
                //return null;
            }
            if (!Directory.Exists(acProgramDirPath))
                Directory.CreateDirectory(acProgramDirPath);
            return acProgramDirPath;
        }

        private Tuple<string, DateTime> GetProdOrderProgramNoAndInsertDate(vd.ACProgram acProgram)
        {
            vd.ProdOrder prodOrder = acProgram.ACProgramLog_ACProgram.Where(c => c.OrderLog_VBiACProgramLog != null && c.OrderLog_VBiACProgramLog.ProdOrderPartslistPos != null)
                                                        .Select(x => x.OrderLog_VBiACProgramLog.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder).Distinct().FirstOrDefault();

            if (prodOrder == null)
            {
                prodOrder = acProgram.ACProgramLog_ACProgram.Where(c => c.OrderLog_VBiACProgramLog != null && c.OrderLog_VBiACProgramLog.ProdOrderPartslistPosRelation != null)
                                     .Select(x => x.OrderLog_VBiACProgramLog.ProdOrderPartslistPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder).Distinct().FirstOrDefault();
            }

            if (prodOrder != null)
            {
                return new Tuple<string, DateTime>(prodOrder.ProgramNo, prodOrder.InsertDate);
            }
            return null;
        }

        private void ArchiveOrderLog(IEnumerable<vd.OrderLog> orderLogList, ACProgram acProgram, string exportPath, vd.DatabaseApp dbApp)
        {
            if (exportPath == null)
                return;

            string filePath = string.Format("{0}\\OrderLog.xml", exportPath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                //AddAlarm("ArchivePrograms(1)", string.Format("File {0} already exist!!!", filePath));
                //return;
            }

            try
            {
                foreach (var item in orderLogList)
                    item.GetObjectContext().Detach(item);
            }
            catch(Exception ec)
            {
                //Error50219: Detaching OrderLog from database context is fail!!! Error message: {0}
                Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "ArchiveOrderLog", 164, "Error50219", ec.Message);
                AddAlarm(PropNameExportAlarm, msg);

                string msgEc = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msgEc += " Inner:" + ec.InnerException.Message;

                Messages.LogException(ClassName, "ArchiveOrderLog(0)", msgEc);

                return;
            }

            DataContractSerializer serializer = new DataContractSerializer(typeof(List<vd.OrderLog>));
            try
            {
                using (FileStream fs = File.Open(filePath, FileMode.Create))
                {
                    serializer.WriteObject(fs, orderLogList);
                }
            }
            catch (Exception ec)
            {
                //Error50218: Order log serialization is fail! Error message: {0}
                Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "ArchiveOrderLog", 187, "Error50218", ec.Message);
                AddAlarm(PropNameExportAlarm, msg);

                string msgEc = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msgEc += " Inner:" + ec.InnerException.Message;

                Messages.LogException(ClassName, "ArchiveOrderLog(10)", msgEc);

                return;
            }

            try
            {
                dbApp.ExecuteStoreCommand("delete OrderLog from OrderLog ol inner join ACProgramLog pl on pl.ACProgramLogID = ol.VBiACProgramLogID inner join ACProgram p on p.ACProgramID = pl.ACProgramID where pl.ACProgramID = {0}", acProgram.ACProgramID);
            }
            catch (Exception ec)
            {
                //Error50217: Order log archive is fail! Error message: {0}
                Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "ArchiveOrderLog", 206, "Error50217", ec.Message);
                AddAlarm(PropNameExportAlarm, msg);

                string msgEc = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msgEc += " Inner:" + ec.InnerException.Message;

                Messages.LogException(ClassName, "ArchiveOrderLog(20)", msgEc);

                return;
            }
        }

        #endregion


        #region Client-Methods

        [ACMethodInfo("", "en{'Restore program log from archive'}de{'Programmablaufprotokoll-Wiederherstellung aus dem Archiv'}", 500)]
        public string RestoreArchivedProgramLogVB(string prodOrderProgramNo, DateTime prodOrderInsertDate)
        {
            bool _isActive = false;

            using (ACMonitor.Lock(IsArchivingActiveLock))
                _isActive = IsArchivingActive;

            if (_isActive)
                return "Warning50018";

            DelegateQueue.Add(() => FindProgramLogInArchive(prodOrderProgramNo, prodOrderInsertDate));
            return null;
        }

        private void FindProgramLogInArchive(string prodOrderProgramNo, DateTime prodOrderInsertDate)
        {
            PAFileCyclicExport paFileCyclicExport = ParentACComponent as PAFileCyclicExport;
            if (paFileCyclicExport == null)
                return;

            string pathWithDate = string.Format("{0}\\{1}\\{2:00}", paFileCyclicExport.Path, prodOrderInsertDate.Year, prodOrderInsertDate.Month);

            if (!Directory.Exists(pathWithDate))
            {
                //Error50213: Restore ACProgram: Can't find the directory with path: {0}
                Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "FindProgramLogInArchive", 253, "Error50213", pathWithDate);
                AddAlarm(PropNameExportAlarm, msg);
                return;
            }

            //string[] archives = Directory.GetFiles(pathWithDate);
            string[] targetFilePaths = Directory.GetFiles(pathWithDate, string.Format("ProdOrderProgramNo({0})*.zip", prodOrderProgramNo));
            foreach(var targetFilePath in targetFilePaths)
            {
                ProcessArchiveFile(targetFilePath, paFileCyclicExport.Path);
            }
        }

        private void ProcessArchiveFile(string targetFilePath, string defaultArchivePath)
        {
            string targetDirPath = targetFilePath.Replace(".zip", "");
            if (!Directory.Exists(targetDirPath))
            {
                Directory.CreateDirectory(targetDirPath);
                ZipFile.ExtractToDirectory(targetFilePath, targetDirPath);
            }

            string[] archive = Directory.GetFiles(targetDirPath);
            string orderLogPath = archive.FirstOrDefault(c => c.Contains("OrderLog"));

            if (string.IsNullOrEmpty(orderLogPath))
            {
                //Error50216:Restore ACProgram: Can't find the OrderLog file path!
                Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "FindProgramLogInArchive", 281, "Error50216");
                AddAlarm(PropNameExportAlarm, msg);
                return;
            }

            ProcessACProgramArchiveFile(targetFilePath);

            try
            {
                DataContractSerializer serializerOrder = new DataContractSerializer(typeof(List<vd.OrderLog>));
                using (FileStream fs = File.Open(orderLogPath, FileMode.Open))
                {
                    List<vd.OrderLog> orders = serializerOrder.ReadObject(fs) as List<vd.OrderLog>;
                    using (vd.DatabaseApp dbApp = new vd.DatabaseApp())
                    {
                        foreach (vd.OrderLog log in orders)
                        {
                            if (!dbApp.OrderLog.Any(c => c.VBiACProgramLogID == log.VBiACProgramLogID))
                            {
                                if (String.IsNullOrEmpty(log.XMLConfig))
                                    log.ACProperties.Serialize();
                                dbApp.AddToOrderLog(log);
                            }
                        }
                        MsgWithDetails msg = dbApp.ACSaveChanges();
                        if (msg != null)
                        {
                            AddAlarm(PropNameExportAlarm, msg);
                            Messages.LogError(GetACUrl(), "FindProgramLogInArchive(9)", msg.Message);
                            Messages.LogError(GetACUrl(), "FindProgramLogInArchive(9)", msg.InnerMessage);
                            return;
                        }
                    }
                }
                CleanUpAfterRestore(targetFilePath, defaultArchivePath);
            }
            catch (Exception ec)
            {
                //Error50215: Order log deserialization is fail! Error message: {0}
                Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "FindProgramLogInArchive", 314, "Error50215", ec.Message);
                AddAlarm(PropNameExportAlarm, msg);

                string msgEc = ec.Message;
                if (ec.InnerException != null && ec.InnerException.Message != null)
                    msgEc += " Inner:" + ec.InnerException.Message;

                Messages.LogException(ClassName, "FindProgramLogInArchive(10)", msgEc);

                return;
            }
        }


        [ACMethodInfo("", "en{'Archive program log'}de{'Archiviere Programmablaufprotokoll'}", 510)]
        public string ArchiveProgramLogVBManual(string prodOrderProgramNo, DateTime prodOrderInsertDate, string acProgramNo)
        {
            bool _isActive = false;

            using (ACMonitor.Lock(IsArchivingActiveLock))
                _isActive = IsArchivingActive;

            if (_isActive)
                return "Warning50018";

            bool isArchiveable = false;
            using (var db = new Database())
            {
                isArchiveable = db.ACProgram.Where(c => c.ProgramNo == acProgramNo && !c.ACClassTask_ACProgram.Any()).Any();
            }
            if (!isArchiveable)
                 return "Warning50019";

            DelegateQueue.Add(() => ArchiveProgramLogVB(prodOrderProgramNo, prodOrderInsertDate, acProgramNo));
            return null;
        }

        public void ArchiveProgramLogVB(string prodOrderProgramNo, DateTime prodOrderInsertDate, string acProgramNo)
        {
            PAFileCyclicExport paFileCyclicExport = ParentACComponent as PAFileCyclicExport;
            if (paFileCyclicExport == null)
                return;

            string pathWithDate = String.Format("{0}\\{1}\\{2:00}", paFileCyclicExport.Path, prodOrderInsertDate.Year, prodOrderInsertDate.Month);
            ACProgram acProgram = null;
            vd.ACProgram acProgramVB = null;
            using (Database db = new core.datamodel.Database())
            {
                using (vd.DatabaseApp dbApp = new vd.DatabaseApp(db))
                {
                    vd.ProdOrder prodOrder = dbApp.ProdOrder.FirstOrDefault(c => c.ProgramNo == prodOrderProgramNo);
                    acProgramVB = dbApp.OrderLog.Where(c => (c.ProdOrderPartslistPos != null && c.ProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProdOrderID == prodOrder.ProdOrderID)
                                                    || (c.ProdOrderPartslistPosRelation != null
                                                    && c.ProdOrderPartslistPosRelation.SourceProdOrderPartslistPos.ProdOrderPartslist.ProdOrder.ProdOrderID == prodOrder.ProdOrderID)
                                                    && c.VBiACProgramLog.ACProgramLog1_ParentACProgramLog == null)
                                                    .ToList().Select(x => x.VBiACProgramLog.ACProgram).Distinct().FirstOrDefault(c => c.ProgramNo == acProgramNo);
                    if (acProgramVB != null)
                        acProgram = db.ACProgram.FirstOrDefault(c => c.ACProgramID == acProgramVB.ACProgramID);


                    if (acProgram == null)
                    {
                        //Error50214: Archive ProgramLog: Can't find a ACProgram!
                        Msg msg = new Msg(this, eMsgLevel.Error, ClassName, "AcrhiveProgramLogVB", 379, "Error50214");
                        AddAlarm(PropNameExportAlarm, msg);
                        return;
                    }

                    try
                    {
                        ArchiveProgram(paFileCyclicExport.Path, acProgram, db);
                        MsgWithDetails msg1 = db.ACSaveChanges();
                        if (msg1 != null)
                        {
                            AddAlarm(PropNameExportAlarm, msg1);
                            Messages.LogError(GetACUrl(), "ArchiveProgramLogVB(9)", msg1.Message);
                            Messages.LogError(GetACUrl(), "ArchiveProgramLogVB(10)", msg1.InnerMessage);
                        }
                    }
                    catch (Exception e)
                    {
                        Msg msg = new Msg(e.Message, this, eMsgLevel.Exception, ClassName, "", 394);
                        AddAlarm(PropNameExportAlarm, msg);

                        string msgEc = e.Message;
                        if (e.InnerException != null && e.InnerException.Message != null)
                            msgEc += " Inner:" + e.InnerException.Message;

                        Messages.LogException(ClassName, "ArchiveProgramLogVB(10)", msgEc);
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
