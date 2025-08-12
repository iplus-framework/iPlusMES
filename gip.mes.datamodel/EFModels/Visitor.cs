using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class Visitor : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public Visitor()
    {
    }

    private Visitor(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _VisitorID;
    public Guid VisitorID 
    {
        get { return _VisitorID; }
        set { SetProperty<Guid>(ref _VisitorID, value); }
    }

    Guid? _VisitedCompanyID;
    public Guid? VisitedCompanyID 
    {
        get { return _VisitedCompanyID; }
        set { SetForeignKeyProperty<Guid?>(ref _VisitedCompanyID, value, "VisitedCompany", _VisitedCompany, VisitedCompany != null ? VisitedCompany.CompanyID : default(Guid?)); }
    }

    string _VisitorNo;
    public string VisitorNo 
    {
        get { return _VisitorNo; }
        set { SetProperty<string>(ref _VisitorNo, value); }
    }

    Guid? _VisitorCompanyID;
    public Guid? VisitorCompanyID 
    {
        get { return _VisitorCompanyID; }
        set { SetForeignKeyProperty<Guid?>(ref _VisitorCompanyID, value, "VisitorCompany", _VisitorCompany, VisitorCompany != null ? VisitorCompany.CompanyID : default(Guid?)); }
    }

    Guid? _VisitorCompanyPersonID;
    public Guid? VisitorCompanyPersonID 
    {
        get { return _VisitorCompanyPersonID; }
        set { SetForeignKeyProperty<Guid?>(ref _VisitorCompanyPersonID, value, "VisitorCompanyPerson", _VisitorCompanyPerson, VisitorCompanyPerson != null ? VisitorCompanyPerson.CompanyPersonID : default(Guid?)); }
    }

    Guid? _MDVisitorCardID;
    public Guid? MDVisitorCardID 
    {
        get { return _MDVisitorCardID; }
        set { SetForeignKeyProperty<Guid?>(ref _MDVisitorCardID, value, "MDVisitorCard", _MDVisitorCard, MDVisitorCard != null ? MDVisitorCard.MDVisitorCardID : default(Guid?)); }
    }

    Guid? _VehicleFacilityID;
    public Guid? VehicleFacilityID 
    {
        get { return _VehicleFacilityID; }
        set { SetForeignKeyProperty<Guid?>(ref _VehicleFacilityID, value, "VehicleFacility", _VehicleFacility, VehicleFacility != null ? VehicleFacility.FacilityID : default(Guid?)); }
    }

    Guid? _TrailerFacilityID;
    public Guid? TrailerFacilityID 
    {
        get { return _TrailerFacilityID; }
        set { SetForeignKeyProperty<Guid?>(ref _TrailerFacilityID, value, "TrailerFacility", _TrailerFacility, TrailerFacility != null ? TrailerFacility.FacilityID : default(Guid?)); }
    }

    DateTime? _ScheduledFromDate;
    public DateTime? ScheduledFromDate 
    {
        get { return _ScheduledFromDate; }
        set { SetProperty<DateTime?>(ref _ScheduledFromDate, value); }
    }

    DateTime? _ScheduledToDate;
    public DateTime? ScheduledToDate 
    {
        get { return _ScheduledToDate; }
        set { SetProperty<DateTime?>(ref _ScheduledToDate, value); }
    }

    bool _IsFinished;
    public bool IsFinished 
    {
        get { return _IsFinished; }
        set { SetProperty<bool>(ref _IsFinished, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
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

    Guid? _MaterialID;
    public Guid? MaterialID 
    {
        get { return _MaterialID; }
        set { SetForeignKeyProperty<Guid?>(ref _MaterialID, value, "Material", _Material, Material != null ? Material.MaterialID : default(Guid?)); }
    }

    private MDVisitorCard _MDVisitorCard;
    public virtual MDVisitorCard MDVisitorCard
    { 
        get { return LazyLoader.Load(this, ref _MDVisitorCard); } 
        set { SetProperty<MDVisitorCard>(ref _MDVisitorCard, value); }
    }

    public bool MDVisitorCard_IsLoaded
    {
        get
        {
            return _MDVisitorCard != null;
        }
    }

    public virtual ReferenceEntry MDVisitorCardReference 
    {
        get { return Context.Entry(this).Reference("MDVisitorCard"); }
    }
    
    private Material _Material;
    public virtual Material Material
    { 
        get { return LazyLoader.Load(this, ref _Material); } 
        set { SetProperty<Material>(ref _Material, value); }
    }

    public bool Material_IsLoaded
    {
        get
        {
            return _Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private Facility _TrailerFacility;
    public virtual Facility TrailerFacility
    { 
        get { return LazyLoader.Load(this, ref _TrailerFacility); } 
        set { SetProperty<Facility>(ref _TrailerFacility, value); }
    }

    public bool TrailerFacility_IsLoaded
    {
        get
        {
            return _TrailerFacility != null;
        }
    }

    public virtual ReferenceEntry TrailerFacilityReference 
    {
        get { return Context.Entry(this).Reference("TrailerFacility"); }
    }
    
    private Facility _VehicleFacility;
    public virtual Facility VehicleFacility
    { 
        get { return LazyLoader.Load(this, ref _VehicleFacility); } 
        set { SetProperty<Facility>(ref _VehicleFacility, value); }
    }

    public bool VehicleFacility_IsLoaded
    {
        get
        {
            return _VehicleFacility != null;
        }
    }

    public virtual ReferenceEntry VehicleFacilityReference 
    {
        get { return Context.Entry(this).Reference("VehicleFacility"); }
    }
    
    private Company _VisitedCompany;
    public virtual Company VisitedCompany
    { 
        get { return LazyLoader.Load(this, ref _VisitedCompany); } 
        set { SetProperty<Company>(ref _VisitedCompany, value); }
    }

    public bool VisitedCompany_IsLoaded
    {
        get
        {
            return _VisitedCompany != null;
        }
    }

    public virtual ReferenceEntry VisitedCompanyReference 
    {
        get { return Context.Entry(this).Reference("VisitedCompany"); }
    }
    
    private Company _VisitorCompany;
    public virtual Company VisitorCompany
    { 
        get { return LazyLoader.Load(this, ref _VisitorCompany); } 
        set { SetProperty<Company>(ref _VisitorCompany, value); }
    }

    public bool VisitorCompany_IsLoaded
    {
        get
        {
            return _VisitorCompany != null;
        }
    }

    public virtual ReferenceEntry VisitorCompanyReference 
    {
        get { return Context.Entry(this).Reference("VisitorCompany"); }
    }
    
    private CompanyPerson _VisitorCompanyPerson;
    public virtual CompanyPerson VisitorCompanyPerson
    { 
        get { return LazyLoader.Load(this, ref _VisitorCompanyPerson); } 
        set { SetProperty<CompanyPerson>(ref _VisitorCompanyPerson, value); }
    }

    public bool VisitorCompanyPerson_IsLoaded
    {
        get
        {
            return _VisitorCompanyPerson != null;
        }
    }

    public virtual ReferenceEntry VisitorCompanyPersonReference 
    {
        get { return Context.Entry(this).Reference("VisitorCompanyPerson"); }
    }
    
    private ICollection<VisitorVoucher> _VisitorVoucher_Visitor;
    public virtual ICollection<VisitorVoucher> VisitorVoucher_Visitor
    {
        get { return LazyLoader.Load(this, ref _VisitorVoucher_Visitor); }
        set { SetProperty<ICollection<VisitorVoucher>>(ref _VisitorVoucher_Visitor, value); }
    }

    public bool VisitorVoucher_Visitor_IsLoaded
    {
        get
        {
            return _VisitorVoucher_Visitor != null;
        }
    }

    public virtual CollectionEntry VisitorVoucher_VisitorReference
    {
        get { return Context.Entry(this).Collection(c => c.VisitorVoucher_Visitor); }
    }
}
