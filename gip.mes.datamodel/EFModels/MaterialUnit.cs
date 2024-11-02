// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class MaterialUnit : VBEntityObject, IInsertInfo, IUpdateInfo
{

    public MaterialUnit()
    {
    }

    private MaterialUnit(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _MaterialUnitID;
    public Guid MaterialUnitID 
    {
        get { return _MaterialUnitID; }
        set { SetProperty<Guid>(ref _MaterialUnitID, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    Guid _ToMDUnitID;
    public Guid ToMDUnitID 
    {
        get { return _ToMDUnitID; }
        set { SetProperty<Guid>(ref _ToMDUnitID, value); }
    }

    double _Multiplier;
    public double Multiplier 
    {
        get { return _Multiplier; }
        set { SetProperty<double>(ref _Multiplier, value); }
    }

    double _Divisor;
    public double Divisor 
    {
        get { return _Divisor; }
        set { SetProperty<double>(ref _Divisor, value); }
    }

    double _NetWeight;
    public double NetWeight 
    {
        get { return _NetWeight; }
        set { SetProperty<double>(ref _NetWeight, value); }
    }

    double _GrossWeight;
    public double GrossWeight 
    {
        get { return _GrossWeight; }
        set { SetProperty<double>(ref _GrossWeight, value); }
    }

    double _ProductionWeight;
    public double ProductionWeight 
    {
        get { return _ProductionWeight; }
        set { SetProperty<double>(ref _ProductionWeight, value); }
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
    
    private MDUnit _ToMDUnit;
    public virtual MDUnit ToMDUnit
    { 
        get { return LazyLoader.Load(this, ref _ToMDUnit); } 
        set { SetProperty<MDUnit>(ref _ToMDUnit, value); }
    }

    public bool ToMDUnit_IsLoaded
    {
        get
        {
            return ToMDUnit != null;
        }
    }

    public virtual ReferenceEntry ToMDUnitReference 
    {
        get { return Context.Entry(this).Reference("ToMDUnit"); }
    }
    }
