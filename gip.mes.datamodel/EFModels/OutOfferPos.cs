using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class OutOfferPos : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence, ITargetQuantity
{

    public OutOfferPos()
    {
    }

    private OutOfferPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _OutOfferPosID;
    public Guid OutOfferPosID 
    {
        get { return _OutOfferPosID; }
        set { SetProperty<Guid>(ref _OutOfferPosID, value); }
    }

    Guid _OutOfferID;
    public Guid OutOfferID 
    {
        get { return _OutOfferID; }
        set { SetProperty<Guid>(ref _OutOfferID, value); }
    }

    Guid? _ParentOutOfferPosID;
    public Guid? ParentOutOfferPosID 
    {
        get { return _ParentOutOfferPosID; }
        set { SetProperty<Guid?>(ref _ParentOutOfferPosID, value); }
    }

    Guid? _GroupOutOfferPosID;
    public Guid? GroupOutOfferPosID 
    {
        get { return _GroupOutOfferPosID; }
        set { SetProperty<Guid?>(ref _GroupOutOfferPosID, value); }
    }

    Guid? _MDTimeRangeID;
    public Guid? MDTimeRangeID 
    {
        get { return _MDTimeRangeID; }
        set { SetProperty<Guid?>(ref _MDTimeRangeID, value); }
    }

    short _MaterialPosTypeIndex;
    public short MaterialPosTypeIndex 
    {
        get { return _MaterialPosTypeIndex; }
        set { SetProperty<short>(ref _MaterialPosTypeIndex, value); }
    }

    Guid? _MDCountrySalesTaxID;
    public Guid? MDCountrySalesTaxID 
    {
        get { return _MDCountrySalesTaxID; }
        set { SetProperty<Guid?>(ref _MDCountrySalesTaxID, value); }
    }

    Guid? _MDCountrySalesTaxMDMaterialGroupID;
    public Guid? MDCountrySalesTaxMDMaterialGroupID 
    {
        get { return _MDCountrySalesTaxMDMaterialGroupID; }
        set { SetProperty<Guid?>(ref _MDCountrySalesTaxMDMaterialGroupID, value); }
    }

    Guid? _MDCountrySalesTaxMaterialID;
    public Guid? MDCountrySalesTaxMaterialID 
    {
        get { return _MDCountrySalesTaxMaterialID; }
        set { SetProperty<Guid?>(ref _MDCountrySalesTaxMaterialID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    Guid? _MDUnitID;
    public Guid? MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetProperty<Guid?>(ref _MDUnitID, value); }
    }

    double _TargetQuantityUOM;
    public double TargetQuantityUOM 
    {
        get { return _TargetQuantityUOM; }
        set { SetProperty<double>(ref _TargetQuantityUOM, value); }
    }

    double _TargetQuantity;
    public double TargetQuantity 
    {
        get { return _TargetQuantity; }
        set { SetProperty<double>(ref _TargetQuantity, value); }
    }

    decimal _PriceNet;
    public decimal PriceNet 
    {
        get { return _PriceNet; }
        set { SetProperty<decimal>(ref _PriceNet, value); }
    }

    decimal _PriceGross;
    public decimal PriceGross 
    {
        get { return _PriceGross; }
        set { SetProperty<decimal>(ref _PriceGross, value); }
    }

    decimal _SalesTax;
    public decimal SalesTax 
    {
        get { return _SalesTax; }
        set { SetProperty<decimal>(ref _SalesTax, value); }
    }

    double _TargetWeight;
    public double TargetWeight 
    {
        get { return _TargetWeight; }
        set { SetProperty<double>(ref _TargetWeight, value); }
    }

    DateTime _TargetDeliveryDate;
    public DateTime TargetDeliveryDate 
    {
        get { return _TargetDeliveryDate; }
        set { SetProperty<DateTime>(ref _TargetDeliveryDate, value); }
    }

    DateTime? _TargetDeliveryMaxDate;
    public DateTime? TargetDeliveryMaxDate 
    {
        get { return _TargetDeliveryMaxDate; }
        set { SetProperty<DateTime?>(ref _TargetDeliveryMaxDate, value); }
    }

    short _TargetDeliveryPriority;
    public short TargetDeliveryPriority 
    {
        get { return _TargetDeliveryPriority; }
        set { SetProperty<short>(ref _TargetDeliveryPriority, value); }
    }

    bool _GroupSum;
    public bool GroupSum 
    {
        get { return _GroupSum; }
        set { SetProperty<bool>(ref _GroupSum, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    string _Comment2;
    public string Comment2 
    {
        get { return _Comment2; }
        set { SetProperty<string>(ref _Comment2, value); }
    }

    string _XMLDesign;
    public string XMLDesign 
    {
        get { return _XMLDesign; }
        set { SetProperty<string>(ref _XMLDesign, value); }
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

    private OutOfferPos _OutOfferPos1_GroupOutOfferPos;
    public virtual OutOfferPos OutOfferPos1_GroupOutOfferPos
    { 
        get { return LazyLoader.Load(this, ref _OutOfferPos1_GroupOutOfferPos); } 
        set { SetProperty<OutOfferPos>(ref _OutOfferPos1_GroupOutOfferPos, value); }
    }

    public bool OutOfferPos1_GroupOutOfferPos_IsLoaded
    {
        get
        {
            return _OutOfferPos1_GroupOutOfferPos != null;
        }
    }

    public virtual ReferenceEntry OutOfferPos1_GroupOutOfferPosReference 
    {
        get { return Context.Entry(this).Reference("OutOfferPos1_GroupOutOfferPos"); }
    }
    
    private ICollection<OutOfferPos> _OutOfferPos_GroupOutOfferPos;
    public virtual ICollection<OutOfferPos> OutOfferPos_GroupOutOfferPos
    {
        get { return LazyLoader.Load(this, ref _OutOfferPos_GroupOutOfferPos); }
        set { _OutOfferPos_GroupOutOfferPos = value; }
    }

    public bool OutOfferPos_GroupOutOfferPos_IsLoaded
    {
        get
        {
            return _OutOfferPos_GroupOutOfferPos != null;
        }
    }

    public virtual CollectionEntry OutOfferPos_GroupOutOfferPosReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPos_GroupOutOfferPos); }
    }

    private ICollection<OutOfferPos> _OutOfferPos_ParentOutOfferPos;
    public virtual ICollection<OutOfferPos> OutOfferPos_ParentOutOfferPos
    {
        get { return LazyLoader.Load(this, ref _OutOfferPos_ParentOutOfferPos); }
        set { _OutOfferPos_ParentOutOfferPos = value; }
    }

    public bool OutOfferPos_ParentOutOfferPos_IsLoaded
    {
        get
        {
            return _OutOfferPos_ParentOutOfferPos != null;
        }
    }

    public virtual CollectionEntry OutOfferPos_ParentOutOfferPosReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPos_ParentOutOfferPos); }
    }

    private MDCountrySalesTax _MDCountrySalesTax;
    public virtual MDCountrySalesTax MDCountrySalesTax
    { 
        get { return LazyLoader.Load(this, ref _MDCountrySalesTax); } 
        set { SetProperty<MDCountrySalesTax>(ref _MDCountrySalesTax, value); }
    }

    public bool MDCountrySalesTax_IsLoaded
    {
        get
        {
            return _MDCountrySalesTax != null;
        }
    }

    public virtual ReferenceEntry MDCountrySalesTaxReference 
    {
        get { return Context.Entry(this).Reference("MDCountrySalesTax"); }
    }
    
    private MDCountrySalesTaxMDMaterialGroup _MDCountrySalesTaxMDMaterialGroup;
    public virtual MDCountrySalesTaxMDMaterialGroup MDCountrySalesTaxMDMaterialGroup
    { 
        get { return LazyLoader.Load(this, ref _MDCountrySalesTaxMDMaterialGroup); } 
        set { SetProperty<MDCountrySalesTaxMDMaterialGroup>(ref _MDCountrySalesTaxMDMaterialGroup, value); }
    }

    public bool MDCountrySalesTaxMDMaterialGroup_IsLoaded
    {
        get
        {
            return _MDCountrySalesTaxMDMaterialGroup != null;
        }
    }

    public virtual ReferenceEntry MDCountrySalesTaxMDMaterialGroupReference 
    {
        get { return Context.Entry(this).Reference("MDCountrySalesTaxMDMaterialGroup"); }
    }
    
    private MDCountrySalesTaxMaterial _MDCountrySalesTaxMaterial;
    public virtual MDCountrySalesTaxMaterial MDCountrySalesTaxMaterial
    { 
        get { return LazyLoader.Load(this, ref _MDCountrySalesTaxMaterial); } 
        set { SetProperty<MDCountrySalesTaxMaterial>(ref _MDCountrySalesTaxMaterial, value); }
    }

    public bool MDCountrySalesTaxMaterial_IsLoaded
    {
        get
        {
            return _MDCountrySalesTaxMaterial != null;
        }
    }

    public virtual ReferenceEntry MDCountrySalesTaxMaterialReference 
    {
        get { return Context.Entry(this).Reference("MDCountrySalesTaxMaterial"); }
    }
    
    private MDTimeRange _MDTimeRange;
    public virtual MDTimeRange MDTimeRange
    { 
        get { return LazyLoader.Load(this, ref _MDTimeRange); } 
        set { SetProperty<MDTimeRange>(ref _MDTimeRange, value); }
    }

    public bool MDTimeRange_IsLoaded
    {
        get
        {
            return _MDTimeRange != null;
        }
    }

    public virtual ReferenceEntry MDTimeRangeReference 
    {
        get { return Context.Entry(this).Reference("MDTimeRange"); }
    }
    
    private MDUnit _MDUnit;
    public virtual MDUnit MDUnit
    { 
        get { return LazyLoader.Load(this, ref _MDUnit); } 
        set { SetProperty<MDUnit>(ref _MDUnit, value); }
    }

    public bool MDUnit_IsLoaded
    {
        get
        {
            return _MDUnit != null;
        }
    }

    public virtual ReferenceEntry MDUnitReference 
    {
        get { return Context.Entry(this).Reference("MDUnit"); }
    }
    
    private Material _Material;
    public virtual Material Material
    { 
        get { return LazyLoader.Load(this, ref _Material); } 
        set { SetProperty<Material>(ref _Material, value); }
    }

    public bool Material_IsLoaded
    {
        get
        {
            return _Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private OutOffer _OutOffer;
    public virtual OutOffer OutOffer
    { 
        get { return LazyLoader.Load(this, ref _OutOffer); } 
        set { SetProperty<OutOffer>(ref _OutOffer, value); }
    }

    public bool OutOffer_IsLoaded
    {
        get
        {
            return _OutOffer != null;
        }
    }

    public virtual ReferenceEntry OutOfferReference 
    {
        get { return Context.Entry(this).Reference("OutOffer"); }
    }
    
    private OutOfferPos _OutOfferPos1_ParentOutOfferPos;
    public virtual OutOfferPos OutOfferPos1_ParentOutOfferPos
    { 
        get { return LazyLoader.Load(this, ref _OutOfferPos1_ParentOutOfferPos); } 
        set { SetProperty<OutOfferPos>(ref _OutOfferPos1_ParentOutOfferPos, value); }
    }

    public bool OutOfferPos1_ParentOutOfferPos_IsLoaded
    {
        get
        {
            return _OutOfferPos1_ParentOutOfferPos != null;
        }
    }

    public virtual ReferenceEntry OutOfferPos1_ParentOutOfferPosReference 
    {
        get { return Context.Entry(this).Reference("OutOfferPos1_ParentOutOfferPos"); }
    }
    }
