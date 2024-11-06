using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PartslistPosRelation : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence, ITargetQuantity
{

    public PartslistPosRelation()
    {
    }

    private PartslistPosRelation(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PartslistPosRelationID;
    public Guid PartslistPosRelationID 
    {
        get { return _PartslistPosRelationID; }
        set { SetProperty<Guid>(ref _PartslistPosRelationID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    Guid _TargetPartslistPosID;
    public Guid TargetPartslistPosID 
    {
        get { return _TargetPartslistPosID; }
        set { SetProperty<Guid>(ref _TargetPartslistPosID, value); }
    }

    Guid _SourcePartslistPosID;
    public Guid SourcePartslistPosID 
    {
        get { return _SourcePartslistPosID; }
        set { SetProperty<Guid>(ref _SourcePartslistPosID, value); }
    }

    double _TargetQuantity;
    public double TargetQuantity 
    {
        get { return _TargetQuantity; }
        set { SetProperty<double>(ref _TargetQuantity, value); }
    }

    double _TargetQuantityUOM;
    public double TargetQuantityUOM 
    {
        get { return _TargetQuantityUOM; }
        set { SetProperty<double>(ref _TargetQuantityUOM, value); }
    }

    Guid? _MaterialWFRelationID;
    public Guid? MaterialWFRelationID 
    {
        get { return _MaterialWFRelationID; }
        set { SetProperty<Guid?>(ref _MaterialWFRelationID, value); }
    }

    bool? _RetrogradeFIFO;
    public bool? RetrogradeFIFO 
    {
        get { return _RetrogradeFIFO; }
        set { SetProperty<bool?>(ref _RetrogradeFIFO, value); }
    }

    bool? _Anterograde;
    public bool? Anterograde 
    {
        get { return _Anterograde; }
        set { SetProperty<bool?>(ref _Anterograde, value); }
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

    private MaterialWFRelation _MaterialWFRelation;
    public virtual MaterialWFRelation MaterialWFRelation
    { 
        get { return LazyLoader.Load(this, ref _MaterialWFRelation); } 
        set { SetProperty<MaterialWFRelation>(ref _MaterialWFRelation, value); }
    }

    public bool MaterialWFRelation_IsLoaded
    {
        get
        {
            return MaterialWFRelation != null;
        }
    }

    public virtual ReferenceEntry MaterialWFRelationReference 
    {
        get { return Context.Entry(this).Reference("MaterialWFRelation"); }
    }
    
    private PartslistPos _SourcePartslistPos;
    public virtual PartslistPos SourcePartslistPos
    { 
        get { return LazyLoader.Load(this, ref _SourcePartslistPos); } 
        set { SetProperty<PartslistPos>(ref _SourcePartslistPos, value); }
    }

    public bool SourcePartslistPos_IsLoaded
    {
        get
        {
            return SourcePartslistPos != null;
        }
    }

    public virtual ReferenceEntry SourcePartslistPosReference 
    {
        get { return Context.Entry(this).Reference("SourcePartslistPos"); }
    }
    
    private PartslistPos _TargetPartslistPos;
    public virtual PartslistPos TargetPartslistPos
    { 
        get { return LazyLoader.Load(this, ref _TargetPartslistPos); } 
        set { SetProperty<PartslistPos>(ref _TargetPartslistPos, value); }
    }

    public bool TargetPartslistPos_IsLoaded
    {
        get
        {
            return TargetPartslistPos != null;
        }
    }

    public virtual ReferenceEntry TargetPartslistPosReference 
    {
        get { return Context.Entry(this).Reference("TargetPartslistPos"); }
    }
    }
