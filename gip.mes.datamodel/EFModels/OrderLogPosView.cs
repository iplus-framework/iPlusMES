// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class OrderLogPosView : VBEntityObject
{

    public OrderLogPosView()
    {
    }

    private OrderLogPosView(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    Guid _ACProgramLogID;
    public Guid ACProgramLogID 
    {
        get { return _ACProgramLogID; }
        set { SetProperty<Guid>(ref _ACProgramLogID, value); }
    }

    string _ProgramNo;
    public string ProgramNo 
    {
        get { return _ProgramNo; }
        set { SetProperty<string>(ref _ProgramNo, value); }
    }

    string _ACUrl;
    public string ACUrl 
    {
        get { return _ACUrl; }
        set { SetProperty<string>(ref _ACUrl, value); }
    }

    Guid? _ProdOrderPartslistPosID;
    public Guid? ProdOrderPartslistPosID 
    {
        get { return _ProdOrderPartslistPosID; }
        set { SetProperty<Guid?>(ref _ProdOrderPartslistPosID, value); }
    }

    int _PosSequence;
    public int PosSequence 
    {
        get { return _PosSequence; }
        set { SetProperty<int>(ref _PosSequence, value); }
    }

    string _PosBatchNo;
    public string PosBatchNo 
    {
        get { return _PosBatchNo; }
        set { SetProperty<string>(ref _PosBatchNo, value); }
    }

    short _MaterialPosTypeIndex;
    public short MaterialPosTypeIndex 
    {
        get { return _MaterialPosTypeIndex; }
        set { SetProperty<short>(ref _MaterialPosTypeIndex, value); }
    }

    string _PosMaterialNo;
    public string PosMaterialNo 
    {
        get { return _PosMaterialNo; }
        set { SetProperty<string>(ref _PosMaterialNo, value); }
    }

    string _PosMaterialName;
    public string PosMaterialName 
    {
        get { return _PosMaterialName; }
        set { SetProperty<string>(ref _PosMaterialName, value); }
    }

    double _PosTargetQuantityUOM;
    public double PosTargetQuantityUOM 
    {
        get { return _PosTargetQuantityUOM; }
        set { SetProperty<double>(ref _PosTargetQuantityUOM, value); }
    }

    double _PosActualQuantityUOM;
    public double PosActualQuantityUOM 
    {
        get { return _PosActualQuantityUOM; }
        set { SetProperty<double>(ref _PosActualQuantityUOM, value); }
    }

    int _PartslistSequence;
    public int PartslistSequence 
    {
        get { return _PartslistSequence; }
        set { SetProperty<int>(ref _PartslistSequence, value); }
    }

    string _ProdOrderProgramNo;
    public string ProdOrderProgramNo 
    {
        get { return _ProdOrderProgramNo; }
        set { SetProperty<string>(ref _ProdOrderProgramNo, value); }
    }
}
