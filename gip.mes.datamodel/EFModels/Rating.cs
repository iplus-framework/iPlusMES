using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class Rating : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public Rating()
    {
    }

    private Rating(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _RatingID;
    public Guid RatingID 
    {
        get { return _RatingID; }
        set { SetProperty<Guid>(ref _RatingID, value); }
    }

    Guid? _CompanyID;
    public Guid? CompanyID 
    {
        get { return _CompanyID; }
        set { SetForeignKeyProperty<Guid?>(ref _CompanyID, value, "Company", _Company, Company != null ? Company.CompanyID : default(Guid?)); }
    }

    Guid? _CompanyPersonID;
    public Guid? CompanyPersonID 
    {
        get { return _CompanyPersonID; }
        set { SetForeignKeyProperty<Guid?>(ref _CompanyPersonID, value, "CompanyPerson", _CompanyPerson, CompanyPerson != null ? CompanyPerson.CompanyPersonID : default(Guid?)); }
    }

    Guid? _DeliveryNoteID;
    public Guid? DeliveryNoteID 
    {
        get { return _DeliveryNoteID; }
        set { SetForeignKeyProperty<Guid?>(ref _DeliveryNoteID, value, "DeliveryNote", _DeliveryNote, DeliveryNote != null ? DeliveryNote.DeliveryNoteID : default(Guid?)); }
    }

    decimal _Score;
    public decimal Score 
    {
        get { return _Score; }
        set { SetProperty<decimal>(ref _Score, value); }
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
            return _Company != null;
        }
    }

    public virtual ReferenceEntry CompanyReference 
    {
        get { return Context.Entry(this).Reference("Company"); }
    }
    
    private CompanyPerson _CompanyPerson;
    public virtual CompanyPerson CompanyPerson
    { 
        get { return LazyLoader.Load(this, ref _CompanyPerson); } 
        set { SetProperty<CompanyPerson>(ref _CompanyPerson, value); }
    }

    public bool CompanyPerson_IsLoaded
    {
        get
        {
            return _CompanyPerson != null;
        }
    }

    public virtual ReferenceEntry CompanyPersonReference 
    {
        get { return Context.Entry(this).Reference("CompanyPerson"); }
    }
    
    private DeliveryNote _DeliveryNote;
    public virtual DeliveryNote DeliveryNote
    { 
        get { return LazyLoader.Load(this, ref _DeliveryNote); } 
        set { SetProperty<DeliveryNote>(ref _DeliveryNote, value); }
    }

    public bool DeliveryNote_IsLoaded
    {
        get
        {
            return _DeliveryNote != null;
        }
    }

    public virtual ReferenceEntry DeliveryNoteReference 
    {
        get { return Context.Entry(this).Reference("DeliveryNote"); }
    }
    
    private ICollection<RatingComplaint> _RatingComplaint_Rating;
    public virtual ICollection<RatingComplaint> RatingComplaint_Rating
    {
        get { return LazyLoader.Load(this, ref _RatingComplaint_Rating); }
        set { SetProperty<ICollection<RatingComplaint>>(ref _RatingComplaint_Rating, value); }
    }

    public bool RatingComplaint_Rating_IsLoaded
    {
        get
        {
            return _RatingComplaint_Rating != null;
        }
    }

    public virtual CollectionEntry RatingComplaint_RatingReference
    {
        get { return Context.Entry(this).Collection(c => c.RatingComplaint_Rating); }
    }
}
