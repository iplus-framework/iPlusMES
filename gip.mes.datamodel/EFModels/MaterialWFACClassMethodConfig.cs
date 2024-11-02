// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaterialWFACClassMethodConfig : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MaterialWFACClassMethodConfig()
    {
    }

    private MaterialWFACClassMethodConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaterialWFACClassMethodConfigID;
    public Guid MaterialWFACClassMethodConfigID 
    {
        get { return _MaterialWFACClassMethodConfigID; }
        set { SetProperty<Guid>(ref _MaterialWFACClassMethodConfigID, value); }
    }

    Guid? _MaterialWFACClassMethodID;
    public Guid? MaterialWFACClassMethodID 
    {
        get { return _MaterialWFACClassMethodID; }
        set { SetProperty<Guid?>(ref _MaterialWFACClassMethodID, value); }
    }

    Guid? _VBiACClassID;
    public Guid? VBiACClassID 
    {
        get { return _VBiACClassID; }
        set { SetProperty<Guid?>(ref _VBiACClassID, value); }
    }

    Guid? _VBiACClassPropertyRelationID;
    public Guid? VBiACClassPropertyRelationID 
    {
        get { return _VBiACClassPropertyRelationID; }
        set { SetProperty<Guid?>(ref _VBiACClassPropertyRelationID, value); }
    }

    Guid? _ParentMaterialWFACClassMethodConfigID;
    public Guid? ParentMaterialWFACClassMethodConfigID 
    {
        get { return _ParentMaterialWFACClassMethodConfigID; }
        set { SetProperty<Guid?>(ref _ParentMaterialWFACClassMethodConfigID, value); }
    }

    Guid _VBiValueTypeACClassID;
    public Guid VBiValueTypeACClassID 
    {
        get { return _VBiValueTypeACClassID; }
        set { SetProperty<Guid>(ref _VBiValueTypeACClassID, value); }
    }

    string _KeyACUrl;
    public string KeyACUrl 
    {
        get { return _KeyACUrl; }
        set { SetProperty<string>(ref _KeyACUrl, value); }
    }

    string _PreConfigACUrl;
    public string PreConfigACUrl 
    {
        get { return _PreConfigACUrl; }
        set { SetProperty<string>(ref _PreConfigACUrl, value); }
    }

    string _LocalConfigACUrl;
    public string LocalConfigACUrl 
    {
        get { return _LocalConfigACUrl; }
        set { SetProperty<string>(ref _LocalConfigACUrl, value); }
    }

    string _Expression;
    public string Expression 
    {
        get { return _Expression; }
        set { SetProperty<string>(ref _Expression, value); }
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

    Guid? _VBiACClassWFID;
    public Guid? VBiACClassWFID 
    {
        get { return _VBiACClassWFID; }
        set { SetProperty<Guid?>(ref _VBiACClassWFID, value); }
    }

    private ICollection<MaterialWFACClassMethodConfig> _MaterialWFACClassMethodConfig_ParentMaterialWFACClassMethodConfig;
    public virtual ICollection<MaterialWFACClassMethodConfig> MaterialWFACClassMethodConfig_ParentMaterialWFACClassMethodConfig
    {
        get { return LazyLoader.Load(this, ref _MaterialWFACClassMethodConfig_ParentMaterialWFACClassMethodConfig); }
        set { _MaterialWFACClassMethodConfig_ParentMaterialWFACClassMethodConfig = value; }
    }

    public bool MaterialWFACClassMethodConfig_ParentMaterialWFACClassMethodConfig_IsLoaded
    {
        get
        {
            return MaterialWFACClassMethodConfig_ParentMaterialWFACClassMethodConfig != null;
        }
    }

    public virtual CollectionEntry MaterialWFACClassMethodConfig_ParentMaterialWFACClassMethodConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.MaterialWFACClassMethodConfig_ParentMaterialWFACClassMethodConfig); }
    }

    private MaterialWFACClassMethod _MaterialWFACClassMethod;
    public virtual MaterialWFACClassMethod MaterialWFACClassMethod
    { 
        get { return LazyLoader.Load(this, ref _MaterialWFACClassMethod); } 
        set { SetProperty<MaterialWFACClassMethod>(ref _MaterialWFACClassMethod, value); }
    }

    public bool MaterialWFACClassMethod_IsLoaded
    {
        get
        {
            return MaterialWFACClassMethod != null;
        }
    }

    public virtual ReferenceEntry MaterialWFACClassMethodReference 
    {
        get { return Context.Entry(this).Reference("MaterialWFACClassMethod"); }
    }
    
    private MaterialWFACClassMethodConfig _MaterialWFACClassMethodConfig1_ParentMaterialWFACClassMethodConfig;
    public virtual MaterialWFACClassMethodConfig MaterialWFACClassMethodConfig1_ParentMaterialWFACClassMethodConfig
    { 
        get { return LazyLoader.Load(this, ref _MaterialWFACClassMethodConfig1_ParentMaterialWFACClassMethodConfig); } 
        set { SetProperty<MaterialWFACClassMethodConfig>(ref _MaterialWFACClassMethodConfig1_ParentMaterialWFACClassMethodConfig, value); }
    }

    public bool MaterialWFACClassMethodConfig1_ParentMaterialWFACClassMethodConfig_IsLoaded
    {
        get
        {
            return MaterialWFACClassMethodConfig1_ParentMaterialWFACClassMethodConfig != null;
        }
    }

    public virtual ReferenceEntry MaterialWFACClassMethodConfig1_ParentMaterialWFACClassMethodConfigReference 
    {
        get { return Context.Entry(this).Reference("MaterialWFACClassMethodConfig1_ParentMaterialWFACClassMethodConfig"); }
    }
    
    private ACClass _VBiACClass;
    public virtual ACClass VBiACClass
    { 
        get { return LazyLoader.Load(this, ref _VBiACClass); } 
        set { SetProperty<ACClass>(ref _VBiACClass, value); }
    }

    public bool VBiACClass_IsLoaded
    {
        get
        {
            return VBiACClass != null;
        }
    }

    public virtual ReferenceEntry VBiACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiACClass"); }
    }
    
    private ACClassPropertyRelation _VBiACClassPropertyRelation;
    public virtual ACClassPropertyRelation VBiACClassPropertyRelation
    { 
        get { return LazyLoader.Load(this, ref _VBiACClassPropertyRelation); } 
        set { SetProperty<ACClassPropertyRelation>(ref _VBiACClassPropertyRelation, value); }
    }

    public bool VBiACClassPropertyRelation_IsLoaded
    {
        get
        {
            return VBiACClassPropertyRelation != null;
        }
    }

    public virtual ReferenceEntry VBiACClassPropertyRelationReference 
    {
        get { return Context.Entry(this).Reference("VBiACClassPropertyRelation"); }
    }
    
    private ACClassWF _VBiACClassWF;
    public virtual ACClassWF VBiACClassWF
    { 
        get { return LazyLoader.Load(this, ref _VBiACClassWF); } 
        set { SetProperty<ACClassWF>(ref _VBiACClassWF, value); }
    }

    public bool VBiACClassWF_IsLoaded
    {
        get
        {
            return VBiACClassWF != null;
        }
    }

    public virtual ReferenceEntry VBiACClassWFReference 
    {
        get { return Context.Entry(this).Reference("VBiACClassWF"); }
    }
    
    private ACClass _VBiValueTypeACClass;
    public virtual ACClass VBiValueTypeACClass
    { 
        get { return LazyLoader.Load(this, ref _VBiValueTypeACClass); } 
        set { SetProperty<ACClass>(ref _VBiValueTypeACClass, value); }
    }

    public bool VBiValueTypeACClass_IsLoaded
    {
        get
        {
            return VBiValueTypeACClass != null;
        }
    }

    public virtual ReferenceEntry VBiValueTypeACClassReference 
    {
        get { return Context.Entry(this).Reference("VBiValueTypeACClass"); }
    }
    }
