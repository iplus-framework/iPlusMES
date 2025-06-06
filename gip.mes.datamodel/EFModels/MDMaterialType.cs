﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDMaterialType : VBEntityObject, IInsertInfo, IUpdateInfo, IMDTrans
{

    public MDMaterialType()
    {
    }

    private MDMaterialType(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDMaterialTypeID;
    public Guid MDMaterialTypeID 
    {
        get { return _MDMaterialTypeID; }
        set { SetProperty<Guid>(ref _MDMaterialTypeID, value); }
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

    short _MDMaterialTypeIndex;
    public short MDMaterialTypeIndex 
    {
        get { return _MDMaterialTypeIndex; }
        set { SetProperty<short>(ref _MDMaterialTypeIndex, value); }
    }

    private ICollection<Material> _Material_MDMaterialType;
    public virtual ICollection<Material> Material_MDMaterialType
    {
        get { return LazyLoader.Load(this, ref _Material_MDMaterialType); }
        set { _Material_MDMaterialType = value; }
    }

    public bool Material_MDMaterialType_IsLoaded
    {
        get
        {
            return _Material_MDMaterialType != null;
        }
    }

    public virtual CollectionEntry Material_MDMaterialTypeReference
    {
        get { return Context.Entry(this).Collection(c => c.Material_MDMaterialType); }
    }
}
