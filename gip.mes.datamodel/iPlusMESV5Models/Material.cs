using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class Material : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public Material()
    {
    }

    private Material(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    string _MaterialNo;
    public string MaterialNo 
    {
        get { return _MaterialNo; }
        set { SetProperty<string>(ref _MaterialNo, value); }
    }

    string _MaterialName1;
    public string MaterialName1 
    {
        get { return _MaterialName1; }
        set { SetProperty<string>(ref _MaterialName1, value); }
    }

    string _MaterialName2;
    public string MaterialName2 
    {
        get { return _MaterialName2; }
        set { SetProperty<string>(ref _MaterialName2, value); }
    }

    string _MaterialName3;
    public string MaterialName3 
    {
        get { return _MaterialName3; }
        set { SetProperty<string>(ref _MaterialName3, value); }
    }

    Guid _MDMaterialGroupID;
    public Guid MDMaterialGroupID 
    {
        get { return _MDMaterialGroupID; }
        set { SetProperty<Guid>(ref _MDMaterialGroupID, value); }
    }

    Guid? _MDMaterialTypeID;
    public Guid? MDMaterialTypeID 
    {
        get { return _MDMaterialTypeID; }
        set { SetProperty<Guid?>(ref _MDMaterialTypeID, value); }
    }

    Guid _MDFacilityManagementTypeID;
    public Guid MDFacilityManagementTypeID 
    {
        get { return _MDFacilityManagementTypeID; }
        set { SetProperty<Guid>(ref _MDFacilityManagementTypeID, value); }
    }

    double? _MinStockQuantity;
    public double? MinStockQuantity 
    {
        get { return _MinStockQuantity; }
        set { SetProperty<double?>(ref _MinStockQuantity, value); }
    }

    double? _OptStockQuantity;
    public double? OptStockQuantity 
    {
        get { return _OptStockQuantity; }
        set { SetProperty<double?>(ref _OptStockQuantity, value); }
    }

    bool _DontAllowNegativeStock;
    public bool DontAllowNegativeStock 
    {
        get { return _DontAllowNegativeStock; }
        set { SetProperty<bool>(ref _DontAllowNegativeStock, value); }
    }

    Guid? _InFacilityID;
    public Guid? InFacilityID 
    {
        get { return _InFacilityID; }
        set { SetProperty<Guid?>(ref _InFacilityID, value); }
    }

    Guid? _OutFacilityID;
    public Guid? OutFacilityID 
    {
        get { return _OutFacilityID; }
        set { SetProperty<Guid?>(ref _OutFacilityID, value); }
    }

    Guid? _VBiStackCalculatorACClassID;
    public Guid? VBiStackCalculatorACClassID 
    {
        get { return _VBiStackCalculatorACClassID; }
        set { SetProperty<Guid?>(ref _VBiStackCalculatorACClassID, value); }
    }

    Guid _MDInventoryManagementTypeID;
    public Guid MDInventoryManagementTypeID 
    {
        get { return _MDInventoryManagementTypeID; }
        set { SetProperty<Guid>(ref _MDInventoryManagementTypeID, value); }
    }

    Guid? _MDGMPMaterialGroupID;
    public Guid? MDGMPMaterialGroupID 
    {
        get { return _MDGMPMaterialGroupID; }
        set { SetProperty<Guid?>(ref _MDGMPMaterialGroupID, value); }
    }

    int _StorageLife;
    public int StorageLife 
    {
        get { return _StorageLife; }
        set { SetProperty<int>(ref _StorageLife, value); }
    }

    Guid? _ProductionMaterialID;
    public Guid? ProductionMaterialID 
    {
        get { return _ProductionMaterialID; }
        set { SetProperty<Guid?>(ref _ProductionMaterialID, value); }
    }

    Guid? _VBiProgramACClassMethodID;
    public Guid? VBiProgramACClassMethodID 
    {
        get { return _VBiProgramACClassMethodID; }
        set { SetProperty<Guid?>(ref _VBiProgramACClassMethodID, value); }
    }

    bool _UsageInOrder;
    public bool UsageInOrder 
    {
        get { return _UsageInOrder; }
        set { SetProperty<bool>(ref _UsageInOrder, value); }
    }

    bool _UsageOutOrder;
    public bool UsageOutOrder 
    {
        get { return _UsageOutOrder; }
        set { SetProperty<bool>(ref _UsageOutOrder, value); }
    }

    bool _UsageACProgram;
    public bool UsageACProgram 
    {
        get { return _UsageACProgram; }
        set { SetProperty<bool>(ref _UsageACProgram, value); }
    }

    bool? _UsageOwnProduct;
    public bool? UsageOwnProduct 
    {
        get { return _UsageOwnProduct; }
        set { SetProperty<bool?>(ref _UsageOwnProduct, value); }
    }

    bool _IsActive;
    public bool IsActive 
    {
        get { return _IsActive; }
        set { SetProperty<bool>(ref _IsActive, value); }
    }

    bool _ContractorStock;
    public bool ContractorStock 
    {
        get { return _ContractorStock; }
        set { SetProperty<bool>(ref _ContractorStock, value); }
    }

    Guid _BaseMDUnitID;
    public Guid BaseMDUnitID 
    {
        get { return _BaseMDUnitID; }
        set { SetProperty<Guid>(ref _BaseMDUnitID, value); }
    }

    double _NetWeight;
    public double NetWeight 
    {
        get { return _NetWeight; }
        set { SetProperty<double>(ref _NetWeight, value); }
    }

    double _GrossWeight;
    public double GrossWeight 
    {
        get { return _GrossWeight; }
        set { SetProperty<double>(ref _GrossWeight, value); }
    }

    double _ProductionWeight;
    public double ProductionWeight 
    {
        get { return _ProductionWeight; }
        set { SetProperty<double>(ref _ProductionWeight, value); }
    }

    double _Density;
    public double Density 
    {
        get { return _Density; }
        set { SetProperty<double>(ref _Density, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    string _UpdateName;
    public string UpdateName 
    {
        get { return _UpdateName; }
        set { SetProperty<string>(ref _UpdateName, value); }
    }

    DateTime _UpdateDate;
    public DateTime UpdateDate 
    {
        get { return _UpdateDate; }
        set { SetProperty<DateTime>(ref _UpdateDate, value); }
    }

    short _PetroleumGroupIndex;
    public short PetroleumGroupIndex 
    {
        get { return _PetroleumGroupIndex; }
        set { SetProperty<short>(ref _PetroleumGroupIndex, value); }
    }

    double _DensityAmb;
    public double DensityAmb 
    {
        get { return _DensityAmb; }
        set { SetProperty<double>(ref _DensityAmb, value); }
    }

    Guid? _LabelID;
    public Guid? LabelID 
    {
        get { return _LabelID; }
        set { SetProperty<Guid?>(ref _LabelID, value); }
    }

    bool _IsIntermediate;
    public bool IsIntermediate 
    {
        get { return _IsIntermediate; }
        set { SetProperty<bool>(ref _IsIntermediate, value); }
    }

    double _ZeroBookingTolerance;
    public double ZeroBookingTolerance 
    {
        get { return _ZeroBookingTolerance; }
        set { SetProperty<double>(ref _ZeroBookingTolerance, value); }
    }

    bool? _RetrogradeFIFO;
    public bool? RetrogradeFIFO 
    {
        get { return _RetrogradeFIFO; }
        set { SetProperty<bool?>(ref _RetrogradeFIFO, value); }
    }

    bool? _ExplosionOff;
    public bool? ExplosionOff 
    {
        get { return _ExplosionOff; }
        set { SetProperty<bool?>(ref _ExplosionOff, value); }
    }

    double _SpecHeatCapacity;
    public double SpecHeatCapacity 
    {
        get { return _SpecHeatCapacity; }
        set { SetProperty<double>(ref _SpecHeatCapacity, value); }
    }

    string _KeyOfExtSys;
    public string KeyOfExtSys 
    {
        get { return _KeyOfExtSys; }
        set { SetProperty<string>(ref _KeyOfExtSys, value); }
    }

    bool? _Anterograde;
    public bool? Anterograde 
    {
        get { return _Anterograde; }
        set { SetProperty<bool?>(ref _Anterograde, value); }
    }

    bool _ExcludeFromSumCalc;
    public bool ExcludeFromSumCalc 
    {
        get { return _ExcludeFromSumCalc; }
        set { SetProperty<bool>(ref _ExcludeFromSumCalc, value); }
    }

    private MDUnit _BaseMDUnit;
    public virtual MDUnit BaseMDUnit
    { 
        get => LazyLoader.Load(this, ref _BaseMDUnit);
        set => _BaseMDUnit = value;
    }

    public bool BaseMDUnit_IsLoaded
    {
        get
        {
            return BaseMDUnit != null;
        }
    }

    public virtual ReferenceEntry BaseMDUnitReference 
    {
        get { return Context.Entry(this).Reference("BaseMDUnit"); }
    }
    
    private ICollection<CompanyMaterial> _CompanyMaterial_Material;
    public virtual ICollection<CompanyMaterial> CompanyMaterial_Material
    {
        get => LazyLoader.Load(this, ref _CompanyMaterial_Material);
        set => _CompanyMaterial_Material = value;
    }

    public bool CompanyMaterial_Material_IsLoaded
    {
        get
        {
            return CompanyMaterial_Material != null;
        }
    }

    public virtual CollectionEntry CompanyMaterial_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterial_Material); }
    }

    private ICollection<DemandOrderPos> _DemandOrderPos_Material;
    public virtual ICollection<DemandOrderPos> DemandOrderPos_Material
    {
        get => LazyLoader.Load(this, ref _DemandOrderPos_Material);
        set => _DemandOrderPos_Material = value;
    }

    public bool DemandOrderPos_Material_IsLoaded
    {
        get
        {
            return DemandOrderPos_Material != null;
        }
    }

    public virtual CollectionEntry DemandOrderPos_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.DemandOrderPos_Material); }
    }

    private ICollection<DemandPrimary> _DemandPrimary_Material;
    public virtual ICollection<DemandPrimary> DemandPrimary_Material
    {
        get => LazyLoader.Load(this, ref _DemandPrimary_Material);
        set => _DemandPrimary_Material = value;
    }

    public bool DemandPrimary_Material_IsLoaded
    {
        get
        {
            return DemandPrimary_Material != null;
        }
    }

    public virtual CollectionEntry DemandPrimary_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.DemandPrimary_Material); }
    }

    private ICollection<Facility> _Facility_Material;
    public virtual ICollection<Facility> Facility_Material
    {
        get => LazyLoader.Load(this, ref _Facility_Material);
        set => _Facility_Material = value;
    }

    public bool Facility_Material_IsLoaded
    {
        get
        {
            return Facility_Material != null;
        }
    }

    public virtual CollectionEntry Facility_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_Material); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_InwardMaterial;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_InwardMaterial
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_InwardMaterial);
        set => _FacilityBookingCharge_InwardMaterial = value;
    }

    public bool FacilityBookingCharge_InwardMaterial_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_InwardMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_InwardMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_InwardMaterial); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_OutwardMaterial;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_OutwardMaterial
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_OutwardMaterial);
        set => _FacilityBookingCharge_OutwardMaterial = value;
    }

    public bool FacilityBookingCharge_OutwardMaterial_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_OutwardMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_OutwardMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_OutwardMaterial); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_InwardMaterial;
    public virtual ICollection<FacilityBooking> FacilityBooking_InwardMaterial
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_InwardMaterial);
        set => _FacilityBooking_InwardMaterial = value;
    }

    public bool FacilityBooking_InwardMaterial_IsLoaded
    {
        get
        {
            return FacilityBooking_InwardMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_InwardMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_InwardMaterial); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_OutwardMaterial;
    public virtual ICollection<FacilityBooking> FacilityBooking_OutwardMaterial
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_OutwardMaterial);
        set => _FacilityBooking_OutwardMaterial = value;
    }

    public bool FacilityBooking_OutwardMaterial_IsLoaded
    {
        get
        {
            return FacilityBooking_OutwardMaterial != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_OutwardMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_OutwardMaterial); }
    }

    private ICollection<FacilityCharge> _FacilityCharge_Material;
    public virtual ICollection<FacilityCharge> FacilityCharge_Material
    {
        get => LazyLoader.Load(this, ref _FacilityCharge_Material);
        set => _FacilityCharge_Material = value;
    }

    public bool FacilityCharge_Material_IsLoaded
    {
        get
        {
            return FacilityCharge_Material != null;
        }
    }

    public virtual CollectionEntry FacilityCharge_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityCharge_Material); }
    }

    private ICollection<FacilityLot> _FacilityLot_Material;
    public virtual ICollection<FacilityLot> FacilityLot_Material
    {
        get => LazyLoader.Load(this, ref _FacilityLot_Material);
        set => _FacilityLot_Material = value;
    }

    public bool FacilityLot_Material_IsLoaded
    {
        get
        {
            return FacilityLot_Material != null;
        }
    }

    public virtual CollectionEntry FacilityLot_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityLot_Material); }
    }

    private ICollection<FacilityMaterial> _FacilityMaterial_Material;
    public virtual ICollection<FacilityMaterial> FacilityMaterial_Material
    {
        get => LazyLoader.Load(this, ref _FacilityMaterial_Material);
        set => _FacilityMaterial_Material = value;
    }

    public bool FacilityMaterial_Material_IsLoaded
    {
        get
        {
            return FacilityMaterial_Material != null;
        }
    }

    public virtual CollectionEntry FacilityMaterial_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityMaterial_Material); }
    }

    private ICollection<FacilityReservation> _FacilityReservation_Material;
    public virtual ICollection<FacilityReservation> FacilityReservation_Material
    {
        get => LazyLoader.Load(this, ref _FacilityReservation_Material);
        set => _FacilityReservation_Material = value;
    }

    public bool FacilityReservation_Material_IsLoaded
    {
        get
        {
            return FacilityReservation_Material != null;
        }
    }

    public virtual CollectionEntry FacilityReservation_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityReservation_Material); }
    }

    private Facility _InFacility;
    public virtual Facility InFacility
    { 
        get => LazyLoader.Load(this, ref _InFacility);
        set => _InFacility = value;
    }

    public bool InFacility_IsLoaded
    {
        get
        {
            return InFacility != null;
        }
    }

    public virtual ReferenceEntry InFacilityReference 
    {
        get { return Context.Entry(this).Reference("InFacility"); }
    }
    
    private ICollection<InOrderConfig> _InOrderConfig_Material;
    public virtual ICollection<InOrderConfig> InOrderConfig_Material
    {
        get => LazyLoader.Load(this, ref _InOrderConfig_Material);
        set => _InOrderConfig_Material = value;
    }

    public bool InOrderConfig_Material_IsLoaded
    {
        get
        {
            return InOrderConfig_Material != null;
        }
    }

    public virtual CollectionEntry InOrderConfig_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderConfig_Material); }
    }

    private ICollection<InOrderPos> _InOrderPos_Material;
    public virtual ICollection<InOrderPos> InOrderPos_Material
    {
        get => LazyLoader.Load(this, ref _InOrderPos_Material);
        set => _InOrderPos_Material = value;
    }

    public bool InOrderPos_Material_IsLoaded
    {
        get
        {
            return InOrderPos_Material != null;
        }
    }

    public virtual CollectionEntry InOrderPos_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPos_Material); }
    }

    private ICollection<InRequestConfig> _InRequestConfig_Material;
    public virtual ICollection<InRequestConfig> InRequestConfig_Material
    {
        get => LazyLoader.Load(this, ref _InRequestConfig_Material);
        set => _InRequestConfig_Material = value;
    }

    public bool InRequestConfig_Material_IsLoaded
    {
        get
        {
            return InRequestConfig_Material != null;
        }
    }

    public virtual CollectionEntry InRequestConfig_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestConfig_Material); }
    }

    private ICollection<InRequestPos> _InRequestPos_Material;
    public virtual ICollection<InRequestPos> InRequestPos_Material
    {
        get => LazyLoader.Load(this, ref _InRequestPos_Material);
        set => _InRequestPos_Material = value;
    }

    public bool InRequestPos_Material_IsLoaded
    {
        get
        {
            return InRequestPos_Material != null;
        }
    }

    public virtual CollectionEntry InRequestPos_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestPos_Material); }
    }

    private ICollection<Material> _Material_ProductionMaterial;
    public virtual ICollection<Material> Material_ProductionMaterial
    {
        get => LazyLoader.Load(this, ref _Material_ProductionMaterial);
        set => _Material_ProductionMaterial = value;
    }

    public bool Material_ProductionMaterial_IsLoaded
    {
        get
        {
            return Material_ProductionMaterial != null;
        }
    }

    public virtual CollectionEntry Material_ProductionMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.Material_ProductionMaterial); }
    }

    private ICollection<InvoicePos> _InvoicePos_Material;
    public virtual ICollection<InvoicePos> InvoicePos_Material
    {
        get => LazyLoader.Load(this, ref _InvoicePos_Material);
        set => _InvoicePos_Material = value;
    }

    public bool InvoicePos_Material_IsLoaded
    {
        get
        {
            return InvoicePos_Material != null;
        }
    }

    public virtual CollectionEntry InvoicePos_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.InvoicePos_Material); }
    }

    private ICollection<LabOrder> _LabOrder_Material;
    public virtual ICollection<LabOrder> LabOrder_Material
    {
        get => LazyLoader.Load(this, ref _LabOrder_Material);
        set => _LabOrder_Material = value;
    }

    public bool LabOrder_Material_IsLoaded
    {
        get
        {
            return LabOrder_Material != null;
        }
    }

    public virtual CollectionEntry LabOrder_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.LabOrder_Material); }
    }

    private Label _Label;
    public virtual Label Label
    { 
        get => LazyLoader.Load(this, ref _Label);
        set => _Label = value;
    }

    public bool Label_IsLoaded
    {
        get
        {
            return Label != null;
        }
    }

    public virtual ReferenceEntry LabelReference 
    {
        get { return Context.Entry(this).Reference("Label"); }
    }
    
    private ICollection<MDCountrySalesTaxMaterial> _MDCountrySalesTaxMaterial_Material;
    public virtual ICollection<MDCountrySalesTaxMaterial> MDCountrySalesTaxMaterial_Material
    {
        get => LazyLoader.Load(this, ref _MDCountrySalesTaxMaterial_Material);
        set => _MDCountrySalesTaxMaterial_Material = value;
    }

    public bool MDCountrySalesTaxMaterial_Material_IsLoaded
    {
        get
        {
            return MDCountrySalesTaxMaterial_Material != null;
        }
    }

    public virtual CollectionEntry MDCountrySalesTaxMaterial_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.MDCountrySalesTaxMaterial_Material); }
    }

    private MDFacilityManagementType _MDFacilityManagementType;
    public virtual MDFacilityManagementType MDFacilityManagementType
    { 
        get => LazyLoader.Load(this, ref _MDFacilityManagementType);
        set => _MDFacilityManagementType = value;
    }

    public bool MDFacilityManagementType_IsLoaded
    {
        get
        {
            return MDFacilityManagementType != null;
        }
    }

    public virtual ReferenceEntry MDFacilityManagementTypeReference 
    {
        get { return Context.Entry(this).Reference("MDFacilityManagementType"); }
    }
    
    private MDGMPMaterialGroup _MDGMPMaterialGroup;
    public virtual MDGMPMaterialGroup MDGMPMaterialGroup
    { 
        get => LazyLoader.Load(this, ref _MDGMPMaterialGroup);
        set => _MDGMPMaterialGroup = value;
    }

    public bool MDGMPMaterialGroup_IsLoaded
    {
        get
        {
            return MDGMPMaterialGroup != null;
        }
    }

    public virtual ReferenceEntry MDGMPMaterialGroupReference 
    {
        get { return Context.Entry(this).Reference("MDGMPMaterialGroup"); }
    }
    
    private MDInventoryManagementType _MDInventoryManagementType;
    public virtual MDInventoryManagementType MDInventoryManagementType
    { 
        get => LazyLoader.Load(this, ref _MDInventoryManagementType);
        set => _MDInventoryManagementType = value;
    }

    public bool MDInventoryManagementType_IsLoaded
    {
        get
        {
            return MDInventoryManagementType != null;
        }
    }

    public virtual ReferenceEntry MDInventoryManagementTypeReference 
    {
        get { return Context.Entry(this).Reference("MDInventoryManagementType"); }
    }
    
    private MDMaterialGroup _MDMaterialGroup;
    public virtual MDMaterialGroup MDMaterialGroup
    { 
        get => LazyLoader.Load(this, ref _MDMaterialGroup);
        set => _MDMaterialGroup = value;
    }

    public bool MDMaterialGroup_IsLoaded
    {
        get
        {
            return MDMaterialGroup != null;
        }
    }

    public virtual ReferenceEntry MDMaterialGroupReference 
    {
        get { return Context.Entry(this).Reference("MDMaterialGroup"); }
    }
    
    private MDMaterialType _MDMaterialType;
    public virtual MDMaterialType MDMaterialType
    { 
        get => LazyLoader.Load(this, ref _MDMaterialType);
        set => _MDMaterialType = value;
    }

    public bool MDMaterialType_IsLoaded
    {
        get
        {
            return MDMaterialType != null;
        }
    }

    public virtual ReferenceEntry MDMaterialTypeReference 
    {
        get { return Context.Entry(this).Reference("MDMaterialType"); }
    }
    
    private ICollection<MaterialCalculation> _MaterialCalculation_Material;
    public virtual ICollection<MaterialCalculation> MaterialCalculation_Material
    {
        get => LazyLoader.Load(this, ref _MaterialCalculation_Material);
        set => _MaterialCalculation_Material = value;
    }

    public bool MaterialCalculation_Material_IsLoaded
    {
        get
        {
            return MaterialCalculation_Material != null;
        }
    }

    public virtual CollectionEntry MaterialCalculation_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialCalculation_Material); }
    }

    private ICollection<MaterialConfig> _MaterialConfig_Material;
    public virtual ICollection<MaterialConfig> MaterialConfig_Material
    {
        get => LazyLoader.Load(this, ref _MaterialConfig_Material);
        set => _MaterialConfig_Material = value;
    }

    public bool MaterialConfig_Material_IsLoaded
    {
        get
        {
            return MaterialConfig_Material != null;
        }
    }

    public virtual CollectionEntry MaterialConfig_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialConfig_Material); }
    }

    private ICollection<MaterialGMPAdditive> _MaterialGMPAdditive_Material;
    public virtual ICollection<MaterialGMPAdditive> MaterialGMPAdditive_Material
    {
        get => LazyLoader.Load(this, ref _MaterialGMPAdditive_Material);
        set => _MaterialGMPAdditive_Material = value;
    }

    public bool MaterialGMPAdditive_Material_IsLoaded
    {
        get
        {
            return MaterialGMPAdditive_Material != null;
        }
    }

    public virtual CollectionEntry MaterialGMPAdditive_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialGMPAdditive_Material); }
    }

    private ICollection<MaterialHistory> _MaterialHistory_Material;
    public virtual ICollection<MaterialHistory> MaterialHistory_Material
    {
        get => LazyLoader.Load(this, ref _MaterialHistory_Material);
        set => _MaterialHistory_Material = value;
    }

    public bool MaterialHistory_Material_IsLoaded
    {
        get
        {
            return MaterialHistory_Material != null;
        }
    }

    public virtual CollectionEntry MaterialHistory_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialHistory_Material); }
    }

    private ICollection<MaterialStock> _MaterialStock_Material;
    public virtual ICollection<MaterialStock> MaterialStock_Material
    {
        get => LazyLoader.Load(this, ref _MaterialStock_Material);
        set => _MaterialStock_Material = value;
    }

    public bool MaterialStock_Material_IsLoaded
    {
        get
        {
            return MaterialStock_Material != null;
        }
    }

    public virtual CollectionEntry MaterialStock_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialStock_Material); }
    }

    private ICollection<MaterialUnit> _MaterialUnit_Material;
    public virtual ICollection<MaterialUnit> MaterialUnit_Material
    {
        get => LazyLoader.Load(this, ref _MaterialUnit_Material);
        set => _MaterialUnit_Material = value;
    }

    public bool MaterialUnit_Material_IsLoaded
    {
        get
        {
            return MaterialUnit_Material != null;
        }
    }

    public virtual CollectionEntry MaterialUnit_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialUnit_Material); }
    }

    private ICollection<MaterialWFConnection> _MaterialWFConnection_Material;
    public virtual ICollection<MaterialWFConnection> MaterialWFConnection_Material
    {
        get => LazyLoader.Load(this, ref _MaterialWFConnection_Material);
        set => _MaterialWFConnection_Material = value;
    }

    public bool MaterialWFConnection_Material_IsLoaded
    {
        get
        {
            return MaterialWFConnection_Material != null;
        }
    }

    public virtual CollectionEntry MaterialWFConnection_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialWFConnection_Material); }
    }

    private ICollection<MaterialWFRelation> _MaterialWFRelation_SourceMaterial;
    public virtual ICollection<MaterialWFRelation> MaterialWFRelation_SourceMaterial
    {
        get => LazyLoader.Load(this, ref _MaterialWFRelation_SourceMaterial);
        set => _MaterialWFRelation_SourceMaterial = value;
    }

    public bool MaterialWFRelation_SourceMaterial_IsLoaded
    {
        get
        {
            return MaterialWFRelation_SourceMaterial != null;
        }
    }

    public virtual CollectionEntry MaterialWFRelation_SourceMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialWFRelation_SourceMaterial); }
    }

    private ICollection<MaterialWFRelation> _MaterialWFRelation_TargetMaterial;
    public virtual ICollection<MaterialWFRelation> MaterialWFRelation_TargetMaterial
    {
        get => LazyLoader.Load(this, ref _MaterialWFRelation_TargetMaterial);
        set => _MaterialWFRelation_TargetMaterial = value;
    }

    public bool MaterialWFRelation_TargetMaterial_IsLoaded
    {
        get
        {
            return MaterialWFRelation_TargetMaterial != null;
        }
    }

    public virtual CollectionEntry MaterialWFRelation_TargetMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialWFRelation_TargetMaterial); }
    }

    private Facility _OutFacility;
    public virtual Facility OutFacility
    { 
        get => LazyLoader.Load(this, ref _OutFacility);
        set => _OutFacility = value;
    }

    public bool OutFacility_IsLoaded
    {
        get
        {
            return OutFacility != null;
        }
    }

    public virtual ReferenceEntry OutFacilityReference 
    {
        get { return Context.Entry(this).Reference("OutFacility"); }
    }
    
    private ICollection<OutOfferConfig> _OutOfferConfig_Material;
    public virtual ICollection<OutOfferConfig> OutOfferConfig_Material
    {
        get => LazyLoader.Load(this, ref _OutOfferConfig_Material);
        set => _OutOfferConfig_Material = value;
    }

    public bool OutOfferConfig_Material_IsLoaded
    {
        get
        {
            return OutOfferConfig_Material != null;
        }
    }

    public virtual CollectionEntry OutOfferConfig_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferConfig_Material); }
    }

    private ICollection<OutOfferPos> _OutOfferPos_Material;
    public virtual ICollection<OutOfferPos> OutOfferPos_Material
    {
        get => LazyLoader.Load(this, ref _OutOfferPos_Material);
        set => _OutOfferPos_Material = value;
    }

    public bool OutOfferPos_Material_IsLoaded
    {
        get
        {
            return OutOfferPos_Material != null;
        }
    }

    public virtual CollectionEntry OutOfferPos_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPos_Material); }
    }

    private ICollection<OutOrderConfig> _OutOrderConfig_Material;
    public virtual ICollection<OutOrderConfig> OutOrderConfig_Material
    {
        get => LazyLoader.Load(this, ref _OutOrderConfig_Material);
        set => _OutOrderConfig_Material = value;
    }

    public bool OutOrderConfig_Material_IsLoaded
    {
        get
        {
            return OutOrderConfig_Material != null;
        }
    }

    public virtual CollectionEntry OutOrderConfig_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderConfig_Material); }
    }

    private ICollection<OutOrderPos> _OutOrderPos_Material;
    public virtual ICollection<OutOrderPos> OutOrderPos_Material
    {
        get => LazyLoader.Load(this, ref _OutOrderPos_Material);
        set => _OutOrderPos_Material = value;
    }

    public bool OutOrderPos_Material_IsLoaded
    {
        get
        {
            return OutOrderPos_Material != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_Material); }
    }

    private ICollection<OutOrderPosUtilization> _OutOrderPosUtilization_Material;
    public virtual ICollection<OutOrderPosUtilization> OutOrderPosUtilization_Material
    {
        get => LazyLoader.Load(this, ref _OutOrderPosUtilization_Material);
        set => _OutOrderPosUtilization_Material = value;
    }

    public bool OutOrderPosUtilization_Material_IsLoaded
    {
        get
        {
            return OutOrderPosUtilization_Material != null;
        }
    }

    public virtual CollectionEntry OutOrderPosUtilization_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPosUtilization_Material); }
    }

    private ICollection<PartslistConfig> _PartslistConfig_Material;
    public virtual ICollection<PartslistConfig> PartslistConfig_Material
    {
        get => LazyLoader.Load(this, ref _PartslistConfig_Material);
        set => _PartslistConfig_Material = value;
    }

    public bool PartslistConfig_Material_IsLoaded
    {
        get
        {
            return PartslistConfig_Material != null;
        }
    }

    public virtual CollectionEntry PartslistConfig_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistConfig_Material); }
    }

    private ICollection<PartslistPos> _PartslistPos_Material;
    public virtual ICollection<PartslistPos> PartslistPos_Material
    {
        get => LazyLoader.Load(this, ref _PartslistPos_Material);
        set => _PartslistPos_Material = value;
    }

    public bool PartslistPos_Material_IsLoaded
    {
        get
        {
            return PartslistPos_Material != null;
        }
    }

    public virtual CollectionEntry PartslistPos_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistPos_Material); }
    }

    private ICollection<Partslist> _Partslist_Material;
    public virtual ICollection<Partslist> Partslist_Material
    {
        get => LazyLoader.Load(this, ref _Partslist_Material);
        set => _Partslist_Material = value;
    }

    public bool Partslist_Material_IsLoaded
    {
        get
        {
            return Partslist_Material != null;
        }
    }

    public virtual CollectionEntry Partslist_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.Partslist_Material); }
    }

    private ICollection<PickingConfig> _PickingConfig_Material;
    public virtual ICollection<PickingConfig> PickingConfig_Material
    {
        get => LazyLoader.Load(this, ref _PickingConfig_Material);
        set => _PickingConfig_Material = value;
    }

    public bool PickingConfig_Material_IsLoaded
    {
        get
        {
            return PickingConfig_Material != null;
        }
    }

    public virtual CollectionEntry PickingConfig_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingConfig_Material); }
    }

    private ICollection<PickingPos> _PickingPos_PickingMaterial;
    public virtual ICollection<PickingPos> PickingPos_PickingMaterial
    {
        get => LazyLoader.Load(this, ref _PickingPos_PickingMaterial);
        set => _PickingPos_PickingMaterial = value;
    }

    public bool PickingPos_PickingMaterial_IsLoaded
    {
        get
        {
            return PickingPos_PickingMaterial != null;
        }
    }

    public virtual CollectionEntry PickingPos_PickingMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.PickingPos_PickingMaterial); }
    }

    private ICollection<PriceListMaterial> _PriceListMaterial_Material;
    public virtual ICollection<PriceListMaterial> PriceListMaterial_Material
    {
        get => LazyLoader.Load(this, ref _PriceListMaterial_Material);
        set => _PriceListMaterial_Material = value;
    }

    public bool PriceListMaterial_Material_IsLoaded
    {
        get
        {
            return PriceListMaterial_Material != null;
        }
    }

    public virtual CollectionEntry PriceListMaterial_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.PriceListMaterial_Material); }
    }

    private ICollection<ProdOrderPartslistConfig> _ProdOrderPartslistConfig_Material;
    public virtual ICollection<ProdOrderPartslistConfig> ProdOrderPartslistConfig_Material
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistConfig_Material);
        set => _ProdOrderPartslistConfig_Material = value;
    }

    public bool ProdOrderPartslistConfig_Material_IsLoaded
    {
        get
        {
            return ProdOrderPartslistConfig_Material != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistConfig_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistConfig_Material); }
    }

    private ICollection<ProdOrderPartslistPos> _ProdOrderPartslistPos_Material;
    public virtual ICollection<ProdOrderPartslistPos> ProdOrderPartslistPos_Material
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos_Material);
        set => _ProdOrderPartslistPos_Material = value;
    }

    public bool ProdOrderPartslistPos_Material_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos_Material != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPos_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPos_Material); }
    }

    private Material _Material1_ProductionMaterial;
    public virtual Material Material1_ProductionMaterial
    { 
        get => LazyLoader.Load(this, ref _Material1_ProductionMaterial);
        set => _Material1_ProductionMaterial = value;
    }

    public bool Material1_ProductionMaterial_IsLoaded
    {
        get
        {
            return Material1_ProductionMaterial != null;
        }
    }

    public virtual ReferenceEntry Material1_ProductionMaterialReference 
    {
        get { return Context.Entry(this).Reference("Material1_ProductionMaterial"); }
    }
    

    private ICollection<TandTv3FilterTrackingMaterial> _TandTv3FilterTrackingMaterial_Material;
    public virtual ICollection<TandTv3FilterTrackingMaterial> TandTv3FilterTrackingMaterial_Material
    {
        get => LazyLoader.Load(this, ref _TandTv3FilterTrackingMaterial_Material);
        set => _TandTv3FilterTrackingMaterial_Material = value;
    }

    public bool TandTv3FilterTrackingMaterial_Material_IsLoaded
    {
        get
        {
            return TandTv3FilterTrackingMaterial_Material != null;
        }
    }

    public virtual CollectionEntry TandTv3FilterTrackingMaterial_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3FilterTrackingMaterial_Material); }
    }

    private ICollection<TandTv3MixPoint> _TandTv3MixPoint_InwardMaterial;
    public virtual ICollection<TandTv3MixPoint> TandTv3MixPoint_InwardMaterial
    {
        get => LazyLoader.Load(this, ref _TandTv3MixPoint_InwardMaterial);
        set => _TandTv3MixPoint_InwardMaterial = value;
    }

    public bool TandTv3MixPoint_InwardMaterial_IsLoaded
    {
        get
        {
            return TandTv3MixPoint_InwardMaterial != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPoint_InwardMaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPoint_InwardMaterial); }
    }

    private ICollection<TourplanConfig> _TourplanConfig_Material;
    public virtual ICollection<TourplanConfig> TourplanConfig_Material
    {
        get => LazyLoader.Load(this, ref _TourplanConfig_Material);
        set => _TourplanConfig_Material = value;
    }

    public bool TourplanConfig_Material_IsLoaded
    {
        get
        {
            return TourplanConfig_Material != null;
        }
    }

    public virtual CollectionEntry TourplanConfig_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanConfig_Material); }
    }

    private ACClassMethod _VBiProgramACClassMethod;
    public virtual ACClassMethod VBiProgramACClassMethod
    { 
        get => LazyLoader.Load(this, ref _VBiProgramACClassMethod);
        set => _VBiProgramACClassMethod = value;
    }

    public bool VBiProgramACClassMethod_IsLoaded
    {
        get
        {
            return VBiProgramACClassMethod != null;
        }
    }

    public virtual ReferenceEntry VBiProgramACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("VBiProgramACClassMethod"); }
    }
    
    private ACClass _VBiStackCalculatorACClass;
    public virtual ACClass VBiStackCalculatorACClass
    { 
        get => LazyLoader.Load(this, ref _VBiStackCalculatorACClass);
        set => _VBiStackCalculatorACClass = value;
    }

    public bool VBiStackCalculatorACClass_IsLoaded
    {
        get
        {
            return VBiStackCalculatorACClass != null;
        }
    }

    public virtual ReferenceEntry VBiStackCalculatorACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiStackCalculatorACClass"); }
    }
    
    private ICollection<Visitor> _Visitor_Material;
    public virtual ICollection<Visitor> Visitor_Material
    {
        get => LazyLoader.Load(this, ref _Visitor_Material);
        set => _Visitor_Material = value;
    }

    public bool Visitor_Material_IsLoaded
    {
        get
        {
            return Visitor_Material != null;
        }
    }

    public virtual CollectionEntry Visitor_MaterialReference
    {
        get { return Context.Entry(this).Collection(c => c.Visitor_Material); }
    }
}
