using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDVisitorVoucherState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDVisitorVoucherState()
    {
    }

    private MDVisitorVoucherState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDVisitorVoucherStateID;
    public Guid MDVisitorVoucherStateID 
    {
        get { return _MDVisitorVoucherStateID; }
        set { SetProperty<Guid>(ref _MDVisitorVoucherStateID, value); }
    }

    short _MDVisitorVoucherStateIndex;
    public short MDVisitorVoucherStateIndex 
    {
        get { return _MDVisitorVoucherStateIndex; }
        set { SetProperty<short>(ref _MDVisitorVoucherStateIndex, value); }
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

    private ICollection<VisitorVoucher> _VisitorVoucher_MDVisitorVoucherState;
    public virtual ICollection<VisitorVoucher> VisitorVoucher_MDVisitorVoucherState
    {
        get { return LazyLoader.Load(this, ref _VisitorVoucher_MDVisitorVoucherState); }
        set { SetProperty<ICollection<VisitorVoucher>>(ref _VisitorVoucher_MDVisitorVoucherState, value); }
    }

    public bool VisitorVoucher_MDVisitorVoucherState_IsLoaded
    {
        get
        {
            return _VisitorVoucher_MDVisitorVoucherState != null;
        }
    }

    public virtual CollectionEntry VisitorVoucher_MDVisitorVoucherStateReference
    {
        get { return Context.Entry(this).Collection(c => c.VisitorVoucher_MDVisitorVoucherState); }
    }
}
