// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class PriceListMaterial : VBEntityObject
{

    public PriceListMaterial()
    {
    }

    private PriceListMaterial(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _PriceListMaterialID;
    public Guid PriceListMaterialID 
    {
        get { return _PriceListMaterialID; }
        set { SetProperty<Guid>(ref _PriceListMaterialID, value); }
    }

    Guid _PriceListID;
    public Guid PriceListID 
    {
        get { return _PriceListID; }
        set { SetProperty<Guid>(ref _PriceListID, value); }
    }

    Guid _MaterialID;
    public Guid MaterialID 
    {
        get { return _MaterialID; }
        set { SetProperty<Guid>(ref _MaterialID, value); }
    }

    decimal _Price;
    public decimal Price 
    {
        get { return _Price; }
        set { SetProperty<decimal>(ref _Price, value); }
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
    
    private PriceList _PriceList;
    public virtual PriceList PriceList
    { 
        get { return LazyLoader.Load(this, ref _PriceList); } 
        set { SetProperty<PriceList>(ref _PriceList, value); }
    }

    public bool PriceList_IsLoaded
    {
        get
        {
            return PriceList != null;
        }
    }

    public virtual ReferenceEntry PriceListReference 
    {
        get { return Context.Entry(this).Reference("PriceList"); }
    }
    }
