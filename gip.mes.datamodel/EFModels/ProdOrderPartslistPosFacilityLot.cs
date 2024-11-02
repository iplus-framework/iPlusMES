// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ProdOrderPartslistPosFacilityLot : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ProdOrderPartslistPosFacilityLot()
    {
    }

    private ProdOrderPartslistPosFacilityLot(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ProdOrderPartslistPosFacilityLotID;
    public Guid ProdOrderPartslistPosFacilityLotID 
    {
        get { return _ProdOrderPartslistPosFacilityLotID; }
        set { SetProperty<Guid>(ref _ProdOrderPartslistPosFacilityLotID, value); }
    }

    Guid _ProdOrderPartslistPosID;
    public Guid ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid>(ref _ProdOrderPartslistPosID, value); }
    }

    Guid _FacilityLotID;
    public Guid FacilityLotID 
    {
        get { return _FacilityLotID; }
        set { SetProperty<Guid>(ref _FacilityLotID, value); }
    }

    bool? _IsActive;
    public bool? IsActive 
    {
        get { return _IsActive; }
        set { SetProperty<bool?>(ref _IsActive, value); }
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

    private ICollection<FacilityBookingCharge> _FacilityBookingCharge_ProdOrderPartslistPosFacilityLot;
    public virtual ICollection<FacilityBookingCharge> FacilityBookingCharge_ProdOrderPartslistPosFacilityLot
    {
        get { return LazyLoader.Load(this, ref _FacilityBookingCharge_ProdOrderPartslistPosFacilityLot); }
        set { _FacilityBookingCharge_ProdOrderPartslistPosFacilityLot = value; }
    }

    public bool FacilityBookingCharge_ProdOrderPartslistPosFacilityLot_IsLoaded
    {
        get
        {
            return FacilityBookingCharge_ProdOrderPartslistPosFacilityLot != null;
        }
    }

    public virtual CollectionEntry FacilityBookingCharge_ProdOrderPartslistPosFacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBookingCharge_ProdOrderPartslistPosFacilityLot); }
    }

    private ICollection<FacilityBooking> _FacilityBooking_ProdOrderPartslistPosFacilityLot;
    public virtual ICollection<FacilityBooking> FacilityBooking_ProdOrderPartslistPosFacilityLot
    {
        get { return LazyLoader.Load(this, ref _FacilityBooking_ProdOrderPartslistPosFacilityLot); }
        set { _FacilityBooking_ProdOrderPartslistPosFacilityLot = value; }
    }

    public bool FacilityBooking_ProdOrderPartslistPosFacilityLot_IsLoaded
    {
        get
        {
            return FacilityBooking_ProdOrderPartslistPosFacilityLot != null;
        }
    }

    public virtual CollectionEntry FacilityBooking_ProdOrderPartslistPosFacilityLotReference
    {
        get { return Context.Entry(this).Collection(c => c.FacilityBooking_ProdOrderPartslistPosFacilityLot); }
    }

    private FacilityLot _FacilityLot;
    public virtual FacilityLot FacilityLot
    { 
        get { return LazyLoader.Load(this, ref _FacilityLot); } 
        set { SetProperty<FacilityLot>(ref _FacilityLot, value); }
    }

    public bool FacilityLot_IsLoaded
    {
        get
        {
            return FacilityLot != null;
        }
    }

    public virtual ReferenceEntry FacilityLotReference 
    {
        get { return Context.Entry(this).Reference("FacilityLot"); }
    }
    
    private ProdOrderPartslistPos _ProdOrderPartslistPos;
    public virtual ProdOrderPartslistPos ProdOrderPartslistPos
    { 
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistPos); } 
        set { SetProperty<ProdOrderPartslistPos>(ref _ProdOrderPartslistPos, value); }
    }

    public bool ProdOrderPartslistPos_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos != null;
        }
    }

    public virtual ReferenceEntry ProdOrderPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("ProdOrderPartslistPos"); }
    }
    }
