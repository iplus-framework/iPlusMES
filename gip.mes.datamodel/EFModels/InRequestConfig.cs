// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class InRequestConfig : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public InRequestConfig()
    {
    }

    private InRequestConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _InRequestConfigID;
    public Guid InRequestConfigID 
    {
        get { return _InRequestConfigID; }
        set { SetProperty<Guid>(ref _InRequestConfigID, value); }
    }

    Guid _InRequestID;
    public Guid InRequestID 
    {
        get { return _InRequestID; }
        set { SetProperty<Guid>(ref _InRequestID, value); }
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

    Guid? _MaterialID;
    public Guid? MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid?>(ref _MaterialID, value); }
    }

    Guid? _ParentInRequestConfigID;
    public Guid? ParentInRequestConfigID 
    {
        get { return _ParentInRequestConfigID; }
        set { SetProperty<Guid?>(ref _ParentInRequestConfigID, value); }
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

    private InRequest _InRequest;
    public virtual InRequest InRequest
    { 
        get { return LazyLoader.Load(this, ref _InRequest); } 
        set { SetProperty<InRequest>(ref _InRequest, value); }
    }

    public bool InRequest_IsLoaded
    {
        get
        {
            return InRequest != null;
        }
    }

    public virtual ReferenceEntry InRequestReference 
    {
        get { return Context.Entry(this).Reference("InRequest"); }
    }
    
    private ICollection<InRequestConfig> _InRequestConfig_ParentInRequestConfig;
    public virtual ICollection<InRequestConfig> InRequestConfig_ParentInRequestConfig
    {
        get { return LazyLoader.Load(this, ref _InRequestConfig_ParentInRequestConfig); }
        set { _InRequestConfig_ParentInRequestConfig = value; }
    }

    public bool InRequestConfig_ParentInRequestConfig_IsLoaded
    {
        get
        {
            return InRequestConfig_ParentInRequestConfig != null;
        }
    }

    public virtual CollectionEntry InRequestConfig_ParentInRequestConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.InRequestConfig_ParentInRequestConfig); }
    }

    private Material _Material;
    public virtual Material Material
    { 
        get { return LazyLoader.Load(this, ref _Material); } 
        set { SetProperty<Material>(ref _Material, value); }
    }

    public bool Material_IsLoaded
    {
        get
        {
            return Material != null;
        }
    }

    public virtual ReferenceEntry MaterialReference 
    {
        get { return Context.Entry(this).Reference("Material"); }
    }
    
    private InRequestConfig _InRequestConfig1_ParentInRequestConfig;
    public virtual InRequestConfig InRequestConfig1_ParentInRequestConfig
    { 
        get { return LazyLoader.Load(this, ref _InRequestConfig1_ParentInRequestConfig); } 
        set { SetProperty<InRequestConfig>(ref _InRequestConfig1_ParentInRequestConfig, value); }
    }

    public bool InRequestConfig1_ParentInRequestConfig_IsLoaded
    {
        get
        {
            return InRequestConfig1_ParentInRequestConfig != null;
        }
    }

    public virtual ReferenceEntry InRequestConfig1_ParentInRequestConfigReference 
    {
        get { return Context.Entry(this).Reference("InRequestConfig1_ParentInRequestConfig"); }
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
