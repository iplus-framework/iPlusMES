using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDTimeRange : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDTimeRange()
    {
    }

    private MDTimeRange(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDTimeRangeID;
    public Guid MDTimeRangeID 
    {
        get { return _MDTimeRangeID; }
        set { SetProperty<Guid>(ref _MDTimeRangeID, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    Guid? _ParentMDTimeRangeID;
    public Guid? ParentMDTimeRangeID 
    {
        get { return _ParentMDTimeRangeID; }
        set { SetProperty<Guid?>(ref _ParentMDTimeRangeID, value); }
    }

    TimeOnly _TimeFrom;
    public TimeOnly TimeFrom 
    {
        get { return _TimeFrom; }
        set { SetProperty<TimeOnly>(ref _TimeFrom, value); }
    }

    TimeOnly _TimeTo;
    public TimeOnly TimeTo 
    {
        get { return _TimeTo; }
        set { SetProperty<TimeOnly>(ref _TimeTo, value); }
    }

    bool _IsShiftModel;
    public bool IsShiftModel 
    {
        get { return _IsShiftModel; }
        set { SetProperty<bool>(ref _IsShiftModel, value); }
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

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
    }

    private ICollection<CalendarShift> _CalendarShift_MDTimeRange;
    public virtual ICollection<CalendarShift> CalendarShift_MDTimeRange
    {
        get { return LazyLoader.Load(this, ref _CalendarShift_MDTimeRange); }
        set { _CalendarShift_MDTimeRange = value; }
    }

    public bool CalendarShift_MDTimeRange_IsLoaded
    {
        get
        {
            return _CalendarShift_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry CalendarShift_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.CalendarShift_MDTimeRange); }
    }

    private ICollection<CompanyPerson> _CompanyPerson_MDTimeRange;
    public virtual ICollection<CompanyPerson> CompanyPerson_MDTimeRange
    {
        get { return LazyLoader.Load(this, ref _CompanyPerson_MDTimeRange); }
        set { _CompanyPerson_MDTimeRange = value; }
    }

    public bool CompanyPerson_MDTimeRange_IsLoaded
    {
        get
        {
            return _CompanyPerson_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry CompanyPerson_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyPerson_MDTimeRange); }
    }

    private ICollection<InOrderPos> _InOrderPos_MDTimeRange;
    public virtual ICollection<InOrderPos> InOrderPos_MDTimeRange
    {
        get { return LazyLoader.Load(this, ref _InOrderPos_MDTimeRange); }
        set { _InOrderPos_MDTimeRange = value; }
    }

    public bool InOrderPos_MDTimeRange_IsLoaded
    {
        get
        {
            return _InOrderPos_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry InOrderPos_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPos_MDTimeRange); }
    }

    private ICollection<InOrder> _InOrder_MDTimeRange;
    public virtual ICollection<InOrder> InOrder_MDTimeRange
    {
        get { return LazyLoader.Load(this, ref _InOrder_MDTimeRange); }
        set { _InOrder_MDTimeRange = value; }
    }

    public bool InOrder_MDTimeRange_IsLoaded
    {
        get
        {
            return _InOrder_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry InOrder_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_MDTimeRange); }
    }

    private ICollection<InRequestPos> _InRequestPos_MDTimeRange;
    public virtual ICollection<InRequestPos> InRequestPos_MDTimeRange
    {
        get { return LazyLoader.Load(this, ref _InRequestPos_MDTimeRange); }
        set { _InRequestPos_MDTimeRange = value; }
    }

    public bool InRequestPos_MDTimeRange_IsLoaded
    {
        get
        {
            return _InRequestPos_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry InRequestPos_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestPos_MDTimeRange); }
    }

    private ICollection<InRequest> _InRequest_MDTimeRange;
    public virtual ICollection<InRequest> InRequest_MDTimeRange
    {
        get { return LazyLoader.Load(this, ref _InRequest_MDTimeRange); }
        set { _InRequest_MDTimeRange = value; }
    }

    public bool InRequest_MDTimeRange_IsLoaded
    {
        get
        {
            return _InRequest_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry InRequest_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequest_MDTimeRange); }
    }

    private ICollection<MDTimeRange> _MDTimeRange_ParentMDTimeRange;
    public virtual ICollection<MDTimeRange> MDTimeRange_ParentMDTimeRange
    {
        get { return LazyLoader.Load(this, ref _MDTimeRange_ParentMDTimeRange); }
        set { _MDTimeRange_ParentMDTimeRange = value; }
    }

    public bool MDTimeRange_ParentMDTimeRange_IsLoaded
    {
        get
        {
            return _MDTimeRange_ParentMDTimeRange != null;
        }
    }

    public virtual CollectionEntry MDTimeRange_ParentMDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.MDTimeRange_ParentMDTimeRange); }
    }

    private ICollection<OutOfferPos> _OutOfferPos_MDTimeRange;
    public virtual ICollection<OutOfferPos> OutOfferPos_MDTimeRange
    {
        get { return LazyLoader.Load(this, ref _OutOfferPos_MDTimeRange); }
        set { _OutOfferPos_MDTimeRange = value; }
    }

    public bool OutOfferPos_MDTimeRange_IsLoaded
    {
        get
        {
            return _OutOfferPos_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry OutOfferPos_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPos_MDTimeRange); }
    }

    private ICollection<OutOffer> _OutOffer_MDTimeRange;
    public virtual ICollection<OutOffer> OutOffer_MDTimeRange
    {
        get { return LazyLoader.Load(this, ref _OutOffer_MDTimeRange); }
        set { _OutOffer_MDTimeRange = value; }
    }

    public bool OutOffer_MDTimeRange_IsLoaded
    {
        get
        {
            return _OutOffer_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry OutOffer_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOffer_MDTimeRange); }
    }

    private ICollection<OutOrderPos> _OutOrderPos_MDTimeRange;
    public virtual ICollection<OutOrderPos> OutOrderPos_MDTimeRange
    {
        get { return LazyLoader.Load(this, ref _OutOrderPos_MDTimeRange); }
        set { _OutOrderPos_MDTimeRange = value; }
    }

    public bool OutOrderPos_MDTimeRange_IsLoaded
    {
        get
        {
            return _OutOrderPos_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_MDTimeRange); }
    }

    private ICollection<OutOrder> _OutOrder_MDTimeRange;
    public virtual ICollection<OutOrder> OutOrder_MDTimeRange
    {
        get { return LazyLoader.Load(this, ref _OutOrder_MDTimeRange); }
        set { _OutOrder_MDTimeRange = value; }
    }

    public bool OutOrder_MDTimeRange_IsLoaded
    {
        get
        {
            return _OutOrder_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry OutOrder_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_MDTimeRange); }
    }

    private MDTimeRange _MDTimeRange1_ParentMDTimeRange;
    public virtual MDTimeRange MDTimeRange1_ParentMDTimeRange
    { 
        get { return LazyLoader.Load(this, ref _MDTimeRange1_ParentMDTimeRange); } 
        set { SetProperty<MDTimeRange>(ref _MDTimeRange1_ParentMDTimeRange, value); }
    }

    public bool MDTimeRange1_ParentMDTimeRange_IsLoaded
    {
        get
        {
            return _MDTimeRange1_ParentMDTimeRange != null;
        }
    }

    public virtual ReferenceEntry MDTimeRange1_ParentMDTimeRangeReference 
    {
        get { return Context.Entry(this).Reference("MDTimeRange1_ParentMDTimeRange"); }
    }
    
    private ICollection<TourplanPos> _TourplanPos_MDTimeRange;
    public virtual ICollection<TourplanPos> TourplanPos_MDTimeRange
    {
        get { return LazyLoader.Load(this, ref _TourplanPos_MDTimeRange); }
        set { _TourplanPos_MDTimeRange = value; }
    }

    public bool TourplanPos_MDTimeRange_IsLoaded
    {
        get
        {
            return _TourplanPos_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry TourplanPos_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanPos_MDTimeRange); }
    }
}
