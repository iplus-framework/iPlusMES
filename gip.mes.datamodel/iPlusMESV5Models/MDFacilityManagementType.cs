using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDFacilityManagementType : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDFacilityManagementType()
    {
    }

    private MDFacilityManagementType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDFacilityManagementTypeID;
    public Guid MDFacilityManagementTypeID 
    {
        get { return _MDFacilityManagementTypeID; }
        set { SetProperty<Guid>(ref _MDFacilityManagementTypeID, value); }
    }

    short _MDFacilityManagementTypeIndex;
    public short MDFacilityManagementTypeIndex 
    {
        get { return _MDFacilityManagementTypeIndex; }
        set { SetProperty<short>(ref _MDFacilityManagementTypeIndex, value); }
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

    bool _IsDefault;
    public bool IsDefault 
    {
        get { return _IsDefault; }
        set { SetProperty<bool>(ref _IsDefault, value); }
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

    private ObservableHashSet<Material> _Material_MDFacilityManagementType;
    public virtual ObservableHashSet<Material> Material_MDFacilityManagementType
    {
        get => LazyLoader.Load(this, ref _Material_MDFacilityManagementType);
        set => _Material_MDFacilityManagementType = value;
    }

    public bool Material_MDFacilityManagementType_IsLoaded
    {
        get
        {
            return Material_MDFacilityManagementType != null;
        }
    }

    public virtual CollectionEntry Material_MDFacilityManagementTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.Material_MDFacilityManagementType); }
    }
}
