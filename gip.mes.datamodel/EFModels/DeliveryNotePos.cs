// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class DeliveryNotePos : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence
{

    public DeliveryNotePos()
    {
    }

    private DeliveryNotePos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _DeliveryNotePosID;
    public Guid DeliveryNotePosID 
    {
        get { return _DeliveryNotePosID; }
        set { SetProperty<Guid>(ref _DeliveryNotePosID, value); }
    }

    Guid _DeliveryNoteID;
    public Guid DeliveryNoteID 
    {
        get { return _DeliveryNoteID; }
        set { SetProperty<Guid>(ref _DeliveryNoteID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    Guid? _OutOrderPosID;
    public Guid? OutOrderPosID 
    {
        get { return _OutOrderPosID; }
        set { SetProperty<Guid?>(ref _OutOrderPosID, value); }
    }

    Guid? _InOrderPosID;
    public Guid? InOrderPosID 
    {
        get { return _InOrderPosID; }
        set { SetProperty<Guid?>(ref _InOrderPosID, value); }
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

    private DeliveryNote _DeliveryNote;
    public virtual DeliveryNote DeliveryNote
    { 
        get { return LazyLoader.Load(this, ref _DeliveryNote); } 
        set { SetProperty<DeliveryNote>(ref _DeliveryNote, value); }
    }

    public bool DeliveryNote_IsLoaded
    {
        get
        {
            return DeliveryNote != null;
        }
    }

    public virtual ReferenceEntry DeliveryNoteReference 
    {
        get { return Context.Entry(this).Reference("DeliveryNote"); }
    }
    
    private InOrderPos _InOrderPos;
    public virtual InOrderPos InOrderPos
    { 
        get { return LazyLoader.Load(this, ref _InOrderPos); } 
        set { SetProperty<InOrderPos>(ref _InOrderPos, value); }
    }

    public bool InOrderPos_IsLoaded
    {
        get
        {
            return InOrderPos != null;
        }
    }

    public virtual ReferenceEntry InOrderPosReference 
    {
        get { return Context.Entry(this).Reference("InOrderPos"); }
    }
    
    private ICollection<OrderLog> _OrderLog_DeliveryNotePos;
    public virtual ICollection<OrderLog> OrderLog_DeliveryNotePos
    {
        get { return LazyLoader.Load(this, ref _OrderLog_DeliveryNotePos); }
        set { _OrderLog_DeliveryNotePos = value; }
    }

    public bool OrderLog_DeliveryNotePos_IsLoaded
    {
        get
        {
            return OrderLog_DeliveryNotePos != null;
        }
    }

    public virtual CollectionEntry OrderLog_DeliveryNotePosReference
    {
        get { return Context.Entry(this).Collection(c => c.OrderLog_DeliveryNotePos); }
    }

    private OutOrderPos _OutOrderPos;
    public virtual OutOrderPos OutOrderPos
    { 
        get { return LazyLoader.Load(this, ref _OutOrderPos); } 
        set { SetProperty<OutOrderPos>(ref _OutOrderPos, value); }
    }

    public bool OutOrderPos_IsLoaded
    {
        get
        {
            return OutOrderPos != null;
        }
    }

    public virtual ReferenceEntry OutOrderPosReference 
    {
        get { return Context.Entry(this).Reference("OutOrderPos"); }
    }
    
    private ICollection<TandTv3MixPointDeliveryNotePos> _TandTv3MixPointDeliveryNotePos_DeliveryNotePos;
    public virtual ICollection<TandTv3MixPointDeliveryNotePos> TandTv3MixPointDeliveryNotePos_DeliveryNotePos
    {
        get { return LazyLoader.Load(this, ref _TandTv3MixPointDeliveryNotePos_DeliveryNotePos); }
        set { _TandTv3MixPointDeliveryNotePos_DeliveryNotePos = value; }
    }

    public bool TandTv3MixPointDeliveryNotePos_DeliveryNotePos_IsLoaded
    {
        get
        {
            return TandTv3MixPointDeliveryNotePos_DeliveryNotePos != null;
        }
    }

    public virtual CollectionEntry TandTv3MixPointDeliveryNotePos_DeliveryNotePosReference
    {
        get { return Context.Entry(this).Collection(c => c.TandTv3MixPointDeliveryNotePos_DeliveryNotePos); }
    }
}
