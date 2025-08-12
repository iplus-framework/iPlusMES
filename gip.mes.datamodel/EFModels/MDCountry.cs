using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDCountry : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDCountry()
    {
    }

    private MDCountry(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDCountryID;
    public Guid MDCountryID 
    {
        get { return _MDCountryID; }
        set { SetProperty<Guid>(ref _MDCountryID, value); }
    }

    string _CountryCode;
    public string CountryCode 
    {
        get { return _CountryCode; }
        set { SetProperty<string>(ref _CountryCode, value); }
    }

    bool _IsEUMember;
    public bool IsEUMember 
    {
        get { return _IsEUMember; }
        set { SetProperty<bool>(ref _IsEUMember, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    Guid _MDCurrencyID;
    public Guid MDCurrencyID 
    {
        get { return _MDCurrencyID; }
        set { SetForeignKeyProperty<Guid>(ref _MDCurrencyID, value, "MDCurrency", _MDCurrency, MDCurrency != null ? MDCurrency.MDCurrencyID : default(Guid)); }
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

    private ICollection<CalendarHoliday> _CalendarHoliday_MDCountry;
    public virtual ICollection<CalendarHoliday> CalendarHoliday_MDCountry
    {
        get { return LazyLoader.Load(this, ref _CalendarHoliday_MDCountry); }
        set { SetProperty<ICollection<CalendarHoliday>>(ref _CalendarHoliday_MDCountry, value); }
    }

    public bool CalendarHoliday_MDCountry_IsLoaded
    {
        get
        {
            return _CalendarHoliday_MDCountry != null;
        }
    }

    public virtual CollectionEntry CalendarHoliday_MDCountryReference
    {
        get { return Context.Entry(this).Collection(c => c.CalendarHoliday_MDCountry); }
    }

    private ICollection<CompanyAddress> _CompanyAddress_MDCountry;
    public virtual ICollection<CompanyAddress> CompanyAddress_MDCountry
    {
        get { return LazyLoader.Load(this, ref _CompanyAddress_MDCountry); }
        set { SetProperty<ICollection<CompanyAddress>>(ref _CompanyAddress_MDCountry, value); }
    }

    public bool CompanyAddress_MDCountry_IsLoaded
    {
        get
        {
            return _CompanyAddress_MDCountry != null;
        }
    }

    public virtual CollectionEntry CompanyAddress_MDCountryReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyAddress_MDCountry); }
    }

    private ICollection<CompanyPerson> _CompanyPerson_MDCountry;
    public virtual ICollection<CompanyPerson> CompanyPerson_MDCountry
    {
        get { return LazyLoader.Load(this, ref _CompanyPerson_MDCountry); }
        set { SetProperty<ICollection<CompanyPerson>>(ref _CompanyPerson_MDCountry, value); }
    }

    public bool CompanyPerson_MDCountry_IsLoaded
    {
        get
        {
            return _CompanyPerson_MDCountry != null;
        }
    }

    public virtual CollectionEntry CompanyPerson_MDCountryReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyPerson_MDCountry); }
    }

    private ICollection<MDCountryLand> _MDCountryLand_MDCountry;
    public virtual ICollection<MDCountryLand> MDCountryLand_MDCountry
    {
        get { return LazyLoader.Load(this, ref _MDCountryLand_MDCountry); }
        set { SetProperty<ICollection<MDCountryLand>>(ref _MDCountryLand_MDCountry, value); }
    }

    public bool MDCountryLand_MDCountry_IsLoaded
    {
        get
        {
            return _MDCountryLand_MDCountry != null;
        }
    }

    public virtual CollectionEntry MDCountryLand_MDCountryReference
    {
        get { return Context.Entry(this).Collection(c => c.MDCountryLand_MDCountry); }
    }

    private ICollection<MDCountrySalesTax> _MDCountrySalesTax_MDCountry;
    public virtual ICollection<MDCountrySalesTax> MDCountrySalesTax_MDCountry
    {
        get { return LazyLoader.Load(this, ref _MDCountrySalesTax_MDCountry); }
        set { SetProperty<ICollection<MDCountrySalesTax>>(ref _MDCountrySalesTax_MDCountry, value); }
    }

    public bool MDCountrySalesTax_MDCountry_IsLoaded
    {
        get
        {
            return _MDCountrySalesTax_MDCountry != null;
        }
    }

    public virtual CollectionEntry MDCountrySalesTax_MDCountryReference
    {
        get { return Context.Entry(this).Collection(c => c.MDCountrySalesTax_MDCountry); }
    }

    private MDCurrency _MDCurrency;
    public virtual MDCurrency MDCurrency
    { 
        get { return LazyLoader.Load(this, ref _MDCurrency); } 
        set { SetProperty<MDCurrency>(ref _MDCurrency, value); }
    }

    public bool MDCurrency_IsLoaded
    {
        get
        {
            return _MDCurrency != null;
        }
    }

    public virtual ReferenceEntry MDCurrencyReference 
    {
        get { return Context.Entry(this).Reference("MDCurrency"); }
    }
    }
