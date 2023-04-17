using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDInOrderType : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MDInOrderType()
    {
    }

    private MDInOrderType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDInOrderTypeID;
    public Guid MDInOrderTypeID 
    {
        get { return _MDInOrderTypeID; }
        set { SetProperty<Guid>(ref _MDInOrderTypeID, value); }
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

    private ICollection<InOrder> _InOrder_MDInOrderType;
    public virtual ICollection<InOrder> InOrder_MDInOrderType
    {
        get => LazyLoader.Load(this, ref _InOrder_MDInOrderType);
        set => _InOrder_MDInOrderType = value;
    }

    public bool InOrder_MDInOrderType_IsLoaded
    {
        get
        {
            return InOrder_MDInOrderType != null;
        }
    }

    public virtual CollectionEntry InOrder_MDInOrderTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_MDInOrderType); }
    }

    private ICollection<InRequest> _InRequest_MDInOrderType;
    public virtual ICollection<InRequest> InRequest_MDInOrderType
    {
        get => LazyLoader.Load(this, ref _InRequest_MDInOrderType);
        set => _InRequest_MDInOrderType = value;
    }

    public bool InRequest_MDInOrderType_IsLoaded
    {
        get
        {
            return InRequest_MDInOrderType != null;
        }
    }

    public virtual CollectionEntry InRequest_MDInOrderTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequest_MDInOrderType); }
    }
}
