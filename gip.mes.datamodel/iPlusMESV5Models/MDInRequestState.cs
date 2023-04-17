using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDInRequestState : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MDInRequestState()
    {
    }

    private MDInRequestState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDInRequestStateID;
    public Guid MDInRequestStateID 
    {
        get { return _MDInRequestStateID; }
        set { SetProperty<Guid>(ref _MDInRequestStateID, value); }
    }

    short _MDInRequestStateIndex;
    public short MDInRequestStateIndex 
    {
        get { return _MDInRequestStateIndex; }
        set { SetProperty<short>(ref _MDInRequestStateIndex, value); }
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

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
    }

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
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

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    private ICollection<InRequest> _InRequest_MDInRequestState;
    public virtual ICollection<InRequest> InRequest_MDInRequestState
    {
        get => LazyLoader.Load(this, ref _InRequest_MDInRequestState);
        set => _InRequest_MDInRequestState = value;
    }

    public bool InRequest_MDInRequestState_IsLoaded
    {
        get
        {
            return InRequest_MDInRequestState != null;
        }
    }

    public virtual CollectionEntry InRequest_MDInRequestStateReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequest_MDInRequestState); }
    }
}
