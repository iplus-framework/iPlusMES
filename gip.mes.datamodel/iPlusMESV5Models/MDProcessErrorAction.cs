using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDProcessErrorAction : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MDProcessErrorAction()
    {
    }

    private MDProcessErrorAction(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDProcessErrorActionID;
    public Guid MDProcessErrorActionID 
    {
        get { return _MDProcessErrorActionID; }
        set { SetProperty<Guid>(ref _MDProcessErrorActionID, value); }
    }

    short _MDProcessErrorActionIndex;
    public short MDProcessErrorActionIndex 
    {
        get { return _MDProcessErrorActionIndex; }
        set { SetProperty<short>(ref _MDProcessErrorActionIndex, value); }
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

    private ICollection<MDGMPAdditive> _MDGMPAdditive_MDProcessErrorAction;
    public virtual ICollection<MDGMPAdditive> MDGMPAdditive_MDProcessErrorAction
    {
        get => LazyLoader.Load(this, ref _MDGMPAdditive_MDProcessErrorAction);
        set => _MDGMPAdditive_MDProcessErrorAction = value;
    }

    public bool MDGMPAdditive_MDProcessErrorAction_IsLoaded
    {
        get
        {
            return MDGMPAdditive_MDProcessErrorAction != null;
        }
    }

    public virtual CollectionEntry MDGMPAdditive_MDProcessErrorActionReference
    {
        get { return Context.Entry(this).Collection(c => c.MDGMPAdditive_MDProcessErrorAction); }
    }
}
