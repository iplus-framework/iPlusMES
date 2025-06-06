﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDOutOfferState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDOutOfferState()
    {
    }

    private MDOutOfferState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDOutOfferStateID;
    public Guid MDOutOfferStateID 
    {
        get { return _MDOutOfferStateID; }
        set { SetProperty<Guid>(ref _MDOutOfferStateID, value); }
    }

    short _MDOutOfferStateIndex;
    public short MDOutOfferStateIndex 
    {
        get { return _MDOutOfferStateIndex; }
        set { SetProperty<short>(ref _MDOutOfferStateIndex, value); }
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

    private ICollection<OutOffer> _OutOffer_MDOutOfferState;
    public virtual ICollection<OutOffer> OutOffer_MDOutOfferState
    {
        get { return LazyLoader.Load(this, ref _OutOffer_MDOutOfferState); }
        set { _OutOffer_MDOutOfferState = value; }
    }

    public bool OutOffer_MDOutOfferState_IsLoaded
    {
        get
        {
            return _OutOffer_MDOutOfferState != null;
        }
    }

    public virtual CollectionEntry OutOffer_MDOutOfferStateReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOffer_MDOutOfferState); }
    }
}
