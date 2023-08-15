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
        set { SetProperty<Guid?>(ref _CompanyID, value); }
    }

    Guid? _CompanyPersonID;
    public Guid? CompanyPersonID 
    {
        get { return _CompanyPersonID; }
        set { SetProperty<Guid?>(ref _CompanyPersonID, value); }
    }

    Guid? _DeliveryNoteID;
    public Guid? DeliveryNoteID 
    {
        get { return _DeliveryNoteID; }
        set { SetProperty<Guid?>(ref _DeliveryNoteID, value); }
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
        get => LazyLoader.Load(this, ref _Company);
        set => _Company = value;
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
    
    private CompanyPerson _CompanyPerson;
    public virtual CompanyPerson CompanyPerson
    { 
        get => LazyLoader.Load(this, ref _CompanyPerson);
        set => _CompanyPerson = value;
    }

    public bool CompanyPerson_IsLoaded
    {
        get
        {
            return CompanyPerson != null;
        }
    }

    public virtual ReferenceEntry CompanyPersonReference 
    {
        get { return Context.Entry(this).Reference("CompanyPerson"); }
    }
    
    private DeliveryNote _DeliveryNote;
    public virtual DeliveryNote DeliveryNote
    { 
        get => LazyLoader.Load(this, ref _DeliveryNote);
        set => _DeliveryNote = value;
    }

    public bool DeliveryNote_IsLoaded
    {
        get
        {
            return DeliveryNote != null;
        }
    }

    public virtual ReferenceEntry DeliveryNoteReference 
    {
        get { return Context.Entry(this).Reference("DeliveryNote"); }
    }
    
    private ICollection<RatingComplaint> _RatingComplaint_Rating;
    public virtual ICollection<RatingComplaint> RatingComplaint_Rating
    {
        get => LazyLoader.Load(this, ref _RatingComplaint_Rating);
        set => _RatingComplaint_Rating = value;
    }

    public bool RatingComplaint_Rating_IsLoaded
    {
        get
        {
            return RatingComplaint_Rating != null;
        }
    }

    public virtual CollectionEntry RatingComplaint_RatingReference
    {
        get { return Context.Entry(this).Collection(c => c.RatingComplaint_Rating); }
    }
}
