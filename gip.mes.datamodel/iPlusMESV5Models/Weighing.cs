﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class Weighing : VBEntityObject
{

    public Weighing()
    {
    }

    private Weighing(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _WeighingID;
    public Guid WeighingID 
    {
        get { return _WeighingID; }
        set { SetProperty<Guid>(ref _WeighingID, value); }
    }

    string _WeighingNo;
    public string WeighingNo 
    {
        get { return _WeighingNo; }
        set { SetProperty<string>(ref _WeighingNo, value); }
    }

    Guid? _VBiACClassID;
    public Guid? VBiACClassID 
    {
        get { return _VBiACClassID; }
        set { SetProperty<Guid?>(ref _VBiACClassID, value); }
    }

    string _WeighingTotalXML;
    public string WeighingTotalXML 
    {
        get { return _WeighingTotalXML; }
        set { SetProperty<string>(ref _WeighingTotalXML, value); }
    }

    DateTime _StartDate;
    public DateTime StartDate 
    {
        get { return _StartDate; }
        set { SetProperty<DateTime>(ref _StartDate, value); }
    }

    DateTime? _EndDate;
    public DateTime? EndDate 
    {
        get { return _EndDate; }
        set { SetProperty<DateTime?>(ref _EndDate, value); }
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

    double _Weight;
    public double Weight 
    {
        get { return _Weight; }
        set { SetProperty<double>(ref _Weight, value); }
    }

    string _IdentNr;
    public string IdentNr 
    {
        get { return _IdentNr; }
        set { SetProperty<string>(ref _IdentNr, value); }
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

    Guid? _PickingPosID;
    public Guid? PickingPosID 
    {
        get { return _PickingPosID; }
        set { SetProperty<Guid?>(ref _PickingPosID, value); }
    }

    Guid? _LabOrderPosID;
    public Guid? LabOrderPosID 
    {
        get { return _LabOrderPosID; }
        set { SetProperty<Guid?>(ref _LabOrderPosID, value); }
    }

    private InOrderPos _InOrderPos;
    public virtual InOrderPos InOrderPos
    { 
        get => LazyLoader.Load(this, ref _InOrderPos);
        set => _InOrderPos = value;
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
    
    private LabOrderPos _LabOrderPos;
    public virtual LabOrderPos LabOrderPos
    { 
        get => LazyLoader.Load(this, ref _LabOrderPos);
        set => _LabOrderPos = value;
    }

    public bool LabOrderPos_IsLoaded
    {
        get
        {
            return LabOrderPos != null;
        }
    }

    public virtual ReferenceEntry LabOrderPosReference 
    {
        get { return Context.Entry(this).Reference("LabOrderPos"); }
    }
    
    private OutOrderPos _OutOrderPos;
    public virtual OutOrderPos OutOrderPos
    { 
        get => LazyLoader.Load(this, ref _OutOrderPos);
        set => _OutOrderPos = value;
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
    
    private PickingPos _PickingPos;
    public virtual PickingPos PickingPos
    { 
        get => LazyLoader.Load(this, ref _PickingPos);
        set => _PickingPos = value;
    }

    public bool PickingPos_IsLoaded
    {
        get
        {
            return PickingPos != null;
        }
    }

    public virtual ReferenceEntry PickingPosReference 
    {
        get { return Context.Entry(this).Reference("PickingPos"); }
    }
    }