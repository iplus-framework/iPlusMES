using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDInvoiceType : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDInvoiceType()
    {
    }

    private MDInvoiceType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDInvoiceTypeID;
    public Guid MDInvoiceTypeID 
    {
        get { return _MDInvoiceTypeID; }
        set { SetProperty<Guid>(ref _MDInvoiceTypeID, value); }
    }

    short _InvoiceTypeIndex;
    public short InvoiceTypeIndex 
    {
        get { return _InvoiceTypeIndex; }
        set { SetProperty<short>(ref _InvoiceTypeIndex, value); }
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

    DateTime _UpdateDate;
    public DateTime UpdateDate 
    {
        get { return _UpdateDate; }
        set { SetProperty<DateTime>(ref _UpdateDate, value); }
    }

    string _UpdateName;
    public string UpdateName 
    {
        get { return _UpdateName; }
        set { SetProperty<string>(ref _UpdateName, value); }
    }

    DateTime _InsertDate;
    public DateTime InsertDate 
    {
        get { return _InsertDate; }
        set { SetProperty<DateTime>(ref _InsertDate, value); }
    }

    string _InsertName;
    public string InsertName 
    {
        get { return _InsertName; }
        set { SetProperty<string>(ref _InsertName, value); }
    }

    string _MDKey;
    public string MDKey 
    {
        get { return _MDKey; }
        set { SetProperty<string>(ref _MDKey, value); }
    }

    private ICollection<Invoice> _Invoice_MDInvoiceType;
    public virtual ICollection<Invoice> Invoice_MDInvoiceType
    {
        get => LazyLoader.Load(this, ref _Invoice_MDInvoiceType);
        set => _Invoice_MDInvoiceType = value;
    }

    public bool Invoice_MDInvoiceType_IsLoaded
    {
        get
        {
            return Invoice_MDInvoiceType != null;
        }
    }

    public virtual CollectionEntry Invoice_MDInvoiceTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.Invoice_MDInvoiceType); }
    }
}
