using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDProdOrderPartslistPosState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDProdOrderPartslistPosState()
    {
    }

    private MDProdOrderPartslistPosState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDProdOrderPartslistPosStateID;
    public Guid MDProdOrderPartslistPosStateID 
    {
        get { return _MDProdOrderPartslistPosStateID; }
        set { SetProperty<Guid>(ref _MDProdOrderPartslistPosStateID, value); }
    }

    short _MDProdOrderPartslistPosStateIndex;
    public short MDProdOrderPartslistPosStateIndex 
    {
        get { return _MDProdOrderPartslistPosStateIndex; }
        set { SetProperty<short>(ref _MDProdOrderPartslistPosStateIndex, value); }
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

    private ICollection<ProdOrderPartslistPos> _ProdOrderPartslistPos_MDProdOrderPartslistPosState;
    public virtual ICollection<ProdOrderPartslistPos> ProdOrderPartslistPos_MDProdOrderPartslistPosState
    {
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistPos_MDProdOrderPartslistPosState); }
        set { _ProdOrderPartslistPos_MDProdOrderPartslistPosState = value; }
    }

    public bool ProdOrderPartslistPos_MDProdOrderPartslistPosState_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPos_MDProdOrderPartslistPosState != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPos_MDProdOrderPartslistPosStateReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPos_MDProdOrderPartslistPosState); }
    }

    private ICollection<ProdOrderPartslistPosRelation> _ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState;
    public virtual ICollection<ProdOrderPartslistPosRelation> ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState
    {
        get { return LazyLoader.Load(this, ref _ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState); }
        set { _ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState = value; }
    }

    public bool ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState_IsLoaded
    {
        get
        {
            return ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState != null;
        }
    }

    public virtual CollectionEntry ProdOrderPartslistPosRelation_MDProdOrderPartslistPosStateReference
    {
        get { return Context.Entry(this).Collection(c => c.ProdOrderPartslistPosRelation_MDProdOrderPartslistPosState); }
    }
}
