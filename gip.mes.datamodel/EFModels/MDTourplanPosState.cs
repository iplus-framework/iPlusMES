using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDTourplanPosState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
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

    private ICollection<OutOrderPos> _OutOrderPos_MDTourplanPosState;
    public virtual ICollection<OutOrderPos> OutOrderPos_MDTourplanPosState
    {
        get { return LazyLoader.Load(this, ref _OutOrderPos_MDTourplanPosState); }
        set { _OutOrderPos_MDTourplanPosState = value; }
    }

    public bool OutOrderPos_MDTourplanPosState_IsLoaded
    {
        get
        {
            return _OutOrderPos_MDTourplanPosState != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_MDTourplanPosStateReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_MDTourplanPosState); }
    }

    private ICollection<TourplanPos> _TourplanPos_MDTourplanPosState;
    public virtual ICollection<TourplanPos> TourplanPos_MDTourplanPosState
    {
        get { return LazyLoader.Load(this, ref _TourplanPos_MDTourplanPosState); }
        set { _TourplanPos_MDTourplanPosState = value; }
    }

    public bool TourplanPos_MDTourplanPosState_IsLoaded
    {
        get
        {
            return _TourplanPos_MDTourplanPosState != null;
        }
    }

    public virtual CollectionEntry TourplanPos_MDTourplanPosStateReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanPos_MDTourplanPosState); }
    }
}
