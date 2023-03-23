using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using gip.mes.processapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'Manual addition'}de{'Manuelle Zugabe'}", Global.ACKinds.TACBSO, Global.ACStorableTypes.NotStorable, true, true, SortIndex = 200)]
    public class BSOManualAddition : BSOManualWeighing
    {
        public BSOManualAddition(ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "") : 
            base(acType, content, parentACObject, parameter, acIdentifier)
        {
        }

        public override bool ACInit(Global.ACStartTypes startChildMode = Global.ACStartTypes.Automatic)
        {
            return base.ACInit(startChildMode);
        }


        public override double ScaleActualWeight => ScaleAddAcutalWeight;

        public override double ScaleAddAcutalWeight
        {
            get => base.ScaleAddAcutalWeight;
            set
            {
                _ScaleAddAcutalWeight = value;
                ScaleBckgrState = DetermineBackgroundState(_TolerancePlus, _ToleranceMinus, TargetWeight, value);
                OnPropertyChanged("ScaleActualWeight");
                OnPropertyChanged("ScaleDifferenceWeight");
            }
        }

        public override double ScaleRealWeight 
        { 
            get => base.ScaleRealWeight; 
            set
            {
                _ScaleRealWeight = value;
                ScaleBckgrState = DetermineBackgroundState(_TolerancePlus, _ToleranceMinus, TargetWeight, ScaleAddAcutalWeight);
                OnPropertyChanged("ScaleActualWeight");
                OnPropertyChanged("ScaleDifferenceWeight");
            }
        }

        public override IACComponent GetTargetFunction(IEnumerable<IACComponent> processModuleChildrenComponents)
        {
            using (ACMonitor.Lock(Database.QueryLock_1X000))
            {
                return processModuleChildrenComponents.FirstOrDefault(c => typeof(PAFManualAddition).IsAssignableFrom(c.ComponentClass.ObjectType));
            }
        }

        public override ScaleBackgroundState? OnDetermineBackgroundState(double? tolPlus, double? tolMinus, double target, double actual)
        {
            bool? suggestQOnPosting = SelectedWeighingMaterial?.PosRelation?.SourceProdOrderPartslistPos?.BasedOnPartslistPos?.SuggestQuantQOnPosting;
            if (suggestQOnPosting.HasValue && suggestQOnPosting.Value && actual > 0)
                return ScaleBackgroundState.InTolerance;

            return null;
        }

        public override bool OnHandleWeighingComponentInfo(WeighingComponentInfo compInfo)
        {
            if (compInfo.IsManualAddition)
                return true;
            return false;
        }

        public override List<ACRef<IACComponentPWNode>> FindWFNodes(IEnumerable<ACChildInstanceInfo> availablePWNodes, IACComponentPWNode pwGroup)
        {
            List<ACRef<IACComponentPWNode>> result = new List<ACRef<IACComponentPWNode>>();

            var nodes = availablePWNodes.Where(c => typeof(PWManualAddition).IsAssignableFrom(c.ACType.ValueT.ObjectType)).ToArray();

            foreach (var node in nodes)
            {
                IACComponentPWNode pwNode = pwGroup.ACUrlCommand(node.ACUrlParent + "\\" + node.ACIdentifier) as IACComponentPWNode;
                if (pwNode == null)
                {
                    //Error50331: The PWManualAddition node with ACUrl: {0} is not available!
                    Messages.Error(this, "Error50331", false, node.ACUrlParent + "\\" + node.ACIdentifier);
                    continue;
                }
                var refPWNode = new ACRef<IACComponentPWNode>(pwNode, this);
                result.Add(refPWNode);
            }

            return result;
        }

        public override void OnLoadPWConfiguration(ACMethod acMethod)
        {
            var onlyAck = acMethod.ParameterValueList.GetACValue("OnlyAcknowledge");
            if (onlyAck != null)
                OnlyAcknowledge = onlyAck.ParamAsBoolean;
        }

        public bool OnlyAcknowledge
        {
            get;
            set;
        }


        public override FacilityChargeItem SelectedFacilityCharge 
        { 
            get => base.SelectedFacilityCharge;
            set
            {
                base.SelectedFacilityCharge = value;

                if (OnlyAcknowledge)
                {
                    if (value != null
                        && (_SelFacilityCharge == null
                            || _SelFacilityCharge.FacilityChargeID != value.FacilityChargeID
                            /*|| (_PAFManuallyAddedQuantity != null && Math.Abs(_PAFManuallyAddedQuantity.ValueT) <= Double.Epsilon)*/))
                    {
                        _SelFacilityCharge = value;

                        double diff = SelectedWeighingMaterial.TargetQuantity;

                        bool? suggestQOnPosting = SelectedWeighingMaterial.PosRelation?.SourceProdOrderPartslistPos?.BasedOnPartslistPos?.SuggestQuantQOnPosting;
                        if (suggestQOnPosting.HasValue && suggestQOnPosting.Value)
                        {
                            diff = value.StockQuantityUOM;
                            SelectedWeighingMaterial.AddValue = diff;
                        }
                        else
                        {
                            ACMethod pafMethod = GetPAFCurrentACMethod();
                            if (pafMethod != null)
                            {
                                double targetQuantity = pafMethod.ParameterValueList.GetDouble("TargetQuantity");
                                diff = targetQuantity - ScaleActualWeight;
                            }

                            if (value.StockQuantityUOM <= 0.0001 || value.StockQuantityUOM > diff)
                                SelectedWeighingMaterial.AddValue = diff;
                            else
                                SelectedWeighingMaterial.AddValue = value.StockQuantityUOM;
                        }

                        if (SelectedWeighingMaterial.WeighingMatState == WeighingComponentState.Selected)
                        {
                            SelectedWeighingMaterial.ChangeComponentState(WeighingComponentState.InWeighing, DatabaseApp);
                        }

                        if (diff > 0.00001)
                        {
                            var paf = CurrentPAFManualWeighing;
                            if (paf != null)
                            {
                                double newQuantity = SelectedWeighingMaterial.AddKg(_PAFManuallyAddedQuantity.ValueT);
                                paf.ExecuteMethod(nameof(PAFManualAddition.ChangeManuallyAddedQuantity), newQuantity, value.FacilityChargeID);
                            }
                        }
                        else
                        {
                            AddKg();
                        }
                    }
                    else if (_SelFacilityCharge != null && value == null && SelectedWeighingMaterial == null)
                    {
                        _SelFacilityCharge = null;
                    }
                }
            }
        }
    }
}
