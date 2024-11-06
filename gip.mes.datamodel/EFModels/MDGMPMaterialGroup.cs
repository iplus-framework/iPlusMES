using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDGMPMaterialGroup : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDGMPMaterialGroup()
    {
    }

    private MDGMPMaterialGroup(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDGMPMaterialGroupID;
    public Guid MDGMPMaterialGroupID 
    {
        get { return _MDGMPMaterialGroupID; }
        set { SetProperty<Guid>(ref _MDGMPMaterialGroupID, value); }
    }

    string _MDGMPMaterialGroupNo;
    public string MDGMPMaterialGroupNo 
    {
        get { return _MDGMPMaterialGroupNo; }
        set { SetProperty<string>(ref _MDGMPMaterialGroupNo, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    string _WearFacilityNo;
    public string WearFacilityNo 
    {
        get { return _WearFacilityNo; }
        set { SetProperty<string>(ref _WearFacilityNo, value); }
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

    private ICollection<MDGMPMaterialGroupPos> _MDGMPMaterialGroupPos_MDGMPMaterialGroup;
    public virtual ICollection<MDGMPMaterialGroupPos> MDGMPMaterialGroupPos_MDGMPMaterialGroup
    {
        get { return LazyLoader.Load(this, ref _MDGMPMaterialGroupPos_MDGMPMaterialGroup); }
        set { _MDGMPMaterialGroupPos_MDGMPMaterialGroup = value; }
    }

    public bool MDGMPMaterialGroupPos_MDGMPMaterialGroup_IsLoaded
    {
        get
        {
            return MDGMPMaterialGroupPos_MDGMPMaterialGroup != null;
        }
    }

    public virtual CollectionEntry MDGMPMaterialGroupPos_MDGMPMaterialGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.MDGMPMaterialGroupPos_MDGMPMaterialGroup); }
    }

    private ICollection<Material> _Material_MDGMPMaterialGroup;
    public virtual ICollection<Material> Material_MDGMPMaterialGroup
    {
        get { return LazyLoader.Load(this, ref _Material_MDGMPMaterialGroup); }
        set { _Material_MDGMPMaterialGroup = value; }
    }

    public bool Material_MDGMPMaterialGroup_IsLoaded
    {
        get
        {
            return Material_MDGMPMaterialGroup != null;
        }
    }

    public virtual CollectionEntry Material_MDGMPMaterialGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.Material_MDGMPMaterialGroup); }
    }
}
