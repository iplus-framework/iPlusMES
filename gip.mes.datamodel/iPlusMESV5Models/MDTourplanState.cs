using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDTourplanState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDTourplanState()
    {
    }

    private MDTourplanState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDTourplanStateID;
    public Guid MDTourplanStateID 
    {
        get { return _MDTourplanStateID; }
        set { SetProperty<Guid>(ref _MDTourplanStateID, value); }
    }

    short _MDTourplanStateIndex;
    public short MDTourplanStateIndex 
    {
        get { return _MDTourplanStateIndex; }
        set { SetProperty<short>(ref _MDTourplanStateIndex, value); }
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

    private ICollection<Tourplan> _Tourplan_MDTourplanState;
    public virtual ICollection<Tourplan> Tourplan_MDTourplanState
    {
        get { return LazyLoader.Load(this, ref _Tourplan_MDTourplanState); }
        set { _Tourplan_MDTourplanState = value; }
    }

    public bool Tourplan_MDTourplanState_IsLoaded
    {
        get
        {
            return Tourplan_MDTourplanState != null;
        }
    }

    public virtual CollectionEntry Tourplan_MDTourplanStateReference
    {
        get { return Context.Entry(this).Collection(c => c.Tourplan_MDTourplanState); }
    }
}
