using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaintOrderPos : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MaintOrderPos()
    {
    }

    private MaintOrderPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaintOrderPosID;
    public Guid MaintOrderPosID 
    {
        get { return _MaintOrderPosID; }
        set { SetProperty<Guid>(ref _MaintOrderPosID, value); }
    }

    Guid _MaintOrderID;
    public Guid MaintOrderID 
    {
        get { return _MaintOrderID; }
        set { SetProperty<Guid>(ref _MaintOrderID, value); }
    }

    Guid _ParentMaintOrderPosID;
    public Guid ParentMaintOrderPosID 
    {
        get { return _ParentMaintOrderPosID; }
        set { SetProperty<Guid>(ref _ParentMaintOrderPosID, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    double _Quantity;
    public double Quantity 
    {
        get { return _Quantity; }
        set { SetProperty<double>(ref _Quantity, value); }
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

    private MaintOrder _MaintOrder;
    public virtual MaintOrder MaintOrder
    { 
        get { return LazyLoader.Load(this, ref _MaintOrder); } 
        set { SetProperty<MaintOrder>(ref _MaintOrder, value); }
    }

    public bool MaintOrder_IsLoaded
    {
        get
        {
            return MaintOrder != null;
        }
    }

    public virtual ReferenceEntry MaintOrderReference 
    {
        get { return Context.Entry(this).Reference("MaintOrder"); }
    }
    
    private Material _Material;
    public virtual Material Material
    { 
        get { return LazyLoader.Load(this, ref _Material); } 
        set { SetProperty<Material>(ref _Material, value); }
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
    }
