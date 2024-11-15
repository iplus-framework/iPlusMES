using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class DemandOrderPos : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence, ITargetQuantity
{

    public DemandOrderPos()
    {
    }

    private DemandOrderPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _DemandOrderPosID;
    public Guid DemandOrderPosID 
    {
        get { return _DemandOrderPosID; }
        set { SetProperty<Guid>(ref _DemandOrderPosID, value); }
    }

    Guid? _DemandOrderID;
    public Guid? DemandOrderID 
    {
        get { return _DemandOrderID; }
        set { SetProperty<Guid?>(ref _DemandOrderID, value); }
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

    double _TargetQuantity;
    public double TargetQuantity 
    {
        get { return _TargetQuantity; }
        set { SetProperty<double>(ref _TargetQuantity, value); }
    }

    double _TargetWeight;
    public double TargetWeight 
    {
        get { return _TargetWeight; }
        set { SetProperty<double>(ref _TargetWeight, value); }
    }

    Guid? _PartslistID;
    public Guid? PartslistID 
    {
        get { return _PartslistID; }
        set { SetProperty<Guid?>(ref _PartslistID, value); }
    }

    Guid _VBiProgramACClassMethodID;
    public Guid VBiProgramACClassMethodID 
    {
        get { return _VBiProgramACClassMethodID; }
        set { SetProperty<Guid>(ref _VBiProgramACClassMethodID, value); }
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

    Guid? _ACProgramID;
    public Guid? ACProgramID 
    {
        get { return _ACProgramID; }
        set { SetProperty<Guid?>(ref _ACProgramID, value); }
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

    string _LineNumber;
    public string LineNumber 
    {
        get { return _LineNumber; }
        set { SetProperty<string>(ref _LineNumber, value); }
    }

    private ACProgram _ACProgram;
    public virtual ACProgram ACProgram
    { 
        get { return LazyLoader.Load(this, ref _ACProgram); } 
        set { SetProperty<ACProgram>(ref _ACProgram, value); }
    }

    public bool ACProgram_IsLoaded
    {
        get
        {
            return _ACProgram != null;
        }
    }

    public virtual ReferenceEntry ACProgramReference 
    {
        get { return Context.Entry(this).Reference("ACProgram"); }
    }
    
    private DemandOrder _DemandOrder;
    public virtual DemandOrder DemandOrder
    { 
        get { return LazyLoader.Load(this, ref _DemandOrder); } 
        set { SetProperty<DemandOrder>(ref _DemandOrder, value); }
    }

    public bool DemandOrder_IsLoaded
    {
        get
        {
            return _DemandOrder != null;
        }
    }

    public virtual ReferenceEntry DemandOrderReference 
    {
        get { return Context.Entry(this).Reference("DemandOrder"); }
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
    
    private Partslist _Partslist;
    public virtual Partslist Partslist
    { 
        get { return LazyLoader.Load(this, ref _Partslist); } 
        set { SetProperty<Partslist>(ref _Partslist, value); }
    }

    public bool Partslist_IsLoaded
    {
        get
        {
            return _Partslist != null;
        }
    }

    public virtual ReferenceEntry PartslistReference 
    {
        get { return Context.Entry(this).Reference("Partslist"); }
    }
    
    private ACClassMethod _VBiProgramACClassMethod;
    public virtual ACClassMethod VBiProgramACClassMethod
    { 
        get { return LazyLoader.Load(this, ref _VBiProgramACClassMethod); } 
        set { SetProperty<ACClassMethod>(ref _VBiProgramACClassMethod, value); }
    }

    public bool VBiProgramACClassMethod_IsLoaded
    {
        get
        {
            return _VBiProgramACClassMethod != null;
        }
    }

    public virtual ReferenceEntry VBiProgramACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("VBiProgramACClassMethod"); }
    }
    }
