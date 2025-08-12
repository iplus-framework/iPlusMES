using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDInvoiceState : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDInvoiceState()
    {
    }

    private MDInvoiceState(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDInvoiceStateID;
    public Guid MDInvoiceStateID 
    {
        get { return _MDInvoiceStateID; }
        set { SetProperty<Guid>(ref _MDInvoiceStateID, value); }
    }

    short _InvoiceStateIndex;
    public short InvoiceStateIndex 
    {
        get { return _InvoiceStateIndex; }
        set { SetProperty<short>(ref _InvoiceStateIndex, value); }
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

    private ICollection<Invoice> _Invoice_MDInvoiceState;
    public virtual ICollection<Invoice> Invoice_MDInvoiceState
    {
        get { return LazyLoader.Load(this, ref _Invoice_MDInvoiceState); }
        set { SetProperty<ICollection<Invoice>>(ref _Invoice_MDInvoiceState, value); }
    }

    public bool Invoice_MDInvoiceState_IsLoaded
    {
        get
        {
            return _Invoice_MDInvoiceState != null;
        }
    }

    public virtual CollectionEntry Invoice_MDInvoiceStateReference
    {
        get { return Context.Entry(this).Collection(c => c.Invoice_MDInvoiceState); }
    }
}
