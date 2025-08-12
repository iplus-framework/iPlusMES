using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PartslistPosSplit : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence, ITargetQuantity
{

    public PartslistPosSplit()
    {
    }

    private PartslistPosSplit(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PartslistPosSplitID;
    public Guid PartslistPosSplitID 
    {
        get { return _PartslistPosSplitID; }
        set { SetProperty<Guid>(ref _PartslistPosSplitID, value); }
    }

    Guid _PartslistPosID;
    public Guid PartslistPosID 
    {
        get { return _PartslistPosID; }
        set { SetForeignKeyProperty<Guid>(ref _PartslistPosID, value, "PartslistPos", _PartslistPos, PartslistPos != null ? PartslistPos.PartslistPosID : default(Guid)); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    double _TargetWeight;
    public double TargetWeight 
    {
        get { return _TargetWeight; }
        set { SetProperty<double>(ref _TargetWeight, value); }
    }

    double _TargetQuantity;
    public double TargetQuantity 
    {
        get { return _TargetQuantity; }
        set { SetProperty<double>(ref _TargetQuantity, value); }
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

    private PartslistPos _PartslistPos;
    public virtual PartslistPos PartslistPos
    { 
        get { return LazyLoader.Load(this, ref _PartslistPos); } 
        set { SetProperty<PartslistPos>(ref _PartslistPos, value); }
    }

    public bool PartslistPos_IsLoaded
    {
        get
        {
            return _PartslistPos != null;
        }
    }

    public virtual ReferenceEntry PartslistPosReference 
    {
        get { return Context.Entry(this).Reference("PartslistPos"); }
    }
    }
