﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TandTv3MixPointDeliveryNotePos : VBEntityObject
{

    public TandTv3MixPointDeliveryNotePos()
    {
    }

    private TandTv3MixPointDeliveryNotePos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _TandTv3MixPointDeliveryNotePosID;
    public Guid TandTv3MixPointDeliveryNotePosID 
    {
        get { return _TandTv3MixPointDeliveryNotePosID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointDeliveryNotePosID, value); }
    }

    Guid _TandTv3MixPointID;
    public Guid TandTv3MixPointID 
    {
        get { return _TandTv3MixPointID; }
        set { SetProperty<Guid>(ref _TandTv3MixPointID, value); }
    }

    Guid _DeliveryNotePosID;
    public Guid DeliveryNotePosID 
    {
        get { return _DeliveryNotePosID; }
        set { SetProperty<Guid>(ref _DeliveryNotePosID, value); }
    }

    private DeliveryNotePos _DeliveryNotePos;
    public virtual DeliveryNotePos DeliveryNotePos
    { 
        get { return LazyLoader.Load(this, ref _DeliveryNotePos); } 
        set { SetProperty<DeliveryNotePos>(ref _DeliveryNotePos, value); }
    }

    public bool DeliveryNotePos_IsLoaded
    {
        get
        {
            return _DeliveryNotePos != null;
        }
    }

    public virtual ReferenceEntry DeliveryNotePosReference 
    {
        get { return Context.Entry(this).Reference("DeliveryNotePos"); }
    }
    
    private TandTv3MixPoint _TandTv3MixPoint;
    public virtual TandTv3MixPoint TandTv3MixPoint
    { 
        get { return LazyLoader.Load(this, ref _TandTv3MixPoint); } 
        set { SetProperty<TandTv3MixPoint>(ref _TandTv3MixPoint, value); }
    }

    public bool TandTv3MixPoint_IsLoaded
    {
        get
        {
            return _TandTv3MixPoint != null;
        }
    }

    public virtual ReferenceEntry TandTv3MixPointReference 
    {
        get { return Context.Entry(this).Reference("TandTv3MixPoint"); }
    }
    }
