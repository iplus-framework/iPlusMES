using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDVisitorCard : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MDVisitorCard()
    {
    }

    private MDVisitorCard(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDVisitorCardID;
    public Guid MDVisitorCardID 
    {
        get { return _MDVisitorCardID; }
        set { SetProperty<Guid>(ref _MDVisitorCardID, value); }
    }

    string _MDVisitorCardNo;
    public string MDVisitorCardNo 
    {
        get { return _MDVisitorCardNo; }
        set { SetProperty<string>(ref _MDVisitorCardNo, value); }
    }

    string _MDVisitorCardKey;
    public string MDVisitorCardKey 
    {
        get { return _MDVisitorCardKey; }
        set { SetProperty<string>(ref _MDVisitorCardKey, value); }
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

    Guid _MDVisitorCardStateID;
    public Guid MDVisitorCardStateID 
    {
        get { return _MDVisitorCardStateID; }
        set { SetForeignKeyProperty<Guid>(ref _MDVisitorCardStateID, value, "MDVisitorCardState", _MDVisitorCardState, MDVisitorCardState != null ? MDVisitorCardState.MDVisitorCardStateID : default(Guid)); }
    }

    private MDVisitorCardState _MDVisitorCardState;
    public virtual MDVisitorCardState MDVisitorCardState
    { 
        get { return LazyLoader.Load(this, ref _MDVisitorCardState); } 
        set { SetProperty<MDVisitorCardState>(ref _MDVisitorCardState, value); }
    }

    public bool MDVisitorCardState_IsLoaded
    {
        get
        {
            return _MDVisitorCardState != null;
        }
    }

    public virtual ReferenceEntry MDVisitorCardStateReference 
    {
        get { return Context.Entry(this).Reference("MDVisitorCardState"); }
    }
    
    private ICollection<VisitorVoucher> _VisitorVoucher_MDVisitorCard;
    public virtual ICollection<VisitorVoucher> VisitorVoucher_MDVisitorCard
    {
        get { return LazyLoader.Load(this, ref _VisitorVoucher_MDVisitorCard); }
        set { SetProperty<ICollection<VisitorVoucher>>(ref _VisitorVoucher_MDVisitorCard, value); }
    }

    public bool VisitorVoucher_MDVisitorCard_IsLoaded
    {
        get
        {
            return _VisitorVoucher_MDVisitorCard != null;
        }
    }

    public virtual CollectionEntry VisitorVoucher_MDVisitorCardReference
    {
        get { return Context.Entry(this).Collection(c => c.VisitorVoucher_MDVisitorCard); }
    }

    private ICollection<Visitor> _Visitor_MDVisitorCard;
    public virtual ICollection<Visitor> Visitor_MDVisitorCard
    {
        get { return LazyLoader.Load(this, ref _Visitor_MDVisitorCard); }
        set { SetProperty<ICollection<Visitor>>(ref _Visitor_MDVisitorCard, value); }
    }

    public bool Visitor_MDVisitorCard_IsLoaded
    {
        get
        {
            return _Visitor_MDVisitorCard != null;
        }
    }

    public virtual CollectionEntry Visitor_MDVisitorCardReference
    {
        get { return Context.Entry(this).Collection(c => c.Visitor_MDVisitorCard); }
    }
}
