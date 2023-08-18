using gip.core.datamodel;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace gip.mes.datamodel
{
    [ACClassInfo(Const.PackName_VarioSystem, ConstApp.OperationLog, Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, true, "", "BSOOperationLog")]
    [ACPropertyEntity(1, "FacilityCharge", "en{'Quant'}de{'Quant'}", Const.ContextDatabase + "\\" + FacilityCharge.ClassName + Const.DBSetAsEnumerablePostfix, "", true)]
    [ACPropertyEntity(2, "OperationTime", "en{'Time'}de{'Zeit'}", "", "", true)]
    [ACPropertyEntity(31, "Operation", "en{'Operation'}de{'Operation'}", "", "", true)]
    [ACPropertyEntity(31, "OperationState", "en{'State'}de{'Status'}", "", "", true)]
    [ACPropertyEntity(9999, "XMLValue", "en{'Value'}de{'Wert'}")]
    public partial class OperationLog 

    {
        public static OperationLog NewACObject(DatabaseApp dbApp, IACObject parentACObject)
        {
            OperationLog entity = new OperationLog();
            entity.OperationLogID = Guid.NewGuid();

            ACClass refACClass = parentACObject as ACClass;
            if (refACClass != null)
                entity.RefACClassID = refACClass.ACClassID;

            entity.SetInsertAndUpdateInfo(dbApp.UserName, dbApp);
            
            return entity;
        }

        #region Additional properties

        [NotMapped]
        private TimeSpan _MinDuration;
        [ACPropertyInfo(9999, "", "en{'Min. duration'}de{'Min. Dauer'}")]
        [NotMapped]
        public TimeSpan MinDuration
        {
            get => _MinDuration;
            set
            {
                _MinDuration = value;
                OnPropertyChanged(nameof(MinDuration));
            }
        }


        [NotMapped]
        private DateTime _CompleteTime;
        [ACPropertyInfo(9999, "", "en{'Completed at'}de{'Abgeschlossen am'}")]
        [NotMapped]
        public DateTime CompleteTime
        {
            get => _CompleteTime;
            set
            {
                _CompleteTime = value;
                OnPropertyChanged(nameof(CompleteTime));
            }
        }

        #endregion

        #region Methods

        public static MsgWithDetails CloseOperationLog(DatabaseApp dbApp, OperationLog inOperationLog, ACMethod method)
        {
            inOperationLog.OperationState = (short)OperationLogStateEnum.Closed;

            OperationLog outOperationLog = OperationLog.NewACObject(dbApp, null);
            outOperationLog.RefACClassID = inOperationLog.RefACClassID;
            outOperationLog.FacilityChargeID = inOperationLog.FacilityChargeID;
            outOperationLog.Operation = (short)OperationLogEnum.UnregisterEntityOnScan;
            outOperationLog.OperationState = (short)OperationLogStateEnum.Closed;
            outOperationLog.OperationTime = DateTime.Now;
            outOperationLog.ACProgramLogID = inOperationLog.ACProgramLogID;

            if(method != null)
            {
                string xml = gip.core.datamodel.ACClassMethod.SerializeACMethod(method);
                outOperationLog.XMLValue = xml;
            }

            dbApp.OperationLog.Add(outOperationLog);

            return dbApp.ACSaveChanges();
        }

        #endregion
    }
}
