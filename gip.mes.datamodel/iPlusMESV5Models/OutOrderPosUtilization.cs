using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class OutOrderPosUtilization : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public OutOrderPosUtilization()
    {
    }

    private OutOrderPosUtilization(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _OutOrderPosUtilizationID;
    public Guid OutOrderPosUtilizationID 
    {
        get { return _OutOrderPosUtilizationID; }
        set { SetProperty<Guid>(ref _OutOrderPosUtilizationID, value); }
    }

    Guid _OutOrderPosID;
    public Guid OutOrderPosID 
    {
        get { return _OutOrderPosID; }
        set { SetProperty<Guid>(ref _OutOrderPosID, value); }
    }

    string _OutOrderPosUtilizationNo;
    public string OutOrderPosUtilizationNo 
    {
        get { return _OutOrderPosUtilizationNo; }
        set { SetProperty<string>(ref _OutOrderPosUtilizationNo, value); }
    }

    DateTime _TimeFrom;
    public DateTime TimeFrom 
    {
        get { return _TimeFrom; }
        set { SetProperty<DateTime>(ref _TimeFrom, value); }
    }

    DateTime _TimeTo;
    public DateTime TimeTo 
    {
        get { return _TimeTo; }
        set { SetProperty<DateTime>(ref _TimeTo, value); }
    }

    Guid? _MaterialID;
    public Guid? MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid?>(ref _MaterialID, value); }
    }

    double _ActualQuantity;
    public double ActualQuantity 
    {
        get { return _ActualQuantity; }
        set { SetProperty<double>(ref _ActualQuantity, value); }
    }

    double _ActualWeight;
    public double ActualWeight 
    {
        get { return _ActualWeight; }
        set { SetProperty<double>(ref _ActualWeight, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
    }

    bool _IsToBill;
    public bool IsToBill 
    {
        get { return _IsToBill; }
        set { SetProperty<bool>(ref _IsToBill, value); }
    }

    bool _IsBilled;
    public bool IsBilled 
    {
        get { return _IsBilled; }
        set { SetProperty<bool>(ref _IsBilled, value); }
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

    private Material _Material;
    public virtual Material Material
    { 
        get => LazyLoader.Load(this, ref _Material);
        set => _Material = value;
    }

    public bool Material_IsLoaded
    {
        get
        {
            return Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
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
    }
