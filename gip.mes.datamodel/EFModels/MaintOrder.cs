using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaintOrder : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MaintOrder()
    {
    }

    private MaintOrder(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaintOrderID;
    public Guid MaintOrderID 
    {
        get { return _MaintOrderID; }
        set { SetProperty<Guid>(ref _MaintOrderID, value); }
    }

    Guid? _BasedOnMaintOrderID;
    public Guid? BasedOnMaintOrderID 
    {
        get { return _BasedOnMaintOrderID; }
        set { SetForeignKeyProperty<Guid?>(ref _BasedOnMaintOrderID, value, "MaintOrder1_BasedOnMaintOrder", _MaintOrder1_BasedOnMaintOrder, MaintOrder1_BasedOnMaintOrder != null ? MaintOrder1_BasedOnMaintOrder.MaintOrderID : default(Guid?)); }
    }

    string _MaintOrderNo;
    public string MaintOrderNo 
    {
        get { return _MaintOrderNo; }
        set { SetProperty<string>(ref _MaintOrderNo, value); }
    }

    Guid? _MDMaintOrderStateID;
    public Guid? MDMaintOrderStateID 
    {
        get { return _MDMaintOrderStateID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDMaintOrderStateID, value, "MDMaintOrderState", _MDMaintOrderState, MDMaintOrderState != null ? MDMaintOrderState.MDMaintOrderStateID : default(Guid?)); }
    }

    Guid? _MaintACClassID;
    public Guid? MaintACClassID 
    {
        get { return _MaintACClassID; }
        set { SetForeignKeyProperty<Guid?>(ref _MaintACClassID, value, "MaintACClass", _MaintACClass, MaintACClass != null ? MaintACClass.MaintACClassID : default(Guid?)); }
    }

    Guid? _FacilityID;
    public Guid? FacilityID 
    {
        get { return _FacilityID; }
        set { SetForeignKeyProperty<Guid?>(ref _FacilityID, value, "Facility", _Facility, Facility != null ? Facility.FacilityID : default(Guid?)); }
    }

    Guid? _PickingID;
    public Guid? PickingID 
    {
        get { return _PickingID; }
        set { SetForeignKeyProperty<Guid?>(ref _PickingID, value, "Picking", _Picking, Picking != null ? Picking.PickingID : default(Guid?)); }
    }

    int _MaintModeIndex;
    public int MaintModeIndex 
    {
        get { return _MaintModeIndex; }
        set { SetProperty<int>(ref _MaintModeIndex, value); }
    }

    bool _IsActive;
    public bool IsActive 
    {
        get { return _IsActive; }
        set { SetProperty<bool>(ref _IsActive, value); }
    }

    int? _MaintInterval;
    public int? MaintInterval 
    {
        get { return _MaintInterval; }
        set { SetProperty<int?>(ref _MaintInterval, value); }
    }

    DateTime? _LastMaintTerm;
    public DateTime? LastMaintTerm 
    {
        get { return _LastMaintTerm; }
        set { SetProperty<DateTime?>(ref _LastMaintTerm, value); }
    }

    int? _WarningDiff;
    public int? WarningDiff 
    {
        get { return _WarningDiff; }
        set { SetProperty<int?>(ref _WarningDiff, value); }
    }

    DateTime? _PlannedStartDate;
    public DateTime? PlannedStartDate 
    {
        get { return _PlannedStartDate; }
        set { SetProperty<DateTime?>(ref _PlannedStartDate, value); }
    }

    int? _PlannedDuration;
    public int? PlannedDuration 
    {
        get { return _PlannedDuration; }
        set { SetProperty<int?>(ref _PlannedDuration, value); }
    }

    DateTime? _StartDate;
    public DateTime? StartDate 
    {
        get { return _StartDate; }
        set { SetProperty<DateTime?>(ref _StartDate, value); }
    }

    DateTime? _EndDate;
    public DateTime? EndDate 
    {
        get { return _EndDate; }
        set { SetProperty<DateTime?>(ref _EndDate, value); }
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

    Guid? _VBiPAACClassID;
    public Guid? VBiPAACClassID 
    {
        get { return _VBiPAACClassID; }
        set { SetForeignKeyProperty<Guid?>(ref _VBiPAACClassID, value, "VBiPAACClass", _VBiPAACClass, VBiPAACClass != null ? VBiPAACClass.ACClassID : default(Guid?)); }
    }

    private MaintOrder _MaintOrder1_BasedOnMaintOrder;
    public virtual MaintOrder MaintOrder1_BasedOnMaintOrder
    { 
        get { return LazyLoader.Load(this, ref _MaintOrder1_BasedOnMaintOrder); } 
        set { SetProperty<MaintOrder>(ref _MaintOrder1_BasedOnMaintOrder, value); }
    }

    public bool MaintOrder1_BasedOnMaintOrder_IsLoaded
    {
        get
        {
            return _MaintOrder1_BasedOnMaintOrder != null;
        }
    }

    public virtual ReferenceEntry MaintOrder1_BasedOnMaintOrderReference 
    {
        get { return Context.Entry(this).Reference("MaintOrder1_BasedOnMaintOrder"); }
    }
    
    private Facility _Facility;
    public virtual Facility Facility
    { 
        get { return LazyLoader.Load(this, ref _Facility); } 
        set { SetProperty<Facility>(ref _Facility, value); }
    }

    public bool Facility_IsLoaded
    {
        get
        {
            return _Facility != null;
        }
    }

    public virtual ReferenceEntry FacilityReference 
    {
        get { return Context.Entry(this).Reference("Facility"); }
    }
    
    private ICollection<MaintOrder> _MaintOrder_BasedOnMaintOrder;
    public virtual ICollection<MaintOrder> MaintOrder_BasedOnMaintOrder
    {
        get { return LazyLoader.Load(this, ref _MaintOrder_BasedOnMaintOrder); }
        set { SetProperty<ICollection<MaintOrder>>(ref _MaintOrder_BasedOnMaintOrder, value); }
    }

    public bool MaintOrder_BasedOnMaintOrder_IsLoaded
    {
        get
        {
            return _MaintOrder_BasedOnMaintOrder != null;
        }
    }

    public virtual CollectionEntry MaintOrder_BasedOnMaintOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrder_BasedOnMaintOrder); }
    }

    private MDMaintOrderState _MDMaintOrderState;
    public virtual MDMaintOrderState MDMaintOrderState
    { 
        get { return LazyLoader.Load(this, ref _MDMaintOrderState); } 
        set { SetProperty<MDMaintOrderState>(ref _MDMaintOrderState, value); }
    }

    public bool MDMaintOrderState_IsLoaded
    {
        get
        {
            return _MDMaintOrderState != null;
        }
    }

    public virtual ReferenceEntry MDMaintOrderStateReference 
    {
        get { return Context.Entry(this).Reference("MDMaintOrderState"); }
    }
    
    private MaintACClass _MaintACClass;
    public virtual MaintACClass MaintACClass
    { 
        get { return LazyLoader.Load(this, ref _MaintACClass); } 
        set { SetProperty<MaintACClass>(ref _MaintACClass, value); }
    }

    public bool MaintACClass_IsLoaded
    {
        get
        {
            return _MaintACClass != null;
        }
    }

    public virtual ReferenceEntry MaintACClassReference 
    {
        get { return Context.Entry(this).Reference("MaintACClass"); }
    }
    
    private ICollection<MaintOrderAssignment> _MaintOrderAssignment_MaintOrder;
    public virtual ICollection<MaintOrderAssignment> MaintOrderAssignment_MaintOrder
    {
        get { return LazyLoader.Load(this, ref _MaintOrderAssignment_MaintOrder); }
        set { SetProperty<ICollection<MaintOrderAssignment>>(ref _MaintOrderAssignment_MaintOrder, value); }
    }

    public bool MaintOrderAssignment_MaintOrder_IsLoaded
    {
        get
        {
            return _MaintOrderAssignment_MaintOrder != null;
        }
    }

    public virtual CollectionEntry MaintOrderAssignment_MaintOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrderAssignment_MaintOrder); }
    }

    private ICollection<MaintOrderPos> _MaintOrderPos_MaintOrder;
    public virtual ICollection<MaintOrderPos> MaintOrderPos_MaintOrder
    {
        get { return LazyLoader.Load(this, ref _MaintOrderPos_MaintOrder); }
        set { SetProperty<ICollection<MaintOrderPos>>(ref _MaintOrderPos_MaintOrder, value); }
    }

    public bool MaintOrderPos_MaintOrder_IsLoaded
    {
        get
        {
            return _MaintOrderPos_MaintOrder != null;
        }
    }

    public virtual CollectionEntry MaintOrderPos_MaintOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrderPos_MaintOrder); }
    }

    private ICollection<MaintOrderProperty> _MaintOrderProperty_MaintOrder;
    public virtual ICollection<MaintOrderProperty> MaintOrderProperty_MaintOrder
    {
        get { return LazyLoader.Load(this, ref _MaintOrderProperty_MaintOrder); }
        set { SetProperty<ICollection<MaintOrderProperty>>(ref _MaintOrderProperty_MaintOrder, value); }
    }

    public bool MaintOrderProperty_MaintOrder_IsLoaded
    {
        get
        {
            return _MaintOrderProperty_MaintOrder != null;
        }
    }

    public virtual CollectionEntry MaintOrderProperty_MaintOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrderProperty_MaintOrder); }
    }

    private ICollection<MaintOrderTask> _MaintOrderTask_MaintOrder;
    public virtual ICollection<MaintOrderTask> MaintOrderTask_MaintOrder
    {
        get { return LazyLoader.Load(this, ref _MaintOrderTask_MaintOrder); }
        set { SetProperty<ICollection<MaintOrderTask>>(ref _MaintOrderTask_MaintOrder, value); }
    }

    public bool MaintOrderTask_MaintOrder_IsLoaded
    {
        get
        {
            return _MaintOrderTask_MaintOrder != null;
        }
    }

    public virtual CollectionEntry MaintOrderTask_MaintOrderReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintOrderTask_MaintOrder); }
    }

    private Picking _Picking;
    public virtual Picking Picking
    { 
        get { return LazyLoader.Load(this, ref _Picking); } 
        set { SetProperty<Picking>(ref _Picking, value); }
    }

    public bool Picking_IsLoaded
    {
        get
        {
            return _Picking != null;
        }
    }

    public virtual ReferenceEntry PickingReference 
    {
        get { return Context.Entry(this).Reference("Picking"); }
    }
    
    private ACClass _VBiPAACClass;
    public virtual ACClass VBiPAACClass
    { 
        get { return LazyLoader.Load(this, ref _VBiPAACClass); } 
        set { SetProperty<ACClass>(ref _VBiPAACClass, value); }
    }

    public bool VBiPAACClass_IsLoaded
    {
        get
        {
            return _VBiPAACClass != null;
        }
    }

    public virtual ReferenceEntry VBiPAACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiPAACClass"); }
    }
    }
