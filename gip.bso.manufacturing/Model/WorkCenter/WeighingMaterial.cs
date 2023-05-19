using System.Runtime.CompilerServices;
using gip.core.autocomponent;
using gip.core.datamodel;
using gip.mes.autocomponent;
using System;
using System.Collections.Generic;
using System.Linq;
using VD = gip.mes.datamodel;
using System.Text;
using System.Threading.Tasks;
using gip.mes.processapplication;
using System.ComponentModel;
using System.Data;
using gip.core.processapplication;
using System.Threading;
using gip.mes.facility;
using gip.mes.datamodel;

namespace gip.bso.manufacturing
{
    [ACClassInfo(Const.PackName_VarioManufacturing, "en{'WeighingMaterial'}de{'WeighingMaterial'}", Global.ACKinds.TACSimpleClass, Global.ACStorableTypes.NotStorable, true, true)]
    public class WeighingMaterial : IACObject, INotifyPropertyChanged
    {
        #region c'tors

        public WeighingMaterial(VD.ProdOrderPartslistPosRelation posRelation, WeighingComponentState state, core.datamodel.ACClassDesign materialIconDesign, IACObject parent)
        {
            PosRelation = posRelation;
            MaterialUnitList = PosRelation?.SourceProdOrderPartslistPos?.Material.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).ToArray();
            WeighingMatState = state;
            MaterialIconDesign = materialIconDesign;
            _ParentACObject = parent;
            OnPropertyChanged(nameof(MaterialUnitList));
        }

        public WeighingMaterial(VD.PickingPos pickingPos, WeighingComponentState state, core.datamodel.ACClassDesign materialIconDesign)
        {
            PickingPosition = pickingPos;

            MaterialName = pickingPos.Material.MaterialName1;
            MaterialNo = pickingPos.Material.MaterialNo;
            IsLotManaged = pickingPos.Material.IsLotManaged;
            UnitName = pickingPos.Material?.BaseMDUnit?.MDUnitName;
            Sequence = pickingPos.Sequence;

            MaterialUnitList = PosRelation?.SourceProdOrderPartslistPos?.Material.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).ToArray();
            WeighingMatState = state;
            MaterialIconDesign = materialIconDesign;
            //_ParentACObject = parent;
            OnPropertyChanged(nameof(MaterialUnitList));
        }

        #endregion

        #region Properties

        private VD.ProdOrderPartslistPosRelation _PosRelation;
        [ACPropertyInfo(100)]
        public VD.ProdOrderPartslistPosRelation PosRelation
        {
            get => _PosRelation;
            set
            {
                _PosRelation = value;
                if (_PosRelation == null)
                {
                    MaterialName = null;
                    MaterialNo = null;
                    IsLotManaged = false;
                    TargetQuantity = 0;
                    ActualQuantity = 0;
                }
                else
                {
                    MaterialName = _PosRelation.SourceProdOrderPartslistPos?.Material?.MaterialName1;
                    MaterialNo = _PosRelation.SourceProdOrderPartslistPos?.Material?.MaterialNo;
                    IsLotManaged = _PosRelation.SourceProdOrderPartslistPos != null && _PosRelation.SourceProdOrderPartslistPos.Material != null ?
                                   _PosRelation.SourceProdOrderPartslistPos.Material.IsLotManaged : false;
                    UnitName = _PosRelation.SourceProdOrderPartslistPos?.Material?.BaseMDUnit?.MDUnitName;

                    if (_PosRelation.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)VD.MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Completed ||
                        _PosRelation.MDProdOrderPartslistPosState.MDProdOrderPartslistPosStateIndex == (short)VD.MDProdOrderPartslistPosState.ProdOrderPartslistPosStates.Cancelled)
                    {
                        TargetQuantity = _PosRelation.TargetWeight;
                        ActualQuantity = _PosRelation.ActualWeight;
                        TargetQtyInUnits = _PosRelation.TargetQuantityUOM;
                        ActualQtyInUnits = _PosRelation.ActualQuantityUOM;
                    }
                    else
                    {
                        TargetQuantity = Math.Abs(PosRelation.RemainingDosingWeight);
                        ActualQuantity = 0;
                        TargetQtyInUnits = PosRelation.RemainingDosingQuantityUOM;
                        ActualQtyInUnits = 0;
                    }

                    Sequence = _PosRelation.Sequence;

                }
                OnPropertyChanged();
            }
        }

        [ACPropertyInfo(100)]
        public VD.PickingPos PickingPosition
        {
            get;
            set;
        }

        [ACPropertyInfo(100)]
        public int Sequence
        {
            get;
            set;
        }

        private string _MaterialName;
        [ACPropertyInfo(101, "", "en{'Material Desc. 1'}de{'Materialbez. 1'}")]
        public string MaterialName
        {
            get => _MaterialName;
            set
            {
                _MaterialName = value;
                OnPropertyChanged();
            }
        }

        private string _MaterialNo;
        [ACPropertyInfo(102, "", "en{'Material No.'}de{'Material-Nr.'}")]
        public string MaterialNo
        {
            get => _MaterialNo;
            set
            {
                _MaterialNo = value;
                OnPropertyChanged();
            }
        }

        private bool _IsLotManaged = false;
        [ACPropertyInfo(103)]
        public bool IsLotManaged
        {
            get => _IsLotManaged;
            set
            {
                _IsLotManaged = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        internal void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private WeighingComponentState _WeighingMaterialState;
        [ACPropertyInfo(104, "", "en{'Weighed'}de{'Gewogen'}")]
        public WeighingComponentState WeighingMatState
        {
            get => _WeighingMaterialState;
            set
            {

                _WeighingMaterialState = value;
                OnPropertyChanged();
            }
        }

        [ACPropertyInfo(105)]
        public core.datamodel.ACClassDesign MaterialIconDesign
        {
            get;
            set;
        }

        private double _TargetQuantity;
        /// <summary>
        /// Target Quantity is weight in kg!
        /// </summary>
        [ACPropertyInfo(106, "", "en{'Target weight in kg'}de{'Sollgewicht in kg'}")]
        public double TargetQuantity
        {
            get => _TargetQuantity;
            set
            {
                _TargetQuantity = value;
                OnPropertyChanged();
            }
        }

        private double _ActualQuantity;
        /// <summary>
        /// Target Quantity is weight in kg!
        /// </summary>
        [ACPropertyInfo(107, "", "en{'Actual weight in kg'}de{'Aktuelles Gewicht in kg'}")]
        public double ActualQuantity
        {
            get => _ActualQuantity;
            set
            {
                _ActualQuantity = value;
                OnPropertyChanged();
            }
        }

        private string _UnitName;
        [ACPropertyInfo(107, "", "en{'Unit'}de{'Einheit'}")]
        public string UnitName
        {
            get => _UnitName;
            set
            {
                _UnitName = value;
                OnPropertyChanged();
            }
        }

        private double _TargetQtyInUnits;
        [ACPropertyInfo(106, "", "en{'Target quantity in Unit'}de{'Sollmenge in Einheiten'}")]
        public double TargetQtyInUnits
        {
            get => _TargetQtyInUnits;
            set
            {
                _TargetQtyInUnits = value;
                OnPropertyChanged();
            }
        }

        private double _ActualQtyInUnits;
        [ACPropertyInfo(107, "", "en{'Actual weight in kg'}de{'Sollemgne in Einheiten'}")]
        public double ActualQtyInUnits
        {
            get => _ActualQtyInUnits;
            set
            {
                _ActualQtyInUnits = value;
                OnPropertyChanged();
            }
        }

        private bool _DiffWeighOnEnd;
        [ACPropertyInfo(9999)]
        public bool DiffWeighOnEnd
        {
            get => _DiffWeighOnEnd;
            set
            {
                _DiffWeighOnEnd = value;
                OnPropertyChanged();
            }
        }

        public Guid? QuantInWeighing
        {
            get;
            set;
        }

        public BSOManualWeighing ParentBSO
        {
            get => ParentACObject as BSOManualWeighing;
        }

        #endregion

        #region IACObject

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        public string ACIdentifier => PosRelation?.ACIdentifier;

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        public string ACCaption => PosRelation?.ACCaption;

        /// <summary>
        /// Metadata (iPlus-Type) of this instance. ATTENTION: IACType are EF-Objects. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>  iPlus-Type (EF-Object from ACClass*-Tables)</value>
        public IACType ACType => this.ReflectACType();

        /// <summary>
        /// A "content list" contains references to the most important data that this instance primarily works with. It is primarily used to control the interaction between users, visual objects, and the data model in a generic way. For example, drag-and-drop or context menu operations. A "content list" can also be null.
        /// </summary>
        /// <value> A nullable list ob IACObjects.</value>
        public IEnumerable<IACObject> ACContentList => this.ReflectGetACContentList();

        private IACObject _ParentACObject = null;
        /// <summary>
        /// Returns the parent object
        /// </summary>
        /// <value>Reference to the parent object</value>
        public IACObject ParentACObject
        {
            get { return _ParentACObject; }
        }

        /// <summary>
        /// The ACUrlCommand is a universal method that can be used to query the existence of an instance via a string (ACUrl) to:
        /// 1. get references to components,
        /// 2. query property values,
        /// 3. execute method calls,
        /// 4. start and stop Components,
        /// 5. and send messages to other components.
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>Result if a property was accessed or a method was invoked. Void-Methods returns null.</returns>
        public object ACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectACUrlCommand(acUrl, acParameter);
        }

        /// <summary>
        /// This method is called before ACUrlCommand if a method-command was encoded in the ACUrl
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>true if ACUrlCommand can be invoked</returns>
        public bool IsEnabledACUrlCommand(string acUrl, params object[] acParameter)
        {
            return this.ReflectIsEnabledACUrlCommand(acUrl, acParameter);
        }

        /// <summary>
        /// Returns a ACUrl relatively to the passed object.
        /// If the passed object is null then the absolute path is returned
        /// </summary>
        /// <param name="rootACObject">Object for creating a realtive path to it</param>
        /// <returns>ACUrl as string</returns>
        public string GetACUrl(IACObject rootACObject = null)
        {
            return this.ReflectGetACUrl(rootACObject);
        }

        /// <summary>
        /// Method that returns a source and path for WPF-Bindings by passing a ACUrl.
        /// </summary>
        /// <param name="acUrl">ACUrl of the Component, Property or Method</param>
        /// <param name="acTypeInfo">Reference to the iPlus-Type (ACClass)</param>
        /// <param name="source">The Source for WPF-Databinding</param>
        /// <param name="path">Relative path from the returned source for WPF-Databinding</param>
        /// <param name="rightControlMode">Information about access rights for the requested object</param>
        /// <returns><c>true</c> if binding could resolved for the passed ACUrl<c>false</c> otherwise</returns>
        public bool ACUrlBinding(string acUrl, ref IACType acTypeInfo, ref object source, ref string path, ref Global.ControlModes rightControlMode)
        {
            return this.ReflectACUrlBinding(acUrl, ref acTypeInfo, ref source, ref path, ref rightControlMode);
        }

        #endregion

        #region Properties => MaterialUnit/Quantity

        private double? _AddValue;
        [ACPropertyInfo(108, "", "en{'Quantity to add'}de{'Menge zum Hinzufügen'}")]
        public double? AddValue
        {
            get => _AddValue;
            set
            {
                _AddValue = value;
                OnPropertyChanged();
            }
        }

        private VD.MaterialUnit _SelectedMaterialUnit;
        [ACPropertySelected(109, "ManAddMaterialUnit", "en{'Material unit to add'}de{'Materialeinheit zum Hinzufügen'}")]
        public VD.MaterialUnit SelectedMaterialUnit
        {
            get => _SelectedMaterialUnit;
            set
            {
                _SelectedMaterialUnit = value;
                OnPropertyChanged();
            }
        }

        private IEnumerable<VD.MaterialUnit> _MaterialUnitList;
        [ACPropertyList(110, "ManAddMaterialUnit")]
        public IEnumerable<VD.MaterialUnit> MaterialUnitList
        {
            get
            {
                //return _MaterialUnitList;
                if (PosRelation != null && PosRelation.SourceProdOrderPartslistPos != null)
                    return PosRelation.SourceProdOrderPartslistPos.Material.MaterialUnit_Material.OrderBy(c => c.ToMDUnit != null ? c.ToMDUnit.SortIndex : 0).ToArray();
                return null;
            }
            set
            {
                _MaterialUnitList = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        public double AddKg(double currentWeight)
        {
            if (AddValue.HasValue)
            {
                currentWeight += AddValue.Value;
                return currentWeight;
            }

            if (SelectedMaterialUnit != null)
            {
                currentWeight += SelectedMaterialUnit.Multiplier;
                return currentWeight;
            }

            var calcWeight = currentWeight + 1;
            return calcWeight;
        }

        public double RemoveKg(double currentWeight)
        {
            if (currentWeight <= 0)
                return currentWeight;

            if (AddValue.HasValue)
            {
                if (currentWeight - AddValue.Value > 0)
                    currentWeight -= AddValue.Value;
                else
                    currentWeight = 0;
                return currentWeight;
            }

            if (SelectedMaterialUnit != null)
            {
                if (currentWeight - SelectedMaterialUnit.Multiplier > 0)
                    currentWeight -= SelectedMaterialUnit.Multiplier;
                else
                    currentWeight = 0;
                return currentWeight;
            }

            var calcWeight = currentWeight - 1;
            return calcWeight--;
        }

        public void ChangeComponentState(WeighingComponentState newState, VD.DatabaseApp dbApp)
        {
            if (_WeighingMaterialState == WeighingComponentState.InWeighing && newState >= WeighingComponentState.WeighingCompleted)
            {
                try
                {
                    using (ACMonitor.Lock(dbApp.QueryLock_1X000))
                    {
                        double remainingDosingWeight = 0;

                        if (PosRelation != null)
                        {
                            PosRelation.AutoRefresh();
                            remainingDosingWeight = PosRelation.RemainingDosingWeight;
                        }
                        else if (PickingPosition != null)
                        {
                            PickingPosition.AutoRefresh();
                            PickingPosition.OnLocalPropertyChanged(nameof(PickingPosition.ActualQuantityUOM));
                            remainingDosingWeight = PickingPosition.RemainingDosingWeight;
                        }

                        ActualQuantity = TargetQuantity + remainingDosingWeight;
                    }
                }
                catch
                {

                }
            }
            else if ((_WeighingMaterialState == WeighingComponentState.Aborted || _WeighingMaterialState == WeighingComponentState.WeighingCompleted) &&
                      (newState == WeighingComponentState.ReadyToWeighing))
            {
                try
                {
                    using (ACMonitor.Lock(dbApp.QueryLock_1X000))
                    {
                        double remainingDosingWeight = 0;

                        if (PosRelation != null)
                        {
                            PosRelation.AutoRefresh();
                            PosRelation.FacilityBooking_ProdOrderPartslistPosRelation.AutoLoad(PosRelation.FacilityBooking_ProdOrderPartslistPosRelationReference, PosRelation);
                            remainingDosingWeight = PosRelation.RemainingDosingWeight;
                        }
                        else if (PickingPosition != null)
                        {
                            PickingPosition.AutoRefresh();
                            PickingPosition.OnLocalPropertyChanged(nameof(PickingPosition.ActualQuantityUOM));
                            PickingPosition.FacilityBooking_PickingPos.AutoLoad(PickingPosition.FacilityBooking_PickingPosReference, PickingPosition);
                            remainingDosingWeight = PickingPosition.RemainingDosingWeight;
                        }

                        TargetQuantity = Math.Abs(remainingDosingWeight);
                    }
                    ActualQuantity = 0;
                }
                catch
                {

                }
            }

            bool? diffWeighing = ParentBSO?.DiffWeighing;

            if(diffWeighing.HasValue && diffWeighing.Value)
            {
                try
                {
                    PosRelation.FacilityPreBooking_ProdOrderPartslistPosRelation.AutoLoad(PosRelation.FacilityPreBooking_ProdOrderPartslistPosRelationReference, PosRelation);

                    var prebooking = PosRelation.FacilityPreBooking_ProdOrderPartslistPosRelation.FirstOrDefault();
                    DiffWeighOnEnd = prebooking != null;
                    if (DiffWeighOnEnd)
                        QuantInWeighing = prebooking?.OutwardFacilityCharge?.FacilityChargeID;

                    PosRelation.AutoRefresh();
                    ActualQuantity = TargetQuantity + PosRelation.RemainingDosingWeight;


                }
                catch (Exception e)
                {
                    dbApp.Root().Messages.LogException(nameof(WeighingMaterial), "ChangeComponentState(DiffWeighing)", e);
                }
            }

            if (newState != WeighingComponentState.PartialCompleted)
                WeighingMatState = newState;
            ParentBSO?.OnComponentStateChanged(this);
        }

        public void RefreshFromPickingPos(PickingPos pos)
        {
            if (PickingPosition == null)
                return;

            TargetQuantity = pos.TargetQuantityUOM;
            ActualQuantity = pos.ActualQuantityUOM;
        }

        #endregion
    }
}
