using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDToleranceState : VBEntityObject , IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDToleranceState()
    {
    }

    private MDToleranceState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDToleranceStateID;
    public Guid MDToleranceStateID 
    {
        get { return _MDToleranceStateID; }
        set { SetProperty<Guid>(ref _MDToleranceStateID, value); }
    }

    short _MDToleranceStateIndex;
    public short MDToleranceStateIndex 
    {
        get { return _MDToleranceStateIndex; }
        set { SetProperty<short>(ref _MDToleranceStateIndex, value); }
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

    private ICollection<OutOrderPos> _OutOrderPos_MDToleranceState;
    public virtual ICollection<OutOrderPos> OutOrderPos_MDToleranceState
    {
        get => LazyLoader.Load(this, ref _OutOrderPos_MDToleranceState);
        set => _OutOrderPos_MDToleranceState = value;
    }

    public bool OutOrderPos_MDToleranceState_IsLoaded
    {
        get
        {
            return OutOrderPos_MDToleranceState != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_MDToleranceStateReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_MDToleranceState); }
    }

    private ICollection<ProdOrderPartslistPos> _ProdOrderPartslistPos_MDToleranceState;
    public virtual ICollection<ProdOrderPartslistPos> ProdOrderPartslistPos_MDToleranceState
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPos_MDToleranceState);
        set => _ProdOrderPartslistPos_MDToleranceState = value;
    }

    public bool ProdOrderPartslistPos_MDToleranceState_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos_MDToleranceState != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPos_MDToleranceStateReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPos_MDToleranceState); }
    }

    private ICollection<ProdOrderPartslistPosRelation> _ProdOrderPartslistPosRelation_MDToleranceState;
    public virtual ICollection<ProdOrderPartslistPosRelation> ProdOrderPartslistPosRelation_MDToleranceState
    {
        get => LazyLoader.Load(this, ref _ProdOrderPartslistPosRelation_MDToleranceState);
        set => _ProdOrderPartslistPosRelation_MDToleranceState = value;
    }

    public bool ProdOrderPartslistPosRelation_MDToleranceState_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosRelation_MDToleranceState != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPosRelation_MDToleranceStateReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPosRelation_MDToleranceState); }
    }
}
