using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class RatingComplaint : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public RatingComplaint()
    {
    }

    private RatingComplaint(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _RatingComplaintID;
    public Guid RatingComplaintID 
    {
        get { return _RatingComplaintID; }
        set { SetProperty<Guid>(ref _RatingComplaintID, value); }
    }

    Guid _RatingID;
    public Guid RatingID 
    {
        get { return _RatingID; }
        set { SetProperty<Guid>(ref _RatingID, value); }
    }

    Guid _MDRatingComplaintTypeID;
    public Guid MDRatingComplaintTypeID 
    {
        get { return _MDRatingComplaintTypeID; }
        set { SetProperty<Guid>(ref _MDRatingComplaintTypeID, value); }
    }

    decimal _Score;
    public decimal Score 
    {
        get { return _Score; }
        set { SetProperty<decimal>(ref _Score, value); }
    }

    string _Comment;
    public string Comment 
    {
        get { return _Comment; }
        set { SetProperty<string>(ref _Comment, value); }
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

    private MDRatingComplaintType _MDRatingComplaintType;
    public virtual MDRatingComplaintType MDRatingComplaintType
    { 
        get { return LazyLoader.Load(this, ref _MDRatingComplaintType); } 
        set { SetProperty<MDRatingComplaintType>(ref _MDRatingComplaintType, value); }
    }

    public bool MDRatingComplaintType_IsLoaded
    {
        get
        {
            return MDRatingComplaintType != null;
        }
    }

    public virtual ReferenceEntry MDRatingComplaintTypeReference 
    {
        get { return Context.Entry(this).Reference("MDRatingComplaintType"); }
    }
    
    private Rating _Rating;
    public virtual Rating Rating
    { 
        get { return LazyLoader.Load(this, ref _Rating); } 
        set { SetProperty<Rating>(ref _Rating, value); }
    }

    public bool Rating_IsLoaded
    {
        get
        {
            return Rating != null;
        }
    }

    public virtual ReferenceEntry RatingReference 
    {
        get { return Context.Entry(this).Reference("Rating"); }
    }
    }
