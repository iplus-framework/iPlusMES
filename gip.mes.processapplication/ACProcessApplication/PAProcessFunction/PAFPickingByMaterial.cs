using gip.core.autocomponent;
using gip.core.datamodel;
using gip.core.reporthandler;
using gip.mes.datamodel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace gip.mes.processapplication
{
    [ACClassInfo(Const.PackName_VarioAutomation, "en{'Picking by material'}de{'Kommissionierung nach Material'}", Global.ACKinds.TPAProcessFunction, Global.ACStorableTypes.Required, false, true, PWInfoACClass = nameof(PWPickingByMaterial), BSOConfig = "BSOPickingByMaterial", SortIndex = 600)]
    public class PAFPickingByMaterial : PAProcessFunction
    {
        static PAFPickingByMaterial()
        {
            ACMethod.RegisterVirtualMethod(typeof(PAFPickingByMaterial), ACStateConst.TMStart, CreateVirtualMethod("PickingByMat", "en{'Picking by material'}de{'Kommissionierung nach Material'}", typeof(PWPickingByMaterial)));
            RegisterExecuteHandler(typeof(PAFPickingByMaterial), HandleExecuteACMethod_PAFPickingByMaterial);
        }

        public PAFPickingByMaterial(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") 
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }

        public override bool ACPostInit()
        {
            bool result = base.ACPostInit();

            _PrintManager = ACPrintManager.ACRefToServiceInstance(this);

            return result;
        }

        public override bool ACDeInit(bool deleteACClassTask = false)
        {
            if (_PrintManager != null)
            {
                _PrintManager.Detach();
                _PrintManager = null;
            }
            return base.ACDeInit(deleteACClassTask);
        }

        [ACPropertyBindingTarget]
        public IACContainerTNet<Guid> ScannedFacilityCharge
        {
            get;
            set;
        }

        private ACRef<ACPrintManager> _PrintManager;
        
        protected ACPrintManager PrintManager
        {
            get
            {
                if (_PrintManager != null)
                {
                    return _PrintManager.ValueT;
                }
                return null;
            }
        }


        [ACMethodAsync("Process", "en{'Start'}de{'Start'}", (short)MISort.Start, false)]
        public override ACMethodEventArgs Start(ACMethod acMethod)
        {
            return base.Start(acMethod);
        }

        public override void SMStarting()
        {
            base.SMStarting();
        }

        [ACMethodInfo("","",9999)]
        public string GetCurrentTaskACUrl()
        {
            if (CurrentTask != null && CurrentTask.ValueT != null)
                return CurrentTask.ValueT.ACUrl;
            return null;
        }

        [ACMethodInfo("", "", 9999)]
        public Msg PrintOverPAOrderInfo(PAOrderInfo info)
        {
            if (PrintManager != null)
            {
                return PrintManager.Print(info, 1);
            }
            return new Msg(eMsgLevel.Error, "Print manager is not configured!");
        }

        protected static ACMethodWrapper CreateVirtualMethod(string acIdentifier, string captionTranslation, Type pwClass)
        {
            ACMethod method = new ACMethod(acIdentifier);
            Dictionary<string, string> paramTranslation = new Dictionary<string, string>();
            Dictionary<string, string> resultTranslation = new Dictionary<string, string>();
            return new ACMethodWrapper(method, captionTranslation, pwClass, paramTranslation, resultTranslation);
        }

        public static bool HandleExecuteACMethod_PAFPickingByMaterial(out object result, IACComponent acComponent, string acMethodName, gip.core.datamodel.ACClassMethod acClassMethod, params object[] acParameter)
        {
            return HandleExecuteACMethod_PAProcessFunction(out result, acComponent, acMethodName, acClassMethod, acParameter);
        }

        [ACMethodInfo("OnScanEvent", "en{'OnScanEvent'}de{'OnScanEvent'}", 503)]
        public BarcodeSequenceBase OnScanEvent(BarcodeSequenceBase sequence, bool previousLotConsumed, Guid facilityID, Guid facilityChargeID, int scanSequence, short? questionResult)
        {
            BarcodeSequenceBase resultSequence = new BarcodeSequenceBase();
            if (scanSequence == 1)
            {
                // Info50088: Scan a lot number or a other identifier to identify the material or quant. (Scannen Sie eine Los- bzw. Chargennummer oder ein anderes Kennzeichen zur Identifikation des Materials bzw. Quants.)
                resultSequence.Message = new Msg(this, eMsgLevel.Info, nameof(PAFPickingByMaterial), "OnScanEvent(10)", 10, "Info50088");
                resultSequence.State = BarcodeSequenceBase.ActionState.ScanAgain;
            }
            else
            {
                ScannedFacilityCharge.ValueT = Guid.Empty;

                if (facilityChargeID == Guid.Empty && facilityID == Guid.Empty)
                {
                    // Error50596: Unsupported command sequence!  (Nicht unterstützte Befehlsfolge!)
                    resultSequence.Message = new Msg(this, eMsgLevel.Error, nameof(PAFPickingByMaterial), "OnScanEvent(20)", 20, "Error50596");
                    resultSequence.State = BarcodeSequenceBase.ActionState.Cancelled;
                }
                else
                {
                    ScannedFacilityCharge.ValueT = facilityChargeID;
                    
                    // Info50089: A new lot was activated or changed. (Neue Charge wurde aktiviert bzw. gewechselt.)
                    resultSequence.Message = new Msg(this, eMsgLevel.Info, nameof(PAFPickingByMaterial), "OnScanEvent(40)", 40, "Info50089");
                    resultSequence.State = BarcodeSequenceBase.ActionState.Completed;
                    
                }
            }
            return resultSequence;
        }
    }
}
