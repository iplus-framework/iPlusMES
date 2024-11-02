// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using gip.core.datamodel;

namespace gip.mes.datamodel;

public partial class ProdOrderInwardsView : VBEntityObject
{

    public ProdOrderInwardsView()
    {
    }

    private ProdOrderInwardsView(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    private ILazyLoader LazyLoader { get; set; }
    
    string _ProgramNo;
    public string ProgramNo 
    {
        get { return _ProgramNo; }
        set { SetProperty<string>(ref _ProgramNo, value); }
    }

    int _PartslistSequence;
    public int PartslistSequence 
    {
        get { return _PartslistSequence; }
        set { SetProperty<int>(ref _PartslistSequence, value); }
    }

    int _IntermediateSequence;
    public int IntermediateSequence 
    {
        get { return _IntermediateSequence; }
        set { SetProperty<int>(ref _IntermediateSequence, value); }
    }

    string _IntermediateMaterial;
    public string IntermediateMaterial 
    {
        get { return _IntermediateMaterial; }
        set { SetProperty<string>(ref _IntermediateMaterial, value); }
    }

    int _BatchSequence;
    public int BatchSequence 
    {
        get { return _BatchSequence; }
        set { SetProperty<int>(ref _BatchSequence, value); }
    }

    string _ProdOrderBatchNo;
    public string ProdOrderBatchNo 
    {
        get { return _ProdOrderBatchNo; }
        set { SetProperty<string>(ref _ProdOrderBatchNo, value); }
    }

    string _FacilityBookingChargeNo;
    public string FacilityBookingChargeNo 
    {
        get { return _FacilityBookingChargeNo; }
        set { SetProperty<string>(ref _FacilityBookingChargeNo, value); }
    }

    string _LotNo;
    public string LotNo 
    {
        get { return _LotNo; }
        set { SetProperty<string>(ref _LotNo, value); }
    }
}
