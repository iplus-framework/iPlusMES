// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3FilterTracking : VBEntityObject
{

    public TandTv3FilterTracking()
    {
    }

    private TandTv3FilterTracking(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _TandTv3FilterTrackingID;
    public Guid TandTv3FilterTrackingID 
    {
        get { return _TandTv3FilterTrackingID; }
        set { SetProperty<Guid>(ref _TandTv3FilterTrackingID, value); }
    }

    string _TandTv3MDTrackingDirectionID;
    public string TandTv3MDTrackingDirectionID 
    {
        get { return _TandTv3MDTrackingDirectionID; }
        set { SetProperty<string>(ref _TandTv3MDTrackingDirectionID, value); }
    }

    string _TandTv3MDTrackingStartItemTypeID;
    public string TandTv3MDTrackingStartItemTypeID 
    {
        get { return _TandTv3MDTrackingStartItemTypeID; }
        set { SetProperty<string>(ref _TandTv3MDTrackingStartItemTypeID, value); }
    }

    string _FilterTrackingNo;
    public string FilterTrackingNo 
    {
        get { return _FilterTrackingNo; }
        set { SetProperty<string>(ref _FilterTrackingNo, value); }
    }

    DateTime? _FilterDateFrom;
    public DateTime? FilterDateFrom 
    {
        get { return _FilterDateFrom; }
        set { SetProperty<DateTime?>(ref _FilterDateFrom, value); }
    }

    DateTime? _FilterDateTo;
    public DateTime? FilterDateTo 
    {
        get { return _FilterDateTo; }
        set { SetProperty<DateTime?>(ref _FilterDateTo, value); }
    }

    DateTime _StartTime;
    public DateTime StartTime 
    {
        get { return _StartTime; }
        set { SetProperty<DateTime>(ref _StartTime, value); }
    }

    DateTime? _EndTime;
    public DateTime? EndTime 
    {
        get { return _EndTime; }
        set { SetProperty<DateTime?>(ref _EndTime, value); }
    }

    string _ItemSystemNo;
    public string ItemSystemNo 
    {
        get { return _ItemSystemNo; }
        set { SetProperty<string>(ref _ItemSystemNo, value); }
    }

    Guid _PrimaryKeyID;
    public Guid PrimaryKeyID 
    {
        get { return _PrimaryKeyID; }
        set { SetProperty<Guid>(ref _PrimaryKeyID, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    private ICollection<TandTv3FilterTrackingMaterial> _TandTv3FilterTrackingMaterial_TandTv3FilterTracking;
    public virtual ICollection<TandTv3FilterTrackingMaterial> TandTv3FilterTrackingMaterial_TandTv3FilterTracking
    {
        get { return LazyLoader.Load(this, ref _TandTv3FilterTrackingMaterial_TandTv3FilterTracking); }
        set { _TandTv3FilterTrackingMaterial_TandTv3FilterTracking = value; }
    }

    public bool TandTv3FilterTrackingMaterial_TandTv3FilterTracking_IsLoaded
    {
        get
        {
            return TandTv3FilterTrackingMaterial_TandTv3FilterTracking != null;
        }
    }

    public virtual CollectionEntry TandTv3FilterTrackingMaterial_TandTv3FilterTrackingReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3FilterTrackingMaterial_TandTv3FilterTracking); }
    }

    private TandTv3MDTrackingDirection _TandTv3MDTrackingDirection;
    public virtual TandTv3MDTrackingDirection TandTv3MDTrackingDirection
    { 
        get { return LazyLoader.Load(this, ref _TandTv3MDTrackingDirection); } 
        set { SetProperty<TandTv3MDTrackingDirection>(ref _TandTv3MDTrackingDirection, value); }
    }

    public bool TandTv3MDTrackingDirection_IsLoaded
    {
        get
        {
            return TandTv3MDTrackingDirection != null;
        }
    }

    public virtual ReferenceEntry TandTv3MDTrackingDirectionReference 
    {
        get { return Context.Entry(this).Reference("TandTv3MDTrackingDirection"); }
    }
    
    private TandTv3MDTrackingStartItemType _TandTv3MDTrackingStartItemType;
    public virtual TandTv3MDTrackingStartItemType TandTv3MDTrackingStartItemType
    { 
        get { return LazyLoader.Load(this, ref _TandTv3MDTrackingStartItemType); } 
        set { SetProperty<TandTv3MDTrackingStartItemType>(ref _TandTv3MDTrackingStartItemType, value); }
    }

    public bool TandTv3MDTrackingStartItemType_IsLoaded
    {
        get
        {
            return TandTv3MDTrackingStartItemType != null;
        }
    }

    public virtual ReferenceEntry TandTv3MDTrackingStartItemTypeReference 
    {
        get { return Context.Entry(this).Reference("TandTv3MDTrackingStartItemType"); }
    }
    
    private ICollection<TandTv3Step> _TandTv3Step_TandTv3FilterTracking;
    public virtual ICollection<TandTv3Step> TandTv3Step_TandTv3FilterTracking
    {
        get { return LazyLoader.Load(this, ref _TandTv3Step_TandTv3FilterTracking); }
        set { _TandTv3Step_TandTv3FilterTracking = value; }
    }

    public bool TandTv3Step_TandTv3FilterTracking_IsLoaded
    {
        get
        {
            return TandTv3Step_TandTv3FilterTracking != null;
        }
    }

    public virtual CollectionEntry TandTv3Step_TandTv3FilterTrackingReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3Step_TandTv3FilterTracking); }
    }
}
