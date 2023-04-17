using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDUnit : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MDUnit()
    {
    }

    private MDUnit(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDUnitID;
    public Guid MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetProperty<Guid>(ref _MDUnitID, value); }
    }

    string _MDUnitNameTrans;
    public string MDUnitNameTrans 
    {
        get { return _MDUnitNameTrans; }
        set { SetProperty<string>(ref _MDUnitNameTrans, value); }
    }

    string _SymbolTrans;
    public string SymbolTrans 
    {
        get { return _SymbolTrans; }
        set { SetProperty<string>(ref _SymbolTrans, value); }
    }

    string _TechnicalSymbol;
    public string TechnicalSymbol 
    {
        get { return _TechnicalSymbol; }
        set { SetProperty<string>(ref _TechnicalSymbol, value); }
    }

    short _SIDimensionIndex;
    public short SIDimensionIndex 
    {
        get { return _SIDimensionIndex; }
        set { SetProperty<short>(ref _SIDimensionIndex, value); }
    }

    bool _IsSIUnit;
    public bool IsSIUnit 
    {
        get { return _IsSIUnit; }
        set { SetProperty<bool>(ref _IsSIUnit, value); }
    }

    int _Rounding;
    public int Rounding 
    {
        get { return _Rounding; }
        set { SetProperty<int>(ref _Rounding, value); }
    }

    bool _IsQuantityUnit;
    public bool IsQuantityUnit 
    {
        get { return _IsQuantityUnit; }
        set { SetProperty<bool>(ref _IsQuantityUnit, value); }
    }

    string _ISOCode;
    public string ISOCode 
    {
        get { return _ISOCode; }
        set { SetProperty<string>(ref _ISOCode, value); }
    }

    short _SortIndex;
    public short SortIndex 
    {
        get { return _SortIndex; }
        set { SetProperty<short>(ref _SortIndex, value); }
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

    string _KeyOfExtSys;
    public string KeyOfExtSys 
    {
        get { return _KeyOfExtSys; }
        set { SetProperty<string>(ref _KeyOfExtSys, value); }
    }

    private ICollection<CompanyMaterial> _CompanyMaterial_MDUnit;
    public virtual ICollection<CompanyMaterial> CompanyMaterial_MDUnit
    {
        get => LazyLoader.Load(this, ref _CompanyMaterial_MDUnit);
        set => _CompanyMaterial_MDUnit = value;
    }

    public bool CompanyMaterial_MDUnit_IsLoaded
    {
        get
        {
            return CompanyMaterial_MDUnit != null;
        }
    }

    public virtual CollectionEntry CompanyMaterial_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyMaterial_MDUnit); }
    }

    private ICollection<Facility> _Facility_MDUnit;
    public virtual ICollection<Facility> Facility_MDUnit
    {
        get => LazyLoader.Load(this, ref _Facility_MDUnit);
        set => _Facility_MDUnit = value;
    }

    public bool Facility_MDUnit_IsLoaded
    {
        get
        {
            return Facility_MDUnit != null;
        }
    }

    public virtual CollectionEntry Facility_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_MDUnit); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_MDUnit;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_MDUnit
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_MDUnit);
        set => _FacilityBookingCharge_MDUnit = value;
    }

    public bool FacilityBookingCharge_MDUnit_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_MDUnit != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_MDUnit); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_MDUnit;
    public virtual ICollection<FacilityBooking> FacilityBooking_MDUnit
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_MDUnit);
        set => _FacilityBooking_MDUnit = value;
    }

    public bool FacilityBooking_MDUnit_IsLoaded
    {
        get
        {
            return FacilityBooking_MDUnit != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_MDUnit); }
    }

    private ICollection<FacilityCharge> _FacilityCharge_MDUnit;
    public virtual ICollection<FacilityCharge> FacilityCharge_MDUnit
    {
        get => LazyLoader.Load(this, ref _FacilityCharge_MDUnit);
        set => _FacilityCharge_MDUnit = value;
    }

    public bool FacilityCharge_MDUnit_IsLoaded
    {
        get
        {
            return FacilityCharge_MDUnit != null;
        }
    }

    public virtual CollectionEntry FacilityCharge_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityCharge_MDUnit); }
    }

    private ICollection<InOrderPos> _InOrderPo_MDUnit;
    public virtual ICollection<InOrderPos> InOrderPo_MDUnit
    {
        get => LazyLoader.Load(this, ref _InOrderPo_MDUnit);
        set => _InOrderPo_MDUnit = value;
    }

    public bool InOrderPo_MDUnit_IsLoaded
    {
        get
        {
            return InOrderPo_MDUnit != null;
        }
    }

    public virtual CollectionEntry InOrderPo_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPo_MDUnit); }
    }

    private ICollection<InRequestPos> _InRequestPo_MDUnit;
    public virtual ICollection<InRequestPos> InRequestPo_MDUnit
    {
        get => LazyLoader.Load(this, ref _InRequestPo_MDUnit);
        set => _InRequestPo_MDUnit = value;
    }

    public bool InRequestPo_MDUnit_IsLoaded
    {
        get
        {
            return InRequestPo_MDUnit != null;
        }
    }

    public virtual CollectionEntry InRequestPo_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestPo_MDUnit); }
    }

    private ICollection<InvoicePos> _InvoicePo_MDUnit;
    public virtual ICollection<InvoicePos> InvoicePo_MDUnit
    {
        get => LazyLoader.Load(this, ref _InvoicePo_MDUnit);
        set => _InvoicePo_MDUnit = value;
    }

    public bool InvoicePo_MDUnit_IsLoaded
    {
        get
        {
            return InvoicePo_MDUnit != null;
        }
    }

    public virtual CollectionEntry InvoicePo_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.InvoicePo_MDUnit); }
    }

    private ICollection<MDGMPAdditive> _MDGMPAdditive_MDQuantityUnit;
    public virtual ICollection<MDGMPAdditive> MDGMPAdditive_MDQuantityUnit
    {
        get => LazyLoader.Load(this, ref _MDGMPAdditive_MDQuantityUnit);
        set => _MDGMPAdditive_MDQuantityUnit = value;
    }

    public bool MDGMPAdditive_MDQuantityUnit_IsLoaded
    {
        get
        {
            return MDGMPAdditive_MDQuantityUnit != null;
        }
    }

    public virtual CollectionEntry MDGMPAdditive_MDQuantityUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.MDGMPAdditive_MDQuantityUnit); }
    }

    private ICollection<MDUnitConversion> _MDUnitConversion_MDUnit;
    public virtual ICollection<MDUnitConversion> MDUnitConversion_MDUnit
    {
        get => LazyLoader.Load(this, ref _MDUnitConversion_MDUnit);
        set => _MDUnitConversion_MDUnit = value;
    }

    public bool MDUnitConversion_MDUnit_IsLoaded
    {
        get
        {
            return MDUnitConversion_MDUnit != null;
        }
    }

    public virtual CollectionEntry MDUnitConversion_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.MDUnitConversion_MDUnit); }
    }

    private ICollection<MDUnitConversion> _MDUnitConversion_ToMDUnit;
    public virtual ICollection<MDUnitConversion> MDUnitConversion_ToMDUnit
    {
        get => LazyLoader.Load(this, ref _MDUnitConversion_ToMDUnit);
        set => _MDUnitConversion_ToMDUnit = value;
    }

    public bool MDUnitConversion_ToMDUnit_IsLoaded
    {
        get
        {
            return MDUnitConversion_ToMDUnit != null;
        }
    }

    public virtual CollectionEntry MDUnitConversion_ToMDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.MDUnitConversion_ToMDUnit); }
    }

    private ICollection<MaterialUnit> _MaterialUnit_ToMDUnit;
    public virtual ICollection<MaterialUnit> MaterialUnit_ToMDUnit
    {
        get => LazyLoader.Load(this, ref _MaterialUnit_ToMDUnit);
        set => _MaterialUnit_ToMDUnit = value;
    }

    public bool MaterialUnit_ToMDUnit_IsLoaded
    {
        get
        {
            return MaterialUnit_ToMDUnit != null;
        }
    }

    public virtual CollectionEntry MaterialUnit_ToMDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialUnit_ToMDUnit); }
    }

    private ICollection<Material> _Material_BaseMDUnit;
    public virtual ICollection<Material> Material_BaseMDUnit
    {
        get => LazyLoader.Load(this, ref _Material_BaseMDUnit);
        set => _Material_BaseMDUnit = value;
    }

    public bool Material_BaseMDUnit_IsLoaded
    {
        get
        {
            return Material_BaseMDUnit != null;
        }
    }

    public virtual CollectionEntry Material_BaseMDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.Material_BaseMDUnit); }
    }

    private ICollection<OutOfferPos> _OutOfferPo_MDUnit;
    public virtual ICollection<OutOfferPos> OutOfferPo_MDUnit
    {
        get => LazyLoader.Load(this, ref _OutOfferPo_MDUnit);
        set => _OutOfferPo_MDUnit = value;
    }

    public bool OutOfferPo_MDUnit_IsLoaded
    {
        get
        {
            return OutOfferPo_MDUnit != null;
        }
    }

    public virtual CollectionEntry OutOfferPo_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPo_MDUnit); }
    }

    private ICollection<OutOrderPos> _OutOrderPo_MDUnit;
    public virtual ICollection<OutOrderPos> OutOrderPo_MDUnit
    {
        get => LazyLoader.Load(this, ref _OutOrderPo_MDUnit);
        set => _OutOrderPo_MDUnit = value;
    }

    public bool OutOrderPo_MDUnit_IsLoaded
    {
        get
        {
            return OutOrderPo_MDUnit != null;
        }
    }

    public virtual CollectionEntry OutOrderPo_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPo_MDUnit); }
    }

    private ICollection<PartslistPos> _PartslistPo_MDUnit;
    public virtual ICollection<PartslistPos> PartslistPo_MDUnit
    {
        get => LazyLoader.Load(this, ref _PartslistPo_MDUnit);
        set => _PartslistPo_MDUnit = value;
    }

    public bool PartslistPo_MDUnit_IsLoaded
    {
        get
        {
            return PartslistPo_MDUnit != null;
        }
    }

    public virtual CollectionEntry PartslistPo_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistPo_MDUnit); }
    }

    private ICollection<Partslist> _Partslist_MDUnit;
    public virtual ICollection<Partslist> Partslist_MDUnit
    {
        get => LazyLoader.Load(this, ref _Partslist_MDUnit);
        set => _Partslist_MDUnit = value;
    }

    public bool Partslist_MDUnit_IsLoaded
    {
        get
        {
            return Partslist_MDUnit != null;
        }
    }

    public virtual CollectionEntry Partslist_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.Partslist_MDUnit); }
    }

    private ICollection<ProdOrderPartslistPos> _ProdOrderPartslistPo_MDUnit;
    public virtual ICollection<ProdOrderPartslistPos> ProdOrderPartslistPo_MDUnit
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPo_MDUnit);
        set => _ProdOrderPartslistPo_MDUnit = value;
    }

    public bool ProdOrderPartslistPo_MDUnit_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPo_MDUnit != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPo_MDUnitReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPo_MDUnit); }
    }
}
