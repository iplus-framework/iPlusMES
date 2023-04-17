using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDFacilityType : VBEntityObject , IInsertInfo, IUpdateInfo
{

    public MDFacilityType()
    {
    }

    private MDFacilityType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    Guid _MDFacilityTypeID;
    public Guid MDFacilityTypeID 
    {
        get { return _MDFacilityTypeID; }
        set { SetProperty<Guid>(ref _MDFacilityTypeID, value); }
    }

    short _MDFacilityTypeIndex;
    public short MDFacilityTypeIndex 
    {
        get { return _MDFacilityTypeIndex; }
        set { SetProperty<short>(ref _MDFacilityTypeIndex, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
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

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
    }

    private ICollection<Facility> _Facility_MDFacilityType;
    public virtual ICollection<Facility> Facility_MDFacilityType
    {
        get => LazyLoader.Load(this, ref _Facility_MDFacilityType);
        set => _Facility_MDFacilityType = value;
    }

    public bool Facility_MDFacilityType_IsLoaded
    {
        get
        {
            return Facility_MDFacilityType != null;
        }
    }

    public virtual CollectionEntry Facility_MDFacilityTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_MDFacilityType); }
    }
}
