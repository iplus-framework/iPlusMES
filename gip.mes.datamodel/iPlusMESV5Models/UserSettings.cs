using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class UserSettings : VBEntityObject 
{

    public UserSettings()
    {
    }

    private UserSettings(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _UserSettingsID;
    public Guid UserSettingsID 
    {
        get { return _UserSettingsID; }
        set { SetProperty<Guid>(ref _UserSettingsID, value); }
    }

    Guid _VBUserID;
    public Guid VBUserID 
    {
        get { return _VBUserID; }
        set { SetProperty<Guid>(ref _VBUserID, value); }
    }

    Guid _TenantCompanyID;
    public Guid TenantCompanyID 
    {
        get { return _TenantCompanyID; }
        set { SetProperty<Guid>(ref _TenantCompanyID, value); }
    }

    Guid? _InvoiceCompanyAddressID;
    public Guid? InvoiceCompanyAddressID 
    {
        get { return _InvoiceCompanyAddressID; }
        set { SetProperty<Guid?>(ref _InvoiceCompanyAddressID, value); }
    }

    Guid? _InvoiceCompanyPersonID;
    public Guid? InvoiceCompanyPersonID 
    {
        get { return _InvoiceCompanyPersonID; }
        set { SetProperty<Guid?>(ref _InvoiceCompanyPersonID, value); }
    }

    private CompanyAddress _InvoiceCompanyAddress;
    public virtual CompanyAddress InvoiceCompanyAddress
    { 
        get => LazyLoader.Load(this, ref _InvoiceCompanyAddress);
        set => _InvoiceCompanyAddress = value;
    }

    public bool InvoiceCompanyAddress_IsLoaded
    {
        get
        {
            return InvoiceCompanyAddress != null;
        }
    }

    public virtual ReferenceEntry InvoiceCompanyAddressReference 
    {
        get { return Context.Entry(this).Reference("InvoiceCompanyAddress"); }
    }
    
    private CompanyPerson _InvoiceCompanyPerson;
    public virtual CompanyPerson InvoiceCompanyPerson
    { 
        get => LazyLoader.Load(this, ref _InvoiceCompanyPerson);
        set => _InvoiceCompanyPerson = value;
    }

    public bool InvoiceCompanyPerson_IsLoaded
    {
        get
        {
            return InvoiceCompanyPerson != null;
        }
    }

    public virtual ReferenceEntry InvoiceCompanyPersonReference 
    {
        get { return Context.Entry(this).Reference("InvoiceCompanyPerson"); }
    }
    
    private Company _TenantCompany;
    public virtual Company TenantCompany
    { 
        get => LazyLoader.Load(this, ref _TenantCompany);
        set => _TenantCompany = value;
    }

    public bool TenantCompany_IsLoaded
    {
        get
        {
            return TenantCompany != null;
        }
    }

    public virtual ReferenceEntry TenantCompanyReference 
    {
        get { return Context.Entry(this).Reference("TenantCompany"); }
    }
    
    private VBUser _VBUser;
    public virtual VBUser VBUser
    { 
        get => LazyLoader.Load(this, ref _VBUser);
        set => _VBUser = value;
    }

    public bool VBUser_IsLoaded
    {
        get
        {
            return VBUser != null;
        }
    }

    public virtual ReferenceEntry VBUserReference 
    {
        get { return Context.Entry(this).Reference("VBUser"); }
    }
    }
