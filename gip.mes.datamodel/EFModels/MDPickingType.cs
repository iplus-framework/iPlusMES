﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDPickingType : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDPickingType()
    {
    }

    private MDPickingType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDPickingTypeID;
    public Guid MDPickingTypeID 
    {
        get { return _MDPickingTypeID; }
        set { SetProperty<Guid>(ref _MDPickingTypeID, value); }
    }

    short _MDPickingTypeIndex;
    public short MDPickingTypeIndex 
    {
        get { return _MDPickingTypeIndex; }
        set { SetProperty<short>(ref _MDPickingTypeIndex, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
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

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
    }

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
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

    private ICollection<FacilityMDSchedulingGroup> _FacilityMDSchedulingGroup_MDPickingType;
    public virtual ICollection<FacilityMDSchedulingGroup> FacilityMDSchedulingGroup_MDPickingType
    {
        get { return LazyLoader.Load(this, ref _FacilityMDSchedulingGroup_MDPickingType); }
        set { _FacilityMDSchedulingGroup_MDPickingType = value; }
    }

    public bool FacilityMDSchedulingGroup_MDPickingType_IsLoaded
    {
        get
        {
            return FacilityMDSchedulingGroup_MDPickingType != null;
        }
    }

    public virtual CollectionEntry FacilityMDSchedulingGroup_MDPickingTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityMDSchedulingGroup_MDPickingType); }
    }

    private ICollection<Picking> _Picking_MDPickingType;
    public virtual ICollection<Picking> Picking_MDPickingType
    {
        get { return LazyLoader.Load(this, ref _Picking_MDPickingType); }
        set { _Picking_MDPickingType = value; }
    }

    public bool Picking_MDPickingType_IsLoaded
    {
        get
        {
            return Picking_MDPickingType != null;
        }
    }

    public virtual CollectionEntry Picking_MDPickingTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.Picking_MDPickingType); }
    }
}