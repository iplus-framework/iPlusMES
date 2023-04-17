using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDTimeRange : VBEntityObject , IInsertInfo, IUpdateInfo
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

    TimeSpan _TimeFrom;
    public TimeSpan TimeFrom 
    {
        get { return _TimeFrom; }
        set { SetProperty<TimeSpan>(ref _TimeFrom, value); }
    }

    TimeSpan _TimeTo;
    public TimeSpan TimeTo 
    {
        get { return _TimeTo; }
        set { SetProperty<TimeSpan>(ref _TimeTo, value); }
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
        get => LazyLoader.Load(this, ref _CalendarShift_MDTimeRange);
        set => _CalendarShift_MDTimeRange = value;
    }

    public bool CalendarShift_MDTimeRange_IsLoaded
    {
        get
        {
            return CalendarShift_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry CalendarShift_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.CalendarShift_MDTimeRange); }
    }

    private ICollection<CompanyPerson> _CompanyPerson_MDTimeRange;
    public virtual ICollection<CompanyPerson> CompanyPerson_MDTimeRange
    {
        get => LazyLoader.Load(this, ref _CompanyPerson_MDTimeRange);
        set => _CompanyPerson_MDTimeRange = value;
    }

    public bool CompanyPerson_MDTimeRange_IsLoaded
    {
        get
        {
            return CompanyPerson_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry CompanyPerson_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.CompanyPerson_MDTimeRange); }
    }

    private ICollection<InOrderPos> _InOrderPo_MDTimeRange;
    public virtual ICollection<InOrderPos> InOrderPo_MDTimeRange
    {
        get => LazyLoader.Load(this, ref _InOrderPo_MDTimeRange);
        set => _InOrderPo_MDTimeRange = value;
    }

    public bool InOrderPo_MDTimeRange_IsLoaded
    {
        get
        {
            return InOrderPo_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry InOrderPo_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrderPo_MDTimeRange); }
    }

    private ICollection<InOrder> _InOrder_MDTimeRange;
    public virtual ICollection<InOrder> InOrder_MDTimeRange
    {
        get => LazyLoader.Load(this, ref _InOrder_MDTimeRange);
        set => _InOrder_MDTimeRange = value;
    }

    public bool InOrder_MDTimeRange_IsLoaded
    {
        get
        {
            return InOrder_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry InOrder_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.InOrder_MDTimeRange); }
    }

    private ICollection<InRequestPos> _InRequestPo_MDTimeRange;
    public virtual ICollection<InRequestPos> InRequestPo_MDTimeRange
    {
        get => LazyLoader.Load(this, ref _InRequestPo_MDTimeRange);
        set => _InRequestPo_MDTimeRange = value;
    }

    public bool InRequestPo_MDTimeRange_IsLoaded
    {
        get
        {
            return InRequestPo_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry InRequestPo_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestPo_MDTimeRange); }
    }

    private ICollection<InRequest> _InRequest_MDTimeRange;
    public virtual ICollection<InRequest> InRequest_MDTimeRange
    {
        get => LazyLoader.Load(this, ref _InRequest_MDTimeRange);
        set => _InRequest_MDTimeRange = value;
    }

    public bool InRequest_MDTimeRange_IsLoaded
    {
        get
        {
            return InRequest_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry InRequest_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequest_MDTimeRange); }
    }

    private ICollection<MDTimeRange> _MDTimeRange_ParentMDTimeRange;
    public virtual ICollection<MDTimeRange> MDTimeRange_ParentMDTimeRange
    {
        get => LazyLoader.Load(this, ref _MDTimeRange_ParentMDTimeRange);
        set => _MDTimeRange_ParentMDTimeRange = value;
    }

    public bool MDTimeRange_ParentMDTimeRange_IsLoaded
    {
        get
        {
            return MDTimeRange_ParentMDTimeRange != null;
        }
    }

    public virtual CollectionEntry MDTimeRange_ParentMDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.MDTimeRange_ParentMDTimeRange); }
    }

    private ICollection<OutOfferPos> _OutOfferPo_MDTimeRange;
    public virtual ICollection<OutOfferPos> OutOfferPo_MDTimeRange
    {
        get => LazyLoader.Load(this, ref _OutOfferPo_MDTimeRange);
        set => _OutOfferPo_MDTimeRange = value;
    }

    public bool OutOfferPo_MDTimeRange_IsLoaded
    {
        get
        {
            return OutOfferPo_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry OutOfferPo_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferPo_MDTimeRange); }
    }

    private ICollection<OutOffer> _OutOffer_MDTimeRange;
    public virtual ICollection<OutOffer> OutOffer_MDTimeRange
    {
        get => LazyLoader.Load(this, ref _OutOffer_MDTimeRange);
        set => _OutOffer_MDTimeRange = value;
    }

    public bool OutOffer_MDTimeRange_IsLoaded
    {
        get
        {
            return OutOffer_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry OutOffer_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOffer_MDTimeRange); }
    }

    private ICollection<OutOrderPos> _OutOrderPo_MDTimeRange;
    public virtual ICollection<OutOrderPos> OutOrderPo_MDTimeRange
    {
        get => LazyLoader.Load(this, ref _OutOrderPo_MDTimeRange);
        set => _OutOrderPo_MDTimeRange = value;
    }

    public bool OutOrderPo_MDTimeRange_IsLoaded
    {
        get
        {
            return OutOrderPo_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry OutOrderPo_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPo_MDTimeRange); }
    }

    private ICollection<OutOrder> _OutOrder_MDTimeRange;
    public virtual ICollection<OutOrder> OutOrder_MDTimeRange
    {
        get => LazyLoader.Load(this, ref _OutOrder_MDTimeRange);
        set => _OutOrder_MDTimeRange = value;
    }

    public bool OutOrder_MDTimeRange_IsLoaded
    {
        get
        {
            return OutOrder_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry OutOrder_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrder_MDTimeRange); }
    }

    private MDTimeRange _MDTimeRange1_ParentMDTimeRange;
    public virtual MDTimeRange MDTimeRange1_ParentMDTimeRange
    { 
        get => LazyLoader.Load(this, ref _MDTimeRange1_ParentMDTimeRange);
        set => _MDTimeRange1_ParentMDTimeRange = value;
    }

    public bool MDTimeRange1_ParentMDTimeRange_IsLoaded
    {
        get
        {
            return MDTimeRange1_ParentMDTimeRange != null;
        }
    }

    public virtual ReferenceEntry MDTimeRange1_ParentMDTimeRangeReference 
    {
        get { return Context.Entry(this).Reference("MDTimeRange1_ParentMDTimeRange"); }
    }
    
    private ICollection<TourplanPos> _TourplanPo_MDTimeRange;
    public virtual ICollection<TourplanPos> TourplanPo_MDTimeRange
    {
        get => LazyLoader.Load(this, ref _TourplanPo_MDTimeRange);
        set => _TourplanPo_MDTimeRange = value;
    }

    public bool TourplanPo_MDTimeRange_IsLoaded
    {
        get
        {
            return TourplanPo_MDTimeRange != null;
        }
    }

    public virtual CollectionEntry TourplanPo_MDTimeRangeReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanPo_MDTimeRange); }
    }
}
