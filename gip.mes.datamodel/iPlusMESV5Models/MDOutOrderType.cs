using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDOutOrderType : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDOutOrderType()
    {
    }

    private MDOutOrderType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDOutOrderTypeID;
    public Guid MDOutOrderTypeID 
    {
        get { return _MDOutOrderTypeID; }
        set { SetProperty<Guid>(ref _MDOutOrderTypeID, value); }
    }

    short _OrderTypeIndex;
    public short OrderTypeIndex 
    {
        get { return _OrderTypeIndex; }
        set { SetProperty<short>(ref _OrderTypeIndex, value); }
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

    DateTime _UpdateDate;
    public DateTime UpdateDate 
    {
        get { return _UpdateDate; }
        set { SetProperty<DateTime>(ref _UpdateDate, value); }
    }

    string _UpdateName;
    public string UpdateName 
    {
        get { return _UpdateName; }
        set { SetProperty<string>(ref _UpdateName, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    private ICollection<OutOffer> _OutOffer_MDOutOrderType;
    public virtual ICollection<OutOffer> OutOffer_MDOutOrderType
    {
        get => LazyLoader.Load(this, ref _OutOffer_MDOutOrderType);
        set => _OutOffer_MDOutOrderType = value;
    }

    public bool OutOffer_MDOutOrderType_IsLoaded
    {
        get
        {
            return OutOffer_MDOutOrderType != null;
        }
    }

    public virtual CollectionEntry OutOffer_MDOutOrderTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOffer_MDOutOrderType); }
    }

    private ICollection<OutOrder> _OutOrder_MDOutOrderType;
    public virtual ICollection<OutOrder> OutOrder_MDOutOrderType
    {
        get => LazyLoader.Load(this, ref _OutOrder_MDOutOrderType);
        set => _OutOrder_MDOutOrderType = value;
    }

    public bool OutOrder_MDOutOrderType_IsLoaded
    {
        get
        {
            return OutOrder_MDOutOrderType != null;
        }
    }

    public virtual CollectionEntry OutOrder_MDOutOrderTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_MDOutOrderType); }
    }
}
