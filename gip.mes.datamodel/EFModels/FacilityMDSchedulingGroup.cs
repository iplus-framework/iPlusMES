using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class FacilityMDSchedulingGroup : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public FacilityMDSchedulingGroup()
    {
    }

    private FacilityMDSchedulingGroup(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _FacilityMDSchedulingGroupID;
    public Guid FacilityMDSchedulingGroupID 
    {
        get { return _FacilityMDSchedulingGroupID; }
        set { SetProperty<Guid>(ref _FacilityMDSchedulingGroupID, value); }
    }

    Guid _FacilityID;
    public Guid FacilityID 
    {
        get { return _FacilityID; }
        set { SetProperty<Guid>(ref _FacilityID, value); }
    }

    Guid _MDSchedulingGroupID;
    public Guid MDSchedulingGroupID 
    {
        get { return _MDSchedulingGroupID; }
        set { SetProperty<Guid>(ref _MDSchedulingGroupID, value); }
    }

    Guid? _MDPickingTypeID;
    public Guid? MDPickingTypeID 
    {
        get { return _MDPickingTypeID; }
        set { SetProperty<Guid?>(ref _MDPickingTypeID, value); }
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
            return Facility != null;
        }
    }

    public virtual ReferenceEntry FacilityReference 
    {
        get { return Context.Entry(this).Reference("Facility"); }
    }
    
    private MDPickingType _MDPickingType;
    public virtual MDPickingType MDPickingType
    { 
        get { return LazyLoader.Load(this, ref _MDPickingType); } 
        set { SetProperty<MDPickingType>(ref _MDPickingType, value); }
    }

    public bool MDPickingType_IsLoaded
    {
        get
        {
            return MDPickingType != null;
        }
    }

    public virtual ReferenceEntry MDPickingTypeReference 
    {
        get { return Context.Entry(this).Reference("MDPickingType"); }
    }
    
    private MDSchedulingGroup _MDSchedulingGroup;
    public virtual MDSchedulingGroup MDSchedulingGroup
    { 
        get { return LazyLoader.Load(this, ref _MDSchedulingGroup); } 
        set { SetProperty<MDSchedulingGroup>(ref _MDSchedulingGroup, value); }
    }

    public bool MDSchedulingGroup_IsLoaded
    {
        get
        {
            return MDSchedulingGroup != null;
        }
    }

    public virtual ReferenceEntry MDSchedulingGroupReference 
    {
        get { return Context.Entry(this).Reference("MDSchedulingGroup"); }
    }
    }
