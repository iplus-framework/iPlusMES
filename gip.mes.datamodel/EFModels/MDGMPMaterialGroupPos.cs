// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MDGMPMaterialGroupPos : VBEntityObject, IInsertInfo, IUpdateInfo, ISequence
{

    public MDGMPMaterialGroupPos()
    {
    }

    private MDGMPMaterialGroupPos(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MDGMPMaterialGroupPosID;
    public Guid MDGMPMaterialGroupPosID 
    {
        get { return _MDGMPMaterialGroupPosID; }
        set { SetProperty<Guid>(ref _MDGMPMaterialGroupPosID, value); }
    }

    Guid _MDGMPMaterialGroupID;
    public Guid MDGMPMaterialGroupID 
    {
        get { return _MDGMPMaterialGroupID; }
        set { SetProperty<Guid>(ref _MDGMPMaterialGroupID, value); }
    }

    Guid _MDGMPAdditiveID;
    public Guid MDGMPAdditiveID 
    {
        get { return _MDGMPAdditiveID; }
        set { SetProperty<Guid>(ref _MDGMPAdditiveID, value); }
    }

    int _Sequence;
    public int Sequence 
    {
        get { return _Sequence; }
        set { SetProperty<int>(ref _Sequence, value); }
    }

    float _MaxConcentration;
    public float MaxConcentration 
    {
        get { return _MaxConcentration; }
        set { SetProperty<float>(ref _MaxConcentration, value); }
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

    private MDGMPAdditive _MDGMPAdditive;
    public virtual MDGMPAdditive MDGMPAdditive
    { 
        get { return LazyLoader.Load(this, ref _MDGMPAdditive); } 
        set { SetProperty<MDGMPAdditive>(ref _MDGMPAdditive, value); }
    }

    public bool MDGMPAdditive_IsLoaded
    {
        get
        {
            return MDGMPAdditive != null;
        }
    }

    public virtual ReferenceEntry MDGMPAdditiveReference 
    {
        get { return Context.Entry(this).Reference("MDGMPAdditive"); }
    }
    
    private MDGMPMaterialGroup _MDGMPMaterialGroup;
    public virtual MDGMPMaterialGroup MDGMPMaterialGroup
    { 
        get { return LazyLoader.Load(this, ref _MDGMPMaterialGroup); } 
        set { SetProperty<MDGMPMaterialGroup>(ref _MDGMPMaterialGroup, value); }
    }

    public bool MDGMPMaterialGroup_IsLoaded
    {
        get
        {
            return MDGMPMaterialGroup != null;
        }
    }

    public virtual ReferenceEntry MDGMPMaterialGroupReference 
    {
        get { return Context.Entry(this).Reference("MDGMPMaterialGroup"); }
    }
    }
