// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
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
        get { return LazyLoader.Load(this, ref _InvoiceCompanyAddress); } 
        set { SetProperty<CompanyAddress>(ref _InvoiceCompanyAddress, value); }
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
        get { return LazyLoader.Load(this, ref _InvoiceCompanyPerson); } 
        set { SetProperty<CompanyPerson>(ref _InvoiceCompanyPerson, value); }
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
        get { return LazyLoader.Load(this, ref _TenantCompany); } 
        set { SetProperty<Company>(ref _TenantCompany, value); }
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
        get { return LazyLoader.Load(this, ref _VBUser); } 
        set { SetProperty<VBUser>(ref _VBUser, value); }
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
