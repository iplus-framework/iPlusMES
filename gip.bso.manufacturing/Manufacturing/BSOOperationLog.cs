using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    public class BSOOperationLog : ACBSOvb
    {
        #region c'tors

        public BSOOperationLog(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") :
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();

            ProcessFunctionsList = BSOWorkCenterSelector.s_cQry_GetRelevantPAProcessFunctions(DatabaseApp.ContextIPlus, nameof(gip.mes.processapplication.PAFInOutOperationOnScan), "")
                                     .Distinct()
                                     .ToList();

            if (ProcessFunctionsList != null && ProcessFunctionsList.Any())
            {
                SelectedProcessFunction = ProcessFunctionsList.FirstOrDefault();
            }


            return result;
        }

        #endregion

        #region Properties

        private core.datamodel.ACClass _SelectedProcessFunction;
        
        public core.datamodel.ACClass SelectedProcessFunction
        {
            get => _SelectedProcessFunction;
            set
            {
                _SelectedProcessFunction = value;

                if (_SelectedProcessFunction != null)
                {
                    OperationLogList = DatabaseApp.OperationLog.Where(c => c.RefACClassID == _SelectedProcessFunction.ACClassID
                                                                        && c.OperationState == (short)OperationLogStateEnum.Open)
                                                               .OrderBy(c => c.OperationTime)
                                                               .ToList();
                }
                else
                {
                    OperationLogList = null;
                }

                OnPropertyChanged();
            }
        }

        private List<core.datamodel.ACClass> _ProcessFunctionsList;

        public List<core.datamodel.ACClass> ProcessFunctionsList
        {
            get => _ProcessFunctionsList;
            set
            {
                _ProcessFunctionsList = value;
                OnPropertyChanged();
            }
        }

        private OperationLog _SelectedOperationLog;

        public OperationLog SelectedOperationLog
        {
            get => _SelectedOperationLog;
            set
            {
                _SelectedOperationLog = value;
                OnPropertyChanged();
            }
        }

        private List<OperationLog> _OperationLogList;

        public List<OperationLog> OperationLogList
        {
            get => _OperationLogList;
            set
            {
                _OperationLogList = value;
                OnPropertyChanged();
            }
        }


        #endregion


        #region Methods

        [ACMethodInfo("", "en{'Close operation log'}de{'Close operation log'}", 9999, true)]
        public void CloseSelectedOperationLog()
        {
            SelectedOperationLog.OperationState = (short)OperationLogStateEnum.Closed;

            OperationLog outOperationLog = OperationLog.NewACObject(DatabaseApp, null);
            outOperationLog.RefACClassID = this.ComponentClass.ACClassID;
            outOperationLog.FacilityChargeID = SelectedOperationLog.FacilityChargeID;
            outOperationLog.Operation = (short)OperationLogEnum.UnregisterEntityOnScan;
            outOperationLog.OperationState = (short)OperationLogStateEnum.Closed;
            outOperationLog.OperationTime = DateTime.Now;
            outOperationLog.ACProgramLogID = SelectedOperationLog.ACProgramLogID;

            DatabaseApp.OperationLog.AddObject(outOperationLog);

            Msg msg = DatabaseApp.ACSaveChanges();
            if (msg != null)
            {
                Messages.Msg(msg);
                return;
            }

            OperationLogList = DatabaseApp.OperationLog.Where(c => c.RefACClassID == _SelectedProcessFunction.ACClassID
                                                                        && c.OperationState == (short)OperationLogStateEnum.Open)
                                                               .OrderBy(c => c.OperationTime)
                                                               .ToList();
        }

        public bool IsEnabledCloseSelectedOperationLog()
        {
            return SelectedOperationLog != null && SelectedOperationLog.OperationState == (short)OperationLogStateEnum.Closed
                                                && SelectedOperationLog.Operation == (short)OperationLogEnum.RegisterEntityOnScan;
        }

        #endregion
    }
}
