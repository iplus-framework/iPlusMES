using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityInventory : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public FacilityInventory()
    {
    }

    private FacilityInventory(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _FacilityInventoryID;
    public Guid FacilityInventoryID 
    {
        get { return _FacilityInventoryID; }
        set { SetProperty<Guid>(ref _FacilityInventoryID, value); }
    }

    string _FacilityInventoryNo;
    public string FacilityInventoryNo 
    {
        get { return _FacilityInventoryNo; }
        set { SetProperty<string>(ref _FacilityInventoryNo, value); }
    }

    string _FacilityInventoryName;
    public string FacilityInventoryName 
    {
        get { return _FacilityInventoryName; }
        set { SetProperty<string>(ref _FacilityInventoryName, value); }
    }

    Guid _MDFacilityInventoryStateID;
    public Guid MDFacilityInventoryStateID 
    {
        get { return _MDFacilityInventoryStateID; }
        set { SetForeignKeyProperty<Guid>(ref _MDFacilityInventoryStateID, value, "MDFacilityInventoryState", _MDFacilityInventoryState, MDFacilityInventoryState != null ? MDFacilityInventoryState.MDFacilityInventoryStateID : default(Guid)); }
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

    Guid? _FacilityID;
    public Guid? FacilityID 
    {
        get { return _FacilityID; }
        set { SetForeignKeyProperty<Guid?>(ref _FacilityID, value, "Facility", _Facility, Facility != null ? Facility.FacilityID : default(Guid?)); }
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
    
    private ICollection<FacilityInventoryPos> _FacilityInventoryPos_FacilityInventory;
    public virtual ICollection<FacilityInventoryPos> FacilityInventoryPos_FacilityInventory
    {
        get { return LazyLoader.Load(this, ref _FacilityInventoryPos_FacilityInventory); }
        set { SetProperty<ICollection<FacilityInventoryPos>>(ref _FacilityInventoryPos_FacilityInventory, value); }
    }

    public bool FacilityInventoryPos_FacilityInventory_IsLoaded
    {
        get
        {
            return _FacilityInventoryPos_FacilityInventory != null;
        }
    }

    public virtual CollectionEntry FacilityInventoryPos_FacilityInventoryReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityInventoryPos_FacilityInventory); }
    }

    private MDFacilityInventoryState _MDFacilityInventoryState;
    public virtual MDFacilityInventoryState MDFacilityInventoryState
    { 
        get { return LazyLoader.Load(this, ref _MDFacilityInventoryState); } 
        set { SetProperty<MDFacilityInventoryState>(ref _MDFacilityInventoryState, value); }
    }

    public bool MDFacilityInventoryState_IsLoaded
    {
        get
        {
            return _MDFacilityInventoryState != null;
        }
    }

    public virtual ReferenceEntry MDFacilityInventoryStateReference 
    {
        get { return Context.Entry(this).Reference("MDFacilityInventoryState"); }
    }
    }
