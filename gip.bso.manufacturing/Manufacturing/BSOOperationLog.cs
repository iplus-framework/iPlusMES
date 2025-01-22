using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Operation logs'}de{'Betriebsprotokolle'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true)]
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

            ProcessFunctionsList = s_cQry_GetRelevantPAProcessFunctions(DatabaseApp.ContextIPlus, nameof(gip.mes.processapplication.PAFWorkInOutOperation))
                                                        .ToArray().OrderBy(c => c.ACCaption).ToList();

            if (ProcessFunctionsList != null && ProcessFunctionsList.Any())
            {
                SelectedProcessFunction = ProcessFunctionsList.FirstOrDefault();
            }

            return result;
        }

        #endregion

        #region Properties

        private core.datamodel.ACClass _SelectedProcessFunction;
        [ACPropertySelected(9999, "ProcessFunction", "en{'Selected process function'}de{'Ausgewählte Prozessfunktion'}")]
        public core.datamodel.ACClass SelectedProcessFunction
        {
            get => _SelectedProcessFunction;
            set
            {
                _SelectedProcessFunction = value;

                if (_SelectedProcessFunction != null)
                {
                    OperationLogList = DatabaseApp.OperationLog.Include(c => c.FacilityCharge)
                                                               .Include(c => c.FacilityCharge.Material)
                                                               .Include(c => c.FacilityCharge.FacilityLot)
                                                               .Where(c => c.RefACClassID == _SelectedProcessFunction.ACClassID
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
        [ACPropertyList(9999, "ProcessFunction")]
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
        [ACPropertySelected(9999, "OperationLog", "en{'Operation log'}de{'Betriebsprotokoll'}")]
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
        [ACPropertyList(9999, "OperationLog")]
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

        [ACMethodInfo("", "en{'Close operation'}de{'Betrieb schließen'}", 9999, true)]
        public void CloseSelectedOperationLog()
        {
            MsgWithDetails msg = OperationLog.CloseOperationLog(DatabaseApp, SelectedOperationLog, null);
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
            return SelectedOperationLog != null && SelectedOperationLog.OperationState == (short)OperationLogStateEnum.Open
                                                && SelectedOperationLog.Operation == (short)OperationLogEnum.RegisterEntityOnScan;
        }

        [ACMethodInteraction("", "en{'Navigate to Quantmanagement'}de{'Navigieren Sie zu Quantverwaltung'}", 9999, true, nameof(SelectedOperationLog))]
        public void NavigateToQuantManagement()
        {
            PAShowDlgManagerBase service = PAShowDlgManagerBase.GetServiceInstance(this);
            if (service != null)
            {
                PAOrderInfo info = new PAOrderInfo() { DialogSelectInfo = 0 };
                info.Entities.Add(new PAOrderInfoEntry(nameof(FacilityCharge), SelectedOperationLog.FacilityCharge.FacilityChargeID));
                service.ShowDialogOrder(this, info);
            }
        }

        public bool IsEnabledNavigateToQuantManagement()
        {
            return SelectedOperationLog != null && SelectedOperationLog.FacilityCharge != null;
        }

        #endregion

        #region PrecompiledQuery

        public static readonly Func<Database, string, IQueryable<gip.core.datamodel.ACClass>> s_cQry_GetRelevantPAProcessFunctions =
CompiledQuery.Compile<Database, string, IQueryable<gip.core.datamodel.ACClass>>(
    (ctx, pafACIdentifier) => ctx.ACClass.Where(c => (c.BasedOnACClassID.HasValue
                                                    && (c.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier // 1. Ableitungsstufe
                                                        || (c.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier // 2. Ableitungsstufe
                                                                    || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                && (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier // 3. Ableitungsstufe
                                                                                    || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                        && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier) // 4. Ableitungsstufe
                                                                                            || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier) // 5. Ableitungsstufe
                                                                                                    || (c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.BasedOnACClassID.HasValue
                                                                                                    && c.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACClass1_BasedOnACClass.ACIdentifier == pafACIdentifier)
                                                                                    )
                                                                        )
                                                                    )
                                                            )
                                                        )
                                                    
 

    ) && c.ACProject != null && c.ACProject.ACProjectTypeIndex == (short)Global.ACProjectTypes.Application && c.ACStartTypeIndex == (short)Global.ACStartTypes.Automatic));

        #endregion
    }
}
