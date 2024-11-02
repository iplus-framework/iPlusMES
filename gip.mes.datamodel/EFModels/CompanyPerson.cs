// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class CompanyPerson : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public CompanyPerson()
    {
    }

    private CompanyPerson(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _CompanyPersonID;
    public Guid CompanyPersonID 
    {
        get { return _CompanyPersonID; }
        set { SetProperty<Guid>(ref _CompanyPersonID, value); }
    }

    Guid _CompanyID;
    public Guid CompanyID 
    {
        get { return _CompanyID; }
        set { SetProperty<Guid>(ref _CompanyID, value); }
    }

    string _Name1;
    public string Name1 
    {
        get { return _Name1; }
        set { SetProperty<string>(ref _Name1, value); }
    }

    string _Name2;
    public string Name2 
    {
        get { return _Name2; }
        set { SetProperty<string>(ref _Name2, value); }
    }

    string _Name3;
    public string Name3 
    {
        get { return _Name3; }
        set { SetProperty<string>(ref _Name3, value); }
    }

    string _Street;
    public string Street 
    {
        get { return _Street; }
        set { SetProperty<string>(ref _Street, value); }
    }

    string _City;
    public string City 
    {
        get { return _City; }
        set { SetProperty<string>(ref _City, value); }
    }

    string _Postcode;
    public string Postcode 
    {
        get { return _Postcode; }
        set { SetProperty<string>(ref _Postcode, value); }
    }

    string _PostOfficeBox;
    public string PostOfficeBox 
    {
        get { return _PostOfficeBox; }
        set { SetProperty<string>(ref _PostOfficeBox, value); }
    }

    string _Phone;
    public string Phone 
    {
        get { return _Phone; }
        set { SetProperty<string>(ref _Phone, value); }
    }

    string _Fax;
    public string Fax 
    {
        get { return _Fax; }
        set { SetProperty<string>(ref _Fax, value); }
    }

    string _Mobile;
    public string Mobile 
    {
        get { return _Mobile; }
        set { SetProperty<string>(ref _Mobile, value); }
    }

    Guid? _MDCountryID;
    public Guid? MDCountryID 
    {
        get { return _MDCountryID; }
        set { SetProperty<Guid?>(ref _MDCountryID, value); }
    }

    Guid? _MDTimeRangeID;
    public Guid? MDTimeRangeID 
    {
        get { return _MDTimeRangeID; }
        set { SetProperty<Guid?>(ref _MDTimeRangeID, value); }
    }

    string _XMLConfig;
    public override string XMLConfig 
    {
        get { return _XMLConfig; }
        set { SetProperty<string>(ref _XMLConfig, value); }
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

    string _CompanyPersonNo;
    public string CompanyPersonNo 
    {
        get { return _CompanyPersonNo; }
        set { SetProperty<string>(ref _CompanyPersonNo, value); }
    }

    private ICollection<CalendarShiftPerson> _CalendarShiftPerson_CompanyPerson;
    public virtual ICollection<CalendarShiftPerson> CalendarShiftPerson_CompanyPerson
    {
        get { return LazyLoader.Load(this, ref _CalendarShiftPerson_CompanyPerson); }
        set { _CalendarShiftPerson_CompanyPerson = value; }
    }

    public bool CalendarShiftPerson_CompanyPerson_IsLoaded
    {
        get
        {
            return CalendarShiftPerson_CompanyPerson != null;
        }
    }

    public virtual CollectionEntry CalendarShiftPerson_CompanyPersonReference
    {
        get { return Context.Entry(this).Collection(c => c.CalendarShiftPerson_CompanyPerson); }
    }

    private Company _Company;
    public virtual Company Company
    { 
        get { return LazyLoader.Load(this, ref _Company); } 
        set { SetProperty<Company>(ref _Company, value); }
    }

    public bool Company_IsLoaded
    {
        get
        {
            return Company != null;
        }
    }

    public virtual ReferenceEntry CompanyReference 
    {
        get { return Context.Entry(this).Reference("Company"); }
    }
    
    private ICollection<CompanyPersonRole> _CompanyPersonRole_CompanyPerson;
    public virtual ICollection<CompanyPersonRole> CompanyPersonRole_CompanyPerson
    {
        get { return LazyLoader.Load(this, ref _CompanyPersonRole_CompanyPerson); }
        set { _CompanyPersonRole_CompanyPerson = value; }
    }

    public bool CompanyPersonRole_CompanyPerson_IsLoaded
    {
        get
        {
            return CompanyPersonRole_CompanyPerson != null;
        }
    }

    public virtual CollectionEntry CompanyPersonRole_CompanyPersonReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyPersonRole_CompanyPerson); }
    }

    private ICollection<Facility> _Facility_CompanyPerson;
    public virtual ICollection<Facility> Facility_CompanyPerson
    {
        get { return LazyLoader.Load(this, ref _Facility_CompanyPerson); }
        set { _Facility_CompanyPerson = value; }
    }

    public bool Facility_CompanyPerson_IsLoaded
    {
        get
        {
            return Facility_CompanyPerson != null;
        }
    }

    public virtual CollectionEntry Facility_CompanyPersonReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_CompanyPerson); }
    }

    private ICollection<InOrder> _InOrder_IssuerCompanyPerson;
    public virtual ICollection<InOrder> InOrder_IssuerCompanyPerson
    {
        get { return LazyLoader.Load(this, ref _InOrder_IssuerCompanyPerson); }
        set { _InOrder_IssuerCompanyPerson = value; }
    }

    public bool InOrder_IssuerCompanyPerson_IsLoaded
    {
        get
        {
            return InOrder_IssuerCompanyPerson != null;
        }
    }

    public virtual CollectionEntry InOrder_IssuerCompanyPersonReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_IssuerCompanyPerson); }
    }

    private ICollection<Invoice> _Invoice_IssuerCompanyPerson;
    public virtual ICollection<Invoice> Invoice_IssuerCompanyPerson
    {
        get { return LazyLoader.Load(this, ref _Invoice_IssuerCompanyPerson); }
        set { _Invoice_IssuerCompanyPerson = value; }
    }

    public bool Invoice_IssuerCompanyPerson_IsLoaded
    {
        get
        {
            return Invoice_IssuerCompanyPerson != null;
        }
    }

    public virtual CollectionEntry Invoice_IssuerCompanyPersonReference
    {
        get { return Context.Entry(this).Collection(c => c.Invoice_IssuerCompanyPerson); }
    }

    private MDCountry _MDCountry;
    public virtual MDCountry MDCountry
    { 
        get { return LazyLoader.Load(this, ref _MDCountry); } 
        set { SetProperty<MDCountry>(ref _MDCountry, value); }
    }

    public bool MDCountry_IsLoaded
    {
        get
        {
            return MDCountry != null;
        }
    }

    public virtual ReferenceEntry MDCountryReference 
    {
        get { return Context.Entry(this).Reference("MDCountry"); }
    }
    
    private MDTimeRange _MDTimeRange;
    public virtual MDTimeRange MDTimeRange
    { 
        get { return LazyLoader.Load(this, ref _MDTimeRange); } 
        set { SetProperty<MDTimeRange>(ref _MDTimeRange, value); }
    }

    public bool MDTimeRange_IsLoaded
    {
        get
        {
            return MDTimeRange != null;
        }
    }

    public virtual ReferenceEntry MDTimeRangeReference 
    {
        get { return Context.Entry(this).Reference("MDTimeRange"); }
    }
    
    private ICollection<OutOffer> _OutOffer_IssuerCompanyPerson;
    public virtual ICollection<OutOffer> OutOffer_IssuerCompanyPerson
    {
        get { return LazyLoader.Load(this, ref _OutOffer_IssuerCompanyPerson); }
        set { _OutOffer_IssuerCompanyPerson = value; }
    }

    public bool OutOffer_IssuerCompanyPerson_IsLoaded
    {
        get
        {
            return OutOffer_IssuerCompanyPerson != null;
        }
    }

    public virtual CollectionEntry OutOffer_IssuerCompanyPersonReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOffer_IssuerCompanyPerson); }
    }

    private ICollection<OutOrder> _OutOrder_IssuerCompanyPerson;
    public virtual ICollection<OutOrder> OutOrder_IssuerCompanyPerson
    {
        get { return LazyLoader.Load(this, ref _OutOrder_IssuerCompanyPerson); }
        set { _OutOrder_IssuerCompanyPerson = value; }
    }

    public bool OutOrder_IssuerCompanyPerson_IsLoaded
    {
        get
        {
            return OutOrder_IssuerCompanyPerson != null;
        }
    }

    public virtual CollectionEntry OutOrder_IssuerCompanyPersonReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_IssuerCompanyPerson); }
    }

    private ICollection<Rating> _Rating_CompanyPerson;
    public virtual ICollection<Rating> Rating_CompanyPerson
    {
        get { return LazyLoader.Load(this, ref _Rating_CompanyPerson); }
        set { _Rating_CompanyPerson = value; }
    }

    public bool Rating_CompanyPerson_IsLoaded
    {
        get
        {
            return Rating_CompanyPerson != null;
        }
    }

    public virtual CollectionEntry Rating_CompanyPersonReference
    {
        get { return Context.Entry(this).Collection(c => c.Rating_CompanyPerson); }
    }

    private ICollection<UserSettings> _UserSettings_InvoiceCompanyPerson;
    public virtual ICollection<UserSettings> UserSettings_InvoiceCompanyPerson
    {
        get { return LazyLoader.Load(this, ref _UserSettings_InvoiceCompanyPerson); }
        set { _UserSettings_InvoiceCompanyPerson = value; }
    }

    public bool UserSettings_InvoiceCompanyPerson_IsLoaded
    {
        get
        {
            return UserSettings_InvoiceCompanyPerson != null;
        }
    }

    public virtual CollectionEntry UserSettings_InvoiceCompanyPersonReference
    {
        get { return Context.Entry(this).Collection(c => c.UserSettings_InvoiceCompanyPerson); }
    }

    private ICollection<VisitorVoucher> _VisitorVoucher_VisitorCompanyPerson;
    public virtual ICollection<VisitorVoucher> VisitorVoucher_VisitorCompanyPerson
    {
        get { return LazyLoader.Load(this, ref _VisitorVoucher_VisitorCompanyPerson); }
        set { _VisitorVoucher_VisitorCompanyPerson = value; }
    }

    public bool VisitorVoucher_VisitorCompanyPerson_IsLoaded
    {
        get
        {
            return VisitorVoucher_VisitorCompanyPerson != null;
        }
    }

    public virtual CollectionEntry VisitorVoucher_VisitorCompanyPersonReference
    {
        get { return Context.Entry(this).Collection(c => c.VisitorVoucher_VisitorCompanyPerson); }
    }

    private ICollection<Visitor> _Visitor_VisitorCompanyPerson;
    public virtual ICollection<Visitor> Visitor_VisitorCompanyPerson
    {
        get { return LazyLoader.Load(this, ref _Visitor_VisitorCompanyPerson); }
        set { _Visitor_VisitorCompanyPerson = value; }
    }

    public bool Visitor_VisitorCompanyPerson_IsLoaded
    {
        get
        {
            return Visitor_VisitorCompanyPerson != null;
        }
    }

    public virtual CollectionEntry Visitor_VisitorCompanyPersonReference
    {
        get { return Context.Entry(this).Collection(c => c.Visitor_VisitorCompanyPerson); }
    }
}
