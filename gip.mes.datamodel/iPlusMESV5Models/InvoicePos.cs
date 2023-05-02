using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class InvoicePos : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence, ITargetQuantity
{

    public InvoicePos()
    {
    }

    private InvoicePos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _InvoicePosID;
    public Guid InvoicePosID 
    {
        get { return _InvoicePosID; }
        set { SetProperty<Guid>(ref _InvoicePosID, value); }
    }

    Guid _InvoiceID;
    public Guid InvoiceID 
    {
        get { return _InvoiceID; }
        set { SetProperty<Guid>(ref _InvoiceID, value); }
    }

    Guid? _OutOrderPosID;
    public Guid? OutOrderPosID 
    {
        get { return _OutOrderPosID; }
        set { SetProperty<Guid?>(ref _OutOrderPosID, value); }
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

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
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

    private Invoice _Invoice;
    public virtual Invoice Invoice
    { 
        get => LazyLoader.Load(this, ref _Invoice);
        set => _Invoice = value;
    }

    public bool Invoice_IsLoaded
    {
        get
        {
            return Invoice != null;
        }
    }

    public virtual ReferenceEntry InvoiceReference 
    {
        get { return Context.Entry(this).Reference("Invoice"); }
    }
    
    private MDCountrySalesTax _MDCountrySalesTax;
    public virtual MDCountrySalesTax MDCountrySalesTax
    { 
        get => LazyLoader.Load(this, ref _MDCountrySalesTax);
        set => _MDCountrySalesTax = value;
    }

    public bool MDCountrySalesTax_IsLoaded
    {
        get
        {
            return MDCountrySalesTax != null;
        }
    }

    public virtual ReferenceEntry MDCountrySalesTaxReference 
    {
        get { return Context.Entry(this).Reference("MDCountrySalesTax"); }
    }
    
    private MDCountrySalesTaxMDMaterialGroup _MDCountrySalesTaxMDMaterialGroup;
    public virtual MDCountrySalesTaxMDMaterialGroup MDCountrySalesTaxMDMaterialGroup
    { 
        get => LazyLoader.Load(this, ref _MDCountrySalesTaxMDMaterialGroup);
        set => _MDCountrySalesTaxMDMaterialGroup = value;
    }

    public bool MDCountrySalesTaxMDMaterialGroup_IsLoaded
    {
        get
        {
            return MDCountrySalesTaxMDMaterialGroup != null;
        }
    }

    public virtual ReferenceEntry MDCountrySalesTaxMDMaterialGroupReference 
    {
        get { return Context.Entry(this).Reference("MDCountrySalesTaxMDMaterialGroup"); }
    }
    
    private MDCountrySalesTaxMaterial _MDCountrySalesTaxMaterial;
    public virtual MDCountrySalesTaxMaterial MDCountrySalesTaxMaterial
    { 
        get => LazyLoader.Load(this, ref _MDCountrySalesTaxMaterial);
        set => _MDCountrySalesTaxMaterial = value;
    }

    public bool MDCountrySalesTaxMaterial_IsLoaded
    {
        get
        {
            return MDCountrySalesTaxMaterial != null;
        }
    }

    public virtual ReferenceEntry MDCountrySalesTaxMaterialReference 
    {
        get { return Context.Entry(this).Reference("MDCountrySalesTaxMaterial"); }
    }
    
    private MDUnit _MDUnit;
    public virtual MDUnit MDUnit
    { 
        get => LazyLoader.Load(this, ref _MDUnit);
        set => _MDUnit = value;
    }

    public bool MDUnit_IsLoaded
    {
        get
        {
            return MDUnit != null;
        }
    }

    public virtual ReferenceEntry MDUnitReference 
    {
        get { return Context.Entry(this).Reference("MDUnit"); }
    }
    
    private Material _Material;
    public virtual Material Material
    { 
        get => LazyLoader.Load(this, ref _Material);
        set => _Material = value;
    }

    public bool Material_IsLoaded
    {
        get
        {
            return Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private OutOrderPos _OutOrderPos;
    public virtual OutOrderPos OutOrderPos
    { 
        get => LazyLoader.Load(this, ref _OutOrderPos);
        set => _OutOrderPos = value;
    }

    public bool OutOrderPos_IsLoaded
    {
        get
        {
            return OutOrderPos != null;
        }
    }

    public virtual ReferenceEntry OutOrderPosReference 
    {
        get { return Context.Entry(this).Reference("OutOrderPos"); }
    }
    }
