﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ACPropertyLog : VBEntityObject
{

    public ACPropertyLog()
    {
    }

    private ACPropertyLog(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACPropertyLogID;
    public Guid ACPropertyLogID 
    {
        get { return _ACPropertyLogID; }
        set { SetProperty<Guid>(ref _ACPropertyLogID, value); }
    }

    Guid _ACClassID;
    public Guid ACClassID 
    {
        get { return _ACClassID; }
        set { SetProperty<Guid>(ref _ACClassID, value); }
    }

    Guid _ACClassPropertyID;
    public Guid ACClassPropertyID 
    {
        get { return _ACClassPropertyID; }
        set { SetProperty<Guid>(ref _ACClassPropertyID, value); }
    }

    Guid? _ACProgramLogID;
    public Guid? ACProgramLogID 
    {
        get { return _ACProgramLogID; }
        set { SetProperty<Guid?>(ref _ACProgramLogID, value); }
    }

    DateTime _EventTime;
    public DateTime EventTime 
    {
        get { return _EventTime; }
        set { SetProperty<DateTime>(ref _EventTime, value); }
    }

    string _Value;
    public string Value 
    {
        get { return _Value; }
        set { SetProperty<string>(ref _Value, value); }
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
    
    private ACClassProperty _ACClassProperty;
    public virtual ACClassProperty ACClassProperty
    { 
        get { return LazyLoader.Load(this, ref _ACClassProperty); } 
        set { SetProperty<ACClassProperty>(ref _ACClassProperty, value); }
    }

    public bool ACClassProperty_IsLoaded
    {
        get
        {
            return ACClassProperty != null;
        }
    }

    public virtual ReferenceEntry ACClassPropertyReference 
    {
        get { return Context.Entry(this).Reference("ACClassProperty"); }
    }
    }