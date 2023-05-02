using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDMaintTaskState : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MDMaintTaskState()
    {
    }

    private MDMaintTaskState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDMaintTaskStateID;
    public Guid MDMaintTaskStateID 
    {
        get { return _MDMaintTaskStateID; }
        set { SetProperty<Guid>(ref _MDMaintTaskStateID, value); }
    }

    short _MDMaintTaskStateIndex;
    public short MDMaintTaskStateIndex 
    {
        get { return _MDMaintTaskStateIndex; }
        set { SetProperty<short>(ref _MDMaintTaskStateIndex, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    short _SortIndex;
    public short SortIndex 
    {
        get { return _SortIndex; }
        set { SetProperty<short>(ref _SortIndex, value); }
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

    private ICollection<MaintTask> _MaintTask_MDMaintTaskState;
    public virtual ICollection<MaintTask> MaintTask_MDMaintTaskState
    {
        get => LazyLoader.Load(this, ref _MaintTask_MDMaintTaskState);
        set => _MaintTask_MDMaintTaskState = value;
    }

    public bool MaintTask_MDMaintTaskState_IsLoaded
    {
        get
        {
            return MaintTask_MDMaintTaskState != null;
        }
    }

    public virtual CollectionEntry MaintTask_MDMaintTaskStateReference
    {
        get { return Context.Entry(this).Collection(c => c.MaintTask_MDMaintTaskState); }
    }
}
