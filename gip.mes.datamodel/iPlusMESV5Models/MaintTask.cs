using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaintTask : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MaintTask()
    {
    }

    private MaintTask(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MaintTaskID;
    public Guid MaintTaskID 
    {
        get { return _MaintTaskID; }
        set { SetProperty<Guid>(ref _MaintTaskID, value); }
    }

    Guid _MaintOrderID;
    public Guid MaintOrderID 
    {
        get { return _MaintOrderID; }
        set { SetProperty<Guid>(ref _MaintOrderID, value); }
    }

    Guid _MaintACClassVBGroupID;
    public Guid MaintACClassVBGroupID 
    {
        get { return _MaintACClassVBGroupID; }
        set { SetProperty<Guid>(ref _MaintACClassVBGroupID, value); }
    }

    Guid _MDMaintTaskStateID;
    public Guid MDMaintTaskStateID 
    {
        get { return _MDMaintTaskStateID; }
        set { SetProperty<Guid>(ref _MDMaintTaskStateID, value); }
    }

    bool _IsRepair;
    public bool IsRepair 
    {
        get { return _IsRepair; }
        set { SetProperty<bool>(ref _IsRepair, value); }
    }

    DateTime? _StartTaskDate;
    public DateTime? StartTaskDate 
    {
        get { return _StartTaskDate; }
        set { SetProperty<DateTime?>(ref _StartTaskDate, value); }
    }

    DateTime? _EndTaskDate;
    public DateTime? EndTaskDate 
    {
        get { return _EndTaskDate; }
        set { SetProperty<DateTime?>(ref _EndTaskDate, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
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

    private MDMaintTaskState _MDMaintTaskState;
    public virtual MDMaintTaskState MDMaintTaskState
    { 
        get => LazyLoader.Load(this, ref _MDMaintTaskState);
        set => _MDMaintTaskState = value;
    }

    public bool MDMaintTaskState_IsLoaded
    {
        get
        {
            return MDMaintTaskState != null;
        }
    }

    public virtual ReferenceEntry MDMaintTaskStateReference 
    {
        get { return Context.Entry(this).Reference("MDMaintTaskState"); }
    }
    
    private MaintACClassVBGroup _MaintACClassVBGroup;
    public virtual MaintACClassVBGroup MaintACClassVBGroup
    { 
        get => LazyLoader.Load(this, ref _MaintACClassVBGroup);
        set => _MaintACClassVBGroup = value;
    }

    public bool MaintACClassVBGroup_IsLoaded
    {
        get
        {
            return MaintACClassVBGroup != null;
        }
    }

    public virtual ReferenceEntry MaintACClassVBGroupReference 
    {
        get { return Context.Entry(this).Reference("MaintACClassVBGroup"); }
    }
    
    private MaintOrder _MaintOrder;
    public virtual MaintOrder MaintOrder
    { 
        get => LazyLoader.Load(this, ref _MaintOrder);
        set => _MaintOrder = value;
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
    }
