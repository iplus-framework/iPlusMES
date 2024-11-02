// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class LabOrderPos : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence
{

    public LabOrderPos()
    {
    }

    private LabOrderPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _LabOrderPosID;
    public Guid LabOrderPosID 
    {
        get { return _LabOrderPosID; }
        set { SetProperty<Guid>(ref _LabOrderPosID, value); }
    }

    Guid? _LabOrderID;
    public Guid? LabOrderID 
    {
        get { return _LabOrderID; }
        set { SetProperty<Guid?>(ref _LabOrderID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    double? _ReferenceValue;
    public double? ReferenceValue 
    {
        get { return _ReferenceValue; }
        set { SetProperty<double?>(ref _ReferenceValue, value); }
    }

    double? _ActualValue;
    public double? ActualValue 
    {
        get { return _ActualValue; }
        set { SetProperty<double?>(ref _ActualValue, value); }
    }

    double? _ValueMinMin;
    public double? ValueMinMin 
    {
        get { return _ValueMinMin; }
        set { SetProperty<double?>(ref _ValueMinMin, value); }
    }

    double? _ValueMin;
    public double? ValueMin 
    {
        get { return _ValueMin; }
        set { SetProperty<double?>(ref _ValueMin, value); }
    }

    double? _ValueMax;
    public double? ValueMax 
    {
        get { return _ValueMax; }
        set { SetProperty<double?>(ref _ValueMax, value); }
    }

    double? _ValueMaxMax;
    public double? ValueMaxMax 
    {
        get { return _ValueMaxMax; }
        set { SetProperty<double?>(ref _ValueMaxMax, value); }
    }

    Guid? _MDLabOrderPosStateID;
    public Guid? MDLabOrderPosStateID 
    {
        get { return _MDLabOrderPosStateID; }
        set { SetProperty<Guid?>(ref _MDLabOrderPosStateID, value); }
    }

    Guid _MDLabTagID;
    public Guid MDLabTagID 
    {
        get { return _MDLabTagID; }
        set { SetProperty<Guid>(ref _MDLabTagID, value); }
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

    DateTime _UpdateDate;
    public DateTime UpdateDate 
    {
        get { return _UpdateDate; }
        set { SetProperty<DateTime>(ref _UpdateDate, value); }
    }

    string _UpdateName;
    public string UpdateName 
    {
        get { return _UpdateName; }
        set { SetProperty<string>(ref _UpdateName, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    string _LineNumber;
    public string LineNumber 
    {
        get { return _LineNumber; }
        set { SetProperty<string>(ref _LineNumber, value); }
    }

    private LabOrder _LabOrder;
    public virtual LabOrder LabOrder
    { 
        get { return LazyLoader.Load(this, ref _LabOrder); } 
        set { SetProperty<LabOrder>(ref _LabOrder, value); }
    }

    public bool LabOrder_IsLoaded
    {
        get
        {
            return LabOrder != null;
        }
    }

    public virtual ReferenceEntry LabOrderReference 
    {
        get { return Context.Entry(this).Reference("LabOrder"); }
    }
    
    private MDLabOrderPosState _MDLabOrderPosState;
    public virtual MDLabOrderPosState MDLabOrderPosState
    { 
        get { return LazyLoader.Load(this, ref _MDLabOrderPosState); } 
        set { SetProperty<MDLabOrderPosState>(ref _MDLabOrderPosState, value); }
    }

    public bool MDLabOrderPosState_IsLoaded
    {
        get
        {
            return MDLabOrderPosState != null;
        }
    }

    public virtual ReferenceEntry MDLabOrderPosStateReference 
    {
        get { return Context.Entry(this).Reference("MDLabOrderPosState"); }
    }
    
    private MDLabTag _MDLabTag;
    public virtual MDLabTag MDLabTag
    { 
        get { return LazyLoader.Load(this, ref _MDLabTag); } 
        set { SetProperty<MDLabTag>(ref _MDLabTag, value); }
    }

    public bool MDLabTag_IsLoaded
    {
        get
        {
            return MDLabTag != null;
        }
    }

    public virtual ReferenceEntry MDLabTagReference 
    {
        get { return Context.Entry(this).Reference("MDLabTag"); }
    }
    
    private ICollection<Weighing> _Weighing_LabOrderPos;
    public virtual ICollection<Weighing> Weighing_LabOrderPos
    {
        get { return LazyLoader.Load(this, ref _Weighing_LabOrderPos); }
        set { _Weighing_LabOrderPos = value; }
    }

    public bool Weighing_LabOrderPos_IsLoaded
    {
        get
        {
            return Weighing_LabOrderPos != null;
        }
    }

    public virtual CollectionEntry Weighing_LabOrderPosReference
    {
        get { return Context.Entry(this).Collection(c => c.Weighing_LabOrderPos); }
    }
}
