// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACClassConfig : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public ACClassConfig()
    {
    }

    private ACClassConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACClassConfigID;
    public Guid ACClassConfigID 
    {
        get { return _ACClassConfigID; }
        set { SetProperty<Guid>(ref _ACClassConfigID, value); }
    }

    Guid _ACClassID;
    public Guid ACClassID 
    {
        get { return _ACClassID; }
        set { SetProperty<Guid>(ref _ACClassID, value); }
    }

    Guid? _ACClassPropertyRelationID;
    public Guid? ACClassPropertyRelationID 
    {
        get { return _ACClassPropertyRelationID; }
        set { SetProperty<Guid?>(ref _ACClassPropertyRelationID, value); }
    }

    Guid? _ParentACClassConfigID;
    public Guid? ParentACClassConfigID 
    {
        get { return _ParentACClassConfigID; }
        set { SetProperty<Guid?>(ref _ParentACClassConfigID, value); }
    }

    Guid _ValueTypeACClassID;
    public Guid ValueTypeACClassID 
    {
        get { return _ValueTypeACClassID; }
        set { SetProperty<Guid>(ref _ValueTypeACClassID, value); }
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

    int _BranchNo;
    public int BranchNo 
    {
        get { return _BranchNo; }
        set { SetProperty<int>(ref _BranchNo, value); }
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

    private ACClass _ACClass;
    public virtual ACClass ACClass
    { 
        get { return LazyLoader.Load(this, ref _ACClass); } 
        set { SetProperty<ACClass>(ref _ACClass, value); }
    }

    public bool ACClass_IsLoaded
    {
        get
        {
            return ACClass != null;
        }
    }

    public virtual ReferenceEntry ACClassReference 
    {
        get { return Context.Entry(this).Reference("ACClass"); }
    }
    
    private ACClassPropertyRelation _ACClassPropertyRelation;
    public virtual ACClassPropertyRelation ACClassPropertyRelation
    { 
        get { return LazyLoader.Load(this, ref _ACClassPropertyRelation); } 
        set { SetProperty<ACClassPropertyRelation>(ref _ACClassPropertyRelation, value); }
    }

    public bool ACClassPropertyRelation_IsLoaded
    {
        get
        {
            return ACClassPropertyRelation != null;
        }
    }

    public virtual ReferenceEntry ACClassPropertyRelationReference 
    {
        get { return Context.Entry(this).Reference("ACClassPropertyRelation"); }
    }
    
    private ICollection<ACClassConfig> _ACClassConfig_ParentACClassConfig;
    public virtual ICollection<ACClassConfig> ACClassConfig_ParentACClassConfig
    {
        get { return LazyLoader.Load(this, ref _ACClassConfig_ParentACClassConfig); }
        set { _ACClassConfig_ParentACClassConfig = value; }
    }

    public bool ACClassConfig_ParentACClassConfig_IsLoaded
    {
        get
        {
            return ACClassConfig_ParentACClassConfig != null;
        }
    }

    public virtual CollectionEntry ACClassConfig_ParentACClassConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.ACClassConfig_ParentACClassConfig); }
    }

    private ACClassConfig _ACClassConfig1_ParentACClassConfig;
    public virtual ACClassConfig ACClassConfig1_ParentACClassConfig
    { 
        get { return LazyLoader.Load(this, ref _ACClassConfig1_ParentACClassConfig); } 
        set { SetProperty<ACClassConfig>(ref _ACClassConfig1_ParentACClassConfig, value); }
    }

    public bool ACClassConfig1_ParentACClassConfig_IsLoaded
    {
        get
        {
            return ACClassConfig1_ParentACClassConfig != null;
        }
    }

    public virtual ReferenceEntry ACClassConfig1_ParentACClassConfigReference 
    {
        get { return Context.Entry(this).Reference("ACClassConfig1_ParentACClassConfig"); }
    }
    
    private ACClass _ValueTypeACClass;
    public virtual ACClass ValueTypeACClass
    { 
        get { return LazyLoader.Load(this, ref _ValueTypeACClass); } 
        set { SetProperty<ACClass>(ref _ValueTypeACClass, value); }
    }

    public bool ValueTypeACClass_IsLoaded
    {
        get
        {
            return ValueTypeACClass != null;
        }
    }

    public virtual ReferenceEntry ValueTypeACClassReference 
    {
        get { return Context.Entry(this).Reference("ValueTypeACClass"); }
    }
    }
