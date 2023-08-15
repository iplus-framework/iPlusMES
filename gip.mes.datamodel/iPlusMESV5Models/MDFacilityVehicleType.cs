using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDFacilityVehicleType : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDFacilityVehicleType()
    {
    }

    private MDFacilityVehicleType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDFacilityVehicleTypeID;
    public Guid MDFacilityVehicleTypeID 
    {
        get { return _MDFacilityVehicleTypeID; }
        set { SetProperty<Guid>(ref _MDFacilityVehicleTypeID, value); }
    }

    short _MDFacilityVehicleTypeIndex;
    public short MDFacilityVehicleTypeIndex 
    {
        get { return _MDFacilityVehicleTypeIndex; }
        set { SetProperty<short>(ref _MDFacilityVehicleTypeIndex, value); }
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

    private ICollection<Facility> _Facility_MDFacilityVehicleType;
    public virtual ICollection<Facility> Facility_MDFacilityVehicleType
    {
        get => LazyLoader.Load(this, ref _Facility_MDFacilityVehicleType);
        set => _Facility_MDFacilityVehicleType = value;
    }

    public bool Facility_MDFacilityVehicleType_IsLoaded
    {
        get
        {
            return Facility_MDFacilityVehicleType != null;
        }
    }

    public virtual CollectionEntry Facility_MDFacilityVehicleTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.Facility_MDFacilityVehicleType); }
    }
}
