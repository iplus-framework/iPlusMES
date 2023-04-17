using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDTourplanPosState : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MDTourplanPosState()
    {
    }

    private MDTourplanPosState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDTourplanPosStateID;
    public Guid MDTourplanPosStateID 
    {
        get { return _MDTourplanPosStateID; }
        set { SetProperty<Guid>(ref _MDTourplanPosStateID, value); }
    }

    short _MDTourplanPosStateIndex;
    public short MDTourplanPosStateIndex 
    {
        get { return _MDTourplanPosStateIndex; }
        set { SetProperty<short>(ref _MDTourplanPosStateIndex, value); }
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

    private ICollection<OutOrderPos> _OutOrderPo_MDTourplanPosState;
    public virtual ICollection<OutOrderPos> OutOrderPo_MDTourplanPosState
    {
        get => LazyLoader.Load(this, ref _OutOrderPo_MDTourplanPosState);
        set => _OutOrderPo_MDTourplanPosState = value;
    }

    public bool OutOrderPo_MDTourplanPosState_IsLoaded
    {
        get
        {
            return OutOrderPo_MDTourplanPosState != null;
        }
    }

    public virtual CollectionEntry OutOrderPo_MDTourplanPosStateReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPo_MDTourplanPosState); }
    }

    private ICollection<TourplanPos> _TourplanPo_MDTourplanPosState;
    public virtual ICollection<TourplanPos> TourplanPo_MDTourplanPosState
    {
        get => LazyLoader.Load(this, ref _TourplanPo_MDTourplanPosState);
        set => _TourplanPo_MDTourplanPosState = value;
    }

    public bool TourplanPo_MDTourplanPosState_IsLoaded
    {
        get
        {
            return TourplanPo_MDTourplanPosState != null;
        }
    }

    public virtual CollectionEntry TourplanPo_MDTourplanPosStateReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanPo_MDTourplanPosState); }
    }
}
