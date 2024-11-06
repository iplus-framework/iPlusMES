using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDTermOfPayment : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDTermOfPayment()
    {
    }

    private MDTermOfPayment(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDTermOfPaymentID;
    public Guid MDTermOfPaymentID 
    {
        get { return _MDTermOfPaymentID; }
        set { SetProperty<Guid>(ref _MDTermOfPaymentID, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    string _Description;
    public string Description 
    {
        get { return _Description; }
        set { SetProperty<string>(ref _Description, value); }
    }

    int _TermOfPaymentDays;
    public int TermOfPaymentDays 
    {
        get { return _TermOfPaymentDays; }
        set { SetProperty<int>(ref _TermOfPaymentDays, value); }
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

    private ICollection<Company> _Company_BillingMDTermOfPayment;
    public virtual ICollection<Company> Company_BillingMDTermOfPayment
    {
        get { return LazyLoader.Load(this, ref _Company_BillingMDTermOfPayment); }
        set { _Company_BillingMDTermOfPayment = value; }
    }

    public bool Company_BillingMDTermOfPayment_IsLoaded
    {
        get
        {
            return Company_BillingMDTermOfPayment != null;
        }
    }

    public virtual CollectionEntry Company_BillingMDTermOfPaymentReference
    {
        get { return Context.Entry(this).Collection(c => c.Company_BillingMDTermOfPayment); }
    }

    private ICollection<Company> _Company_ShippingMDTermOfPayment;
    public virtual ICollection<Company> Company_ShippingMDTermOfPayment
    {
        get { return LazyLoader.Load(this, ref _Company_ShippingMDTermOfPayment); }
        set { _Company_ShippingMDTermOfPayment = value; }
    }

    public bool Company_ShippingMDTermOfPayment_IsLoaded
    {
        get
        {
            return Company_ShippingMDTermOfPayment != null;
        }
    }

    public virtual CollectionEntry Company_ShippingMDTermOfPaymentReference
    {
        get { return Context.Entry(this).Collection(c => c.Company_ShippingMDTermOfPayment); }
    }

    private ICollection<InOrder> _InOrder_MDTermOfPayment;
    public virtual ICollection<InOrder> InOrder_MDTermOfPayment
    {
        get { return LazyLoader.Load(this, ref _InOrder_MDTermOfPayment); }
        set { _InOrder_MDTermOfPayment = value; }
    }

    public bool InOrder_MDTermOfPayment_IsLoaded
    {
        get
        {
            return InOrder_MDTermOfPayment != null;
        }
    }

    public virtual CollectionEntry InOrder_MDTermOfPaymentReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_MDTermOfPayment); }
    }

    private ICollection<InRequest> _InRequest_MDTermOfPayment;
    public virtual ICollection<InRequest> InRequest_MDTermOfPayment
    {
        get { return LazyLoader.Load(this, ref _InRequest_MDTermOfPayment); }
        set { _InRequest_MDTermOfPayment = value; }
    }

    public bool InRequest_MDTermOfPayment_IsLoaded
    {
        get
        {
            return InRequest_MDTermOfPayment != null;
        }
    }

    public virtual CollectionEntry InRequest_MDTermOfPaymentReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequest_MDTermOfPayment); }
    }

    private ICollection<Invoice> _Invoice_MDTermOfPayment;
    public virtual ICollection<Invoice> Invoice_MDTermOfPayment
    {
        get { return LazyLoader.Load(this, ref _Invoice_MDTermOfPayment); }
        set { _Invoice_MDTermOfPayment = value; }
    }

    public bool Invoice_MDTermOfPayment_IsLoaded
    {
        get
        {
            return Invoice_MDTermOfPayment != null;
        }
    }

    public virtual CollectionEntry Invoice_MDTermOfPaymentReference
    {
        get { return Context.Entry(this).Collection(c => c.Invoice_MDTermOfPayment); }
    }

    private ICollection<OutOffer> _OutOffer_MDTermOfPayment;
    public virtual ICollection<OutOffer> OutOffer_MDTermOfPayment
    {
        get { return LazyLoader.Load(this, ref _OutOffer_MDTermOfPayment); }
        set { _OutOffer_MDTermOfPayment = value; }
    }

    public bool OutOffer_MDTermOfPayment_IsLoaded
    {
        get
        {
            return OutOffer_MDTermOfPayment != null;
        }
    }

    public virtual CollectionEntry OutOffer_MDTermOfPaymentReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOffer_MDTermOfPayment); }
    }

    private ICollection<OutOrder> _OutOrder_MDTermOfPayment;
    public virtual ICollection<OutOrder> OutOrder_MDTermOfPayment
    {
        get { return LazyLoader.Load(this, ref _OutOrder_MDTermOfPayment); }
        set { _OutOrder_MDTermOfPayment = value; }
    }

    public bool OutOrder_MDTermOfPayment_IsLoaded
    {
        get
        {
            return OutOrder_MDTermOfPayment != null;
        }
    }

    public virtual CollectionEntry OutOrder_MDTermOfPaymentReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_MDTermOfPayment); }
    }
}
