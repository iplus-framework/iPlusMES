using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDProdOrderState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDProdOrderState()
    {
    }

    private MDProdOrderState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDProdOrderStateID;
    public Guid MDProdOrderStateID 
    {
        get { return _MDProdOrderStateID; }
        set { SetProperty<Guid>(ref _MDProdOrderStateID, value); }
    }

    short _MDProdOrderStateIndex;
    public short MDProdOrderStateIndex 
    {
        get { return _MDProdOrderStateIndex; }
        set { SetProperty<short>(ref _MDProdOrderStateIndex, value); }
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

    private ICollection<ProdOrderBatch> _ProdOrderBatch_MDProdOrderState;
    public virtual ICollection<ProdOrderBatch> ProdOrderBatch_MDProdOrderState
    {
        get { return LazyLoader.Load(this, ref _ProdOrderBatch_MDProdOrderState); }
        set { SetProperty<ICollection<ProdOrderBatch>>(ref _ProdOrderBatch_MDProdOrderState, value); }
    }

    public bool ProdOrderBatch_MDProdOrderState_IsLoaded
    {
        get
        {
            return _ProdOrderBatch_MDProdOrderState != null;
        }
    }

    public virtual CollectionEntry ProdOrderBatch_MDProdOrderStateReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderBatch_MDProdOrderState); }
    }

    private ICollection<ProdOrderPartslist> _ProdOrderPartslist_MDProdOrderState;
    public virtual ICollection<ProdOrderPartslist> ProdOrderPartslist_MDProdOrderState
    {
        get { return LazyLoader.Load(this, ref _ProdOrderPartslist_MDProdOrderState); }
        set { SetProperty<ICollection<ProdOrderPartslist>>(ref _ProdOrderPartslist_MDProdOrderState, value); }
    }

    public bool ProdOrderPartslist_MDProdOrderState_IsLoaded
    {
        get
        {
            return _ProdOrderPartslist_MDProdOrderState != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslist_MDProdOrderStateReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslist_MDProdOrderState); }
    }

    private ICollection<ProdOrder> _ProdOrder_MDProdOrderState;
    public virtual ICollection<ProdOrder> ProdOrder_MDProdOrderState
    {
        get { return LazyLoader.Load(this, ref _ProdOrder_MDProdOrderState); }
        set { SetProperty<ICollection<ProdOrder>>(ref _ProdOrder_MDProdOrderState, value); }
    }

    public bool ProdOrder_MDProdOrderState_IsLoaded
    {
        get
        {
            return _ProdOrder_MDProdOrderState != null;
        }
    }

    public virtual CollectionEntry ProdOrder_MDProdOrderStateReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrder_MDProdOrderState); }
    }
}
