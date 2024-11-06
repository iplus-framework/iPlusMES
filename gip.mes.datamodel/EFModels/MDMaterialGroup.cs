using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDMaterialGroup : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDMaterialGroup()
    {
    }

    private MDMaterialGroup(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDMaterialGroupID;
    public Guid MDMaterialGroupID 
    {
        get { return _MDMaterialGroupID; }
        set { SetProperty<Guid>(ref _MDMaterialGroupID, value); }
    }

    short _MDMaterialGroupIndex;
    public short MDMaterialGroupIndex 
    {
        get { return _MDMaterialGroupIndex; }
        set { SetProperty<short>(ref _MDMaterialGroupIndex, value); }
    }

    string _MDNameTrans;
    public string MDNameTrans 
    {
        get { return _MDNameTrans; }
        set { SetProperty<string>(ref _MDNameTrans, value); }
    }

    bool _IsNotPercantage;
    public bool IsNotPercantage 
    {
        get { return _IsNotPercantage; }
        set { SetProperty<bool>(ref _IsNotPercantage, value); }
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

    private ICollection<MDCountrySalesTaxMDMaterialGroup> _MDCountrySalesTaxMDMaterialGroup_MDMaterialGroup;
    public virtual ICollection<MDCountrySalesTaxMDMaterialGroup> MDCountrySalesTaxMDMaterialGroup_MDMaterialGroup
    {
        get { return LazyLoader.Load(this, ref _MDCountrySalesTaxMDMaterialGroup_MDMaterialGroup); }
        set { _MDCountrySalesTaxMDMaterialGroup_MDMaterialGroup = value; }
    }

    public bool MDCountrySalesTaxMDMaterialGroup_MDMaterialGroup_IsLoaded
    {
        get
        {
            return MDCountrySalesTaxMDMaterialGroup_MDMaterialGroup != null;
        }
    }

    public virtual CollectionEntry MDCountrySalesTaxMDMaterialGroup_MDMaterialGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.MDCountrySalesTaxMDMaterialGroup_MDMaterialGroup); }
    }

    private ICollection<Material> _Material_MDMaterialGroup;
    public virtual ICollection<Material> Material_MDMaterialGroup
    {
        get { return LazyLoader.Load(this, ref _Material_MDMaterialGroup); }
        set { _Material_MDMaterialGroup = value; }
    }

    public bool Material_MDMaterialGroup_IsLoaded
    {
        get
        {
            return Material_MDMaterialGroup != null;
        }
    }

    public virtual CollectionEntry Material_MDMaterialGroupReference
    {
        get { return Context.Entry(this).Collection(c => c.Material_MDMaterialGroup); }
    }
}
