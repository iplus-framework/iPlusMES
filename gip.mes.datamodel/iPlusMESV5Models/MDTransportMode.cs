using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDTransportMode : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MDTransportMode()
    {
    }

    private MDTransportMode(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDTransportModeID;
    public Guid MDTransportModeID 
    {
        get { return _MDTransportModeID; }
        set { SetProperty<Guid>(ref _MDTransportModeID, value); }
    }

    short _MDTransportModeIndex;
    public short MDTransportModeIndex 
    {
        get { return _MDTransportModeIndex; }
        set { SetProperty<short>(ref _MDTransportModeIndex, value); }
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

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
    }

    private ICollection<InOrderPos> _InOrderPo_MDTransportMode;
    public virtual ICollection<InOrderPos> InOrderPo_MDTransportMode
    {
        get => LazyLoader.Load(this, ref _InOrderPo_MDTransportMode);
        set => _InOrderPo_MDTransportMode = value;
    }

    public bool InOrderPo_MDTransportMode_IsLoaded
    {
        get
        {
            return InOrderPo_MDTransportMode != null;
        }
    }

    public virtual CollectionEntry InOrderPo_MDTransportModeReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPo_MDTransportMode); }
    }

    private ICollection<OutOrderPos> _OutOrderPo_MDTransportMode;
    public virtual ICollection<OutOrderPos> OutOrderPo_MDTransportMode
    {
        get => LazyLoader.Load(this, ref _OutOrderPo_MDTransportMode);
        set => _OutOrderPo_MDTransportMode = value;
    }

    public bool OutOrderPo_MDTransportMode_IsLoaded
    {
        get
        {
            return OutOrderPo_MDTransportMode != null;
        }
    }

    public virtual CollectionEntry OutOrderPo_MDTransportModeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPo_MDTransportMode); }
    }
}
