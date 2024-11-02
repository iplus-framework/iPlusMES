// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class OutOfferConfig : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public OutOfferConfig()
    {
    }

    private OutOfferConfig(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _OutOfferConfigID;
    public Guid OutOfferConfigID 
    {
        get { return _OutOfferConfigID; }
        set { SetProperty<Guid>(ref _OutOfferConfigID, value); }
    }

    Guid _OutOfferID;
    public Guid OutOfferID 
    {
        get { return _OutOfferID; }
        set { SetProperty<Guid>(ref _OutOfferID, value); }
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

    Guid? _ParentOutOfferConfigID;
    public Guid? ParentOutOfferConfigID 
    {
        get { return _ParentOutOfferConfigID; }
        set { SetProperty<Guid?>(ref _ParentOutOfferConfigID, value); }
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

    private ICollection<OutOfferConfig> _OutOfferConfig_ParentOutOfferConfig;
    public virtual ICollection<OutOfferConfig> OutOfferConfig_ParentOutOfferConfig
    {
        get { return LazyLoader.Load(this, ref _OutOfferConfig_ParentOutOfferConfig); }
        set { _OutOfferConfig_ParentOutOfferConfig = value; }
    }

    public bool OutOfferConfig_ParentOutOfferConfig_IsLoaded
    {
        get
        {
            return OutOfferConfig_ParentOutOfferConfig != null;
        }
    }

    public virtual CollectionEntry OutOfferConfig_ParentOutOfferConfigReference
    {
        get { return Context.Entry(this).Collection(c => c.OutOfferConfig_ParentOutOfferConfig); }
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
    
    private OutOffer _OutOffer;
    public virtual OutOffer OutOffer
    { 
        get { return LazyLoader.Load(this, ref _OutOffer); } 
        set { SetProperty<OutOffer>(ref _OutOffer, value); }
    }

    public bool OutOffer_IsLoaded
    {
        get
        {
            return OutOffer != null;
        }
    }

    public virtual ReferenceEntry OutOfferReference 
    {
        get { return Context.Entry(this).Reference("OutOffer"); }
    }
    
    private OutOfferConfig _OutOfferConfig1_ParentOutOfferConfig;
    public virtual OutOfferConfig OutOfferConfig1_ParentOutOfferConfig
    { 
        get { return LazyLoader.Load(this, ref _OutOfferConfig1_ParentOutOfferConfig); } 
        set { SetProperty<OutOfferConfig>(ref _OutOfferConfig1_ParentOutOfferConfig, value); }
    }

    public bool OutOfferConfig1_ParentOutOfferConfig_IsLoaded
    {
        get
        {
            return OutOfferConfig1_ParentOutOfferConfig != null;
        }
    }

    public virtual ReferenceEntry OutOfferConfig1_ParentOutOfferConfigReference 
    {
        get { return Context.Entry(this).Reference("OutOfferConfig1_ParentOutOfferConfig"); }
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
