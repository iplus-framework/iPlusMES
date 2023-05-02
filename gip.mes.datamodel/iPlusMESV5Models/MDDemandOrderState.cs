using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDDemandOrderState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDDemandOrderState()
    {
    }

    private MDDemandOrderState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDDemandOrderStateID;
    public Guid MDDemandOrderStateID 
    {
        get { return _MDDemandOrderStateID; }
        set { SetProperty<Guid>(ref _MDDemandOrderStateID, value); }
    }

    short _MDDemandOrderStateIndex;
    public short MDDemandOrderStateIndex 
    {
        get { return _MDDemandOrderStateIndex; }
        set { SetProperty<short>(ref _MDDemandOrderStateIndex, value); }
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

    private ICollection<DemandOrder> _DemandOrder_MDDemandOrderState;
    public virtual ICollection<DemandOrder> DemandOrder_MDDemandOrderState
    {
        get => LazyLoader.Load(this, ref _DemandOrder_MDDemandOrderState);
        set => _DemandOrder_MDDemandOrderState = value;
    }

    public bool DemandOrder_MDDemandOrderState_IsLoaded
    {
        get
        {
            return DemandOrder_MDDemandOrderState != null;
        }
    }

    public virtual CollectionEntry DemandOrder_MDDemandOrderStateReference
    {
        get { return Context.Entry(this).Collection(c => c.DemandOrder_MDDemandOrderState); }
    }
}
