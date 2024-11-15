using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class TourplanPos : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public TourplanPos()
    {
    }

    private TourplanPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _TourplanPosID;
    public Guid TourplanPosID 
    {
        get { return _TourplanPosID; }
        set { SetProperty<Guid>(ref _TourplanPosID, value); }
    }

    Guid _TourplanID;
    public Guid TourplanID 
    {
        get { return _TourplanID; }
        set { SetProperty<Guid>(ref _TourplanID, value); }
    }

    short _SortIndex;
    public short SortIndex 
    {
        get { return _SortIndex; }
        set { SetProperty<short>(ref _SortIndex, value); }
    }

    Guid _MDTourplanPosStateID;
    public Guid MDTourplanPosStateID 
    {
        get { return _MDTourplanPosStateID; }
        set { SetProperty<Guid>(ref _MDTourplanPosStateID, value); }
    }

    Guid _CompanyID;
    public Guid CompanyID 
    {
        get { return _CompanyID; }
        set { SetProperty<Guid>(ref _CompanyID, value); }
    }

    Guid _CompanyAddressID;
    public Guid CompanyAddressID 
    {
        get { return _CompanyAddressID; }
        set { SetProperty<Guid>(ref _CompanyAddressID, value); }
    }

    Guid? _CompanyAddressUnloadingpointID;
    public Guid? CompanyAddressUnloadingpointID 
    {
        get { return _CompanyAddressUnloadingpointID; }
        set { SetProperty<Guid?>(ref _CompanyAddressUnloadingpointID, value); }
    }

    Guid? _MDTimeRangeID;
    public Guid? MDTimeRangeID 
    {
        get { return _MDTimeRangeID; }
        set { SetProperty<Guid?>(ref _MDTimeRangeID, value); }
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
    
    private CompanyAddress _CompanyAddress;
    public virtual CompanyAddress CompanyAddress
    { 
        get { return LazyLoader.Load(this, ref _CompanyAddress); } 
        set { SetProperty<CompanyAddress>(ref _CompanyAddress, value); }
    }

    public bool CompanyAddress_IsLoaded
    {
        get
        {
            return _CompanyAddress != null;
        }
    }

    public virtual ReferenceEntry CompanyAddressReference 
    {
        get { return Context.Entry(this).Reference("CompanyAddress"); }
    }
    
    private CompanyAddressUnloadingpoint _CompanyAddressUnloadingpoint;
    public virtual CompanyAddressUnloadingpoint CompanyAddressUnloadingpoint
    { 
        get { return LazyLoader.Load(this, ref _CompanyAddressUnloadingpoint); } 
        set { SetProperty<CompanyAddressUnloadingpoint>(ref _CompanyAddressUnloadingpoint, value); }
    }

    public bool CompanyAddressUnloadingpoint_IsLoaded
    {
        get
        {
            return _CompanyAddressUnloadingpoint != null;
        }
    }

    public virtual ReferenceEntry CompanyAddressUnloadingpointReference 
    {
        get { return Context.Entry(this).Reference("CompanyAddressUnloadingpoint"); }
    }
    
    private ICollection<DeliveryNote> _DeliveryNote_TourplanPos;
    public virtual ICollection<DeliveryNote> DeliveryNote_TourplanPos
    {
        get { return LazyLoader.Load(this, ref _DeliveryNote_TourplanPos); }
        set { _DeliveryNote_TourplanPos = value; }
    }

    public bool DeliveryNote_TourplanPos_IsLoaded
    {
        get
        {
            return _DeliveryNote_TourplanPos != null;
        }
    }

    public virtual CollectionEntry DeliveryNote_TourplanPosReference
    {
        get { return Context.Entry(this).Collection(c => c.DeliveryNote_TourplanPos); }
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
            return _MDTimeRange != null;
        }
    }

    public virtual ReferenceEntry MDTimeRangeReference 
    {
        get { return Context.Entry(this).Reference("MDTimeRange"); }
    }
    
    private MDTourplanPosState _MDTourplanPosState;
    public virtual MDTourplanPosState MDTourplanPosState
    { 
        get { return LazyLoader.Load(this, ref _MDTourplanPosState); } 
        set { SetProperty<MDTourplanPosState>(ref _MDTourplanPosState, value); }
    }

    public bool MDTourplanPosState_IsLoaded
    {
        get
        {
            return _MDTourplanPosState != null;
        }
    }

    public virtual ReferenceEntry MDTourplanPosStateReference 
    {
        get { return Context.Entry(this).Reference("MDTourplanPosState"); }
    }
    
    private Tourplan _Tourplan;
    public virtual Tourplan Tourplan
    { 
        get { return LazyLoader.Load(this, ref _Tourplan); } 
        set { SetProperty<Tourplan>(ref _Tourplan, value); }
    }

    public bool Tourplan_IsLoaded
    {
        get
        {
            return _Tourplan != null;
        }
    }

    public virtual ReferenceEntry TourplanReference 
    {
        get { return Context.Entry(this).Reference("Tourplan"); }
    }
    }
