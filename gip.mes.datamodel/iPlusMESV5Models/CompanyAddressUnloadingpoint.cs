using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class CompanyAddressUnloadingpoint : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence
{

    public CompanyAddressUnloadingpoint()
    {
    }

    private CompanyAddressUnloadingpoint(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _CompanyAddressUnloadingpointID;
    public Guid CompanyAddressUnloadingpointID 
    {
        get { return _CompanyAddressUnloadingpointID; }
        set { SetProperty<Guid>(ref _CompanyAddressUnloadingpointID, value); }
    }

    Guid _CompanyAddressID;
    public Guid CompanyAddressID 
    {
        get { return _CompanyAddressID; }
        set { SetProperty<Guid>(ref _CompanyAddressID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    string _UnloadingPointName;
    public string UnloadingPointName 
    {
        get { return _UnloadingPointName; }
        set { SetProperty<string>(ref _UnloadingPointName, value); }
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

    private CompanyAddress _CompanyAddress;
    public virtual CompanyAddress CompanyAddress
    { 
        get => LazyLoader.Load(this, ref _CompanyAddress);
        set => _CompanyAddress = value;
    }

    public bool CompanyAddress_IsLoaded
    {
        get
        {
            return CompanyAddress != null;
        }
    }

    public virtual ReferenceEntry CompanyAddressReference 
    {
        get { return Context.Entry(this).Reference("CompanyAddress"); }
    }
    
    private ICollection<OutOrderPos> _OutOrderPos_CompanyAddressUnloadingpoint;
    public virtual ICollection<OutOrderPos> OutOrderPos_CompanyAddressUnloadingpoint
    {
        get => LazyLoader.Load(this, ref _OutOrderPos_CompanyAddressUnloadingpoint);
        set => _OutOrderPos_CompanyAddressUnloadingpoint = value;
    }

    public bool OutOrderPos_CompanyAddressUnloadingpoint_IsLoaded
    {
        get
        {
            return OutOrderPos_CompanyAddressUnloadingpoint != null;
        }
    }

    public virtual CollectionEntry OutOrderPos_CompanyAddressUnloadingpointReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOrderPos_CompanyAddressUnloadingpoint); }
    }

    private ICollection<TourplanPos> _TourplanPos_CompanyAddressUnloadingpoint;
    public virtual ICollection<TourplanPos> TourplanPos_CompanyAddressUnloadingpoint
    {
        get => LazyLoader.Load(this, ref _TourplanPos_CompanyAddressUnloadingpoint);
        set => _TourplanPos_CompanyAddressUnloadingpoint = value;
    }

    public bool TourplanPos_CompanyAddressUnloadingpoint_IsLoaded
    {
        get
        {
            return TourplanPos_CompanyAddressUnloadingpoint != null;
        }
    }

    public virtual CollectionEntry TourplanPos_CompanyAddressUnloadingpointReference
    {
        get { return Context.Entry(this).Collection(c => c.TourplanPos_CompanyAddressUnloadingpoint); }
    }
}
