using gip.core.datamodel;
using gip.mes.datamodel;
using gip.mes.facility.TandTv3;
using gip.mes.facility.TandTv3.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TandTv3 = gip.mes.facility.TandTv3;

namespace gip.mes.facility
{
    [ACClassInfo(Const.PackName_VarioFacility, "en{'TandTv3Point'}de{'TandTv3Point'}", Global.ACKinds.TACDBA, Global.ACStorableTypes.NotStorable, false, false)]
    public class TandTv3Point : IACObject, INotifyPropertyChanged
    {

        #region ctor's

        public TandTv3Point()
        {
            MixPointID = Guid.NewGuid();

            // Step 

            // Lots
            OutwardLotsList = new List<FacilityLotModel>();
            InwardLot = new FacilityLotModel() { LotNo = TandTv3.TandTv3Command.EmptyLotName };

            // Bookings
            InwardBookings = new List<TandTv3.FacilityBookingPreveiw>();
            OutwardBookings = new List<TandTv3.FacilityBookingPreveiw>();

            // PreBookings
            InwardPreBookings = new List<TandTv3.FacilityPreBookingPreveiw>();
            OutwardPreBookings = new List<TandTv3.FacilityPreBookingPreveiw>();

            // Purchase
            DeliveryNotePositions = new List<DeliveryNotePos>();

            // Manufacturing
            Relations = new List<ProdOrderPartslistPosRelation>();
            // Store
            InwardFacilities = new Dictionary<string, FacilityPreview>();
            OutwardFacilities = new Dictionary<string, FacilityPreview>();
            //Machines = new List<gip.core.datamodel.ACClass>();
            InwardMachines = new List<gip.core.datamodel.ACClass>();
            OutwardMachines = new List<gip.core.datamodel.ACClass>();

            // items with LabOrder
            ItemsWithLabOrder = new List<MixPointLabOrder>();

            ProductionPositions = new List<ProdOrderPartslistPos>();
            BatchNoList = new List<string>();
            InOrderPositions = new List<InOrderPos>();
            OutOrderPositions = new List<OutOrderPos>();
            PickingPositions = new List<PickingPos>();

            // FacilityChargeIDs = new List<Guid>();

            // Material
            OutwardMaterials = new List<Material>();
        }

        #endregion

        #region TandTMixPoint items

        public Guid MixPointID { get; set; }

        public bool IsProcessFinished { get; set; }

        public TandTv3.TandTStep Step { get; set; }

        [ACPropertyInfo(1, "StepNo", "en{'Step'}de{'Schritt'}")]
        public int StepNo
        {
            get
            {
                if (Step == null) return 0;
                return Step.StepNo;
            }
        }

        [ACPropertyInfo(2, "BatchNos", "en{'Batch No.'}de{'Batch Nr.'}")]
        public string BatchNos
        {
            get
            {
                string batchNos = null;
                if (BatchNoList.Any())
                {
                    var items =
                        BatchNoList
                        .Distinct()
                        .OrderBy(c => c);
                    if (items.Count() <= 3)
                        batchNos = string.Join(",", items);
                    else
                        batchNos = string.Format(@"{0}-{1}", items.FirstOrDefault(), items.LastOrDefault());
                }
                return batchNos;
            }
        }

        #endregion

        #region Lots

        #region Lots -> Properties

        #region Inward to store, Produced Lots from Order, Output
        [ACPropertyInfo(11, "InwardLot", "en{'Lot info'}de{'Los Info'}")]
        public FacilityLotModel InwardLot { get; set; }

        [ACPropertyInfo(12, "InwardBatchNo", "en{'Batch No.'}de{'Batchnummer'}")]
        public string InwardBatchNo { get; set; }

        [ACPropertyInfo(13, "InwardBatchNos", "en{'Produced Batches'}de{'Hergestellte Batche'}")]
        public string InwardBatchNos
        {
            get
            {
                if (this is TandTv3PointPosGrouped)
                    return string.Join(",", (this as TandTv3PointPosGrouped).InwardBatchList.OrderBy(c => c));
                if (this is TandTv3Point)
                    return InwardBatchNo;
                return null;
            }
        }

        [ACPropertyInfo(14, "InwardLotsNos", "en{'Produced lots'}de{'Hergestellte Lose'}")]
        public virtual string InwardLotsNos
        {
            get
            {
                if (InwardLot != null) 
                    return InwardLot.LotNo;
                return TandTv3Command.EmptyLotName;
            }
        }
        #endregion


        #region Outward from Store, Used Lots in Order, Input
        [ACPropertyList(20, "OutwardLots", "en{'Used lots'}de{'Eingesetzte Lose'}")]
        public List<FacilityLotModel> OutwardLotsList { get; set; }

        private FacilityLotModel _SelectedOutwardLots;
        [ACPropertySelected(21, "OutwardLots")]
        public FacilityLotModel SelectedOutwardLots
        {
            get
            {
                return _SelectedOutwardLots;
            }
            set
            {
                if (_SelectedOutwardLots != value)
                {
                    _SelectedOutwardLots = value;
                    OnPropertyChanged("SelectedOutwardLots");
                }
            }
        }

        [ACPropertyInfo(22, "OutwardLotsNos", "en{'Used lots'}de{'Eingesetzte Lose'}")]
        public string OutwardLotsNos
        {
            get
            {
                if (OutwardLotsList == null || !OutwardLotsList.Any())
                    return "";
                return string.Join(",", OutwardLotsList.Select(c => c.LotNo).OrderBy(c => c));
            }
        }

        #endregion

        #endregion

        #region Lots -> Methods (Outward)

        public FacilityLotModel AddOutwardLot(FacilityLot facilityLot)
        {
            FacilityLotModel facilityLotModel = OutwardLotsList.FirstOrDefault(c => c.LotNo == facilityLot.LotNo);
            if (facilityLotModel == null)
            {
                facilityLotModel = new FacilityLotModel();
                facilityLotModel.LotNo = facilityLot.LotNo;
                facilityLotModel.ExternLotNo = facilityLot.ExternLotNo;
                facilityLotModel.ExternLotNo2 = facilityLot.ExternLotNo2;
                facilityLotModel.Comment = facilityLot.Comment;
                facilityLotModel.InsertDate = facilityLot.InsertDate;
                if (facilityLot.MaterialID != null)
                {
                    facilityLotModel.MaterialNo = facilityLot.Material.MaterialNo;
                    facilityLotModel.MaterialName1 = facilityLot.Material.MaterialName1;
                }
                facilityLotModel.FacilityLotID = facilityLot.FacilityLotID;
                OutwardLotsList.Add(facilityLotModel);
            }
            return facilityLotModel;
        }

        public void AddOutwardLotQuantity(FacilityBookingCharge fbc)
        {
            FacilityLot facilityLot = fbc.OutwardFacilityCharge.FacilityLot;
            FacilityLotModel facilityLotModel = null;
            if (facilityLot != null)
                facilityLotModel = AddOutwardLot(facilityLot);
            else
                facilityLotModel = new FacilityLotModel();
            if (string.IsNullOrEmpty(facilityLotModel.MaterialNo))
            {
                facilityLotModel.MaterialNo = fbc.OutwardMaterial.MaterialNo;
                facilityLotModel.MaterialName1 = fbc.OutwardMaterial.MaterialName1;
            }
            facilityLotModel.ActualQuantity += fbc.OutwardQuantityUOM;
        }

        public void AddOutwardLotQuantity(FacilityPreBooking facilityPreBooking)
        {
            FacilityLot facilityLot = facilityPreBooking.OutwardFacilityCharge.FacilityLot;
            FacilityLotModel facilityLotModel = AddOutwardLot(facilityLot);
            facilityLotModel.ActualQuantity += facilityPreBooking.OutwardQuantity ?? 0;
        }

        #endregion

        #region Lots -> Methods (Inward)
        public FacilityLotModel AddInwardLot(FacilityLot facilityLot)
        {
            InwardLot = new FacilityLotModel();
            InwardLot.LotNo = facilityLot.LotNo;
            InwardLot.ExternLotNo = facilityLot.ExternLotNo;
            InwardLot.ExternLotNo2 = facilityLot.ExternLotNo2;
            InwardLot.Comment = facilityLot.Comment;
            InwardLot.InsertDate = facilityLot.InsertDate;
            if (facilityLot.MaterialID != null)
            {
                InwardLot.MaterialNo = facilityLot.Material.MaterialNo;
                InwardLot.MaterialName1 = facilityLot.Material.MaterialName1;
            }
            InwardLot.FacilityLotID = facilityLot.FacilityLotID;
            return InwardLot;
        }

        public void AddInwardLotQuantity(FacilityBookingCharge fbc)
        {
            if (string.IsNullOrEmpty(InwardLot.MaterialNo))
            {
                InwardLot.MaterialNo = fbc.InwardMaterial.MaterialNo;
                InwardLot.MaterialName1 = fbc.InwardMaterial.MaterialName1;
            }
            InwardLot.ActualQuantity += fbc.InwardQuantityUOM;
        }
        #endregion

        #endregion

        #region Materials


        #region Materials -> Preview
        [ACPropertyInfo(3, "InwardMaterialNo", "en{'Material-No.'}de{'Artikel-Nr.'}")]
        public string InwardMaterialNo { get; set; }

        [ACPropertyInfo(4, "InwardMaterialName", "en{'Material Desc.'}de{'Materialbez.'}")]
        public string InwardMaterialName { get; set; }
        #endregion

        #region Materials -> Outward
        public List<Material> OutwardMaterials { get; set; }
        #endregion

        #region Materials -> Inward
        Material _InwardMaterial;
        public Material InwardMaterial 
        {
            get
            {
                return _InwardMaterial;
            }
            set
            {
                _InwardMaterial = value;
            }
        }
        #endregion

        #endregion

        #region Bookings

        public List<TandTv3.FacilityBookingPreveiw> InwardBookings { get; set; }
        public List<TandTv3.FacilityBookingPreveiw> OutwardBookings { get; set; }

        #region Bookings -> Methods

        public bool AddOutwardBooking(FacilityBookingCharge outwardFacilityBookingCharge)
        {
            bool added = false;
            if (!OutwardBookings.Select(c => c.FacilityBookingChargeNo).Contains(outwardFacilityBookingCharge.FacilityBookingChargeNo))
            {
                OutwardBookings.Add(new FacilityBookingPreveiw()
                {
                    FacilityBookingChargeNo = outwardFacilityBookingCharge.FacilityBookingChargeNo,
                    FacilityBookingNo = outwardFacilityBookingCharge.FacilityBooking.FacilityBookingNo,
                    InsertDate = outwardFacilityBookingCharge.InsertDate,
                    FacilityNo = outwardFacilityBookingCharge.OutwardFacilityID != null ? outwardFacilityBookingCharge.OutwardFacility.FacilityNo : "",
                    LotNo = outwardFacilityBookingCharge.OutwardFacilityChargeID != null && outwardFacilityBookingCharge.OutwardFacilityCharge.FacilityLotID != null ?
                            outwardFacilityBookingCharge.OutwardFacilityCharge.FacilityLot.LotNo : "",
                    FacilityChargeID = outwardFacilityBookingCharge.OutwardFacilityChargeID,
                    FacilityBookingChargeID = outwardFacilityBookingCharge.FacilityBookingChargeID,
                    FacilityBookingCharge = outwardFacilityBookingCharge
                });
                AddOutwardFacility(outwardFacilityBookingCharge, outwardFacilityBookingCharge.OutwardQuantityUOM);
                added = true;

                if (!OutwardMaterials.Select(c => c.MaterialNo).Contains(outwardFacilityBookingCharge.OutwardMaterial.MaterialNo))
                    OutwardMaterials.Add(outwardFacilityBookingCharge.OutwardMaterial);
            }
            return added;
        }

        public bool AddInwardBooking(FacilityBookingCharge inwardFacilityBookingCharge)
        {
            bool added = false;
            if (!InwardBookings.Select(c => c.FacilityBookingChargeNo).Contains(inwardFacilityBookingCharge.FacilityBookingChargeNo))
            {
                InwardBookings.Add(new FacilityBookingPreveiw()
                {
                    FacilityBookingChargeNo = inwardFacilityBookingCharge.FacilityBookingChargeNo,
                    FacilityBookingNo = inwardFacilityBookingCharge.FacilityBooking.FacilityBookingNo,
                    InsertDate = inwardFacilityBookingCharge.InsertDate,
                    FacilityNo = inwardFacilityBookingCharge.InwardFacilityID != null ? inwardFacilityBookingCharge.InwardFacility.FacilityNo : "",
                    LotNo = inwardFacilityBookingCharge.InwardFacilityChargeID != null && inwardFacilityBookingCharge.InwardFacilityCharge.FacilityLotID != null ?
                    inwardFacilityBookingCharge.InwardFacilityCharge.FacilityLot.LotNo : "",
                    FacilityChargeID = inwardFacilityBookingCharge.InwardFacilityChargeID,
                    FacilityBookingChargeID = inwardFacilityBookingCharge.FacilityBookingChargeID,
                    FacilityBookingCharge = inwardFacilityBookingCharge,
                });
                AddInwardFacility(inwardFacilityBookingCharge, inwardFacilityBookingCharge.InwardQuantityUOM);
                added = true;
            }
            return added;
        }
        #endregion

        #endregion

        #region FaciltiyPreBooking

        public List<TandTv3.FacilityPreBookingPreveiw> InwardPreBookings { get; set; }
        public List<TandTv3.FacilityPreBookingPreveiw> OutwardPreBookings { get; set; }

        #endregion

        #region Purchase

        [ACPropertyInfo(5, "DeliveryNo", "en{'Delivery Note No.'}de{'Lieferschein-Nr.'}")]
        public string DeliveryNo { get; set; }

        public List<DeliveryNotePos> DeliveryNotePositions { get; set; }

        #endregion

        #region Manufacturing

        public bool IsProductionPoint { get; set; }

        public bool IsInputPoint { get; set; }
        public bool IsOutputPoint { get; set; }

        public List<ProdOrderPartslistPos> ProductionPositions { get; set; }


        private double? _PositionsActualQuantityUOM;
        [ACPropertyInfo(6, "PositionsActualQuantityUOM", "en{'Actual Quantity (UOM)'}de{'Istmenge (BME)'}")]
        public double PositionsActualQuantityUOM
        {
            get
            {
                if (_PositionsActualQuantityUOM == null)
                    _PositionsActualQuantityUOM = ProductionPositions.Sum(x => x.ActualQuantityUOM);
                return _PositionsActualQuantityUOM ?? 0;
            }
        }

        public List<InOrderPos> InOrderPositions { get; set; }
        public List<OutOrderPos> OutOrderPositions { get; set; }
        public List<PickingPos> PickingPositions { get; set; }

        public List<ProdOrderPartslistPosRelation> Relations { get; set; }


        [ACPropertyInfo(7, "ProgramNo", "en{'Prod.-Order'}de{'Prod.-Auftrag'}")]
        public string ProgramNo { get; set; }

        public ProdOrder ProdOrder { get; set; }

        [ACPropertyInfo(8, "PartslistSequence", "en{'Production depth'}de{'Produktionstiefe'}")]
        public int PartslistSequence { get; set; }

        public List<string> BatchNoList { get; set; }

        #endregion

        #region Store

        #region Store -> Facility

        public Dictionary<string, FacilityPreview> InwardFacilities { get; set; }
        public Dictionary<string, FacilityPreview> OutwardFacilities { get; set; }

        public void AddInwardFacility(FacilityBookingCharge facilityBookingCharge, double quantity)
        {
            Material material = null;
            if (facilityBookingCharge.InwardFacility != null
                && facilityBookingCharge.InwardFacility.MDFacilityType != null
                && facilityBookingCharge.InwardFacility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
            {
                material = facilityBookingCharge.InwardMaterial;
            }
            AddFacility(InwardFacilities, facilityBookingCharge.InwardFacility, material, quantity);
        }

        public void AddOutwardFacility(FacilityBookingCharge facilityBookingCharge, double quantity)
        {
            Material material = null;
            if (facilityBookingCharge.OutwardFacility != null
                && facilityBookingCharge.OutwardFacility.MDFacilityType != null
                && facilityBookingCharge.OutwardFacility.MDFacilityType.MDFacilityTypeIndex == (short)FacilityTypesEnum.StorageBinContainer)
            {
                material = facilityBookingCharge.OutwardMaterial;
            }
            AddFacility(OutwardFacilities, facilityBookingCharge.OutwardFacility, material, quantity);
        }

        public void AddFacility(Dictionary<string, FacilityPreview> list, Facility facility, Material material, double quantity)
        {
            FacilityPreview facilityPreview = null;
            if (list.Keys.Contains(facility.FacilityNo))
                facilityPreview = list[facility.FacilityNo];
            else
            {
                facilityPreview = new FacilityPreview()
                {
                    FacilityNo = facility.FacilityNo,
                    FacilityID = facility.FacilityID,
                    VBiFacilityACClassID = facility.VBiFacilityACClassID,
                    FacilityName = facility.FacilityName
                };

                if (material != null)
                {
                    facilityPreview.MaterialNo = material.MaterialNo;
                    facilityPreview.MaterialName1 = material.MaterialName1;
                }

                list.Add(facility.FacilityNo, facilityPreview);
            }
            facilityPreview.StockQuantityUOM += quantity;
        }
        #endregion

        public List<gip.core.datamodel.ACClass> Machines { get; set; }
        public List<gip.core.datamodel.ACClass> InwardMachines { get; set; }
        public List<gip.core.datamodel.ACClass> OutwardMachines { get; set; }


        private string _InwardFacilityNo;
        public string InwardFacilityNo
        {
            get
            {
                if (_InwardFacilityNo == null)
                    _InwardFacilityNo = string.Join(",", InwardFacilities.Select(c => c.Key));
                return _InwardFacilityNo;
            }
        }

        #endregion

        #region LabOrders
        [ACPropertyInfo(9, "ExistLabOrder", "en{'Lab.-Order'}de{'Laborauftrag'}")]
        public bool ExistLabOrder { get; set; }

        public List<MixPointLabOrder> ItemsWithLabOrder { get; set; }
        #endregion

        #region IACObject

        public virtual string GetACCaption()
        {
            if (InwardLot != null)
                if (IsProductionPoint)
                    return string.Format(@"{0}({1}) {2:0.00}", InwardMaterialName, InwardLot.LotNo, PositionsActualQuantityUOM);
                else
                    return string.Format(@"{0}({1})", InwardMaterialName, InwardLot.LotNo);
            else
                return null;
        }

        /// <summary>Translated Label/Description of this instance (depends on the current logon)</summary>
        /// <value>  Translated description</value>
        public string ACCaption
        {
            get
            {
                return GetACCaption();
            }
        }

        /// <summary>
        /// Metadata (iPlus-Type) of this instance. ATTENTION: IACType are EF-Objects. Therefore the access to Navigation-Properties must be secured using the QueryLock_1X000 of the Global Database-Context!
        /// </summary>
        /// <value>  iPlus-Type (EF-Object from ACClass*-Tables)</value>
        public IACType ACType
        {
            get
            {
                return this.ReflectACType();
            }
        }

        /// <summary>
        /// A "content list" contains references to the most important data that this instance primarily works with. It is primarily used to control the interaction between users, visual objects, and the data model in a generic way. For example, drag-and-drop or context menu operations. A "content list" can also be null.
        /// </summary>
        /// <value> A nullable list ob IACObjects.</value>
        public IEnumerable<IACObject> ACContentList
        {
            get { return null; }
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
            return null;
        }

        /// <summary>
        /// This method is called before ACUrlCommand if a method-command was encoded in the ACUrl
        /// </summary>
        /// <param name="acUrl">String that adresses a command</param>
        /// <param name="acParameter">Parameters if a method should be invoked</param>
        /// <returns>true if ACUrlCommand can be invoked</returns>
        public bool IsEnabledACUrlCommand(string acUrl, params Object[] acParameter)
        {
            return false;
        }

        /// <summary>
        /// Returns the parent object
        /// </summary>
        /// <value>Reference to the parent object</value>
        public IACObject ParentACObject
        {
            get { return null; }
        }

        /// <summary>
        /// Returns a ACUrl relatively to the passed object.
        /// If the passed object is null then the absolute path is returned
        /// </summary>
        /// <param name="rootACObject">Object for creating a realtive path to it</param>
        /// <returns>ACUrl as string</returns>
        public string GetACUrl(IACObject rootACObject = null)
        {
            return null;
        }

        /// <summary>Unique Identifier in a Parent-/Child-Relationship.</summary>
        /// <value>The Unique Identifier as string</value>
        public string ACIdentifier
        {
            get
            {
                string inwardLotNo = "-";
                if (InwardLot != null)
                    inwardLotNo = InwardLot.LotNo;
                string tmpACIdentifier = string.Format(@"MP_{0}({1})", InwardMaterialNo, inwardLotNo);
                tmpACIdentifier = ACUrlHelper.GetTrimmedName(tmpACIdentifier);
                return tmpACIdentifier;
            }
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
            return false;
        }
        
        public bool ACUrlTypeInfo(string acUrl, ref ACUrlTypeInfo acUrlTypeInfo)
        {
            return this.ReflectACUrlTypeInfo(acUrl, ref acUrlTypeInfo);
        }
        #endregion

        #region Override & Misc.

        public override string ToString()
        {
            string inwardLotNo = "-";
            if (InwardLot != null)
                inwardLotNo = InwardLot.LotNo;
            return string.Format(@"MixPoint: {0} [{1}] {2}", inwardLotNo, InwardMaterialNo, InwardMaterialName);
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Testkommentar OnPropertyChanged
        /// </summary>
        /// <param name="name">Hello</param>
        [ACMethodInfo("ACComponent", "en{'PropertyChanged'}de{'PropertyChanged'}", 9999)]
        public virtual void OnPropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }
        #endregion

        #region Methods

        public virtual void Finish()
        {
            OutwardLotsList = OutwardLotsList.OrderBy(c => c.InsertDate).ToList();
        }
        #endregion

    }
}
