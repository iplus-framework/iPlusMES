using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class Partslist : VBEntityObject, IInsertInfo, IUpdateInfo, IDeleteInfo, ITargetQuantity
{

    public Partslist()
    {
    }

    private Partslist(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PartslistID;
    public Guid PartslistID 
    {
        get { return _PartslistID; }
        set { SetProperty<Guid>(ref _PartslistID, value); }
    }

    string _PartslistNo;
    public string PartslistNo 
    {
        get { return _PartslistNo; }
        set { SetProperty<string>(ref _PartslistNo, value); }
    }

    string _PartslistName;
    public string PartslistName 
    {
        get { return _PartslistName; }
        set { SetProperty<string>(ref _PartslistName, value); }
    }

    string _PartslistVersion;
    public string PartslistVersion 
    {
        get { return _PartslistVersion; }
        set { SetProperty<string>(ref _PartslistVersion, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    double _TargetQuantityUOM;
    public double TargetQuantityUOM 
    {
        get { return _TargetQuantityUOM; }
        set { SetProperty<double>(ref _TargetQuantityUOM, value); }
    }

    Guid? _MDUnitID;
    public Guid? MDUnitID 
    {
        get { return _MDUnitID; }
        set { SetProperty<Guid?>(ref _MDUnitID, value); }
    }

    double _TargetQuantity;
    public double TargetQuantity 
    {
        get { return _TargetQuantity; }
        set { SetProperty<double>(ref _TargetQuantity, value); }
    }

    double _GrossWeight;
    public double GrossWeight 
    {
        get { return _GrossWeight; }
        set { SetProperty<double>(ref _GrossWeight, value); }
    }

    bool _IsEnabled;
    public bool IsEnabled 
    {
        get { return _IsEnabled; }
        set { SetProperty<bool>(ref _IsEnabled, value); }
    }

    DateTime? _EnabledFrom;
    public DateTime? EnabledFrom 
    {
        get { return _EnabledFrom; }
        set { SetProperty<DateTime?>(ref _EnabledFrom, value); }
    }

    DateTime? _EnabledTo;
    public DateTime? EnabledTo 
    {
        get { return _EnabledTo; }
        set { SetProperty<DateTime?>(ref _EnabledTo, value); }
    }

    float _ProductionWeight;
    public float ProductionWeight 
    {
        get { return _ProductionWeight; }
        set { SetProperty<float>(ref _ProductionWeight, value); }
    }

    bool _IsStandard;
    public bool IsStandard 
    {
        get { return _IsStandard; }
        set { SetProperty<bool>(ref _IsStandard, value); }
    }

    bool _IsProductionUnit;
    public bool IsProductionUnit 
    {
        get { return _IsProductionUnit; }
        set { SetProperty<bool>(ref _IsProductionUnit, value); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
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

    Guid? _PreviousPartslistID;
    public Guid? PreviousPartslistID 
    {
        get { return _PreviousPartslistID; }
        set { SetProperty<Guid?>(ref _PreviousPartslistID, value); }
    }

    DateTime? _DeleteDate;
    public DateTime? DeleteDate 
    {
        get { return _DeleteDate; }
        set { SetProperty<DateTime?>(ref _DeleteDate, value); }
    }

    string _DeleteName;
    public string DeleteName 
    {
        get { return _DeleteName; }
        set { SetProperty<string>(ref _DeleteName, value); }
    }

    bool? _IsInEnabledPeriod;
    public bool? IsInEnabledPeriod 
    {
        get { return _IsInEnabledPeriod; }
        set { SetProperty<bool?>(ref _IsInEnabledPeriod, value); }
    }

    Guid? _MaterialWFID;
    public Guid? MaterialWFID 
    {
        get { return _MaterialWFID; }
        set { SetProperty<Guid?>(ref _MaterialWFID, value); }
    }

    double? _ProductionUnits;
    public double? ProductionUnits 
    {
        get { return _ProductionUnits; }
        set { SetProperty<double?>(ref _ProductionUnits, value); }
    }

    string _XMLComment;
    public string XMLComment 
    {
        get { return _XMLComment; }
        set { SetProperty<string>(ref _XMLComment, value); }
    }

    DateTime _LastFormulaChange;
    public DateTime LastFormulaChange 
    {
        get { return _LastFormulaChange; }
        set { SetProperty<DateTime>(ref _LastFormulaChange, value); }
    }

    string _KeyOfExtSys;
    public string KeyOfExtSys 
    {
        get { return _KeyOfExtSys; }
        set { SetProperty<string>(ref _KeyOfExtSys, value); }
    }

    private ICollection<DemandOrderPos> _DemandOrderPos_Partslist;
    public virtual ICollection<DemandOrderPos> DemandOrderPos_Partslist
    {
        get => LazyLoader.Load(this, ref _DemandOrderPos_Partslist);
        set => _DemandOrderPos_Partslist = value;
    }

    public bool DemandOrderPos_Partslist_IsLoaded
    {
        get
        {
            return DemandOrderPos_Partslist != null;
        }
    }

    public virtual CollectionEntry DemandOrderPos_PartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.DemandOrderPos_Partslist); }
    }

    private ICollection<Facility> _Facility_Partslist;
    public virtual ICollection<Facility> Facility_Partslist
    {
        get => LazyLoader.Load(this, ref _Facility_Partslist);
        set => _Facility_Partslist = value;
    }

    public bool Facility_Partslist_IsLoaded
    {
        get
        {
            return Facility_Partslist != null;
        }
    }

    public virtual CollectionEntry Facility_PartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_Partslist); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_InwardPartslist;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_InwardPartslist
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_InwardPartslist);
        set => _FacilityBookingCharge_InwardPartslist = value;
    }

    public bool FacilityBookingCharge_InwardPartslist_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_InwardPartslist != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_InwardPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_InwardPartslist); }
    }

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_OutwardPartslist;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_OutwardPartslist
    {
        get => LazyLoader.Load(this, ref _FacilityBookingCharge_OutwardPartslist);
        set => _FacilityBookingCharge_OutwardPartslist = value;
    }

    public bool FacilityBookingCharge_OutwardPartslist_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_OutwardPartslist != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_OutwardPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_OutwardPartslist); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_InwardPartslist;
    public virtual ICollection<FacilityBooking> FacilityBooking_InwardPartslist
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_InwardPartslist);
        set => _FacilityBooking_InwardPartslist = value;
    }

    public bool FacilityBooking_InwardPartslist_IsLoaded
    {
        get
        {
            return FacilityBooking_InwardPartslist != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_InwardPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_InwardPartslist); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_OutwardPartslist;
    public virtual ICollection<FacilityBooking> FacilityBooking_OutwardPartslist
    {
        get => LazyLoader.Load(this, ref _FacilityBooking_OutwardPartslist);
        set => _FacilityBooking_OutwardPartslist = value;
    }

    public bool FacilityBooking_OutwardPartslist_IsLoaded
    {
        get
        {
            return FacilityBooking_OutwardPartslist != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_OutwardPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_OutwardPartslist); }
    }

    private ICollection<FacilityCharge> _FacilityCharge_Partslist;
    public virtual ICollection<FacilityCharge> FacilityCharge_Partslist
    {
        get => LazyLoader.Load(this, ref _FacilityCharge_Partslist);
        set => _FacilityCharge_Partslist = value;
    }

    public bool FacilityCharge_Partslist_IsLoaded
    {
        get
        {
            return FacilityCharge_Partslist != null;
        }
    }

    public virtual CollectionEntry FacilityCharge_PartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityCharge_Partslist); }
    }

    private ICollection<Partslist> _Partslist_PreviousPartslist;
    public virtual ICollection<Partslist> Partslist_PreviousPartslist
    {
        get => LazyLoader.Load(this, ref _Partslist_PreviousPartslist);
        set => _Partslist_PreviousPartslist = value;
    }

    public bool Partslist_PreviousPartslist_IsLoaded
    {
        get
        {
            return Partslist_PreviousPartslist != null;
        }
    }

    public virtual CollectionEntry Partslist_PreviousPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.Partslist_PreviousPartslist); }
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
    
    private MaterialWF _MaterialWF;
    public virtual MaterialWF MaterialWF
    { 
        get => LazyLoader.Load(this, ref _MaterialWF);
        set => _MaterialWF = value;
    }

    public bool MaterialWF_IsLoaded
    {
        get
        {
            return MaterialWF != null;
        }
    }

    public virtual ReferenceEntry MaterialWFReference 
    {
        get { return Context.Entry(this).Reference("MaterialWF"); }
    }
    
    private ICollection<PartslistACClassMethod> _PartslistACClassMethod_Partslist;
    public virtual ICollection<PartslistACClassMethod> PartslistACClassMethod_Partslist
    {
        get => LazyLoader.Load(this, ref _PartslistACClassMethod_Partslist);
        set => _PartslistACClassMethod_Partslist = value;
    }

    public bool PartslistACClassMethod_Partslist_IsLoaded
    {
        get
        {
            return PartslistACClassMethod_Partslist != null;
        }
    }

    public virtual CollectionEntry PartslistACClassMethod_PartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistACClassMethod_Partslist); }
    }

    private ICollection<PartslistConfig> _PartslistConfig_Partslist;
    public virtual ICollection<PartslistConfig> PartslistConfig_Partslist
    {
        get => LazyLoader.Load(this, ref _PartslistConfig_Partslist);
        set => _PartslistConfig_Partslist = value;
    }

    public bool PartslistConfig_Partslist_IsLoaded
    {
        get
        {
            return PartslistConfig_Partslist != null;
        }
    }

    public virtual CollectionEntry PartslistConfig_PartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistConfig_Partslist); }
    }

    private ICollection<PartslistPos> _PartslistPos_ParentPartslist;
    public virtual ICollection<PartslistPos> PartslistPos_ParentPartslist
    {
        get => LazyLoader.Load(this, ref _PartslistPos_ParentPartslist);
        set => _PartslistPos_ParentPartslist = value;
    }

    public bool PartslistPos_ParentPartslist_IsLoaded
    {
        get
        {
            return PartslistPos_ParentPartslist != null;
        }
    }

    public virtual CollectionEntry PartslistPos_ParentPartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistPos_ParentPartslist); }
    }

    private ICollection<PartslistPos> _PartslistPos_Partslist;
    public virtual ICollection<PartslistPos> PartslistPos_Partslist
    {
        get => LazyLoader.Load(this, ref _PartslistPos_Partslist);
        set => _PartslistPos_Partslist = value;
    }

    public bool PartslistPos_Partslist_IsLoaded
    {
        get
        {
            return PartslistPos_Partslist != null;
        }
    }

    public virtual CollectionEntry PartslistPos_PartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistPos_Partslist); }
    }

    private ICollection<PartslistStock> _PartslistStock_Partslist;
    public virtual ICollection<PartslistStock> PartslistStock_Partslist
    {
        get => LazyLoader.Load(this, ref _PartslistStock_Partslist);
        set => _PartslistStock_Partslist = value;
    }

    public bool PartslistStock_Partslist_IsLoaded
    {
        get
        {
            return PartslistStock_Partslist != null;
        }
    }

    public virtual CollectionEntry PartslistStock_PartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.PartslistStock_Partslist); }
    }

    private Partslist _Partslist1_PreviousPartslist;
    public virtual Partslist Partslist1_PreviousPartslist
    { 
        get => LazyLoader.Load(this, ref _Partslist1_PreviousPartslist);
        set => _Partslist1_PreviousPartslist = value;
    }

    public bool Partslist1_PreviousPartslist_IsLoaded
    {
        get
        {
            return Partslist1_PreviousPartslist != null;
        }
    }

    public virtual ReferenceEntry Partslist1_PreviousPartslistReference 
    {
        get { return Context.Entry(this).Reference("Partslist1_PreviousPartslist"); }
    }
    
    private ICollection<ProdOrderPartslist> _ProdOrderPartslist_Partslist;
    public virtual ICollection<ProdOrderPartslist> ProdOrderPartslist_Partslist
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslist_Partslist);
        set => _ProdOrderPartslist_Partslist = value;
    }

    public bool ProdOrderPartslist_Partslist_IsLoaded
    {
        get
        {
            return ProdOrderPartslist_Partslist != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslist_PartslistReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslist_Partslist); }
    }
}
