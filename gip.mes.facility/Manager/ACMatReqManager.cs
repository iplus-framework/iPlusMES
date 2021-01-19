using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gip.core.datamodel;
using gip.core.autocomponent;
using gip.mes.datamodel;
using System.Globalization;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "", Global.ACKinds.TPARole, Global.ACStorableTypes.NotStorable, false, false)]
    public class ACMatReqManager : PARole
    {
        #region ctor's
        public ACMatReqManager(core.datamodel.ACClass acType, IACObject content, IACObject parentACObject, ACValueList parameter, string acIdentifier = "")
            : base(acType, content, parentACObject, parameter, acIdentifier)
        {

        }
        #endregion

        #region Properties
        public const string C_DefaultServiceACIdentifier = "ACMatReqManager";
        #endregion

        #region static Methods
        public static ACMatReqManager GetServiceInstance(ACComponent requester)
        {
            return GetServiceInstance<ACMatReqManager>(requester, C_DefaultServiceACIdentifier, CreationBehaviour.OnlyLocal);
        }

        public static ACRef<ACMatReqManager> ACRefToServiceInstance(ACComponent requester)
        {
            ACMatReqManager serviceInstance = GetServiceInstance(requester);
            if (serviceInstance != null)
                return new ACRef<ACMatReqManager>(serviceInstance, requester);
            return null;
        }
        #endregion

        #region private 
        #endregion

        #region Methods

        public MsgWithDetails CheckMaterialsRequirement(DatabaseApp dbApp, ProdOrderBatchPlan batchPlan)
        {
            MsgWithDetails msg = new MsgWithDetails();
            Msg mainMsg = null;
            bool isOneMaterialMissing = false;
            double maxProducibleQuantity = double.MaxValue;

            var requiredMaterials = batchPlan.ProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist.Where(x => x.MaterialPosTypeIndex == (short)(GlobalApp.MaterialPosTypes.OutwardRoot) &&
                                                                                                                     x.ParentProdOrderPartslistPosID == null)
                                                                .Select(c => new Tuple<Material, double>(c.Material, batchPlan.TotalSize * (c.TargetQuantity / c.ProdOrderPartslist.TargetQuantity)));

            var requiredMaterialsActualQuantity = requiredMaterials.Select(c => new Tuple<Material, double>(c.Item1, c.Item1.CurrentMaterialStock.StockQuantity)).ToList();

            var batches = dbApp.ProdOrderBatchPlan.Where(c => c.PlanStateIndex == (short)GlobalApp.BatchPlanState.AutoStart ||
                                                                    c.PlanStateIndex == (short)GlobalApp.BatchPlanState.InProcess ||
                                                                    c.PlanStateIndex == (short)GlobalApp.BatchPlanState.Created &&
                                                                    c.ProdOrderBatchPlanID != batchPlan.ProdOrderBatchPlanID).ToList();

            var calculatedMaterialRequirements = batches.Select(x => x.ProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                              .Where(c => requiredMaterials.Any(n => n.Item1 == c.Material))
                              .Select(k => new Tuple<ProdOrderPartslistPos, double>(k, Math.Abs(x.RemainingQuantity) * (k.TargetQuantity / k.ProdOrderPartslist.TargetQuantity))))
                              .SelectMany(t => t);

            var reservedMaterialsQuantity = calculatedMaterialRequirements.GroupBy(x => x.Item1.Material).Select(c => new Tuple<Material, double>(c.Key, c.Sum(k => k.Item2))).ToList(); 

            foreach(var item in requiredMaterials)
            {
                Msg tempMainMsg = null;
                Msg result = CheckMaterialRequirement(ref isOneMaterialMissing, ref maxProducibleQuantity, item, requiredMaterialsActualQuantity, reservedMaterialsQuantity, batchPlan, out tempMainMsg);
                if (result != null)
                    msg.AddDetailMessage(result);
                if (tempMainMsg != null)
                    mainMsg = tempMainMsg;
            }

            if (mainMsg != null)
                msg.Message = mainMsg.Message;

            if (msg.MsgDetails.Any())
                return msg;
            return null;
        }

        private Msg CheckMaterialRequirement(ref bool isOneMaterialMissing, ref double maxProducibleQuantity, Tuple<Material, double> currentMaterial, IEnumerable<Tuple<Material, double>> reqMatActQuantity, 
                                              IEnumerable<Tuple<Material, double>> reservedMatQuantity, ProdOrderBatchPlan batchPlan, out Msg mainMsg)
        {
            mainMsg = null;
            var actualMaterialQuantity = reqMatActQuantity.FirstOrDefault(c => c.Item1 == currentMaterial.Item1);
            var reservedMaterialQuantity = reservedMatQuantity.FirstOrDefault(c => c.Item1 == currentMaterial.Item1);

            double availableQuantity = actualMaterialQuantity.Item2 - Math.Abs(reservedMaterialQuantity.Item2);
            double calc = availableQuantity - Math.Abs(currentMaterial.Item2);
            if (availableQuantity < 0)
            {
                isOneMaterialMissing = true;
                mainMsg = new Msg() { Message = "Current batch plan is not producible!" };

                return new Msg()
                {
                    Message = string.Format("{0} {1} of material {2} {3} is missing for all active batch plans.",
                        Math.Round(Math.Abs(calc), 2), currentMaterial.Item1.BaseMDUnit.MDUnitName, currentMaterial.Item1.MaterialNo, currentMaterial.Item1.MaterialName1)
                };
            }
            else if (calc < 0)
            {
                if (!isOneMaterialMissing)
                    mainMsg = CalculateMaxProducibleQuantity(ref isOneMaterialMissing, ref maxProducibleQuantity, batchPlan, currentMaterial.Item2, availableQuantity);

                return new Msg()
                {
                    Message = string.Format("{0} {1} of material {2} {3} is missing for the current batch plan.",
                        Math.Round(Math.Abs(availableQuantity - currentMaterial.Item2), 2), currentMaterial.Item1.BaseMDUnit.MDUnitName, 
                        currentMaterial.Item1.MaterialNo, currentMaterial.Item1.MaterialName1)
                };
            }
            return null;
        }

        private Msg CalculateMaxProducibleQuantity(ref bool isOneMaterialMissing, ref double maxProducibleQuantity, ProdOrderBatchPlan batchPlan, double targetQ, double actualQ)
        {
            double tempQuantity = (batchPlan.TotalSize / targetQ) * actualQ;
            if (tempQuantity < maxProducibleQuantity)
                maxProducibleQuantity = tempQuantity;

            double maxBatch = Math.Truncate(maxProducibleQuantity / batchPlan.BatchSize);
            string batchText = string.Format(" or {0} batches per {1} {2}.", maxBatch, batchPlan.BatchSize, batchPlan.ProdOrderPartslistPos.Material.BaseMDUnit.MDUnitName);
            if (double.IsInfinity(maxBatch))
                batchText = ".";
            return new Msg() { Message = string.Format("Maximum producible quantity is {0} {1}{2} ", Math.Round(maxProducibleQuantity, 2), 
                                                        batchPlan.ProdOrderPartslistPos.Material.BaseMDUnit.MDUnitName, batchText) };
        }

        public IEnumerable<Msg> CheckMaterialsRequirement(DatabaseApp dbApp, IEnumerable<ProdOrderBatchPlan> batchPlans)
        {
            List<Msg> msg = new List<Msg>();

            //var batchPlanToCheck = batchPlans.Where(c => c.PlanStateIndex == (short)GlobalApp.BatchPlanState.AutoStart ||
            //                                             c.PlanStateIndex == (short)GlobalApp.BatchPlanState.InProcess ||
            //                                             c.PlanStateIndex == (short)GlobalApp.BatchPlanState.Created).ToList();

            //var result = CalculateMaterialsRequirement(batchPlanToCheck);
            IEnumerable<MatReqResult> result = CheckMaterialsRequirementOverAllOrders(dbApp, batchPlans);

            result = result.Where(c => !c.IsDiffPositive);

            if (result.Any())
            {
                foreach (MatReqResult item in result)
                {
                    msg.Add(new Msg()
                    {
                        ACIdentifier = "Material requirement",
                        MessageLevel = eMsgLevel.Warning,
                        Message = Root.Environment.TranslateMessage(this, "Warning50029", Math.Abs(item.DiffQuantity).ToString("N2", CultureInfo.CurrentCulture), 
                                                                          item.CurrentMaterial.BaseMDUnit.MDUnitName, item.CurrentMaterial.MaterialNo, item.CurrentMaterial.MaterialName1)
                    });
                }
            }

            return msg;
        }
        
        public IEnumerable<MatReqResult> CalculateMaterialsRequirement(IEnumerable<ProdOrderBatchPlan> batchPlans)
        {
            var result = batchPlans.Select(batchPlan => batchPlan.ProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                                   .Where(x => x.MaterialPosTypeIndex == (short)(GlobalApp.MaterialPosTypes.OutwardRoot) && x.ParentProdOrderPartslistPosID == null)
                                   .Select(pos => new Tuple<Material, double>(pos.Material, (batchPlan.RemainingQuantity / pos.ProdOrderPartslist.TargetQuantity)
                                                                                            * pos.TargetQuantity)))
                                   .SelectMany(x => x).GroupBy(c => c.Item1)
                                   .Select(k => new MatReqResult(k.Key, k.Sum(t => t.Item2), k.Key.MaterialStock_Material.Any() ? k.Key.MaterialStock_Material.Sum(q => q.AvailableQuantity) : 0));
            return result;
        }

        public IEnumerable<MatReqResult> CheckMaterialsRequirementOverAllOrders(DatabaseApp dbApp, IEnumerable<ProdOrderBatchPlan> batchPlans)
        {
            ProdOrderBatchPlan[] batchPlanToCheck = batchPlans.Where(c => c.PlanStateIndex == (short)GlobalApp.BatchPlanState.AutoStart 
                                                                       || c.PlanStateIndex == (short)GlobalApp.BatchPlanState.InProcess 
                                                                       || c.PlanStateIndex == (short)GlobalApp.BatchPlanState.Created).ToArray();

            var currentOrderResult = batchPlanToCheck.Select(batchPlan => batchPlan.ProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                                                     .Where(x => x.MaterialPosTypeIndex == (short)(GlobalApp.MaterialPosTypes.OutwardRoot) 
                                                              && x.ParentProdOrderPartslistPosID == null)
                                                     .Select(pos => new Tuple<Material, double>(pos.Material, (batchPlan.RemainingQuantity / pos.ProdOrderPartslist.TargetQuantity)
                                                                                                              * pos.TargetQuantity)))
                                                     .SelectMany(x => x).GroupBy(c => c.Item1)
                                                     .Select(k => new MatReqResult(k.Key, k.Sum(t => t.Item2), k.Key.MaterialStock_Material.Any() ? 
                                                                                          k.Key.MaterialStock_Material.Sum(q => q.AvailableQuantity) : 0)).ToArray();

            IEnumerable<ProdOrderBatchPlan> otherBatchPlans = dbApp.ProdOrderBatchPlan.Where(c => c.PlanStateIndex == (short)GlobalApp.BatchPlanState.InProcess)
                                                                                            .ToArray()
                                                                                            .Except(batchPlanToCheck);

            foreach (var item in currentOrderResult)
            {
                var reqQuantity = otherBatchPlans.Select(batchPlan => batchPlan.ProdOrderPartslist.ProdOrderPartslistPos_ProdOrderPartslist
                                          .Where(pos => pos.MaterialPosTypeIndex == (short)(GlobalApp.MaterialPosTypes.OutwardRoot)
                                                   && pos.ParentProdOrderPartslistPosID == null
                                                   && pos.MaterialID == item.CurrentMaterial.MaterialID).Sum(c => (batchPlan.RemainingQuantity / c.ProdOrderPartslist.TargetQuantity)
                                                                                                                  * c.TargetQuantity))
                                          .Sum();

                item.RequiredQuantity = item.RequiredQuantity + reqQuantity;
            }

            return currentOrderResult;
        }

        #endregion
    }

    [ACClassInfo(Const.PackName_VarioFacility, "", Global.ACKinds.TACSimpleClass)]
    public class MatReqResult : ACObjectItem
    {
        public MatReqResult(Material material, double requiredQ, double availableQ) : base ("")
        {
            CurrentMaterial = material;
            RequiredQuantity = requiredQ;
            AvailableQuantity = availableQ;
        }

        private Material _CurrentMaterial;
        [ACPropertyInfo(999,"","en{'Material'}de{'Material'}")]
        public Material CurrentMaterial
        {
            get
            {
                return _CurrentMaterial;
            }
            set
            {
                _CurrentMaterial = value;
                OnPropertyChanged("CurrentMaterial");
            }
        }

        private double _RequiredQuantity;
        [ACPropertyInfo(999, "", "en{'Required Quantity'}de{'Benötigte Menge'}")]
        public double RequiredQuantity
        {
            get
            {
                return _RequiredQuantity;
            }
            set
            {
                _RequiredQuantity = value;
                OnPropertyChanged("RequiredQuantity");
                OnPropertyChanged("DiffQuantity");
                OnPropertyChanged("IsDiffPositive");
            }
        }

        private double _AvailableQuantity;
        [ACPropertyInfo(999, "", "en{'Available Quantity'}de{'Verfügbare Menge'}")]
        public double AvailableQuantity
        {
            get
            {
                return _AvailableQuantity;
            }
            set
            {
                _AvailableQuantity = value;
                OnPropertyChanged("AvailableQuantity");
                OnPropertyChanged("DiffQuantity");
                OnPropertyChanged("IsDiffPositive");
            }
        }

        [ACPropertyInfo(999, "", "en{'Difference'}de{'Differenz'}")]
        public double DiffQuantity
        {
            get
            {
                return AvailableQuantity - RequiredQuantity;
            }
        }

        [ACPropertyInfo(999)]
        public bool IsDiffPositive
        {
            get
            {
                return DiffQuantity >= 0;
            }
        }
    }
}
